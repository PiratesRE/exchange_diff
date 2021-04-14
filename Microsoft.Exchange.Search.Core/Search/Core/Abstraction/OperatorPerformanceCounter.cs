using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal enum OperatorPerformanceCounter
	{
		RetrieverNumberOfItemsProcessedIn0to50Milliseconds,
		RetrieverNumberOfItemsProcessedIn50to100Milliseconds,
		RetrieverNumberOfItemsProcessedIn100to2000Milliseconds,
		RetrieverNumberOfItemsProcessedInGreaterThan2000Milliseconds,
		RetrieverNumberOfItemsWithValidAnnotationToken,
		RetrieverNumberOfItemsWithoutAnnotationToken,
		RetrieverItemsWithRightsManagementAttempted,
		RetrieverItemsWithRightsManagementPartiallyProcessed,
		RetrieverItemsWithRightsManagementSkipped,
		TotalAnnotationTokenBytes,
		TotalAttachmentBytes,
		TotalAttachmentsRead,
		TotalBodyChars,
		IndexablePropertiesSize,
		TotalPoisonDocumentsProcessed,
		TotalTimeSpentConvertingToTextual,
		TotalTimeSpentDocParser,
		TotalTimeSpentLanguageDetection,
		TotalTimeSpentProcessingDocuments,
		TotalTimeSpentTokenDeserializer,
		TotalTimeSpentWorkBreaking
	}
}
