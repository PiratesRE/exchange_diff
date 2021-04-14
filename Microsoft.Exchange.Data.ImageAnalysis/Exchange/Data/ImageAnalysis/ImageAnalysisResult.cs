using System;

namespace Microsoft.Exchange.Data.ImageAnalysis
{
	public enum ImageAnalysisResult
	{
		ThumbnailSuccess,
		SalientRegionSuccess,
		UnknownFailure,
		ImageTooSmallForAnalysis,
		UnableToPerformSalientRegionAnalysis,
		ImageTooBigForAnalysis
	}
}
