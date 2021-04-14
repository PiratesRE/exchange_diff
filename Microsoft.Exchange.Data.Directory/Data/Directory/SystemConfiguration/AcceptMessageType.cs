using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum AcceptMessageType
	{
		Default = 0,
		LegacyOOF = 1,
		AutoReply = 2,
		AutoForward = 4,
		DR = 8,
		NDR = 16,
		BlockOOF = 32,
		InternalDomain = 64,
		MFN = 128,
		TargetDeliveryDomain = 256,
		UseSimpleDisplayName = 512,
		NDRDiagnosticInfoDisabled = 1024
	}
}
