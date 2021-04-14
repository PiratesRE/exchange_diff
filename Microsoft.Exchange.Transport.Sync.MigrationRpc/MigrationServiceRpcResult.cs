using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MigrationServiceRpcResult
	{
		internal MigrationServiceRpcResult(MigrationServiceRpcMethodCode methodCode) : this(methodCode, MigrationServiceRpcResultCode.Success, null)
		{
		}

		internal MigrationServiceRpcResult(MigrationServiceRpcMethodCode methodCode, MigrationServiceRpcResultCode resultCode, string errorDetails)
		{
			this.methodCode = methodCode;
			this.resultCode = resultCode;
			this.message = errorDetails;
		}

		internal MigrationServiceRpcResult(MdbefPropertyCollection inputArgs)
		{
			object obj;
			if (!inputArgs.TryGetValue(2936012803U, out obj) || !(obj is int))
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ResultParseError, "Result Code not found in return MbdefPropertyCollection");
			}
			this.resultCode = (MigrationServiceRpcResultCode)((int)obj);
			if (inputArgs.TryGetValue(2684420099U, out obj))
			{
				this.methodCode = (MigrationServiceRpcMethodCode)((int)obj);
			}
			else
			{
				this.methodCode = MigrationServiceRpcMethodCode.None;
			}
			if (inputArgs.TryGetValue(2936340511U, out obj))
			{
				this.message = (string)obj;
			}
		}

		internal MigrationServiceRpcResultCode ResultCode
		{
			get
			{
				return this.resultCode;
			}
		}

		internal MigrationServiceRpcMethodCode MethodCode
		{
			get
			{
				return this.methodCode;
			}
		}

		internal string Message
		{
			get
			{
				return this.message;
			}
		}

		internal bool IsSuccess
		{
			get
			{
				return this.ResultCode == MigrationServiceRpcResultCode.Success;
			}
		}

		internal MdbefPropertyCollection Marshal()
		{
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			mdbefPropertyCollection[2936012803U] = (int)this.ResultCode;
			mdbefPropertyCollection[2684420099U] = (int)this.MethodCode;
			if (this.message != null)
			{
				mdbefPropertyCollection[2936340511U] = this.message;
			}
			this.WriteTo(mdbefPropertyCollection);
			return mdbefPropertyCollection;
		}

		protected void ThrowIfVerifyFails(MigrationServiceRpcMethodCode expectedMethod)
		{
			if (this.MethodCode != MigrationServiceRpcMethodCode.None && this.MethodCode != expectedMethod)
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.IncorrectMethodInvokedError, string.Format("Client requested method {0} to be invoked, but {1} was invoked", expectedMethod, this.MethodCode));
			}
			int num = (int)this.ResultCode;
			if ((num & 4096) != 0)
			{
				return;
			}
			if ((num & 256) != 0)
			{
				if (num == 16644)
				{
					throw new MigrationSubscriptionNotFoundException(this.ResultCode, this.message);
				}
				throw new MigrationObjectNotHostedException(this.ResultCode, this.message);
			}
			else
			{
				if ((num & 32768) != 0)
				{
					throw new MigrationServiceRpcTransientException(this.ResultCode, this.message);
				}
				if ((num & 8192) != 0)
				{
					throw new MigrationCommunicationException(this.ResultCode, this.message);
				}
				if ((num & 16384) != 0)
				{
					throw new MigrationTargetInvocationException(this.ResultCode, this.message);
				}
				throw new InvalidOperationException(string.Format("uknown rpc result code {0}", num));
			}
		}

		protected abstract void WriteTo(MdbefPropertyCollection collection);

		private readonly MigrationServiceRpcResultCode resultCode;

		private readonly MigrationServiceRpcMethodCode methodCode;

		private readonly string message;
	}
}
