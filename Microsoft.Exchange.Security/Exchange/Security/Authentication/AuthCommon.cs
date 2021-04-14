using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Security.Authentication
{
	internal static class AuthCommon
	{
		public static bool IsFrontEnd
		{
			get
			{
				return !string.IsNullOrEmpty(AuthCommon.HttpProxyProtocolType.Value);
			}
		}

		public static ADRawEntry GetHttpContextADRawEntry(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			return httpContext.Items[AuthCommon.AuthenticatedUserObjectKey] as ADRawEntry;
		}

		public static void SetHttpContextADRawEntry(HttpContext httpContext, ADRawEntry adRawEntry)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			httpContext.Items[AuthCommon.AuthenticatedUserObjectKey] = adRawEntry;
		}

		public static bool IsMultiTenancyEnabled
		{
			get
			{
				return AuthCommon.isMultiTenancyEnabled;
			}
		}

		public static bool IsWindowsLiveIDEnabled
		{
			get
			{
				return AuthCommon.isWindowsLiveIDEnabled;
			}
		}

		internal const string DefaultAnchorMailboxCookieName = "DefaultAnchorMailbox";

		public static readonly string AuthenticatedUserObjectKey = "AuthenticatedUserObjectKey";

		internal static readonly SecurityIdentifier MemberNameNullSid = new SecurityIdentifier(WellKnownSidType.NullSid, null);

		private static readonly bool isMultiTenancyEnabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;

		private static readonly bool isWindowsLiveIDEnabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.WindowsLiveID.Enabled;

		public static readonly StringAppSettingsEntry HttpProxyProtocolType = new StringAppSettingsEntry("HttpProxy.ProtocolType", string.Empty, ExTraceGlobals.AuthenticationTracer);
	}
}
