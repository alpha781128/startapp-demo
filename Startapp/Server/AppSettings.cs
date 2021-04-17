namespace Startapp.Server
{
    public class AppSettings
    {
        public string DefaultUserRole { get; set; }
        public SmtpConfig SmtpConfig { get; set; }
        public PayPal PayPal { get; set; }

    }

    public class SmtpConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }

        public string Name { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class PayPal
    {
        public string BrandName { get; set; }
        public string LandingPage { get; set; }
        public string PAYPAL_CLIENT_ID { get; set; }
        public string PAYPAL_CLIENT_SECRET { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
