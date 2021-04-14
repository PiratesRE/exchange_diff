using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Probes
{
	public class DnsServiceProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			using (ServiceController serviceController = new ServiceController("DNS"))
			{
				if (serviceController.Status == ServiceControllerStatus.StartPending)
				{
					serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15.0));
					serviceController.Refresh();
				}
				if (serviceController.Status != ServiceControllerStatus.Running)
				{
					this.LogMessageAndThrow("The '{0}' service is not running; its current status is '{1}'.", new object[]
					{
						"DNS",
						serviceController.Status
					});
				}
			}
			this.LogMessage("The '{0}' service is running.", new object[]
			{
				"DNS"
			});
			cancellationToken.ThrowIfCancellationRequested();
			string text = DnsServiceProbe.AttemptDnsQuery();
			if (text != null)
			{
				this.LogMessageAndThrow("The '{0}' service is running but it did not respond successfully to a trivial nslookup query: {1}", new object[]
				{
					"DNS",
					text
				});
			}
			this.LogMessage("The '{0}' service successfully responded to a trivial nslookup query.", new object[]
			{
				"DNS"
			});
		}

		private static string AttemptDnsQuery()
		{
			string text = null;
			try
			{
				using (Process process = Process.Start(new ProcessStartInfo
				{
					FileName = Environment.SystemDirectory + "\\nslookup.exe",
					Arguments = "127.0.0.1 127.0.0.1",
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				}))
				{
					if (!process.WaitForExit(1000))
					{
						process.Kill();
						text = "nslookup did not exit within 1 second.";
					}
					text = process.StandardError.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				text = ex.Message;
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}

		private void LogMessage(string message, params object[] formatArgs)
		{
			if (formatArgs != null && formatArgs.Length > 0)
			{
				message = string.Format(message, formatArgs);
			}
			NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, message, new object[0]);
		}

		private void LogMessageAndThrow(string message, params object[] formatArgs)
		{
			if (formatArgs != null && formatArgs.Length > 0)
			{
				message = string.Format(message, formatArgs);
			}
			NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, message, new object[0]);
			throw new Exception(message);
		}

		public const string DnsServiceName = "DNS";
	}
}
