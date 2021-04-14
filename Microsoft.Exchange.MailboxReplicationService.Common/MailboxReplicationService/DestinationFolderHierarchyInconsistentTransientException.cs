using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DestinationFolderHierarchyInconsistentTransientException : MailboxReplicationTransientException
	{
		public DestinationFolderHierarchyInconsistentTransientException() : base(MrsStrings.DestinationFolderHierarchyInconsistent)
		{
		}

		public DestinationFolderHierarchyInconsistentTransientException(Exception innerException) : base(MrsStrings.DestinationFolderHierarchyInconsistent, innerException)
		{
		}

		protected DestinationFolderHierarchyInconsistentTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
