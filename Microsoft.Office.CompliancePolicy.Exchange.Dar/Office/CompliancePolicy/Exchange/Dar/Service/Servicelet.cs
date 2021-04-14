using System;
using Microsoft.Exchange.ServiceHost;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Service
{
	internal class Servicelet : Servicelet
	{
		public override void Work()
		{
			bool isStarted = false;
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						LogItem.Publish("Runtime", "StartingRuntime");
						if (!HostRpcServer.Start())
						{
							LogItem.Publish("Runtime", "RuntimeStartupFailed", "Failed to start DAR Runtime", ResultSeverityLevel.Critical);
						}
						else
						{
							isStarted = true;
							LogItem.Publish("Runtime", "StartedRuntime");
							this.StopEvent.WaitOne();
						}
					}
					finally
					{
						if (isStarted)
						{
							LogItem.Publish("Runtime", "StoppingRuntime");
							HostRpcServer.Stop();
						}
					}
				});
			}
			catch (GrayException ex)
			{
				LogItem.Publish("Runtime", "RuntimeStartupFailed", "Critical error starting runtime: " + ex.ToString(), ResultSeverityLevel.Critical);
			}
		}
	}
}
