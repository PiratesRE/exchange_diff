using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal static class HttpStatusCodeExtensions
	{
		public const int InvalidOperationResponseCode = 422;

		public const int TooManyRequestsResponseCode = 429;
	}
}
