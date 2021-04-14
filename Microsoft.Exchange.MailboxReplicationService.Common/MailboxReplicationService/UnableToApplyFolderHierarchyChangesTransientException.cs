using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToApplyFolderHierarchyChangesTransientException : MailboxReplicationTransientException
	{
		public UnableToApplyFolderHierarchyChangesTransientException() : base(MrsStrings.UnableToApplyFolderHierarchyChanges)
		{
		}

		public UnableToApplyFolderHierarchyChangesTransientException(Exception innerException) : base(MrsStrings.UnableToApplyFolderHierarchyChanges, innerException)
		{
		}

		protected UnableToApplyFolderHierarchyChangesTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
