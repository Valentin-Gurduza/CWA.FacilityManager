using QRCoder;

namespace CWA.FacilityManager.Server.Services
{
    public interface IQrCodeService
    {
        string GenerateQrCodeDataUri(string data);
        byte[] GenerateQrCodeBytes(string data);
    }

    public class QrCodeService : IQrCodeService
    {
        private readonly ILogger<QrCodeService> _logger;
        private static readonly QRCodeGenerator _qrGenerator = new();
        private const int DefaultGraphicSize = 20;

        public QrCodeService(ILogger<QrCodeService> logger)
        {
            _logger = logger;
        }

        public string GenerateQrCodeDataUri(string data)
        {
            try
            {
                var qrCodeSvg = GenerateQrCodeSvg(data);
                var svgBytes = System.Text.Encoding.UTF8.GetBytes(qrCodeSvg);
                return $"data:image/svg+xml;base64,{Convert.ToBase64String(svgBytes)}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate QR code data URI");
                throw;
            }
        }

        public byte[] GenerateQrCodeBytes(string data)
        {
            try
            {
                var qrCodeSvg = GenerateQrCodeSvg(data);
                return System.Text.Encoding.UTF8.GetBytes(qrCodeSvg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate QR code bytes");
                throw;
            }
        }

        private static string GenerateQrCodeSvg(string data)
        {
            var qrCodeData = _qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new SvgQRCode(qrCodeData);
            return qrCode.GetGraphic(DefaultGraphicSize);
        }
    }
}