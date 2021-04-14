using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	internal abstract class ImageProcessor<TPixel, TValue, TTile> where TPixel : struct, IPixel<TValue> where TValue : struct, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue> where TTile : Tile<TPixel, TValue, TTile>
	{
		protected ImageProcessor(TiledImage<TPixel, TValue, TTile> image)
		{
			this.tiledImage = image;
		}

		public bool AllAroundScan { get; set; }

		public List<RegionInfo<TPixel, TValue, TTile>> Process(int minimumGroupSize)
		{
			List<RegionInfo<TPixel, TValue, TTile>> list = this.ProcessTiles(minimumGroupSize);
			foreach (RegionInfo<TPixel, TValue, TTile> regionInfo in list)
			{
				regionInfo.Lock();
			}
			return list;
		}

		protected abstract RegionInfo<TPixel, TValue, TTile> CreateRegionInfo(TiledImage<TPixel, TValue, TTile> parent);

		protected abstract bool Group(RegionInfo<TPixel, TValue, TTile> context, TTile originatingTile, TTile currentTile);

		private List<RegionInfo<TPixel, TValue, TTile>> ProcessTiles(int minimumGroupSize)
		{
			int num = 0;
			List<RegionInfo<TPixel, TValue, TTile>> list = new List<RegionInfo<TPixel, TValue, TTile>>();
			RegionInfo<TPixel, TValue, TTile> regionInfo = this.CreateRegionInfo(this.tiledImage);
			regionInfo.RegionId = num++;
			for (int i = 0; i < this.tiledImage.HeightInTiles; i++)
			{
				for (int j = 0; j < this.tiledImage.WidthInTiles; j++)
				{
					if (this.ProcessTile(regionInfo, new Point(j, i)))
					{
						if (regionInfo.TileCount > minimumGroupSize)
						{
							list.Add(regionInfo);
						}
						regionInfo = this.CreateRegionInfo(this.tiledImage);
						regionInfo.RegionId = num++;
					}
				}
			}
			return list;
		}

		private bool ProcessTile(RegionInfo<TPixel, TValue, TTile> context, Point location)
		{
			TTile tileOrDefault = this.tiledImage.GetTileOrDefault(location);
			if (tileOrDefault == null || tileOrDefault.IsProcessed)
			{
				return false;
			}
			tileOrDefault.IsProcessed = true;
			if (!this.Group(context, default(TTile), tileOrDefault))
			{
				return false;
			}
			Queue<Point> queue = new Queue<Point>();
			queue.Enqueue(location);
			while (queue.Count > 0)
			{
				location = queue.Dequeue();
				TTile tileOrDefault2 = this.tiledImage.GetTileOrDefault(location);
				context.AddTile(tileOrDefault2);
				foreach (Point pt in ImageProcessor<TPixel, TValue, TTile>.ScanDirections.Take(this.AllAroundScan ? 8 : 4))
				{
					Point point = pt + new Size(location);
					TTile tileOrDefault3 = this.tiledImage.GetTileOrDefault(point);
					if (tileOrDefault3 != null && !tileOrDefault3.IsProcessed)
					{
						tileOrDefault3.IsProcessed = true;
						if (this.Group(context, tileOrDefault2, tileOrDefault3))
						{
							queue.Enqueue(point);
						}
					}
				}
			}
			return true;
		}

		private const int DirectionsAll = 8;

		private const int DirectionsNormal = 4;

		private static readonly Point[] ScanDirections = new Point[]
		{
			new Point(1, 0),
			new Point(1, 1),
			new Point(0, 1),
			new Point(-1, 1),
			new Point(-1, 0),
			new Point(-1, -1),
			new Point(0, -1),
			new Point(1, -1)
		};

		private readonly TiledImage<TPixel, TValue, TTile> tiledImage;
	}
}
