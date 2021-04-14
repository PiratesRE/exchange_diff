using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class ActiveManagerPerfmonInstance : PerformanceCounterInstance
	{
		internal ActiveManagerPerfmonInstance(string instanceName, ActiveManagerPerfmonInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Active Manager")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.IsMounted = new ExPerformanceCounter(base.CategoryName, "Database Mounted", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.IsMounted, new ExPerformanceCounter[0]);
				list.Add(this.IsMounted);
				this.CopyRoleIsActive = new ExPerformanceCounter(base.CategoryName, "Database Copy Role Active", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CopyRoleIsActive, new ExPerformanceCounter[0]);
				list.Add(this.CopyRoleIsActive);
				long num = this.IsMounted.RawValue;
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

		internal ActiveManagerPerfmonInstance(string instanceName) : base(instanceName, "MSExchange Active Manager")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.IsMounted = new ExPerformanceCounter(base.CategoryName, "Database Mounted", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.IsMounted);
				this.CopyRoleIsActive = new ExPerformanceCounter(base.CategoryName, "Database Copy Role Active", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CopyRoleIsActive);
				long num = this.IsMounted.RawValue;
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

		public readonly ExPerformanceCounter IsMounted;

		public readonly ExPerformanceCounter CopyRoleIsActive;
	}
}
