namespace Event.Process.Services.Main.Extensions;

public static class ExtensionsSerialization
{
    public static string Serialize<T>(this T objectToSerialize) => System.Text.Json.JsonSerializer.Serialize<T>(objectToSerialize);
        
    public static T Deserialize<T>(this string jsonText) => System.Text.Json.JsonSerializer.Deserialize<T>(jsonText);

    public static byte[] ToByteArray(this string text) => System.Text.Encoding.UTF8.GetBytes(text);

    public static string ToUTF8String(this byte[] bytes) => System.Text.Encoding.UTF8.GetString(bytes);

    public static ReadOnlyMemory<byte> ToReadOnlyMemory(this byte[] bytes) => new ReadOnlyMemory<byte>(bytes);
}
