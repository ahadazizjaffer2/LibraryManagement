using Dapper;
using librarymanagement.Dtos;
using librarymanagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace librarymanagement.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly string _connectionString;

        public BooksController(IConfiguration configuration)
        {
            _connectionString = _configuration["DATABASE_CONNECTION_STRING"];
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var books = await connection.QueryAsync<Book>("SELECT * FROM books ORDER BY title");
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var book = await connection.QueryFirstOrDefaultAsync<Book>(
                "SELECT * FROM books WHERE id = @Id", new { Id = id });

            if (book == null)
                return NotFound("Book not found.");

            return Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> AddBook([FromBody] CreateBookDto bookDto)
        {
            var newBook = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                YearPublished = bookDto.YearPublished,
                CopiesAvailable = bookDto.CopiesAvailable,
                Genre = bookDto.Genre
            };

            using var connection = new NpgsqlConnection(_connectionString);
            var query = "INSERT INTO books (title, author, yearpublished, CopiesAvailable, Genre) VALUES (@Title, @Author, @YearPublished, @CopiesAvailable, @Genre)";
            await connection.ExecuteAsync(query, newBook);

            return Ok("Book added successfully.");
        }

        [HttpPut]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> UpdateBook([FromBody] UpdateBookDto bookDto)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var existingBook = await connection.QueryFirstOrDefaultAsync<Book>(
                "SELECT * FROM books WHERE id = @Id", new { Id = bookDto.Id });

            if (existingBook == null)
                return NotFound("Book not found.");

            var query = "UPDATE books SET title = @Title, author = @Author, yearpublished = @YearPublished, copiesavailable = @CopiesAvailable, Genre = @Genre WHERE id = @Id";
            await connection.ExecuteAsync(query, new
            {
                bookDto.Title,
                bookDto.Author,
                bookDto.YearPublished,
                bookDto.CopiesAvailable,
                bookDto.Genre,
                Id = bookDto.Id
            });

            return Ok("Book updated successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var result = await connection.ExecuteAsync("DELETE FROM books WHERE id = @Id", new { Id = id });

            if (result == 0)
                return NotFound("Book not found.");

            return Ok("Book deleted successfully.");
        }
    }
}
