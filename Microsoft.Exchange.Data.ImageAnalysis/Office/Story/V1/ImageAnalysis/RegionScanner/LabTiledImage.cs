using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	[DataContract]
	[Serializable]
	internal class LabTiledImage : TiledImage<ArgbPixel, byte, LabTile>
	{
		public LabTiledImage(ImageBase<ArgbPixel, byte> image, int tileSize, float scale) : base(image, tileSize, scale, false, new Func<TiledImage<ArgbPixel, byte, LabTile>, TileCoordinate, LabTile>(LabTiledImage.CreateTile))
		{
		}

		private static LabTile CreateTile(TiledImage<ArgbPixel, byte, LabTile> parent, TileCoordinate location)
		{
			return new LabTile(parent, location);
		}
	}
}
