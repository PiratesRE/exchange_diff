using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.Authentication
{
	internal sealed class AuthenticationCountersInstance : PerformanceCounterInstance
	{
		internal AuthenticationCountersInstance(string instanceName, AuthenticationCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Authentication")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.OutstandingAuthenticationRequests = new ExPerformanceCounter(base.CategoryName, "Outstanding Authentication Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutstandingAuthenticationRequests);
				this.TotalAuthenticationRequests = new ExPerformanceCounter(base.CategoryName, "Total Authentication Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalAuthenticationRequests);
				this.RejectedAuthenticationRequests = new ExPerformanceCounter(base.CategoryName, "Rejected Authentication Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RejectedAuthenticationRequests);
				this.AuthenticationLatency = new ExPerformanceCounter(base.CategoryName, "Authentication Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AuthenticationLatency);
				long num = this.OutstandingAuthenticationRequests.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal AuthenticationCountersInstance(string instanceName) : base(instanceName, "MSExchange Authentication")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.OutstandingAuthenticationRequests = new ExPerformanceCounter(base.CategoryName, "Outstanding Authentication Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutstandingAuthenticationRequests);
				this.TotalAuthenticationRequests = new ExPerformanceCounter(base.CategoryName, "Total Authentication Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalAuthenticationRequests);
				this.RejectedAuthenticationRequests = new ExPerformanceCounter(base.CategoryName, "Rejected Authentication Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.RejectedAuthenticationRequests);
				this.AuthenticationLatency = new ExPerformanceCounter(base.CategoryName, "Authentication Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AuthenticationLatency);
				long num = this.OutstandingAuthenticationRequests.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
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

		public readonly ExPerformanceCounter OutstandingAuthenticationRequests;

		public readonly ExPerformanceCounter TotalAuthenticationRequests;

		public readonly ExPerformanceCounter RejectedAuthenticationRequests;

		public readonly ExPerformanceCounter AuthenticationLatency;
	}
}
