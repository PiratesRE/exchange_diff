using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TaskSchedulerStoppedPermanentException : MailboxReplicationPermanentException
	{
		public TaskSchedulerStoppedPermanentException() : base(MrsStrings.TaskSchedulerStopped)
		{
		}

		public TaskSchedulerStoppedPermanentException(Exception innerException) : base(MrsStrings.TaskSchedulerStopped, innerException)
		{
		}

		protected TaskSchedulerStoppedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
