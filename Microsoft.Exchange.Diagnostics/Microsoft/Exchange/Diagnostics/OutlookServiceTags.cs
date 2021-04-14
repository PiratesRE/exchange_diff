using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct OutlookServiceTags
	{
		public const int FaultInjection = 0;

		public const int Framework = 1;

		public const int Features = 2;

		public const int StorageNotificationSubscription = 3;

		public static Guid guid = new Guid("33858cdd-8b16-4201-8490-dc180f17036e");
	}
}
