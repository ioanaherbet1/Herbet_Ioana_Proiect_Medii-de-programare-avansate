using System.ComponentModel.DataAnnotations;

namespace Herbet_Ioana_Proiect.Models
{
    public class Shop
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Shop Name")]
        [StringLength(50)]
        public string ShopName { get; set; }

        [StringLength(70)]
        public string Adress { get; set; }
        public ICollection<AvailableDress>? AvailableDresses { get; set; }
    }
}
