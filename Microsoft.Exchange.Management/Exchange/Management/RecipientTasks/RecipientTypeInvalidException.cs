using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecipientTypeInvalidException : LocalizedException
	{
		public RecipientTypeInvalidException(string mailboxId) : base(Strings.RecipientTypeInvalidException(mailboxId))
		{
			this.mailboxId = mailboxId;
		}

		public RecipientTypeInvalidException(string mailboxId, Exception innerException) : base(Strings.RecipientTypeInvalidException(mailboxId), innerException)
		{
			this.mailboxId = mailboxId;
		}

		protected RecipientTypeInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
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
