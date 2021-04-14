using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Office.Story.V1.CommonMath;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	[DataContract]
	[Serializable]
	internal class Tile<TPixel, TValue, TTile> where TPixel : struct, IPixel<TValue> where TValue : struct, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue> where TTile : Tile<TPixel, TValue, TTile>
	{
		internal Tile()
		{
		}

		internal Tile(TiledImage<TPixel, TValue, TTile> parent, TileCoordinate coordinate)
		{
			this.Parent = parent;
			this.Coordinate = coordinate;
		}

		[DataMember]
		public TiledImage<TPixel, TValue, TTile> Parent { get; internal set; }

		[DataMember]
		public TileCoordinate Coordinate { get; internal set; }

		[DataMember]
		public float Luminance { get; private set; }

		[DataMember]
		public float Intensity { get; private set; }

		[DataMember]
		public int RegisteredPixels { get; internal set; }

		[DataMember]
		public bool IsProcessed { get; set; }

		public int TotalPixels
		{
			get
			{
				return this.TileLinearSize * this.TileLinearSize;
			}
		}

		public int TileLinearSize
		{
			get
			{
				return this.Parent.TileLinearSize;
			}
		}

		public Box2D Location
		{
			get
			{
				return this.Parent.GetTileOutline(this.Coordinate);
			}
		}

		public bool IsBoundary
		{
			get
			{
				return this.RegisteredPixels < this.TotalPixels;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} Luminance, {1}processed @ {2}", new object[]
			{
				this.Luminance,
				this.IsProcessed ? string.Empty : "not ",
				this.Location
			});
		}

		internal void Reset()
		{
			this.IsProcessed = false;
		}

		internal virtual void RegisterPixel(TPixel pixel)
		{
			this.Intensity += pixel.Intensity;
			this.RegisteredPixels++;
		}

		internal virtual void Lock()
		{
			if (this.RegisteredPixels > 0)
			{
				this.Intensity /= (float)this.RegisteredPixels;
				this.Luminance = MathHelper.ChannelLuminance(this.Intensity);
			}
		}
	}
}
