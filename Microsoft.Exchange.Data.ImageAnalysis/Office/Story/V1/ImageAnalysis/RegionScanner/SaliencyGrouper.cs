using System;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	internal class SaliencyGrouper : ImageProcessor<ArgbPixel, byte, LabTile>
	{
		public SaliencyGrouper(TiledImage<ArgbPixel, byte, LabTile> image, float saliencyThreshold) : base(image)
		{
			this.SaliencyThreshold = saliencyThreshold;
			base.AllAroundScan = true;
		}

		public float SaliencyThreshold { get; set; }

		protected override bool Group(RegionInfo<ArgbPixel, byte, LabTile> context, LabTile originatingTile, LabTile currentTile)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (currentTile == null)
			{
				throw new ArgumentNullException("currentTile");
			}
			return currentTile.Saliency > this.SaliencyThreshold;
		}

		protected override RegionInfo<ArgbPixel, byte, LabTile> CreateRegionInfo(TiledImage<ArgbPixel, byte, LabTile> parent)
		{
			return new RegionInfo<ArgbPixel, byte, LabTile>(parent, null, (LabTile tile) => tile.Saliency);
		}
	}
}
