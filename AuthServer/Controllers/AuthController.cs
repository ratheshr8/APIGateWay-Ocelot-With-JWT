namespace AuthServer.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IOptions<Audience> _settings;

        public AuthController(IOptions<Audience> settings)
        {
            _settings = settings;
        }

        [HttpGet]
        public IActionResult Get(string name, string pwd)
        {
            var audience = _settings.Value;
            if (string.IsNullOrWhiteSpace(audience.Secret)
                || string.IsNullOrWhiteSpace(audience.DemoUser)
                || string.IsNullOrWhiteSpace(audience.DemoPassword))
            {
                return BadRequest("Set Audience Secret, DemoUser, and DemoPassword in AuthServer appsettings before calling this endpoint.");
            }

            if (name != audience.DemoUser || pwd != audience.DemoPassword)
            {
                return Unauthorized();
            }

            var now = DateTime.UtcNow;
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audience.Secret));
            var jwt = new JwtSecurityToken(
                issuer: audience.Iss,
                audience: audience.Aud,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Json(new
            {
                access_token = encodedJwt,
                expires_in = (int)TimeSpan.FromMinutes(2).TotalSeconds
            });
        }
    }

    public class Audience
    {
        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
        public string DemoUser { get; set; }
        public string DemoPassword { get; set; }
    }
}
