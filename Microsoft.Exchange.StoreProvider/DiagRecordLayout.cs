using System;

namespace Microsoft.Mapi
{
	internal enum DiagRecordLayout
	{
		Header,
		dwParam,
		GenericError,
		WindowsError,
		StoreError,
		InfoEx1,
		PtagError,
		Long,
		Guid,
		RpcCall = 16,
		RpcReturn,
		RpcException,
		DeadRpcPool,
		Version,
		Custom = 255
	}
}
