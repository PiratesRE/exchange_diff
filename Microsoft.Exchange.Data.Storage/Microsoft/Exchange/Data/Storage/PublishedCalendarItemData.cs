using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct PublishedCalendarItemData
	{
		public string Subject { get; internal set; }

		public string Location { get; internal set; }

		public string When { get; internal set; }

		public string BodyText { get; internal set; }
	}
}
