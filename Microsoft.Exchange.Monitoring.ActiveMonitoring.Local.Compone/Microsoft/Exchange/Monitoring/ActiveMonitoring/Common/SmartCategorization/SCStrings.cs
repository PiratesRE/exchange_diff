using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common.SmartCategorization
{
	internal static class SCStrings
	{
		public static readonly string UnrecognizedFailure = "Unrecognized failure.";

		public static readonly string FailureDetails = "{0} failure in \"{1}\" component.";

		public static readonly string FailureFrontendBackendAuthN = "Cafe could not authenticate itself with the mailbox server. Potentially an issue with AD cross-forest trust or authentication on the backend.";

		public static readonly string FailureActiveDirectory = "An error occured during an active directory operation. Look at the request failure context below or the protocol logs for the full details and contact Directory team if required.";

		public static readonly string FailureServerLocator = "Cafe could not lookup the user's backend to proxy to due to an error in server locator. Contact Directory/LiveId team for assistance.";

		public static readonly string FailureLiveId = "The monitoring account failed to authenticate with Cafe due to a {0} error in the LiveIdBasic auth module. Contact Directory/LiveId team for assistance.";

		public static readonly string FailureMonitoringAccount = "The monitoring account failed to authenticate with Cafe due to AuthFailure error. Monitoring account is likely misconfigured - contact monitoring team for assistance.";
	}
}
