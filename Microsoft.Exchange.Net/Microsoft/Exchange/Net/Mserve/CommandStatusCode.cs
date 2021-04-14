using System;

namespace Microsoft.Exchange.Net.Mserve
{
	internal enum CommandStatusCode
	{
		Success = 1,
		NotAuthorized = 3103,
		UserDoesNotExist = 3201,
		UserAlreadyExists = 3215,
		ConcurrentOperation = 4108,
		InvalidAccountName = 4202,
		MserveCacheServiceChannelFaulted = 5101
	}
}
