using Dapper;
using librarymanagement.Dtos;
using librarymanagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace librarymanagement.Controllers
{
    [ApiController]
    [Route("api/borrowed-books")]
    public class BorrowedBooksController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<BorrowedBooksController> _logger;

        public BorrowedBooksController(IConfiguration configuration, ILogger<BorrowedBooksController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // [HttpGet("user/{userId}")]
        // [Authorize(Roles = "Customer, Librarian")]
        // public async Task<IActionResult> GetBorrowedBooksByUserId(int userId)
        // {
        //     _logger.LogInformation("Getting borrowed books for user with ID {UserId}", userId);
        //     using var connection = new NpgsqlConnection(_connectionString);
        //     var borrowedBooks = await connection.QueryAsync<BorrowedBook>(
        //         "SELECT * FROM borrowedbooks WHERE userid = @UserId", new { UserId = userId });

        //     return Ok(borrowedBooks);
        // }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Customer, Librarian")]
        public async Task<IActionResult> GetBorrowedBooksByUserId(int userId)
        {
            _logger.LogInformation("Getting borrowed books for user with ID {UserId}", userId);

            using var connection = new NpgsqlConnection(_connectionString);

            // SQL Query with joins to get the required data
            var borrowedBooks = await connection.QueryAsync<BorrowedBookReturnDto>(
                @"SELECT u.username, b.title AS bookname, b.id AS bookid, bb.borroweddate, bb.returndate, bb.id, bb.isreturned
          FROM borrowedbooks bb
          JOIN users u ON u.id = bb.userid
          JOIN books b ON b.id = bb.bookid
          WHERE bb.userid = @UserId",
                new { UserId = userId });

            return Ok(borrowedBooks);
        }


        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetAllBorrowedBooks()
        {
            _logger.LogInformation("Getting all borrowed books");
            using var connection = new NpgsqlConnection(_connectionString);
            var borrowedBooks = await connection.QueryAsync<BorrowedBookReturnDto>(
                @"SELECT u.username, b.title AS bookname, b.id AS bookid,  bb.borroweddate, bb.returndate, bb.id, bb.isreturned
          FROM borrowedbooks bb
          JOIN users u ON u.id = bb.userid
          JOIN books b ON b.id = bb.bookid");

            return Ok(borrowedBooks);
        }

        [HttpGet("book/{id}")]
        [Authorize(Roles = "Customer, Librarian")]
        public async Task<IActionResult> GetBorrowedBookById(int id)
        {
            _logger.LogInformation("Getting borrowed book with ID {Id}", id);
            using var connection = new NpgsqlConnection(_connectionString);
            var borrowedBook = await connection.QueryFirstOrDefaultAsync<BorrowedBook>(
                "SELECT * FROM borrowedbooks WHERE id = @Id", new { Id = id });

            if (borrowedBook == null)
            {
                _logger.LogWarning("Borrowed book with ID {Id} not found", id);
                return NotFound("Borrowed book not found.");
            }

            return Ok(borrowedBook);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> BorrowBook([FromBody] BorrowBookDto borrowBookDto)
        {
            _logger.LogInformation("User {UserId} is borrowing book with ID {BookId}", borrowBookDto.UserId, borrowBookDto.BookId);
            using var connection = new NpgsqlConnection(_connectionString);
            var book = await connection.QueryFirstOrDefaultAsync<Book>(
                "SELECT * FROM books WHERE id = @BookId", new { borrowBookDto.BookId });

            if (book == null)
            {
                _logger.LogWarning("Book with ID {BookId} not found", borrowBookDto.BookId);
                return NotFound("Book not found.");
            }

            if (book.CopiesAvailable <= 0)
            {
                _logger.LogWarning("No copies available for book with ID {BookId}", borrowBookDto.BookId);
                return BadRequest("No copies available for borrowing.");
            }

            var borrowedBook = new BorrowedBook
            {
                UserId = borrowBookDto.UserId,
                BookId = borrowBookDto.BookId,
                BorrowedDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(14)
            };

            var query = "INSERT INTO borrowedbooks (userid, bookid, borroweddate, returndate) VALUES (@UserId, @BookId, @BorrowedDate, @ReturnDate)";
            await connection.ExecuteAsync(query, borrowedBook);

            await connection.ExecuteAsync("UPDATE books SET copiesavailable = copiesavailable - 1 WHERE id = @BookId", new { borrowBookDto.BookId });

            _logger.LogInformation("Book with ID {BookId} borrowed successfully by user {UserId}", borrowBookDto.BookId, borrowBookDto.UserId);
            return Ok("Book borrowed successfully.");
        }

        [HttpPut]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnBookDto returnBookDto)
        {
            _logger.LogInformation("User {UserId} is returning book with ID {BookId}", returnBookDto.UserId, returnBookDto.BookId);
            using var connection = new NpgsqlConnection(_connectionString);
            var borrowedBook = await connection.QueryFirstOrDefaultAsync<BorrowedBook>(
                "SELECT * FROM borrowedbooks WHERE bookid = @Id", new { Id = returnBookDto.BookId });

            if (borrowedBook == null)
            {
                _logger.LogWarning("Borrowed book with ID {BookId} not found", returnBookDto.BookId);
                return NotFound("Borrowed book not found.");
            }

            if (borrowedBook.UserId != returnBookDto.UserId)
            {
                _logger.LogWarning("User {UserId} is not authorized to return book with ID {BookId}", returnBookDto.UserId, returnBookDto.BookId);
                return Unauthorized("You are not authorized to return this book.");
            }

            var query = "UPDATE borrowedbooks SET isreturned = true WHERE id = @Id";
            await connection.ExecuteAsync(query, new { Id = borrowedBook.Id });

            await connection.ExecuteAsync("UPDATE books SET copiesavailable = copiesavailable + 1 WHERE id = @BookId", new { borrowedBook.BookId });

            _logger.LogInformation("Book with ID {BookId} returned successfully by user {UserId}", returnBookDto.BookId, returnBookDto.UserId);
            return Ok("Book returned successfully.");
        }
    }
}
