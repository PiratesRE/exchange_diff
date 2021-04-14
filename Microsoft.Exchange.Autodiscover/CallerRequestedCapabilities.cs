using System;
using System.Web;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.Autodiscover.WCF;

namespace Microsoft.Exchange.Autodiscover
{
	internal class CallerRequestedCapabilities
	{
		protected CallerRequestedCapabilities(bool canFollowRedirect, bool canHandleExHttpNodesInResponse, Version outlookClientVersion, bool isCallerCrossForestAvailabilityService, bool isWindow7OrNewerClient, bool supportsNego, bool supportsNegoEx)
		{
			this.CanFollowRedirect = canFollowRedirect;
			this.CanHandleExHttpNodesInAutoDiscResponse = canHandleExHttpNodesInResponse;
			this.OutlookClientVersion = outlookClientVersion;
			this.IsCallerCrossForestAvailabilityService = isCallerCrossForestAvailabilityService;
			this.IsWindow7OrNewerClient = isWindow7OrNewerClient;
			this.CanHandleNegotiateCorrectly = supportsNego;
			if (supportsNegoEx && !supportsNego)
			{
				throw new ArgumentException("You cannot specify only NegotiateEx, Negotiate must also be supported or none of them!");
			}
			this.CanHandleNegotiateEx = supportsNegoEx;
		}

		public static CallerRequestedCapabilities GetInstance(HttpContext requestHttpContext)
		{
			if (requestHttpContext == null)
			{
				throw new ArgumentNullException("requestHttpContext", "You must call this GetInstance method only in the cases where an HttpContext is available");
			}
			Version requestingClientVersion = null;
			Version windowsVersion = null;
			string text = Common.SafeGetUserAgent(requestHttpContext.Request);
			bool canFollowRedirect = !AutodiscoverProxy.CanRedirectOutlookClient(text);
			bool canHandleExHttpNodesInResponse = UserAgentHelper.IsWindowsClient(text);
			bool flag = UserAgentHelper.ValidateClientSoftwareVersions(text, delegate(int major, int minor)
			{
				windowsVersion = new Version(major, minor);
				return true;
			}, delegate(int major, int minor, int buildMajor)
			{
				requestingClientVersion = new Version(major, minor, buildMajor, 0);
				return true;
			}) && windowsVersion != null && requestingClientVersion != null;
			bool isCallerCrossForestAvailabilityService = !string.IsNullOrWhiteSpace(text) && text.StartsWith("ASAutoDiscover/CrossForest", StringComparison.OrdinalIgnoreCase) && requestHttpContext.User != null && requestHttpContext.User.Identity is ExternalIdentity;
			bool flag2 = flag && UserAgentHelper.IsClientWin7OrGreater(text);
			bool flag3 = false;
			bool flag4 = false;
			if (flag2 && CallerRequestedCapabilities.CheckIfClientSupportsNegotiate(requestingClientVersion, out flag3))
			{
				flag4 = true;
			}
			OptInCapabilities optInCapabilities;
			if (CallerRequestedCapabilities.TryParseOptInCapabillitiesHeader(requestHttpContext, out optInCapabilities))
			{
				if ((optInCapabilities & OptInCapabilities.Negotiate) != OptInCapabilities.None)
				{
					flag4 = true;
				}
				if ((optInCapabilities & OptInCapabilities.ExHttpInfo) != OptInCapabilities.None)
				{
					canHandleExHttpNodesInResponse = true;
				}
			}
			return new CallerRequestedCapabilities(canFollowRedirect, canHandleExHttpNodesInResponse, requestingClientVersion, isCallerCrossForestAvailabilityService, flag2, flag4, flag4 && flag3);
		}

		public bool CanFollowRedirect { get; private set; }

		public bool CanHandleExHttpNodesInAutoDiscResponse { get; private set; }

		public Version OutlookClientVersion { get; private set; }

		public bool IsCallerCrossForestAvailabilityService { get; private set; }

		public bool IsWindow7OrNewerClient { get; private set; }

		public bool CanHandleNegotiateCorrectly { get; private set; }

		public bool CanHandleNegotiateEx { get; private set; }

		private static bool CheckIfClientSupportsNegotiate(Version clientVersion, out bool useNego2)
		{
			useNego2 = false;
			if (clientVersion.Major > 14 || (clientVersion.Major == 14 && clientVersion.Build >= 5133) || (clientVersion.Major == 12 && clientVersion.Build >= 6552))
			{
				return true;
			}
			if (clientVersion.Major == 14)
			{
				useNego2 = true;
				return true;
			}
			return false;
		}

		private static bool TryParseOptInCapabillitiesHeader(HttpContext requestHttpContext, out OptInCapabilities optInCapabilities)
		{
			optInCapabilities = OptInCapabilities.None;
			string text = requestHttpContext.Request.Headers[CallerRequestedCapabilities.CapabillitiesHeaderName];
			if (!string.IsNullOrWhiteSpace(text))
			{
				bool result = false;
				foreach (string value in text.Split(new char[]
				{
					' '
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					OptInCapabilities optInCapabilities2;
					if (Enum.TryParse<OptInCapabilities>(value, true, out optInCapabilities2))
					{
						optInCapabilities |= optInCapabilities2;
						result = true;
					}
				}
				return result;
			}
			return false;
		}

		private static readonly string CapabillitiesHeaderName = "X-ClientCanHandle";
	}
}
