using System.Text.Json;
using System.Text.Json.Serialization;

namespace SME_API_MSME.Models
{
    public class ProjectModels
    {
        [JsonPropertyName("projectCode")]
        [JsonConverter(typeof(LongFromStringJsonConverter))]
        public long ProjectCode { get; set; }

        public string? BudgetYear { get; set; }

        public string? DateApprove { get; set; }
        [JsonConverter(typeof(NullableLongEmptyStringConverter))] // Add this converter
        public long? OrgId { get; set; } = null;
      

        public string? OrgName { get; set; }

        public decimal? ProjectBudget { get; set; }

        public decimal? ProjectOffBudget { get; set; }

        public decimal? ProjectSumBudget { get; set; }

        public string? SmeProjectStatusName { get; set; }

        public string? LegalGroupName { get; set; }

        public string? ProjectName { get; set; }

        public string? ProjectNameInitials { get; set; }

        public string? ProjectReason { get; set; }

        public string? ProjectPurpose { get; set; }

        public string? TypeBudget { get; set; }

        public string? TypeResultMsme { get; set; }

        public string? PlanMessage { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }
    }


    public class ResultApiResponeProject
    {
        public List<ProjectModels> result { get; set; }

        public int responseCode { get; set; }
        public string responseMsg { get; set; }
    }

    // Add this converter in the same file or a shared location
    public class LongFromStringJsonConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (long.TryParse(reader.GetString(), out var value))
                    return value;
                throw new JsonException("Invalid long value in string.");
            }
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt64();
            }
            throw new JsonException("Invalid token type for long.");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
    public class NullableLongEmptyStringConverter : JsonConverter<long?>
    {
        public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return null; // Convert "" to null
                }
                if (long.TryParse(stringValue, out long result))
                {
                    return result;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt64();
            }
            // Handle other cases like null directly
            return null;
        }

        public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
