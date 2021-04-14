using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InputDataIsInvalidPermanentException : MailboxReplicationPermanentException
	{
		public InputDataIsInvalidPermanentException() : base(MrsStrings.InputDataIsInvalid)
		{
		}

		public InputDataIsInvalidPermanentException(Exception innerException) : base(MrsStrings.InputDataIsInvalid, innerException)
		{
		}

		protected InputDataIsInvalidPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
