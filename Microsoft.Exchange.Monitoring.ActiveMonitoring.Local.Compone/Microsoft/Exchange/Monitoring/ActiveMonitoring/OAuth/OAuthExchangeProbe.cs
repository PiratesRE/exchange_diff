using System;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OAuth
{
	public class OAuthExchangeProbe : OAuthPartnerProbe
	{
		internal static ProbeDefinition CreateDefinition(string monitoringUser, string probeName, string targetResourceName, Uri targetEndpoint)
		{
			return OAuthPartnerProbe.CreateDefinition(monitoringUser, probeName, targetResourceName, targetEndpoint, OAuthExchangeProbe.ProbeTypeName);
		}

		protected override OAuthPartnerProbe.ProbeState RunOAuthPartnerProbe()
		{
			string empty = string.Empty;
			ResultType resultType = TestOAuthConnectivityHelper.SendExchangeOAuthRequest(base.MonitoringUser, null, new Uri(base.Definition.Endpoint), out empty, false, false, false);
			base.DiagnosticMessage = empty;
			if (resultType != ResultType.Error)
			{
				return OAuthPartnerProbe.ProbeState.Passed;
			}
			return OAuthPartnerProbe.ProbeState.FailedRequest;
		}

		private static readonly string ProbeTypeName = typeof(OAuthExchangeProbe).FullName;
	}
}
