using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum SaveResult
	{
		Success,
		SuccessWithConflictResolution,
		IrresolvableConflict,
		SuccessWithoutSaving
	}
}
