using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RecipientCounts
	{
		internal RecipientCounts(int delivered, int pending, int transferred, int unsuccessful)
		{
			this.deliveredRecipients = delivered;
			this.pendingRecipients = pending;
			this.transferredRecipients = transferred;
			this.unsuccessfulRecipients = unsuccessful;
		}

		[DataMember]
		public int Delivered
		{
			get
			{
				return this.deliveredRecipients;
			}
			private set
			{
				this.deliveredRecipients = value;
			}
		}

		[DataMember]
		public int Pending
		{
			get
			{
				return this.pendingRecipients;
			}
			private set
			{
				this.pendingRecipients = value;
			}
		}

		[DataMember]
		public int Transferred
		{
			get
			{
				return this.transferredRecipients;
			}
			private set
			{
				this.transferredRecipients = value;
			}
		}

		[DataMember]
		public int Unsuccessful
		{
			get
			{
				return this.unsuccessfulRecipients;
			}
			private set
			{
				this.unsuccessfulRecipients = value;
			}
		}

		[DataMember]
		public int Total
		{
			get
			{
				return this.deliveredRecipients + this.pendingRecipients + this.transferredRecipients + this.unsuccessfulRecipients;
			}
			private set
			{
			}
		}

		[DataMember]
		public int MaxRecipientsInList
		{
			get
			{
				return 30;
			}
			private set
			{
			}
		}

		private int deliveredRecipients;

		private int pendingRecipients;

		private int unsuccessfulRecipients;

		private int transferredRecipients;
	}
}
