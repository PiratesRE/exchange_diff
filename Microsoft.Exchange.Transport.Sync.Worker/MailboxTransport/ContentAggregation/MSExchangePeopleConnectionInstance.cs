using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MSExchangePeopleConnectionInstance : PerformanceCounterInstance
	{
		internal MSExchangePeopleConnectionInstance(string instanceName, MSExchangePeopleConnectionInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Transport Sync - Contacts")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Bytes Downloaded/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalBytesDownloaded = new ExPerformanceCounter(base.CategoryName, "Total bytes downloaded", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalBytesDownloaded);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Bytes Uploaded/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalBytesUploaded = new ExPerformanceCounter(base.CategoryName, "Total bytes uploaded", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalBytesUploaded);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Contacts Downloaded/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalContactsDownloaded = new ExPerformanceCounter(base.CategoryName, "Total Contacts Downloaded", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TotalContactsDownloaded);
				this.TotalRequests = new ExPerformanceCounter(base.CategoryName, "Total Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRequests);
				this.TotalRequestsWithNoChanges = new ExPerformanceCounter(base.CategoryName, "Total Requests with No Changes", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRequestsWithNoChanges);
				long num = this.TotalBytesDownloaded.RawValue;
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

		internal MSExchangePeopleConnectionInstance(string instanceName) : base(instanceName, "MSExchange Transport Sync - Contacts")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Bytes Downloaded/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalBytesDownloaded = new ExPerformanceCounter(base.CategoryName, "Total bytes downloaded", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalBytesDownloaded);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Bytes Uploaded/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalBytesUploaded = new ExPerformanceCounter(base.CategoryName, "Total bytes uploaded", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalBytesUploaded);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Contacts Downloaded/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TotalContactsDownloaded = new ExPerformanceCounter(base.CategoryName, "Total Contacts Downloaded", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TotalContactsDownloaded);
				this.TotalRequests = new ExPerformanceCounter(base.CategoryName, "Total Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRequests);
				this.TotalRequestsWithNoChanges = new ExPerformanceCounter(base.CategoryName, "Total Requests with No Changes", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalRequestsWithNoChanges);
				long num = this.TotalBytesDownloaded.RawValue;
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

		public readonly ExPerformanceCounter TotalBytesDownloaded;

		public readonly ExPerformanceCounter TotalBytesUploaded;

		public readonly ExPerformanceCounter TotalContactsDownloaded;

		public readonly ExPerformanceCounter TotalRequests;

		public readonly ExPerformanceCounter TotalRequestsWithNoChanges;
	}
}
