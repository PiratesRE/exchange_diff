using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MoveRequestMissingInfoSavePermanentException : MailboxReplicationPermanentException
	{
		public MoveRequestMissingInfoSavePermanentException() : base(MrsStrings.MoveRequestMissingInfoSave)
		{
		}

		public MoveRequestMissingInfoSavePermanentException(Exception innerException) : base(MrsStrings.MoveRequestMissingInfoSave, innerException)
		{
		}

		protected MoveRequestMissingInfoSavePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
