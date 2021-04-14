using System;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Probes
{
	public class StoreAdminRPCInterfaceProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			Exception ex = null;
			TimeSpan value = TimeSpan.FromSeconds(30.0);
			DateTime utcNow = DateTime.UtcNow;
			try
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Starting store admin RPC interface check on server {0}", Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreAdminRPCInterfaceProbe.cs", 38);
				string text;
				if (base.Definition.Attributes.TryGetValue("ValidationTimeoutSeconds", out text) && !string.IsNullOrWhiteSpace(text))
				{
					TimeSpan.TryParse(text, out value);
				}
				MdbStatus[] array;
				if (!AmStoreHelper.GetAllDatabaseStatuses(new AmServerName(Environment.MachineName), true, "Client=StoreActiveMonitoring", new TimeSpan?(value), out array, out ex))
				{
					if (ex != null)
					{
						base.Result.StateAttribute1 = ((ex.InnerException != null) ? ex.InnerException.GetType().FullName : ex.GetType().FullName);
						WTFDiagnostics.TraceError<string, Exception>(ExTraceGlobals.StoreTracer, base.TraceContext, "Store admin RPC interface check on server {0} failed with error {1}", Environment.MachineName, ex, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreAdminRPCInterfaceProbe.cs", 69);
						throw ex;
					}
					throw new Exception(Strings.StoreAdminRPCInterfaceNotResponding(Environment.MachineName));
				}
				else
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Successfully finished store admin RPC interface check on server {0}", Environment.MachineName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\StoreAdminRPCInterfaceProbe.cs", 84);
				}
			}
			finally
			{
				base.Result.SampleValue = (double)((int)(DateTime.UtcNow - utcNow).TotalMilliseconds);
			}
		}
	}
}
