using System.Text.Json.Serialization;

namespace OnlineStoreLambda.DTOs
{
    public class SenderData
    {
        [JsonPropertyName("SenderEmailAddress")]
        public string SenderEmailAddress { get; set; } = string.Empty;
        [JsonPropertyName("SenderFirstName")]
        public string SenderFirstName { get; set; } = string.Empty;
        [JsonPropertyName("SenderLastName")]
        public string SenderLastName { get; set; } = string.Empty;
        [JsonPropertyName("SenderMessage")]
        public string SenderMessage { get; set; } = string.Empty;
        [JsonPropertyName("SenderPhoneNumber")]
        public string SenderPhoneNumber { get; set; } = string.Empty;
    }
}
