using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	[DataContract]
	[Serializable]
	internal class LabTile : Tile<ArgbPixel, byte, LabTile>
	{
		public LabTile()
		{
		}

		public LabTile(TiledImage<ArgbPixel, byte, LabTile> parent, TileCoordinate location) : base(parent, location)
		{
		}

		[DataMember]
		public float L { get; private set; }

		[DataMember]
		public float A { get; private set; }

		[DataMember]
		public float B { get; private set; }

		[DataMember]
		public float Saliency { get; set; }

		public float ComputeLabDistance(LabTile other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			float num = (this.L - other.L) * (this.L - other.L) + (this.A - other.A) * (this.A - other.A) + (this.B - other.B) * (this.B - other.B);
			return (float)Math.Sqrt((double)num);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}\t{1}\t{2} @ {3}", new object[]
			{
				this.L,
				this.A,
				this.B,
				base.Location
			});
		}

		internal override void RegisterPixel(ArgbPixel pixel)
		{
			this.L += (float)pixel.R;
			this.A += (float)pixel.G;
			this.B += (float)pixel.B;
			base.RegisterPixel(pixel);
		}

		internal override void Lock()
		{
			float num = this.L / (float)base.RegisteredPixels * 0.003921569f;
			float num2 = this.A / (float)base.RegisteredPixels * 0.003921569f;
			float num3 = this.B / (float)base.RegisteredPixels * 0.003921569f;
			LabTile.RgbToLab(num, num2, num3, out num, out num2, out num3);
			this.L = num;
			this.A = num2;
			this.B = num3;
			base.Lock();
		}

		private static float Fxyz(float t)
		{
			return (float)((t > 0.008856f) ? Math.Pow((double)t, 0.33333333333333331) : ((double)(7.787f * t + 0.137931034f)));
		}

		private static float LinearToSrgb(float linear)
		{
			if (linear <= 0.04045f)
			{
				return linear / 12.92f;
			}
			return (float)Math.Pow((double)((linear + 0.055f) / 1.055f), 2.2000000476837158);
		}

		private static void RgbToLab(float rLinear, float gLinear, float bLinear, out float l, out float a, out float b)
		{
			float num = LabTile.LinearToSrgb(rLinear);
			float num2 = LabTile.LinearToSrgb(gLinear);
			float num3 = LabTile.LinearToSrgb(bLinear);
			float num4 = num * 0.4124f + num2 * 0.3576f + num3 * 0.1805f;
			float num5 = num * 0.2126f + num2 * 0.7152f + num3 * 0.0722f;
			float num6 = num * 0.0193f + num2 * 0.1192f + num3 * 0.9505f;
			num4 = ((num4 > 0.9505f) ? 0.9505f : ((num4 < 0f) ? 0f : num4));
			num5 = ((num5 > 1f) ? 1f : ((num5 < 0f) ? 0f : num5));
			num6 = ((num6 > 1.089f) ? 1.089f : ((num6 < 0f) ? 0f : num6));
			l = 116f * LabTile.Fxyz(num5 * 1f) - 16f;
			a = 500f * (LabTile.Fxyz(num4 * 1.05207789f) - LabTile.Fxyz(num5 * 1f));
			b = 200f * (LabTile.Fxyz(num5 * 1f) - LabTile.Fxyz(num6 * 0.9182736f));
		}
	}
}
