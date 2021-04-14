using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal sealed class MessageDepotPerfCountersInstance : PerformanceCounterInstance
	{
		internal MessageDepotPerfCountersInstance(string instanceName, MessageDepotPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport MessageDepot")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ReadyMessages = new ExPerformanceCounter(base.CategoryName, "Ready Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ReadyMessages);
				this.DeferredMessages = new ExPerformanceCounter(base.CategoryName, "Deferred Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DeferredMessages);
				this.SuspendedMessages = new ExPerformanceCounter(base.CategoryName, "Suspended Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SuspendedMessages);
				this.PoisonMessages = new ExPerformanceCounter(base.CategoryName, "Poison Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PoisonMessages);
				this.RetryMessages = new ExPerformanceCounter(base.CategoryName, "Retry Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RetryMessages);
				this.ProcessingMessages = new ExPerformanceCounter(base.CategoryName, "Processing Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingMessages);
				this.ExpiringMessages = new ExPerformanceCounter(base.CategoryName, "Expiring Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpiringMessages);
				long num = this.ReadyMessages.RawValue;
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

		internal MessageDepotPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport MessageDepot")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ReadyMessages = new ExPerformanceCounter(base.CategoryName, "Ready Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ReadyMessages);
				this.DeferredMessages = new ExPerformanceCounter(base.CategoryName, "Deferred Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DeferredMessages);
				this.SuspendedMessages = new ExPerformanceCounter(base.CategoryName, "Suspended Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SuspendedMessages);
				this.PoisonMessages = new ExPerformanceCounter(base.CategoryName, "Poison Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PoisonMessages);
				this.RetryMessages = new ExPerformanceCounter(base.CategoryName, "Retry Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RetryMessages);
				this.ProcessingMessages = new ExPerformanceCounter(base.CategoryName, "Processing Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingMessages);
				this.ExpiringMessages = new ExPerformanceCounter(base.CategoryName, "Expiring Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpiringMessages);
				long num = this.ReadyMessages.RawValue;
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

		public readonly ExPerformanceCounter ReadyMessages;

		public readonly ExPerformanceCounter DeferredMessages;

		public readonly ExPerformanceCounter SuspendedMessages;

		public readonly ExPerformanceCounter PoisonMessages;

		public readonly ExPerformanceCounter RetryMessages;

		public readonly ExPerformanceCounter ProcessingMessages;

		public readonly ExPerformanceCounter ExpiringMessages;
	}
}
