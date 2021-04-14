using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToReadADPermanentException : MailboxReplicationPermanentException
	{
		public UnableToReadADPermanentException() : base(MrsStrings.UnableToReadAD)
		{
		}

		public UnableToReadADPermanentException(Exception innerException) : base(MrsStrings.UnableToReadAD, innerException)
		{
		}

		protected UnableToReadADPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
