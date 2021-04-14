using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.Exchange.Clients.Common
{
	public class ClientIdCookie
	{
		private ClientIdCookie()
		{
		}

		public static string GetCookieValueAndSetIfNull(HttpContext httpContext)
		{
			ClientIdCookie clientIdCookie = new ClientIdCookie();
			return clientIdCookie.GetCookieValueAndSetIfNullInternal(httpContext);
		}

		public static string ParseToPrintableString(string cookieValue)
		{
			if (cookieValue.Length <= 12)
			{
				cookieValue = cookieValue.PadRight(16, ' ');
			}
			return string.Format("{0} - {1} - {2} - {3}", new object[]
			{
				cookieValue.Substring(0, 4),
				cookieValue.Substring(4, 4),
				cookieValue.Substring(8, 4),
				cookieValue.Substring(12)
			});
		}

		private string GetCookieValueAndSetIfNullInternal(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				return string.Empty;
			}
			HttpCookie httpCookie = httpContext.Request.Cookies["ClientId"];
			if (httpCookie == null)
			{
				httpCookie = new HttpCookie("ClientId");
				httpCookie.Value = this.GetUniqueClientIdCookieValue();
				httpCookie.HttpOnly = true;
				httpCookie.Expires = DateTime.UtcNow.AddDays(365.0);
				httpContext.Response.Cookies.Add(httpCookie);
			}
			return httpCookie.Value;
		}

		private string GetUniqueClientIdCookieValue()
		{
			Regex regex = new Regex("[^a-z,0,9,A-Z]");
			return regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), string.Empty).ToUpper();
		}

		public const string ClientIdCookieName = "ClientId";

		public const string ClientIdCookieDisplayName = "X-ClientId: ";
	}
}
