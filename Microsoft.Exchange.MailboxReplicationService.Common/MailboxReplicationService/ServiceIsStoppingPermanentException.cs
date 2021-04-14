using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceIsStoppingPermanentException : MailboxReplicationPermanentException
	{
		public ServiceIsStoppingPermanentException() : base(MrsStrings.ServiceIsStopping)
		{
		}

		public ServiceIsStoppingPermanentException(Exception innerException) : base(MrsStrings.ServiceIsStopping, innerException)
		{
		}

		protected ServiceIsStoppingPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
