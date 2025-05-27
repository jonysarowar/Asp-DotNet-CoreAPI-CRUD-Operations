using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public bool IsPermanent { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        public string? ImageName { get; set; }
        public string? ImageUrl { get; set; }
        public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();

    }

    public class Facility
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; } = null!;
        public int MemberId { get; set; }
        public virtual Member Member { get; set; } = null!;

    }

    public class AppDbContext:DbContext
    {
        public AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions<AppDbContext> op)
        : base(op) { }
        public DbSet<Member> Members { get; set; }
        public DbSet<Facility> Facilities { get; set; }
    }
}
