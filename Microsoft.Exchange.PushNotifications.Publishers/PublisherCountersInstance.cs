using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class PublisherCountersInstance : PerformanceCounterInstance
	{
		internal PublisherCountersInstance(string instanceName, PublisherCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Push Notifications Publishers")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.QueueSize = new ExPerformanceCounter(base.CategoryName, "Publisher Queue Size - Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.QueueSize);
				this.QueueSizePeak = new ExPerformanceCounter(base.CategoryName, "Publisher Queue Size - Peak", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.QueueSizePeak);
				this.QueueSizeTotal = new ExPerformanceCounter(base.CategoryName, "Publisher Queue Size - Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.QueueSizeTotal);
				this.EnqueueError = new ExPerformanceCounter(base.CategoryName, "Publisher Enqueue Error - Counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EnqueueError);
				this.PreprocessError = new ExPerformanceCounter(base.CategoryName, "Preprocess Error - Counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PreprocessError);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Total Notifications Sent/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalNotificationsSent = new ExPerformanceCounter(base.CategoryName, "Total Notifications Sent - Counter", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalNotificationsSent);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Total Notifications Discarded/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalNotificationsDiscarded = new ExPerformanceCounter(base.CategoryName, "Total Notifications Discarded - Counter", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalNotificationsDiscarded);
				this.AveragePublisherSendTime = new ExPerformanceCounter(base.CategoryName, "Publisher Send - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AveragePublisherSendTime);
				this.AveragePublisherSendTimeBase = new ExPerformanceCounter(base.CategoryName, "Publisher Send - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AveragePublisherSendTimeBase);
				this.AveragePreprocessTime = new ExPerformanceCounter(base.CategoryName, "Preprocess - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AveragePreprocessTime);
				this.AveragePreprocessTimeBase = new ExPerformanceCounter(base.CategoryName, "Preprocess - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AveragePreprocessTimeBase);
				this.AverageQueueItemTime = new ExPerformanceCounter(base.CategoryName, "Queue Item - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageQueueItemTime);
				this.AverageQueueItemTimeBase = new ExPerformanceCounter(base.CategoryName, "Queue Item - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageQueueItemTimeBase);
				long num = this.QueueSize.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter3 in list)
					{
						exPerformanceCounter3.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal PublisherCountersInstance(string instanceName) : base(instanceName, "MSExchange Push Notifications Publishers")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.QueueSize = new ExPerformanceCounter(base.CategoryName, "Publisher Queue Size - Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.QueueSize);
				this.QueueSizePeak = new ExPerformanceCounter(base.CategoryName, "Publisher Queue Size - Peak", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.QueueSizePeak);
				this.QueueSizeTotal = new ExPerformanceCounter(base.CategoryName, "Publisher Queue Size - Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.QueueSizeTotal);
				this.EnqueueError = new ExPerformanceCounter(base.CategoryName, "Publisher Enqueue Error - Counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EnqueueError);
				this.PreprocessError = new ExPerformanceCounter(base.CategoryName, "Preprocess Error - Counter", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PreprocessError);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Total Notifications Sent/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TotalNotificationsSent = new ExPerformanceCounter(base.CategoryName, "Total Notifications Sent - Counter", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TotalNotificationsSent);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Total Notifications Discarded/Sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TotalNotificationsDiscarded = new ExPerformanceCounter(base.CategoryName, "Total Notifications Discarded - Counter", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TotalNotificationsDiscarded);
				this.AveragePublisherSendTime = new ExPerformanceCounter(base.CategoryName, "Publisher Send - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AveragePublisherSendTime);
				this.AveragePublisherSendTimeBase = new ExPerformanceCounter(base.CategoryName, "Publisher Send - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AveragePublisherSendTimeBase);
				this.AveragePreprocessTime = new ExPerformanceCounter(base.CategoryName, "Preprocess - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AveragePreprocessTime);
				this.AveragePreprocessTimeBase = new ExPerformanceCounter(base.CategoryName, "Preprocess - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AveragePreprocessTimeBase);
				this.AverageQueueItemTime = new ExPerformanceCounter(base.CategoryName, "Queue Item - Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageQueueItemTime);
				this.AverageQueueItemTimeBase = new ExPerformanceCounter(base.CategoryName, "Queue Item - Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageQueueItemTimeBase);
				long num = this.QueueSize.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter3 in list)
					{
						exPerformanceCounter3.Close();
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

		public readonly ExPerformanceCounter QueueSize;

		public readonly ExPerformanceCounter QueueSizePeak;

		public readonly ExPerformanceCounter QueueSizeTotal;

		public readonly ExPerformanceCounter EnqueueError;

		public readonly ExPerformanceCounter PreprocessError;

		public readonly ExPerformanceCounter TotalNotificationsSent;

		public readonly ExPerformanceCounter TotalNotificationsDiscarded;

		public readonly ExPerformanceCounter AveragePublisherSendTime;

		public readonly ExPerformanceCounter AveragePublisherSendTimeBase;

		public readonly ExPerformanceCounter AveragePreprocessTime;

		public readonly ExPerformanceCounter AveragePreprocessTimeBase;

		public readonly ExPerformanceCounter AverageQueueItemTime;

		public readonly ExPerformanceCounter AverageQueueItemTimeBase;
	}
}
