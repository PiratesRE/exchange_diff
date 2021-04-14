using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public enum OperationDetail
	{
		KindMultiple = 1000,
		None = 0,
		MapiObjectType = 1000,
		DoMaintenanceTask = 2000,
		StoreIntegrityCheck = 3000,
		AdminExecuteTask = 4000,
		MapiStreamType = 5000
	}
}
