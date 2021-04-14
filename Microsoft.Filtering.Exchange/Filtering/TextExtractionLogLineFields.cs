using System;

namespace Microsoft.Filtering
{
	internal enum TextExtractionLogLineFields : short
	{
		Timestamp,
		ExMessageId,
		StreamId,
		StreamSize,
		ParentId,
		TeTypes,
		TeModuleUsed,
		TeResult,
		TeSkippedModules,
		TeFailedModules,
		TeDisabledModules,
		AdditionalInformation
	}
}
