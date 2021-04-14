using System;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.Exchange.Net.MapiHttp
{
	public static class MapiHttpEndpoints
	{
		public static string GetMailboxUrl(string fqdn, string mailboxId)
		{
			return string.Format("https://{0}{1}?{2}={3}", new object[]
			{
				fqdn,
				MapiHttpEndpoints.VdirPathEmsmdb,
				"MailboxId",
				mailboxId
			});
		}

		public static string GetMailboxUrl(string fqdn)
		{
			return string.Format("https://{0}{1}", fqdn, MapiHttpEndpoints.VdirPathEmsmdb);
		}

		public static string GetAddressBookUrl(string fqdn, string mailboxId)
		{
			return string.Format("https://{0}{1}?{2}={3}", new object[]
			{
				fqdn,
				MapiHttpEndpoints.VdirPathNspi,
				"MailboxId",
				mailboxId
			});
		}

		public static string GetAddressBookUrl(string fqdn)
		{
			return string.Format("https://{0}{1}", fqdn, MapiHttpEndpoints.VdirPathNspi);
		}

		public static string GetClientRequestInfo(HttpContext context)
		{
			return MapiHttpEndpoints.GetClientRequestInfo(context.Request.Headers);
		}

		public static string GetClientRequestInfo(NameValueCollection headers)
		{
			return string.Concat(new string[]
			{
				"R:",
				headers.GetHeaderValue("X-RequestId"),
				";CI:",
				headers.GetHeaderValue("X-ClientInfo"),
				";RT:",
				headers.GetHeaderValue("X-RequestType")
			});
		}

		private static string GetHeaderValue(this NameValueCollection headers, string header)
		{
			string text = headers[header];
			if (text == null)
			{
				return "<null>";
			}
			if (string.IsNullOrEmpty(text))
			{
				return "<empty>";
			}
			return text;
		}

		private const string HeaderRequestId = "X-RequestId";

		private const string HeaderClientInfo = "X-ClientInfo";

		private const string HeaderRequestType = "X-RequestType";

		internal const string VdirPathMapi = "/mapi/";

		internal const string ParameterMailboxId = "MailboxId";

		internal const string ParameterShowDebug = "ShowDebug";

		internal const string ParameterShowDebugActivationValue = "yes";

		internal static readonly string VdirPathEmsmdb = "/mapi/emsmdb/";

		internal static readonly string VdirPathNspi = "/mapi/nspi/";
	}
}
