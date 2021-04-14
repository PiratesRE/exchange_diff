using System;
using System.Web;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public static class OwaUserAgentUtilities
	{
		public static UserAgent CreateUserAgentAnonymous(HttpContext context)
		{
			return OwaUserAgentUtilities.CreateUserAgentWithLayoutOverride(context, OwaUserAgentUtilities.GetLayoutString(context), false);
		}

		public static UserAgent CreateUserAgentWithLayoutOverride(HttpContext context)
		{
			return OwaUserAgentUtilities.CreateUserAgentWithLayoutOverride(context, OwaUserAgentUtilities.GetLayoutString(context), true);
		}

		public static UserAgent CreateUserAgentWithLayoutOverride(HttpContext context, string layout, bool userContextAvailable = true)
		{
			bool changeLayoutFeatureEnabled = false;
			if (userContextAvailable)
			{
				try
				{
					changeLayoutFeatureEnabled = UserContextManager.GetUserContext(context).FeaturesManager.ClientServerSettings.ChangeLayout.Enabled;
				}
				catch (OwaLockException)
				{
				}
				catch (OwaIdentityException)
				{
				}
				catch (NullReferenceException)
				{
				}
			}
			UserAgent userAgent = new UserAgent(context.Request.UserAgent, changeLayoutFeatureEnabled, context.Request.Cookies);
			userAgent.SetLayoutFromString(layout);
			return userAgent;
		}

		private static string GetLayoutString(HttpContext context)
		{
			if (RequestDispatcherUtilities.GetStringUrlParameter(context, "sharepointapp") == "true")
			{
				return "mouse";
			}
			string text = RequestDispatcherUtilities.GetStringUrlParameter(context, "layout");
			if (string.IsNullOrEmpty(text))
			{
				text = OwaUserAgentUtilities.GetAppCacheManiestLayoutCookieValue(context);
			}
			return text;
		}

		private static string GetAppCacheManiestLayoutCookieValue(HttpContext context)
		{
			HttpCookie httpCookie = context.Request.Cookies["ManifestLayout"];
			return (httpCookie != null) ? httpCookie.Value : null;
		}

		public const string SharepointAppParamName = "sharepointapp";

		public const string ManifestLayoutCookieName = "ManifestLayout";
	}
}
