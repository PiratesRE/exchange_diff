using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.AirSync
{
	public class NotificationManagerResult
	{
		public int CreatesPerMinute { get; set; }

		public int HitsPerMinute { get; set; }

		public int ContentionsPerMinute { get; set; }

		public int StealsPerMinute { get; set; }

		public int CacheCount { get; set; }

		public int ActiveCount { get; set; }

		public int RemovedCount { get; set; }

		public int InactiveCount { get; set; }

		[XmlArrayItem("Instance")]
		public List<NotificationManagerResultItem> ActiveInstances { get; set; }

		[XmlArrayItem("Instance")]
		public List<NotificationManagerResultItem> InactiveInstances { get; set; }

		[XmlArrayItem("Instance")]
		public List<NotificationManagerResultItem> RemovedInstances { get; set; }
	}
}
