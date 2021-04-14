using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InternalAccessFolderCreationIsNotSupportedException : MailboxReplicationPermanentException
	{
		public InternalAccessFolderCreationIsNotSupportedException() : base(MrsStrings.InternalAccessFolderCreationIsNotSupported)
		{
		}

		public InternalAccessFolderCreationIsNotSupportedException(Exception innerException) : base(MrsStrings.InternalAccessFolderCreationIsNotSupported, innerException)
		{
		}

		protected InternalAccessFolderCreationIsNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
