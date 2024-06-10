using Microsoft.AspNetCore.Mvc;
using System.Web;
using WebIcecream_FE_USER.Areas.VNPayAPI.Util;

namespace WebIcecream_FE_USER.Areas.VNPayAPI.Controllers
{
    [Area("VNPayAPI")]
    public class HomeController : Controller
    {
        public string url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public string returnUrl = $"https://localhost:30214/VNPayAPI/PaymentConfirm";
        public string tmnCode = "JITYBPC1";
        public string hashSecret = "UHPETABJAMBLLTBVQJICHJRGOIQCMDSR";

        public ActionResult Index()
        {
            return View();
        }

        [Route("/VNPayAPI/Payment")]
        public ActionResult Payment(string amount, string infor, string orderinfor)
        {
            string hostName = System.Net.Dns.GetHostName();
            string clientIPAddress = System.Net.Dns.GetHostAddresses(hostName).GetValue(0).ToString();
            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); // Phiên bản API của VNPAY
            pay.AddRequestData("vnp_Command", "pay"); // Lệnh thanh toán
            pay.AddRequestData("vnp_TmnCode", tmnCode); // Mã website của merchant trên hệ thống của VNPAY
            pay.AddRequestData("vnp_Amount", (Convert.ToDecimal(amount) * 25000).ToString()); // Số tiền cần thanh toán, nhân với 100 để chuyển sang đơn vị VNĐ
            pay.AddRequestData("vnp_BankCode", ""); // Mã Ngân hàng thanh toán
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); // Ngày tạo giao dịch
            pay.AddRequestData("vnp_CurrCode", "VND"); // Đơn vị tiền tệ
            pay.AddRequestData("vnp_IpAddr", clientIPAddress); // Địa chỉ IP của khách hàng
            pay.AddRequestData("vnp_Locale", "vn"); // Ngôn ngữ hiển thị
            pay.AddRequestData("vnp_OrderInfo", infor); // Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); // Loại đơn hàng
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); // URL thông báo kết quả giao dịch

            // Đảm bảo mã giao dịch là duy nhất
            pay.AddRequestData("vnp_TxnRef", Guid.NewGuid().ToString()); // Mã giao dịch duy nhất

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            return Redirect(paymentUrl);
        }

        [Route("/VNPayAPI/PaymentConfirm")]
        public IActionResult PaymentConfirm()
        {
            if (Request.QueryString.HasValue)
            {
                var queryString = Request.QueryString.Value.Substring(1); // Bỏ dấu '?' ở đầu
                var json = HttpUtility.ParseQueryString(queryString);

                string orderId = json["vnp_TxnRef"];
                string orderInfor = json["vnp_OrderInfo"];
                long vnpayTranId = Convert.ToInt64(json["vnp_TransactionNo"]);
                string vnp_ResponseCode = json["vnp_ResponseCode"];
                string vnp_SecureHash = json["vnp_SecureHash"];
                string vnp_TmnCode = json["vnp_TmnCode"];

                // Loại bỏ vnp_SecureHash khỏi chuỗi raw data
                var pos = queryString.IndexOf("&vnp_SecureHash");
                var rawData = queryString.Substring(0, pos);

                // Kiểm tra chữ ký
                bool checkSignature = ValidateSignature(rawData, vnp_SecureHash, hashSecret);

                if (checkSignature && tmnCode == vnp_TmnCode)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }
            else
            {
                ViewBag.Message = "Phản hồi không hợp lệ";
            }
            return View("PaymentConfirm");
        }

        public bool ValidateSignature(string rspraw, string inputHash, string secretKey)
        {
            string myChecksum = PayLib.HmacSHA512(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

    }
}