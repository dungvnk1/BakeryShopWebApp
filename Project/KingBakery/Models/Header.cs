using Microsoft.AspNetCore.Mvc;

namespace KingBakery.Models
{
    public class Header : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
