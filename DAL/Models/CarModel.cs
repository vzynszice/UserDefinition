
namespace DAL.Models
{
    public class CarModel
    {
        public int id { get; set; }
        public string Plate { get; set; }
        public ICollection<PartDamage> PartDamages { get; set; }

    }
}
