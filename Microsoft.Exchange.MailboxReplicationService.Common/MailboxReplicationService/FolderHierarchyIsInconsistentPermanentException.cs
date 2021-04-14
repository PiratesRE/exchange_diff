using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderHierarchyIsInconsistentPermanentException : MailboxReplicationPermanentException
	{
		public FolderHierarchyIsInconsistentPermanentException(LocalizedString message) : base(message)
		{
		}

		public FolderHierarchyIsInconsistentPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected FolderHierarchyIsInconsistentPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
