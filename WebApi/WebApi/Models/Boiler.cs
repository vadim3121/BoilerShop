using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Models
{
    [Table("Boiler")]
    public class Boiler
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("price")]
        public int Price { get; set; }

        [Column("size")]
        public int Size { get; set; }

        [Column("color")]
        public string Color { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("brandid")]
        public long BrandId { get; set; }

        [Column("categoryid")]
        public long CategoryId { get; set; }

        public Brand? Brand { get; set; }

        public Category? Category { get; set; }

        [JsonIgnore]
        public ICollection<CartItem> CartItems { get; set; }

        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
