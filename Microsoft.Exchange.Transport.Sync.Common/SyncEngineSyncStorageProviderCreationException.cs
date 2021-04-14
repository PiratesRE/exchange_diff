using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SyncEngineSyncStorageProviderCreationException : LocalizedException
	{
		public SyncEngineSyncStorageProviderCreationException() : base(Strings.SyncEngineSyncStorageProviderCreationException)
		{
		}

		public SyncEngineSyncStorageProviderCreationException(Exception innerException) : base(Strings.SyncEngineSyncStorageProviderCreationException, innerException)
		{
		}

		protected SyncEngineSyncStorageProviderCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
