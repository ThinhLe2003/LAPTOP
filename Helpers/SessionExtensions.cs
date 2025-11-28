using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text.Json;

namespace LAPTOP.Helpers
{
	public static class SessionExtensions
	{
		// Lưu object vào session
		public static void SetObjectAsJson(this ISession session, string key, object value)
		{
			session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
		}

		// Lấy object từ session
		public static T GetObjectFromJson<T>(this ISession session, string key)
		{
			var value = session.GetString(key);
			return value == null ? default(T) : System.Text.Json.JsonSerializer.Deserialize<T>(value);
		}
	}
}