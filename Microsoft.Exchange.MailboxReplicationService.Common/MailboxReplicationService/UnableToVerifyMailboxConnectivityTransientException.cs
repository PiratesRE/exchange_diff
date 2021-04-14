using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToVerifyMailboxConnectivityTransientException : MailboxReplicationTransientException
	{
		public UnableToVerifyMailboxConnectivityTransientException(LocalizedString mailboxId) : base(MrsStrings.UnableToVerifyMailboxConnectivity(mailboxId))
		{
			this.mailboxId = mailboxId;
		}

		public UnableToVerifyMailboxConnectivityTransientException(LocalizedString mailboxId, Exception innerException) : base(MrsStrings.UnableToVerifyMailboxConnectivity(mailboxId), innerException)
		{
			this.mailboxId = mailboxId;
		}

		protected UnableToVerifyMailboxConnectivityTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxId = (LocalizedString)info.GetValue("mailboxId", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxId", this.mailboxId);
		}

		public LocalizedString MailboxId
		{
			get
			{
				return this.mailboxId;
			}
		}

		private readonly LocalizedString mailboxId;
	}
}
