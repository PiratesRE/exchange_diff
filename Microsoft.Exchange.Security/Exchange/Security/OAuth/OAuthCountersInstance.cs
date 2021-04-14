using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class OAuthCountersInstance : PerformanceCounterInstance
	{
		internal OAuthCountersInstance(string instanceName, OAuthCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange OAuth")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Inbound: Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfAuthRequests = new ExPerformanceCounter(base.CategoryName, "Inbound: Total Auth Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfAuthRequests);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Inbound: Failed Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.NumberOfFailedAuthRequests = new ExPerformanceCounter(base.CategoryName, "Inbound: Failed Auth Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.NumberOfFailedAuthRequests);
				this.AverageResponseTime = new ExPerformanceCounter(base.CategoryName, "Inbound: Average Auth Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageResponseTime);
				this.AverageAuthServerResponseTime = new ExPerformanceCounter(base.CategoryName, "Outbound: Average AuthServer Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAuthServerResponseTime);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Outbound: Token Requests to AuthServer/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.NumberOfAuthServerTokenRequests = new ExPerformanceCounter(base.CategoryName, "Outbound: Total Token Requests to AuthServer", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.NumberOfAuthServerTokenRequests);
				this.NumberOfPendingAuthServerRequests = new ExPerformanceCounter(base.CategoryName, "Outbound: Number of Pending AuthServer Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfPendingAuthServerRequests);
				this.AuthServerTokenCacheSize = new ExPerformanceCounter(base.CategoryName, "Outbound: AuthServer Token Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AuthServerTokenCacheSize);
				this.NumberOfAuthServerTimeoutTokenRequests = new ExPerformanceCounter(base.CategoryName, "Outbound: Total Timeout Token Requests to AuthServer", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfAuthServerTimeoutTokenRequests);
				long num = this.NumberOfAuthRequests.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal OAuthCountersInstance(string instanceName) : base(instanceName, "MSExchange OAuth")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Inbound: Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfAuthRequests = new ExPerformanceCounter(base.CategoryName, "Inbound: Total Auth Requests", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfAuthRequests);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Inbound: Failed Auth Requests/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.NumberOfFailedAuthRequests = new ExPerformanceCounter(base.CategoryName, "Inbound: Failed Auth Requests Total", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.NumberOfFailedAuthRequests);
				this.AverageResponseTime = new ExPerformanceCounter(base.CategoryName, "Inbound: Average Auth Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageResponseTime);
				this.AverageAuthServerResponseTime = new ExPerformanceCounter(base.CategoryName, "Outbound: Average AuthServer Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAuthServerResponseTime);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Outbound: Token Requests to AuthServer/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.NumberOfAuthServerTokenRequests = new ExPerformanceCounter(base.CategoryName, "Outbound: Total Token Requests to AuthServer", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.NumberOfAuthServerTokenRequests);
				this.NumberOfPendingAuthServerRequests = new ExPerformanceCounter(base.CategoryName, "Outbound: Number of Pending AuthServer Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfPendingAuthServerRequests);
				this.AuthServerTokenCacheSize = new ExPerformanceCounter(base.CategoryName, "Outbound: AuthServer Token Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AuthServerTokenCacheSize);
				this.NumberOfAuthServerTimeoutTokenRequests = new ExPerformanceCounter(base.CategoryName, "Outbound: Total Timeout Token Requests to AuthServer", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfAuthServerTimeoutTokenRequests);
				long num = this.NumberOfAuthRequests.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
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

		public readonly ExPerformanceCounter NumberOfAuthRequests;

		public readonly ExPerformanceCounter NumberOfFailedAuthRequests;

		public readonly ExPerformanceCounter AverageResponseTime;

		public readonly ExPerformanceCounter AverageAuthServerResponseTime;

		public readonly ExPerformanceCounter NumberOfAuthServerTokenRequests;

		public readonly ExPerformanceCounter NumberOfPendingAuthServerRequests;

		public readonly ExPerformanceCounter AuthServerTokenCacheSize;

		public readonly ExPerformanceCounter NumberOfAuthServerTimeoutTokenRequests;
	}
}
