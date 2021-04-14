using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MSExchangeDatabasePingerInstance : PerformanceCounterInstance
	{
		internal MSExchangeDatabasePingerInstance(string instanceName, MSExchangeDatabasePingerInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Database Pinger")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.PingsPerMinute = new ExPerformanceCounter(base.CategoryName, "Pings Per Minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PingsPerMinute);
				this.FailedPings = new ExPerformanceCounter(base.CategoryName, "Failed Pings", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailedPings);
				this.PingTimeouts = new ExPerformanceCounter(base.CategoryName, "Number of Ping Timeouts", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PingTimeouts);
				this.CacheSize = new ExPerformanceCounter(base.CategoryName, "Cache Size", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheSize);
				long num = this.PingsPerMinute.RawValue;
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

		internal MSExchangeDatabasePingerInstance(string instanceName) : base(instanceName, "MSExchange Database Pinger")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.PingsPerMinute = new ExPerformanceCounter(base.CategoryName, "Pings Per Minute", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PingsPerMinute);
				this.FailedPings = new ExPerformanceCounter(base.CategoryName, "Failed Pings", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailedPings);
				this.PingTimeouts = new ExPerformanceCounter(base.CategoryName, "Number of Ping Timeouts", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PingTimeouts);
				this.CacheSize = new ExPerformanceCounter(base.CategoryName, "Cache Size", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CacheSize);
				long num = this.PingsPerMinute.RawValue;
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

		public readonly ExPerformanceCounter PingsPerMinute;

		public readonly ExPerformanceCounter FailedPings;

		public readonly ExPerformanceCounter PingTimeouts;

		public readonly ExPerformanceCounter CacheSize;
	}
}
