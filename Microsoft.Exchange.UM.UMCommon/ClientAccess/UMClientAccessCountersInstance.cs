using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.ClientAccess
{
	internal sealed class UMClientAccessCountersInstance : PerformanceCounterInstance
	{
		internal UMClientAccessCountersInstance(string instanceName, UMClientAccessCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeUMClientAccess")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalPlayOnPhoneRequests = new ExPerformanceCounter(base.CategoryName, "Total Number of Play on Phone Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalPlayOnPhoneRequests);
				this.TotalPlayOnPhoneErrors = new ExPerformanceCounter(base.CategoryName, "Total Number of Failed Play on Phone Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalPlayOnPhoneErrors);
				this.TotalPINResetRequests = new ExPerformanceCounter(base.CategoryName, "Total Number of PIN Reset Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalPINResetRequests);
				this.TotalPINResetErrors = new ExPerformanceCounter(base.CategoryName, "Total Number of Failed PIN Reset Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalPINResetErrors);
				this.PID = new ExPerformanceCounter(base.CategoryName, "PID", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PID);
				long num = this.TotalPlayOnPhoneRequests.RawValue;
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

		internal UMClientAccessCountersInstance(string instanceName) : base(instanceName, "MSExchangeUMClientAccess")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalPlayOnPhoneRequests = new ExPerformanceCounter(base.CategoryName, "Total Number of Play on Phone Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalPlayOnPhoneRequests);
				this.TotalPlayOnPhoneErrors = new ExPerformanceCounter(base.CategoryName, "Total Number of Failed Play on Phone Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalPlayOnPhoneErrors);
				this.TotalPINResetRequests = new ExPerformanceCounter(base.CategoryName, "Total Number of PIN Reset Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalPINResetRequests);
				this.TotalPINResetErrors = new ExPerformanceCounter(base.CategoryName, "Total Number of Failed PIN Reset Requests", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalPINResetErrors);
				this.PID = new ExPerformanceCounter(base.CategoryName, "PID", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PID);
				long num = this.TotalPlayOnPhoneRequests.RawValue;
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

		public readonly ExPerformanceCounter TotalPlayOnPhoneRequests;

		public readonly ExPerformanceCounter TotalPlayOnPhoneErrors;

		public readonly ExPerformanceCounter TotalPINResetRequests;

		public readonly ExPerformanceCounter TotalPINResetErrors;

		public readonly ExPerformanceCounter PID;
	}
}
