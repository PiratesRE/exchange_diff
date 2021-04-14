using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public struct EventWatermark
	{
		public EventWatermark(Guid mailboxGuid, Guid consumerGuid, long eventCounter)
		{
			this.mailboxGuid = mailboxGuid;
			this.consumerGuid = consumerGuid;
			this.eventCounter = eventCounter;
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public Guid ConsumerGuid
		{
			get
			{
				return this.consumerGuid;
			}
		}

		public long EventCounter
		{
			get
			{
				return this.eventCounter;
			}
		}

		private Guid mailboxGuid;

		private Guid consumerGuid;

		private long eventCounter;
	}
}
