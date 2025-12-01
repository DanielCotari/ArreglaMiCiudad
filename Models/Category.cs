namespace ArreglaMiCiudad.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }

        public ICollection<Report>? Reports { get; set; }
    }
}
