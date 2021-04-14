using System;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal enum ExecuteRequestFlag : uint
	{
		NoCompression = 1U,
		NoObfuscation,
		Chain = 4U,
		ExtendedError = 8U
	}
}
