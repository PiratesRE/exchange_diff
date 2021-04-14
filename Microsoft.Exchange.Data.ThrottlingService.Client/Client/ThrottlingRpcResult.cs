using System;

namespace Microsoft.Exchange.Data.ThrottlingService.Client
{
	internal enum ThrottlingRpcResult
	{
		Allowed,
		Bypassed,
		Denied,
		Failed
	}
}
