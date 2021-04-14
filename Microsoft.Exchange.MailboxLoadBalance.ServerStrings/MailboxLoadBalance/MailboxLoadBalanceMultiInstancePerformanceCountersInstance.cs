using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	internal sealed class MailboxLoadBalanceMultiInstancePerformanceCountersInstance : PerformanceCounterInstance
	{
		internal MailboxLoadBalanceMultiInstancePerformanceCountersInstance(string instanceName, MailboxLoadBalanceMultiInstancePerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Mailbox Load Balancing Queues")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.InjectionQueueLength = new ExPerformanceCounter(base.CategoryName, "Injection Queue Length", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InjectionQueueLength, new ExPerformanceCounter[0]);
				list.Add(this.InjectionQueueLength);
				this.InjectionRate = new ExPerformanceCounter(base.CategoryName, "Injection Rate: Injections/Sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InjectionRate, new ExPerformanceCounter[0]);
				list.Add(this.InjectionRate);
				this.InjectionRateBase = new ExPerformanceCounter(base.CategoryName, "Injection Rate: Injections/Sec (base)", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InjectionRateBase, new ExPerformanceCounter[0]);
				list.Add(this.InjectionRateBase);
				this.InjectionRequestRate = new ExPerformanceCounter(base.CategoryName, "Injection Request Rate: Requests/Sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InjectionRequestRate, new ExPerformanceCounter[0]);
				list.Add(this.InjectionRequestRate);
				this.InjectionRequestRateBase = new ExPerformanceCounter(base.CategoryName, "Injection Request Rate: Requests/Sec (base)", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InjectionRequestRateBase, new ExPerformanceCounter[0]);
				list.Add(this.InjectionRequestRateBase);
				this.ProcessingQueueLength = new ExPerformanceCounter(base.CategoryName, "Processing Queue Length", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ProcessingQueueLength, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingQueueLength);
				this.ProcessingRate = new ExPerformanceCounter(base.CategoryName, "Processing Rate: Processings/Sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ProcessingRate, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingRate);
				this.ProcessingRateBase = new ExPerformanceCounter(base.CategoryName, "Processing Rate: Processings/Sec (base)", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ProcessingRateBase, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingRateBase);
				this.ProcessingRequestRate = new ExPerformanceCounter(base.CategoryName, "Processing Request Rate: Requests/Sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ProcessingRequestRate, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingRequestRate);
				this.ProcessingRequestRateBase = new ExPerformanceCounter(base.CategoryName, "Processing Request Rate: Requests/Sec (base)", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ProcessingRequestRateBase, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingRequestRateBase);
				long num = this.InjectionQueueLength.RawValue;
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

		internal MailboxLoadBalanceMultiInstancePerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchange Mailbox Load Balancing Queues")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.InjectionQueueLength = new ExPerformanceCounter(base.CategoryName, "Injection Queue Length", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.InjectionQueueLength);
				this.InjectionRate = new ExPerformanceCounter(base.CategoryName, "Injection Rate: Injections/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.InjectionRate);
				this.InjectionRateBase = new ExPerformanceCounter(base.CategoryName, "Injection Rate: Injections/Sec (base)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.InjectionRateBase);
				this.InjectionRequestRate = new ExPerformanceCounter(base.CategoryName, "Injection Request Rate: Requests/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.InjectionRequestRate);
				this.InjectionRequestRateBase = new ExPerformanceCounter(base.CategoryName, "Injection Request Rate: Requests/Sec (base)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.InjectionRequestRateBase);
				this.ProcessingQueueLength = new ExPerformanceCounter(base.CategoryName, "Processing Queue Length", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingQueueLength);
				this.ProcessingRate = new ExPerformanceCounter(base.CategoryName, "Processing Rate: Processings/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingRate);
				this.ProcessingRateBase = new ExPerformanceCounter(base.CategoryName, "Processing Rate: Processings/Sec (base)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingRateBase);
				this.ProcessingRequestRate = new ExPerformanceCounter(base.CategoryName, "Processing Request Rate: Requests/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingRequestRate);
				this.ProcessingRequestRateBase = new ExPerformanceCounter(base.CategoryName, "Processing Request Rate: Requests/Sec (base)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingRequestRateBase);
				long num = this.InjectionQueueLength.RawValue;
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

		public readonly ExPerformanceCounter InjectionQueueLength;

		public readonly ExPerformanceCounter InjectionRate;

		public readonly ExPerformanceCounter InjectionRateBase;

		public readonly ExPerformanceCounter InjectionRequestRate;

		public readonly ExPerformanceCounter InjectionRequestRateBase;

		public readonly ExPerformanceCounter ProcessingQueueLength;

		public readonly ExPerformanceCounter ProcessingRate;

		public readonly ExPerformanceCounter ProcessingRateBase;

		public readonly ExPerformanceCounter ProcessingRequestRate;

		public readonly ExPerformanceCounter ProcessingRequestRateBase;
	}
}
