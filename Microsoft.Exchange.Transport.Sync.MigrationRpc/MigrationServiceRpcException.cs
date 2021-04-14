using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationServiceRpcException : MigrationPermanentException
	{
		internal MigrationServiceRpcException(MigrationServiceRpcResultCode resultCode, string message) : base(Strings.MigrationRpcFailed(resultCode.ToString()), message)
		{
			this.ResultCode = resultCode;
		}

		internal MigrationServiceRpcException(MigrationServiceRpcResultCode resultCode, string message, Exception innerException) : base(Strings.MigrationRpcFailed(resultCode.ToString()), message, innerException)
		{
			this.ResultCode = resultCode;
		}

		internal MigrationServiceRpcResultCode ResultCode
		{
			get
			{
				return (MigrationServiceRpcResultCode)this.Data["ResultCode"];
			}
			private set
			{
				this.Data["ResultCode"] = value;
			}
		}
	}
}
