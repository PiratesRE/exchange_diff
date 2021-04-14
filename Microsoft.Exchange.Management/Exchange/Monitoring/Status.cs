using System;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class Status
	{
		public Status(StatusType statusType)
		{
			this.statusType = statusType;
		}

		public StatusType StatusType
		{
			get
			{
				return this.statusType;
			}
		}

		public string StatusMessage
		{
			get
			{
				return this.statusMessage;
			}
			internal set
			{
				this.statusMessage = value;
			}
		}

		public DateTime StartDateTime
		{
			get
			{
				return this.startDateTime;
			}
			internal set
			{
				this.startDateTime = value;
			}
		}

		public DateTime? EndDateTime
		{
			get
			{
				return this.endDateTime;
			}
			internal set
			{
				this.endDateTime = value;
			}
		}

		public int ImpactedUserCount
		{
			get
			{
				return this.impactedUserCount;
			}
			internal set
			{
				this.impactedUserCount = value;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", this.StatusType, this.StatusMessage);
		}

		private StatusType statusType;

		private string statusMessage;

		private DateTime startDateTime;

		private DateTime? endDateTime;

		private int impactedUserCount;
	}
}
