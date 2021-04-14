using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class RequestJobTimestampData
	{
		public RequestJobTimestampData()
		{
		}

		public RequestJobTimestampData(RequestJobTimestamp id, DateTime timestamp)
		{
			this.id = id;
			this.timestamp = timestamp;
		}

		[XmlIgnore]
		public RequestJobTimestamp Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		[XmlAttribute("Id")]
		public int IdInt
		{
			get
			{
				return (int)this.id;
			}
			set
			{
				this.id = (RequestJobTimestamp)value;
			}
		}

		[XmlAttribute("T")]
		public DateTime Timestamp
		{
			get
			{
				return this.timestamp;
			}
			set
			{
				this.timestamp = value;
			}
		}

		private RequestJobTimestamp id;

		private DateTime timestamp;
	}
}
