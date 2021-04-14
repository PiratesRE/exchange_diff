using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	internal enum ActiveManagerOperationResultCode
	{
		Success,
		TransientError,
		PermanentError,
		ServerForDatabaseNotFoundException
	}
}
