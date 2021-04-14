using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.Data.ImageAnalysis
{
	public interface IImageAnalysis
	{
		KeyValuePair<byte[], ImageAnalysisResult> GenerateThumbnail(Stream imageStream, int minImageWidth, int minImageHeight, int maxThumbnailWidth, int maxThumbnailHeight, out int width, out int height);

		ISalientObjectAnalysis GetSalientObjectanalysis(byte[] imageData, int imageWidth, int imageHeight);
	}
}
