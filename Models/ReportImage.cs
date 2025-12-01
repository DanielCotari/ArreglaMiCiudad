namespace ArreglaMiCiudad.Models
{
    public class ReportImage
    {
        public int ReportImageId { get; set; }
        public int ReportId { get; set; }
        public string? ImageUrl { get; set; }
        public Report? Report { get; set; }
    }
}
