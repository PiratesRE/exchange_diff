using System;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal enum ReturnCode
	{
		RC_SUCCESS,
		RC_ERROR,
		RC_PROTOCOL,
		RC_SYNTAX,
		RC_EOF,
		RC_IMPLEMENTATION,
		RC_SOFTWARE,
		RC_CONFIG,
		RC_MEMORY,
		RC_CONTENTION,
		RC_NOTFOUND,
		RC_DISKSPACE,
		RC_SHUTDOWN,
		RC_EXPIRED,
		RC_TIMEOUT,
		RC_INVALID_PARAMETER,
		RC_LAST
	}
}
