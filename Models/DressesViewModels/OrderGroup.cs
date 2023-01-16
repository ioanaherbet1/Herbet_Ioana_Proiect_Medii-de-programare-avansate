using System.ComponentModel.DataAnnotations;

namespace Herbet_Ioana_Proiect.Models.DressesViewModels
{
    public class OrderGroup
    {
        [DataType(DataType.Date)]
        public DateTime? OrderDate { get; set; }
        public int DressCount { get; set; }
    }
}
