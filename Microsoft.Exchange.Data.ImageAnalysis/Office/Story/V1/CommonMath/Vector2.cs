using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Microsoft.Office.Story.V1.CommonMath
{
	internal struct Vector2 : IEquatable<Vector2>
	{
		public Vector2(float x, float y)
		{
			this = default(Vector2);
			this.X = x;
			this.Y = y;
		}

		public Vector2(float value)
		{
			this = default(Vector2);
			this.X = value;
			this.Y = value;
		}

		public static Vector2 Zero
		{
			get
			{
				return Vector2.zero;
			}
		}

		[DataMember]
		public float X { get; private set; }

		[DataMember]
		public float Y { get; private set; }

		[IgnoreDataMember]
		public float Length
		{
			get
			{
				return (float)Math.Sqrt((double)this.LengthSquared);
			}
		}

		[IgnoreDataMember]
		public float LengthSquared
		{
			get
			{
				return this.X * this.X + this.Y * this.Y;
			}
		}

		public static bool operator ==(Vector2 value1, Vector2 value2)
		{
			return value1.Equals(value2);
		}

		public static bool operator !=(Vector2 value1, Vector2 value2)
		{
			return !value1.Equals(value2);
		}

		public static Vector2 operator +(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X + value2.X, value1.Y + value2.Y);
		}

		public static Vector2 operator -(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X - value2.X, value1.Y - value2.Y);
		}

		public static Vector2 operator *(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X * value2.X, value1.Y * value2.Y);
		}

		public static Vector2 operator *(Vector2 value, float scaleFactor)
		{
			return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
		}

		public static Vector2 operator /(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X / value2.X, value1.Y / value2.Y);
		}

		public static Vector2 operator /(Vector2 value1, float divider)
		{
			float num = 1f / divider;
			return new Vector2(value1.X * num, value1.Y * num);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{{X:{0} Y:{1}}}", new object[]
			{
				this.X,
				this.Y
			});
		}

		public bool Equals(Vector2 other)
		{
			return this.X.ExactEquals(other.X) && this.Y.ExactEquals(other.Y);
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			if (obj is Vector2)
			{
				result = this.Equals((Vector2)obj);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode();
		}

		public static readonly int SizeOf = Marshal.SizeOf(typeof(Vector2));

		private static readonly Vector2 zero = default(Vector2);
	}
}
