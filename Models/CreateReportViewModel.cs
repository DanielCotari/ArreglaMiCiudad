using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArreglaMiCiudad.Models
{
    public class CreateReportViewModel
    {
        public int CategoryId { get; set; }
        public string? Description { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public List<IFormFile>? Images { get; set; }

    }
}
