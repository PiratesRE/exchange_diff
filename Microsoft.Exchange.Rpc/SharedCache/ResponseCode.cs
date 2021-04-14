using System;

namespace Microsoft.Exchange.Rpc.SharedCache
{
	public enum ResponseCode
	{
		OK,
		NoOp,
		KeyNotFound,
		InternalServerError,
		CacheGuidNotFound,
		RpcError,
		Timeout,
		InvalidInsertTimestamp,
		EntryCorrupt,
		TooManyOutstandingRequests
	}
}
