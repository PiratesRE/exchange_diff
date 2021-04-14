using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecipientAggregatedMailboxNotFoundPermanentException : MailboxReplicationPermanentException
	{
		public RecipientAggregatedMailboxNotFoundPermanentException(string recipient, string recipientAggregatedMailboxGuidsAsString, Guid targetAggregatedMailboxGuid) : base(MrsStrings.RecipientAggregatedMailboxNotFound(recipient, recipientAggregatedMailboxGuidsAsString, targetAggregatedMailboxGuid))
		{
			this.recipient = recipient;
			this.recipientAggregatedMailboxGuidsAsString = recipientAggregatedMailboxGuidsAsString;
			this.targetAggregatedMailboxGuid = targetAggregatedMailboxGuid;
		}

		public RecipientAggregatedMailboxNotFoundPermanentException(string recipient, string recipientAggregatedMailboxGuidsAsString, Guid targetAggregatedMailboxGuid, Exception innerException) : base(MrsStrings.RecipientAggregatedMailboxNotFound(recipient, recipientAggregatedMailboxGuidsAsString, targetAggregatedMailboxGuid), innerException)
		{
			this.recipient = recipient;
			this.recipientAggregatedMailboxGuidsAsString = recipientAggregatedMailboxGuidsAsString;
			this.targetAggregatedMailboxGuid = targetAggregatedMailboxGuid;
		}

		protected RecipientAggregatedMailboxNotFoundPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recipient = (string)info.GetValue("recipient", typeof(string));
			this.recipientAggregatedMailboxGuidsAsString = (string)info.GetValue("recipientAggregatedMailboxGuidsAsString", typeof(string));
			this.targetAggregatedMailboxGuid = (Guid)info.GetValue("targetAggregatedMailboxGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recipient", this.recipient);
			info.AddValue("recipientAggregatedMailboxGuidsAsString", this.recipientAggregatedMailboxGuidsAsString);
			info.AddValue("targetAggregatedMailboxGuid", this.targetAggregatedMailboxGuid);
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		public string RecipientAggregatedMailboxGuidsAsString
		{
			get
			{
				return this.recipientAggregatedMailboxGuidsAsString;
			}
		}

		public Guid TargetAggregatedMailboxGuid
		{
			get
			{
				return this.targetAggregatedMailboxGuid;
			}
		}

		private readonly string recipient;

		private readonly string recipientAggregatedMailboxGuidsAsString;

		private readonly Guid targetAggregatedMailboxGuid;
	}
}
