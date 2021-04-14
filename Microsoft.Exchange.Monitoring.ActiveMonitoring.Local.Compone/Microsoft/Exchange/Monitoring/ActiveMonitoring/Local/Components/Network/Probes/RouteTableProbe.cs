using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Probes
{
	internal class RouteTableProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.localCancellationToken = cancellationToken;
			bool flag = false;
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * FROM Win32_OperatingSystem"))
			{
				foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
				{
					string strA = managementBaseObject["Version"].ToString();
					flag = (string.Compare(strA, "6.2.9", StringComparison.Ordinal) > 0);
				}
			}
			if (!flag)
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "This machine has operating system version earlier than 6.2.9*. Probe will not run on it. Returning...", new object[0]);
				return;
			}
			List<string> routes = this.GetRoutes("SELECT * FROM Win32_IP4RouteTable");
			if (!routes.Any<string>())
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Probe will throw. No management objects found, recovery required.", new object[0]);
				throw new Exception(string.Format("No management objects found, recovery required.", new object[0]));
			}
			NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "{0} active routes are found. Network Destinations are as follows: {1}.", new object[]
			{
				routes.Count,
				string.Join(", ", routes)
			});
			List<string> routes2 = this.GetRoutes("SELECT * FROM Win32_IP4PersistedRouteTable");
			if (!routes2.Any<string>())
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "No persistent routes could be found, automatic recovery cannot fix this, returning...", new object[0]);
				return;
			}
			NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "{0} persistent routes are found. Network Destinations are as follows: {1}.", new object[]
			{
				routes2.Count,
				string.Join(", ", routes2)
			});
			IEnumerable<string> enumerable2 = routes.Intersect(routes2);
			IList<string> enumerable = (enumerable2 as IList<string>) ?? enumerable2.ToList<string>();
			if (!routes2.Aggregate(true, (bool current, string route) => current & enumerable.Contains(route)))
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "Probe will throw. Missing persistent route entry is detected in active route table, recovery required.", new object[0]);
				throw new Exception(string.Format("Probe will throw. Missing persistent route entry is detected in active route table, recovery required.", new object[0]));
			}
			NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "All persistent routes are in active routes list. Nothing to recover. Returning...", new object[0]);
		}

		private List<string> GetRoutes(string query)
		{
			if (this.localCancellationToken.IsCancellationRequested)
			{
				NetworkUtils.LogWorkItemMessage(base.TraceContext, base.Result, "GetRoutes: Operation is cancelled", new object[0]);
				throw new OperationCanceledException(this.localCancellationToken);
			}
			List<string> result;
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", query))
			{
				result = (from ManagementObject mgmtobj in managementObjectSearcher.Get()
				select string.Concat(new string[]
				{
					mgmtobj["Name"].ToString(),
					" ",
					mgmtobj["Mask"].ToString(),
					" ",
					mgmtobj["Nexthop"].ToString()
				})).ToList<string>();
			}
			return result;
		}

		private CancellationToken localCancellationToken;
	}
}
