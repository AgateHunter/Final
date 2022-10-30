using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        //Category Id will be the Foreign Key for this table
        [Range(1,int.MaxValue,ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
        public  Category? Category {get; set;}

        [Range(1, int.MaxValue, ErrorMessage = "Time must be greater than 0.")]
        public int Time { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string CategoryTitleWithIcon {
            get
            {
                return Category == null ? "" : Category.Icon + " " + Category.Title;
            } 
        }

        [NotMapped]
        public string FormattedTime
        {
            get
            {
                return ((Category == null || Category.Type == "Purposeful" ) ? "+ " :  "- ") + Time.ToString("C0");
            }
        }

    }
}
