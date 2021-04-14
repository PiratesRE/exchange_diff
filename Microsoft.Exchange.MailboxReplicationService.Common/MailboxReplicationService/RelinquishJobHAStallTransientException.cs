using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobHAStallTransientException : RelinquishJobTransientException
	{
		public RelinquishJobHAStallTransientException() : base(MrsStrings.JobHasBeenRelinquishedDueToHAStall)
		{
		}

		public RelinquishJobHAStallTransientException(Exception innerException) : base(MrsStrings.JobHasBeenRelinquishedDueToHAStall, innerException)
		{
		}

		protected RelinquishJobHAStallTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
