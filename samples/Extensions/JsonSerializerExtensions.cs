using Newtonsoft.Json;

namespace GithubApiProxy.Extensions
{
    internal static class JsonSerializerExtensions
    {
        public static T? Deserialize<T>(this JsonSerializer serializer, Stream stream) where T : class
        {
            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            var response = serializer.Deserialize<T>(jsonReader);

            return response;
        }

        public static T? Deserialize<T>(this JsonSerializer serializer, string text) where T : class
        {
            using var reader = new StringReader(text);
            using var textReader = new JsonTextReader(reader);
            var response = serializer.Deserialize<T>(textReader);

            return response;
        }

        public static T? Deserialize<T>(this JsonSerializer serializer, MemoryStream memoryStream) where T : class
        {
            using var reader = new StreamReader(memoryStream, leaveOpen: true);
            using var jsonReader = new JsonTextReader(reader);
            return serializer.Deserialize<T>(jsonReader);
        }

        public static string Serialize<T>(this JsonSerializer serializer, T value)
        {
            using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);
            serializer.Serialize(jsonWriter, value);
            return stringWriter.ToString();
        }

        public static void Serialize(this JsonSerializer serializer, FileStream stream, object? value)
        {
            using var writer = new StreamWriter(stream);
            using var jsonWriter = new JsonTextWriter(writer);

            serializer.Serialize(jsonWriter, value);

            jsonWriter.Flush();
        }
    }
}
