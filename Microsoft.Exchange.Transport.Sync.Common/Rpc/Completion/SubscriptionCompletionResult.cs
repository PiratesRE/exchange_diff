using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Completion
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SubscriptionCompletionResult
	{
		public static bool IsSuccess(uint errorCode)
		{
			return (errorCode & 4026531840U) == 0U;
		}

		public static string GetStringForErrorCode(uint errorCode)
		{
			uint num = errorCode;
			if (num == 0U)
			{
				return "Success";
			}
			switch (num)
			{
			case 268435457U:
				return "ServerVersionMismatch";
			case 268435458U:
				return "ServerStopped";
			case 268435459U:
				return "InvalidSubscriptionMessageId";
			default:
				return errorCode.ToString("X");
			}
		}

		public const uint Success = 0U;

		public const uint ServerVersionMismatch = 268435457U;

		public const uint ServerStopped = 268435458U;

		public const uint InvalidSubscriptionMessageId = 268435459U;

		public const uint InvalidSubscription = 268435460U;

		private const uint SuccessBit = 0U;

		private const uint UnexpectedErrorBit = 268435456U;

		private const uint CategoryBit = 4026531840U;
	}
}
