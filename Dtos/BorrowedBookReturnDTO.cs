namespace librarymanagement.Dtos
{
    public class BorrowedBookReturnDto
    {
        public string Username { get; set; }
        public string Bookname { get; set; }
        public DateTime BorrowedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int BookId { get; set; }  // The ID from the "books" table
        public int Id { get; set; }  // The ID from the "borrowedbooks" table
        public bool IsReturned { get; set; }
    }
}