using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotDesk.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotDesk.Controllers
{
    public class TestController : Controller
    {
        private readonly HotDeskDbContext _context;

        public TestController(HotDeskDbContext context)=>_context = context;

        [Route("/")]
        [AllowAnonymous]
        public object Test()
        {
            return _context.Reservations.ToList(); ;
        }
    }
}
