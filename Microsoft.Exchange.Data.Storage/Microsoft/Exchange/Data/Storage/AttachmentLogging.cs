using System;
using Microsoft.Exchange.Data.ImageAnalysis;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AttachmentLogging
	{
		internal static void LogImageAnalysisResultCode(ImageAnalysisResult resultCode, ImageAnalysisLogData logData)
		{
			AttachmentProcessingLogger.Initialize();
			switch (resultCode)
			{
			case ImageAnalysisResult.ThumbnailSuccess:
				AttachmentProcessingLogger.LogEvent("StorageAttachmentImageAnalysis", "ThumbnailResult", "Success", "ThumbnailOperationTime", logData.operationTimeMs.ToString());
				AttachmentProcessingLogger.LogEvent("StorageAttachmentImageAnalysis", "ThumbnailSize", logData.thumbnailSize.ToString());
				AttachmentProcessingLogger.LogEvent("StorageAttachmentImageAnalysis", "ThumbnailWidth", logData.thumbnailWidth.ToString(), "ThumbnailHeight", logData.thumbnailHeight.ToString());
				return;
			case ImageAnalysisResult.SalientRegionSuccess:
				AttachmentProcessingLogger.LogEvent("StorageAttachmentImageAnalysis", "SalientRegionResult", "Success", "SalientRegionOperationTime", logData.operationTimeMs.ToString());
				return;
			case ImageAnalysisResult.UnknownFailure:
				AttachmentProcessingLogger.LogEvent("StorageAttachmentImageAnalysis", "Failure", logData.operationTimeMs.ToString());
				return;
			case ImageAnalysisResult.ImageTooSmallForAnalysis:
				AttachmentProcessingLogger.LogEvent("StorageAttachmentImageAnalysis", "ThumbnailResult", "ImageTooSmall", "ThumbnailOperationTime", logData.operationTimeMs.ToString());
				return;
			case ImageAnalysisResult.UnableToPerformSalientRegionAnalysis:
				AttachmentProcessingLogger.LogEvent("StorageAttachmentImageAnalysis", "SalientRegionResult", "Failure", "SalientRegionOperationTime", logData.operationTimeMs.ToString());
				return;
			case ImageAnalysisResult.ImageTooBigForAnalysis:
				AttachmentProcessingLogger.LogEvent("StorageAttachmentImageAnalysis", "ThumbnailResult", "ImageTooBigForAnalysis", "ImageSize", logData.thumbnailSize.ToString());
				return;
			default:
				return;
			}
		}

		internal static void LogImageAnalysisException(string callStack)
		{
			AttachmentProcessingLogger.Initialize();
			AttachmentProcessingLogger.LogEvent("StorageAttachmentImageAnalysis", "ThumbnailException", callStack);
		}

		private const string ImageAnalysisLogKeyword = "StorageAttachmentImageAnalysis";

		private const string ThumbnailResultKeyword = "ThumbnailResult";

		private const string ThumbnailResultTimeKeyword = "ThumbnailOperationTime";

		private const string SalientRegionResultKeyword = "SalientRegionResult";

		private const string SalientRegionResultTimeKeyword = "SalientRegionOperationTime";
	}
}
