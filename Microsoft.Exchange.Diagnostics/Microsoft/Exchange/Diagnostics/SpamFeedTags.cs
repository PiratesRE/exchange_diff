using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct SpamFeedTags
	{
		public const int Routing = 0;

		public const int DeliveryAgent = 1;

		public const int KEStore = 2;

		public static Guid guid = new Guid("4A0B58AE-577F-415c-AD4F-4C577162EBDD");
	}
}
