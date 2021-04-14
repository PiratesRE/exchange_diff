using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class ActiveDirectoryConnectivityProbe : ProbeWorkItem
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
			DirectoryUtils.Logger(this, StxLogType.TestActiveDirectoryConnectivity, delegate
			{
				string arg = new ServerIdParameter().ToString();
				if (!base.Definition.Attributes.ContainsKey("ADConnectivityThreshold"))
				{
					throw new ArgumentException("ADConnectivityThreshold");
				}
				int num = int.Parse(base.Definition.Attributes["ADConnectivityThreshold"]);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting ADDriver availability check against server {0}", arg, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\ActiveDirectoryConnectivityProbe.cs", 82);
				bool flag = true;
				StringBuilder stringBuilder = new StringBuilder();
				PartitionId[] array = ADAccountPartitionLocator.GetAllAccountPartitionIds();
				if (array.Length == 0)
				{
					array = new PartitionId[]
					{
						new PartitionId(TopologyProvider.LocalForestFqdn)
					};
				}
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "*");
				foreach (PartitionId partitionId in array)
				{
					IDirectorySession directorySession;
					if (partitionId.IsLocalForestPartition())
					{
						directorySession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 105, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\ActiveDirectoryConnectivityProbe.cs");
					}
					else
					{
						ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
						directorySession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 110, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\ActiveDirectoryConnectivityProbe.cs");
					}
					double num2 = double.MaxValue;
					for (int j = 0; j < 3; j++)
					{
						Stopwatch stopwatch = Stopwatch.StartNew();
						directorySession.Find(null, QueryScope.Base, filter, null, 2, new ADPropertyDefinition[]
						{
							ADObjectSchema.ObjectClass
						});
						directorySession.DomainController = directorySession.LastUsedDc;
						stopwatch.Stop();
						if ((double)stopwatch.ElapsedMilliseconds < num2)
						{
							num2 = (double)stopwatch.ElapsedMilliseconds;
						}
					}
					if (num2 > (double)num)
					{
						stringBuilder.Append(string.Format(" Search took {0} ms against DC {1} in the partition {2}. Threshold {3} ms. ", new object[]
						{
							base.Result.SampleValue,
							directorySession.LastUsedDc,
							partitionId.ForestFQDN,
							num
						}));
						flag = false;
					}
					base.Result.StateAttribute1 = base.Result.StateAttribute1 + directorySession.LastUsedDc + "; ";
					base.Result.StateAttribute2 = base.Result.StateAttribute2 + num2.ToString() + "; ";
				}
				base.Result.Error = stringBuilder.ToString();
				WTFDiagnostics.TraceInformation<bool, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Output {1} Error {2}", true, base.Result.StateAttribute1, stringBuilder.ToString(), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\ActiveDirectoryConnectivityProbe.cs", 152);
				if (!flag)
				{
					throw new Exception(stringBuilder.ToString());
				}
			});
		}
	}
}
