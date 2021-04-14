using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderFilterPermanentException : MailboxReplicationPermanentException
	{
		public FolderFilterPermanentException(LocalizedString message) : base(message)
		{
		}

		public FolderFilterPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected FolderFilterPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
