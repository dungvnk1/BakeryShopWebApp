using Microsoft.AspNetCore.Mvc;

namespace KingBakery.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
