using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class RidSetReferencesMonitorProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestRidSetMonitor, delegate
			{
				if (!DirectoryUtils.IsRidMaster())
				{
					base.Result.StateAttribute5 = "This DC is not a RID master.  Probe will be skipped.";
					return;
				}
				bool flag = false;
				StringBuilder stringBuilder = new StringBuilder();
				using (DirectoryEntry directoryEntry = new DirectoryEntry())
				{
					using (DirectoryEntry directoryEntry2 = new DirectoryEntry("LDAP://OU=Domain Controllers," + directoryEntry.Properties["distinguishedName"].Value.ToString()))
					{
						foreach (object obj in directoryEntry2.Children)
						{
							DirectoryEntry directoryEntry3 = (DirectoryEntry)obj;
							using (DirectoryEntry directoryEntry4 = new DirectoryEntry("LDAP://CN=RID Set," + directoryEntry3.Properties["distinguishedName"].Value.ToString()))
							{
								if (directoryEntry4 != null && directoryEntry3.Properties["rIDSetReferences"] == null)
								{
									flag = true;
									stringBuilder.AppendLine(string.Format("{0} is missing RidSetReferences", directoryEntry3.Properties["dNSHostName"].Value.ToString()));
								}
							}
						}
					}
				}
				base.Result.Error = stringBuilder.ToString();
				base.Result.StateAttribute1 = stringBuilder.ToString();
				WTFDiagnostics.TraceInformation<bool, double, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", !flag, base.Result.SampleValue, base.Result.StateAttribute1, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\RidSetReferencesMonitorProbe.cs", 79);
				if (flag)
				{
					throw new Exception(stringBuilder.ToString());
				}
			});
		}
	}
}
