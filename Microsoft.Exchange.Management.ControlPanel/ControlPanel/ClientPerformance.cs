using System;
using System.Linq;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ClientPerformance : IClientPerformanceService
	{
		public string ReportWatson(ClientWatson report)
		{
			if (report != null)
			{
				try
				{
					ClientExceptionLogger.Instance.LogEvent(new ClientExceptionLoggerEvent(report));
				}
				catch
				{
				}
			}
			return "OK";
		}

		public bool LogClientDatapoint(Datapoint[] datapoints)
		{
			if (datapoints == null)
			{
				return false;
			}
			ClientLogger.Instance.LogEvent((from c in datapoints
			where c != null
			select new ClientLogEvent(c)).ToArray<ClientLogEvent>());
			return true;
		}
	}
}
