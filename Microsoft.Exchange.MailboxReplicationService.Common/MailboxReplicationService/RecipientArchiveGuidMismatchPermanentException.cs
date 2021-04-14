using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecipientArchiveGuidMismatchPermanentException : MailboxReplicationPermanentException
	{
		public RecipientArchiveGuidMismatchPermanentException(string recipient, Guid recipientArchiveGuid, Guid targetArchiveGuid) : base(MrsStrings.RecipientArchiveGuidMismatch(recipient, recipientArchiveGuid, targetArchiveGuid))
		{
			this.recipient = recipient;
			this.recipientArchiveGuid = recipientArchiveGuid;
			this.targetArchiveGuid = targetArchiveGuid;
		}

		public RecipientArchiveGuidMismatchPermanentException(string recipient, Guid recipientArchiveGuid, Guid targetArchiveGuid, Exception innerException) : base(MrsStrings.RecipientArchiveGuidMismatch(recipient, recipientArchiveGuid, targetArchiveGuid), innerException)
		{
			this.recipient = recipient;
			this.recipientArchiveGuid = recipientArchiveGuid;
			this.targetArchiveGuid = targetArchiveGuid;
		}

		protected RecipientArchiveGuidMismatchPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recipient = (string)info.GetValue("recipient", typeof(string));
			this.recipientArchiveGuid = (Guid)info.GetValue("recipientArchiveGuid", typeof(Guid));
			this.targetArchiveGuid = (Guid)info.GetValue("targetArchiveGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recipient", this.recipient);
			info.AddValue("recipientArchiveGuid", this.recipientArchiveGuid);
			info.AddValue("targetArchiveGuid", this.targetArchiveGuid);
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		public Guid RecipientArchiveGuid
		{
			get
			{
				return this.recipientArchiveGuid;
			}
		}

		public Guid TargetArchiveGuid
		{
			get
			{
				return this.targetArchiveGuid;
			}
		}

		private readonly string recipient;

		private readonly Guid recipientArchiveGuid;

		private readonly Guid targetArchiveGuid;
	}
}
