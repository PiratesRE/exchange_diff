using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal sealed class HttpProxyCountersInstance : PerformanceCounterInstance
	{
		internal HttpProxyCountersInstance(string instanceName, HttpProxyCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange HttpProxy")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Requests/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalRequests = new ExPerformanceCounter(base.CategoryName, "Total Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalRequests);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Bytes Out/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalBytesOut = new ExPerformanceCounter(base.CategoryName, "Total Bytes Out", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalBytesOut);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Bytes In/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalBytesIn = new ExPerformanceCounter(base.CategoryName, "Total Bytes In", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TotalBytesIn);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Proxy Requests/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.TotalProxyRequests = new ExPerformanceCounter(base.CategoryName, "Total Proxy Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.TotalProxyRequests);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Calls/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.MailboxServerLocatorCalls = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Calls", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.MailboxServerLocatorCalls);
				this.MailboxServerLocatorFailedCalls = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Failed Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxServerLocatorFailedCalls);
				this.MailboxServerLocatorRetriedCalls = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Retried Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxServerLocatorRetriedCalls);
				this.MailboxServerLocatorLatency = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Last Call Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxServerLocatorLatency);
				this.MailboxServerLocatorAverageLatency = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Average Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxServerLocatorAverageLatency);
				this.MailboxServerLocatorAverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Average Latency Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxServerLocatorAverageLatencyBase);
				this.DownLevelTotalServers = new ExPerformanceCounter(base.CategoryName, "ClientAccess 2010 Total Servers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DownLevelTotalServers);
				this.DownLevelHealthyServers = new ExPerformanceCounter(base.CategoryName, "ClientAccess 2010 Healthy Servers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DownLevelHealthyServers);
				this.DownLevelServersLastPing = new ExPerformanceCounter(base.CategoryName, "ClientAccess 2010 Servers Last Ping", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DownLevelServersLastPing);
				this.LoadBalancerHealthChecks = new ExPerformanceCounter(base.CategoryName, "Load Balancer Health Checks", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LoadBalancerHealthChecks);
				this.MovingAverageCasLatency = new ExPerformanceCounter(base.CategoryName, "Average ClientAccess Server Processing Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingAverageCasLatency);
				this.MovingAverageTenantLookupLatency = new ExPerformanceCounter(base.CategoryName, "Average Tenant Lookup Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingAverageTenantLookupLatency);
				this.MovingAverageAuthenticationLatency = new ExPerformanceCounter(base.CategoryName, "Average Authentication Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingAverageAuthenticationLatency);
				this.MovingAverageMailboxServerLocatorLatency = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Average Latency (Moving Average)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingAverageMailboxServerLocatorLatency);
				this.MovingAverageSharedCacheLatency = new ExPerformanceCounter(base.CategoryName, "Shared Cache Client Average Latency (Moving Average)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingAverageSharedCacheLatency);
				this.MovingPercentageMailboxServerFailure = new ExPerformanceCounter(base.CategoryName, "Mailbox Server Proxy Failure Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageMailboxServerFailure);
				this.MovingPercentageMailboxServerLocatorRetriedCalls = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Retried Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageMailboxServerLocatorRetriedCalls);
				this.MovingPercentageMailboxServerLocatorFailedCalls = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Failure Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageMailboxServerLocatorFailedCalls);
				this.MovingPercentageNewProxyConnectionCreation = new ExPerformanceCounter(base.CategoryName, "Proxy Connection Creation Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageNewProxyConnectionCreation);
				this.TotalRetryOnError = new ExPerformanceCounter(base.CategoryName, "Total RetryOnError", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRetryOnError);
				this.FailedRetryOnError = new ExPerformanceCounter(base.CategoryName, "Failed RetryOnError", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FailedRetryOnError);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Accepted Connection Count Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.AcceptedConnectionCount = new ExPerformanceCounter(base.CategoryName, "Accepted Connection Count", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.AcceptedConnectionCount);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Rejected Connection Count Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.RejectedConnectionCount = new ExPerformanceCounter(base.CategoryName, "Rejected Connection Count", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter7
				});
				list.Add(this.RejectedConnectionCount);
				this.OutstandingProxyRequests = new ExPerformanceCounter(base.CategoryName, "Outstanding Proxy Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutstandingProxyRequests);
				this.OutstandingProxyRequestsToForest = new ExPerformanceCounter(base.CategoryName, "Outstanding Proxy Requests to Target Forest", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutstandingProxyRequestsToForest);
				this.OutstandingSharedCacheRequests = new ExPerformanceCounter(base.CategoryName, "Outstanding Shared Cache Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutstandingSharedCacheRequests);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Redirect By Sender Mailbox Count Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				this.RedirectBySenderMailboxCount = new ExPerformanceCounter(base.CategoryName, "Redirect By Sender Mailbox Count", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter8
				});
				list.Add(this.RedirectBySenderMailboxCount);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "Redirect By Tenant Mailbox Count Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.RedirectByTenantMailboxCount = new ExPerformanceCounter(base.CategoryName, "Redirect By Tenant Mailbox Count", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter9
				});
				list.Add(this.RedirectByTenantMailboxCount);
				long num = this.TotalRequests.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter10 in list)
					{
						exPerformanceCounter10.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal HttpProxyCountersInstance(string instanceName) : base(instanceName, "MSExchange HttpProxy")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Requests/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalRequests = new ExPerformanceCounter(base.CategoryName, "Total Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalRequests);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Bytes Out/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalBytesOut = new ExPerformanceCounter(base.CategoryName, "Total Bytes Out", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalBytesOut);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Bytes In/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalBytesIn = new ExPerformanceCounter(base.CategoryName, "Total Bytes In", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TotalBytesIn);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Proxy Requests/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.TotalProxyRequests = new ExPerformanceCounter(base.CategoryName, "Total Proxy Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.TotalProxyRequests);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Calls/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.MailboxServerLocatorCalls = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Calls", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.MailboxServerLocatorCalls);
				this.MailboxServerLocatorFailedCalls = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Failed Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxServerLocatorFailedCalls);
				this.MailboxServerLocatorRetriedCalls = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Retried Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxServerLocatorRetriedCalls);
				this.MailboxServerLocatorLatency = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Last Call Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxServerLocatorLatency);
				this.MailboxServerLocatorAverageLatency = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Average Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxServerLocatorAverageLatency);
				this.MailboxServerLocatorAverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Average Latency Base", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxServerLocatorAverageLatencyBase);
				this.DownLevelTotalServers = new ExPerformanceCounter(base.CategoryName, "ClientAccess 2010 Total Servers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DownLevelTotalServers);
				this.DownLevelHealthyServers = new ExPerformanceCounter(base.CategoryName, "ClientAccess 2010 Healthy Servers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DownLevelHealthyServers);
				this.DownLevelServersLastPing = new ExPerformanceCounter(base.CategoryName, "ClientAccess 2010 Servers Last Ping", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DownLevelServersLastPing);
				this.LoadBalancerHealthChecks = new ExPerformanceCounter(base.CategoryName, "Load Balancer Health Checks", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LoadBalancerHealthChecks);
				this.MovingAverageCasLatency = new ExPerformanceCounter(base.CategoryName, "Average ClientAccess Server Processing Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingAverageCasLatency);
				this.MovingAverageTenantLookupLatency = new ExPerformanceCounter(base.CategoryName, "Average Tenant Lookup Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingAverageTenantLookupLatency);
				this.MovingAverageAuthenticationLatency = new ExPerformanceCounter(base.CategoryName, "Average Authentication Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingAverageAuthenticationLatency);
				this.MovingAverageMailboxServerLocatorLatency = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Average Latency (Moving Average)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingAverageMailboxServerLocatorLatency);
				this.MovingAverageSharedCacheLatency = new ExPerformanceCounter(base.CategoryName, "Shared Cache Client Average Latency (Moving Average)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingAverageSharedCacheLatency);
				this.MovingPercentageMailboxServerFailure = new ExPerformanceCounter(base.CategoryName, "Mailbox Server Proxy Failure Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageMailboxServerFailure);
				this.MovingPercentageMailboxServerLocatorRetriedCalls = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Retried Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageMailboxServerLocatorRetriedCalls);
				this.MovingPercentageMailboxServerLocatorFailedCalls = new ExPerformanceCounter(base.CategoryName, "MailboxServerLocator Failure Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageMailboxServerLocatorFailedCalls);
				this.MovingPercentageNewProxyConnectionCreation = new ExPerformanceCounter(base.CategoryName, "Proxy Connection Creation Rate", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MovingPercentageNewProxyConnectionCreation);
				this.TotalRetryOnError = new ExPerformanceCounter(base.CategoryName, "Total RetryOnError", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRetryOnError);
				this.FailedRetryOnError = new ExPerformanceCounter(base.CategoryName, "Failed RetryOnError", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.FailedRetryOnError);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Accepted Connection Count Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.AcceptedConnectionCount = new ExPerformanceCounter(base.CategoryName, "Accepted Connection Count", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.AcceptedConnectionCount);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Rejected Connection Count Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.RejectedConnectionCount = new ExPerformanceCounter(base.CategoryName, "Rejected Connection Count", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter7
				});
				list.Add(this.RejectedConnectionCount);
				this.OutstandingProxyRequests = new ExPerformanceCounter(base.CategoryName, "Outstanding Proxy Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutstandingProxyRequests);
				this.OutstandingProxyRequestsToForest = new ExPerformanceCounter(base.CategoryName, "Outstanding Proxy Requests to Target Forest", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutstandingProxyRequestsToForest);
				this.OutstandingSharedCacheRequests = new ExPerformanceCounter(base.CategoryName, "Outstanding Shared Cache Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutstandingSharedCacheRequests);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Redirect By Sender Mailbox Count Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				this.RedirectBySenderMailboxCount = new ExPerformanceCounter(base.CategoryName, "Redirect By Sender Mailbox Count", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter8
				});
				list.Add(this.RedirectBySenderMailboxCount);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "Redirect By Tenant Mailbox Count Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.RedirectByTenantMailboxCount = new ExPerformanceCounter(base.CategoryName, "Redirect By Tenant Mailbox Count", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter9
				});
				list.Add(this.RedirectByTenantMailboxCount);
				long num = this.TotalRequests.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter10 in list)
					{
						exPerformanceCounter10.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter TotalRequests;

		public readonly ExPerformanceCounter TotalBytesOut;

		public readonly ExPerformanceCounter TotalBytesIn;

		public readonly ExPerformanceCounter TotalProxyRequests;

		public readonly ExPerformanceCounter MailboxServerLocatorCalls;

		public readonly ExPerformanceCounter MailboxServerLocatorFailedCalls;

		public readonly ExPerformanceCounter MailboxServerLocatorRetriedCalls;

		public readonly ExPerformanceCounter MailboxServerLocatorLatency;

		public readonly ExPerformanceCounter MailboxServerLocatorAverageLatency;

		public readonly ExPerformanceCounter MailboxServerLocatorAverageLatencyBase;

		public readonly ExPerformanceCounter DownLevelTotalServers;

		public readonly ExPerformanceCounter DownLevelHealthyServers;

		public readonly ExPerformanceCounter DownLevelServersLastPing;

		public readonly ExPerformanceCounter LoadBalancerHealthChecks;

		public readonly ExPerformanceCounter MovingAverageCasLatency;

		public readonly ExPerformanceCounter MovingAverageTenantLookupLatency;

		public readonly ExPerformanceCounter MovingAverageAuthenticationLatency;

		public readonly ExPerformanceCounter MovingAverageMailboxServerLocatorLatency;

		public readonly ExPerformanceCounter MovingAverageSharedCacheLatency;

		public readonly ExPerformanceCounter MovingPercentageMailboxServerFailure;

		public readonly ExPerformanceCounter MovingPercentageMailboxServerLocatorRetriedCalls;

		public readonly ExPerformanceCounter MovingPercentageMailboxServerLocatorFailedCalls;

		public readonly ExPerformanceCounter MovingPercentageNewProxyConnectionCreation;

		public readonly ExPerformanceCounter TotalRetryOnError;

		public readonly ExPerformanceCounter FailedRetryOnError;

		public readonly ExPerformanceCounter AcceptedConnectionCount;

		public readonly ExPerformanceCounter RejectedConnectionCount;

		public readonly ExPerformanceCounter OutstandingProxyRequests;

		public readonly ExPerformanceCounter OutstandingProxyRequestsToForest;

		public readonly ExPerformanceCounter OutstandingSharedCacheRequests;

		public readonly ExPerformanceCounter RedirectBySenderMailboxCount;

		public readonly ExPerformanceCounter RedirectByTenantMailboxCount;
	}
}
