using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Autodiscover
{
	internal sealed class AutodiscoverDatacenterPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal AutodiscoverDatacenterPerformanceCountersInstance(string instanceName, AutodiscoverDatacenterPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeAutodiscover:Datacenter")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Partner Token Requests/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalPartnerTokenRequests = new ExPerformanceCounter(base.CategoryName, "Total Partner Token Requests", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalPartnerTokenRequests, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalPartnerTokenRequests);
				this.TotalPartnerTokenRequestsPerTimeWindow = new ExPerformanceCounter(base.CategoryName, "Total Partner Token Requests/Time Window", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalPartnerTokenRequestsPerTimeWindow, new ExPerformanceCounter[0]);
				list.Add(this.TotalPartnerTokenRequestsPerTimeWindow);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Partner Token Requests Failed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalPartnerTokenRequestsFailed = new ExPerformanceCounter(base.CategoryName, "Total Partner Token Requests Failed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalPartnerTokenRequestsFailed, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalPartnerTokenRequestsFailed);
				this.TotalPartnerTokenRequestsFailedPerTimeWindow = new ExPerformanceCounter(base.CategoryName, "Total Partner Token Requests Failed/Time Window", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalPartnerTokenRequestsFailedPerTimeWindow, new ExPerformanceCounter[0]);
				list.Add(this.TotalPartnerTokenRequestsFailedPerTimeWindow);
				this.TotalCertAuthRequestsFailed = new ExPerformanceCounter(base.CategoryName, "Total Certificate Authentication Requests Failed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalCertAuthRequestsFailed, new ExPerformanceCounter[0]);
				list.Add(this.TotalCertAuthRequestsFailed);
				this.TotalCertAuthRequestsFailedPerTimeWindow = new ExPerformanceCounter(base.CategoryName, "Total Certificate Authentication Requests Failed/Time Window", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalCertAuthRequestsFailedPerTimeWindow, new ExPerformanceCounter[0]);
				list.Add(this.TotalCertAuthRequestsFailedPerTimeWindow);
				this.AveragePartnerInfoQueryTime = new ExPerformanceCounter(base.CategoryName, "Average Partner Info Query Time", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AveragePartnerInfoQueryTime, new ExPerformanceCounter[0]);
				list.Add(this.AveragePartnerInfoQueryTime);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Requests Received with Partner Token/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalRequestsReceivedWithPartnerToken = new ExPerformanceCounter(base.CategoryName, "Total Requests Received with Partner Token", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalRequestsReceivedWithPartnerToken, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TotalRequestsReceivedWithPartnerToken);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Unauthorized Requests Received with Partner Token/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.TotalUnauthorizedRequestsReceivedWithPartnerToken = new ExPerformanceCounter(base.CategoryName, "Total Unauthorized Requests Received with Partner Token", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalUnauthorizedRequestsReceivedWithPartnerToken, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.TotalUnauthorizedRequestsReceivedWithPartnerToken);
				long num = this.TotalPartnerTokenRequests.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter5 in list)
					{
						exPerformanceCounter5.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal AutodiscoverDatacenterPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangeAutodiscover:Datacenter")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Partner Token Requests/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalPartnerTokenRequests = new ExPerformanceCounter(base.CategoryName, "Total Partner Token Requests", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalPartnerTokenRequests);
				this.TotalPartnerTokenRequestsPerTimeWindow = new ExPerformanceCounter(base.CategoryName, "Total Partner Token Requests/Time Window", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalPartnerTokenRequestsPerTimeWindow);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Partner Token Requests Failed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalPartnerTokenRequestsFailed = new ExPerformanceCounter(base.CategoryName, "Total Partner Token Requests Failed", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalPartnerTokenRequestsFailed);
				this.TotalPartnerTokenRequestsFailedPerTimeWindow = new ExPerformanceCounter(base.CategoryName, "Total Partner Token Requests Failed/Time Window", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalPartnerTokenRequestsFailedPerTimeWindow);
				this.TotalCertAuthRequestsFailed = new ExPerformanceCounter(base.CategoryName, "Total Certificate Authentication Requests Failed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalCertAuthRequestsFailed);
				this.TotalCertAuthRequestsFailedPerTimeWindow = new ExPerformanceCounter(base.CategoryName, "Total Certificate Authentication Requests Failed/Time Window", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalCertAuthRequestsFailedPerTimeWindow);
				this.AveragePartnerInfoQueryTime = new ExPerformanceCounter(base.CategoryName, "Average Partner Info Query Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AveragePartnerInfoQueryTime);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Requests Received with Partner Token/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalRequestsReceivedWithPartnerToken = new ExPerformanceCounter(base.CategoryName, "Total Requests Received with Partner Token", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TotalRequestsReceivedWithPartnerToken);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Unauthorized Requests Received with Partner Token/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.TotalUnauthorizedRequestsReceivedWithPartnerToken = new ExPerformanceCounter(base.CategoryName, "Total Unauthorized Requests Received with Partner Token", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.TotalUnauthorizedRequestsReceivedWithPartnerToken);
				long num = this.TotalPartnerTokenRequests.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter5 in list)
					{
						exPerformanceCounter5.Close();
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

		public readonly ExPerformanceCounter TotalPartnerTokenRequests;

		public readonly ExPerformanceCounter TotalPartnerTokenRequestsPerTimeWindow;

		public readonly ExPerformanceCounter TotalPartnerTokenRequestsFailed;

		public readonly ExPerformanceCounter TotalPartnerTokenRequestsFailedPerTimeWindow;

		public readonly ExPerformanceCounter TotalCertAuthRequestsFailed;

		public readonly ExPerformanceCounter TotalCertAuthRequestsFailedPerTimeWindow;

		public readonly ExPerformanceCounter AveragePartnerInfoQueryTime;

		public readonly ExPerformanceCounter TotalRequestsReceivedWithPartnerToken;

		public readonly ExPerformanceCounter TotalUnauthorizedRequestsReceivedWithPartnerToken;
	}
}
