using Microsoft.AspNetCore.Mvc;

namespace KingBakery.ViewComponents
{
    public class TopBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
