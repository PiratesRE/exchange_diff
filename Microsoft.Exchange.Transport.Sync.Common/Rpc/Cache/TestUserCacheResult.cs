using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Rpc.Cache
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class TestUserCacheResult
	{
		public const uint Success = 0U;

		public const uint UnexpectedError = 268435456U;

		public const uint ServerStopped = 268435457U;

		public const uint ServerVersionMismatch = 268435458U;

		private const uint SuccessBit = 0U;

		private const uint UnexpectedErrorBit = 268435456U;

		private const uint CategoryBit = 4026531840U;
	}
}
