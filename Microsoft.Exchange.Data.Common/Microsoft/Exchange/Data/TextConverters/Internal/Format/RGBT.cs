using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct RGBT
	{
		public RGBT(uint rawValue)
		{
			this.rawValue = rawValue;
		}

		public RGBT(uint red, uint green, uint blue)
		{
			this.rawValue = (red << 16 | green << 8 | blue);
		}

		public RGBT(float redPercentage, float greenPercentage, float bluePercentage)
		{
			this.rawValue = ((uint)(redPercentage * 255f / 100f) << 16 | (uint)(greenPercentage * 255f / 100f) << 8 | (uint)(bluePercentage * 255f / 100f));
		}

		public RGBT(uint red, uint green, uint blue, uint transparency)
		{
			this.rawValue = (red << 16 | green << 8 | blue | transparency << 24);
		}

		public RGBT(float redPercentage, float greenPercentage, float bluePercentage, float transparencyPercentage)
		{
			this.rawValue = ((uint)(redPercentage * 255f / 100f) << 16 | (uint)(greenPercentage * 255f / 100f) << 8 | (uint)(bluePercentage * 255f / 100f) | (uint)(transparencyPercentage * 7f / 100f));
		}

		public uint RawValue
		{
			get
			{
				return this.rawValue;
			}
		}

		public uint RGB
		{
			get
			{
				return this.rawValue & 16777215U;
			}
		}

		public bool IsTransparent
		{
			get
			{
				return this.Transparency == 7U;
			}
		}

		public bool IsOpaque
		{
			get
			{
				return this.Transparency == 0U;
			}
		}

		public uint Transparency
		{
			get
			{
				return this.rawValue >> 24 & 7U;
			}
		}

		public uint Red
		{
			get
			{
				return this.rawValue >> 16 & 255U;
			}
		}

		public uint Green
		{
			get
			{
				return this.rawValue >> 8 & 255U;
			}
		}

		public uint Blue
		{
			get
			{
				return this.rawValue & 255U;
			}
		}

		public float RedPercentage
		{
			get
			{
				return (this.rawValue >> 16 & 255U) * 0.392156869f;
			}
		}

		public float GreenPercentage
		{
			get
			{
				return (this.rawValue >> 8 & 255U) * 0.392156869f;
			}
		}

		public float BluePercentage
		{
			get
			{
				return (this.rawValue & 255U) * 0.392156869f;
			}
		}

		public float TransparencyPercentage
		{
			get
			{
				return (this.rawValue >> 24 & 7U) * 14.2857141f;
			}
		}

		public override string ToString()
		{
			if (!this.IsTransparent)
			{
				return string.Concat(new string[]
				{
					"rgb(",
					this.Red.ToString(),
					", ",
					this.Green.ToString(),
					", ",
					this.Blue.ToString(),
					")",
					(this.Transparency != 0U) ? ("+t" + this.Transparency.ToString()) : string.Empty
				});
			}
			return "transparent";
		}

		private uint rawValue;
	}
}
