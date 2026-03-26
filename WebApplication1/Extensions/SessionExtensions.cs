using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Extensions;

public static class SessionExtensions
{
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T? GetObjectFromJson<T>(this ISession session)
    {
        var data = session.GetString("Cart");
        if (data == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(data);
    }

    public static T? GetObjectFromJson<T>(this ISession session, string key)
    {
        var data = session.GetString(key);
        if (data == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(data);
    }
}
