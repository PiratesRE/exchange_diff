using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Office.Story.V1.CommonMath;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	[DataContract]
	[Serializable]
	internal class TiledImage<TPixel, TValue, TTile> : IEnumerable<TTile>, IEnumerable where TPixel : struct, IPixel<TValue> where TValue : struct, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue> where TTile : Tile<TPixel, TValue, TTile>
	{
		public TiledImage(ImageBase<TPixel, TValue> image, int tileSize, float scale, bool boundaryTilesAllowed, Func<TiledImage<TPixel, TValue, TTile>, TileCoordinate, TTile> tileCreator)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			this.Scale = scale;
			this.BoundaryTilesAllowed = boundaryTilesAllowed;
			this.Boundary = Box2D.FromSize(0f, 0f, (float)image.Width * this.Scale, (float)image.Height * this.Scale);
			this.TileLinearSize = tileSize;
			this.TileSize = new Vector2((float)tileSize);
			this.UnscaledTileSize = this.TileSize * this.Scale;
			this.CreateTiles(image, tileCreator);
		}

		public TiledImage(TTile[] tiles, int widthInTiles, int heightInTiles, int tileLinearSize, float scale)
		{
			if (tiles == null)
			{
				throw new ArgumentNullException("tiles");
			}
			if (widthInTiles < 1 || heightInTiles < 1)
			{
				throw new ArgumentException("Width and height should be positive number above zero.");
			}
			if (tiles.Length < widthInTiles * heightInTiles)
			{
				throw new ArgumentException("Array of tiles must have at least widthInTiles * heightInTiles tiles.");
			}
			this.Scale = scale;
			this.TileLinearSize = tileLinearSize;
			this.WidthInTiles = widthInTiles;
			this.HeightInTiles = heightInTiles;
			this.Boundary = Box2D.FromSize(0f, 0f, (float)(this.TileLinearSize * this.WidthInTiles) * this.Scale, (float)(this.TileLinearSize * heightInTiles) * this.Scale);
			this.TileSize = new Vector2((float)this.TileLinearSize);
			this.UnscaledTileSize = this.TileSize * this.Scale;
			this.tiles = tiles;
			for (int i = 0; i < this.tiles.Length; i++)
			{
				TTile ttile = tiles[i];
				if (ttile != null)
				{
					if (ttile.Parent == null)
					{
						ttile.Parent = this;
						ttile.Coordinate = this.CreateCoordinate(i);
					}
					else if (ttile.TileLinearSize != this.TileLinearSize)
					{
						throw new InvalidOperationException("Parented tile size does not match this image.");
					}
				}
			}
		}

		[DataMember]
		public bool BoundaryTilesAllowed { get; private set; }

		[DataMember]
		public float Scale { get; private set; }

		[DataMember]
		public int WidthInTiles { get; private set; }

		[DataMember]
		public int HeightInTiles { get; private set; }

		[DataMember]
		public Vector2 TileSize { get; private set; }

		[DataMember]
		public int TileLinearSize { get; private set; }

		[DataMember]
		public Box2D Boundary { get; private set; }

		public int AreaInTiles
		{
			get
			{
				return this.WidthInTiles * this.HeightInTiles;
			}
		}

		[DataMember]
		internal Vector2 UnscaledTileSize { get; private set; }

		public TTile this[int tile]
		{
			get
			{
				return this.tiles[tile];
			}
			private set
			{
				this.tiles[tile] = value;
			}
		}

		public TTile this[int x, int y]
		{
			get
			{
				return this[y * this.WidthInTiles + x];
			}
		}

		public TTile this[TileCoordinate coordinate]
		{
			get
			{
				return this[(int)coordinate.Index];
			}
		}

		public TileCoordinate CreateCoordinate(int n)
		{
			return new TileCoordinate(n);
		}

		public TTile GetTileOrDefault(Point location)
		{
			TTile result = default(TTile);
			int x = location.X;
			int y = location.Y;
			if (x >= 0 && x < this.WidthInTiles && y >= 0 && y < this.HeightInTiles)
			{
				result = this[x, y];
			}
			return result;
		}

		public CountStatistics CreateStats(Func<TTile, double> extractor)
		{
			return new CountStatistics(from tile in this.tiles
			select extractor(tile));
		}

		public Box2D GetTileOutline(TileCoordinate coordinate)
		{
			int num = (int)coordinate.Index % this.WidthInTiles;
			int num2 = (int)coordinate.Index / this.WidthInTiles;
			return Box2D.FromSize(this.Boundary.Min + this.UnscaledTileSize * new Vector2((float)num, (float)num2), this.UnscaledTileSize);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} by {1} tiles of size {2} and type {3}.", new object[]
			{
				this.WidthInTiles,
				this.HeightInTiles,
				this.TileLinearSize,
				typeof(TTile).Name
			});
		}

		public IEnumerator<TTile> GetEnumerator()
		{
			return ((IEnumerable<TTile>)this.tiles).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.tiles.GetEnumerator();
		}

		private static TTile CreateTile(TiledImage<TPixel, TValue, TTile> parent, TileCoordinate coordinate)
		{
			return (TTile)((object)new Tile<TPixel, TValue, TTile>(parent, coordinate));
		}

		private void CreateTiles(ImageBase<TPixel, TValue> image, Func<TiledImage<TPixel, TValue, TTile>, TileCoordinate, TTile> tileCreator)
		{
			tileCreator = (tileCreator ?? new Func<TiledImage<TPixel, TValue, TTile>, TileCoordinate, TTile>(TiledImage<TPixel, TValue, TTile>.CreateTile));
			this.WidthInTiles = image.Width / this.TileLinearSize;
			this.HeightInTiles = image.Height / this.TileLinearSize;
			if (this.BoundaryTilesAllowed)
			{
				if (image.Width % this.TileLinearSize > 0)
				{
					this.WidthInTiles++;
				}
				if (image.Height % this.TileLinearSize > 0)
				{
					this.HeightInTiles++;
				}
			}
			this.tiles = new TTile[this.WidthInTiles * this.HeightInTiles];
			for (int i = 0; i < this.tiles.Length; i++)
			{
				this[i] = tileCreator(this, this.CreateCoordinate(i));
			}
			int num = this.BoundaryTilesAllowed ? image.Width : (this.WidthInTiles * this.TileLinearSize);
			int num2 = this.BoundaryTilesAllowed ? image.Height : (this.HeightInTiles * this.TileLinearSize);
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k < num; k++)
				{
					TTile ttile = this[k / this.TileLinearSize, j / this.TileLinearSize];
					ttile.RegisterPixel(image[k, j]);
				}
			}
			foreach (TTile ttile2 in this.tiles)
			{
				ttile2.Lock();
			}
		}

		[DataMember]
		private TTile[] tiles;
	}
}
