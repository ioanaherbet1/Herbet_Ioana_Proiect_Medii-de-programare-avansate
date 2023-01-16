using System.Security.Policy;

namespace Herbet_Ioana_Proiect.Models
{
    public class AvailableDress
    {
        public int ShopID { get; set; }
        public int DressID { get; set; }
        public Shop? Shop { get; set; }
        public Dress? Dress { get; set; }
    }
}
