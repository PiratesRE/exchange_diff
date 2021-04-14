using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	[Flags]
	internal enum ChainEngineOptions : uint
	{
		CacheEndCert = 1U,
		ThreadStoreSync = 2U,
		CacheOnlyUrlRetrieval = 4U,
		UseLocalMachineStore = 8U,
		EnableCacheAutoUpdate = 16U,
		EnableShareStore = 32U
	}
}
