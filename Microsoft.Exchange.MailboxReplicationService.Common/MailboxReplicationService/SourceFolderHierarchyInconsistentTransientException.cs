using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SourceFolderHierarchyInconsistentTransientException : MailboxReplicationTransientException
	{
		public SourceFolderHierarchyInconsistentTransientException() : base(MrsStrings.SourceFolderHierarchyInconsistent)
		{
		}

		public SourceFolderHierarchyInconsistentTransientException(Exception innerException) : base(MrsStrings.SourceFolderHierarchyInconsistent, innerException)
		{
		}

		protected SourceFolderHierarchyInconsistentTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
