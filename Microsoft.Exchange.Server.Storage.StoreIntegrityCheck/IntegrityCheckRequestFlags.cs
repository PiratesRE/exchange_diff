using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	[Flags]
	public enum IntegrityCheckRequestFlags : uint
	{
		None = 0U,
		DetectOnly = 1U,
		Force = 2U,
		Maintenance = 4U,
		Verbose = 2147483648U
	}
}
