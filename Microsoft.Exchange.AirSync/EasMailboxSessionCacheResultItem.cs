using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.AirSync
{
	[XmlType("CacheEntry")]
	public class EasMailboxSessionCacheResultItem
	{
		public EasMailboxSessionCacheResultItem()
		{
		}

		public EasMailboxSessionCacheResultItem(string id, TimeSpan timeToLive)
		{
			this.Identifier = id;
			this.TimeToLive = timeToLive.ToString();
		}

		[XmlElement("Id")]
		public string Identifier { get; set; }

		public string TimeToLive { get; set; }
	}
}
