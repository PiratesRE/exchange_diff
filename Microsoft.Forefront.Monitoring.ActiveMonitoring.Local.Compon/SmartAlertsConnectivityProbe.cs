using System;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class SmartAlertsConnectivityProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				this.CallRemotePowerShell();
			}
			catch (AggregateException ex)
			{
				StringBuilder sb = new StringBuilder();
				int innerExceptionCount = 0;
				ex.Handle(delegate(Exception x)
				{
					string text = x.Message;
					Match match = Regex.Match(text, "Failed to invoke via endpoint 'https://(?<IPAddr>[^/]+)[\\s\\S]+'https://(?<HostName>[^/]+)");
					if (match.Success)
					{
						sb.AppendFormat(" ### Error[{0}] IP '{1}' ( Host '{2}' ) ", innerExceptionCount, match.Groups["IPAddr"].Value, match.Groups["HostName"].Value);
					}
					else
					{
						int num = text.IndexOf("-->");
						num = ((num < 0) ? text.Length : num);
						text = text.Substring(0, Math.Min(num, 200));
						sb.AppendFormat(" Error[{0}] {1} ", innerExceptionCount, text);
					}
					innerExceptionCount++;
					return true;
				});
				sb.AppendLine("#####");
				sb.Insert(0, "Exceptions Total: " + innerExceptionCount);
				throw new Exception(sb.ToString(), ex.Flatten().InnerException);
			}
			ProbeResult result = base.Result;
			result.ExecutionContext += "SmartAlertsConnectivityProbe finished.";
		}

		private void CallRemotePowerShell()
		{
			string text;
			string text2;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(SmartAlertsConnectivityProbe.ActiveMonitoringRegistryPath, false))
			{
				if (registryKey == null)
				{
					throw new Exception("CallRemotePowerShell can't get registry path for RPS.");
				}
				text = (string)registryKey.GetValue("RPSCertificateSubject", null);
				text2 = (string)registryKey.GetValue("RPSEndpoint", null);
				if (text == null || text2 == null)
				{
					throw new Exception("CallRemotePowerShell RPSCertificateSubject or RPSEndpoint is null.");
				}
				ProbeResult result = base.Result;
				result.ExecutionContext += string.Format("RPSEndpoints:'{0}'. ", text2);
			}
			RemotePowerShell remotePowerShell;
			if (text2.Contains(";"))
			{
				remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCertificate(text2.Split(new char[]
				{
					';'
				}), text, true);
			}
			else
			{
				remotePowerShell = RemotePowerShell.CreateRemotePowerShellByCertificate(new Uri(text2), text, true);
			}
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Get-OnCallRotation");
			pscommand.AddParameter("Filter", "Team -eq 'Deployment'");
			remotePowerShell.InvokePSCommand(pscommand);
		}

		private static readonly string ActiveMonitoringRegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\";
	}
}
