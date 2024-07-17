using KingBakery.ViewModel;

namespace KingBakery.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExcute(IQueryCollection collections);
    }
}
