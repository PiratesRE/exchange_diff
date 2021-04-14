using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobThrottledTransientException : RelinquishJobTransientException
	{
		public RelinquishJobThrottledTransientException() : base(MrsStrings.JobHasBeenRelinquished)
		{
		}

		public RelinquishJobThrottledTransientException(Exception innerException) : base(MrsStrings.JobHasBeenRelinquished, innerException)
		{
		}

		protected RelinquishJobThrottledTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
