using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecipientNotFoundPermanentException : MailboxReplicationPermanentException
	{
		public RecipientNotFoundPermanentException(Guid mailboxGuid) : base(MrsStrings.RecipientNotFound(mailboxGuid))
		{
			this.mailboxGuid = mailboxGuid;
		}

		public RecipientNotFoundPermanentException(Guid mailboxGuid, Exception innerException) : base(MrsStrings.RecipientNotFound(mailboxGuid), innerException)
		{
			this.mailboxGuid = mailboxGuid;
		}

		protected RecipientNotFoundPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxGuid = (Guid)info.GetValue("mailboxGuid", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxGuid", this.mailboxGuid);
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		private readonly Guid mailboxGuid;
	}
}
