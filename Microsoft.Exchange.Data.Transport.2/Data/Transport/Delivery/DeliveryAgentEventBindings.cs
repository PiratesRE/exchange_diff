using System;

namespace Microsoft.Exchange.Data.Transport.Delivery
{
	internal class DeliveryAgentEventBindings
	{
		public const string OnOpenConnection = "OnOpenConnection";

		public const string OnDeliverMailItem = "OnDeliverMailItem";

		public const string OnCloseConnection = "OnCloseConnection";

		public static readonly string[] All = new string[]
		{
			"OnOpenConnection",
			"OnDeliverMailItem",
			"OnCloseConnection"
		};
	}
}
