using Microsoft.AspNetCore.Mvc;

namespace KingBakery.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
