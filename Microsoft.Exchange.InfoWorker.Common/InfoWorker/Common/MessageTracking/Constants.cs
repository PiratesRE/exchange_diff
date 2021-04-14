using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal static class Constants
	{
		public const int ExpectedNodesInFailoverGroup = 5;

		public const int DefaultClientSideTimeoutMilliseconds = 240000;

		public const int DefaultServerSideCallOverheadMilliseconds = 3000;

		public const int DefaultServerSideTimeoutMilliseconds = 237000;

		public static readonly TimeSpan DefaultServerSideTimeout = TimeSpan.FromMilliseconds(237000.0);

		public static readonly TimeSpan MaximumTimeout = TimeSpan.FromDays(1.0);

		public static readonly int E14SP1ModerationReferralSupportVersion = new ServerVersion(14, 1, 176, 0).ToInt();
	}
}
