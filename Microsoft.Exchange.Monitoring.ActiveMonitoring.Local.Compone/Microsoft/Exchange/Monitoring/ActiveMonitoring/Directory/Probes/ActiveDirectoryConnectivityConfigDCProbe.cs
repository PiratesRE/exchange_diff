using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class ActiveDirectoryConnectivityConfigDCProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (propertyBag.ContainsKey("ADConnectivityThreshold"))
			{
				pDef.Attributes["ADConnectivityThreshold"] = propertyBag["ADConnectivityThreshold"].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value forADConnectivityThreshold");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestActivedirectoryConnectivityForConfigDC, delegate
			{
				string arg = new ServerIdParameter().ToString();
				if (!base.Definition.Attributes.ContainsKey("ADConnectivityThreshold"))
				{
					throw new ArgumentException("ADConnectivityThreshold");
				}
				int num = int.Parse(base.Definition.Attributes["ADConnectivityThreshold"]);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting ADDriver availability check against server {0} for config DC", arg, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\ActiveDirectoryConnectivityConfigDCProbe.cs", 82);
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "*");
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 89, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\ActiveDirectoryConnectivityConfigDCProbe.cs");
				double num2 = double.MaxValue;
				for (int i = 0; i < 3; i++)
				{
					Stopwatch stopwatch = Stopwatch.StartNew();
					topologyConfigurationSession.Find(null, QueryScope.Base, filter, null, 2, new ADPropertyDefinition[]
					{
						ADObjectSchema.ObjectClass
					});
					topologyConfigurationSession.DomainController = topologyConfigurationSession.LastUsedDc;
					stopwatch.Stop();
					if ((double)stopwatch.ElapsedMilliseconds < num2)
					{
						num2 = (double)stopwatch.ElapsedMilliseconds;
					}
				}
				base.Result.StateAttribute1 = topologyConfigurationSession.LastUsedDc;
				base.Result.SampleValue = num2;
				WTFDiagnostics.TraceInformation<bool, double, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", true, base.Result.SampleValue, base.Result.StateAttribute1, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\ActiveDirectoryConnectivityConfigDCProbe.cs", 121);
				if (base.Result.SampleValue > (double)num)
				{
					throw new Exception(string.Format(" Search took {0} ms against configuration DC {1}. Threshold {2} ms.", base.Result.SampleValue, topologyConfigurationSession.LastUsedDc, num));
				}
			});
		}
	}
}
