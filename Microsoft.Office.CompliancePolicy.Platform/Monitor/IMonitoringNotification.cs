using System;

namespace Microsoft.Office.CompliancePolicy.Monitor
{
	public interface IMonitoringNotification
	{
		void PublishEvent(string componentName, string organization, string context, Exception exception);
	}
}
