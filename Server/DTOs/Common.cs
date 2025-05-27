using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    public class Common
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public bool IsPermanent { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        public string? ImageName { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Facilities { get; set; }
    }
}
