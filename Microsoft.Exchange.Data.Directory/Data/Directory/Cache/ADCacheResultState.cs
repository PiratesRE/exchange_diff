using System;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	public enum ADCacheResultState : byte
	{
		Succeed,
		NotFound,
		CNFedObject,
		SoftDeletedObject,
		TenantIsBeingRelocated,
		OranizationIdMismatch,
		CacheModeIsNotRead,
		ExceptionHappened,
		PropertiesMissing,
		WrongForest,
		WritableSession = 32
	}
}
