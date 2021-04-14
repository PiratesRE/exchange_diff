using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class TopologyServiceAvailabilityProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestTopologyService, delegate
			{
				string text = new ServerIdParameter().ToString();
				bool flag = true;
				StringBuilder stringBuilder = new StringBuilder();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting topology availability check against server {0}", text, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TopologyServiceAvailabilityProbe.cs", 57);
				using (TopologyServiceClient topologyServiceClient = TopologyServiceClient.CreateClient(text))
				{
					string text2 = string.Empty;
					Stopwatch stopwatch = Stopwatch.StartNew();
					PartitionId[] array = ADAccountPartitionLocator.GetAllAccountPartitionIds();
					if (array.Length == 0)
					{
						array = new PartitionId[]
						{
							new PartitionId(TopologyProvider.LocalForestFqdn)
						};
					}
					foreach (PartitionId partitionId in array)
					{
						string forestFQDN = partitionId.ForestFQDN;
						IList<ServerInfo> serversForRole = topologyServiceClient.GetServersForRole(forestFQDN, new List<string>(), ADServerRole.GlobalCatalog, 20, false);
						if (serversForRole == null || serversForRole.Count == 0)
						{
							text2 = text2 + " No servers returned from TopologyService for " + forestFQDN;
							flag = false;
						}
						else
						{
							foreach (ServerInfo serverInfo in serversForRole)
							{
								stringBuilder.Append(serverInfo.Fqdn + Environment.NewLine);
							}
						}
					}
					stopwatch.Stop();
					base.Result.StateAttribute1 = stringBuilder.ToString();
					base.Result.Error = text2;
					base.Result.SampleValue = (double)stopwatch.ElapsedMilliseconds;
					WTFDiagnostics.TraceInformation<bool, double, string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Operation succeeded: {0} Time Taken {1} Output {2} Error{3}", flag, base.Result.SampleValue, base.Result.StateAttribute1, base.Result.Error, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TopologyServiceAvailabilityProbe.cs", 97);
					if (!flag)
					{
						throw new Exception(text2);
					}
				}
			});
		}
	}
}
