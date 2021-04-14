using System;
using System.Web;
using Microsoft.Exchange.Autodiscover;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	internal static class ResourceUrlBuilder
	{
		public static string GetResourceUrl(string protocol, string hostName)
		{
			return string.Format("https://{0}{1}", hostName, ResourceUrlBuilder.GetResourceUrlSuffixForProtocol(protocol, hostName));
		}

		public static AutoDiscoverV2Response GetRedirectResponse(RequestDetailsLogger logger, string hostName, string redirectEmailAddress, string protocol, uint currentRedirectCount, string hostNameHint = null)
		{
			string text;
			if (hostNameHint != null)
			{
				text = string.Format("https://{0}/{1}?Email={2}&Protocol={3}&RedirectCount={4}&HostNameHint={5}", new object[]
				{
					hostName,
					"autodiscover/autodiscover.json",
					HttpUtility.UrlEncode(redirectEmailAddress),
					HttpUtility.UrlEncode(protocol),
					currentRedirectCount + 1U,
					hostNameHint
				});
			}
			else
			{
				text = string.Format("https://{0}/{1}?Email={2}&Protocol={3}&RedirectCount={4}", new object[]
				{
					hostName,
					"autodiscover/autodiscover.json",
					HttpUtility.UrlEncode(redirectEmailAddress),
					HttpUtility.UrlEncode(protocol),
					currentRedirectCount + 1U
				});
			}
			logger.AppendGenericInfo("GetRedirectUrlRedirectAbsoluteUrl", text);
			return new AutoDiscoverV2Response
			{
				RedirectUrl = text
			};
		}

		private static string GetResourceUrlSuffixForProtocol(string protocol, string hostName)
		{
			SupportedProtocol supportedProtocol;
			Enum.TryParse<SupportedProtocol>(protocol, true, out supportedProtocol);
			switch (supportedProtocol)
			{
			case SupportedProtocol.Unknown:
				throw AutoDiscoverResponseException.BadRequest("InvalidProtocol", string.Format("The given protocol value '{0}' is invalid. Supported values are '{1}'", protocol, "ActiveSync, Ews"), null);
			case SupportedProtocol.Rest:
				throw AutoDiscoverResponseException.NotFound("MailboxNotEnabledForRESTAPI", "REST API is not yet supported for this mailbox.", null);
			case SupportedProtocol.ActiveSync:
				return "/Microsoft-Server-ActiveSync";
			case SupportedProtocol.Ews:
				return "/EWS/Exchange.asmx";
			default:
				return null;
			}
		}

		internal const string ActiveSyncUrlSuffix = "/Microsoft-Server-ActiveSync";

		internal const string WebServiceUrlSuffix = "/EWS/Exchange.asmx";

		internal const string EmailRedirectUrlSuffix = "autodiscover/autodiscover.json";
	}
}
