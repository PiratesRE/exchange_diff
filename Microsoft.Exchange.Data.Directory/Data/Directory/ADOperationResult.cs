using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADOperationResult
	{
		public ADOperationResult(ADOperationErrorCode errorCode, Exception e)
		{
			this.errorCode = errorCode;
			this.exception = e;
		}

		public bool Succeeded
		{
			get
			{
				return this.errorCode == ADOperationErrorCode.Success;
			}
		}

		public ADOperationErrorCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		private ADOperationErrorCode errorCode;

		private Exception exception;

		public static ADOperationResult Success = new ADOperationResult(ADOperationErrorCode.Success, null);
	}
}
