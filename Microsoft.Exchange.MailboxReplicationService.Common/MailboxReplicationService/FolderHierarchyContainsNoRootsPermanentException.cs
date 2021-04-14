using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderHierarchyContainsNoRootsPermanentException : FolderHierarchyIsInconsistentPermanentException
	{
		public FolderHierarchyContainsNoRootsPermanentException() : base(MrsStrings.FolderHierarchyContainsNoRoots)
		{
		}

		public FolderHierarchyContainsNoRootsPermanentException(Exception innerException) : base(MrsStrings.FolderHierarchyContainsNoRoots, innerException)
		{
		}

		protected FolderHierarchyContainsNoRootsPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
