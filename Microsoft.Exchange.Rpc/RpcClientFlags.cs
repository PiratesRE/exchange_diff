using System;

namespace Microsoft.Exchange.Rpc
{
	[Flags]
	internal enum RpcClientFlags : uint
	{
		None = 0U,
		AllowImpersonation = 1U,
		UseEncryptedConnection = 2U,
		IgnoreInvalidServerCertificate = 4U,
		UniqueBinding = 8U,
		ExplicitEndpointLookup = 16U,
		UseSsl = 32U
	}
}
