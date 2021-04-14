using System;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	[Flags]
	internal enum ChainPolicyOptions : uint
	{
		None = 0U,
		IgnoreNotTimeValid = 1U,
		IgnoreCTLNotTimeValid = 2U,
		IgnoreNotTimeNested = 4U,
		IgnoreInvalidBasicConstraints = 8U,
		AllowUnknownCA = 16U,
		IgnoreWrongUsage = 32U,
		IgnoreInvalidName = 64U,
		IgnoreInvalidPolicy = 128U,
		IgnoreEndRevUnknown = 256U,
		IgnoreCTLSignerRevUnknown = 512U,
		IgnoreCARevUnknown = 1024U,
		IgnoreRootRevUnknown = 2048U,
		AllowTestRoot = 32768U,
		TrustTestRoot = 16384U
	}
}
