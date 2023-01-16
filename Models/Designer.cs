namespace Herbet_Ioana_Proiect.Models
{
    public class Designer
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Dress>? Dresses { get; set; }
    }
}
