using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct ByteQuantifiedSize : IComparable, IComparable<ByteQuantifiedSize>, IFormattable
	{
		public ByteQuantifiedSize(ulong byteValue)
		{
			this.bytes = byteValue;
			this.unit = ByteQuantifiedSize.Quantifier.None;
			this.canonicalForm = null;
		}

		private ByteQuantifiedSize(ulong byteValue, ByteQuantifiedSize.Quantifier desiredUnitToDisplay)
		{
			this.bytes = byteValue;
			this.unit = desiredUnitToDisplay;
			this.canonicalForm = null;
		}

		public static ByteQuantifiedSize FromBytes(ulong bytesValue)
		{
			return new ByteQuantifiedSize(bytesValue, ByteQuantifiedSize.Quantifier.None);
		}

		public static ByteQuantifiedSize FromKB(ulong kbValue)
		{
			if (18014398509481983UL < kbValue)
			{
				throw new OverflowException(DataStrings.ExceptionValueOverflow(ByteQuantifiedSize.MinValue.ToString("K"), ByteQuantifiedSize.MaxValue.ToString("K"), kbValue.ToString()));
			}
			ulong byteValue = kbValue << 10;
			return new ByteQuantifiedSize(byteValue, ByteQuantifiedSize.Quantifier.KB);
		}

		public static ByteQuantifiedSize FromMB(ulong mbValue)
		{
			if (17592186044415UL < mbValue)
			{
				throw new OverflowException(DataStrings.ExceptionValueOverflow(ByteQuantifiedSize.MinValue.ToString("M"), ByteQuantifiedSize.MaxValue.ToString("M"), mbValue.ToString()));
			}
			ulong byteValue = mbValue << 20;
			return new ByteQuantifiedSize(byteValue, ByteQuantifiedSize.Quantifier.MB);
		}

		public static ByteQuantifiedSize FromGB(ulong gbValue)
		{
			if (17179869183UL < gbValue)
			{
				throw new OverflowException(DataStrings.ExceptionValueOverflow(ByteQuantifiedSize.MinValue.ToString("G"), ByteQuantifiedSize.MaxValue.ToString("G"), gbValue.ToString()));
			}
			ulong byteValue = gbValue << 30;
			return new ByteQuantifiedSize(byteValue, ByteQuantifiedSize.Quantifier.GB);
		}

		public static ByteQuantifiedSize FromTB(ulong tbValue)
		{
			if (16777215UL < tbValue)
			{
				throw new OverflowException(DataStrings.ExceptionValueOverflow(ByteQuantifiedSize.MinValue.ToString("T"), ByteQuantifiedSize.MaxValue.ToString("T"), tbValue.ToString()));
			}
			ulong byteValue = tbValue << 40;
			return new ByteQuantifiedSize(byteValue, ByteQuantifiedSize.Quantifier.TB);
		}

		public ulong ToBytes()
		{
			return this.bytes;
		}

		public ulong ToKB()
		{
			return this.bytes >> 10;
		}

		public ulong ToMB()
		{
			return this.bytes >> 20;
		}

		public ulong ToGB()
		{
			return this.bytes >> 30;
		}

		public ulong ToTB()
		{
			return this.bytes >> 40;
		}

		private string ToLargestAppropriateUnitFormatString(bool includeFullBytes = true)
		{
			if (includeFullBytes)
			{
				if (this.ToTB() > 0UL)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:G4} {1} ({2:N0} bytes)", new object[]
					{
						this.ToBytes() / 1099511627776.0,
						"TB",
						this.ToBytes()
					});
				}
				if (this.ToGB() > 0UL)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:G4} {1} ({2:N0} bytes)", new object[]
					{
						this.ToBytes() / 1073741824.0,
						"GB",
						this.ToBytes()
					});
				}
				if (this.ToMB() > 0UL)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:G4} {1} ({2:N0} bytes)", new object[]
					{
						this.ToBytes() / 1048576.0,
						"MB",
						this.ToBytes()
					});
				}
				if (this.ToKB() > 0UL)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:G4} {1} ({2:N0} bytes)", new object[]
					{
						this.ToBytes() / 1024.0,
						"KB",
						this.ToBytes()
					});
				}
				return string.Format(CultureInfo.InvariantCulture, "{0:G4} {1} ({2:N0} bytes)", new object[]
				{
					this.ToBytes(),
					"B",
					this.ToBytes()
				});
			}
			else
			{
				if (this.ToTB() > 0UL)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:G4} {1}", new object[]
					{
						this.ToBytes() / 1099511627776.0,
						"TB"
					});
				}
				if (this.ToGB() > 0UL)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:G4} {1}", new object[]
					{
						this.ToBytes() / 1073741824.0,
						"GB"
					});
				}
				if (this.ToMB() > 0UL)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:G4} {1}", new object[]
					{
						this.ToBytes() / 1048576.0,
						"MB"
					});
				}
				if (this.ToKB() > 0UL)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:G4} {1}", new object[]
					{
						this.ToBytes() / 1024.0,
						"KB"
					});
				}
				return string.Format(CultureInfo.InvariantCulture, "{0:G4} {1}", new object[]
				{
					this.ToBytes(),
					"B"
				});
			}
		}

		public ulong RoundUpToUnit(ByteQuantifiedSize.Quantifier quantifier)
		{
			if (quantifier == (ByteQuantifiedSize.Quantifier)0UL || (quantifier & quantifier - 1UL) != (ByteQuantifiedSize.Quantifier)0UL)
			{
				throw new ArgumentException("Invalid quantifier value", "quantifier");
			}
			ulong num = this.bytes / (ulong)quantifier;
			if ((this.bytes & quantifier - ByteQuantifiedSize.Quantifier.None) != 0UL)
			{
				num += 1UL;
			}
			return num;
		}

		public override string ToString()
		{
			if (this.canonicalForm == null)
			{
				this.canonicalForm = this.ToString("A");
			}
			return this.canonicalForm;
		}

		public string ToString(string format)
		{
			if (!string.IsNullOrEmpty(format) && format.Length == 1)
			{
				char c = format[0];
				if (c <= 'G')
				{
					switch (c)
					{
					case 'A':
						return this.ToLargestAppropriateUnitFormatString(true);
					case 'B':
						return this.ToString(ByteQuantifiedSize.Quantifier.None);
					default:
						if (c == 'G')
						{
							return this.ToString(ByteQuantifiedSize.Quantifier.GB);
						}
						break;
					}
				}
				else
				{
					switch (c)
					{
					case 'K':
						return this.ToString(ByteQuantifiedSize.Quantifier.KB);
					case 'L':
						break;
					case 'M':
						return this.ToString(ByteQuantifiedSize.Quantifier.MB);
					default:
						if (c == 'T')
						{
							return this.ToString(ByteQuantifiedSize.Quantifier.TB);
						}
						if (c == 'a')
						{
							return this.ToLargestAppropriateUnitFormatString(false);
						}
						break;
					}
				}
				throw new FormatException(DataStrings.ExceptionFormatNotSupported);
			}
			throw new FormatException(DataStrings.ExceptionFormatNotSupported);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (string.IsNullOrEmpty(format))
			{
				return this.ToString();
			}
			return this.ToString(format);
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is ByteQuantifiedSize && this.Equals((ByteQuantifiedSize)obj);
		}

		public bool Equals(ByteQuantifiedSize other)
		{
			return other.bytes == this.bytes;
		}

		public override int GetHashCode()
		{
			return this.bytes.GetHashCode();
		}

		public static bool operator <(ByteQuantifiedSize value1, ByteQuantifiedSize value2)
		{
			return value1.bytes < value2.bytes;
		}

		public static bool operator <=(ByteQuantifiedSize value1, ByteQuantifiedSize value2)
		{
			return value1.bytes <= value2.bytes;
		}

		public static bool operator >(ByteQuantifiedSize value1, ByteQuantifiedSize value2)
		{
			return value1.bytes > value2.bytes;
		}

		public static bool operator >=(ByteQuantifiedSize value1, ByteQuantifiedSize value2)
		{
			return value1.bytes >= value2.bytes;
		}

		public static bool operator ==(ByteQuantifiedSize value1, ByteQuantifiedSize value2)
		{
			return value1.bytes == value2.bytes;
		}

		public static bool operator !=(ByteQuantifiedSize value1, ByteQuantifiedSize value2)
		{
			return value1.bytes != value2.bytes;
		}

		public static ByteQuantifiedSize operator *(ByteQuantifiedSize value1, ByteQuantifiedSize value2)
		{
			return new ByteQuantifiedSize(checked(value1.bytes * value2.bytes));
		}

		public static ByteQuantifiedSize operator *(ByteQuantifiedSize value1, ulong value2)
		{
			return new ByteQuantifiedSize(checked(value1.bytes * value2));
		}

		public static ByteQuantifiedSize operator *(ByteQuantifiedSize value1, int value2)
		{
			return new ByteQuantifiedSize(checked(value1.bytes * (ulong)value2));
		}

		public static ByteQuantifiedSize operator /(ByteQuantifiedSize value1, ByteQuantifiedSize value2)
		{
			return new ByteQuantifiedSize(value1.bytes / value2.bytes);
		}

		public static ByteQuantifiedSize operator /(ByteQuantifiedSize value1, ulong value2)
		{
			return new ByteQuantifiedSize(value1.bytes / value2);
		}

		public static ByteQuantifiedSize operator /(ByteQuantifiedSize value1, int value2)
		{
			return new ByteQuantifiedSize(value1.bytes / (ulong)((long)value2));
		}

		public static ByteQuantifiedSize operator +(ByteQuantifiedSize value1, ulong value2)
		{
			return new ByteQuantifiedSize(checked(value1.bytes + value2));
		}

		public static ByteQuantifiedSize operator +(ByteQuantifiedSize value1, int value2)
		{
			return new ByteQuantifiedSize(checked(value1.bytes + (ulong)value2));
		}

		public static ByteQuantifiedSize operator +(ByteQuantifiedSize value1, ByteQuantifiedSize value2)
		{
			return new ByteQuantifiedSize(checked(value1.bytes + value2.bytes));
		}

		public static ByteQuantifiedSize operator -(ByteQuantifiedSize value1, ByteQuantifiedSize value2)
		{
			return new ByteQuantifiedSize(checked(value1.bytes - value2.bytes));
		}

		public static ByteQuantifiedSize operator -(ByteQuantifiedSize value1, ulong value2)
		{
			return new ByteQuantifiedSize(checked(value1.bytes - value2));
		}

		public static ByteQuantifiedSize operator -(ByteQuantifiedSize value1, int value2)
		{
			return new ByteQuantifiedSize(checked(value1.bytes - (ulong)value2));
		}

		public static explicit operator ulong(ByteQuantifiedSize size)
		{
			return size.bytes;
		}

		public static explicit operator double(ByteQuantifiedSize size)
		{
			return size.bytes;
		}

		public static ByteQuantifiedSize Parse(string expression, ByteQuantifiedSize.Quantifier defaultUnit)
		{
			ulong number;
			ByteQuantifiedSize result;
			if (ulong.TryParse(expression, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out number))
			{
				result = ByteQuantifiedSize.FromSpecifiedUnit(number, defaultUnit);
			}
			else
			{
				result = ByteQuantifiedSize.Parse(expression);
			}
			return result;
		}

		public static bool TryParse(string expression, ByteQuantifiedSize.Quantifier defaultUnit, out ByteQuantifiedSize byteQuantifiedSize)
		{
			bool result = true;
			ulong number;
			if (ulong.TryParse(expression, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out number))
			{
				try
				{
					byteQuantifiedSize = ByteQuantifiedSize.FromSpecifiedUnit(number, defaultUnit);
					return result;
				}
				catch (ArgumentException)
				{
					byteQuantifiedSize = default(ByteQuantifiedSize);
					return false;
				}
			}
			result = ByteQuantifiedSize.TryParse(expression, out byteQuantifiedSize);
			return result;
		}

		public static ByteQuantifiedSize Parse(string expression)
		{
			ByteQuantifiedSize result = default(ByteQuantifiedSize);
			ulong num = 0UL;
			ByteQuantifiedSize.Quantifier quantifier;
			Exception ex;
			if (ByteQuantifiedSize.InternalTryParse(expression, out num, out quantifier, out ex))
			{
				result.bytes = num;
				result.unit = quantifier;
				result.canonicalForm = result.ToString("A");
				return result;
			}
			throw ex;
		}

		public static bool TryParse(string expression, out ByteQuantifiedSize byteQuantifiedSize)
		{
			byteQuantifiedSize = default(ByteQuantifiedSize);
			byteQuantifiedSize.bytes = 0UL;
			byteQuantifiedSize.unit = ByteQuantifiedSize.Quantifier.None;
			ulong num = 0UL;
			ByteQuantifiedSize.Quantifier quantifier;
			Exception ex;
			if (ByteQuantifiedSize.InternalTryParse(expression, out num, out quantifier, out ex))
			{
				byteQuantifiedSize.bytes = num;
				byteQuantifiedSize.unit = quantifier;
				byteQuantifiedSize.canonicalForm = byteQuantifiedSize.ToString("A");
				return true;
			}
			return false;
		}

		private static bool InternalTryParse(string expression, out ulong bytes, out ByteQuantifiedSize.Quantifier unit, out Exception error)
		{
			bytes = 0UL;
			unit = ByteQuantifiedSize.Quantifier.None;
			error = null;
			expression = expression.Trim();
			Match match = ByteQuantifiedSize.LargestAppropriateUnitFormatPattern.Match(expression);
			if (!match.Success)
			{
				error = new FormatException(DataStrings.ExceptionFormatNotCorrect(expression));
				return false;
			}
			string a;
			ByteQuantifiedSize.MaximumValues maximumValues;
			if ((a = match.Groups["Unit"].Value.ToUpper()) != null)
			{
				if (a == "KB")
				{
					unit = ByteQuantifiedSize.Quantifier.KB;
					maximumValues = ByteQuantifiedSize.MaximumValues.KB;
					goto IL_EC;
				}
				if (a == "MB")
				{
					unit = ByteQuantifiedSize.Quantifier.MB;
					maximumValues = ByteQuantifiedSize.MaximumValues.MB;
					goto IL_EC;
				}
				if (a == "GB")
				{
					unit = ByteQuantifiedSize.Quantifier.GB;
					maximumValues = ByteQuantifiedSize.MaximumValues.GB;
					goto IL_EC;
				}
				if (a == "TB")
				{
					unit = ByteQuantifiedSize.Quantifier.TB;
					maximumValues = ByteQuantifiedSize.MaximumValues.TB;
					goto IL_EC;
				}
			}
			unit = ByteQuantifiedSize.Quantifier.None;
			maximumValues = ByteQuantifiedSize.MaximumValues.None;
			IL_EC:
			if (match.Groups["LongBytes"].Success)
			{
				if (!ulong.TryParse(match.Groups["LongBytes"].Value, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out bytes))
				{
					error = new FormatException(DataStrings.ExceptionFormatNotCorrect(expression));
					return false;
				}
			}
			else
			{
				double num;
				if (!double.TryParse(match.Groups["ShortBytes"].Value, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, null, out num))
				{
					error = new FormatException(DataStrings.ExceptionFormatNotCorrect(expression));
					return false;
				}
				if (maximumValues < num || 0.0 > num)
				{
					error = new OverflowException(DataStrings.ExceptionValueOverflow(ByteQuantifiedSize.MinValue.ToString(unit), ByteQuantifiedSize.MaxValue.ToString(unit), expression));
					return false;
				}
				bytes = (ulong)(unit * num);
			}
			return true;
		}

		public static ByteQuantifiedSize FromSpecifiedUnit(ulong number, ByteQuantifiedSize.Quantifier specifiedUnit)
		{
			ByteQuantifiedSize byteQuantifiedSize = default(ByteQuantifiedSize);
			if (specifiedUnit <= ByteQuantifiedSize.Quantifier.KB)
			{
				if (specifiedUnit == ByteQuantifiedSize.Quantifier.None)
				{
					return ByteQuantifiedSize.FromBytes(number);
				}
				if (specifiedUnit == ByteQuantifiedSize.Quantifier.KB)
				{
					return ByteQuantifiedSize.FromKB(number);
				}
			}
			else
			{
				if (specifiedUnit == ByteQuantifiedSize.Quantifier.MB)
				{
					return ByteQuantifiedSize.FromMB(number);
				}
				if (specifiedUnit == ByteQuantifiedSize.Quantifier.GB)
				{
					return ByteQuantifiedSize.FromGB(number);
				}
				if (specifiedUnit == ByteQuantifiedSize.Quantifier.TB)
				{
					return ByteQuantifiedSize.FromTB(number);
				}
			}
			throw new ArgumentException(DataStrings.ExceptionUnknownUnit);
		}

		public int CompareTo(ByteQuantifiedSize other)
		{
			return Comparer<ulong>.Default.Compare(this.bytes, other.bytes);
		}

		int IComparable.CompareTo(object other)
		{
			if (other == null)
			{
				return 1;
			}
			if (other is ByteQuantifiedSize)
			{
				return this.CompareTo((ByteQuantifiedSize)other);
			}
			throw new ArgumentException(DataStrings.ExceptionObjectInvalid);
		}

		private string ToString(ByteQuantifiedSize.Quantifier quantifier)
		{
			if (quantifier <= ByteQuantifiedSize.Quantifier.KB)
			{
				if (quantifier == ByteQuantifiedSize.Quantifier.None)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:N0} B", new object[]
					{
						this.ToBytes()
					});
				}
				if (quantifier == ByteQuantifiedSize.Quantifier.KB)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:N0} KB", new object[]
					{
						this.ToKB()
					});
				}
			}
			else
			{
				if (quantifier == ByteQuantifiedSize.Quantifier.MB)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:N0} MB", new object[]
					{
						this.ToMB()
					});
				}
				if (quantifier == ByteQuantifiedSize.Quantifier.GB)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:N0} GB", new object[]
					{
						this.ToGB()
					});
				}
				if (quantifier == ByteQuantifiedSize.Quantifier.TB)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0:N0} TB", new object[]
					{
						this.ToTB()
					});
				}
			}
			return this.ToLargestAppropriateUnitFormatString(true);
		}

		private static readonly Regex LargestAppropriateUnitFormatPattern = new Regex("^(?<ShortBytes>\\S{0,14}\\d)( ?(?<Unit>[KMGT]?B)( \\((?<LongBytes>\\S+) bytes\\))?)?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public static readonly ByteQuantifiedSize MinValue = ByteQuantifiedSize.FromBytes(0UL);

		public static readonly ByteQuantifiedSize MaxValue = ByteQuantifiedSize.FromBytes(ulong.MaxValue);

		public static readonly ByteQuantifiedSize Zero = ByteQuantifiedSize.FromBytes(0UL);

		public static readonly IFormatProvider KilobyteQuantifierProvider = new ByteQuantifiedSize.QuantifierProvider(ByteQuantifiedSize.Quantifier.KB);

		public static readonly IFormatProvider MegabyteQuantifierProvider = new ByteQuantifiedSize.QuantifierProvider(ByteQuantifiedSize.Quantifier.MB);

		private ulong bytes;

		private ByteQuantifiedSize.Quantifier unit;

		private string canonicalForm;

		public enum Quantifier : ulong
		{
			None = 1UL,
			KB = 1024UL,
			MB = 1048576UL,
			GB = 1073741824UL,
			TB = 1099511627776UL
		}

		[Serializable]
		private class QuantifierProvider : IFormatProvider
		{
			public QuantifierProvider(ByteQuantifiedSize.Quantifier quantifier)
			{
				this.quantifier = quantifier;
			}

			public object GetFormat(Type formatType)
			{
				if (formatType != typeof(ByteQuantifiedSize.Quantifier))
				{
					throw new ArgumentException("This kind of format type is not implemented", "formatType");
				}
				return this.quantifier;
			}

			private readonly ByteQuantifiedSize.Quantifier quantifier;
		}

		private enum MaximumValues : ulong
		{
			None = 18446744073709551615UL,
			KB = 18014398509481983UL,
			MB = 17592186044415UL,
			GB = 17179869183UL,
			TB = 16777215UL
		}

		private enum QuantifierZeroBits
		{
			None,
			KB = 10,
			MB = 20,
			GB = 30,
			TB = 40
		}
	}
}
