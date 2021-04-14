using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct PropertyValue
	{
		public PropertyValue(uint rawValue)
		{
			this.rawValue = rawValue;
		}

		public PropertyValue(bool value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(value);
		}

		public PropertyValue(PropertyType type, int value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(type, value);
		}

		public PropertyValue(PropertyType type, uint value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(type, value);
		}

		public PropertyValue(LengthUnits lengthUnits, float value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(lengthUnits, value);
		}

		public PropertyValue(LengthUnits lengthUnits, int value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(lengthUnits, value);
		}

		public PropertyValue(PropertyType type, float value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(type, value);
		}

		public PropertyValue(RGBT color)
		{
			this.rawValue = PropertyValue.ComposeRawValue(color);
		}

		public PropertyValue(Enum value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(value);
		}

		public uint RawValue
		{
			get
			{
				return this.rawValue;
			}
			set
			{
				this.rawValue = value;
			}
		}

		public uint RawType
		{
			get
			{
				return this.rawValue & 4160749568U;
			}
		}

		public PropertyType Type
		{
			get
			{
				return (PropertyType)((this.rawValue & 4160749568U) >> 27);
			}
		}

		public int Value
		{
			get
			{
				return (int)((int)(this.rawValue & 134217727U) << 5) >> 5;
			}
		}

		public uint UnsignedValue
		{
			get
			{
				return this.rawValue & 134217727U;
			}
		}

		public bool IsNull
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Null);
			}
		}

		public bool IsCalculated
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Calculated);
			}
		}

		public bool IsBool
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Bool);
			}
		}

		public bool IsEnum
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Enum);
			}
		}

		public bool IsString
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.String);
			}
		}

		public bool IsMultiValue
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.MultiValue);
			}
		}

		public bool IsRefCountedHandle
		{
			get
			{
				return this.IsString || this.IsMultiValue;
			}
		}

		public bool IsColor
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Color);
			}
		}

		public bool IsInteger
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Integer);
			}
		}

		public bool IsFractional
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Fractional);
			}
		}

		public bool IsPercentage
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Percentage);
			}
		}

		public bool IsAbsLength
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.AbsLength);
			}
		}

		public bool IsPixels
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Pixels);
			}
		}

		public bool IsEms
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Ems);
			}
		}

		public bool IsExs
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Exs);
			}
		}

		public bool IsMilliseconds
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Milliseconds);
			}
		}

		public bool IsKHz
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.kHz);
			}
		}

		public bool IsDegrees
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.Degrees);
			}
		}

		public bool IsHtmlFontUnits
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.HtmlFontUnits);
			}
		}

		public bool IsRelativeHtmlFontUnits
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.RelHtmlFontUnits);
			}
		}

		public bool IsAbsRelLength
		{
			get
			{
				return this.RawType == PropertyValue.GetRawType(PropertyType.AbsLength) || this.RawType == PropertyValue.GetRawType(PropertyType.RelLength) || this.RawType == PropertyValue.GetRawType(PropertyType.Pixels);
			}
		}

		public int StringHandle
		{
			get
			{
				return this.Value;
			}
		}

		public int MultiValueHandle
		{
			get
			{
				return this.Value;
			}
		}

		public bool Bool
		{
			get
			{
				return this.UnsignedValue != 0U;
			}
		}

		public int Enum
		{
			get
			{
				return (int)this.UnsignedValue;
			}
		}

		public RGBT Color
		{
			get
			{
				return new RGBT(this.UnsignedValue);
			}
		}

		public float Percentage
		{
			get
			{
				return (float)this.Value / 10000f;
			}
		}

		public int Percentage10K
		{
			get
			{
				return this.Value;
			}
		}

		public float Fractional
		{
			get
			{
				return (float)this.Value / 10000f;
			}
		}

		public int Integer
		{
			get
			{
				return this.Value;
			}
		}

		public int Milliseconds
		{
			get
			{
				return this.Value;
			}
		}

		public int KHz
		{
			get
			{
				return this.Value;
			}
		}

		public int Degrees
		{
			get
			{
				return this.Value;
			}
		}

		public int BaseUnits
		{
			get
			{
				return this.Value;
			}
		}

		public float Twips
		{
			get
			{
				return (float)this.Value / 8f;
			}
		}

		public int TwipsInteger
		{
			get
			{
				return this.Value / 8;
			}
		}

		public float Points
		{
			get
			{
				return (float)this.Value / 160f;
			}
		}

		public int PointsInteger
		{
			get
			{
				return this.Value / 160;
			}
		}

		public int PointsInteger160
		{
			get
			{
				return this.Value;
			}
		}

		public float Picas
		{
			get
			{
				return (float)this.Value / 1920f;
			}
		}

		public float Inches
		{
			get
			{
				return (float)this.Value / 11520f;
			}
		}

		public float Centimeters
		{
			get
			{
				return (float)this.Value / 4535.433f;
			}
		}

		public int MillimetersInteger
		{
			get
			{
				return this.Value / 454;
			}
		}

		public float Millimeters
		{
			get
			{
				return (float)this.Value / 453.5433f;
			}
		}

		public int HtmlFontUnits
		{
			get
			{
				return this.Value;
			}
		}

		public float Pixels
		{
			get
			{
				return (float)this.Value / 96f;
			}
		}

		public int PixelsInteger
		{
			get
			{
				return this.Value / 96;
			}
		}

		public int PixelsInteger96
		{
			get
			{
				return this.Value;
			}
		}

		public float Ems
		{
			get
			{
				return (float)this.Value / 160f;
			}
		}

		public int EmsInteger
		{
			get
			{
				return this.Value / 160;
			}
		}

		public int EmsInteger160
		{
			get
			{
				return this.Value;
			}
		}

		public float Exs
		{
			get
			{
				return (float)this.Value / 160f;
			}
		}

		public int ExsInteger
		{
			get
			{
				return this.Value / 160;
			}
		}

		public int ExsInteger160
		{
			get
			{
				return this.Value;
			}
		}

		public int RelativeHtmlFontUnits
		{
			get
			{
				return this.Value;
			}
		}

		public static uint GetRawType(PropertyType type)
		{
			return (uint)((uint)type << 27);
		}

		public static bool operator ==(PropertyValue x, PropertyValue y)
		{
			return x.rawValue == y.rawValue;
		}

		public static bool operator !=(PropertyValue x, PropertyValue y)
		{
			return x.rawValue != y.rawValue;
		}

		public void Set(uint rawValue)
		{
			this.rawValue = rawValue;
		}

		public void Set(bool value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(value);
		}

		public void Set(PropertyType type, int value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(type, value);
		}

		public void Set(PropertyType type, uint value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(type, value);
		}

		public void Set(LengthUnits lengthUnits, float value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(lengthUnits, value);
		}

		public void Set(PropertyType type, float value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(type, value);
		}

		public void Set(RGBT color)
		{
			this.rawValue = PropertyValue.ComposeRawValue(color);
		}

		public void Set(Enum value)
		{
			this.rawValue = PropertyValue.ComposeRawValue(value);
		}

		public override string ToString()
		{
			switch (this.Type)
			{
			case PropertyType.Null:
				return "null";
			case PropertyType.Calculated:
				return "calculated";
			case PropertyType.Bool:
				return this.Bool.ToString();
			case PropertyType.String:
				return "string: " + this.StringHandle.ToString();
			case PropertyType.MultiValue:
				return "multi: " + this.MultiValueHandle.ToString();
			case PropertyType.Enum:
				return "enum: " + this.Enum.ToString();
			case PropertyType.Color:
				return "color: " + this.Color.ToString();
			case PropertyType.Integer:
				return this.Integer.ToString() + " (integer)";
			case PropertyType.Fractional:
				return this.Fractional.ToString() + " (fractional)";
			case PropertyType.Percentage:
				return this.Percentage.ToString() + "%";
			case PropertyType.AbsLength:
				return string.Concat(new string[]
				{
					this.Points.ToString(),
					"pt (",
					this.Inches.ToString(),
					"in, ",
					this.Millimeters.ToString(),
					"mm) (abs)"
				});
			case PropertyType.RelLength:
				return string.Concat(new string[]
				{
					this.Points.ToString(),
					"pt (",
					this.Inches.ToString(),
					"in, ",
					this.Millimeters.ToString(),
					"mm) (rel)"
				});
			case PropertyType.Pixels:
				return this.Pixels.ToString() + "px";
			case PropertyType.Ems:
				return this.Ems.ToString() + "em";
			case PropertyType.Exs:
				return this.Exs.ToString() + "ex";
			case PropertyType.HtmlFontUnits:
				return this.HtmlFontUnits.ToString() + " (html font units)";
			case PropertyType.RelHtmlFontUnits:
				return this.RelativeHtmlFontUnits.ToString() + " (relative html font units)";
			case PropertyType.Multiple:
				return this.Integer.ToString() + "*";
			case PropertyType.Milliseconds:
				return this.Milliseconds.ToString() + "ms";
			case PropertyType.kHz:
				return this.KHz.ToString() + "kHz";
			case PropertyType.Degrees:
				return this.Degrees.ToString() + "deg";
			default:
				return "unknown value type";
			}
		}

		public override bool Equals(object obj)
		{
			return obj is PropertyValue && this.rawValue == ((PropertyValue)obj).rawValue;
		}

		public override int GetHashCode()
		{
			return (int)this.rawValue;
		}

		internal static int ConvertHtmlFontUnitsToTwips(int nHtmlSize)
		{
			nHtmlSize = Math.Max(1, Math.Min(7, nHtmlSize));
			return PropertyValue.sizesInTwips[nHtmlSize - 1];
		}

		internal static int ConvertTwipsToHtmlFontUnits(int twips)
		{
			for (int i = 0; i < PropertyValue.maxSizesInTwips.Length; i++)
			{
				if (twips <= PropertyValue.maxSizesInTwips[i])
				{
					return i + 1;
				}
			}
			return PropertyValue.maxSizesInTwips.Length + 1;
		}

		private static uint ComposeRawValue(bool value)
		{
			return PropertyValue.GetRawType(PropertyType.Bool) | (value ? 1U : 0U);
		}

		private static uint ComposeRawValue(PropertyType type, int value)
		{
			return (uint)((int)((int)type << 27) | (value & 134217727));
		}

		private static uint ComposeRawValue(PropertyType type, uint value)
		{
			return (uint)((uint)type << 27) | value;
		}

		private static uint ComposeRawValue(LengthUnits lengthUnits, float len)
		{
			switch (lengthUnits)
			{
			case LengthUnits.BaseUnits:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | ((uint)len & 134217727U);
			case LengthUnits.Twips:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | ((uint)(len * 8f) & 134217727U);
			case LengthUnits.Points:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | ((uint)(len * 160f) & 134217727U);
			case LengthUnits.Picas:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | ((uint)(len * 1920f) & 134217727U);
			case LengthUnits.Inches:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | ((uint)(len * 11520f) & 134217727U);
			case LengthUnits.Centimeters:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | ((uint)(len * 4535.433f) & 134217727U);
			case LengthUnits.Millimeters:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | ((uint)(len * 453.5433f) & 134217727U);
			case LengthUnits.HtmlFontUnits:
				return PropertyValue.GetRawType(PropertyType.HtmlFontUnits) | ((uint)len & 134217727U);
			case LengthUnits.Pixels:
				return PropertyValue.GetRawType(PropertyType.Pixels) | ((uint)(len * 96f) & 134217727U);
			case LengthUnits.Ems:
				return PropertyValue.GetRawType(PropertyType.Ems) | ((uint)(len * 160f) & 134217727U);
			case LengthUnits.Exs:
				return PropertyValue.GetRawType(PropertyType.Exs) | ((uint)(len * 160f) & 134217727U);
			case LengthUnits.RelativeHtmlFontUnits:
				return PropertyValue.GetRawType(PropertyType.RelHtmlFontUnits) | (uint)((int)len & 134217727);
			case LengthUnits.Percents:
				return PropertyValue.GetRawType(PropertyType.Percentage) | ((uint)len & 134217727U);
			default:
				return 0U;
			}
		}

		private static uint ComposeRawValue(LengthUnits lengthUnits, int len)
		{
			switch (lengthUnits)
			{
			case LengthUnits.BaseUnits:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint)(len & 134217727);
			case LengthUnits.Twips:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint)(len * 8 & 134217727);
			case LengthUnits.Points:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint)(len * 160 & 134217727);
			case LengthUnits.Picas:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint)(len * 1920 & 134217727);
			case LengthUnits.Inches:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint)(len * 11520 & 134217727);
			case LengthUnits.Centimeters:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint)(len * 4535 & 134217727);
			case LengthUnits.Millimeters:
				return PropertyValue.GetRawType(PropertyType.AbsLength) | (uint)(len * 453 & 134217727);
			case LengthUnits.HtmlFontUnits:
				return PropertyValue.GetRawType(PropertyType.HtmlFontUnits) | (uint)(len & 134217727);
			case LengthUnits.Pixels:
				return PropertyValue.GetRawType(PropertyType.Pixels) | (uint)(len * 96 & 134217727);
			case LengthUnits.Ems:
				return PropertyValue.GetRawType(PropertyType.Ems) | (uint)(len * 160 & 134217727);
			case LengthUnits.Exs:
				return PropertyValue.GetRawType(PropertyType.Exs) | (uint)(len * 160 & 134217727);
			case LengthUnits.RelativeHtmlFontUnits:
				return PropertyValue.GetRawType(PropertyType.RelHtmlFontUnits) | (uint)(len & 134217727);
			case LengthUnits.Percents:
				return PropertyValue.GetRawType(PropertyType.Percentage) | (uint)(len & 134217727);
			default:
				return 0U;
			}
		}

		private static uint ComposeRawValue(PropertyType type, float value)
		{
			return PropertyValue.GetRawType(type) | ((uint)(value * 10000f) & 134217727U);
		}

		private static uint ComposeRawValue(RGBT color)
		{
			return PropertyValue.GetRawType(PropertyType.Color) | (color.RawValue & 134217727U);
		}

		private static uint ComposeRawValue(Enum value)
		{
			return PropertyValue.GetRawType(PropertyType.Enum) | (Convert.ToUInt32(value) & 134217727U);
		}

		public const int ValueMax = 67108863;

		public const int ValueMin = -67108863;

		private const uint TypeMask = 4160749568U;

		private const int TypeShift = 27;

		private const uint ValueMask = 134217727U;

		private const int ValueShift = 5;

		public static readonly PropertyValue Null = default(PropertyValue);

		public static readonly PropertyValue True = new PropertyValue(true);

		public static readonly PropertyValue False = new PropertyValue(false);

		private static readonly int[] sizesInTwips = new int[]
		{
			151,
			200,
			240,
			271,
			360,
			480,
			720
		};

		private static readonly int[] maxSizesInTwips = new int[]
		{
			160,
			220,
			260,
			320,
			420,
			620
		};

		private uint rawValue;
	}
}
