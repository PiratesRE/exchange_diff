using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	[DataContract]
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	internal struct ArgbPixel : IPixel<byte>, IEquatable<ArgbPixel>
	{
		public ArgbPixel(Color color)
		{
			this.ARGB = 0U;
			this.A = color.A;
			this.R = color.R;
			this.G = color.G;
			this.B = color.B;
		}

		[IgnoreDataMember]
		public float Luminance
		{
			get
			{
				return 0.2126f * this.Rl + 0.7152f * this.Gl + 0.0722f * this.Bl;
			}
		}

		[IgnoreDataMember]
		public float Intensity
		{
			get
			{
				return 0.299f * this.Rs + 0.587f * this.Gs + 0.114f * this.Bs;
			}
		}

		public int Bands
		{
			get
			{
				return 4;
			}
		}

		internal float Rs
		{
			get
			{
				return (float)this.R * 0.003921569f;
			}
		}

		internal float Gs
		{
			get
			{
				return (float)this.G * 0.003921569f;
			}
		}

		internal float Bs
		{
			get
			{
				return (float)this.G * 0.003921569f;
			}
		}

		internal float Rl
		{
			get
			{
				return ArgbPixel.ChannelLuminance[(int)this.R];
			}
		}

		internal float Gl
		{
			get
			{
				return ArgbPixel.ChannelLuminance[(int)this.G];
			}
		}

		internal float Bl
		{
			get
			{
				return ArgbPixel.ChannelLuminance[(int)this.B];
			}
		}

		private static float[] ChannelLuminance
		{
			get
			{
				if (ArgbPixel.channelLuminanceCache == null)
				{
					float[] array = new float[256];
					for (int i = 0; i <= 255; i++)
					{
						float num = (float)i * 0.003921569f;
						array[i] = ((num <= 0.03928f) ? (num / 12.92f) : ((float)Math.Pow((double)((num + 0.055f) / 1.055f), 2.4000000953674316)));
					}
					ArgbPixel.channelLuminanceCache = array;
				}
				return ArgbPixel.channelLuminanceCache;
			}
		}

		public byte this[int band]
		{
			get
			{
				switch (band)
				{
				case 0:
					return this.B;
				case 1:
					return this.G;
				case 2:
					return this.R;
				case 3:
					return this.A;
				default:
					throw new IndexOutOfRangeException("band");
				}
			}
		}

		public static bool operator ==(ArgbPixel one, ArgbPixel another)
		{
			return one.Equals(another);
		}

		public static bool operator !=(ArgbPixel one, ArgbPixel another)
		{
			return !one.Equals(another);
		}

		public int CompareByIntensity(IPixel<byte> another)
		{
			if (another == null)
			{
				throw new ArgumentNullException("another");
			}
			return this.Intensity.CompareTo(another.Intensity);
		}

		public override bool Equals(object other)
		{
			return other is ArgbPixel && this.Equals((ArgbPixel)other);
		}

		public override int GetHashCode()
		{
			return this.ARGB.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "A:{0:X2} R:{1:X2} G:{2:X2} B:{3:X2}", new object[]
			{
				this.A,
				this.R,
				this.G,
				this.B
			});
		}

		public bool Equals(ArgbPixel other)
		{
			return this.ARGB.Equals(other.ARGB);
		}

		internal const int MaxChannelValue = 255;

		internal const float NormalizationCoefficient = 0.003921569f;

		[DataMember]
		[FieldOffset(0)]
		public uint ARGB;

		[IgnoreDataMember]
		[NonSerialized]
		[FieldOffset(3)]
		public byte A;

		[IgnoreDataMember]
		[NonSerialized]
		[FieldOffset(2)]
		public byte R;

		[IgnoreDataMember]
		[NonSerialized]
		[FieldOffset(1)]
		public byte G;

		[IgnoreDataMember]
		[NonSerialized]
		[FieldOffset(0)]
		public byte B;

		private static float[] channelLuminanceCache;
	}
}
