using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterNotFoundPermanentException : MailboxReplicationPermanentException
	{
		public ClusterNotFoundPermanentException() : base(MrsStrings.ClusterNotFound)
		{
		}

		public ClusterNotFoundPermanentException(Exception innerException) : base(MrsStrings.ClusterNotFound, innerException)
		{
		}

		protected ClusterNotFoundPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
