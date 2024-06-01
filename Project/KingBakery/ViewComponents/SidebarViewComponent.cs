using Microsoft.AspNetCore.Mvc;

namespace KingBakery.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
