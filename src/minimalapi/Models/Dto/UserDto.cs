
using System.Text.Json.Serialization;

namespace minimalapi.Models.Dto
{
    public class UserDto
    {
        //[JsonPropertyName("userId")]
        //public Guid? UserId { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}
