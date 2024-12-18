namespace librarymanagement.Dtos
{
    public class UpdateBookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime YearPublished { get; set; }
        public int CopiesAvailable { get; set; }

        public string Genre { get; set; }
    }
}
