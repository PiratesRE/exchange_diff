using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.Story.V1.CommonMath
{
	internal struct Box2D : IEquatable<Box2D>
	{
		public Box2D(Vector2 min, Vector2 max)
		{
			this = default(Box2D);
			this.Min = min;
			this.Max = max;
			if (min.X > max.X || min.Y > max.Y)
			{
				this = Box2D.Null;
			}
		}

		public Box2D(double xmin, double ymin, double xmax, double ymax)
		{
			this = new Box2D((float)xmin, (float)ymin, (float)xmax, (float)ymax);
		}

		public Box2D(float xmin, float ymin, float xmax, float ymax)
		{
			this = new Box2D(new Vector2(xmin, ymin), new Vector2(xmax, ymax));
		}

		[DataMember]
		public Vector2 Min { get; private set; }

		[DataMember]
		public Vector2 Max { get; private set; }

		public float X
		{
			get
			{
				return this.Min.X;
			}
		}

		public float Left
		{
			get
			{
				return this.Min.X;
			}
		}

		public float Y
		{
			get
			{
				return this.Min.Y;
			}
		}

		public float Top
		{
			get
			{
				return this.Min.Y;
			}
		}

		public float Right
		{
			get
			{
				return this.Max.X;
			}
		}

		public float Bottom
		{
			get
			{
				return this.Max.Y;
			}
		}

		public float Width
		{
			get
			{
				float num = this.Max.X - this.Min.X;
				if (num <= 0f)
				{
					return 0f;
				}
				return num;
			}
		}

		public float Height
		{
			get
			{
				float num = this.Max.Y - this.Min.Y;
				if (num <= 0f)
				{
					return 0f;
				}
				return num;
			}
		}

		public float DiagonalSize
		{
			get
			{
				return this.Size.Length;
			}
		}

		public float Area
		{
			get
			{
				return this.Width * this.Height;
			}
		}

		public Vector2 Center
		{
			get
			{
				return this.Min + (this.Max - this.Min) / 2f;
			}
		}

		public bool IsNull
		{
			get
			{
				return this.Min.X > this.Max.X || this.Min.Y > this.Max.Y;
			}
		}

		public Vector2 Size
		{
			get
			{
				return this.Max - this.Min;
			}
		}

		public static Box2D FromSize(float x, float y, float width, float height)
		{
			return new Box2D(x, y, x + width, y + height);
		}

		public static Box2D FromSize(float width, float height)
		{
			return new Box2D(0f, 0f, width, height);
		}

		public static Box2D FromSize(Vector2 location, Vector2 size)
		{
			return new Box2D(location, location + size);
		}

		public static bool operator ==(Box2D box1, Box2D box2)
		{
			return box1.Min == box2.Min && box1.Max == box2.Max;
		}

		public static bool operator !=(Box2D box1, Box2D box2)
		{
			return !(box1 == box2);
		}

		public static Box2D operator +(Box2D box1, Box2D box2)
		{
			return new Box2D(Math.Min(box1.Min.X, box2.Min.X), Math.Min(box1.Min.Y, box2.Min.Y), Math.Max(box1.Max.X, box2.Max.X), Math.Max(box1.Max.Y, box2.Max.Y));
		}

		public static Box2D operator *(Box2D box, float scale)
		{
			return new Box2D(box.Min * scale, box.Max * scale);
		}

		public static Box2D operator *(Box2D box, Vector2 scale)
		{
			return new Box2D(box.Min * scale, box.Max * scale);
		}

		public Box2D Intersect(Box2D other)
		{
			return new Box2D(Math.Max(this.Min.X, other.Min.X), Math.Max(this.Min.Y, other.Min.Y), Math.Min(this.Max.X, other.Max.X), Math.Min(this.Max.Y, other.Max.Y));
		}

		public override int GetHashCode()
		{
			return this.Max.GetHashCode() ^ this.Min.GetHashCode();
		}

		public bool Equals(Box2D other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			return obj is Box2D && this == (Box2D)obj;
		}

		public static readonly Box2D Null = new Box2D
		{
			Min = new Vector2(float.PositiveInfinity, float.PositiveInfinity),
			Max = new Vector2(float.NegativeInfinity, float.NegativeInfinity)
		};

		public static readonly Box2D Zero = default(Box2D);
	}
}
