using System;
using System.Security.Principal;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class OwaUtils
	{
		internal static void AddBackendAuthenticationParameters(ProbeDefinition probe, string userSid, string monitoringDomain)
		{
			CommonAccessToken commonAccessToken = null;
			if (!Datacenter.IsMicrosoftHostedOnly(true))
			{
				using (WindowsIdentity windowsIdentity = new WindowsIdentity(probe.Account))
				{
					commonAccessToken = CommonAccessTokenHelper.CreateWindows(windowsIdentity);
					goto IL_54;
				}
			}
			if (userSid == null)
			{
				throw new ArgumentNullException(string.Format("Probe {0} cannot be instantiated because SID for user {1} is null", probe.Name, probe.Account));
			}
			commonAccessToken = CommonAccessTokenHelper.CreateLiveId(probe.Account);
			IL_54:
			probe.Attributes["CommonAccessToken"] = commonAccessToken.Serialize();
		}

		internal static AuthenticationParameters ReadBackendAuthenticationParameters(ProbeDefinition probeDefinition)
		{
			string valueOrDefault = probeDefinition.Attributes.GetValueOrDefault("CommonAccessToken", null);
			if (valueOrDefault != null)
			{
				CommonAccessToken commonAccessToken = CommonAccessToken.Deserialize(valueOrDefault);
				return new AuthenticationParameters
				{
					CommonAccessToken = commonAccessToken
				};
			}
			return null;
		}

		internal const string CommonAccessTokenParameterName = "CommonAccessToken";
	}
}
