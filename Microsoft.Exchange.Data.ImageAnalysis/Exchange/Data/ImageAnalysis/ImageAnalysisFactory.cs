using System;

namespace Microsoft.Exchange.Data.ImageAnalysis
{
	public static class ImageAnalysisFactory
	{
		public static IImageAnalysis GetImageAnalysisImpl()
		{
			return new ImageAnalysis();
		}
	}
}
