namespace librarymanagement.Dtos
{
    public class CreateBookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime YearPublished { get; set; }
        public int CopiesAvailable { get; set; } // Number of available copies
        public string Genre { get; set; }
    }
}
