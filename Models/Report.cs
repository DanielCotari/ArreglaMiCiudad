using System.Collections;

namespace ArreglaMiCiudad.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
        public int StatusId { get; set; }
        public string? StatusText { get; set; }
        public DateTime CreatedAt { get; set; }
        public Category? Category { get; set; }
        public ICollection<ReportImage>? ReportImages { get; set; }
    }
}
