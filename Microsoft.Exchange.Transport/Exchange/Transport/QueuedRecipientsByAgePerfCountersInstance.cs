using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal sealed class QueuedRecipientsByAgePerfCountersInstance : PerformanceCounterInstance
	{
		internal QueuedRecipientsByAgePerfCountersInstance(string instanceName, QueuedRecipientsByAgePerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Queued Recipients By Age")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.SubmissionNormalPriority = new ExPerformanceCounter(base.CategoryName, "Submission Normal Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SubmissionNormalPriority, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionNormalPriority);
				this.InternalHopNormalPriority = new ExPerformanceCounter(base.CategoryName, "Internal Hop Normal Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InternalHopNormalPriority, new ExPerformanceCounter[0]);
				list.Add(this.InternalHopNormalPriority);
				this.InternalMailboxDeliveryNormalPriority = new ExPerformanceCounter(base.CategoryName, "Internal Mailbox Delivery Normal Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InternalMailboxDeliveryNormalPriority, new ExPerformanceCounter[0]);
				list.Add(this.InternalMailboxDeliveryNormalPriority);
				this.ExternalDeliveryNormalPriority = new ExPerformanceCounter(base.CategoryName, "External Delivery Normal Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ExternalDeliveryNormalPriority, new ExPerformanceCounter[0]);
				list.Add(this.ExternalDeliveryNormalPriority);
				long num = this.SubmissionNormalPriority.RawValue;
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

		internal QueuedRecipientsByAgePerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Queued Recipients By Age")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.SubmissionNormalPriority = new ExPerformanceCounter(base.CategoryName, "Submission Normal Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionNormalPriority);
				this.InternalHopNormalPriority = new ExPerformanceCounter(base.CategoryName, "Internal Hop Normal Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalHopNormalPriority);
				this.InternalMailboxDeliveryNormalPriority = new ExPerformanceCounter(base.CategoryName, "Internal Mailbox Delivery Normal Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalMailboxDeliveryNormalPriority);
				this.ExternalDeliveryNormalPriority = new ExPerformanceCounter(base.CategoryName, "External Delivery Normal Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalDeliveryNormalPriority);
				long num = this.SubmissionNormalPriority.RawValue;
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

		public readonly ExPerformanceCounter SubmissionNormalPriority;

		public readonly ExPerformanceCounter InternalHopNormalPriority;

		public readonly ExPerformanceCounter InternalMailboxDeliveryNormalPriority;

		public readonly ExPerformanceCounter ExternalDeliveryNormalPriority;
	}
}
