using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace LAPTOP.Helpers
{
    public static class SessionExtensions
    {
        // Lưu object (giỏ hàng) vào session dưới dạng JSON
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Lấy object từ session và chuyển ngược lại thành List/Object
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }
}