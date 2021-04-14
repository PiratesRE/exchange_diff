using System;

namespace Microsoft.Exchange.Data.Directory.Diagnostics
{
	internal enum Operation : byte
	{
		None,
		GetOperation,
		RemoveOperation,
		PutOperation,
		WCFGetOperation,
		WCFRemoveOperation,
		WCFPutOperation,
		ObjectInitialization,
		ObjectCreation,
		TotalWCFGetOperation,
		TotalWCFRemoveOperation,
		TotalWCFPutOperation,
		WCFBeginOperation,
		WCFEndOperation,
		DataSize,
		TenantRelocationCheck,
		WCFProxyObjectCreation
	}
}
