using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobCIStallTransientException : RelinquishJobTransientException
	{
		public RelinquishJobCIStallTransientException() : base(MrsStrings.JobHasBeenRelinquishedDueToCIStall)
		{
		}

		public RelinquishJobCIStallTransientException(Exception innerException) : base(MrsStrings.JobHasBeenRelinquishedDueToCIStall, innerException)
		{
		}

		protected RelinquishJobCIStallTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
