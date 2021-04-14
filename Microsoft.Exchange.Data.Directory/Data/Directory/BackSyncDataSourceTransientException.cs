using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class BackSyncDataSourceTransientException : LocalizedException
	{
		public BackSyncDataSourceTransientException() : base(DirectoryStrings.BackSyncDataSourceTransientErrorMessage)
		{
		}

		public BackSyncDataSourceTransientException(Exception innerException) : base(DirectoryStrings.BackSyncDataSourceTransientErrorMessage, innerException)
		{
		}

		protected BackSyncDataSourceTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
