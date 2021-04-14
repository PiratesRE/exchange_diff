using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RootFolderNotFoundPermananentException : MailboxReplicationPermanentException
	{
		public RootFolderNotFoundPermananentException() : base(MrsStrings.MailboxRootFolderNotFound)
		{
		}

		public RootFolderNotFoundPermananentException(Exception innerException) : base(MrsStrings.MailboxRootFolderNotFound, innerException)
		{
		}

		protected RootFolderNotFoundPermananentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
