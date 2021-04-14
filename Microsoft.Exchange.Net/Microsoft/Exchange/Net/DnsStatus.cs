using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Net
{
	internal enum DnsStatus
	{
		Success,
		InfoNoRecords,
		InfoDomainNonexistent,
		InfoMxLoopback,
		ErrorInvalidData,
		ErrorExcessiveData,
		InfoTruncated,
		ErrorRetry,
		[EditorBrowsable(EditorBrowsableState.Never)]
		ErrorTimeout,
		[EditorBrowsable(EditorBrowsableState.Never)]
		ErrorDisconnectException,
		[EditorBrowsable(EditorBrowsableState.Never)]
		Pending,
		[EditorBrowsable(EditorBrowsableState.Never)]
		ServerFailure,
		ConfigChanged,
		ErrorSubQueryTimeout,
		ErrorNoDns,
		NoOutboundFrontendServers
	}
}
