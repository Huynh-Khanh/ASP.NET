namespace HuynhDuyKhanh_2122110004.Auth
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = "THIS_IS_A_SUPER_SECRET_KEY_1234567890_32";
        public string Issuer { get; set; } = "HuynhDuyKhanh_2122110004App";
        public string Audience { get; set; } = "HuynhDuyKhanh_2122110004Users";
        public int ExpirationMinutes { get; set; } = 60;
    }
}
