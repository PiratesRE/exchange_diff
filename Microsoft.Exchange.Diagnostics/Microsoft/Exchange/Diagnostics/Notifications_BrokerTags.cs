using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct Notifications_BrokerTags
	{
		public const int Client = 0;

		public const int Service = 1;

		public const int MailboxChange = 2;

		public const int Subscriptions = 3;

		public const int RemoteConduit = 4;

		public const int Generator = 5;

		public static Guid guid = new Guid("f16b990e-bd72-4a46-b231-b1ed417eaa17");
	}
}
