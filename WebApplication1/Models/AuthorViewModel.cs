namespace WebApplication1.Models
{
    public class AuthorViewModel
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public List<BookViewModel> BookTitles { get; set; } 
    }
}