using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationServiceRpcTransientException : MigrationTransientException
	{
		internal MigrationServiceRpcTransientException(int rpcErrorCode, bool isConnectionError, Exception innerException) : base(Strings.MigrationRpcFailed(rpcErrorCode.ToString(CultureInfo.InvariantCulture)), innerException)
		{
			this.isConnectionError = isConnectionError;
			this.rpcErrorCode = new int?(rpcErrorCode);
			this.resultCode = MigrationServiceRpcResultCode.RpcTransientException;
		}

		internal MigrationServiceRpcTransientException(MigrationServiceRpcResultCode resultCode, string message) : base(Strings.MigrationRpcFailed(resultCode.ToString()), message)
		{
			this.isConnectionError = false;
			this.rpcErrorCode = null;
			this.resultCode = resultCode;
		}

		public int? RpcErrorCode
		{
			get
			{
				return this.rpcErrorCode;
			}
		}

		public bool IsConnectionError
		{
			get
			{
				return this.isConnectionError;
			}
		}

		internal MigrationServiceRpcResultCode ResultCode
		{
			get
			{
				return this.resultCode;
			}
		}

		private readonly int? rpcErrorCode;

		private readonly bool isConnectionError;

		private readonly MigrationServiceRpcResultCode resultCode;
	}
}
