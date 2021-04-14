using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Services.OnlineMeetings.Autodiscover.DataContract;
using Microsoft.Exchange.Services.OnlineMeetings.ResourceContract;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover
{
	internal static class AutodiscoverExtensions
	{
		internal static string RootRedirectUrl(this AutodiscoverResponse thisObject)
		{
			return AutodiscoverExtensions.GetRootLinkToken(thisObject, "redirect");
		}

		internal static string RootOAuthToken(this AutodiscoverResponse thisObject)
		{
			return AutodiscoverExtensions.GetRootLinkToken(thisObject, "oauth");
		}

		internal static string UserRedirectUrl(this AutodiscoverResponse thisObject)
		{
			string userLinkToken = AutodiscoverExtensions.GetUserLinkToken(thisObject, "redirect");
			if (string.IsNullOrEmpty(userLinkToken))
			{
				return thisObject.RootOAuthToken();
			}
			return userLinkToken;
		}

		internal static string UserExternalUcwaToken(this AutodiscoverResponse thisObject)
		{
			return AutodiscoverExtensions.GetUserLinkToken(thisObject, "external/ucwa");
		}

		internal static string UserInternalUcwaToken(this AutodiscoverResponse thisObject)
		{
			return AutodiscoverExtensions.GetUserLinkToken(thisObject, "internal/ucwa");
		}

		internal static string GetRequestHeadersAsString(this WebRequest thisObject)
		{
			if (thisObject != null)
			{
				return thisObject.Headers.GetWebHeaderCollectionAsString();
			}
			return "Count:0";
		}

		internal static string GetResponseHeadersAsString(this WebResponse thisObject)
		{
			if (thisObject != null)
			{
				return thisObject.Headers.GetWebHeaderCollectionAsString();
			}
			return "Count:0";
		}

		internal static string GetWebHeaderCollectionAsString(this WebHeaderCollection thisObject)
		{
			if (thisObject != null)
			{
				StringBuilder stringBuilder = new StringBuilder(string.Format("Count:{0}", thisObject.Count));
				try
				{
					foreach (object obj in thisObject.Keys)
					{
						string text = (string)obj;
						string arg = (string.Compare(text, WellKnownHeader.Authorization, StringComparison.OrdinalIgnoreCase) != 0) ? thisObject[text] : "[redacted]";
						stringBuilder.AppendFormat("\n{0}:{1}", text, arg);
					}
				}
				catch (InvalidOperationException)
				{
				}
				return stringBuilder.ToString();
			}
			return "Count:0";
		}

		internal static string GetResponseBodyAsString(this WebResponse thisObject)
		{
			if (thisObject != null)
			{
				using (Stream responseStream = thisObject.GetResponseStream())
				{
					if (responseStream != null)
					{
						using (StreamReader streamReader = new StreamReader(responseStream))
						{
							responseStream.Seek(0L, SeekOrigin.Begin);
							return streamReader.ReadToEnd();
						}
					}
				}
			}
			return string.Empty;
		}

		private static string GetUserLinkToken(AutodiscoverResponse response, string tokenName)
		{
			if (string.IsNullOrEmpty(tokenName))
			{
				return string.Empty;
			}
			IEnumerable<string> source = from links in response.UserLinks
			where string.Compare(links.Token.ToLowerInvariant(), tokenName.ToLowerInvariant(), StringComparison.Ordinal) == 0
			select links.Href;
			return source.FirstOrDefault<string>();
		}

		private static string GetRootLinkToken(AutodiscoverResponse response, string tokenName)
		{
			if (string.IsNullOrEmpty(tokenName))
			{
				return string.Empty;
			}
			IEnumerable<string> source = from links in response.RootLinks
			where string.Compare(links.Token.ToLowerInvariant(), tokenName.ToLowerInvariant(), StringComparison.Ordinal) == 0
			select links.Href;
			return source.FirstOrDefault<string>();
		}

		private const string ZeroCountString = "Count:0";

		private const string RedirectToken = "redirect";

		private const string OAuthToken = "oauth";

		private const string ExternalUcwaToken = "external/ucwa";

		private const string InternalUcwaToken = "internal/ucwa";
	}
}
