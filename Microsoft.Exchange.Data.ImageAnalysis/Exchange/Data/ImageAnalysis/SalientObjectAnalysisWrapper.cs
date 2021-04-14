using System;
using System.Collections.Generic;
using Microsoft.Office.Story.V1.ImageAnalysis;
using Microsoft.Office.Story.V1.ImageAnalysis.SalientObjectDetection;

namespace Microsoft.Exchange.Data.ImageAnalysis
{
	internal class SalientObjectAnalysisWrapper : ISalientObjectAnalysis
	{
		internal SalientObjectAnalysisWrapper(byte[] image, int imageWidth, int imageHeight)
		{
			this.image = image;
			this.imageWidth = imageWidth;
			this.imageHeight = imageHeight;
			this.imageSource = null;
			this.info = null;
			this.salientAnalysis = null;
			this.salientRegions = null;
		}

		public KeyValuePair<List<RegionRect>, ImageAnalysisResult> GetSalientRectsAsList()
		{
			ImageAnalysisResult imageAnalysisResult = this.EnsureSalientObjectAnalysisWrapper();
			if (this.salientRegions == null && imageAnalysisResult == ImageAnalysisResult.SalientRegionSuccess)
			{
				this.salientRegions = new List<RegionRect>();
				foreach (SalientObject salientObject in this.salientAnalysis.SalientObjects)
				{
					RegionRect item = new RegionRect((int)salientObject.Region.BestOutline.Left, (int)salientObject.Region.BestOutline.Top, (int)salientObject.Region.BestOutline.Right, (int)salientObject.Region.BestOutline.Bottom);
					this.salientRegions.Add(item);
				}
				imageAnalysisResult = ImageAnalysisResult.SalientRegionSuccess;
			}
			else
			{
				imageAnalysisResult = ImageAnalysisResult.UnableToPerformSalientRegionAnalysis;
			}
			return new KeyValuePair<List<RegionRect>, ImageAnalysisResult>(this.salientRegions, imageAnalysisResult);
		}

		public KeyValuePair<byte[], ImageAnalysisResult> GetSalientRectsAsByteArray()
		{
			ImageAnalysisResult imageAnalysisResult = this.EnsureSalientObjectAnalysisWrapper();
			if (this.salientRegionsAsByte == null && imageAnalysisResult == ImageAnalysisResult.SalientRegionSuccess)
			{
				this.salientRegionsAsByte = new byte[this.salientAnalysis.SalientObjects.Count * 4];
				int num = 0;
				foreach (SalientObject salientObject in this.salientAnalysis.SalientObjects)
				{
					this.salientRegionsAsByte[num] = (byte)((int)salientObject.Region.BestOutline.Top * 255 / this.imageHeight);
					this.salientRegionsAsByte[num + 1] = (byte)((int)salientObject.Region.BestOutline.Left * 255 / this.imageWidth);
					this.salientRegionsAsByte[num + 2] = (byte)((int)salientObject.Region.BestOutline.Bottom * 255 / this.imageHeight);
					this.salientRegionsAsByte[num + 3] = (byte)((int)salientObject.Region.BestOutline.Right * 255 / this.imageWidth);
					num += 4;
				}
				imageAnalysisResult = ImageAnalysisResult.SalientRegionSuccess;
			}
			else
			{
				imageAnalysisResult = ImageAnalysisResult.UnableToPerformSalientRegionAnalysis;
			}
			return new KeyValuePair<byte[], ImageAnalysisResult>(this.salientRegionsAsByte, imageAnalysisResult);
		}

		private ImageAnalysisResult EnsureSalientObjectAnalysisWrapper()
		{
			try
			{
				if (this.imageSource == null)
				{
					this.imageSource = new ImageSource(this.image);
				}
				if (this.info == null)
				{
					this.info = new ImageInfo(this.imageSource);
				}
				if (this.salientAnalysis == null)
				{
					this.salientAnalysis = this.info.PerformSalientObjectAnalysis();
				}
			}
			catch (Exception)
			{
				return ImageAnalysisResult.UnableToPerformSalientRegionAnalysis;
			}
			return ImageAnalysisResult.SalientRegionSuccess;
		}

		private readonly int imageWidth;

		private readonly int imageHeight;

		private ImageSource imageSource;

		private ImageInfo info;

		private SalientObjectAnalysis salientAnalysis;

		private List<RegionRect> salientRegions;

		private byte[] salientRegionsAsByte;

		private byte[] image;
	}
}
