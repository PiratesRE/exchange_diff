using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal sealed class MwiLoadBalancerPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal MwiLoadBalancerPerformanceCountersInstance(string instanceName, MwiLoadBalancerPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeUMMessageWaitingIndicator")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalMwiMessages = new ExPerformanceCounter(base.CategoryName, "Total MWI Messages", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalMwiMessages);
				this.TotalFailedMwiMessages = new ExPerformanceCounter(base.CategoryName, "Total Failed MWI Messages", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalFailedMwiMessages);
				this.AverageMwiProcessingTime = new ExPerformanceCounter(base.CategoryName, "Average MWI Processing Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMwiProcessingTime);
				long num = this.TotalMwiMessages.RawValue;
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

		internal MwiLoadBalancerPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangeUMMessageWaitingIndicator")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalMwiMessages = new ExPerformanceCounter(base.CategoryName, "Total MWI Messages", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalMwiMessages);
				this.TotalFailedMwiMessages = new ExPerformanceCounter(base.CategoryName, "Total Failed MWI Messages", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalFailedMwiMessages);
				this.AverageMwiProcessingTime = new ExPerformanceCounter(base.CategoryName, "Average MWI Processing Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMwiProcessingTime);
				long num = this.TotalMwiMessages.RawValue;
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

		public readonly ExPerformanceCounter TotalMwiMessages;

		public readonly ExPerformanceCounter TotalFailedMwiMessages;

		public readonly ExPerformanceCounter AverageMwiProcessingTime;
	}
}
