using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSIT150Site.Models;

namespace MSIT150Site.Controllers
{
    public class ApiController : Controller
    {
        private readonly DemoContext _context;
        private readonly IWebHostEnvironment _host;

        public ApiController(DemoContext context,IWebHostEnvironment host)
        {
            _host = host;
            _context = context;
        }
        public IActionResult Index()
        {
            System.Threading.Thread.Sleep(5000);
            return Content("Hello Ajax!!!");
        }
        public IActionResult GetDemo(UserViewModel user) //public IActionResult GetDemo(string name,int age=30)
        {
            if (string.IsNullOrEmpty(user.name))
            {
                user.name = "guest";
            }
            return Content($"Hello {user.name}, You are {user.age} years old.");
        }
        public IActionResult Register(Members member,IFormFile file)
        {
            string filePath = Path.Combine(_host.WebRootPath, "uploads", file.FileName);
            using(var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            //return Content($"檔案已上傳到：{filePath}");

            //將圖轉成二進位
            byte[]? imgByte = null;
            using(var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                imgByte = memoryStream.ToArray();
            }

            member.FileName = file.FileName;
            member.FileData = imgByte;
            _context.Members.Add(member);
            _context.SaveChanges();

            return Content("新增成功!!");
        }


        public IActionResult GetImageByte(int id = 5)
        {
            Members? member = _context.Members.Find(id); //find是找primary key
            byte[]? img = member.FileData;
            return File(img, "image/jpeg");
        }

        public IActionResult Cities()
        {
            var cites =_context.Address.Select(c=>c.City).Distinct();
            return Json(cites);
        }

        public IActionResult Districts(string city) 
        {
            var districts = _context.Address.Where(a=>a.City == city).Select(c=>c.SiteId).Distinct();
            return Json(districts);
        }


        public IActionResult Roads(string siteid)
        {
            var roads = _context.Address.Where(a => a.SiteId == siteid).Select(c => c.Road).Distinct();
            return Json(roads);
        }




        public IActionResult CheckAccount(UserViewModel user)
        {
            var existingMember = _context.Members.FirstOrDefault(m => m.Name == user.name);
            
            if (existingMember !=null)
                return Content("帳號存在");
            return Content("帳號不存在");
        }
        }
}
