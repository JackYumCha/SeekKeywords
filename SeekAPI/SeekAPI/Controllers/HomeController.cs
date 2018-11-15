using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace SeekAPI.Controllers
{
    public class HomeController: Controller
    {
        /// <summary>
        /// this ensures that your SPA can load via the http services.
        /// </summary>
        /// <returns></returns>
        public IActionResult Spa()
        {
            return File("~/index.html", "text/html");
        }
    }
}
