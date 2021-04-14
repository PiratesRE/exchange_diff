using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Office.Story.V1.CommonMath;
using Microsoft.Office.Story.V1.ImageAnalysis.SalientObjectDetection;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	[DataContract]
	[Serializable]
	internal class RegionInfo<TPixel, TValue, TTile> where TPixel : struct, IPixel<TValue> where TValue : struct, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue> where TTile : Tile<TPixel, TValue, TTile>
	{
		public RegionInfo(TiledImage<TPixel, TValue, TTile> parent, Func<TTile, Box2D> getOutline, Func<TTile, float> getDensity)
		{
			this.parent = parent;
			this.tileCoordinates = new List<TileCoordinate>();
			this.Cluster = new WeightedCluster<TTile>(getOutline ?? new Func<TTile, Box2D>(this.GetOutline), getDensity ?? new Func<TTile, float>(this.GetDensity));
			this.LuminanceStats = new CountStatistics();
			this.IntensityStats = new CountStatistics();
			this.CutOffRatio = RegionInfo<TPixel, TValue, TTile>.DefaultCutOffRatio;
		}

		public int TileLinearSize
		{
			get
			{
				return this.parent.TileLinearSize;
			}
		}

		public Vector2 UnscaledTileSize
		{
			get
			{
				return this.parent.UnscaledTileSize;
			}
		}

		[DataMember]
		public Box2D CutOffRatio { get; set; }

		[DataMember]
		public CountStatistics LuminanceStats { get; protected set; }

		[DataMember]
		public CountStatistics IntensityStats { get; protected set; }

		[DataMember]
		public int RegionId { get; internal set; }

		public IEnumerable<TTile> Tiles
		{
			get
			{
				return from tileCoordinate in this.tileCoordinates
				select this.parent[tileCoordinate];
			}
		}

		public int TileCount
		{
			get
			{
				if (this.tileCoordinates != null)
				{
					return this.tileCoordinates.Count;
				}
				return 0;
			}
		}

		[DataMember]
		public WeightedCluster<TTile> Cluster { get; private set; }

		[DataMember]
		public Box2D BestOutline { get; private set; }

		[IgnoreDataMember]
		internal int JumpDepth { get; set; }

		public void AddTile(TTile tile)
		{
			this.tileCoordinates.Add(tile.Coordinate);
			this.LuminanceStats.Add((double)tile.Luminance);
			this.IntensityStats.Add((double)tile.Intensity);
			this.Cluster.Add(tile);
		}

		public TiledImage<TPixel, TValue, TTile> CreateRegionImage()
		{
			Vector2 min = this.Cluster.Outline.Min;
			int num = (int)(this.Cluster.Outline.Width / this.UnscaledTileSize.X);
			int num2 = (int)(this.Cluster.Outline.Height / this.UnscaledTileSize.Y);
			TTile[] array = new TTile[num * num2];
			foreach (TTile ttile in this.Tiles)
			{
				Vector2 vector = (ttile.Location.Min - min) / this.UnscaledTileSize;
				array[(int)vector.X + (int)vector.Y * num] = ttile;
			}
			return new TiledImage<TPixel, TValue, TTile>(array, num, num2, this.TileLinearSize, 1f);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} tiles @{1} ({2})", new object[]
			{
				this.TileCount,
				this.Cluster.Outline,
				this.Cluster.Center
			});
		}

		internal void Lock()
		{
			this.Cluster.Lock();
			this.BestOutline = this.ComputeBestOutline();
			this.SortTiles();
		}

		protected virtual float GetDensity(TTile tile)
		{
			if (tile == null)
			{
				throw new ArgumentNullException("tile");
			}
			return tile.Intensity;
		}

		protected virtual Box2D GetOutline(TTile tile)
		{
			if (tile == null)
			{
				throw new ArgumentNullException("tile");
			}
			return tile.Location;
		}

		protected virtual void SortTiles()
		{
		}

		private Box2D ComputeBestOutline()
		{
			Box2D cutOffCount = this.CutOffRatio * (float)this.TileCount;
			if (cutOffCount.DiagonalSize < 5f)
			{
				return this.Cluster.Outline;
			}
			Histogram<TTile> histogram = new Histogram<TTile>((TTile item) => (double)item.Location.Center.Y, (double)this.parent.Boundary.Y, (double)this.parent.Boundary.Bottom, (double)this.parent.UnscaledTileSize.Y);
			Histogram<TTile> histogram2 = new Histogram<TTile>((TTile item) => (double)item.Location.Center.X, (double)this.parent.Boundary.X, (double)this.parent.Boundary.Right, (double)this.parent.UnscaledTileSize.X);
			foreach (TTile item2 in this.Tiles)
			{
				histogram.Add(item2);
				histogram2.Add(item2);
			}
			double verticalCutThreshold = (from bin in histogram.Bins
			select bin.Items.Count).Average() / 2.0;
			double horizontalCutThreshold = (from bin in histogram2.Bins
			select bin.Items.Count).Average() / 2.0;
			double ymin = histogram.ScanForRange((double)this.Cluster.Outline.Y, (double)this.Cluster.Outline.Bottom, (Bin<TTile> bin) => (double)bin.Items.Count, (double a, double v) => a + v, (double a, double v) => a >= (double)cutOffCount.Y || v > verticalCutThreshold);
			double ymax = histogram.ScanForRange((double)this.Cluster.Outline.Bottom, (double)this.Cluster.Outline.Y, (Bin<TTile> bin) => (double)bin.Items.Count, (double a, double v) => a + v, (double a, double v) => a >= (double)cutOffCount.Bottom || v > verticalCutThreshold);
			double xmin = histogram2.ScanForRange((double)this.Cluster.Outline.X, (double)this.Cluster.Outline.Right, (Bin<TTile> bin) => (double)bin.Items.Count, (double a, double v) => a + v, (double a, double v) => a >= (double)cutOffCount.X || v > horizontalCutThreshold);
			double xmax = histogram2.ScanForRange((double)this.Cluster.Outline.Right, (double)this.Cluster.Outline.X, (Bin<TTile> bin) => (double)bin.Items.Count, (double a, double v) => a + v, (double a, double v) => a >= (double)cutOffCount.Right || v > horizontalCutThreshold);
			Box2D result = new Box2D(xmin, ymin, xmax, ymax);
			return result;
		}

		private static readonly Box2D DefaultCutOffRatio = new Box2D(0.05f, 0f, 0.05f, 0.05f);

		[DataMember]
		private readonly List<TileCoordinate> tileCoordinates;

		[DataMember]
		private TiledImage<TPixel, TValue, TTile> parent;
	}
}
