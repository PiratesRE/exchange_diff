using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Network;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network.Probes
{
	public class DnsServerForwardersProbe : ProbeWorkItem
	{
		static DnsServerForwardersProbe()
		{
			bool flag = Environment.MachineName.Length >= 6 && Environment.MachineName.Substring(3, 3).ToUpperInvariant() == "MGT";
			if (flag)
			{
				DnsServerForwardersProbe.TargetDomainNames = DnsServerForwardersProbe.TargetDomainNamesForManagementForest;
				return;
			}
			DnsServerForwardersProbe.TargetDomainNames = DnsServerForwardersProbe.TargetDomainNamesForCapacityForest;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.TraceInformation("Begin DnsServerForwardersProbe.", new object[0]);
			this.dnsService = new WmiDnsClient(".");
			DnsServerForwardersProbe.ForwarderData[] array = this.DiscoverForwarders();
			if (array == null)
			{
				return;
			}
			this.TraceInformation("Test the health of each forwarder.", new object[0]);
			Parallel.ForEach<DnsServerForwardersProbe.ForwarderData>(array, new ParallelOptions
			{
				CancellationToken = cancellationToken
			}, delegate(DnsServerForwardersProbe.ForwarderData forwarder)
			{
				DnsServerForwardersProbe.TestForwarder(forwarder);
			});
			cancellationToken.ThrowIfCancellationRequested();
			Array.Sort<DnsServerForwardersProbe.ForwarderData>(array);
			this.TraceForwarders(array);
			this.AssessForwarders(array);
		}

		private static void TestForwarder(DnsServerForwardersProbe.ForwarderData forwarder)
		{
			forwarder.IsUnhealthy = false;
			forwarder.SuccessCount = 0;
			Parallel.ForEach<string>(DnsServerForwardersProbe.TargetDomainNames, delegate(string domainName)
			{
				Win32DnsQueryResult<IPAddress> win32DnsQueryResult = Win32DnsQuery.ResolveRecordsA(domainName, forwarder.IPAddress);
				forwarder.TallyTestResult(win32DnsQueryResult.Success);
			});
		}

		private void AssessForwarders(DnsServerForwardersProbe.ForwarderData[] forwarders)
		{
			this.TraceInformation("Assess the overall health of the forwarders.", new object[0]);
			if (forwarders.Length > 1 && forwarders[0].OriginalSequence != 0)
			{
				this.TraceInformation("The first forwarder is not the healthiest; reorder the list of forwarders.", new object[0]);
				this.SetForwarders(forwarders);
				string message = "Reordered forwarders: {0}.";
				object[] array = new object[1];
				array[0] = string.Join<IPAddress>(", ", from f in forwarders
				select f.IPAddress);
				this.TraceDebug(message, array);
			}
			int num = forwarders.Length;
			int num2 = forwarders.Count((DnsServerForwardersProbe.ForwarderData f) => f.IsUnhealthy);
			int num3 = num - num2;
			float num4 = (float)(num * 60) / 100f;
			if (num3 < 2)
			{
				this.TraceWarning("Fewer than {0} forwarders are healthy.", new object[]
				{
					2
				});
				this.SetRecoverySignal();
				return;
			}
			if ((float)num3 < num4)
			{
				this.TraceWarning("Fewer than {0}% of the forwarders are healthy.", new object[]
				{
					60
				});
				this.SetRecoverySignal();
				return;
			}
			this.TraceInformation("The number of healthy forwarders is sufficient.", new object[0]);
			this.ClearRecoverySignal();
		}

		private void ClearRecoverySignal()
		{
			FileInfo fileInfo = new FileInfo("D:\\NetworkMonitoring\\DnsServerForwardersProbe.signal");
			if (fileInfo.Exists)
			{
				try
				{
					this.TraceInformation("Prior persistent recovery signal exists; clearing it.", new object[0]);
					fileInfo.Delete();
				}
				catch (SystemException ex)
				{
					this.TraceWarning("Unable to clear persistent signal: " + ex.Message, new object[0]);
				}
			}
		}

		private DnsServerForwardersProbe.ForwarderData[] DiscoverForwarders()
		{
			this.TraceInformation("Discover the configured forwarders.", new object[0]);
			IPAddress[] forwarders = this.dnsService.GetForwarders();
			if (forwarders == null || forwarders.Length == 0)
			{
				this.ReportFailure("No DNS forwarders are configured for this DNS server.");
				return null;
			}
			this.TraceDebug("Configured forwarders: {0}.", new object[]
			{
				string.Join<IPAddress>(", ", forwarders)
			});
			return forwarders.Select((IPAddress address, int index) => new DnsServerForwardersProbe.ForwarderData(address, index)).ToArray<DnsServerForwardersProbe.ForwarderData>();
		}

		private void SetForwarders(DnsServerForwardersProbe.ForwarderData[] forwarders)
		{
			this.dnsService.SetForwarders((from f in forwarders
			select f.IPAddress).ToArray<IPAddress>());
		}

		private void SetRecoverySignal()
		{
			this.TraceInformation("Set persisent recovery signal that forwarders need to be recalculated.", new object[0]);
			FileInfo fileInfo = new FileInfo("D:\\NetworkMonitoring\\DnsServerForwardersProbe.signal");
			if (fileInfo.Exists)
			{
				this.TraceDebug("Signal already exists with date {0} UTC; not changing it.", new object[]
				{
					fileInfo.LastWriteTimeUtc
				});
				return;
			}
			try
			{
				fileInfo.Directory.Create();
				using (FileStream fileStream = fileInfo.Create())
				{
					fileStream.Close();
				}
			}
			catch (SystemException ex)
			{
				this.TraceWarning("Unable to set persistent signal: " + ex.Message, new object[0]);
			}
		}

		private void ReportFailure(string stub)
		{
			this.TraceWarning(stub, new object[0]);
			throw new ApplicationException(stub);
		}

		private void TraceDebug(string message, params object[] args)
		{
			if (args.Length == 0)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.NetworkTracer, base.TraceContext, message, null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Probes\\DnsServerForwardersProbe.cs", 273);
				return;
			}
			WTFDiagnostics.TraceDebug(ExTraceGlobals.NetworkTracer, base.TraceContext, string.Format(message, args), null, "TraceDebug", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Probes\\DnsServerForwardersProbe.cs", 277);
		}

		private void TraceForwarders(DnsServerForwardersProbe.ForwarderData[] forwarders)
		{
			this.TraceInformation("Forwarders data, in healthiest-first order:", new object[0]);
			this.TraceInformation(DnsServerForwardersProbe.ForwarderData.ToStringHeader, new object[0]);
			foreach (DnsServerForwardersProbe.ForwarderData forwarderData in forwarders)
			{
				this.TraceInformation(forwarderData.ToString(), new object[0]);
			}
		}

		private void TraceInformation(string message, params object[] args)
		{
			if (args.Length == 0)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, message, null, "TraceInformation", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Probes\\DnsServerForwardersProbe.cs", 304);
				return;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.NetworkTracer, base.TraceContext, string.Format(message, args), null, "TraceInformation", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Probes\\DnsServerForwardersProbe.cs", 308);
		}

		private void TraceWarning(string message, params object[] args)
		{
			if (args.Length == 0)
			{
				WTFDiagnostics.TraceWarning(ExTraceGlobals.NetworkTracer, base.TraceContext, message, null, "TraceWarning", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Probes\\DnsServerForwardersProbe.cs", 321);
				return;
			}
			WTFDiagnostics.TraceWarning(ExTraceGlobals.NetworkTracer, base.TraceContext, string.Format(message, args), null, "TraceWarning", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\Probes\\DnsServerForwardersProbe.cs", 325);
		}

		private const int MinimumHealthyForwardersCount = 2;

		private const int MinimumHealthyForwardersPercentage = 60;

		private const string SignalFileName = "D:\\NetworkMonitoring\\DnsServerForwardersProbe.signal";

		private static readonly string[] TargetDomainNames;

		private static readonly string[] TargetDomainNamesForCapacityForest = new string[]
		{
			"brody.outlook.com.",
			"na01b.map.protection.outlook.com.",
			"www.bing.com."
		};

		private static readonly string[] TargetDomainNamesForManagementForest = new string[]
		{
			"www.bing.com.",
			"www.yahoo.com.",
			"www.google.com."
		};

		private WmiDnsClient dnsService;

		private class ForwarderData : IComparable<DnsServerForwardersProbe.ForwarderData>
		{
			public ForwarderData(IPAddress ipAddress, int sequence)
			{
				this.IPAddress = ipAddress;
				this.OriginalSequence = sequence;
			}

			public static string ToStringHeader
			{
				get
				{
					return string.Format("  {0,-15}  {1,11}  {2,12}  {3,11}", new object[]
					{
						"IPAddress",
						"OriginalSeq",
						"SuccessCount",
						"IsUnhealthy"
					});
				}
			}

			public IPAddress IPAddress { get; private set; }

			public bool IsUnhealthy { get; set; }

			public int OriginalSequence { get; private set; }

			public int SuccessCount
			{
				get
				{
					return this.successCount;
				}
				set
				{
					this.successCount = value;
				}
			}

			public void TallyTestResult(bool result)
			{
				if (result)
				{
					Interlocked.Increment(ref this.successCount);
					return;
				}
				this.IsUnhealthy = true;
			}

			public override string ToString()
			{
				return string.Format("  {0,-15}  {1,11}  {2,12}  {3,11}", new object[]
				{
					this.IPAddress,
					this.OriginalSequence,
					this.SuccessCount,
					this.IsUnhealthy
				});
			}

			int IComparable<DnsServerForwardersProbe.ForwarderData>.CompareTo(DnsServerForwardersProbe.ForwarderData other)
			{
				if (other == null)
				{
					return 1;
				}
				if (object.ReferenceEquals(this, other))
				{
					return 0;
				}
				int num = other.SuccessCount - this.SuccessCount;
				if (num == 0)
				{
					num = this.OriginalSequence - other.OriginalSequence;
				}
				return num;
			}

			private const string ToStringTemplate = "  {0,-15}  {1,11}  {2,12}  {3,11}";

			private int successCount;
		}
	}
}
