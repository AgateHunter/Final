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
        public int CategoryId { get; set; }
        public  Category Category {get; set;}

        public int Time { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
