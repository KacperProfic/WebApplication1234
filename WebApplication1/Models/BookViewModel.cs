using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class BookViewModel
    {
        public int BookId { get; set; } 
        
        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        [StringLength(200, ErrorMessage = "Tytuł nie może być dłuższy niż 200 znaków.")]
        public string Title { get; set; } 

        [Required(ErrorMessage = "ISBN jest wymagany.")]
        [RegularExpression(@"\d{13}", ErrorMessage = "ISBN musi składać się z 13 cyfr.")]
        public string Isbn13 { get; set; } 

        [Range(1, int.MaxValue, ErrorMessage = "Liczba stron musi być większa niż 0.")]
        public int? NumPages { get; set; } 

        [DataType(DataType.Date)]
        public DateOnly? PublicationDate { get; set; } 
        
        public int? LanguageId { get; set; }
        
        public int? PublisherId { get; set; }
        public int AuthorsCount { get; set; }
        public int SoldCopies { get; set; }
    }
}