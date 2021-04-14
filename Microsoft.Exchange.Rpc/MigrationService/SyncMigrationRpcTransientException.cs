using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc.MigrationService
{
	[Serializable]
	internal class SyncMigrationRpcTransientException : RpcException
	{
		public SyncMigrationRpcTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public SyncMigrationRpcTransientException(string message, int hr) : base(message, hr)
		{
		}

		public SyncMigrationRpcTransientException(string message) : base(message)
		{
		}
	}
}
