using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ExRpcConnectionCreateFlag
	{
		None = 0,
		UseConMod = 1,
		UseLcidString = 2,
		UseLcidSort = 4,
		UseCpid = 8,
		UseReconnectInterval = 16,
		UseRpcBufferSize = 32,
		UseAuxBufferSize = 64,
		LegacyCall = 65536,
		CompressUp = 131072,
		CompressDown = 262144,
		PackedUp = 524288,
		PackedDown = 1048576,
		XorMagicUp = 2097152,
		XorMagicDown = 4194304,
		WebServices = 8388608
	}
}
