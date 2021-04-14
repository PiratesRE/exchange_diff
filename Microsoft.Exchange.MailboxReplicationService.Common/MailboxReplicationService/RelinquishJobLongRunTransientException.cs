using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobLongRunTransientException : RelinquishJobTransientException
	{
		public RelinquishJobLongRunTransientException() : base(MrsStrings.JobHasBeenRelinquishedDueToLongRun)
		{
		}

		public RelinquishJobLongRunTransientException(Exception innerException) : base(MrsStrings.JobHasBeenRelinquishedDueToLongRun, innerException)
		{
		}

		protected RelinquishJobLongRunTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
