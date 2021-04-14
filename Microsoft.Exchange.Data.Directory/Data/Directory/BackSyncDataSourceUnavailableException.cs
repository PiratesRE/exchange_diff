using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BackSyncDataSourceUnavailableException : LocalizedException
	{
		public BackSyncDataSourceUnavailableException() : base(DirectoryStrings.BackSyncDataSourceUnavailableMessage)
		{
		}

		public BackSyncDataSourceUnavailableException(Exception innerException) : base(DirectoryStrings.BackSyncDataSourceUnavailableMessage, innerException)
		{
		}

		protected BackSyncDataSourceUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
