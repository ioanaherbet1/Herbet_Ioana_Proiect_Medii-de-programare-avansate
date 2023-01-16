namespace Herbet_Ioana_Proiect.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int DressID { get; set; }
        public DateTime? OrderDate { get; set; }
        public Customer? Customer { get; set; }
        public Dress? Dress { get; set; }
    }
}
