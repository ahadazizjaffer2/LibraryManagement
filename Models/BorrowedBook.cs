namespace librarymanagement.Models
{
    public class BorrowedBook
    {
        public int Id { get; set; } // Unique identifier
        public int BookId { get; set; } // Reference to the book being borrowed
        public int UserId { get; set; } // User borrowing the book
        public DateTime BorrowedDate { get; set; } = DateTime.Now; // Date of borrowing
        public DateTime? ReturnDate { get; set; } // Date of return (null if not returned yet)
        public bool IsReturned { get; set; } = false; // Indicates if the book is returned
    }
}
