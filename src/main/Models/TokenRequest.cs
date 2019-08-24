namespace Geheb.DevMon.Agent.Models
{
    internal sealed class TokenRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Audience { get; set; }
        public string GrantType { get; set; }
    }
}
