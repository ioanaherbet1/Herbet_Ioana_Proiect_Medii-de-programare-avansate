using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Herbet_Ioana_Proiect.Models
{
    public class Dress
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int? DesignerID { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }

        public ICollection<Order>? Orders { get; set; }
        public Designer? Designer { get; set; }
        public ICollection<AvailableDress>? AvailableDresses{ get; set; }
    }
}
