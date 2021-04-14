using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class InvokeNowCrimsonHelper
	{
		internal static CrimsonReader<InvokeNowEntry> PrepareReader(RecoveryActionId actionId, string resourceName, string instanceId, RecoveryActionState state, RecoveryActionResult result, DateTime startTime, DateTime endTime, string xpathQueryString = null)
		{
			CrimsonReader<InvokeNowEntry> crimsonReader = new CrimsonReader<InvokeNowEntry>(null, null, "Microsoft-Exchange-ManagedAvailability/InvokeNowRequest");
			crimsonReader.IsReverseDirection = true;
			if (string.IsNullOrEmpty(xpathQueryString))
			{
				crimsonReader.QueryStartTime = new DateTime?(startTime);
				crimsonReader.QueryEndTime = new DateTime?(endTime);
			}
			else
			{
				crimsonReader.ExplicitXPathQuery = xpathQueryString;
			}
			return crimsonReader;
		}

		internal const string InvokeNowRequestChannelName = "Microsoft-Exchange-ManagedAvailability/InvokeNowRequest";

		internal const string InvokeNowResultChannelName = "Microsoft-Exchange-ManagedAvailability/InvokeNowResult";

		internal const int InvokeNowRequestUploadStarted = 2002;

		internal const int InvokeNowRequestUploadSucceeded = 2003;

		internal const int InvokeNowRequestUploadFailed = 2004;

		internal enum InvokeNowChannelType
		{
			Request,
			Result
		}
	}
}
