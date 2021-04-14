using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ProvisioningMailboxNotFoundException : ObjectNotFoundException
	{
		public ProvisioningMailboxNotFoundException(string mailboxId) : base(ServerStrings.ProvisioningMailboxNotFound(mailboxId))
		{
			this.mailboxId = mailboxId;
		}

		public ProvisioningMailboxNotFoundException(string mailboxId, Exception innerException) : base(ServerStrings.ProvisioningMailboxNotFound(mailboxId), innerException)
		{
			this.mailboxId = mailboxId;
		}

		protected ProvisioningMailboxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxId = (string)info.GetValue("mailboxId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxId", this.mailboxId);
		}

		public string MailboxId
		{
			get
			{
				return this.mailboxId;
			}
		}

		private readonly string mailboxId;
	}
}
