using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.AirSync
{
	public class NotificationManagerResultItem
	{
		public string UniqueId { get; set; }

		public string Command { get; set; }

		public string EmailAddress { get; set; }

		public string DeviceId { get; set; }

		public long PolicyKey { get; set; }

		public string LiveTime { get; set; }

		public int QueueCount { get; set; }

		public int TotalAcquires { get; set; }

		public int TotalKills { get; set; }

		public int TotalReleases { get; set; }

		public int TotalTimeouts { get; set; }

		public int TotalXsoEvents { get; set; }

		public int TotalXsoExceptions { get; set; }

		public bool IsExecuting { get; set; }

		public string RequestedWaitTime { get; set; }

		[XmlArrayItem("Action")]
		public List<InstanceAction> Actions { get; set; }

		[XmlArrayItem("Event")]
		public List<string> QueuedEvents { get; set; }
	}
}
