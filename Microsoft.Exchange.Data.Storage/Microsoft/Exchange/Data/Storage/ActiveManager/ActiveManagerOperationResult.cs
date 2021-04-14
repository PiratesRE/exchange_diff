using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActiveManagerOperationResult
	{
		public ActiveManagerOperationResult(bool succeeded, Exception ex)
		{
			this.exception = ex;
			if (succeeded)
			{
				this.errorCode = ActiveManagerOperationResultCode.Success;
				return;
			}
			if (this.Exception is DatabaseNotFoundException)
			{
				this.errorCode = ActiveManagerOperationResultCode.TransientError;
				return;
			}
			if (this.Exception is StorageTransientException)
			{
				this.errorCode = ActiveManagerOperationResultCode.TransientError;
				return;
			}
			if (this.Exception is ObjectNotFoundException)
			{
				this.errorCode = ActiveManagerOperationResultCode.PermanentError;
				return;
			}
			if (this.Exception is StoragePermanentException)
			{
				this.errorCode = ActiveManagerOperationResultCode.PermanentError;
				return;
			}
			if (this.Exception is ServerForDatabaseNotFoundException)
			{
				this.errorCode = ActiveManagerOperationResultCode.ServerForDatabaseNotFoundException;
				return;
			}
			this.errorCode = ActiveManagerOperationResultCode.PermanentError;
		}

		public bool Succeeded
		{
			get
			{
				return this.errorCode == ActiveManagerOperationResultCode.Success;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public ActiveManagerOperationResultCode ResultCode
		{
			get
			{
				return this.errorCode;
			}
		}

		private ActiveManagerOperationResultCode errorCode;

		private Exception exception;
	}
}
