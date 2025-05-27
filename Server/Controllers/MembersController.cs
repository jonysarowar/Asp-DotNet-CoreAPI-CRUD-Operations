using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Server.DTOs;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _web;

        public MembersController(AppDbContext db, IWebHostEnvironment web)
        {
            _db = db;
            _web = web;
        }

        [HttpGet]
        public IActionResult GetMembers()
        {
            List<Member> members = _db.Members.Include(f => f.Facilities).ToList();
            string jsonstring = JsonConvert.SerializeObject(members, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            return Content(jsonstring, "application/json");
        }


        [HttpGet("{id}")]
        public IActionResult GetMember(int id)
        {
            Member member = _db.Members.Include(f => f.Facilities).SingleOrDefault(e => e.MemberId == id);
            if (id == null)
            {
                return NotFound();
            }
            string jsonstring = JsonConvert.SerializeObject(member, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            return Content(jsonstring, "application/json");
        }


        [HttpPost]
        public async Task<IActionResult> PostMember([FromForm] Common objCommon)
        {
            ImgUpload fileApi = new ImgUpload();
            string fileName = objCommon.ImageName + ".png";
            fileApi.ImgName = "\\images\\" + fileName;
            if (objCommon.ImageFile?.Length > 0)
            {
                if (!Directory.Exists(_web.WebRootPath + "\\images"))
                {
                    Directory.CreateDirectory(_web.WebRootPath + "\\images\\");
                }
                string filePath = _web.WebRootPath + "\\images\\" + fileName;
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    objCommon.ImageFile.CopyTo(stream);
                    stream.Flush();
                }
                fileApi.ImgName = "/images/" + fileName;
            }
            Member memObj = new Member();
            memObj.Name = objCommon.Name;
            memObj.IsPermanent = objCommon.IsPermanent;
            memObj.Date = objCommon.Date;
            memObj.ImageName = objCommon.ImageName;
            memObj.ImageUrl = fileApi.ImgName;
            List<Facility> faList = JsonConvert.DeserializeObject<List<Facility>>(objCommon.Facilities);
            memObj.Facilities = faList;
            _db.Members.Add(memObj);
            await _db.SaveChangesAsync();
            return Ok(memObj);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, [FromForm] Common objCommon)
        {
            var memObj = await _db.Members.FindAsync(id);
            if (memObj == null) return NotFound("Member not found");

            ImgUpload fileApi = new ImgUpload();
            string fileName = objCommon.ImageName + ".png";
            fileApi.ImgName = "\\images\\" + fileName;
            if (objCommon.ImageFile?.Length > 0)
            {
                if (!Directory.Exists(_web.WebRootPath + "\\images"))
                {
                    Directory.CreateDirectory(_web.WebRootPath + "\\images\\");
                }
                string filePath = _web.WebRootPath + "\\images\\" + fileName;
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    objCommon.ImageFile.CopyTo(stream);
                    stream.Flush();
                }
                fileApi.ImgName = "/images/" + fileName;
            }

            memObj.Name = objCommon.Name;
            memObj.IsPermanent = objCommon.IsPermanent;
            memObj.Date = objCommon.Date;
            memObj.ImageName = objCommon.ImageName;
            memObj.ImageUrl = fileApi.ImgName;
            List<Facility> faList = JsonConvert.DeserializeObject<List<Facility>>(objCommon.Facilities);

            var exFacility = _db.Facilities.Where(e => e.MemberId == id);

            _db.Facilities.RemoveRange(exFacility);

            if (faList.Any())
            {
                foreach (var item in faList)
                {
                    Facility fa = new Facility
                    {
                        MemberId = memObj.MemberId,
                        FacilityName = item.FacilityName

                    };
                    _db.Facilities.Add(fa);
                }
                await _db.SaveChangesAsync();
            }

            return Ok("Update successfull");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var memObj = await _db.Members.FindAsync(id);
            if (memObj == null) return NotFound("Member not found");
            _db.Members.Remove(memObj);
            await _db.SaveChangesAsync();
            return Ok("Member Deleted Successfully");
        }

    }
}
