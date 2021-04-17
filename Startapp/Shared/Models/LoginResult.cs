using IdentityModel.Client;
using System;

namespace Startapp.Shared.Models
{
    public class ExternalTokenRequest : TokenRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Provider { get; set; }
        public string Token { get; set; }
        public string Scope { get; set; }
    }
    

    public class LoginResponse
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public string Message { get; set; }
    }

    public class AccessToken
    {
        public int Nbf { get; set; }
        public int Exp { get; set; }
        public int AuthTime { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
        public string ClientId { get; set; }
        public string Sub { get; set; }
        public string Idp { get; set; }
        public string Role { get; set; }
        public string Permission { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string Configuration { get; set; }
        public string Scope { get; set; }
        public string Amr { get; set; }
    }
}
