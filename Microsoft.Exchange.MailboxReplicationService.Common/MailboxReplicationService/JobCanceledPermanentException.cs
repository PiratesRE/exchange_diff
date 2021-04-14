using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class JobCanceledPermanentException : MailboxReplicationPermanentException
	{
		public JobCanceledPermanentException() : base(MrsStrings.JobHasBeenCanceled)
		{
		}

		public JobCanceledPermanentException(Exception innerException) : base(MrsStrings.JobHasBeenCanceled, innerException)
		{
		}

		protected JobCanceledPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
