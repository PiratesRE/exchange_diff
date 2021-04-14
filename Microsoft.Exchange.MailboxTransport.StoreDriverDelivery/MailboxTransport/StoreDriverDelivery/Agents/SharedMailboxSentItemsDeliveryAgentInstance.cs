using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal sealed class SharedMailboxSentItemsDeliveryAgentInstance : PerformanceCounterInstance
	{
		internal SharedMailboxSentItemsDeliveryAgentInstance(string instanceName, SharedMailboxSentItemsDeliveryAgentInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Shared Mailbox Sent Items Agent")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageMessageCopyTime = new ExPerformanceCounter(base.CategoryName, "Average Delivery Time", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageMessageCopyTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageMessageCopyTime);
				this.AverageMessageCopyTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Delivery Time", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageMessageCopyTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageMessageCopyTimeBase);
				this.SentItemsMessages = new ExPerformanceCounter(base.CategoryName, "Number of Sent Item Messages", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SentItemsMessages, new ExPerformanceCounter[0]);
				list.Add(this.SentItemsMessages);
				this.Errors = new ExPerformanceCounter(base.CategoryName, "Number of Errors", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.Errors, new ExPerformanceCounter[0]);
				list.Add(this.Errors);
				long num = this.AverageMessageCopyTime.RawValue;
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

		internal SharedMailboxSentItemsDeliveryAgentInstance(string instanceName) : base(instanceName, "MSExchange Shared Mailbox Sent Items Agent")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageMessageCopyTime = new ExPerformanceCounter(base.CategoryName, "Average Delivery Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMessageCopyTime);
				this.AverageMessageCopyTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Delivery Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMessageCopyTimeBase);
				this.SentItemsMessages = new ExPerformanceCounter(base.CategoryName, "Number of Sent Item Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SentItemsMessages);
				this.Errors = new ExPerformanceCounter(base.CategoryName, "Number of Errors", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.Errors);
				long num = this.AverageMessageCopyTime.RawValue;
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

		public readonly ExPerformanceCounter AverageMessageCopyTime;

		public readonly ExPerformanceCounter AverageMessageCopyTimeBase;

		public readonly ExPerformanceCounter SentItemsMessages;

		public readonly ExPerformanceCounter Errors;
	}
}
