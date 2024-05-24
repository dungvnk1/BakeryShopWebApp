using KingBakery.Data;
using Microsoft.AspNetCore.Mvc;

namespace KingBakery.Models
{
    public class Header : ViewComponent
    {
        private readonly KingBakeryContext _context;

        public Header(KingBakeryContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
