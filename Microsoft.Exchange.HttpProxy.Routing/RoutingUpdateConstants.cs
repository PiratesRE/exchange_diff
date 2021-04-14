using System;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public static class RoutingUpdateConstants
	{
		public const string RoutingEntryHeader = "X-RoutingEntry";

		public const string ExtraRoutingEntryHeader = "X-ExtraRoutingEntry";

		public const string LegacyRoutingEntryHeader = "X-LegacyRoutingEntry";

		public const string RoutingEntryUpdateHeader = "X-RoutingEntryUpdate";

		public const string ProtocolTypeAppSettingsKey = "RoutingUpdateModule.ProtocolType";
	}
}
