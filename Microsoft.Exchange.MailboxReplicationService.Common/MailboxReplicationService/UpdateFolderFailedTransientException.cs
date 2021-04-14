using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UpdateFolderFailedTransientException : MailboxReplicationTransientException
	{
		public UpdateFolderFailedTransientException() : base(MrsStrings.UpdateFolderFailed)
		{
		}

		public UpdateFolderFailedTransientException(Exception innerException) : base(MrsStrings.UpdateFolderFailed, innerException)
		{
		}

		protected UpdateFolderFailedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
