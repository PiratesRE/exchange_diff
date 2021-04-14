using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OperationRetryManagerResult
	{
		public OperationRetryManagerResult(OperationRetryManagerResultCode resultCode, Exception ex)
		{
			if (ex == null && resultCode != OperationRetryManagerResultCode.Success)
			{
				throw new ArgumentNullException("ex", "ex can't be null if resultCode != ResultCode.Success");
			}
			this.resultCode = resultCode;
			this.exception = ex;
		}

		public static OperationRetryManagerResult Success
		{
			get
			{
				return OperationRetryManagerResult.successOperationResult;
			}
		}

		public bool Succeeded
		{
			get
			{
				return this.resultCode == OperationRetryManagerResultCode.Success;
			}
		}

		public OperationRetryManagerResultCode ResultCode
		{
			get
			{
				return this.resultCode;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		private readonly OperationRetryManagerResultCode resultCode;

		private static OperationRetryManagerResult successOperationResult = new OperationRetryManagerResult(OperationRetryManagerResultCode.Success, null);

		private Exception exception;
	}
}
