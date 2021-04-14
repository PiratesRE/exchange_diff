using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum PropertyChangeMetadataProcessingFlags
	{
		None = 0,
		SeriesMasterDataPropagationOperation = 1,
		MarkAllPropagatedPropertiesAsException = 2,
		OverrideMetadata = 4
	}
}
