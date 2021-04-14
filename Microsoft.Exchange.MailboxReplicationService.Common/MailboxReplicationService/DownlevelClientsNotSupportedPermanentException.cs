using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DownlevelClientsNotSupportedPermanentException : MailboxReplicationPermanentException
	{
		public DownlevelClientsNotSupportedPermanentException() : base(MrsStrings.ErrorDownlevelClientsNotSupported)
		{
		}

		public DownlevelClientsNotSupportedPermanentException(Exception innerException) : base(MrsStrings.ErrorDownlevelClientsNotSupported, innerException)
		{
		}

		protected DownlevelClientsNotSupportedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
