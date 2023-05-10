using Newtonsoft.Json;
using System.Text.Json;
using System.Text;

namespace VkAppAPI
{
    public static class JsonConvertor
    {
        public static string ToJsonString(this JsonDocument? json)
        {
            if (json is null) { throw new ArgumentNullException(nameof(json), "Json must be non null"); }
            using var stream = new MemoryStream();
            Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = true });
            json.WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public static Dictionary<string, string> ToJsonDict(this JsonDocument? json)
        {
            var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json.ToJsonString());
            if (jsonDict is null) { throw new ArgumentNullException(nameof(jsonDict), "Json Dict must be non null"); }
            return jsonDict;
        }

        public static bool ValidateJsonKeys(Dictionary<string, string> jsonDict, string[] requiredKeys, out string message)
        {
            if (jsonDict is null) { throw new ArgumentNullException(nameof(jsonDict), "Json Dict must be non null"); }
            message = string.Empty;

            if (jsonDict.Count > requiredKeys.Length)
            {
                message = "Json Dict has more keys then it's allowed";
                return false;
            }

            var notFoundKeys = requiredKeys.Except(jsonDict.Keys);
            if (requiredKeys.Except(jsonDict.Keys).Any())
            {
                message = $"Json dosen't have some required keys : " +
                $"{String.Join(", ", notFoundKeys)}"; // проверка корректности ключей в json
                return false;
            }
            return true;
        }

        public static bool ValidateJsonValues(string value, string[] allowedValues, out string message)
        {
            message = string.Empty;
            if (!allowedValues.Contains(value))
            {
                message = $"value {value} is not allowed. Allowed values : {String.Format(", ", allowedValues)}";
                return false;
            }
            return true;
        }
    }
}
