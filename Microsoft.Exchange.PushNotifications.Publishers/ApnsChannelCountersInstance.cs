using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class ApnsChannelCountersInstance : PerformanceCounterInstance
	{
		internal ApnsChannelCountersInstance(string instanceName, ApnsChannelCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Push Notifications Apns Channel")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ApnsConnectionFailed = new ExPerformanceCounter(base.CategoryName, "Apns Connection Failed - Counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ApnsConnectionFailed);
				this.ApnsReadTaskEnded = new ExPerformanceCounter(base.CategoryName, "Apns Read Task Ended - Counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ApnsReadTaskEnded);
				this.ApnsChannelReset = new ExPerformanceCounter(base.CategoryName, "Apns Reset - Counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ApnsChannelReset);
				this.AverageApnsConnectionTime = new ExPerformanceCounter(base.CategoryName, "Apns Connection - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsConnectionTime);
				this.AverageApnsConnectionTimeBase = new ExPerformanceCounter(base.CategoryName, "Apns Connection - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsConnectionTimeBase);
				this.AverageApnsAuthTime = new ExPerformanceCounter(base.CategoryName, "Apns Auth - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsAuthTime);
				this.AverageApnsAuthTimeBase = new ExPerformanceCounter(base.CategoryName, "Apns Auth - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsAuthTimeBase);
				this.AverageApnsChannelSendTime = new ExPerformanceCounter(base.CategoryName, "Apns Channel Send - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsChannelSendTime);
				this.AverageApnsChannelSendTimeBase = new ExPerformanceCounter(base.CategoryName, "Apns Channel Send - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsChannelSendTimeBase);
				this.AverageApnsResponseReadTime = new ExPerformanceCounter(base.CategoryName, "Apns Response Read - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsResponseReadTime);
				this.AverageApnsResponseReadTimeBase = new ExPerformanceCounter(base.CategoryName, "Apns Response Read - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsResponseReadTimeBase);
				this.AverageApnsChannelOpenTime = new ExPerformanceCounter(base.CategoryName, "Apns Channel Connection Open - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsChannelOpenTime);
				this.AverageApnsChannelOpenTimeBase = new ExPerformanceCounter(base.CategoryName, "Apns Channel Connection Open - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsChannelOpenTimeBase);
				long num = this.ApnsConnectionFailed.RawValue;
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

		internal ApnsChannelCountersInstance(string instanceName) : base(instanceName, "MSExchange Push Notifications Apns Channel")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ApnsConnectionFailed = new ExPerformanceCounter(base.CategoryName, "Apns Connection Failed - Counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ApnsConnectionFailed);
				this.ApnsReadTaskEnded = new ExPerformanceCounter(base.CategoryName, "Apns Read Task Ended - Counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ApnsReadTaskEnded);
				this.ApnsChannelReset = new ExPerformanceCounter(base.CategoryName, "Apns Reset - Counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ApnsChannelReset);
				this.AverageApnsConnectionTime = new ExPerformanceCounter(base.CategoryName, "Apns Connection - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsConnectionTime);
				this.AverageApnsConnectionTimeBase = new ExPerformanceCounter(base.CategoryName, "Apns Connection - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsConnectionTimeBase);
				this.AverageApnsAuthTime = new ExPerformanceCounter(base.CategoryName, "Apns Auth - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsAuthTime);
				this.AverageApnsAuthTimeBase = new ExPerformanceCounter(base.CategoryName, "Apns Auth - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsAuthTimeBase);
				this.AverageApnsChannelSendTime = new ExPerformanceCounter(base.CategoryName, "Apns Channel Send - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsChannelSendTime);
				this.AverageApnsChannelSendTimeBase = new ExPerformanceCounter(base.CategoryName, "Apns Channel Send - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsChannelSendTimeBase);
				this.AverageApnsResponseReadTime = new ExPerformanceCounter(base.CategoryName, "Apns Response Read - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsResponseReadTime);
				this.AverageApnsResponseReadTimeBase = new ExPerformanceCounter(base.CategoryName, "Apns Response Read - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsResponseReadTimeBase);
				this.AverageApnsChannelOpenTime = new ExPerformanceCounter(base.CategoryName, "Apns Channel Connection Open - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsChannelOpenTime);
				this.AverageApnsChannelOpenTimeBase = new ExPerformanceCounter(base.CategoryName, "Apns Channel Connection Open - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageApnsChannelOpenTimeBase);
				long num = this.ApnsConnectionFailed.RawValue;
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

		public readonly ExPerformanceCounter ApnsConnectionFailed;

		public readonly ExPerformanceCounter ApnsReadTaskEnded;

		public readonly ExPerformanceCounter ApnsChannelReset;

		public readonly ExPerformanceCounter AverageApnsConnectionTime;

		public readonly ExPerformanceCounter AverageApnsConnectionTimeBase;

		public readonly ExPerformanceCounter AverageApnsAuthTime;

		public readonly ExPerformanceCounter AverageApnsAuthTimeBase;

		public readonly ExPerformanceCounter AverageApnsChannelSendTime;

		public readonly ExPerformanceCounter AverageApnsChannelSendTimeBase;

		public readonly ExPerformanceCounter AverageApnsResponseReadTime;

		public readonly ExPerformanceCounter AverageApnsResponseReadTimeBase;

		public readonly ExPerformanceCounter AverageApnsChannelOpenTime;

		public readonly ExPerformanceCounter AverageApnsChannelOpenTimeBase;
	}
}
