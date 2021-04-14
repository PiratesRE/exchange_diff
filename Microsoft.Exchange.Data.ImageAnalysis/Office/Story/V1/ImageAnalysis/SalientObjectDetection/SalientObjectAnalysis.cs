using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Office.Story.V1.GraphicsInterop;
using Microsoft.Office.Story.V1.GraphicsInterop.Wic;
using Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner;

namespace Microsoft.Office.Story.V1.ImageAnalysis.SalientObjectDetection
{
	[DataContract]
	[Serializable]
	internal class SalientObjectAnalysis : AnalysisBase
	{
		public SalientObjectAnalysis(ImageSource imageSource)
		{
			if (imageSource == null)
			{
				throw new ArgumentNullException("imageSource");
			}
			this.imageSource = imageSource;
			base.PerformAnalysis();
		}

		private SalientObjectAnalysis()
		{
		}

		[DataMember]
		public LabTiledImage TiledImage { get; private set; }

		[DataMember]
		public List<SalientObject> SalientObjects { get; private set; }

		[DataMember]
		public float SaliencyThreshold { get; set; }

		[DataMember]
		public float TotalSalience { get; set; }

		protected override bool CanAnalyze()
		{
			return this.imageSource.Height > 40f && this.imageSource.Width > 40f;
		}

		protected override void AnalysisImplementation()
		{
			this.Detect(this.imageSource);
		}

		protected override void Lock()
		{
			this.imageSource = null;
		}

		protected override void CreateDefaultResults()
		{
			this.SalientObjects = new List<SalientObject>();
		}

		private static List<SalientObject> GroupSalientTiles(LabTiledImage saliencyMap, ref float threshold, int minimumGroupSize = 8)
		{
			if (threshold <= 0f || threshold > 1f)
			{
				CountStatistics countStatistics = saliencyMap.CreateStats((LabTile tile) => (double)tile.Saliency);
				threshold = (1f - (float)countStatistics.Average) * 0.6f;
			}
			SaliencyGrouper saliencyGrouper = new SaliencyGrouper(saliencyMap, threshold);
			List<RegionInfo<ArgbPixel, byte, LabTile>> list = saliencyGrouper.Process(minimumGroupSize);
			list.Sort((RegionInfo<ArgbPixel, byte, LabTile> first, RegionInfo<ArgbPixel, byte, LabTile> second) => second.TileCount.CompareTo(first.TileCount));
			return (from region in list
			select new SalientObject
			{
				Region = region
			}).ToList<SalientObject>();
		}

		private void Detect(ImageSource imageSource)
		{
			IWICImagingFactory iwicimagingFactory = WicUtility.CreateFactory();
			IWICBitmapFrameDecode iwicbitmapFrameDecode = iwicimagingFactory.Load(imageSource.ImageStream);
			this.DetectSalientRegions(iwicimagingFactory, iwicbitmapFrameDecode);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicbitmapFrameDecode);
			GraphicsInteropNativeMethods.SafeReleaseComObject(iwicimagingFactory);
		}

		private void DetectSalientRegions(IWICImagingFactory factory, IWICBitmapSource bitmapSource)
		{
			int num;
			int num2;
			bitmapSource.GetSize(out num, out num2);
			int patchSize = Math.Max((num + num2) / 2 / 50, 4);
			SaliencyMap saliencyMap = new SaliencyMap(new ArgbImage(factory, bitmapSource), patchSize, 1f);
			this.TiledImage = saliencyMap.TiledImage;
			float saliencyThreshold = this.SaliencyThreshold;
			this.SalientObjects = SalientObjectAnalysis.GroupSalientTiles(this.TiledImage, ref saliencyThreshold, 8);
			this.SaliencyThreshold = saliencyThreshold;
			this.TotalSalience = 0f;
			float num3 = -1f;
			foreach (float num4 in from region in this.SalientObjects
			select region.Region.Cluster.Mass)
			{
				float num5 = num4;
				if (num3 < num5)
				{
					num3 = num5;
				}
				this.TotalSalience += num5;
			}
			foreach (SalientObject salientObject in this.SalientObjects)
			{
				salientObject.SaliencePortion = salientObject.Region.Cluster.Mass / this.TotalSalience;
				salientObject.IsPrimary = (salientObject.Region.Cluster.Mass >= num3);
			}
		}

		private const float MinimumImageSize = 40f;

		private const int TargetPatchCount = 50;

		[NonSerialized]
		private ImageSource imageSource;
	}
}
