using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BackSyncDataSourceReplicationException : LocalizedException
	{
		public BackSyncDataSourceReplicationException() : base(DirectoryStrings.BackSyncDataSourceReplicationErrorMessage)
		{
		}

		public BackSyncDataSourceReplicationException(Exception innerException) : base(DirectoryStrings.BackSyncDataSourceReplicationErrorMessage, innerException)
		{
		}

		protected BackSyncDataSourceReplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
