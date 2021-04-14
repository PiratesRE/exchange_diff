using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal interface IUMLocalMonitoringMonitorAndResponder
	{
		void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext);
	}
}
