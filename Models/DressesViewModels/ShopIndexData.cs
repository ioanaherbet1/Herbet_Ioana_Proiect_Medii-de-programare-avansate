using System.Security.Policy;

namespace Herbet_Ioana_Proiect.Models.DressesViewModels
{
    public class ShopIndexData
    {
        public IEnumerable<Shop> Shops { get; set; }
        public IEnumerable<Dress> Dresses { get; set; }
        public IEnumerable<Order> Orders { get; set; }

    }
}
