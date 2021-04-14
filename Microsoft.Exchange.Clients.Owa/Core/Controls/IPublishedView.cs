using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal interface IPublishedView
	{
		string DisplayName { get; }

		string PublisherDisplayName { get; }

		string ICalUrl { get; }

		SanitizedHtmlString PublishTimeRange { get; }
	}
}
