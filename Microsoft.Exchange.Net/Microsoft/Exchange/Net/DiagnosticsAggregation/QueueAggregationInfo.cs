using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[Serializable]
	public class QueueAggregationInfo
	{
		public QueueAggregationInfo()
		{
		}

		public QueueAggregationInfo(List<LocalQueueInfo> queueInfo, DateTime time)
		{
			this.queueInfo = queueInfo;
			this.Time = time;
		}

		public List<LocalQueueInfo> QueueInfo
		{
			get
			{
				return this.queueInfo;
			}
			set
			{
				this.queueInfo = value;
			}
		}

		public DateTime Time { get; set; }

		public int TotalMessageCount { get; set; }

		public int PoisonMessageCount { get; set; }

		private List<LocalQueueInfo> queueInfo = new List<LocalQueueInfo>();
	}
}
