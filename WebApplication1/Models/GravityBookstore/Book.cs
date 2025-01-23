using WebApplication1.Models.GravityBookstore;

public class Book
{
    public int BookId { get; set; } 
    public string Title { get; set; } 
    public string Isbn13 { get; set; } 
    public int? NumPages { get; set; } 
    public DateOnly? PublicationDate { get; set; } 
    public int? PublisherId { get; set; } 
    public int? LanguageId { get; set; } 
    
    public virtual BookLanguage Language { get; set; }
    public virtual ICollection<Author> Authors { get; set; }
    public virtual ICollection<OrderLine> OrderLines { get; set; }
}