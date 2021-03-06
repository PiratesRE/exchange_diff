using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoPublicFolderMailboxFoundInSourceException : MailboxReplicationPermanentException
	{
		public NoPublicFolderMailboxFoundInSourceException() : base(MrsStrings.NoPublicFolderMailboxFoundInSource)
		{
		}

		public NoPublicFolderMailboxFoundInSourceException(Exception innerException) : base(MrsStrings.NoPublicFolderMailboxFoundInSource, innerException)
		{
		}

		protected NoPublicFolderMailboxFoundInSourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
