using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.DataProviders
{
	public static class ConflictResolution
	{
		internal static void ThrowOnIrresolvableConflict(this ConflictResolutionResult result)
		{
			if (result.SaveStatus == SaveResult.IrresolvableConflict)
			{
				throw new IrresolvableConflictException(result);
			}
		}
	}
}
