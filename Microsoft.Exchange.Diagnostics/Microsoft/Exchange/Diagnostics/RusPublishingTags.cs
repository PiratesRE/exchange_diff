using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct RusPublishingTags
	{
		public const int Common = 0;

		public const int PublisherWeb = 1;

		public const int EngineUpdateDownloader = 2;

		public const int EngineUpdatePublisher = 3;

		public const int SignaturePollingJob = 4;

		public const int EnginePackagingStep = 5;

		public const int EngineTestingStep = 6;

		public const int EngineCodeSignStep = 7;

		public const int EngineCleanUpStep = 8;

		public static Guid guid = new Guid("534d6f5a-8ca8-4c44-abd7-481335889364");
	}
}
