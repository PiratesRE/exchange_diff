using System;

namespace Microsoft.Exchange.Data.Transport.Routing
{
	internal class RoutingEventBindings
	{
		public const string EventOnSubmittedMessage = "OnSubmittedMessage";

		public const string EventOnResolvedMessage = "OnResolvedMessage";

		public const string EventOnRoutedMessage = "OnRoutedMessage";

		public const string EventOnCategorizedMessage = "OnCategorizedMessage";

		public static readonly string[] All = new string[]
		{
			"OnSubmittedMessage",
			"OnResolvedMessage",
			"OnRoutedMessage",
			"OnCategorizedMessage"
		};
	}
}
