using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TcpClientConnector;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace FileMigratorWebApp.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment _environment;

        public HomeController(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ICollection<IFormFile> files)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        var stream = reader.BaseStream;

                    //await file.CopyToAsync(fileStream);
                    //var stream = file.OpenReadStream();
                        string hostName = "localhost";
                        IPHostEntry ip = Dns.GetHostEntryAsync(hostName).Result;

                        TcpConnector connector = new TcpConnector(ip.AddressList[1], 8080);
                        await connector.ConnectAsync(stream);
                        
                    }
                }
            }
            return View();
        }
        
    }
}
