using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IContentIndexingSession
	{
		bool EnableWordBreak { get; set; }

		void OnBeforeItemChange(ItemChangeOperation operation, ICoreItem coreItem);
	}
}
