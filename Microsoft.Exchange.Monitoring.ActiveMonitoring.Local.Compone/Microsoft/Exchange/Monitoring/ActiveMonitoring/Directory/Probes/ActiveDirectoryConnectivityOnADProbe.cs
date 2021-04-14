using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class ActiveDirectoryConnectivityOnADProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestActiveDirectorySelfCheck, delegate
			{
				int num = 400;
				string hostName = Dns.GetHostName();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting AD self check connectivity check against server {0}", hostName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\ActiveDirectoryConnectivityOnADProbe.cs", 56);
				string path = string.Format("LDAP://{0}/RootDSE", hostName);
				Stopwatch stopwatch;
				using (DirectoryEntry directoryEntry = new DirectoryEntry(path))
				{
					string arg = string.Empty;
					if (directoryEntry == null || directoryEntry.Properties == null)
					{
						throw new Exception("Check NTDS or DC Health as its not responding to ldap connections");
					}
					arg = directoryEntry.Properties["defaultNamingContext"].Value.ToString();
					path = string.Format("LDAP://{0}/{1}", hostName, arg);
					using (DirectoryEntry directoryEntry2 = new DirectoryEntry(path))
					{
						using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry))
						{
							stopwatch = Stopwatch.StartNew();
							directorySearcher.SearchRoot = directoryEntry2;
							directorySearcher.FindAll();
							stopwatch.Stop();
						}
					}
				}
				base.Result.SampleValue = (double)stopwatch.ElapsedMilliseconds;
				if (base.Result.SampleValue > (double)num)
				{
					throw new Exception(string.Format(" Search took {0} ms. Threshold {1} ms", base.Result.SampleValue, num));
				}
				WTFDiagnostics.TraceInformation<bool, double, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", true, base.Result.SampleValue, base.Result.StateAttribute1, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\ActiveDirectoryConnectivityOnADProbe.cs", 97);
			});
		}
	}
}
