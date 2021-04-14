using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	[Flags]
	internal enum FastTransferSendOption : byte
	{
		UseMAPI = 0,
		Unicode = 1,
		UseCpId = 2,
		Upload = 3,
		RecoverMode = 4,
		ForceUnicode = 8,
		PartialItem = 16,
		SendPropErrors = 32,
		StripLargeRules = 64
	}
}
