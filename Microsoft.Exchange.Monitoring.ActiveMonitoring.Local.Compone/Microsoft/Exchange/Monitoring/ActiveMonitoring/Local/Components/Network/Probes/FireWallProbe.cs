using System;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Probes
{
	public class FireWallProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.localCancellationToken = cancellationToken;
			string text = string.Empty;
			try
			{
				text = Domain.GetComputerDomain().Name;
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, string.Format("This computer is joined to domain {0}", text), new object[0]);
			}
			catch (ActiveDirectoryObjectNotFoundException)
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "This computer is not joined to a domain", new object[0]);
			}
			string text2 = this.RunScript("netsh", "advfirewall show currentprofile state");
			NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Current FireWall configuration of the local machine is {0}", new object[]
			{
				text2
			});
			if (!text2.Contains("ON") && !ExEnvironment.IsTest)
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Current FireWall state is OFF, running script to set it to ON", new object[0]);
				this.RunScript("netsh", "advfirewall set currentprofile state on");
			}
			if (!text2.Contains("Domain") && !string.IsNullOrWhiteSpace(text))
			{
				string text3 = "onenote:https://msft.spoppe.com/teams/exchange/dc/Shared%20Documents/Network/EXO_Network_Guide/Automated%20Escalations.one#FireWallMonitor&section-id={B08EE15E-F03E-4554-BDB3-0E67C681A188}&page-id={6600E52F-8012-4FCD-9741-1810633ED39D}&end";
				string text4 = "https://msft.spoppe.com/teams/exchange/dc/_layouts/OneNote.aspx?id=%2fteams%2fexchange%2fdc%2fShared%20Documents%2fNetwork%2fEXO_Network_Guide&wd=target%28Automated%20Escalations.one%7cB08EE15E-F03E-4554-BDB3-0E67C681A188%2fFireWallMonitor%7c6600E52F-8012-4FCD-9741-1810633ED39D%2f%29";
				string message = string.Format("{0} is not using Domain firewall profile and is joined to domain {1}.<br>Detailed battle card available on: <a href='{2}'>FireWallMonitor</a> (<a href='{3}'>Web view</a>)<br>Current profile: {4}.", new object[]
				{
					Environment.MachineName,
					text,
					text3,
					text4,
					text2.Replace("\r\n", "<br>")
				});
				throw new Exception(message);
			}
			NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "FireWall configuration of the local machine is correct", new object[0]);
		}

		private string RunScript(string command, string arguments)
		{
			if (this.localCancellationToken.IsCancellationRequested)
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "RunScript: Operation is cancelled", new object[0]);
				throw new OperationCanceledException(this.localCancellationToken);
			}
			string result;
			using (Process process = new Process())
			{
				process.StartInfo.FileName = command;
				process.StartInfo.Arguments = arguments;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.Start();
				process.WaitForExit();
				result = process.StandardOutput.ReadToEnd();
			}
			return result;
		}

		private CancellationToken localCancellationToken;
	}
}
