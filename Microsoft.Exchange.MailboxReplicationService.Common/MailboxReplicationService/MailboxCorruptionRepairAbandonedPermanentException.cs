using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxCorruptionRepairAbandonedPermanentException : MailboxReplicationPermanentException
	{
		public MailboxCorruptionRepairAbandonedPermanentException(DateTime firstRepairAttemptedAt) : base(MrsStrings.IsIntegTooLongError(firstRepairAttemptedAt))
		{
			this.firstRepairAttemptedAt = firstRepairAttemptedAt;
		}

		public MailboxCorruptionRepairAbandonedPermanentException(DateTime firstRepairAttemptedAt, Exception innerException) : base(MrsStrings.IsIntegTooLongError(firstRepairAttemptedAt), innerException)
		{
			this.firstRepairAttemptedAt = firstRepairAttemptedAt;
		}

		protected MailboxCorruptionRepairAbandonedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.firstRepairAttemptedAt = (DateTime)info.GetValue("firstRepairAttemptedAt", typeof(DateTime));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("firstRepairAttemptedAt", this.firstRepairAttemptedAt);
		}

		public DateTime FirstRepairAttemptedAt
		{
			get
			{
				return this.firstRepairAttemptedAt;
			}
		}

		private readonly DateTime firstRepairAttemptedAt;
	}
}
