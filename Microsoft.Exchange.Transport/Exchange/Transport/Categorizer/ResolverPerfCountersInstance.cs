using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class ResolverPerfCountersInstance : PerformanceCounterInstance
	{
		internal ResolverPerfCountersInstance(string instanceName, ResolverPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Resolver")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.MessagesRetriedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Retried", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesRetriedTotal);
				this.MessagesCreatedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Created", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesCreatedTotal);
				this.MessagesChippedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Chipped", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesChippedTotal);
				this.FailedRecipientsTotal = new ExPerformanceCounter(base.CategoryName, "Failed Recipients", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailedRecipientsTotal);
				this.UnresolvedOrgRecipientsTotal = new ExPerformanceCounter(base.CategoryName, "Unresolved Org Recipients", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UnresolvedOrgRecipientsTotal);
				this.AmbiguousRecipientsTotal = new ExPerformanceCounter(base.CategoryName, "Ambiguous Recipients", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AmbiguousRecipientsTotal);
				this.LoopRecipientsTotal = new ExPerformanceCounter(base.CategoryName, "Loop Recipients", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LoopRecipientsTotal);
				this.UnresolvedOrgSendersTotal = new ExPerformanceCounter(base.CategoryName, "Unresolved Org Senders", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UnresolvedOrgSendersTotal);
				this.AmbiguousSendersTotal = new ExPerformanceCounter(base.CategoryName, "Ambiguous Senders", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AmbiguousSendersTotal);
				this.CatchAllRecipientsTotal = new ExPerformanceCounter(base.CategoryName, "Message directed to catch-all recipient.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CatchAllRecipientsTotal);
				long num = this.MessagesRetriedTotal.RawValue;
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

		internal ResolverPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Resolver")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.MessagesRetriedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Retried", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesRetriedTotal);
				this.MessagesCreatedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Created", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesCreatedTotal);
				this.MessagesChippedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Chipped", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesChippedTotal);
				this.FailedRecipientsTotal = new ExPerformanceCounter(base.CategoryName, "Failed Recipients", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailedRecipientsTotal);
				this.UnresolvedOrgRecipientsTotal = new ExPerformanceCounter(base.CategoryName, "Unresolved Org Recipients", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UnresolvedOrgRecipientsTotal);
				this.AmbiguousRecipientsTotal = new ExPerformanceCounter(base.CategoryName, "Ambiguous Recipients", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AmbiguousRecipientsTotal);
				this.LoopRecipientsTotal = new ExPerformanceCounter(base.CategoryName, "Loop Recipients", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LoopRecipientsTotal);
				this.UnresolvedOrgSendersTotal = new ExPerformanceCounter(base.CategoryName, "Unresolved Org Senders", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UnresolvedOrgSendersTotal);
				this.AmbiguousSendersTotal = new ExPerformanceCounter(base.CategoryName, "Ambiguous Senders", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AmbiguousSendersTotal);
				this.CatchAllRecipientsTotal = new ExPerformanceCounter(base.CategoryName, "Message directed to catch-all recipient.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CatchAllRecipientsTotal);
				long num = this.MessagesRetriedTotal.RawValue;
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

		public readonly ExPerformanceCounter MessagesRetriedTotal;

		public readonly ExPerformanceCounter MessagesCreatedTotal;

		public readonly ExPerformanceCounter MessagesChippedTotal;

		public readonly ExPerformanceCounter FailedRecipientsTotal;

		public readonly ExPerformanceCounter UnresolvedOrgRecipientsTotal;

		public readonly ExPerformanceCounter AmbiguousRecipientsTotal;

		public readonly ExPerformanceCounter LoopRecipientsTotal;

		public readonly ExPerformanceCounter UnresolvedOrgSendersTotal;

		public readonly ExPerformanceCounter AmbiguousSendersTotal;

		public readonly ExPerformanceCounter CatchAllRecipientsTotal;
	}
}
