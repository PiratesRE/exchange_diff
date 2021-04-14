using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToUnlockMailboxDueToOutstandingMoveRequestException : LocalizedException
	{
		public UnableToUnlockMailboxDueToOutstandingMoveRequestException(string mailboxId, string moveStatus) : base(Strings.UnableToUnlockMailboxDueToOutstandingMoveRequest(mailboxId, moveStatus))
		{
			this.mailboxId = mailboxId;
			this.moveStatus = moveStatus;
		}

		public UnableToUnlockMailboxDueToOutstandingMoveRequestException(string mailboxId, string moveStatus, Exception innerException) : base(Strings.UnableToUnlockMailboxDueToOutstandingMoveRequest(mailboxId, moveStatus), innerException)
		{
			this.mailboxId = mailboxId;
			this.moveStatus = moveStatus;
		}

		protected UnableToUnlockMailboxDueToOutstandingMoveRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxId = (string)info.GetValue("mailboxId", typeof(string));
			this.moveStatus = (string)info.GetValue("moveStatus", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxId", this.mailboxId);
			info.AddValue("moveStatus", this.moveStatus);
		}

		public string MailboxId
		{
			get
			{
				return this.mailboxId;
			}
		}

		public string MoveStatus
		{
			get
			{
				return this.moveStatus;
			}
		}

		private readonly string mailboxId;

		private readonly string moveStatus;
	}
}
