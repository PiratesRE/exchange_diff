using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal enum ResubmitRequestResponseCode
	{
		Success = 1,
		CannotModifyCompletedRequest,
		ResubmitRequestDoesNotExist,
		CannotRemoveRequestInRunningState,
		UnexpectedError
	}
}
