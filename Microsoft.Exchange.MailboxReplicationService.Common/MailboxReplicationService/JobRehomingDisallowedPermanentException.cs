using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class JobRehomingDisallowedPermanentException : MailboxReplicationPermanentException
	{
		public JobRehomingDisallowedPermanentException() : base(MrsStrings.JobCannotBeRehomedWhenInProgress)
		{
		}

		public JobRehomingDisallowedPermanentException(Exception innerException) : base(MrsStrings.JobCannotBeRehomedWhenInProgress, innerException)
		{
		}

		protected JobRehomingDisallowedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
