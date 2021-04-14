using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public abstract class Header : MimeNode
	{
		internal Header(string name, HeaderId headerId)
		{
			this.name = name;
			this.headerId = headerId;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public HeaderId HeaderId
		{
			get
			{
				return this.headerId;
			}
		}

		public abstract string Value { get; set; }

		public virtual bool RequiresSMTPUTF8
		{
			get
			{
				return false;
			}
		}

		internal virtual byte[] RawValue
		{
			get
			{
				if (this.lines.Length == 0)
				{
					return MimeString.EmptyByteArray;
				}
				byte[] array = this.lines.GetSz();
				if (array == null)
				{
					array = MimeString.EmptyByteArray;
				}
				return array;
			}
			set
			{
				this.SetRawValue(value, true);
			}
		}

		internal MimeString FirstRawToken
		{
			get
			{
				MimeStringList mimeStringList;
				if (this.lines.Length == 0)
				{
					byte[] rawValue = this.RawValue;
					mimeStringList = new MimeStringList(rawValue);
				}
				else
				{
					mimeStringList = this.lines;
				}
				DecodingOptions headerDecodingOptions = base.GetHeaderDecodingOptions();
				ValueParser valueParser = new ValueParser(mimeStringList, headerDecodingOptions.AllowUTF8);
				MimeStringList mimeStringList2 = default(MimeStringList);
				valueParser.ParseCFWS(false, ref mimeStringList2, true);
				return valueParser.ParseToken();
			}
		}

		internal MimeStringList Lines
		{
			get
			{
				return this.lines;
			}
			set
			{
				this.SetRawValue(value, true);
			}
		}

		internal int RawLength
		{
			get
			{
				return this.lines.Length;
			}
		}

		internal bool IsDirty
		{
			get
			{
				return this.dirty;
			}
		}

		internal bool IsProtected
		{
			get
			{
				MimePart mimePart = base.GetTreeRoot() as MimePart;
				return mimePart != null && mimePart.IsProtectedHeader(this.Name);
			}
		}

		public static Header Create(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			HeaderId headerId = Header.GetHeaderId(name, true);
			if (headerId != HeaderId.Unknown)
			{
				return Header.Create(name, headerId);
			}
			return new TextHeader(name, HeaderId.Unknown);
		}

		public static Header Create(HeaderId headerId)
		{
			return Header.Create(null, headerId);
		}

		internal static Header Create(string name, HeaderId headerId)
		{
			if (headerId < HeaderId.Unknown || headerId > (HeaderId)MimeData.nameIndex.Length)
			{
				throw new ArgumentException(Strings.InvalidHeaderId, "headerId");
			}
			if (headerId == HeaderId.Unknown)
			{
				throw new ArgumentException(Strings.CannotDetermineHeaderNameFromId, "headerId");
			}
			Header header;
			switch (MimeData.headerNames[(int)MimeData.nameIndex[(int)headerId]].headerType)
			{
			case HeaderType.AsciiText:
				header = new AsciiTextHeader(MimeData.headerNames[(int)MimeData.nameIndex[(int)headerId]].name, headerId);
				break;
			case HeaderType.Date:
				header = new DateHeader(MimeData.headerNames[(int)MimeData.nameIndex[(int)headerId]].name, headerId);
				break;
			case HeaderType.Received:
				header = new ReceivedHeader();
				break;
			case HeaderType.ContentType:
				header = new ContentTypeHeader();
				break;
			case HeaderType.ContentDisposition:
				header = new ContentDispositionHeader();
				break;
			case HeaderType.Address:
				header = new AddressHeader(MimeData.headerNames[(int)MimeData.nameIndex[(int)headerId]].name, headerId);
				break;
			default:
				header = new TextHeader(MimeData.headerNames[(int)MimeData.nameIndex[(int)headerId]].name, headerId);
				break;
			}
			if (!string.IsNullOrEmpty(name) && !header.MatchName(name))
			{
				throw new ArgumentException("name");
			}
			return header;
		}

		public static bool IsHeaderNameValid(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			for (int i = 0; i < name.Length; i++)
			{
				if (name[i] >= '\u0080' || !MimeScan.IsField((byte)name[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static HeaderId GetHeaderId(string name)
		{
			return Header.GetHeaderId(name, true);
		}

		public static Header ReadFrom(MimeHeaderReader reader)
		{
			if (reader.MimeReader == null)
			{
				throw new ArgumentNullException("reader");
			}
			return reader.MimeReader.ReadHeaderObject();
		}

		public virtual bool TryGetValue(out string value)
		{
			value = this.Value;
			return true;
		}

		public virtual bool IsValueValid(string value)
		{
			return false;
		}

		public override void CopyTo(object destination)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (destination == this)
			{
				return;
			}
			Header header = destination as Header;
			if (header == null)
			{
				throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
			}
			base.CopyTo(destination);
			header.lines = this.lines.Clone();
			header.dirty = this.dirty;
		}

		internal static Type TypeFromHeaderId(HeaderId headerId)
		{
			switch (MimeData.headerNames[(int)MimeData.nameIndex[(int)headerId]].headerType)
			{
			case HeaderType.AsciiText:
				return typeof(AsciiTextHeader);
			case HeaderType.Date:
				return typeof(DateHeader);
			case HeaderType.Received:
				return typeof(ReceivedHeader);
			case HeaderType.ContentType:
				return typeof(ContentTypeHeader);
			case HeaderType.ContentDisposition:
				return typeof(ContentDispositionHeader);
			case HeaderType.Address:
				return typeof(AddressHeader);
			default:
				return typeof(TextHeader);
			}
		}

		internal static HeaderId GetHeaderId(string name, bool validateArgument)
		{
			if (name == null)
			{
				if (validateArgument)
				{
					throw new ArgumentNullException("name");
				}
				return HeaderId.Unknown;
			}
			else
			{
				if (name.Length != 0)
				{
					if (validateArgument)
					{
						for (int i = 0; i < name.Length; i++)
						{
							if (name[i] >= '\u0080' || !MimeScan.IsField((byte)name[i]))
							{
								throw new ArgumentException(Strings.InvalidHeaderName(name, i), "name");
							}
						}
					}
					HeaderNameIndex headerNameIndex = Header.LookupName(name);
					return MimeData.headerNames[(int)headerNameIndex].publicHeaderId;
				}
				if (validateArgument)
				{
					throw new ArgumentException("Header name cannot be an empty string", "name");
				}
				return HeaderId.Unknown;
			}
		}

		internal static HeaderId GetHeaderId(byte[] name, int offset, int length)
		{
			HeaderNameIndex headerNameIndex = Header.LookupName(name, offset, length);
			return MimeData.headerNames[(int)headerNameIndex].publicHeaderId;
		}

		internal static string GetHeaderName(HeaderId headerId)
		{
			return MimeData.headerNames[(int)MimeData.nameIndex[(int)headerId]].name;
		}

		internal static Header CreateGeneralHeader(string name)
		{
			return new TextHeader(name, HeaderId.Unknown);
		}

		internal static bool IsRestrictedHeader(HeaderId headerId)
		{
			return MimeData.headerNames[(int)MimeData.nameIndex[(int)headerId]].restricted;
		}

		internal static long WriteLines(MimeStringList lines, Stream stream)
		{
			if (lines.Count == 0)
			{
				MimeStringLength mimeStringLength = new MimeStringLength(0);
				return Header.WriteLineEnd(stream, ref mimeStringLength);
			}
			long num = 0L;
			for (int i = 0; i < lines.Count; i++)
			{
				int num2;
				int num3;
				byte[] data = lines[i].GetData(out num2, out num3);
				if (num3 != 0)
				{
					if (!MimeScan.IsLWSP(data[num2]))
					{
						stream.Write(MimeString.Space, 0, MimeString.Space.Length);
						num += (long)MimeString.Space.Length;
					}
					stream.Write(data, num2, num3);
					num += (long)num3;
				}
				MimeStringLength mimeStringLength2 = new MimeStringLength(0);
				num += Header.WriteLineEnd(stream, ref mimeStringLength2);
			}
			return num;
		}

		private static long WriteToken(byte[] token, int tokenOffset, MimeStringLength tokenLength, Stream stream, ref MimeStringLength currentLineLength, ref Header.LineBuffer lineBuffer, ref bool autoAddedLastLWSP, bool allowUTF8)
		{
			long num = 0L;
			bool flag = token != null && tokenLength.InChars == 1 && MimeScan.IsFWSP(token[tokenOffset]);
			if (!flag && currentLineLength.InChars + lineBuffer.Length.InChars + tokenLength.InChars > 78 && lineBuffer.LengthTillLastLWSP.InBytes >= 0)
			{
				if (lineBuffer.LengthTillLastLWSP.InBytes > 0)
				{
					stream.Write(lineBuffer.Bytes, 0, lineBuffer.LengthTillLastLWSP.InBytes);
					num += (long)lineBuffer.LengthTillLastLWSP.InBytes;
					currentLineLength.IncrementBy(lineBuffer.LengthTillLastLWSP);
				}
				if (currentLineLength.InBytes > 0)
				{
					num += Header.WriteLineEnd(stream, ref currentLineLength);
				}
				if (autoAddedLastLWSP)
				{
					autoAddedLastLWSP = false;
					lineBuffer.LengthTillLastLWSP.IncrementBy(1);
				}
				if (lineBuffer.LengthTillLastLWSP.InBytes != lineBuffer.Length.InBytes)
				{
					Buffer.BlockCopy(lineBuffer.Bytes, lineBuffer.LengthTillLastLWSP.InBytes, lineBuffer.Bytes, 0, lineBuffer.Length.InBytes - lineBuffer.LengthTillLastLWSP.InBytes);
					lineBuffer.Length.DecrementBy(lineBuffer.LengthTillLastLWSP);
					if (lineBuffer.Length.InBytes > 0 && MimeScan.IsFWSP(lineBuffer.Bytes[0]))
					{
						lineBuffer.LengthTillLastLWSP.SetAs(0);
					}
					else
					{
						lineBuffer.LengthTillLastLWSP.SetAs(-1);
					}
				}
				else
				{
					lineBuffer.Length.SetAs(0);
					lineBuffer.LengthTillLastLWSP.SetAs(-1);
				}
				bool flag2 = false;
				if (lineBuffer.Length.InBytes > 0)
				{
					if (!MimeScan.IsFWSP(lineBuffer.Bytes[0]))
					{
						flag2 = true;
					}
				}
				else if (!flag)
				{
					flag2 = true;
				}
				if (flag2)
				{
					stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
					num += (long)Header.LineStartWhitespace.Length;
					currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
				}
			}
			if (currentLineLength.InBytes + lineBuffer.Length.InBytes + tokenLength.InBytes > 998)
			{
				if (lineBuffer.Length.InBytes > 0)
				{
					stream.Write(lineBuffer.Bytes, 0, lineBuffer.Length.InBytes);
					num += (long)lineBuffer.Length.InBytes;
					currentLineLength.IncrementBy(lineBuffer.Length);
					lineBuffer.Length.SetAs(0);
					autoAddedLastLWSP = false;
					lineBuffer.LengthTillLastLWSP.SetAs(-1);
				}
				if (token != null)
				{
					while (currentLineLength.InBytes + tokenLength.InBytes > 998)
					{
						int num2 = Math.Max(0, 998 - currentLineLength.InBytes);
						int num3 = 0;
						int num4 = 0;
						if (allowUTF8)
						{
							int num5;
							for (int i = 0; i < tokenLength.InBytes; i += num5)
							{
								byte b = token[tokenOffset + i];
								num5 = 1;
								if (b >= 128 && !MimeScan.IsUTF8NonASCII(token, tokenOffset + i, tokenOffset + tokenLength.InBytes, out num5))
								{
									num5 = 1;
								}
								if (num4 + num5 > num2)
								{
									break;
								}
								num3++;
								num4 += num5;
							}
						}
						else
						{
							num3 = num2;
							num4 = num2;
						}
						stream.Write(token, tokenOffset, num4);
						num += (long)num4;
						currentLineLength.IncrementBy(num3, num4);
						tokenOffset += num4;
						tokenLength.DecrementBy(num3, num4);
						num += Header.WriteLineEnd(stream, ref currentLineLength);
						if (!flag)
						{
							stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
							num += (long)Header.LineStartWhitespace.Length;
							currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
						}
					}
				}
			}
			if (token != null)
			{
				Buffer.BlockCopy(token, tokenOffset, lineBuffer.Bytes, lineBuffer.Length.InBytes, tokenLength.InBytes);
				if (flag && (lineBuffer.Length.InBytes == 0 || !MimeScan.IsFWSP(lineBuffer.Bytes[lineBuffer.Length.InBytes - 1])))
				{
					autoAddedLastLWSP = false;
					lineBuffer.LengthTillLastLWSP.SetAs(lineBuffer.Length);
				}
				lineBuffer.Length.IncrementBy(tokenLength);
			}
			return num;
		}

		internal static long QuoteAndFold(Stream stream, MimeStringList fragments, uint inputMask, bool quoteOutput, bool addSpaceAtStart, bool allowUTF8, int lastLineReserve, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			long num = 0L;
			Header.LineBuffer lineBuffer = default(Header.LineBuffer);
			lineBuffer.Length = new MimeStringLength(0);
			lineBuffer.LengthTillLastLWSP = new MimeStringLength(-1);
			if (scratchBuffer == null || scratchBuffer.Length < 998)
			{
				scratchBuffer = new byte[998];
			}
			lineBuffer.Bytes = scratchBuffer;
			MimeScan.Token token = quoteOutput ? (MimeScan.Token.Spec | MimeScan.Token.Fwsp) : MimeScan.Token.Fwsp;
			bool flag = false;
			if (addSpaceAtStart && currentLineLength.InBytes != 0)
			{
				num += Header.WriteToken(Header.Space, 0, new MimeStringLength(1), stream, ref currentLineLength, ref lineBuffer, ref flag, allowUTF8);
				flag = true;
			}
			if (quoteOutput)
			{
				num += Header.WriteToken(Header.DoubleQuote, 0, new MimeStringLength(1), stream, ref currentLineLength, ref lineBuffer, ref flag, allowUTF8);
			}
			for (int i = 0; i < fragments.Count; i++)
			{
				MimeString mimeString = fragments[i];
				int num2 = 0;
				int num3 = 0;
				byte[] data = mimeString.GetData(out num2, out num3);
				if ((mimeString.Mask & inputMask) != 0U)
				{
					do
					{
						int valueInChars = 0;
						int num4 = MimeScan.FindNextOf(token, data, num2, num3, out valueInChars, allowUTF8);
						if (num4 > 0)
						{
							num += Header.WriteToken(data, num2, new MimeStringLength(valueInChars, num4), stream, ref currentLineLength, ref lineBuffer, ref flag, allowUTF8);
							num2 += num4;
							num3 -= num4;
						}
						if (num3 != 0)
						{
							byte b = data[num2];
							if ((b == 34 || b == 92) && (mimeString.Mask & 3758096383U) != 0U)
							{
								num += Header.WriteToken(new byte[]
								{
									92,
									data[num2]
								}, 0, new MimeStringLength(2), stream, ref currentLineLength, ref lineBuffer, ref flag, allowUTF8);
								num2++;
								num3--;
							}
							else
							{
								num += Header.WriteToken(new byte[]
								{
									data[num2]
								}, 0, new MimeStringLength(1), stream, ref currentLineLength, ref lineBuffer, ref flag, allowUTF8);
								num2++;
								num3--;
							}
						}
					}
					while (num3 != 0);
				}
			}
			if (quoteOutput)
			{
				num += Header.WriteToken(Header.DoubleQuote, 0, new MimeStringLength(1), stream, ref currentLineLength, ref lineBuffer, ref flag, allowUTF8);
			}
			if (lastLineReserve > 0)
			{
				num += Header.WriteToken(null, 0, new MimeStringLength(lastLineReserve), stream, ref currentLineLength, ref lineBuffer, ref flag, allowUTF8);
			}
			if (lineBuffer.Length.InBytes > 0)
			{
				stream.Write(lineBuffer.Bytes, 0, lineBuffer.Length.InBytes);
				num += (long)lineBuffer.Length.InBytes;
				currentLineLength.IncrementBy(lineBuffer.Length);
			}
			return num;
		}

		internal static int WriteName(Stream stream, string name, ref byte[] scratchBuffer)
		{
			if (scratchBuffer == null || scratchBuffer.Length < name.Length)
			{
				scratchBuffer = new byte[Math.Max(998, name.Length)];
			}
			ByteString.StringToBytes(name, scratchBuffer, 0, false);
			stream.Write(scratchBuffer, 0, name.Length);
			stream.Write(MimeString.Colon, 0, MimeString.Colon.Length);
			return name.Length + MimeString.Colon.Length;
		}

		internal static HeaderNameIndex LookupName(string name)
		{
			if (name.Length >= 2 && name.Length <= 31)
			{
				int num = MimeData.HashName(name);
				int num2 = (int)MimeData.nameHashTable[num];
				if (num2 > 0)
				{
					for (;;)
					{
						string text = MimeData.headerNames[num2].name;
						if (name.Length == text.Length && name.Equals(text, StringComparison.OrdinalIgnoreCase))
						{
							break;
						}
						num2++;
						if ((int)MimeData.headerNames[num2].hash != num)
						{
							return HeaderNameIndex.Unknown;
						}
					}
					return (HeaderNameIndex)num2;
				}
			}
			return HeaderNameIndex.Unknown;
		}

		internal static HeaderNameIndex LookupName(byte[] nameBuffer, int offset, int length)
		{
			if (length >= 2 && length <= 31)
			{
				int num = MimeData.HashName(nameBuffer, offset, length);
				int num2 = (int)MimeData.nameHashTable[num];
				if (num2 > 0)
				{
					for (;;)
					{
						string str = MimeData.headerNames[num2].name;
						if (ByteString.EqualsI(str, nameBuffer, offset, length, false))
						{
							break;
						}
						num2++;
						if ((int)MimeData.headerNames[num2].hash != num)
						{
							return HeaderNameIndex.Unknown;
						}
					}
					return (HeaderNameIndex)num2;
				}
			}
			return HeaderNameIndex.Unknown;
		}

		internal static string NormalizeString(string value)
		{
			if (value.Length >= 2 && value.Length <= 32)
			{
				int num = MimeData.HashValue(value);
				int num2 = MimeData.valueHashTable[num];
				if (num2 > 0)
				{
					string value2;
					for (;;)
					{
						value2 = MimeData.values[num2].value;
						if (value.Length == value2.Length && value.Equals(value2, StringComparison.OrdinalIgnoreCase))
						{
							break;
						}
						num2++;
						if ((int)MimeData.values[num2].hash != num)
						{
							goto IL_68;
						}
					}
					return value2;
				}
			}
			IL_68:
			return value.ToLowerInvariant();
		}

		internal static string NormalizeString(string value, int offset, int length)
		{
			if (length >= 2 && length <= 32)
			{
				int num = MimeData.HashValue(value, offset, length);
				int num2 = MimeData.valueHashTable[num];
				if (num2 > 0)
				{
					string value2;
					for (;;)
					{
						value2 = MimeData.values[num2].value;
						if (length == value2.Length && string.Compare(value2, 0, value, offset, length, StringComparison.OrdinalIgnoreCase) == 0)
						{
							break;
						}
						num2++;
						if ((int)MimeData.values[num2].hash != num)
						{
							goto IL_5E;
						}
					}
					return value2;
				}
			}
			IL_5E:
			return value.Substring(offset, length).ToLowerInvariant();
		}

		internal static string NormalizeString(byte[] value, int offset, int length, bool allowUTF8)
		{
			if (length >= 2 && length <= 32)
			{
				int num = MimeData.HashValue(value, offset, length);
				int num2 = MimeData.valueHashTable[num];
				if (num2 > 0)
				{
					string value2;
					for (;;)
					{
						value2 = MimeData.values[num2].value;
						if (ByteString.EqualsI(value2, value, offset, length, allowUTF8))
						{
							break;
						}
						num2++;
						if ((int)MimeData.values[num2].hash != num)
						{
							goto IL_54;
						}
					}
					return value2;
				}
			}
			IL_54:
			return ByteString.BytesToString(value, offset, length, allowUTF8).ToLowerInvariant();
		}

		internal string GetRawValue(bool allowUTF8)
		{
			byte[] rawValue = this.RawValue;
			if (rawValue == null || rawValue.Length == 0)
			{
				return string.Empty;
			}
			return ByteString.BytesToString(rawValue, allowUTF8);
		}

		internal void SetRawValue(string value, bool markDirty, bool allowUTF8)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.SetRawValue(null, markDirty);
				return;
			}
			byte[] array = ByteString.StringToBytes(value, allowUTF8);
			int num = ByteString.IndexOf(array, 10, 0, array.Length);
			if (num == -1)
			{
				this.SetRawValue(array, markDirty);
				return;
			}
			this.RawValueAboutToChange();
			this.lines = default(MimeStringList);
			int num2 = 0;
			do
			{
				int num3 = num;
				while (num3 > num2 && array[num3 - 1] == 13)
				{
					num3--;
				}
				if (num3 > num2)
				{
					this.lines.Append(new MimeString(array, num2, num3 - num2));
				}
				num2 = num + 1;
				num = ByteString.IndexOf(array, 10, num2, array.Length - num2);
			}
			while (num != -1);
			if (num2 != array.Length)
			{
				this.lines.Append(new MimeString(array, num2, array.Length - num2));
			}
			if (markDirty)
			{
				this.SetDirty();
			}
		}

		internal void SetRawValue(byte[] value, bool markDirty)
		{
			this.RawValueAboutToChange();
			if (value == null || value.Length == 0)
			{
				this.lines = default(MimeStringList);
			}
			else
			{
				this.lines = new MimeStringList(new MimeString(value));
			}
			if (markDirty)
			{
				this.SetDirty();
			}
		}

		internal void SetRawValue(MimeStringList newLines, bool markDirty)
		{
			this.RawValueAboutToChange();
			this.lines = newLines;
			if (markDirty)
			{
				this.SetDirty();
			}
		}

		internal virtual void RawValueAboutToChange()
		{
		}

		internal virtual void ForceParse()
		{
		}

		internal bool IsName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
		}

		internal bool MatchName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this.Name.Equals(name))
			{
				return true;
			}
			if (this.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				this.customName = name;
				return true;
			}
			return false;
		}

		internal override void SetDirty()
		{
			this.dirty = true;
			base.SetDirty();
		}

		internal long WriteName(Stream stream, ref byte[] scratchBuffer)
		{
			if (!this.IsDirty && this.IsProtected)
			{
				string text = string.IsNullOrEmpty(this.customName) ? this.Name : this.customName;
				return (long)Header.WriteName(stream, text, ref scratchBuffer);
			}
			return (long)Header.WriteName(stream, this.Name, ref scratchBuffer);
		}

		internal static long WriteLineEnd(Stream stream, ref MimeStringLength currentLineLength)
		{
			long num = 0L;
			stream.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
			num += (long)MimeString.CrLf.Length;
			currentLineLength.SetAs(0);
			return num;
		}

		internal bool IsHeaderLineTooLong(long nameLength, out bool merge)
		{
			int num = 0;
			bool result = false;
			merge = false;
			for (int i = 0; i < this.lines.Count; i++)
			{
				int num2;
				int num3;
				byte[] data = this.lines[i].GetData(out num2, out num3);
				bool flag = MimeScan.IsLWSP(data[num2]);
				if (num != 0 && !flag)
				{
					result = true;
					merge = true;
				}
				num += num3;
				if (i == 0 && (long)num3 + nameLength + 1L > 998L)
				{
					result = true;
				}
			}
			return result;
		}

		internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			long num = this.WriteName(stream, ref scratchBuffer);
			currentLineLength.IncrementBy((int)num);
			bool flag = false;
			if (!this.IsDirty && this.RawLength != 0)
			{
				if (this.IsProtected)
				{
					num += Header.WriteLines(this.lines, stream);
					currentLineLength.SetAs(0);
					return num;
				}
				if (!this.IsHeaderLineTooLong(num, out flag))
				{
					num += Header.WriteLines(this.lines, stream);
					currentLineLength.SetAs(0);
					return num;
				}
			}
			MimeStringList fragments = this.lines;
			if (flag)
			{
				fragments = Header.MergeLines(fragments);
			}
			num += Header.QuoteAndFold(stream, fragments, 4026531840U, false, fragments.Length > 0, encodingOptions.AllowUTF8, 0, ref currentLineLength, ref scratchBuffer);
			return num + Header.WriteLineEnd(stream, ref currentLineLength);
		}

		internal void AppendLine(MimeString line)
		{
			this.AppendLine(line, true);
		}

		internal virtual void AppendLine(MimeString line, bool markDirty)
		{
			this.RawValueAboutToChange();
			this.lines.Append(line);
			if (markDirty)
			{
				this.SetDirty();
			}
		}

		internal static MimeStringList MergeLines(MimeStringList lines)
		{
			if (lines.Length != 0)
			{
				byte[] array = new byte[lines.Length];
				int num = 0;
				for (int i = 0; i < lines.Count; i++)
				{
					MimeString mimeString = lines[i];
					mimeString.CopyTo(array, num);
					num += mimeString.Length;
				}
				MimeStringList result = new MimeStringList(array);
				return result;
			}
			return lines;
		}

		internal const bool AllowUTF8Name = false;

		internal static readonly byte[] LineStartWhitespace = MimeString.Tabulation;

		internal static readonly byte[] DoubleQuote = new byte[]
		{
			34
		};

		internal static readonly byte[] Space = new byte[]
		{
			32
		};

		private string name;

		private string customName;

		private HeaderId headerId;

		private MimeStringList lines = default(MimeStringList);

		private bool dirty;

		private struct LineBuffer
		{
			public byte[] Bytes;

			public MimeStringLength Length;

			public MimeStringLength LengthTillLastLWSP;
		}
	}
}
