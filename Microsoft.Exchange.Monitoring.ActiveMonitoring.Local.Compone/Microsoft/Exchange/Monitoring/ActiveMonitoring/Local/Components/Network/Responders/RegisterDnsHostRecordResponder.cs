using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Responders
{
	public class RegisterDnsHostRecordResponder : ResponderWorkItem
	{
		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = Environment.SystemDirectory + "\\ipconfig.exe",
				Arguments = "/registerdns"
			};
			using (Process process = Process.Start(startInfo))
			{
				process.WaitForExit(30000);
			}
		}
	}
}
