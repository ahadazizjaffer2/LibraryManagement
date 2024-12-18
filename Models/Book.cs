namespace librarymanagement.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } // Book title
        public string Author { get; set; } // Author name
        public string Genre { get; set; } // Genre of the book
        public int CopiesAvailable { get; set; } // Number of available copies
        public DateTime YearPublished { get; set; } // Date of publication
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
