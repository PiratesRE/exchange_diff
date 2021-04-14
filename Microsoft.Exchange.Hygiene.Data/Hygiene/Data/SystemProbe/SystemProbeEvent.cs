using System;

namespace Microsoft.Exchange.Hygiene.Data.SystemProbe
{
	[Serializable]
	public class SystemProbeEvent
	{
		public Guid MessageId
		{
			get
			{
				return this.messageId;
			}
			set
			{
				this.messageId = value;
			}
		}

		public Guid EventId
		{
			get
			{
				return this.eventId;
			}
			set
			{
				this.eventId = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return this.timeStamp;
			}
			set
			{
				this.timeStamp = value;
			}
		}

		public string ServerHostName
		{
			get
			{
				return this.serverHostName;
			}
			set
			{
				this.serverHostName = value;
			}
		}

		public string ComponentName
		{
			get
			{
				return this.componentName;
			}
			set
			{
				this.componentName = value;
			}
		}

		public SystemProbeEvent.Status EventStatus
		{
			get
			{
				return this.eventStatus;
			}
			set
			{
				this.eventStatus = value;
			}
		}

		public string EventMessage
		{
			get
			{
				return this.eventMessage;
			}
			set
			{
				this.eventMessage = value;
			}
		}

		private Guid messageId;

		private Guid eventId;

		private DateTime timeStamp;

		private string serverHostName;

		private string componentName;

		private SystemProbeEvent.Status eventStatus;

		private string eventMessage;

		public enum Status
		{
			Pass,
			Fail
		}
	}
}
