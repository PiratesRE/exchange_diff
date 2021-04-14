using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderIsMissingTransientException : MailboxReplicationTransientException
	{
		public FolderIsMissingTransientException() : base(MrsStrings.FolderIsMissingInMerge)
		{
		}

		public FolderIsMissingTransientException(Exception innerException) : base(MrsStrings.FolderIsMissingInMerge, innerException)
		{
		}

		protected FolderIsMissingTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
