using Postgrest.Attributes;
using Postgrest.Models;
using System.Text.Json.Serialization;

namespace minimalapi.Models
{
    class users : BaseModel
    {
        [JsonPropertyName("user_id")]
        [PrimaryKey]
        public Guid user_id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("email")]
        public string email { get; set; }
        [JsonPropertyName("password")]
        public string password { get; set; }
    }
}
