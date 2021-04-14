using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UnifiedContent.Exchange
{
	internal interface IMapiFilteringContext
	{
		bool NeedsClassificationScan(Attachment attachment);

		bool NeedsClassificationScan();
	}
}
