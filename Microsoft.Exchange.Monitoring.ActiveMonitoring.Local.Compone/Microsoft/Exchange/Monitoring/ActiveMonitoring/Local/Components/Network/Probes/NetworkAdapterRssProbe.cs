using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Probes
{
	internal class NetworkAdapterRssProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.localCancellationToken = cancellationToken;
			string text = this.NetworkAdapterRssState("interface tcp show global", "TCP Global Parameters could be obtained successfully.");
			string[] source = text.Split(new char[]
			{
				'\n'
			});
			if (source.Any((string o) => o.IndexOf("The TCP parameters differ between IPv4 and IPv6.", StringComparison.Ordinal) != -1))
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "The TCP parameters differ between IPv4 and IPv6. Probe will throw error, so that responder can fix it.", new object[0]);
				throw new Exception("The TCP parameters differ between IPv4 and IPv6. Probe will throw error, so that responder can fix it.");
			}
			string text2 = (from o in source
			where o.IndexOf("Receive-Side Scaling State", StringComparison.Ordinal) != -1
			select o into s
			select s.Substring(s.IndexOf(':') + 1)).First<string>();
			bool flag = text2.ToLowerInvariant().Contains("enabled");
			text2 = text2.Trim(new char[]
			{
				'\r'
			});
			if (!flag)
			{
				this.NetworkAdapterRssState("interface tcp set global rss=enabled", "RSS state has been set to enabled.");
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, string.Format("Network Adapter RSS state is {0}, but it should have been enabled. Probe will throw error, so that responder can fix it.", text2), new object[0]);
				throw new Exception(string.Format("Network Adapter RSS state is {0}, but it should have been enabled. Probe will throw error after setting RSS state to enabled via netsh, so that responder can fix it.", text2));
			}
			NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "RSS is already enabled, no need to throw. Returning...", new object[0]);
		}

		private string NetworkAdapterRssState(string processStartInfoArgument, string message)
		{
			string output = "Process cancelled";
			Task task = new Task(delegate()
			{
				ProcessStartInfo startInfo = new ProcessStartInfo("netsh", processStartInfoArgument)
				{
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};
				using (Process process = new Process())
				{
					process.StartInfo = startInfo;
					process.Start();
					process.WaitForExit(10000);
					output = process.StandardOutput.ReadToEnd();
					NetworkUtils.LogWorkItemMessage(this.TraceContext, this.Result, (process.ExitCode == 0) ? string.Format("{0} {1}", message, output) : process.StandardError.ReadToEnd(), new object[0]);
				}
			});
			task.Start();
			try
			{
				task.Wait(this.localCancellationToken);
			}
			catch (OperationCanceledException)
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "NetworkAdapterRssState: Operation is cancelled", new object[0]);
				throw;
			}
			return output;
		}

		private CancellationToken localCancellationToken;
	}
}
