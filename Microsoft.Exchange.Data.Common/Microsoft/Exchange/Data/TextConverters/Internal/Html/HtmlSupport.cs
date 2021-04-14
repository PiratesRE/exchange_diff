using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal static class HtmlSupport
	{
		public static PropertyValue ParseNumber(BufferString value, HtmlSupport.NumberParseFlags parseFlags)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			ulong num = 0UL;
			int num2 = 0;
			int num3 = 0;
			bool flag4 = false;
			int num4 = 0;
			int length = value.Length;
			while (num4 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num4])))
			{
				num4++;
			}
			if (num4 == length)
			{
				return PropertyValue.Null;
			}
			if (num4 < length && (value[num4] == '-' || value[num4] == '+'))
			{
				flag2 = true;
				flag3 = (value[num4] == '-');
				num4++;
			}
			while (num4 < length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[num4])))
			{
				flag = true;
				if (num < 1844674407370955152UL)
				{
					num = num * 10UL + (ulong)(value[num4] - '0');
				}
				else
				{
					num2++;
				}
				num4++;
			}
			if (num4 < length && value[num4] == '.')
			{
				flag4 = true;
				num4++;
				while (num4 < length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[num4])))
				{
					flag = true;
					if (num < 1844674407370955152UL)
					{
						num = num * 10UL + (ulong)(value[num4] - '0');
						num2--;
					}
					num4++;
				}
				if (num2 >= 0 && (parseFlags & HtmlSupport.NumberParseFlags.Strict) != (HtmlSupport.NumberParseFlags)0)
				{
					return PropertyValue.Null;
				}
			}
			if (!flag)
			{
				return PropertyValue.Null;
			}
			while (num4 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num4])))
			{
				num4++;
			}
			if (num4 < length && (value[num4] | ' ') == 'e' && num4 + 1 < length && (value[num4 + 1] == '-' || value[num4 + 1] == '+' || ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[num4 + 1]))))
			{
				flag4 = true;
				num4++;
				bool flag5 = false;
				if (value[num4] == '-' || value[num4] == '+')
				{
					flag5 = (value[num4] == '-');
					num4++;
				}
				while (num4 < length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[num4])))
				{
					num3 = num3 * 10 + (int)(value[num4++] - '0');
				}
				if (flag5)
				{
					num3 = -num3;
				}
				while (num4 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num4])))
				{
					num4++;
				}
			}
			uint num5 = flag4 ? 10000U : 1U;
			uint num6 = 1U;
			PropertyType propertyType = flag4 ? PropertyType.Fractional : PropertyType.Integer;
			bool flag6 = false;
			int num7 = 0;
			if (num4 + 1 < length)
			{
				if ((value[num4] | ' ') == 'p')
				{
					if ((value[num4 + 1] | ' ') == 'c')
					{
						num5 = 1920U;
						num6 = 1U;
						flag6 = true;
						propertyType = PropertyType.AbsLength;
						num7 = 2;
					}
					else if ((value[num4 + 1] | ' ') == 't')
					{
						num5 = 160U;
						num6 = 1U;
						flag6 = true;
						propertyType = PropertyType.AbsLength;
						num7 = 2;
					}
					else if ((value[num4 + 1] | ' ') == 'x')
					{
						num5 = 11520U;
						num6 = 120U;
						propertyType = PropertyType.Pixels;
						flag6 = true;
						num7 = 2;
					}
				}
				else if ((value[num4] | ' ') == 'e')
				{
					if ((value[num4 + 1] | ' ') == 'm')
					{
						num5 = 160U;
						num6 = 1U;
						propertyType = PropertyType.Ems;
						flag6 = true;
						num7 = 2;
					}
					else if ((value[num4 + 1] | ' ') == 'x')
					{
						num5 = 160U;
						num6 = 1U;
						propertyType = PropertyType.Exs;
						flag6 = true;
						num7 = 2;
					}
				}
				else if ((value[num4] | ' ') == 'i')
				{
					if ((value[num4 + 1] | ' ') == 'n')
					{
						num5 = 11520U;
						num6 = 1U;
						flag6 = true;
						propertyType = PropertyType.AbsLength;
						num7 = 2;
					}
				}
				else if ((value[num4] | ' ') == 'c')
				{
					if ((value[num4 + 1] | ' ') == 'm')
					{
						num5 = 1152000U;
						num6 = 254U;
						flag6 = true;
						propertyType = PropertyType.AbsLength;
						num7 = 2;
					}
				}
				else if ((value[num4] | ' ') == 'm' && (value[num4 + 1] | ' ') == 'm')
				{
					num5 = 115200U;
					num6 = 254U;
					flag6 = true;
					propertyType = PropertyType.AbsLength;
					num7 = 2;
				}
			}
			if (!flag6 && num4 < length)
			{
				if (value[num4] == '%')
				{
					num5 = 10000U;
					num6 = 1U;
					propertyType = PropertyType.Percentage;
					num7 = 1;
				}
				else if (value[num4] == '*')
				{
					num5 = 1U;
					num6 = 1U;
					propertyType = PropertyType.Multiple;
					num7 = 1;
				}
			}
			num4 += num7;
			if (num4 < length)
			{
				while (num4 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num4])))
				{
					num4++;
				}
				if (num4 < length && (parseFlags & (HtmlSupport.NumberParseFlags.StyleSheetProperty | HtmlSupport.NumberParseFlags.Strict)) != (HtmlSupport.NumberParseFlags)0)
				{
					return PropertyValue.Null;
				}
			}
			if (num != 0UL)
			{
				int num8 = num2 + num3;
				if (num8 > 0)
				{
					if (num8 > 20)
					{
						num8 = 0;
						num = ulong.MaxValue;
					}
					else
					{
						while (num8 != 0)
						{
							if (num > 1844674407370955161UL)
							{
								num8 = 0;
								num = ulong.MaxValue;
								break;
							}
							num *= 10UL;
							num8--;
						}
					}
				}
				else if (num8 < -10)
				{
					if (num8 < -21)
					{
						num8 = 0;
						num = 0UL;
					}
					else
					{
						while (num8 != -10)
						{
							num /= 10UL;
							num8++;
						}
					}
				}
				num *= (ulong)num5;
				num /= (ulong)num6;
				while (num8 != 0)
				{
					num /= 10UL;
					num8++;
				}
				if (num > 67108863UL)
				{
					num = 67108863UL;
				}
			}
			int num9 = (int)num;
			if (flag3)
			{
				num9 = -num9;
			}
			if (propertyType == PropertyType.Integer)
			{
				if ((parseFlags & HtmlSupport.NumberParseFlags.Integer) == (HtmlSupport.NumberParseFlags)0)
				{
					if ((parseFlags & HtmlSupport.NumberParseFlags.HtmlFontUnits) != (HtmlSupport.NumberParseFlags)0)
					{
						if (flag2)
						{
							if (num9 < -7)
							{
								num9 = -7;
							}
							else if (num9 > 7)
							{
								num9 = 7;
							}
							propertyType = PropertyType.RelHtmlFontUnits;
						}
						else
						{
							if (num9 < 1)
							{
								num9 = 1;
							}
							else if (num9 > 7)
							{
								num9 = 7;
							}
							propertyType = PropertyType.HtmlFontUnits;
						}
					}
					else if ((parseFlags & HtmlSupport.NumberParseFlags.AbsoluteLength) != (HtmlSupport.NumberParseFlags)0)
					{
						num = num * 11520UL / 120UL;
						if (num > 67108863UL)
						{
							num = 67108863UL;
						}
						num9 = (int)num;
						if (flag3)
						{
							num9 = -num9;
						}
						propertyType = PropertyType.Pixels;
					}
					else
					{
						if ((parseFlags & HtmlSupport.NumberParseFlags.Float) == (HtmlSupport.NumberParseFlags)0)
						{
							return PropertyValue.Null;
						}
						num *= 10000UL;
						if (num > 67108863UL)
						{
							num = 67108863UL;
						}
						num9 = (int)num;
						if (flag3)
						{
							num9 = -num9;
						}
						propertyType = PropertyType.Fractional;
					}
				}
			}
			else if (propertyType == PropertyType.Fractional)
			{
				if ((parseFlags & HtmlSupport.NumberParseFlags.Float) == (HtmlSupport.NumberParseFlags)0)
				{
					if ((parseFlags & HtmlSupport.NumberParseFlags.AbsoluteLength) == (HtmlSupport.NumberParseFlags)0)
					{
						return PropertyValue.Null;
					}
					num = num * 11520UL / 120UL / 10000UL;
					if (num > 67108863UL)
					{
						num = 67108863UL;
					}
					num9 = (int)num;
					if (flag3)
					{
						num9 = -num9;
					}
					propertyType = PropertyType.Pixels;
				}
			}
			else if (propertyType == PropertyType.AbsLength || propertyType == PropertyType.Pixels)
			{
				if ((parseFlags & HtmlSupport.NumberParseFlags.AbsoluteLength) == (HtmlSupport.NumberParseFlags)0)
				{
					return PropertyValue.Null;
				}
			}
			else if (propertyType == PropertyType.Ems || propertyType == PropertyType.Exs)
			{
				if ((parseFlags & HtmlSupport.NumberParseFlags.EmExLength) == (HtmlSupport.NumberParseFlags)0)
				{
					return PropertyValue.Null;
				}
			}
			else if (propertyType == PropertyType.Percentage)
			{
				if ((parseFlags & HtmlSupport.NumberParseFlags.Percentage) == (HtmlSupport.NumberParseFlags)0)
				{
					return PropertyValue.Null;
				}
			}
			else if (propertyType == PropertyType.Multiple && (parseFlags & HtmlSupport.NumberParseFlags.Multiple) == (HtmlSupport.NumberParseFlags)0)
			{
				return PropertyValue.Null;
			}
			if (num9 < 0 && (parseFlags & HtmlSupport.NumberParseFlags.NonNegative) != (HtmlSupport.NumberParseFlags)0 && propertyType != PropertyType.RelHtmlFontUnits)
			{
				return PropertyValue.Null;
			}
			return new PropertyValue(propertyType, num9);
		}

		public static BufferString FormatPixelOrPercentageLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			scratchBuffer.Reset();
			HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Integer | HtmlSupport.NumberParseFlags.Percentage);
			return scratchBuffer.BufferString;
		}

		public static BufferString FormatPixelLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			scratchBuffer.Reset();
			HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Integer);
			return scratchBuffer.BufferString;
		}

		public static BufferString FormatLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			scratchBuffer.Reset();
			HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Length);
			return scratchBuffer.BufferString;
		}

		public static BufferString FormatFontSize(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			scratchBuffer.Reset();
			HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.HtmlFontUnits);
			return scratchBuffer.BufferString;
		}

		public static void AppendPixelOrPercentageLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Integer | HtmlSupport.NumberParseFlags.Percentage);
		}

		public static void AppendPixelLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Integer);
		}

		public static void AppendLength(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Length);
		}

		public static void AppendFontSize(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.HtmlFontUnits);
		}

		public static void AppendCssFontSize(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.FontSize);
		}

		private static void AppendNumber(ref ScratchBuffer scratchBuffer, PropertyValue value, HtmlSupport.NumberParseFlags formatFlags)
		{
			if (value.IsPercentage)
			{
				if ((formatFlags & HtmlSupport.NumberParseFlags.Percentage) != (HtmlSupport.NumberParseFlags)0)
				{
					scratchBuffer.AppendFractional(value.Percentage10K, 10000);
					scratchBuffer.Append('%');
					return;
				}
			}
			else if (value.IsAbsRelLength)
			{
				if ((formatFlags & HtmlSupport.NumberParseFlags.Integer) != (HtmlSupport.NumberParseFlags)0)
				{
					scratchBuffer.AppendInt(value.PixelsInteger);
					return;
				}
				if ((formatFlags & HtmlSupport.NumberParseFlags.AbsoluteLength) != (HtmlSupport.NumberParseFlags)0)
				{
					if (value.IsPixels)
					{
						int pixelsInteger = value.PixelsInteger96;
						scratchBuffer.AppendFractional(pixelsInteger, 96);
						if (pixelsInteger != 0)
						{
							scratchBuffer.Append("px");
							return;
						}
					}
					else
					{
						int pointsInteger = value.PointsInteger160;
						scratchBuffer.AppendFractional(pointsInteger, 160);
						if (pointsInteger != 0)
						{
							scratchBuffer.Append("pt");
							return;
						}
					}
				}
				else if ((formatFlags & HtmlSupport.NumberParseFlags.HtmlFontUnits) != (HtmlSupport.NumberParseFlags)0)
				{
					scratchBuffer.AppendInt(PropertyValue.ConvertTwipsToHtmlFontUnits(value.TwipsInteger));
					return;
				}
			}
			else if (value.IsEms)
			{
				if ((formatFlags & HtmlSupport.NumberParseFlags.EmExLength) != (HtmlSupport.NumberParseFlags)0)
				{
					scratchBuffer.AppendFractional(value.EmsInteger160, 160);
					scratchBuffer.Append("em");
					return;
				}
			}
			else if (value.IsExs)
			{
				if ((formatFlags & HtmlSupport.NumberParseFlags.EmExLength) != (HtmlSupport.NumberParseFlags)0)
				{
					scratchBuffer.AppendFractional(value.ExsInteger160, 160);
					scratchBuffer.Append("ex");
					return;
				}
			}
			else if (value.IsHtmlFontUnits)
			{
				if ((formatFlags & HtmlSupport.NumberParseFlags.HtmlFontUnits) != (HtmlSupport.NumberParseFlags)0)
				{
					scratchBuffer.AppendInt(value.HtmlFontUnits);
					return;
				}
			}
			else if (value.IsRelativeHtmlFontUnits && (formatFlags & HtmlSupport.NumberParseFlags.HtmlFontUnits) != (HtmlSupport.NumberParseFlags)0)
			{
				if (value.RelativeHtmlFontUnits > 0)
				{
					scratchBuffer.Append("+");
					scratchBuffer.AppendInt(value.RelativeHtmlFontUnits);
					return;
				}
				if (value.RelativeHtmlFontUnits < 0)
				{
					scratchBuffer.AppendInt(value.RelativeHtmlFontUnits);
				}
			}
		}

		public static PropertyValue ParseEnum(BufferString value, HtmlSupport.EnumerationDef[] enumerationDefs)
		{
			value.TrimWhitespace();
			if (value.Length == 0)
			{
				return PropertyValue.Null;
			}
			for (int i = 0; i < enumerationDefs.Length; i++)
			{
				if (value.EqualsToLowerCaseStringIgnoreCase(enumerationDefs[i].Name))
				{
					return enumerationDefs[i].Value;
				}
			}
			return PropertyValue.Null;
		}

		public static string GetEnumString(PropertyValue value, HtmlSupport.EnumerationDef[] enumerationDefs)
		{
			for (int i = 0; i < enumerationDefs.Length; i++)
			{
				if (value.RawValue == enumerationDefs[i].Value.RawValue)
				{
					return enumerationDefs[i].Name;
				}
			}
			return null;
		}

		public static PropertyValue ParseBooleanAttribute(BufferString value, FormatConverter formatConverter)
		{
			return PropertyValue.True;
		}

		internal static PropertyValue ParseDirection(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.directionEnumeration);
		}

		internal static PropertyValue ParseTextAlignment(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.TextAlignmentEnumeration);
		}

		internal static string GetTextAlignmentString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.TextAlignmentEnumeration);
		}

		internal static PropertyValue ParseHorizontalAlignment(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.HorizontalAlignmentEnumeration);
		}

		internal static string GetHorizontalAlignmentString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.HorizontalAlignmentEnumeration);
		}

		internal static PropertyValue ParseVerticalAlignment(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.verticalAlignmentEnumeration);
		}

		internal static string GetVerticalAlignmentString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.verticalAlignmentEnumeration);
		}

		internal static PropertyValue ParseBlockAlignment(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.BlockAlignmentEnumeration);
		}

		internal static string GetBlockAlignmentString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.BlockAlignmentEnumeration);
		}

		internal static PropertyValue ParseBorderStyle(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.BorderStyleEnumeration);
		}

		internal static string GetBorderStyleString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.BorderStyleEnumeration);
		}

		internal static PropertyValue ParseTarget(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.targetEnumeration);
		}

		internal static string GetTargetString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.targetEnumeration);
		}

		internal static PropertyValue ParseFontWeight(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.fontWeightEnumeration);
		}

		internal static PropertyValue ParseCssFontSize(BufferString value, FormatConverter formatConverter)
		{
			PropertyValue result = HtmlSupport.ParseEnum(value, HtmlSupport.fontSizeEnumeration);
			if (result.IsNull)
			{
				result = HtmlSupport.ParseFontSize(value, formatConverter);
			}
			return result;
		}

		internal static PropertyValue ParseFontStyle(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.fontStyleEnumeration);
		}

		internal static PropertyValue ParseFontVariant(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.fontVariantEnumeration);
		}

		internal static PropertyValue ParseTableLayout(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.tableLayoutEnumeration);
		}

		internal static PropertyValue ParseBorderCollapse(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.borderCollapseEnumeration);
		}

		internal static PropertyValue ParseEmptyCells(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.emptyCellsEnumeration);
		}

		internal static PropertyValue ParseCaptionSide(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.captionSideEnumeration);
		}

		internal static PropertyValue ParseBorderWidth(BufferString value, FormatConverter formatConverter)
		{
			PropertyValue result = HtmlSupport.ParseEnum(value, HtmlSupport.borderWidthEnumeration);
			if (result.IsNull)
			{
				result = HtmlSupport.ParseNonNegativeLength(value, formatConverter);
			}
			return result;
		}

		internal static PropertyValue ParseTableFrame(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.tableFrameEnumeration);
		}

		internal static string GetTableFrameString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.tableFrameEnumeration);
		}

		internal static PropertyValue ParseTableRules(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.tableRulesEnumeration);
		}

		internal static string GetTableRulesString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.tableRulesEnumeration);
		}

		internal static PropertyValue ParseUnicodeBiDi(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.unicodeBiDiEnumeration);
		}

		internal static string GetUnicodeBiDiString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.unicodeBiDiEnumeration);
		}

		internal static PropertyValue ParseDisplay(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.displayEnumeration);
		}

		internal static string GetDisplayString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.displayEnumeration);
		}

		internal static PropertyValue ParseVisibility(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.visibilityEnumeration);
		}

		internal static PropertyValue ParseAreaShape(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseEnum(value, HtmlSupport.areaShapeEnumeration);
		}

		internal static string GetAreaShapeString(PropertyValue value)
		{
			return HtmlSupport.GetEnumString(value, HtmlSupport.areaShapeEnumeration);
		}

		internal static PropertyValue ParseLanguage(BufferString value, FormatConverter formatConverter)
		{
			value.TrimWhitespace();
			if (value.Length == 0)
			{
				return PropertyValue.Null;
			}
			Culture culture;
			if (Culture.TryGetCulture(value.ToString(), out culture) && culture.LCID != 0)
			{
				return new PropertyValue(PropertyType.Integer, culture.LCID);
			}
			return PropertyValue.Null;
		}

		public static PropertyValue ParseColor(BufferString value, bool enriched, bool css)
		{
			int num = 0;
			if (value.Length == 0 || value[0] != '#' || enriched)
			{
				bool flag = false;
				bool flag2 = false;
				for (int i = 0; i < value.Length; i++)
				{
					if (!ParseSupport.AlphaCharacter(ParseSupport.GetCharClass(value[i])))
					{
						flag = true;
						break;
					}
					if (!flag2 && !ParseSupport.HexCharacter(ParseSupport.GetCharClass(value[i])))
					{
						flag2 = true;
					}
				}
				if (!flag && flag2)
				{
					PropertyValue result = HtmlSupport.ParseNamedColor(value);
					if (!result.IsNull)
					{
						return result;
					}
				}
				if (!enriched)
				{
					PropertyValue result2 = HtmlSupport.ParseRgbColor(value);
					if (!result2.IsNull)
					{
						return result2;
					}
				}
			}
			else
			{
				num++;
			}
			if (value.Length > 0)
			{
				RGBT color;
				if (enriched)
				{
					if (HtmlSupport.ParseHexColorEnriched(value, num, out color))
					{
						return new PropertyValue(color);
					}
				}
				else if (HtmlSupport.ParseHexColor(value, num, css, out color))
				{
					return new PropertyValue(color);
				}
			}
			return PropertyValue.Null;
		}

		private static Dictionary<PropertyValue, string> BuildColorToNameDictionary()
		{
			Dictionary<PropertyValue, string> dictionary = new Dictionary<PropertyValue, string>();
			foreach (HtmlSupport.EnumerationDef enumerationDef in HtmlSupport.colorNames)
			{
				if (!dictionary.ContainsKey(enumerationDef.Value))
				{
					dictionary.Add(enumerationDef.Value, enumerationDef.Name);
				}
			}
			return dictionary;
		}

		private static PropertyValue ParseNamedColor(BufferString value)
		{
			int i = 0;
			int num = HtmlSupport.colorNames.Length - 1;
			while (i <= num)
			{
				int num2 = i + (num - i >> 1);
				int num3 = BufferString.CompareLowerCaseStringToBufferStringIgnoreCase(HtmlSupport.colorNames[num2].Name, value);
				if (num3 == 0)
				{
					return HtmlSupport.colorNames[num2].Value;
				}
				if (num3 < 0)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			return PropertyValue.Null;
		}

		internal static PropertyValue TranslateSystemColor(PropertyValue value)
		{
			switch (value.Enum)
			{
			case 0:
				return new PropertyValue(new RGBT(16777215U));
			case 1:
				return new PropertyValue(new RGBT(16777215U));
			case 2:
				return new PropertyValue(new RGBT(16777215U));
			case 3:
				return new PropertyValue(new RGBT(16777215U));
			case 4:
				return new PropertyValue(new RGBT(16777215U));
			case 5:
				return new PropertyValue(new RGBT(16777215U));
			case 6:
				return new PropertyValue(new RGBT(0U));
			case 7:
				return new PropertyValue(new RGBT(0U));
			case 8:
				return new PropertyValue(new RGBT(0U));
			case 9:
				return new PropertyValue(new RGBT(0U));
			case 10:
				return new PropertyValue(new RGBT(0U));
			case 11:
				return new PropertyValue(new RGBT(0U));
			case 12:
				return new PropertyValue(new RGBT(16777215U));
			case 13:
				return new PropertyValue(new RGBT(16777215U));
			case 14:
				return new PropertyValue(new RGBT(0U));
			case 15:
				return new PropertyValue(new RGBT(16777215U));
			case 16:
				return new PropertyValue(new RGBT(16777215U));
			case 17:
				return new PropertyValue(new RGBT(0U));
			case 18:
				return new PropertyValue(new RGBT(0U));
			case 19:
				return new PropertyValue(new RGBT(0U));
			case 20:
				return new PropertyValue(new RGBT(16777215U));
			case 21:
				return new PropertyValue(new RGBT(16777215U));
			case 22:
				return new PropertyValue(new RGBT(16777215U));
			case 23:
				return new PropertyValue(new RGBT(0U));
			case 24:
				return new PropertyValue(new RGBT(16777215U));
			default:
				return PropertyValue.Null;
			}
		}

		private static PropertyValue ParseRgbColor(BufferString value)
		{
			if (value.Length <= 4 || !value.StartsWithLowerCaseStringIgnoreCase("rgb("))
			{
				return PropertyValue.Null;
			}
			int num = 4;
			uint red;
			if (!HtmlSupport.ParseRgbParam(value, ref num, out red))
			{
				return PropertyValue.Null;
			}
			uint green;
			if (!HtmlSupport.ParseRgbParam(value, ref num, out green))
			{
				return PropertyValue.Null;
			}
			uint blue;
			if (!HtmlSupport.ParseRgbParam(value, ref num, out blue))
			{
				return PropertyValue.Null;
			}
			if (num != value.Length - 1 || value[num] != ')')
			{
				return PropertyValue.Null;
			}
			return new PropertyValue(new RGBT(red, green, blue));
		}

		private static bool ParseRgbParam(BufferString str, ref int offset, out uint result)
		{
			uint num = 0U;
			uint num2 = 1U;
			bool flag = false;
			while (offset < str.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(str[offset])))
			{
				offset++;
			}
			if (offset < str.Length && str[offset] == '-')
			{
				flag = true;
				offset++;
			}
			if (offset == str.Length || !ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
			{
				result = 0U;
				return false;
			}
			while (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
			{
				num *= 10U;
				num += (uint)(str[offset] - '0');
				offset++;
				if (num > 255U)
				{
					while (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
					{
						offset++;
					}
				}
			}
			if (offset < str.Length && str[offset] == '.')
			{
				offset++;
				while (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
				{
					num *= 10U;
					num += (uint)(str[offset] - '0');
					num2 *= 10U;
					offset++;
					if (num > 421075U)
					{
						while (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
						{
							offset++;
						}
					}
				}
			}
			if (offset < str.Length && str[offset] == '%')
			{
				if (num / num2 >= 100U)
				{
					result = 255U;
				}
				else
				{
					result = num * 255U / (num2 * 100U);
				}
				offset++;
			}
			else if (num / num2 > 255U)
			{
				result = 255U;
			}
			else
			{
				result = num / num2;
			}
			while (offset < str.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(str[offset])))
			{
				offset++;
			}
			if (offset < str.Length && str[offset] == ',')
			{
				offset++;
			}
			if (flag)
			{
				result = 0U;
			}
			return true;
		}

		private static bool ParseHexColor(BufferString str, int offset, bool css, out RGBT rgbt)
		{
			int num = str.Length - offset;
			if (css && num != 3 && num != 6 && num != 9)
			{
				rgbt = default(RGBT);
				return false;
			}
			int num2 = (str.Length - offset + 3 - 1) / 3;
			uint num3 = 0U;
			uint num4;
			uint num5;
			uint num6;
			if (!HtmlSupport.ParseHexColorPart(num2, str, ref offset, ref num3, css, out num4) || !HtmlSupport.ParseHexColorPart(num2, str, ref offset, ref num3, css, out num5) || !HtmlSupport.ParseHexColorPart(num2, str, ref offset, ref num3, css, out num6))
			{
				rgbt = default(RGBT);
				return false;
			}
			int num7 = 0;
			while (num3 > 255U)
			{
				num3 >>= 4;
				num7++;
			}
			if (num7 > 0)
			{
				num4 >>= num7 * 4;
				num5 >>= num7 * 4;
				num6 >>= num7 * 4;
			}
			if (css && num2 == 1)
			{
				num4 += num4 << 4;
				num5 += num5 << 4;
				num6 += num6 << 4;
			}
			rgbt = new RGBT(num4, num5, num6);
			return true;
		}

		private static bool ParseHexColorPart(int vlen, BufferString str, ref int offset, ref uint max, bool css, out uint result)
		{
			result = 0U;
			for (int i = 0; i < vlen; i++)
			{
				int num;
				if (offset >= str.Length)
				{
					if (css)
					{
						return false;
					}
					num = 0;
				}
				else if (ParseSupport.HexCharacter(ParseSupport.GetCharClass(str[offset])))
				{
					num = ParseSupport.CharToHex(str[offset]);
				}
				else
				{
					if (css)
					{
						return false;
					}
					num = 0;
				}
				result = (result << 4) + (uint)num;
				offset++;
			}
			if (result > max)
			{
				max = result;
			}
			return true;
		}

		private static bool ParseHexColorEnriched(BufferString str, int offset, out RGBT rgbt)
		{
			uint num;
			uint num2;
			uint num3;
			if (!HtmlSupport.ParseHexColorPartEnriched(str, ref offset, out num) || !HtmlSupport.ParseHexColorPartEnriched(str, ref offset, out num2) || !HtmlSupport.ParseHexColorPartEnriched(str, ref offset, out num3))
			{
				rgbt = default(RGBT);
				return false;
			}
			num >>= 8;
			num2 >>= 8;
			num3 >>= 8;
			rgbt = new RGBT(num, num2, num3);
			return true;
		}

		private static bool ParseHexColorPartEnriched(BufferString str, ref int offset, out uint result)
		{
			result = 0U;
			for (int i = 0; i < 4; i++)
			{
				int num;
				if (offset >= str.Length)
				{
					num = 0;
				}
				else
				{
					if (ParseSupport.HexCharacter(ParseSupport.GetCharClass(str[offset])))
					{
						num = ParseSupport.CharToHex(str[offset]);
					}
					else
					{
						num = 0;
					}
					offset++;
				}
				result = (result << 4) + (uint)num;
			}
			if (offset < str.Length && str[offset] == ',')
			{
				offset++;
			}
			return true;
		}

		internal static PropertyValue ParseStringProperty(BufferString value, FormatConverter formatConverter)
		{
			value.TrimWhitespace();
			if (value.Length == 0)
			{
				return PropertyValue.Null;
			}
			return formatConverter.RegisterStringValue(false, value.ToString(), 0, value.Length).PropertyValue;
		}

		internal static PropertyValue ParseFontFace(BufferString value, FormatConverter formatConverter)
		{
			int num = 0;
			int length = value.Length;
			while (num < length && (value[num] == ',' || ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num]))))
			{
				num++;
			}
			if (num == length)
			{
				return PropertyValue.Null;
			}
			char c = ',';
			if (value[num] == '\'' || value[num] == '"')
			{
				c = value[num];
				num++;
			}
			int i = num;
			int num2 = num;
			while (i < length)
			{
				if (value[i] == c)
				{
					break;
				}
				i++;
				num2++;
			}
			while (num < i && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[i - 1])))
			{
				i--;
			}
			PropertyValue propertyValue = formatConverter.RegisterFaceName(false, value.SubString(num, i - num));
			if (num2 < length)
			{
				num2++;
			}
			num = num2;
			while (num < length && (value[num] == ',' || ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num]))))
			{
				num++;
			}
			if (num == length)
			{
				return propertyValue;
			}
			MultiValueBuilder multiValueBuilder;
			MultiValue multiValue = formatConverter.RegisterMultiValue(false, out multiValueBuilder);
			if (!propertyValue.IsNull)
			{
				multiValueBuilder.AddValue(propertyValue);
			}
			do
			{
				c = ',';
				if (value[num] == '\'' || value[num] == '"')
				{
					c = value[num];
					num++;
				}
				i = num;
				num2 = num;
				while (i < length && value[i] != c)
				{
					i++;
					num2++;
				}
				num2 = i;
				while (num < i && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[i - 1])))
				{
					i--;
				}
				propertyValue = formatConverter.RegisterFaceName(false, value.SubString(num, i - num));
				if (!propertyValue.IsNull)
				{
					multiValueBuilder.AddValue(propertyValue);
				}
				if (multiValueBuilder.Count == MultiValueBuildHelper.MaxValues)
				{
					break;
				}
				if (num2 < length)
				{
					num2++;
				}
				num = num2;
				while (num < length && (value[num] == ',' || ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[num]))))
				{
					num++;
				}
			}
			while (num < length);
			if (multiValueBuilder.Count == 0)
			{
				multiValueBuilder.Cancel();
				multiValue.Release();
				return PropertyValue.Null;
			}
			if (multiValueBuilder.Count == 1)
			{
				propertyValue = multiValueBuilder[0];
				if (propertyValue.IsString)
				{
					formatConverter.GetStringValue(propertyValue).AddRef();
				}
				multiValueBuilder.Cancel();
				multiValue.Release();
				return propertyValue;
			}
			multiValueBuilder.Flush();
			return multiValue.PropertyValue;
		}

		internal static PropertyValue ParseColor(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseColor(value, false, false);
		}

		internal static PropertyValue ParseColorCss(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseColor(value, false, true);
		}

		public static BufferString FormatColor(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			scratchBuffer.Reset();
			HtmlSupport.AppendColor(ref scratchBuffer, value);
			return scratchBuffer.BufferString;
		}

		public static void AppendColor(ref ScratchBuffer scratchBuffer, PropertyValue value)
		{
			if (value.IsColor || value.IsEnum)
			{
				string str;
				if (HtmlSupport.colorToNameDictionary.TryGetValue(value, out str))
				{
					scratchBuffer.Append(str);
					return;
				}
				scratchBuffer.Append("#");
				scratchBuffer.AppendHex2(value.Color.Red);
				scratchBuffer.AppendHex2(value.Color.Green);
				scratchBuffer.AppendHex2(value.Color.Blue);
			}
		}

		internal static PropertyValue ParseFontSize(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseNumber(value, HtmlSupport.NumberParseFlags.FontSize);
		}

		internal static PropertyValue ParseInteger(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseNumber(value, HtmlSupport.NumberParseFlags.Integer);
		}

		internal static PropertyValue ParseNonNegativeInteger(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseNumber(value, HtmlSupport.NumberParseFlags.Integer | HtmlSupport.NumberParseFlags.NonNegative);
		}

		internal static PropertyValue ParseLength(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseNumber(value, HtmlSupport.NumberParseFlags.Length);
		}

		internal static PropertyValue ParseNonNegativeLength(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseNumber(value, HtmlSupport.NumberParseFlags.NonNegativeLength);
		}

		internal static PropertyValue ParseUrl(BufferString value, FormatConverter formatConverter)
		{
			return HtmlSupport.ParseStringProperty(value, formatConverter);
		}

		public static void ScanSkipWhitespace(ref BufferString value)
		{
			int num = value.Length;
			int num2 = value.Offset;
			while (num != 0 && ParseSupport.WhitespaceCharacter(value.Buffer[num2]))
			{
				num2++;
				num--;
			}
			value.Trim(num2 - value.Offset, num);
		}

		public static void ScanRevertLastToken(ref BufferString value, BufferString token)
		{
			value.Set(value.Buffer, token.Offset, value.Length + token.Length);
		}

		public static BufferString ScanNextNonWhitespaceToken(ref BufferString value)
		{
			int num = value.Length;
			int num2 = value.Offset;
			while (num != 0 && !ParseSupport.WhitespaceCharacter(value.Buffer[num2]))
			{
				num2++;
				num--;
			}
			BufferString result = new BufferString(value.Buffer, value.Offset, num2 - value.Offset);
			value.Trim(num2 - value.Offset, num);
			return result;
		}

		public static BufferString ScanNextParenthesizedToken(ref BufferString value)
		{
			int num = value.Length;
			int num2 = value.Offset;
			char c = '"';
			bool flag = false;
			int num3 = 0;
			while (num != 0 && (num3 != 0 || !ParseSupport.WhitespaceCharacter(value.Buffer[num2])))
			{
				char c2 = value.Buffer[num2];
				if (!flag)
				{
					if (c2 == '\'' || c2 == '"')
					{
						flag = true;
						c = c2;
					}
					else if (num3 != 0 && c2 == ')')
					{
						num3--;
					}
					else if (c2 == '(')
					{
						num3++;
					}
				}
				else if (c2 == c)
				{
					flag = false;
				}
				num2++;
				num--;
			}
			BufferString result = new BufferString(value.Buffer, value.Offset, num2 - value.Offset);
			value.Trim(num2 - value.Offset, num);
			return result;
		}

		public static BufferString ScanNextSize(ref BufferString value)
		{
			int num = value.Length;
			int num2 = value.Offset;
			if (num != 0)
			{
				char c = value.Buffer[num2];
				if (c == '-' || c == '+')
				{
					num2++;
					num--;
				}
				if (num != 0)
				{
					if (ParseSupport.NumericCharacter(c = value.Buffer[num2]) || c == '.')
					{
						while (num != 0 && (ParseSupport.NumericCharacter(c = value.Buffer[num2]) || c == '.'))
						{
							num2++;
							num--;
						}
						int num3 = num2;
						while (num != 0 && ParseSupport.WhitespaceCharacter(value.Buffer[num2]))
						{
							num2++;
							num--;
						}
						if (num >= 2 && (((c = ParseSupport.ToLowerCase(value.Buffer[num2])) == 'i' && ParseSupport.ToLowerCase(value.Buffer[num2 + 1]) == 'n') || (c == 'c' && ParseSupport.ToLowerCase(value.Buffer[num2 + 1]) == 'm') || (c == 'm' && ParseSupport.ToLowerCase(value.Buffer[num2 + 1]) == 'm') || (c == 'e' && (ParseSupport.ToLowerCase(value.Buffer[num2 + 1]) == 'm' || ParseSupport.ToLowerCase(value.Buffer[num2 + 1]) == 'x')) || (c == 'p' && (ParseSupport.ToLowerCase(value.Buffer[num2 + 1]) == 't' || ParseSupport.ToLowerCase(value.Buffer[num2 + 1]) == 'c' || ParseSupport.ToLowerCase(value.Buffer[num2 + 1]) == 'x'))))
						{
							num2 += 2;
							num -= 2;
							goto IL_1AB;
						}
						if (num != 0 && value.Buffer[num2] == '%')
						{
							num2++;
							num--;
							goto IL_1AB;
						}
						num += num2 - num3;
						num2 = num3;
						goto IL_1AB;
					}
				}
				while (num != 0)
				{
					if (!ParseSupport.AlphaCharacter(c = value.Buffer[num2]) && c != '-')
					{
						break;
					}
					num2++;
					num--;
				}
			}
			IL_1AB:
			BufferString result = new BufferString(value.Buffer, value.Offset, num2 - value.Offset);
			value.Trim(num2 - value.Offset, num);
			return result;
		}

		internal static void ParseCompositeFourSidesValue(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount, PropertyValueParsingMethod valueParsingMethod, bool measurements)
		{
			BufferString value2 = BufferString.Null;
			HtmlSupport.ScanSkipWhitespace(ref value);
			int num = 0;
			if (!value.IsEmpty)
			{
				if (!measurements)
				{
					value2 = HtmlSupport.ScanNextParenthesizedToken(ref value);
				}
				else
				{
					value2 = HtmlSupport.ScanNextSize(ref value);
				}
				outputProperties[num].Value = valueParsingMethod(value2, formatConverter);
				if (!outputProperties[num].Value.IsNull)
				{
					num++;
					HtmlSupport.ScanSkipWhitespace(ref value);
					if (!value.IsEmpty)
					{
						if (!measurements)
						{
							value2 = HtmlSupport.ScanNextParenthesizedToken(ref value);
						}
						else
						{
							value2 = HtmlSupport.ScanNextSize(ref value);
						}
						outputProperties[num].Value = valueParsingMethod(value2, formatConverter);
						if (!outputProperties[num].Value.IsNull)
						{
							num++;
							HtmlSupport.ScanSkipWhitespace(ref value);
							if (!value.IsEmpty)
							{
								if (!measurements)
								{
									value2 = HtmlSupport.ScanNextParenthesizedToken(ref value);
								}
								else
								{
									value2 = HtmlSupport.ScanNextSize(ref value);
								}
								outputProperties[num].Value = valueParsingMethod(value2, formatConverter);
								if (!outputProperties[num].Value.IsNull)
								{
									num++;
									HtmlSupport.ScanSkipWhitespace(ref value);
									if (!value.IsEmpty)
									{
										if (!measurements)
										{
											value2 = HtmlSupport.ScanNextParenthesizedToken(ref value);
										}
										else
										{
											value2 = HtmlSupport.ScanNextSize(ref value);
										}
										outputProperties[num].Value = valueParsingMethod(value2, formatConverter);
										if (!outputProperties[num].Value.IsNull)
										{
											num++;
										}
									}
								}
							}
						}
					}
				}
			}
			if (num == 1)
			{
				outputProperties[0].Id = groupPropertyId;
				outputProperties[1].Set(groupPropertyId + 1, outputProperties[0].Value);
				outputProperties[2].Set(groupPropertyId + 2, outputProperties[0].Value);
				outputProperties[3].Set(groupPropertyId + 3, outputProperties[0].Value);
				parsedPropertiesCount = 4;
				return;
			}
			if (num == 2)
			{
				outputProperties[0].Id = groupPropertyId;
				outputProperties[1].Id = groupPropertyId + 1;
				outputProperties[2].Set(groupPropertyId + 2, outputProperties[0].Value);
				outputProperties[3].Set(groupPropertyId + 3, outputProperties[1].Value);
				parsedPropertiesCount = 4;
				return;
			}
			if (num == 3)
			{
				outputProperties[0].Id = groupPropertyId;
				outputProperties[1].Id = groupPropertyId + 1;
				outputProperties[2].Id = groupPropertyId + 2;
				outputProperties[3].Set(groupPropertyId + 3, outputProperties[1].Value);
				parsedPropertiesCount = 4;
				return;
			}
			if (num == 4)
			{
				outputProperties[0].Id = groupPropertyId;
				outputProperties[1].Id = groupPropertyId + 1;
				outputProperties[2].Id = groupPropertyId + 2;
				outputProperties[3].Id = groupPropertyId + 3;
				parsedPropertiesCount = 4;
				return;
			}
			parsedPropertiesCount = 0;
		}

		internal static void ParseCompositeLength(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			HtmlSupport.ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseLength, true);
		}

		internal static void ParseCompositeNonNegativeLength(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			HtmlSupport.ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, true);
		}

		internal static void ParseCompositeColor(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			HtmlSupport.ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, false);
		}

		internal static void ParseCompositeBorderWidth(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			HtmlSupport.ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, true);
		}

		internal static void ParseCompositeBorderStyle(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			HtmlSupport.ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, false);
		}

		internal static void ParseCompoundBorderSpacing(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			BufferString value2 = BufferString.Null;
			HtmlSupport.ScanSkipWhitespace(ref value);
			int num = 0;
			if (!value.IsEmpty)
			{
				value2 = HtmlSupport.ScanNextSize(ref value);
				outputProperties[num].Value = HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength(value2, formatConverter);
				if (!outputProperties[num].Value.IsNull)
				{
					num++;
					HtmlSupport.ScanSkipWhitespace(ref value);
					if (!value.IsEmpty)
					{
						value2 = HtmlSupport.ScanNextSize(ref value);
						outputProperties[num].Value = HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength(value2, formatConverter);
						if (!outputProperties[num].Value.IsNull)
						{
							num++;
						}
					}
				}
			}
			if (num == 1)
			{
				outputProperties[0].Id = groupPropertyId;
				outputProperties[1].Set(groupPropertyId + 1, outputProperties[0].Value);
				parsedPropertiesCount = 2;
				return;
			}
			if (num == 2)
			{
				outputProperties[0].Id = groupPropertyId;
				outputProperties[1].Id = groupPropertyId + 1;
				parsedPropertiesCount = 2;
				return;
			}
			parsedPropertiesCount = 0;
		}

		internal static void ParseCompositeBorder(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			BufferString value2 = BufferString.Null;
			HtmlSupport.ScanSkipWhitespace(ref value);
			int num = 0;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			while (!value.IsEmpty)
			{
				value2 = HtmlSupport.ScanNextParenthesizedToken(ref value);
				PropertyValue value3 = HtmlSupport.ParseBorderWidth(value2, formatConverter);
				if (value3.IsNull)
				{
					value3 = HtmlSupport.ParseBorderStyle(value2, formatConverter);
					if (value3.IsNull)
					{
						value3 = HtmlSupport.ParseColorCss(value2, formatConverter);
						if (value3.IsNull)
						{
							break;
						}
						if (num4 == -1)
						{
							num4 = num;
							outputProperties[num++].Set(groupPropertyId + 8, value3);
						}
						else
						{
							outputProperties[num4].Set(groupPropertyId + 8, value3);
						}
					}
					else if (num3 == -1)
					{
						num3 = num;
						outputProperties[num++].Set(groupPropertyId + 4, value3);
					}
					else
					{
						outputProperties[num3].Set(groupPropertyId + 4, value3);
					}
				}
				else if (num2 == -1)
				{
					num2 = num;
					outputProperties[num++].Set(groupPropertyId, value3);
				}
				else
				{
					outputProperties[num2].Set(groupPropertyId, value3);
				}
				HtmlSupport.ScanSkipWhitespace(ref value);
			}
			parsedPropertiesCount = num;
		}

		internal static void ParseCompositeAllBorders(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			HtmlSupport.ParseCompositeBorder(value, formatConverter, PropertyId.BorderWidths, outputProperties, out parsedPropertiesCount);
			for (int i = 0; i < parsedPropertiesCount; i++)
			{
				for (int j = 1; j < 4; j++)
				{
					outputProperties[parsedPropertiesCount * j + i].Set(outputProperties[i].Id + (byte)j, outputProperties[i].Value);
				}
			}
			parsedPropertiesCount += parsedPropertiesCount * 3;
		}

		internal static void ParseCompositeBackground(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			BufferString value2 = BufferString.Null;
			HtmlSupport.ScanSkipWhitespace(ref value);
			int num = 0;
			if (!value.IsEmpty)
			{
				value2 = HtmlSupport.ScanNextNonWhitespaceToken(ref value);
				outputProperties[num].Set(PropertyId.BackColor, HtmlSupport.ParseColorCss(value2, formatConverter));
				if (!outputProperties[num].Value.IsNull)
				{
					num++;
				}
				HtmlSupport.ScanSkipWhitespace(ref value);
			}
			parsedPropertiesCount = num;
		}

		internal static void ParseCssTextDecoration(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			BufferString value2 = BufferString.Null;
			HtmlSupport.ScanSkipWhitespace(ref value);
			int num = 0;
			value2 = HtmlSupport.ScanNextNonWhitespaceToken(ref value);
			PropertyValue propertyValue = HtmlSupport.ParseEnum(value2, HtmlSupport.cssTextDecorationEnumeration);
			if (!propertyValue.IsNull)
			{
				switch (propertyValue.Enum)
				{
				case 0:
				case 2:
				case 4:
					outputProperties[num++].Set(PropertyId.Underline, PropertyValue.False);
					outputProperties[num++].Set(PropertyId.Strikethrough, PropertyValue.False);
					break;
				case 1:
					outputProperties[num++].Set(PropertyId.Underline, PropertyValue.True);
					outputProperties[num++].Set(PropertyId.Strikethrough, PropertyValue.False);
					break;
				case 3:
					outputProperties[num++].Set(PropertyId.Underline, PropertyValue.False);
					outputProperties[num++].Set(PropertyId.Strikethrough, PropertyValue.True);
					break;
				}
			}
			parsedPropertiesCount = num;
		}

		internal static void ParseCssTextTransform(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			BufferString value2 = BufferString.Null;
			HtmlSupport.ScanSkipWhitespace(ref value);
			int num = 0;
			value2 = HtmlSupport.ScanNextNonWhitespaceToken(ref value);
			PropertyValue propertyValue = HtmlSupport.ParseEnum(value2, HtmlSupport.cssTextTransformEnumeration);
			if (!propertyValue.IsNull)
			{
				switch (propertyValue.Enum)
				{
				case 0:
					outputProperties[num++].Set(PropertyId.Capitalize, PropertyValue.True);
					break;
				case 1:
				case 2:
				case 3:
					outputProperties[num++].Set(PropertyId.Capitalize, PropertyValue.False);
					break;
				}
			}
			parsedPropertiesCount = num;
		}

		internal static void ParseCssVerticalAlignment(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			BufferString value2 = BufferString.Null;
			HtmlSupport.ScanSkipWhitespace(ref value);
			int num = 0;
			value2 = HtmlSupport.ScanNextNonWhitespaceToken(ref value);
			PropertyValue value3 = HtmlSupport.ParseEnum(value2, HtmlSupport.cssVerticalAlignmentEnumeration);
			if (!value3.IsNull)
			{
				switch (value3.Enum)
				{
				case 0:
				case 1:
				case 2:
				case 5:
					outputProperties[num++].Set(PropertyId.BlockAlignment, value3);
					break;
				case 7:
					outputProperties[num++].Set(PropertyId.Subscript, PropertyValue.True);
					outputProperties[num++].Set(PropertyId.Superscript, PropertyValue.False);
					break;
				case 8:
					outputProperties[num++].Set(PropertyId.Superscript, PropertyValue.True);
					outputProperties[num++].Set(PropertyId.Subscript, PropertyValue.False);
					break;
				}
			}
			parsedPropertiesCount = num;
		}

		internal static void ParseCssWhiteSpace(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			BufferString value2 = BufferString.Null;
			HtmlSupport.ScanSkipWhitespace(ref value);
			int num = 0;
			value2 = HtmlSupport.ScanNextNonWhitespaceToken(ref value);
			PropertyValue propertyValue = HtmlSupport.ParseEnum(value2, HtmlSupport.cssWhiteSpaceEnumeration);
			if (!propertyValue.IsNull)
			{
				switch (propertyValue.Enum)
				{
				case 1:
				case 2:
				case 3:
				case 4:
					outputProperties[num++].Set(PropertyId.Preformatted, PropertyValue.True);
					break;
				}
			}
			parsedPropertiesCount = num;
		}

		internal static void ParseCompositeFont(BufferString value, FormatConverter formatConverter, PropertyId groupPropertyId, Property[] outputProperties, out int parsedPropertiesCount)
		{
			BufferString bufferString = BufferString.Null;
			HtmlSupport.ScanSkipWhitespace(ref value);
			int num = 0;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			while (!value.IsEmpty)
			{
				bufferString = HtmlSupport.ScanNextNonWhitespaceToken(ref value);
				PropertyValue value2 = HtmlSupport.ParseFontWeight(bufferString, formatConverter);
				if (value2.IsNull)
				{
					value2 = HtmlSupport.ParseFontStyle(bufferString, formatConverter);
					if (value2.IsNull)
					{
						value2 = HtmlSupport.ParseFontVariant(bufferString, formatConverter);
						if (value2.IsNull)
						{
							break;
						}
						if (num3 == -1)
						{
							num3 = num;
							outputProperties[num++].Set(PropertyId.SmallCaps, value2);
						}
						else
						{
							outputProperties[num3].Set(PropertyId.SmallCaps, value2);
						}
					}
					else if (num4 == -1)
					{
						num4 = num;
						outputProperties[num++].Set(PropertyId.Italic, value2);
					}
					else
					{
						outputProperties[num4].Set(PropertyId.Italic, value2);
					}
				}
				else if (num2 == -1)
				{
					num2 = num;
					outputProperties[num++].Set(PropertyId.FirstFlag, value2);
				}
				else
				{
					outputProperties[num2].Set(PropertyId.FirstFlag, value2);
				}
				bufferString = BufferString.Null;
				HtmlSupport.ScanSkipWhitespace(ref value);
			}
			if (!bufferString.IsEmpty)
			{
				HtmlSupport.ScanRevertLastToken(ref value, bufferString);
				bufferString = HtmlSupport.ScanNextSize(ref value);
				outputProperties[num].Set(PropertyId.FontSize, HtmlSupport.ParseCssFontSize(bufferString, formatConverter));
				if (!outputProperties[num].Value.IsNull)
				{
					num++;
				}
			}
			HtmlSupport.ScanSkipWhitespace(ref value);
			if (!value.IsEmpty && value[0] == '/')
			{
				value.Trim(1, value.Length - 1);
				HtmlSupport.ScanSkipWhitespace(ref value);
				bufferString = HtmlSupport.ScanNextSize(ref value);
				HtmlSupport.ParseNonNegativeLength(bufferString, formatConverter);
				HtmlSupport.ScanSkipWhitespace(ref value);
			}
			if (!value.IsEmpty)
			{
				outputProperties[num].Set(PropertyId.FontSize, HtmlSupport.ParseFontFace(value, formatConverter));
				if (!outputProperties[num].Value.IsNull)
				{
					num++;
				}
			}
			parsedPropertiesCount = num;
		}

		public const int HtmlNestingLimit = 4096;

		public const int MaxAttributeSize = 4096;

		public const int MaxCssPropertySize = 4096;

		public const int MaxNumberOfNonInlineStyles = 128;

		public static readonly byte[] UnsafeAsciiMap = new byte[]
		{
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			0,
			0,
			2,
			2,
			0,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			0,
			2,
			3,
			2,
			2,
			2,
			3,
			2,
			2,
			2,
			2,
			3,
			0,
			0,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			2,
			3,
			2,
			3,
			2,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			2,
			2,
			2,
			0,
			2,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			2,
			2,
			2,
			2,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3,
			3
		};

		public static readonly HtmlEntityIndex[] EntityMap = new HtmlEntityIndex[]
		{
			HtmlEntityIndex.nbsp,
			HtmlEntityIndex.iexcl,
			HtmlEntityIndex.cent,
			HtmlEntityIndex.pound,
			HtmlEntityIndex.curren,
			HtmlEntityIndex.yen,
			HtmlEntityIndex.brvbar,
			HtmlEntityIndex.sect,
			HtmlEntityIndex.uml,
			HtmlEntityIndex.copy,
			HtmlEntityIndex.ordf,
			HtmlEntityIndex.laquo,
			HtmlEntityIndex.not,
			HtmlEntityIndex.shy,
			HtmlEntityIndex.reg,
			HtmlEntityIndex.macr,
			HtmlEntityIndex.deg,
			HtmlEntityIndex.plusmn,
			HtmlEntityIndex.sup2,
			HtmlEntityIndex.sup3,
			HtmlEntityIndex.acute,
			HtmlEntityIndex.micro,
			HtmlEntityIndex.para,
			HtmlEntityIndex.middot,
			HtmlEntityIndex.cedil,
			HtmlEntityIndex.sup1,
			HtmlEntityIndex.ordm,
			HtmlEntityIndex.raquo,
			HtmlEntityIndex.frac14,
			HtmlEntityIndex.frac12,
			HtmlEntityIndex.frac34,
			HtmlEntityIndex.iquest,
			HtmlEntityIndex.Agrave,
			HtmlEntityIndex.Aacute,
			HtmlEntityIndex.Acirc,
			HtmlEntityIndex.Atilde,
			HtmlEntityIndex.Auml,
			HtmlEntityIndex.Aring,
			HtmlEntityIndex.AElig,
			HtmlEntityIndex.Ccedil,
			HtmlEntityIndex.Egrave,
			HtmlEntityIndex.Eacute,
			HtmlEntityIndex.Ecirc,
			HtmlEntityIndex.Euml,
			HtmlEntityIndex.Igrave,
			HtmlEntityIndex.Iacute,
			HtmlEntityIndex.Icirc,
			HtmlEntityIndex.Iuml,
			HtmlEntityIndex.ETH,
			HtmlEntityIndex.Ntilde,
			HtmlEntityIndex.Ograve,
			HtmlEntityIndex.Oacute,
			HtmlEntityIndex.Ocirc,
			HtmlEntityIndex.Otilde,
			HtmlEntityIndex.Ouml,
			HtmlEntityIndex.times,
			HtmlEntityIndex.Oslash,
			HtmlEntityIndex.Ugrave,
			HtmlEntityIndex.Uacute,
			HtmlEntityIndex.Ucirc,
			HtmlEntityIndex.Uuml,
			HtmlEntityIndex.Yacute,
			HtmlEntityIndex.THORN,
			HtmlEntityIndex.szlig,
			HtmlEntityIndex.agrave,
			HtmlEntityIndex.aacute,
			HtmlEntityIndex.acirc,
			HtmlEntityIndex.atilde,
			HtmlEntityIndex.auml,
			HtmlEntityIndex.aring,
			HtmlEntityIndex.aelig,
			HtmlEntityIndex.ccedil,
			HtmlEntityIndex.egrave,
			HtmlEntityIndex.eacute,
			HtmlEntityIndex.ecirc,
			HtmlEntityIndex.euml,
			HtmlEntityIndex.igrave,
			HtmlEntityIndex.iacute,
			HtmlEntityIndex.icirc,
			HtmlEntityIndex.iuml,
			HtmlEntityIndex.eth,
			HtmlEntityIndex.ntilde,
			HtmlEntityIndex.ograve,
			HtmlEntityIndex.oacute,
			HtmlEntityIndex.ocirc,
			HtmlEntityIndex.otilde,
			HtmlEntityIndex.ouml,
			HtmlEntityIndex.divide,
			HtmlEntityIndex.oslash,
			HtmlEntityIndex.ugrave,
			HtmlEntityIndex.uacute,
			HtmlEntityIndex.ucirc,
			HtmlEntityIndex.uuml,
			HtmlEntityIndex.yacute,
			HtmlEntityIndex.thorn,
			HtmlEntityIndex.yuml
		};

		private static HtmlSupport.EnumerationDef[] directionEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("rtl", PropertyValue.True),
			new HtmlSupport.EnumerationDef("ltr", PropertyValue.False)
		};

		internal static HtmlSupport.EnumerationDef[] TextAlignmentEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("left", new PropertyValue(TextAlign.Left)),
			new HtmlSupport.EnumerationDef("center", new PropertyValue(TextAlign.Center)),
			new HtmlSupport.EnumerationDef("right", new PropertyValue(TextAlign.Right)),
			new HtmlSupport.EnumerationDef("justify", new PropertyValue(TextAlign.Justify))
		};

		internal static HtmlSupport.EnumerationDef[] HorizontalAlignmentEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("left", new PropertyValue(BlockHorizontalAlign.Left)),
			new HtmlSupport.EnumerationDef("center", new PropertyValue(BlockHorizontalAlign.Center)),
			new HtmlSupport.EnumerationDef("right", new PropertyValue(BlockHorizontalAlign.Right))
		};

		private static HtmlSupport.EnumerationDef[] verticalAlignmentEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("top", new PropertyValue(BlockVerticalAlign.Top)),
			new HtmlSupport.EnumerationDef("middle", new PropertyValue(BlockVerticalAlign.Middle)),
			new HtmlSupport.EnumerationDef("bottom", new PropertyValue(BlockVerticalAlign.Bottom)),
			new HtmlSupport.EnumerationDef("baseline", new PropertyValue(BlockVerticalAlign.Middle))
		};

		internal static HtmlSupport.EnumerationDef[] BlockAlignmentEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("top", new PropertyValue(BlockHorizontalAlign.Left)),
			new HtmlSupport.EnumerationDef("middle", new PropertyValue(BlockHorizontalAlign.Left)),
			new HtmlSupport.EnumerationDef("bottom", new PropertyValue(BlockHorizontalAlign.Left)),
			new HtmlSupport.EnumerationDef("left", new PropertyValue(BlockHorizontalAlign.Center)),
			new HtmlSupport.EnumerationDef("right", new PropertyValue(BlockHorizontalAlign.Right))
		};

		internal static HtmlSupport.EnumerationDef[] BorderStyleEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("none", new PropertyValue(BorderStyle.None)),
			new HtmlSupport.EnumerationDef("hidden", new PropertyValue(BorderStyle.Hidden)),
			new HtmlSupport.EnumerationDef("dotted", new PropertyValue(BorderStyle.Dotted)),
			new HtmlSupport.EnumerationDef("dashed", new PropertyValue(BorderStyle.Dashed)),
			new HtmlSupport.EnumerationDef("solid", new PropertyValue(BorderStyle.Solid)),
			new HtmlSupport.EnumerationDef("double", new PropertyValue(BorderStyle.Double)),
			new HtmlSupport.EnumerationDef("groove", new PropertyValue(BorderStyle.Groove)),
			new HtmlSupport.EnumerationDef("ridge", new PropertyValue(BorderStyle.Ridge)),
			new HtmlSupport.EnumerationDef("inset", new PropertyValue(BorderStyle.Inset)),
			new HtmlSupport.EnumerationDef("outset", new PropertyValue(BorderStyle.Outset))
		};

		private static HtmlSupport.EnumerationDef[] targetEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("_self", new PropertyValue(LinkTarget.Self)),
			new HtmlSupport.EnumerationDef("_top", new PropertyValue(LinkTarget.Top)),
			new HtmlSupport.EnumerationDef("_blank", new PropertyValue(LinkTarget.Blank)),
			new HtmlSupport.EnumerationDef("_parent", new PropertyValue(LinkTarget.Parent))
		};

		private static HtmlSupport.EnumerationDef[] fontWeightEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("normal", PropertyValue.False),
			new HtmlSupport.EnumerationDef("bold", PropertyValue.True),
			new HtmlSupport.EnumerationDef("lighter", PropertyValue.False),
			new HtmlSupport.EnumerationDef("bolder", PropertyValue.True),
			new HtmlSupport.EnumerationDef("100", PropertyValue.False),
			new HtmlSupport.EnumerationDef("200", PropertyValue.False),
			new HtmlSupport.EnumerationDef("300", PropertyValue.False),
			new HtmlSupport.EnumerationDef("400", PropertyValue.False),
			new HtmlSupport.EnumerationDef("500", PropertyValue.False),
			new HtmlSupport.EnumerationDef("600", PropertyValue.True),
			new HtmlSupport.EnumerationDef("700", PropertyValue.True),
			new HtmlSupport.EnumerationDef("800", PropertyValue.True),
			new HtmlSupport.EnumerationDef("900", PropertyValue.True)
		};

		private static HtmlSupport.EnumerationDef[] fontSizeEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("xx-small", new PropertyValue(LengthUnits.HtmlFontUnits, 1)),
			new HtmlSupport.EnumerationDef("x-small", new PropertyValue(LengthUnits.HtmlFontUnits, 2)),
			new HtmlSupport.EnumerationDef("small", new PropertyValue(LengthUnits.HtmlFontUnits, 2)),
			new HtmlSupport.EnumerationDef("medium", new PropertyValue(LengthUnits.HtmlFontUnits, 3)),
			new HtmlSupport.EnumerationDef("large", new PropertyValue(LengthUnits.HtmlFontUnits, 4)),
			new HtmlSupport.EnumerationDef("x-large", new PropertyValue(LengthUnits.HtmlFontUnits, 5)),
			new HtmlSupport.EnumerationDef("xx-large", new PropertyValue(LengthUnits.HtmlFontUnits, 6))
		};

		private static HtmlSupport.EnumerationDef[] fontStyleEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("normal", PropertyValue.False),
			new HtmlSupport.EnumerationDef("italic", PropertyValue.True),
			new HtmlSupport.EnumerationDef("oblique", PropertyValue.True)
		};

		private static HtmlSupport.EnumerationDef[] fontVariantEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("normal", PropertyValue.False),
			new HtmlSupport.EnumerationDef("small-caps", PropertyValue.True)
		};

		private static HtmlSupport.EnumerationDef[] tableLayoutEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("auto", PropertyValue.False),
			new HtmlSupport.EnumerationDef("fixed", PropertyValue.True)
		};

		private static HtmlSupport.EnumerationDef[] borderCollapseEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("collapse", PropertyValue.True),
			new HtmlSupport.EnumerationDef("separate", PropertyValue.False)
		};

		private static HtmlSupport.EnumerationDef[] emptyCellsEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("show", PropertyValue.True),
			new HtmlSupport.EnumerationDef("hide", PropertyValue.False)
		};

		private static HtmlSupport.EnumerationDef[] captionSideEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("bottom", PropertyValue.True),
			new HtmlSupport.EnumerationDef("top", PropertyValue.False)
		};

		private static HtmlSupport.EnumerationDef[] borderWidthEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("thin", new PropertyValue(LengthUnits.Pixels, 2)),
			new HtmlSupport.EnumerationDef("medium", new PropertyValue(LengthUnits.Pixels, 4)),
			new HtmlSupport.EnumerationDef("thick", new PropertyValue(LengthUnits.Pixels, 6))
		};

		private static HtmlSupport.EnumerationDef[] tableFrameEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("void", new PropertyValue(HtmlSupport.TableFrame.Void)),
			new HtmlSupport.EnumerationDef("above", new PropertyValue(HtmlSupport.TableFrame.Above)),
			new HtmlSupport.EnumerationDef("below", new PropertyValue(HtmlSupport.TableFrame.Below)),
			new HtmlSupport.EnumerationDef("border", new PropertyValue(HtmlSupport.TableFrame.Border)),
			new HtmlSupport.EnumerationDef("box", new PropertyValue(HtmlSupport.TableFrame.Box)),
			new HtmlSupport.EnumerationDef("hsides", new PropertyValue(HtmlSupport.TableFrame.Hsides)),
			new HtmlSupport.EnumerationDef("lhs", new PropertyValue(HtmlSupport.TableFrame.Lhs)),
			new HtmlSupport.EnumerationDef("rhs", new PropertyValue(HtmlSupport.TableFrame.Rhs)),
			new HtmlSupport.EnumerationDef("vsides", new PropertyValue(HtmlSupport.TableFrame.Vsides))
		};

		private static HtmlSupport.EnumerationDef[] tableRulesEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("none", new PropertyValue(HtmlSupport.TableRules.None)),
			new HtmlSupport.EnumerationDef("groups", new PropertyValue(HtmlSupport.TableRules.Groups)),
			new HtmlSupport.EnumerationDef("rows", new PropertyValue(HtmlSupport.TableRules.Rows)),
			new HtmlSupport.EnumerationDef("cells", new PropertyValue(HtmlSupport.TableRules.Cells)),
			new HtmlSupport.EnumerationDef("all", new PropertyValue(HtmlSupport.TableRules.All))
		};

		private static HtmlSupport.EnumerationDef[] unicodeBiDiEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("normal", new PropertyValue(UnicodeBiDi.Normal)),
			new HtmlSupport.EnumerationDef("embed", new PropertyValue(UnicodeBiDi.Embed)),
			new HtmlSupport.EnumerationDef("bidi-override", new PropertyValue(UnicodeBiDi.Override))
		};

		private static HtmlSupport.EnumerationDef[] displayEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("none", new PropertyValue(Display.None)),
			new HtmlSupport.EnumerationDef("inline", new PropertyValue(Display.Inline)),
			new HtmlSupport.EnumerationDef("block", new PropertyValue(Display.Block)),
			new HtmlSupport.EnumerationDef("list-item", new PropertyValue(Display.ListItem)),
			new HtmlSupport.EnumerationDef("run-in", new PropertyValue(Display.RunIn)),
			new HtmlSupport.EnumerationDef("inline-block", new PropertyValue(Display.InlineBlock)),
			new HtmlSupport.EnumerationDef("table", new PropertyValue(Display.Table)),
			new HtmlSupport.EnumerationDef("inline-table", new PropertyValue(Display.InlineTable)),
			new HtmlSupport.EnumerationDef("table-row-group", new PropertyValue(Display.TableRowGroup)),
			new HtmlSupport.EnumerationDef("table-header-group", new PropertyValue(Display.TableHeaderGroup)),
			new HtmlSupport.EnumerationDef("table-footer-group", new PropertyValue(Display.TableFooterGroup)),
			new HtmlSupport.EnumerationDef("table-row", new PropertyValue(Display.TableRow)),
			new HtmlSupport.EnumerationDef("table-column-group", new PropertyValue(Display.TableColumnGroup)),
			new HtmlSupport.EnumerationDef("table-column", new PropertyValue(Display.TableColumn)),
			new HtmlSupport.EnumerationDef("table-cell", new PropertyValue(Display.TableCell)),
			new HtmlSupport.EnumerationDef("table-caption", new PropertyValue(Display.TableCaption))
		};

		private static HtmlSupport.EnumerationDef[] visibilityEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("visible", PropertyValue.True),
			new HtmlSupport.EnumerationDef("hidden", PropertyValue.False),
			new HtmlSupport.EnumerationDef("collapse", PropertyValue.False)
		};

		private static HtmlSupport.EnumerationDef[] areaShapeEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("default", new PropertyValue(AreaShape.Default)),
			new HtmlSupport.EnumerationDef("rect", new PropertyValue(AreaShape.Rectangle)),
			new HtmlSupport.EnumerationDef("rectangle", new PropertyValue(AreaShape.Rectangle)),
			new HtmlSupport.EnumerationDef("circle", new PropertyValue(AreaShape.Circle)),
			new HtmlSupport.EnumerationDef("poly", new PropertyValue(AreaShape.Polygon)),
			new HtmlSupport.EnumerationDef("polygon", new PropertyValue(AreaShape.Polygon))
		};

		private static readonly HtmlSupport.EnumerationDef[] colorNames = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("activeborder", new PropertyValue(HtmlSupport.SystemColors.ActiveBorder)),
			new HtmlSupport.EnumerationDef("activecaption", new PropertyValue(HtmlSupport.SystemColors.ActiveCaption)),
			new HtmlSupport.EnumerationDef("aliceblue", new PropertyValue(new RGBT(15792383U))),
			new HtmlSupport.EnumerationDef("antiquewhite", new PropertyValue(new RGBT(16444375U))),
			new HtmlSupport.EnumerationDef("appworkspace", new PropertyValue(HtmlSupport.SystemColors.AppWorkspace)),
			new HtmlSupport.EnumerationDef("aqua", new PropertyValue(new RGBT(65535U))),
			new HtmlSupport.EnumerationDef("aquamarine", new PropertyValue(new RGBT(8388564U))),
			new HtmlSupport.EnumerationDef("azure", new PropertyValue(new RGBT(15794175U))),
			new HtmlSupport.EnumerationDef("background", new PropertyValue(HtmlSupport.SystemColors.Background)),
			new HtmlSupport.EnumerationDef("beige", new PropertyValue(new RGBT(16119260U))),
			new HtmlSupport.EnumerationDef("bisque", new PropertyValue(new RGBT(16770244U))),
			new HtmlSupport.EnumerationDef("black", new PropertyValue(new RGBT(0U))),
			new HtmlSupport.EnumerationDef("blanchedalmond", new PropertyValue(new RGBT(16772045U))),
			new HtmlSupport.EnumerationDef("blue", new PropertyValue(new RGBT(255U))),
			new HtmlSupport.EnumerationDef("blueviolet", new PropertyValue(new RGBT(9055202U))),
			new HtmlSupport.EnumerationDef("brown", new PropertyValue(new RGBT(10824234U))),
			new HtmlSupport.EnumerationDef("burlywood", new PropertyValue(new RGBT(14596231U))),
			new HtmlSupport.EnumerationDef("buttonface", new PropertyValue(HtmlSupport.SystemColors.ButtonFace)),
			new HtmlSupport.EnumerationDef("buttonhighlight", new PropertyValue(HtmlSupport.SystemColors.ButtonHighlight)),
			new HtmlSupport.EnumerationDef("buttonshadow", new PropertyValue(HtmlSupport.SystemColors.ButtonShadow)),
			new HtmlSupport.EnumerationDef("buttontext", new PropertyValue(HtmlSupport.SystemColors.ButtonText)),
			new HtmlSupport.EnumerationDef("cadetblue", new PropertyValue(new RGBT(6266528U))),
			new HtmlSupport.EnumerationDef("captiontext", new PropertyValue(HtmlSupport.SystemColors.CaptionText)),
			new HtmlSupport.EnumerationDef("chartreuse", new PropertyValue(new RGBT(8388352U))),
			new HtmlSupport.EnumerationDef("chocolate", new PropertyValue(new RGBT(13789470U))),
			new HtmlSupport.EnumerationDef("coral", new PropertyValue(new RGBT(16744272U))),
			new HtmlSupport.EnumerationDef("cornflowerblue", new PropertyValue(new RGBT(6591981U))),
			new HtmlSupport.EnumerationDef("cornsilk", new PropertyValue(new RGBT(16775388U))),
			new HtmlSupport.EnumerationDef("crimson", new PropertyValue(new RGBT(14423100U))),
			new HtmlSupport.EnumerationDef("cyan", new PropertyValue(new RGBT(65535U))),
			new HtmlSupport.EnumerationDef("darkblue", new PropertyValue(new RGBT(139U))),
			new HtmlSupport.EnumerationDef("darkcyan", new PropertyValue(new RGBT(35723U))),
			new HtmlSupport.EnumerationDef("darkgoldenrod", new PropertyValue(new RGBT(12092939U))),
			new HtmlSupport.EnumerationDef("darkgray", new PropertyValue(new RGBT(11119017U))),
			new HtmlSupport.EnumerationDef("darkgreen", new PropertyValue(new RGBT(25600U))),
			new HtmlSupport.EnumerationDef("darkkhaki", new PropertyValue(new RGBT(12433259U))),
			new HtmlSupport.EnumerationDef("darkmagenta", new PropertyValue(new RGBT(9109643U))),
			new HtmlSupport.EnumerationDef("darkolivegreen", new PropertyValue(new RGBT(5597999U))),
			new HtmlSupport.EnumerationDef("darkorange", new PropertyValue(new RGBT(16747520U))),
			new HtmlSupport.EnumerationDef("darkorchid", new PropertyValue(new RGBT(10040012U))),
			new HtmlSupport.EnumerationDef("darkred", new PropertyValue(new RGBT(9109504U))),
			new HtmlSupport.EnumerationDef("darksalmon", new PropertyValue(new RGBT(15308410U))),
			new HtmlSupport.EnumerationDef("darkseagreen", new PropertyValue(new RGBT(9419919U))),
			new HtmlSupport.EnumerationDef("darkslateblue", new PropertyValue(new RGBT(4734347U))),
			new HtmlSupport.EnumerationDef("darkslategray", new PropertyValue(new RGBT(3100495U))),
			new HtmlSupport.EnumerationDef("darkturquoise", new PropertyValue(new RGBT(52945U))),
			new HtmlSupport.EnumerationDef("darkviolet", new PropertyValue(new RGBT(9699539U))),
			new HtmlSupport.EnumerationDef("deeppink", new PropertyValue(new RGBT(16716947U))),
			new HtmlSupport.EnumerationDef("deepskyblue", new PropertyValue(new RGBT(49151U))),
			new HtmlSupport.EnumerationDef("dimgray", new PropertyValue(new RGBT(6908265U))),
			new HtmlSupport.EnumerationDef("dodgerblue", new PropertyValue(new RGBT(2003199U))),
			new HtmlSupport.EnumerationDef("firebrick", new PropertyValue(new RGBT(11674146U))),
			new HtmlSupport.EnumerationDef("floralwhite", new PropertyValue(new RGBT(16775920U))),
			new HtmlSupport.EnumerationDef("forestgreen", new PropertyValue(new RGBT(2263842U))),
			new HtmlSupport.EnumerationDef("fuchsia", new PropertyValue(new RGBT(16711935U))),
			new HtmlSupport.EnumerationDef("gainsboro", new PropertyValue(new RGBT(14474460U))),
			new HtmlSupport.EnumerationDef("ghostwhite", new PropertyValue(new RGBT(16316671U))),
			new HtmlSupport.EnumerationDef("gold", new PropertyValue(new RGBT(16766720U))),
			new HtmlSupport.EnumerationDef("goldenrod", new PropertyValue(new RGBT(14329120U))),
			new HtmlSupport.EnumerationDef("gray", new PropertyValue(new RGBT(8421504U))),
			new HtmlSupport.EnumerationDef("graytext", new PropertyValue(HtmlSupport.SystemColors.GrayText)),
			new HtmlSupport.EnumerationDef("green", new PropertyValue(new RGBT(32768U))),
			new HtmlSupport.EnumerationDef("greenyellow", new PropertyValue(new RGBT(11403055U))),
			new HtmlSupport.EnumerationDef("highlight", new PropertyValue(HtmlSupport.SystemColors.Highlight)),
			new HtmlSupport.EnumerationDef("highlighttext", new PropertyValue(HtmlSupport.SystemColors.HighlightText)),
			new HtmlSupport.EnumerationDef("honeydew", new PropertyValue(new RGBT(15794160U))),
			new HtmlSupport.EnumerationDef("hotpink", new PropertyValue(new RGBT(16738740U))),
			new HtmlSupport.EnumerationDef("inactiveborder", new PropertyValue(HtmlSupport.SystemColors.InactiveBorder)),
			new HtmlSupport.EnumerationDef("inactivecaption", new PropertyValue(HtmlSupport.SystemColors.InactiveCaption)),
			new HtmlSupport.EnumerationDef("inactivecaptiontext", new PropertyValue(HtmlSupport.SystemColors.InactiveCaptionText)),
			new HtmlSupport.EnumerationDef("indianred", new PropertyValue(new RGBT(13458524U))),
			new HtmlSupport.EnumerationDef("indigo", new PropertyValue(new RGBT(4915330U))),
			new HtmlSupport.EnumerationDef("infobackground", new PropertyValue(HtmlSupport.SystemColors.InfoBackground)),
			new HtmlSupport.EnumerationDef("infotext", new PropertyValue(HtmlSupport.SystemColors.InfoText)),
			new HtmlSupport.EnumerationDef("ivory", new PropertyValue(new RGBT(16777200U))),
			new HtmlSupport.EnumerationDef("khaki", new PropertyValue(new RGBT(15787660U))),
			new HtmlSupport.EnumerationDef("lavender", new PropertyValue(new RGBT(15132410U))),
			new HtmlSupport.EnumerationDef("lavenderblush", new PropertyValue(new RGBT(16773365U))),
			new HtmlSupport.EnumerationDef("lawngreen", new PropertyValue(new RGBT(8190976U))),
			new HtmlSupport.EnumerationDef("lemonchiffon", new PropertyValue(new RGBT(16775885U))),
			new HtmlSupport.EnumerationDef("lightblue", new PropertyValue(new RGBT(11393254U))),
			new HtmlSupport.EnumerationDef("lightcoral", new PropertyValue(new RGBT(15761536U))),
			new HtmlSupport.EnumerationDef("lightcyan", new PropertyValue(new RGBT(14745599U))),
			new HtmlSupport.EnumerationDef("lightgoldenrodyellow", new PropertyValue(new RGBT(16448210U))),
			new HtmlSupport.EnumerationDef("lightgreen", new PropertyValue(new RGBT(9498256U))),
			new HtmlSupport.EnumerationDef("lightgrey", new PropertyValue(new RGBT(13882323U))),
			new HtmlSupport.EnumerationDef("lightpink", new PropertyValue(new RGBT(16758465U))),
			new HtmlSupport.EnumerationDef("lightsalmon", new PropertyValue(new RGBT(16752762U))),
			new HtmlSupport.EnumerationDef("lightseagreen", new PropertyValue(new RGBT(2142890U))),
			new HtmlSupport.EnumerationDef("lightskyblue", new PropertyValue(new RGBT(8900346U))),
			new HtmlSupport.EnumerationDef("lightslategray", new PropertyValue(new RGBT(7833753U))),
			new HtmlSupport.EnumerationDef("lightsteelblue", new PropertyValue(new RGBT(11584734U))),
			new HtmlSupport.EnumerationDef("lightyellow", new PropertyValue(new RGBT(16777184U))),
			new HtmlSupport.EnumerationDef("lime", new PropertyValue(new RGBT(65280U))),
			new HtmlSupport.EnumerationDef("limegreen", new PropertyValue(new RGBT(3329330U))),
			new HtmlSupport.EnumerationDef("linen", new PropertyValue(new RGBT(16445670U))),
			new HtmlSupport.EnumerationDef("magenta", new PropertyValue(new RGBT(16711935U))),
			new HtmlSupport.EnumerationDef("maroon", new PropertyValue(new RGBT(8388608U))),
			new HtmlSupport.EnumerationDef("mediumaquamarine", new PropertyValue(new RGBT(6737322U))),
			new HtmlSupport.EnumerationDef("mediumblue", new PropertyValue(new RGBT(205U))),
			new HtmlSupport.EnumerationDef("mediumorchid", new PropertyValue(new RGBT(12211667U))),
			new HtmlSupport.EnumerationDef("mediumpurple", new PropertyValue(new RGBT(9662683U))),
			new HtmlSupport.EnumerationDef("mediumseagreen", new PropertyValue(new RGBT(3978097U))),
			new HtmlSupport.EnumerationDef("mediumslateblue", new PropertyValue(new RGBT(8087790U))),
			new HtmlSupport.EnumerationDef("mediumspringgreen", new PropertyValue(new RGBT(64154U))),
			new HtmlSupport.EnumerationDef("mediumturquoise", new PropertyValue(new RGBT(4772300U))),
			new HtmlSupport.EnumerationDef("mediumvioletred", new PropertyValue(new RGBT(13047173U))),
			new HtmlSupport.EnumerationDef("menu", new PropertyValue(HtmlSupport.SystemColors.Menu)),
			new HtmlSupport.EnumerationDef("menutext", new PropertyValue(HtmlSupport.SystemColors.MenuText)),
			new HtmlSupport.EnumerationDef("midnightblue", new PropertyValue(new RGBT(1644912U))),
			new HtmlSupport.EnumerationDef("mintcream", new PropertyValue(new RGBT(16121850U))),
			new HtmlSupport.EnumerationDef("mistyrose", new PropertyValue(new RGBT(16770273U))),
			new HtmlSupport.EnumerationDef("moccasin", new PropertyValue(new RGBT(16770229U))),
			new HtmlSupport.EnumerationDef("navajowhite", new PropertyValue(new RGBT(16768685U))),
			new HtmlSupport.EnumerationDef("navy", new PropertyValue(new RGBT(128U))),
			new HtmlSupport.EnumerationDef("oldlace", new PropertyValue(new RGBT(16643558U))),
			new HtmlSupport.EnumerationDef("olive", new PropertyValue(new RGBT(8421376U))),
			new HtmlSupport.EnumerationDef("olivedrab", new PropertyValue(new RGBT(7048739U))),
			new HtmlSupport.EnumerationDef("orange", new PropertyValue(new RGBT(16753920U))),
			new HtmlSupport.EnumerationDef("orangered", new PropertyValue(new RGBT(16729344U))),
			new HtmlSupport.EnumerationDef("orchid", new PropertyValue(new RGBT(14315734U))),
			new HtmlSupport.EnumerationDef("palegoldenrod", new PropertyValue(new RGBT(15657130U))),
			new HtmlSupport.EnumerationDef("palegreen", new PropertyValue(new RGBT(10025880U))),
			new HtmlSupport.EnumerationDef("paleturquoise", new PropertyValue(new RGBT(11529966U))),
			new HtmlSupport.EnumerationDef("palevioletred", new PropertyValue(new RGBT(14381203U))),
			new HtmlSupport.EnumerationDef("papayawhip", new PropertyValue(new RGBT(16773077U))),
			new HtmlSupport.EnumerationDef("peachpuff", new PropertyValue(new RGBT(16767673U))),
			new HtmlSupport.EnumerationDef("peru", new PropertyValue(new RGBT(13468991U))),
			new HtmlSupport.EnumerationDef("pink", new PropertyValue(new RGBT(16761035U))),
			new HtmlSupport.EnumerationDef("plum", new PropertyValue(new RGBT(14524637U))),
			new HtmlSupport.EnumerationDef("powderblue", new PropertyValue(new RGBT(11591910U))),
			new HtmlSupport.EnumerationDef("purple", new PropertyValue(new RGBT(8388736U))),
			new HtmlSupport.EnumerationDef("red", new PropertyValue(new RGBT(16711680U))),
			new HtmlSupport.EnumerationDef("rosybrown", new PropertyValue(new RGBT(12357519U))),
			new HtmlSupport.EnumerationDef("royalblue", new PropertyValue(new RGBT(4286945U))),
			new HtmlSupport.EnumerationDef("saddlebrown", new PropertyValue(new RGBT(9127187U))),
			new HtmlSupport.EnumerationDef("salmon", new PropertyValue(new RGBT(16416882U))),
			new HtmlSupport.EnumerationDef("sandybrown", new PropertyValue(new RGBT(16032864U))),
			new HtmlSupport.EnumerationDef("scrollbar", new PropertyValue(HtmlSupport.SystemColors.Scrollbar)),
			new HtmlSupport.EnumerationDef("seagreen", new PropertyValue(new RGBT(3050327U))),
			new HtmlSupport.EnumerationDef("seashell", new PropertyValue(new RGBT(16774638U))),
			new HtmlSupport.EnumerationDef("sienna", new PropertyValue(new RGBT(10506797U))),
			new HtmlSupport.EnumerationDef("silver", new PropertyValue(new RGBT(12632256U))),
			new HtmlSupport.EnumerationDef("skyblue", new PropertyValue(new RGBT(8900331U))),
			new HtmlSupport.EnumerationDef("slateblue", new PropertyValue(new RGBT(6970061U))),
			new HtmlSupport.EnumerationDef("slategray", new PropertyValue(new RGBT(7372944U))),
			new HtmlSupport.EnumerationDef("snow", new PropertyValue(new RGBT(16775930U))),
			new HtmlSupport.EnumerationDef("springgreen", new PropertyValue(new RGBT(65407U))),
			new HtmlSupport.EnumerationDef("steelblue", new PropertyValue(new RGBT(4620980U))),
			new HtmlSupport.EnumerationDef("tan", new PropertyValue(new RGBT(13808780U))),
			new HtmlSupport.EnumerationDef("teal", new PropertyValue(new RGBT(32896U))),
			new HtmlSupport.EnumerationDef("thistle", new PropertyValue(new RGBT(14204888U))),
			new HtmlSupport.EnumerationDef("threeddarkshadow", new PropertyValue(HtmlSupport.SystemColors.ThreeDDarkShadow)),
			new HtmlSupport.EnumerationDef("threedface", new PropertyValue(HtmlSupport.SystemColors.ButtonFace)),
			new HtmlSupport.EnumerationDef("threedhighlight", new PropertyValue(HtmlSupport.SystemColors.ButtonHighlight)),
			new HtmlSupport.EnumerationDef("threedlightshadow", new PropertyValue(HtmlSupport.SystemColors.ThreeDLightShadow)),
			new HtmlSupport.EnumerationDef("threedshadow", new PropertyValue(HtmlSupport.SystemColors.ButtonShadow)),
			new HtmlSupport.EnumerationDef("tomato", new PropertyValue(new RGBT(16737095U))),
			new HtmlSupport.EnumerationDef("transparent", new PropertyValue(new RGBT(0U, 0U, 0U, 7U))),
			new HtmlSupport.EnumerationDef("turquoise", new PropertyValue(new RGBT(4251856U))),
			new HtmlSupport.EnumerationDef("violet", new PropertyValue(new RGBT(15631086U))),
			new HtmlSupport.EnumerationDef("wheat", new PropertyValue(new RGBT(16113331U))),
			new HtmlSupport.EnumerationDef("white", new PropertyValue(new RGBT(16777215U))),
			new HtmlSupport.EnumerationDef("whitesmoke", new PropertyValue(new RGBT(16119285U))),
			new HtmlSupport.EnumerationDef("window", new PropertyValue(HtmlSupport.SystemColors.Window)),
			new HtmlSupport.EnumerationDef("windowframe", new PropertyValue(HtmlSupport.SystemColors.WindowFrame)),
			new HtmlSupport.EnumerationDef("windowtext", new PropertyValue(HtmlSupport.SystemColors.WindowText)),
			new HtmlSupport.EnumerationDef("yellow", new PropertyValue(new RGBT(16776960U))),
			new HtmlSupport.EnumerationDef("yellowgreen", new PropertyValue(new RGBT(10145074U)))
		};

		private static Dictionary<PropertyValue, string> colorToNameDictionary = HtmlSupport.BuildColorToNameDictionary();

		private static HtmlSupport.EnumerationDef[] cssTextDecorationEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("underline", new PropertyValue(HtmlSupport.TextDecoration.Underline)),
			new HtmlSupport.EnumerationDef("overline", new PropertyValue(HtmlSupport.TextDecoration.Overline)),
			new HtmlSupport.EnumerationDef("line-through", new PropertyValue(HtmlSupport.TextDecoration.LineThrough)),
			new HtmlSupport.EnumerationDef("blink", new PropertyValue(HtmlSupport.TextDecoration.Blink)),
			new HtmlSupport.EnumerationDef("none", new PropertyValue(HtmlSupport.TextDecoration.None))
		};

		private static HtmlSupport.EnumerationDef[] cssTextTransformEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("capitalize", new PropertyValue(HtmlSupport.TextTransform.Capitalize)),
			new HtmlSupport.EnumerationDef("uppercase", new PropertyValue(HtmlSupport.TextTransform.Uppercase)),
			new HtmlSupport.EnumerationDef("lowercase", new PropertyValue(HtmlSupport.TextTransform.Lowercase)),
			new HtmlSupport.EnumerationDef("none", new PropertyValue(HtmlSupport.TextTransform.None))
		};

		private static HtmlSupport.EnumerationDef[] cssVerticalAlignmentEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("baseline", new PropertyValue(Align.BaseLine)),
			new HtmlSupport.EnumerationDef("sub", new PropertyValue(Align.Sub)),
			new HtmlSupport.EnumerationDef("super", new PropertyValue(Align.Super)),
			new HtmlSupport.EnumerationDef("top", new PropertyValue(Align.Top)),
			new HtmlSupport.EnumerationDef("text-top", new PropertyValue(Align.TextTop)),
			new HtmlSupport.EnumerationDef("middle", new PropertyValue(Align.Middle)),
			new HtmlSupport.EnumerationDef("bottom", new PropertyValue(Align.Bottom)),
			new HtmlSupport.EnumerationDef("text-bottom", new PropertyValue(Align.TextBottom))
		};

		private static HtmlSupport.EnumerationDef[] cssWhiteSpaceEnumeration = new HtmlSupport.EnumerationDef[]
		{
			new HtmlSupport.EnumerationDef("normal", new PropertyValue(HtmlSupport.CssWhiteSpace.Normal)),
			new HtmlSupport.EnumerationDef("pre", new PropertyValue(HtmlSupport.CssWhiteSpace.Pre)),
			new HtmlSupport.EnumerationDef("nowrap", new PropertyValue(HtmlSupport.CssWhiteSpace.Nowrap)),
			new HtmlSupport.EnumerationDef("pre-wrap", new PropertyValue(HtmlSupport.CssWhiteSpace.PreWrap)),
			new HtmlSupport.EnumerationDef("pre-line", new PropertyValue(HtmlSupport.CssWhiteSpace.PreLine))
		};

		[Flags]
		public enum NumberParseFlags
		{
			Integer = 1,
			Float = 2,
			AbsoluteLength = 4,
			EmExLength = 8,
			Percentage = 16,
			Multiple = 32,
			HtmlFontUnits = 64,
			NonNegative = 8192,
			StyleSheetProperty = 16384,
			Strict = 32768,
			Length = 28,
			NonNegativeLength = 8220,
			FontSize = 8284
		}

		public struct EnumerationDef
		{
			public EnumerationDef(string name, PropertyValue value)
			{
				this.Name = name;
				this.Value = value;
			}

			public string Name;

			public PropertyValue Value;
		}

		internal enum TableFrame
		{
			Void,
			Above,
			Below,
			Border,
			Box,
			Hsides,
			Lhs,
			Rhs,
			Vsides
		}

		internal enum TableRules
		{
			None,
			Groups,
			Rows,
			Cells,
			All
		}

		internal enum SystemColors
		{
			ActiveBorder = 10,
			ActiveCaption = 2,
			AppWorkspace = 12,
			Background = 1,
			ButtonFace = 15,
			ButtonHighlight = 20,
			ButtonShadow = 16,
			ButtonText = 18,
			CaptionText = 9,
			GrayText = 17,
			Highlight = 13,
			HighlightText,
			InactiveBorder = 11,
			InactiveCaption = 3,
			InactiveCaptionText = 19,
			InfoBackground = 24,
			InfoText = 23,
			Menu = 4,
			MenuText = 7,
			Scrollbar = 0,
			ThreeDDarkShadow = 21,
			ThreeDFace = 15,
			ThreeDHighlight = 20,
			ThreeDLightShadow = 22,
			ThreeDShadow = 16,
			Window = 5,
			WindowFrame,
			WindowText = 8
		}

		private enum TextDecoration
		{
			None,
			Underline,
			Overline,
			LineThrough,
			Blink
		}

		private enum TextTransform
		{
			Capitalize,
			Uppercase,
			Lowercase,
			None
		}

		private enum CssWhiteSpace
		{
			Normal,
			Pre,
			Nowrap,
			PreWrap,
			PreLine
		}
	}
}
