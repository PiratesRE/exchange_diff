using System;

namespace Microsoft.Exchange.Data
{
	internal enum OperationRetryManagerResultCode
	{
		Success,
		RetryableError,
		PermanentError
	}
}
