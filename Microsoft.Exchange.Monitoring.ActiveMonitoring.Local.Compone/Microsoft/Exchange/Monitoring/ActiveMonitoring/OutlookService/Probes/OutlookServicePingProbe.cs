using System;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Office.Outlook.V1.Mail;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OutlookService.Probes
{
	public class OutlookServicePingProbe : OutlookServiceSocketProbeBase
	{
		public OutlookServicePingProbe()
		{
			base.Type = 0;
			base.Timeout = TimeSpan.FromSeconds(50.0);
		}

		protected override void ExecuteRequest(SocketClient client)
		{
			client.Ping(new PingRequest(), base.Timeout);
			base.Result.StateAttribute13 = client.ExtraInfo;
			if (client.ExecutionSuccess)
			{
				base.Result.ResultType = ResultType.Succeeded;
				return;
			}
			base.Result.ResultType = ResultType.Failed;
			throw new Exception(string.Format("PingProbe execution failed, Details : {0}", client.ExtraInfo));
		}
	}
}
