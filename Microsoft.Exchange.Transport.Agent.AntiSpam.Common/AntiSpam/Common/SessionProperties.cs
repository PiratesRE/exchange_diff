using System;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	internal static class SessionProperties
	{
		public const string IsAuthenticated = "Microsoft.Exchange.IsAuthenticated";

		public const string IsOnAllowList = "Microsoft.Exchange.IsOnAllowList";

		public const string IsOnDenyList = "Microsoft.Exchange.IsOnDenyList";

		public const string IsOnBlockList = "Microsoft.Exchange.IsOnBlockList";

		public const string BypassedRecipients = "Microsoft.Exchange.BypassedRecipients";

		public const string IsOnSafeList = "Microsoft.Exchange.IsOnSafeList";

		public const string IsOnBlockListErrorMessage = "Microsoft.Exchange.IsOnBlockListErrorMessage";

		public const string IsOnBlockListProvider = "Microsoft..Exchange.IsOnBlockListProvider";

		public const string OnHeloOverriddenSenderAddress = "Microsoft.Exchange.Hygiene.TenantAttribution.OverriddenSenderAddress";

		public const string OnConnectOverriddenSenderAddress = "Microsoft.Forefront.Antispam.IPFilter.OverriddenSenderAddress";

		public const string IsShadowMessage = "Microsoft.Exchange.IsShadow";
	}
}
