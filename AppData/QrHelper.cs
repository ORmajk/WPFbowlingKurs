using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using QRCoder;

namespace WPFbowlingKurs.AppData
{
    internal class QrHelper
    {
        private const string ClubName = "Боулинг-клуб Голден Страйк";
        private const string Account = "40702810000000000001";
        private const string BankName = "ПАО Сбербанк";
        private const string Bic = "044525225";
        private const string CorrAccount = "30101810400000000225";

        public static string GetPaymentText(Bookings booking)
        {
            long kopecks = (long)(booking.TotalAmount * 100);
            return "ST00012|Name=" + ClubName +
                   "|PersonalAcc=" + Account +
                   "|BankName=" + BankName +
                   "|BIC=" + Bic +
                   "|CorrespAcc=" + CorrAccount +
                   "|Sum=" + kopecks +
                   "|Purpose=Оплата брони " + booking.BookingNumber;
        }

        public static BitmapImage GetQrImage(string text)
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCodeData data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M, true);
            PngByteQRCode qrCode = new PngByteQRCode(data);
            byte[] bytes = qrCode.GetGraphic(8);

            BitmapImage image = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
            }
            return image;
        }
    }
}
