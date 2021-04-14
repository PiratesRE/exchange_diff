using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxDataReplicationFailedPermanentException : MailboxReplicationPermanentException
	{
		public MailboxDataReplicationFailedPermanentException(LocalizedString failureReason) : base(MrsStrings.MailboxDataReplicationFailed(failureReason))
		{
			this.failureReason = failureReason;
		}

		public MailboxDataReplicationFailedPermanentException(LocalizedString failureReason, Exception innerException) : base(MrsStrings.MailboxDataReplicationFailed(failureReason), innerException)
		{
			this.failureReason = failureReason;
		}

		protected MailboxDataReplicationFailedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.failureReason = (LocalizedString)info.GetValue("failureReason", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("failureReason", this.failureReason);
		}

		public LocalizedString FailureReason
		{
			get
			{
				return this.failureReason;
			}
		}

		private readonly LocalizedString failureReason;
	}
}
