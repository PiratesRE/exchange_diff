using System;
using System.Linq;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class DagUtils
	{
		internal static string[] GetMailboxServersInSameDag()
		{
			string[] array = null;
			try
			{
				IADServer localServer = CachedAdReader.Instance.LocalServer;
				if (localServer.DatabaseAvailabilityGroup != null)
				{
					IADServer[] allServersInLocalDag = CachedAdReader.Instance.AllServersInLocalDag;
					if (allServersInLocalDag != null)
					{
						array = (from server in allServersInLocalDag
						select server.Name).ToArray<string>();
					}
				}
				else
				{
					array = new string[]
					{
						localServer.Name
					};
				}
				if (array == null || array.Length == 0)
				{
					throw new InvalidOperationException("Could not discover servers in the database availability group");
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.RecoveryActionTracer, TracingContext.Default, "GetMailboxServersInSameDag() failed with exception {0}", ex.ToString(), null, "GetMailboxServersInSameDag", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\DagUtils.cs", 61);
			}
			return array;
		}
	}
}
