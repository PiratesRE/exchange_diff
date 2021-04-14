using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class NtlmConnectivityProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestNtlmConnectivity, delegate
			{
				bool flag = false;
				StringBuilder stringBuilder = new StringBuilder();
				string args = string.Format("/SERVER:{0} /SC_QUERY:{1}", Environment.MachineName, TopologyProvider.LocalForestFqdn);
				StreamReader @null = StreamReader.Null;
				string empty = string.Empty;
				string text = "No DC Found";
				string text2 = this.RunNltest(args, out empty);
				stringBuilder.AppendLine(empty);
				if (text2.Contains("Trusted DC Name \\\\"))
				{
					string text3 = text2.Substring(text2.IndexOf("Trusted DC Name \\\\", StringComparison.CurrentCulture) + "Trusted DC Name \\\\".Length);
					text = text3.Substring(0, text2.IndexOf(" \r\nTrusted DC Connection Status Status", StringComparison.CurrentCulture));
				}
				if (!text2.Contains("Trusted DC Connection Status Status = 0 0x0 NERR_Success"))
				{
					stringBuilder.AppendLine("Nltest failed, will be attempting recovery. Results: " + text2);
					args = string.Format("/SERVER:{0} /SC_VERIFY:{1}", Environment.MachineName, TopologyProvider.LocalForestFqdn);
					text2 = this.RunNltest(args, out empty);
					stringBuilder.AppendLine("Recovery Results:" + empty);
				}
				else
				{
					stringBuilder.AppendLine("Nltest successful and the DC being used is " + text.Trim());
					flag = true;
				}
				base.Result.Error = stringBuilder.ToString();
				base.Result.StateAttribute1 = stringBuilder.ToString();
				WTFDiagnostics.TraceInformation<bool, double, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", flag, base.Result.SampleValue, base.Result.StateAttribute1, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\NtlmConnectivityProbe.cs", 86);
				if (!flag)
				{
					throw new Exception(base.Result.Error);
				}
			});
		}

		private string RunNltest(string args, out string log)
		{
			StreamReader streamReader = StreamReader.Null;
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.Empty;
			int num = 30000;
			string result;
			using (Process process = new Process
			{
				StartInfo = 
				{
					FileName = "nltest",
					Arguments = args,
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true
				}
			})
			{
				try
				{
					process.Start();
					streamReader = process.StandardOutput;
					text = streamReader.ReadToEnd();
					if (!process.WaitForExit(num))
					{
						stringBuilder.AppendLine("Nltest timeout after " + num);
					}
					else
					{
						stringBuilder.AppendLine("Nltest successful");
					}
				}
				catch (Exception ex)
				{
					stringBuilder.AppendLine("Nltest failed with exception  " + ex.ToString());
				}
				log = stringBuilder.ToString();
				result = text;
			}
			return result;
		}
	}
}
