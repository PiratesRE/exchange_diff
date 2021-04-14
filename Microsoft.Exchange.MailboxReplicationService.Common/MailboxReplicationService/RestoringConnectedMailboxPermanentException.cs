using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RestoringConnectedMailboxPermanentException : MailboxReplicationPermanentException
	{
		public RestoringConnectedMailboxPermanentException(Guid mailboxGuid) : base(MrsStrings.RestoringConnectedMailboxError(mailboxGuid))
		{
			this.mailboxGuid = mailboxGuid;
		}

		public RestoringConnectedMailboxPermanentException(Guid mailboxGuid, Exception innerException) : base(MrsStrings.RestoringConnectedMailboxError(mailboxGuid), innerException)
		{
			this.mailboxGuid = mailboxGuid;
		}

		protected RestoringConnectedMailboxPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
