namespace DAL.Models
{
    public class PartDamage
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int PartTypeId { get; set; }
        public int DamageTypeId { get; set; }
    }
}
