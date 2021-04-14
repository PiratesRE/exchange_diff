using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Security
{
	public sealed class PowerShellProfileProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateProbeDefinition(string probeName, Type probeType, string mask, int recurrenceIntervalSeconds, int maxRetryApptempts, bool enabled)
		{
			return new ProbeDefinition
			{
				AssemblyPath = probeType.Assembly.Location,
				TypeName = probeType.FullName,
				Name = probeName,
				TargetResource = mask,
				RecurrenceIntervalSeconds = recurrenceIntervalSeconds,
				TimeoutSeconds = recurrenceIntervalSeconds,
				MaxRetryAttempts = maxRetryApptempts,
				ServiceName = ExchangeComponent.Security.Name,
				Enabled = enabled
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "PowerShellProfileProbe:: DoWork(): Started Execution.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\PowerShellProfileProbe.cs", 71);
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
			string str = folderPath + "\\system32\\WindowsPowerShell\\v1.0\\";
			List<string> list = new List<string>
			{
				str + "profile.ps1",
				str + "Microsoft.PowerShell_profile.ps1"
			};
			foreach (string text in list)
			{
				if (File.Exists(text))
				{
					flag = true;
					stringBuilder.Append(string.Format("Unexpected Powershell profile exists at {0}", text));
				}
			}
			if (flag)
			{
				base.Result.StateAttribute11 = stringBuilder.ToString();
				throw new Exception(stringBuilder.ToString());
			}
			base.Result.StateAttribute11 = "No powershell profiles were found";
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SecurityTracer, base.TraceContext, "PowerShellProfileProbe:: DoWork(): End Execution.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Security\\PowerShellProfileProbe.cs", 92);
		}
	}
}
