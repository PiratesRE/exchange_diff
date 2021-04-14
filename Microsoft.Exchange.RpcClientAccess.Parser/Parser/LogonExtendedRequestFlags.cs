using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum LogonExtendedRequestFlags : uint
	{
		None = 0U,
		UseLocaleInfo = 1U,
		SetAuthContext = 2U,
		AuthContextCompressed = 4U,
		ApplicationId = 8U,
		ReturnLocaleInfo = 16U,
		TenantHint = 32U,
		UnifiedLogon = 64U
	}
}
