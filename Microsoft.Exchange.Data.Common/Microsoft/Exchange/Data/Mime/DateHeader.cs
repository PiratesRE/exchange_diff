using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public class DateHeader : Header
	{
		public DateHeader(string name, DateTime dateTime) : base(name, Header.GetHeaderId(name, true))
		{
			Type type = Header.TypeFromHeaderId(base.HeaderId);
			if (base.HeaderId != HeaderId.Unknown && type != typeof(DateHeader))
			{
				throw new ArgumentException(Strings.NameNotValidForThisHeaderType(name, "DateHeader", type.Name));
			}
			base.SetRawValue(null, true);
			this.parsed = true;
			this.utcDateTime = dateTime.ToUniversalTime();
			if (dateTime.Kind != DateTimeKind.Utc)
			{
				this.timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
			}
		}

		public DateHeader(string name, DateTime dateTime, TimeSpan timeZoneOffset) : base(name, Header.GetHeaderId(name, true))
		{
			Type type = Header.TypeFromHeaderId(base.HeaderId);
			if (base.HeaderId != HeaderId.Unknown && type != typeof(DateHeader))
			{
				throw new ArgumentException(Strings.NameNotValidForThisHeaderType(name, "DateHeader", type.Name));
			}
			base.SetRawValue(null, true);
			this.parsed = true;
			this.utcDateTime = dateTime.ToUniversalTime();
			this.timeZoneOffset = timeZoneOffset;
		}

		internal DateHeader(string name, HeaderId headerId) : base(name, headerId)
		{
		}

		public sealed override string Value
		{
			get
			{
				return base.GetRawValue(false);
			}
			set
			{
				base.SetRawValue(value, true, false);
			}
		}

		public DateTime DateTime
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				if (!(this.utcDateTime == DateHeader.minDateTime))
				{
					return this.utcDateTime.ToLocalTime();
				}
				return DateHeader.minDateTime;
			}
			set
			{
				base.SetRawValue(null, true);
				this.parsed = true;
				this.utcDateTime = value.ToUniversalTime();
				if (value.Kind != DateTimeKind.Utc)
				{
					this.timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(value);
					return;
				}
				this.timeZoneOffset = TimeSpan.Zero;
			}
		}

		public DateTime UtcDateTime
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.utcDateTime;
			}
		}

		public TimeSpan TimeZoneOffset
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.timeZoneOffset;
			}
		}

		internal override byte[] RawValue
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				int num = 0;
				byte[] array = DateHeader.FormatValue(this.utcDateTime, this.timeZoneOffset, out num);
				if (array == null)
				{
					array = MimeString.EmptyByteArray;
				}
				return array;
			}
			set
			{
				base.RawValue = value;
			}
		}

		public sealed override MimeNode Clone()
		{
			DateHeader dateHeader = new DateHeader(base.Name, base.HeaderId);
			this.CopyTo(dateHeader);
			return dateHeader;
		}

		public sealed override void CopyTo(object destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (destination == this)
			{
				return;
			}
			DateHeader dateHeader = destination as DateHeader;
			if (dateHeader == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
			}
			base.CopyTo(destination);
			dateHeader.parsed = this.parsed;
			dateHeader.utcDateTime = this.utcDateTime;
			dateHeader.timeZoneOffset = this.timeZoneOffset;
		}

		public sealed override bool IsValueValid(string value)
		{
			return ByteString.IsStringArgumentValid(value, false);
		}

		internal static long WriteDateHeaderValue(Stream stream, DateTime utcDateTime, TimeSpan timeZoneOffset, ref MimeStringLength currentLineLength)
		{
			long num = 0L;
			int countInChars = 0;
			byte[] array = DateHeader.FormatValue(utcDateTime, timeZoneOffset, out countInChars);
			stream.Write(MimeString.Space, 0, MimeString.Space.Length);
			num += (long)MimeString.Space.Length;
			currentLineLength.IncrementBy(MimeString.Space.Length);
			if (array != null)
			{
				stream.Write(array, 0, array.Length);
				num += (long)array.Length;
				currentLineLength.IncrementBy(countInChars, array.Length);
			}
			else
			{
				stream.Write(MimeString.CommentInvalidDate, 0, MimeString.CommentInvalidDate.Length);
				num += (long)MimeString.CommentInvalidDate.Length;
				currentLineLength.IncrementBy(MimeString.CommentInvalidDate.Length);
			}
			return num + Header.WriteLineEnd(stream, ref currentLineLength);
		}

		internal static DateTime ParseDateHeaderValue(string value)
		{
			byte[] array;
			if (value != null)
			{
				array = ByteString.StringToBytes(value, false);
			}
			else
			{
				array = MimeString.EmptyByteArray;
			}
			MimeStringList list = new MimeStringList(array, 0, array.Length);
			DateTime result;
			TimeSpan timeSpan;
			DateHeader.ParseValue(list, out result, out timeSpan);
			return result;
		}

		internal void SetValue(DateTime value, TimeSpan timeZoneOffset)
		{
			base.SetRawValue(null, true);
			this.parsed = true;
			this.utcDateTime = value.ToUniversalTime();
			this.timeZoneOffset = timeZoneOffset;
		}

		internal override void RawValueAboutToChange()
		{
			this.Reset();
		}

		internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			long num = base.WriteName(stream, ref scratchBuffer);
			currentLineLength.IncrementBy((int)num);
			if (!base.IsDirty && base.RawLength != 0 && base.IsProtected)
			{
				long num2 = Header.WriteLines(base.Lines, stream);
				num += num2;
				currentLineLength.SetAs(0);
				return num;
			}
			if (!this.parsed)
			{
				this.Parse();
			}
			num += DateHeader.WriteDateHeaderValue(stream, this.utcDateTime, this.timeZoneOffset, ref currentLineLength);
			currentLineLength.SetAs(0);
			return num;
		}

		internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			throw new NotSupportedException(Strings.CannotAddChildrenToMimeHeaderDate);
		}

		internal override void ForceParse()
		{
			if (!this.parsed)
			{
				this.Parse();
			}
		}

		private static byte MakeUpper(byte ch)
		{
			return (ch >= 97 && ch <= 122) ? (ch - 97 + 65) : ch;
		}

		private static DateHeader.TimeZoneToken MapZoneToInt(byte[] ptr, int offset, int len)
		{
			byte b = DateHeader.MakeUpper(ptr[offset++]);
			switch (b)
			{
			case 67:
				if (--len == 0 || 68 != DateHeader.MakeUpper(ptr[offset++]))
				{
					return DateHeader.TimeZoneToken.CST;
				}
				return DateHeader.TimeZoneToken.EST;
			case 68:
			case 70:
			case 72:
			case 73:
			case 76:
				break;
			case 69:
				if (--len == 0 || 68 != DateHeader.MakeUpper(ptr[offset++]))
				{
					return DateHeader.TimeZoneToken.EST;
				}
				return DateHeader.TimeZoneToken.EDT;
			case 71:
				return DateHeader.TimeZoneToken.GMT;
			case 74:
				return DateHeader.TimeZoneToken.KST;
			case 75:
				return DateHeader.TimeZoneToken.KST;
			case 77:
				if (--len == 0 || 68 != DateHeader.MakeUpper(ptr[offset++]))
				{
					return DateHeader.TimeZoneToken.MST;
				}
				return DateHeader.TimeZoneToken.CST;
			default:
				if (b != 80)
				{
					if (b == 85)
					{
						return DateHeader.TimeZoneToken.GMT;
					}
				}
				else
				{
					if (--len == 0 || 68 != DateHeader.MakeUpper(ptr[offset++]))
					{
						return DateHeader.TimeZoneToken.PST;
					}
					return DateHeader.TimeZoneToken.MST;
				}
				break;
			}
			return DateHeader.TimeZoneToken.GMT;
		}

		private static int MapMonthToInt(byte[] ptr, int offset, int len)
		{
			byte b = DateHeader.MakeUpper(ptr[offset++]);
			if (b <= 70)
			{
				if (b != 65)
				{
					switch (b)
					{
					case 68:
						return 12;
					case 70:
						return 2;
					}
				}
				else
				{
					if (--len == 0 || 85 != DateHeader.MakeUpper(ptr[offset++]))
					{
						return 4;
					}
					return 8;
				}
			}
			else
			{
				switch (b)
				{
				case 74:
					if (--len == 0 || 85 != DateHeader.MakeUpper(ptr[offset++]))
					{
						return 1;
					}
					if (--len == 0 || 76 != DateHeader.MakeUpper(ptr[offset++]))
					{
						return 6;
					}
					return 7;
				case 75:
				case 76:
					break;
				case 77:
					if (--len == 0 || 65 != DateHeader.MakeUpper(ptr[offset++]))
					{
						return 3;
					}
					if (--len == 0 || 89 != DateHeader.MakeUpper(ptr[offset++]))
					{
						return 3;
					}
					return 5;
				case 78:
					return 11;
				case 79:
					return 10;
				default:
					if (b == 83)
					{
						return 9;
					}
					break;
				}
			}
			return 1;
		}

		private static void ParseValue(MimeStringList list, out DateTime utcDateTime, out TimeSpan timeZoneOffset)
		{
			MimeStringList mimeStringList = default(MimeStringList);
			ValueParser valueParser = new ValueParser(list, false);
			DateHeader.ParseStage parseStage = DateHeader.ParseStage.DayOfWeek;
			int[] array = new int[8];
			byte b = 32;
			while (parseStage != DateHeader.ParseStage.Count)
			{
				valueParser.ParseCFWS(false, ref mimeStringList, true);
				byte b2 = valueParser.ParseGet();
				if (b2 == 0)
				{
					break;
				}
				if (!MimeScan.IsToken(b2) || b2 == 45 || b2 == 43)
				{
					b = b2;
					valueParser.ParseCFWS(false, ref mimeStringList, true);
				}
				else
				{
					if (MimeScan.IsDigit(b2))
					{
						if (parseStage == DateHeader.ParseStage.DayOfWeek)
						{
							parseStage = DateHeader.ParseStage.DayOfMonth;
						}
						if (parseStage == DateHeader.ParseStage.Second && b != 58)
						{
							parseStage = DateHeader.ParseStage.Zone;
						}
						int num = 0;
						do
						{
							num++;
							array[(int)parseStage] *= 10;
							array[(int)parseStage] += (int)(b2 - 48);
							b2 = valueParser.ParseGet();
						}
						while (b2 != 0 && MimeScan.IsDigit(b2));
						if (b2 != 0)
						{
							valueParser.ParseUnget();
						}
						if (parseStage == DateHeader.ParseStage.Year && num <= 2)
						{
							array[(int)parseStage] += ((array[(int)parseStage] < 30) ? 2000 : 1900);
						}
						if (parseStage == DateHeader.ParseStage.Zone && num <= 2)
						{
							array[(int)parseStage] *= 100;
						}
						if (parseStage == DateHeader.ParseStage.Zone && b == 45)
						{
							array[(int)parseStage] = -array[(int)parseStage];
						}
						parseStage++;
					}
					else if (MimeScan.IsAlpha(b2))
					{
						valueParser.ParseUnget();
						MimeString mimeString = valueParser.ParseToken(MimeScan.Token.Alpha);
						if (parseStage == DateHeader.ParseStage.DayOfWeek)
						{
							parseStage = DateHeader.ParseStage.DayOfMonth;
						}
						else if (parseStage == DateHeader.ParseStage.Month)
						{
							array[(int)parseStage] = DateHeader.MapMonthToInt(mimeString.Data, mimeString.Offset, mimeString.Length);
							parseStage = DateHeader.ParseStage.Year;
						}
						else if (parseStage >= DateHeader.ParseStage.Second)
						{
							if (mimeString.Length == 2 && 65 == DateHeader.MakeUpper(mimeString[0]) && 77 == DateHeader.MakeUpper(mimeString[1]))
							{
								if (array[4] == 12)
								{
									array[4] = 0;
								}
								parseStage = DateHeader.ParseStage.Zone;
							}
							else if (mimeString.Length == 2 && 80 == DateHeader.MakeUpper(mimeString[0]) && 77 == DateHeader.MakeUpper(mimeString[1]))
							{
								if (array[4] < 12)
								{
									array[4] += 12;
								}
								parseStage = DateHeader.ParseStage.Zone;
							}
							else
							{
								array[7] = (int)DateHeader.MapZoneToInt(mimeString.Data, mimeString.Offset, mimeString.Length);
								parseStage = DateHeader.ParseStage.Count;
							}
						}
					}
					b = 32;
				}
			}
			if (parseStage > DateHeader.ParseStage.Year)
			{
				int num2 = array[3];
				int num3 = array[2];
				int num4 = array[1];
				int num5 = array[4];
				int num6 = array[5];
				int num7 = array[6];
				if (num5 == 23 && num6 == 59 && num7 == 60)
				{
					num7 = 59;
				}
				if (num2 >= 1900 && num2 <= 9999 && num3 >= 1 && num3 <= 12 && num4 >= 1 && num4 <= DateTime.DaysInMonth(num2, num3) && num5 >= 0 && num5 < 24 && num6 >= 0 && num6 < 60 && num7 >= 0 && num7 < 60)
				{
					try
					{
						utcDateTime = new DateTime(num2, num3, num4, num5, num6, num7, 0, DateTimeKind.Utc);
						goto IL_319;
					}
					catch (ArgumentException)
					{
						utcDateTime = DateHeader.minDateTime;
						goto IL_319;
					}
				}
				utcDateTime = DateHeader.minDateTime;
			}
			else
			{
				utcDateTime = DateHeader.minDateTime;
			}
			IL_319:
			if (parseStage != DateHeader.ParseStage.Count || !(utcDateTime > DateHeader.minDateTime))
			{
				timeZoneOffset = TimeSpan.Zero;
				return;
			}
			int num8 = array[7] / 100;
			int num9 = array[7] % 100;
			if (num8 > 99 || num8 < -99)
			{
				num8 = 0;
				num9 = 0;
			}
			if (num9 > 59 || num9 < -59)
			{
				num9 = 0;
			}
			timeZoneOffset = new TimeSpan(num8, num9, 0);
			if (utcDateTime.Ticks >= timeZoneOffset.Ticks && DateTime.MaxValue.Ticks >= utcDateTime.Ticks - timeZoneOffset.Ticks)
			{
				utcDateTime -= timeZoneOffset;
				return;
			}
			utcDateTime = DateHeader.minDateTime;
			timeZoneOffset = TimeSpan.Zero;
		}

		private static byte[] FormatValue(DateTime utcDateTime, TimeSpan timeZoneOffset, out int characterCount)
		{
			if (DateHeader.minDateTime == utcDateTime)
			{
				characterCount = 0;
				return null;
			}
			string text = (utcDateTime + timeZoneOffset).ToString("ddd, d MMM yyyy HH:mm:ss ", CultureInfo.InvariantCulture);
			int num = ByteString.StringToBytesCount(text, false);
			string text2 = (timeZoneOffset.Hours * 100 + timeZoneOffset.Minutes).ToString("+0000;-0000");
			int num2 = ByteString.StringToBytesCount(text2, false);
			byte[] array = new byte[num + num2];
			ByteString.StringToBytes(text, array, 0, false);
			ByteString.StringToBytes(text2, array, num, false);
			characterCount = text.Length + text2.Length;
			return array;
		}

		private void Reset()
		{
			this.parsed = false;
			this.utcDateTime = DateHeader.minDateTime;
			this.timeZoneOffset = TimeSpan.Zero;
		}

		private void Parse()
		{
			this.parsed = true;
			DateHeader.ParseValue(base.Lines, out this.utcDateTime, out this.timeZoneOffset);
		}

		internal const bool AllowUTF8Value = false;

		private const int Y2KThreshold = 30;

		private static readonly DateTime minDateTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);

		private bool parsed;

		private DateTime utcDateTime = DateHeader.minDateTime;

		private TimeSpan timeZoneOffset = TimeSpan.Zero;

		private enum TimeZoneToken
		{
			GMT,
			EST = -500,
			EDT = -400,
			CST = -600,
			CDT = -500,
			MST = -700,
			MDT = -600,
			PST = -800,
			PDT = -700,
			KST = 900,
			JST = 900
		}

		private enum ParseStage
		{
			DayOfWeek,
			DayOfMonth,
			Month,
			Year,
			Hour,
			Minute,
			Second,
			Zone,
			Count
		}
	}
}
