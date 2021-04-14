using System;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OAuth
{
	public class OAuthLyncProbe : OAuthPartnerProbe
	{
		internal static ProbeDefinition CreateDefinition(string monitoringUser, string probeName, string targetResourceName, Uri targetEndpoint)
		{
			return OAuthPartnerProbe.CreateDefinition(monitoringUser, probeName, targetResourceName, targetEndpoint, OAuthLyncProbe.ProbeTypeName);
		}

		protected override OAuthPartnerProbe.ProbeState RunOAuthPartnerProbe()
		{
			string empty = string.Empty;
			Uri targetUri = string.IsNullOrEmpty(base.Definition.Endpoint) ? null : new Uri(base.Definition.Endpoint);
			ResultType resultType = TestOAuthConnectivityHelper.SendLyncOAuthRequest(base.MonitoringUser, targetUri, out empty, false, false, false);
			base.DiagnosticMessage = empty;
			if (resultType != ResultType.Error)
			{
				return OAuthPartnerProbe.ProbeState.Passed;
			}
			return OAuthPartnerProbe.ProbeState.FailedRequest;
		}

		private static readonly string ProbeTypeName = typeof(OAuthLyncProbe).FullName;
	}
}
