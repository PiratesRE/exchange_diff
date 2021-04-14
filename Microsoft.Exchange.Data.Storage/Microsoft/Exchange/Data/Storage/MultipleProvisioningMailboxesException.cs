using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MultipleProvisioningMailboxesException : LocalizedException
	{
		public MultipleProvisioningMailboxesException(string mailboxId) : base(ServerStrings.MultipleProvisioningMailboxes(mailboxId))
		{
			this.mailboxId = mailboxId;
		}

		public MultipleProvisioningMailboxesException(string mailboxId, Exception innerException) : base(ServerStrings.MultipleProvisioningMailboxes(mailboxId), innerException)
		{
			this.mailboxId = mailboxId;
		}

		protected MultipleProvisioningMailboxesException(SerializationInfo info, StreamingContext context) : base(info, context)
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
