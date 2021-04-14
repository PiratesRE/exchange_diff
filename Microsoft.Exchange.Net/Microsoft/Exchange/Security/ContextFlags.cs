using System;

namespace Microsoft.Exchange.Security
{
	[Flags]
	internal enum ContextFlags
	{
		Zero = 0,
		Delegate = 1,
		MutualAuth = 2,
		ReplayDetect = 4,
		SequenceDetect = 8,
		Confidentiality = 16,
		UseSessionKey = 32,
		AllocateMemory = 256,
		Connection = 2048,
		InitExtendedError = 16384,
		AcceptExtendedError = 32768,
		InitStream = 32768,
		AcceptStream = 65536,
		InitIntegrity = 65536,
		AcceptIntegrity = 131072,
		InitNullSession = 262144,
		AcceptNullSession = 1048576,
		AcceptAllowNonUserLogons = 2097152,
		AcceptNoToken = 16777216,
		InitManualCredValidation = 524288,
		InitUseSuppliedCreds = 128,
		InitIdentify = 131072,
		AcceptIdentify = 524288,
		AllowMissingBindings = 268435456,
		ProxyBindings = 67108864
	}
}
