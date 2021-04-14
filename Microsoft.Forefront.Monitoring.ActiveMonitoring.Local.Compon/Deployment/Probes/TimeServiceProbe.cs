using System;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Deployment.Probes
{
	public class TimeServiceProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.Result.ExecutionContext = string.Format("TimeServiceProbe started at {0}.\r\n", DateTime.UtcNow);
			using (ServiceController serviceController = new ServiceController("W32Time"))
			{
				if (serviceController.Status != ServiceControllerStatus.Running)
				{
					base.Result.FailureContext = "W32Time service is not running.";
					base.Result.Error = TimeServiceProbe.ProbeErrorMessage;
					throw new Exception(TimeServiceProbe.ProbeErrorMessage);
				}
				ProbeResult result = base.Result;
				result.ExecutionContext += "W32Time service is running.\r\n";
			}
			string text = null;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\W32Time\\Parameters"))
			{
				if (registryKey != null)
				{
					text = (registryKey.GetValue("Type", null) as string);
				}
			}
			if (text != null)
			{
				if (!text.Equals("NT5DS", StringComparison.OrdinalIgnoreCase))
				{
					base.Result.FailureContext = string.Format("W32Time is not synchronizing its time from ActiveDirectory. Value of SYSTEM\\CurrentControlSet\\Services\\W32Time\\Parameters\\Type is {0}. Expected value is NT5DS.", text);
					base.Result.Error = TimeServiceProbe.ProbeErrorMessage;
					throw new Exception(TimeServiceProbe.ProbeErrorMessage);
				}
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += "W32Time service is synchronizing time from ActiveDirectory.\r\n";
			}
			else
			{
				base.Result.FailureContext = "Could not read the settings of W32Time service.";
				base.Result.Error = TimeServiceProbe.ProbeErrorMessage;
			}
			ProbeResult result3 = base.Result;
			result3.ExecutionContext += string.Format("TimeServiceProbe finished at {0}.", DateTime.UtcNow);
		}

		public static readonly string ProbeErrorMessage = "Time Service is not running or misconfigured.";
	}
}
