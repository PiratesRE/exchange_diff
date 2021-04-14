using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CorruptRestrictionDataException : MailboxReplicationPermanentException
	{
		public CorruptRestrictionDataException() : base(MrsStrings.CorruptRestrictionData)
		{
		}

		public CorruptRestrictionDataException(Exception innerException) : base(MrsStrings.CorruptRestrictionData, innerException)
		{
		}

		protected CorruptRestrictionDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
