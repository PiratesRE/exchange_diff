using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class ActiveManagerDagNamePerfmonInstance : PerformanceCounterInstance
	{
		internal ActiveManagerDagNamePerfmonInstance(string instanceName, ActiveManagerDagNamePerfmonInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Active Manager Dag Name Instance")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ActiveManagerRoleInDag = new ExPerformanceCounter(base.CategoryName, "Active Manager Role In Dag", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveManagerRoleInDag);
				long num = this.ActiveManagerRoleInDag.RawValue;
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

		internal ActiveManagerDagNamePerfmonInstance(string instanceName) : base(instanceName, "MSExchange Active Manager Dag Name Instance")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ActiveManagerRoleInDag = new ExPerformanceCounter(base.CategoryName, "Active Manager Role In Dag", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveManagerRoleInDag);
				long num = this.ActiveManagerRoleInDag.RawValue;
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

		public readonly ExPerformanceCounter ActiveManagerRoleInDag;
	}
}
