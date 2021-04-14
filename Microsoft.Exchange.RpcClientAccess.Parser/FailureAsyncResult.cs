using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal class FailureAsyncResult<TErrorCode> : EasyCancelableAsyncResult
	{
		public FailureAsyncResult(TErrorCode errorCode, IntPtr contextHandle, Exception exception, CancelableAsyncCallback asyncCallback, object asyncState) : base(asyncCallback, asyncState)
		{
			this.errorCode = errorCode;
			this.contextHandle = contextHandle;
			this.exception = exception;
		}

		public TErrorCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public IntPtr ContextHandle
		{
			get
			{
				return this.contextHandle;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		protected override void InternalCancel()
		{
		}

		private readonly TErrorCode errorCode;

		private readonly IntPtr contextHandle;

		private readonly Exception exception;
	}
}
