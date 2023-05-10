using System.Text.Json.Serialization;

namespace minimalapi.Models.LoginDto
{
    public class LoginDto   
    {
        //[JsonPropertyName("userId")]
        //public Guid? UserId { get; set; }
        
        [JsonPropertyName("Username")]
        public string? Username { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}