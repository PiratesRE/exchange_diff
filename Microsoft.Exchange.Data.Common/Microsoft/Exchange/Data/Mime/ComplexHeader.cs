using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime
{
	public abstract class ComplexHeader : Header, IEnumerable<MimeParameter>, IEnumerable
	{
		internal ComplexHeader(string name, HeaderId headerId) : base(name, headerId)
		{
		}

		public MimeParameter this[string name]
		{
			get
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				for (MimeNode mimeNode = base.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
				{
					MimeParameter mimeParameter = mimeNode as MimeParameter;
					if (mimeParameter.IsName(name))
					{
						return mimeParameter;
					}
				}
				return null;
			}
		}

		public new MimeNode.Enumerator<MimeParameter> GetEnumerator()
		{
			return new MimeNode.Enumerator<MimeParameter>(this);
		}

		IEnumerator<MimeParameter> IEnumerable<MimeParameter>.GetEnumerator()
		{
			return new MimeNode.Enumerator<MimeParameter>(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new MimeNode.Enumerator<MimeParameter>(this);
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
			num += this.WriteValue(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
			for (MimeNode mimeNode = base.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
			{
				stream.Write(MimeString.Semicolon, 0, MimeString.Semicolon.Length);
				num += (long)MimeString.Semicolon.Length;
				currentLineLength.IncrementBy(MimeString.Semicolon.Length);
				num += mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
			}
			return num + Header.WriteLineEnd(stream, ref currentLineLength);
		}

		internal virtual long WriteValue(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
		{
			long num = 0L;
			string value = this.Value;
			if (value != null)
			{
				int num2 = ByteString.StringToBytesCount(value, encodingOptions.AllowUTF8) + 1;
				if (scratchBuffer == null || scratchBuffer.Length < num2)
				{
					scratchBuffer = new byte[Math.Max(998, num2)];
				}
				scratchBuffer[0] = 32;
				int num3 = ByteString.StringToBytes(value, scratchBuffer, 1, encodingOptions.AllowUTF8);
				stream.Write(scratchBuffer, 0, num3 + 1);
				num += (long)(num3 + 1);
				currentLineLength.IncrementBy(value.Length + 1, num3 + 1);
			}
			return num;
		}

		internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
		{
			MimeParameter mimeParameter = newChild as MimeParameter;
			if (mimeParameter == null)
			{
				throw new ArgumentException(Strings.NewChildNotMimeParameter, "newChild");
			}
			MimeParameter mimeParameter2 = this[mimeParameter.Name];
			if (mimeParameter2 != null)
			{
				if (mimeParameter2 == refChild)
				{
					refChild = mimeParameter2.PreviousSibling;
				}
				base.InternalRemoveChild(mimeParameter2);
			}
			return refChild;
		}

		protected void Parse()
		{
			this.parsed = true;
			DecodingOptions headerDecodingOptions = base.GetHeaderDecodingOptions();
			ValueParser parser = new ValueParser(base.Lines, headerDecodingOptions.AllowUTF8);
			this.ParseValue(parser, true);
			this.ParseParameters(ref parser, headerDecodingOptions, int.MaxValue, int.MaxValue);
		}

		internal override void ForceParse()
		{
			if (!this.parsed)
			{
				this.Parse();
			}
		}

		internal override void CheckChildrenLimit(int countLimit, int bytesLimit)
		{
			DecodingOptions headerDecodingOptions = base.GetHeaderDecodingOptions();
			ValueParser parser = new ValueParser(base.Lines, headerDecodingOptions.AllowUTF8);
			this.ParseValue(parser, false);
			this.ParseParameters(ref parser, headerDecodingOptions, countLimit, bytesLimit);
		}

		internal override void RemoveAllUnparsed()
		{
			this.parsed = true;
		}

		internal override MimeNode ParseNextChild()
		{
			if (this.parsed)
			{
				return null;
			}
			MimeNode internalLastChild = base.InternalLastChild;
			this.Parse();
			if (internalLastChild != null)
			{
				return internalLastChild.NextSibling;
			}
			return base.FirstChild;
		}

		internal abstract void ParseValue(ValueParser parser, bool storeValue);

		internal void ParseParameters(ref ValueParser parser, DecodingOptions decodingOptions, int countLimit, int bytesLimit)
		{
			MimeStringList mimeStringList = default(MimeStringList);
			MimeStringList mimeStringList2 = default(MimeStringList);
			bool flag = false;
			int num = 0;
			bool flag2 = DecodingFlags.None != (DecodingFlags.Rfc2231 & decodingOptions.DecodingFlags);
			for (;;)
			{
				parser.ParseCFWS(false, ref mimeStringList, this.handleISO2022);
				byte b = parser.ParseGet();
				if (b != 59)
				{
					if (b == 0)
					{
						break;
					}
					parser.ParseUnget();
					parser.ParseSkipToNextDelimiterByte(59);
				}
				else
				{
					parser.ParseCFWS(false, ref mimeStringList, this.handleISO2022);
					MimeString mimeString = parser.ParseToken();
					if (mimeString.Length != 0 && mimeString.Length < 128)
					{
						parser.ParseCFWS(false, ref mimeStringList, this.handleISO2022);
						b = parser.ParseGet();
						if (b != 61)
						{
							if (b == 0)
							{
								return;
							}
							parser.ParseUnget();
						}
						else
						{
							parser.ParseCFWS(false, ref mimeStringList, this.handleISO2022);
							parser.ParseParameterValue(ref mimeStringList2, ref flag, this.handleISO2022);
							if (2147483647 != countLimit || 2147483647 != bytesLimit)
							{
								if (++num > countLimit)
								{
									goto Block_8;
								}
								if (mimeStringList2.Length > bytesLimit)
								{
									goto Block_9;
								}
							}
							else
							{
								bool valueEncoded = false;
								int num2 = -1;
								if (flag2)
								{
									int num3 = ByteString.IndexOf(mimeString.Data, 42, mimeString.Offset, mimeString.Length);
									if (num3 > 0)
									{
										int num4 = mimeString.Offset + mimeString.Length;
										int num5 = num3 + 1;
										num2 = 0;
										while (num5 != num4 && mimeString.Data[num5] >= 48 && mimeString.Data[num5] <= 57)
										{
											num2 = num2 * 10 + (int)(mimeString.Data[num5] - 48);
											if (10000 < num2)
											{
												num2 = -1;
												break;
											}
											num5++;
										}
										if (-1 != num2)
										{
											bool flag3 = 42 == mimeString.Data[num4 - 1];
											if (num5 < num4 - 1 || (num5 < num4 && !flag3))
											{
												num2 = -1;
											}
											else
											{
												valueEncoded = flag3;
												mimeString.TrimRight(num4 - num3);
											}
										}
									}
								}
								string text = Header.NormalizeString(mimeString.Data, mimeString.Offset, mimeString.Length, false);
								MimeParameter mimeParameter = new MimeParameter(text);
								mimeParameter.AppendValue(ref mimeStringList2);
								mimeParameter.SegmentNumber = num2;
								mimeParameter.ValueEncoded = valueEncoded;
								MimeNode mimeNode;
								MimeNode nextSibling;
								for (mimeNode = base.FirstChild; mimeNode != null; mimeNode = nextSibling)
								{
									nextSibling = mimeNode.NextSibling;
									MimeParameter mimeParameter2 = mimeNode as MimeParameter;
									if (mimeParameter2 != null && mimeParameter2.Name == text)
									{
										break;
									}
								}
								if (0 >= num2)
								{
									if (mimeNode != null)
									{
										mimeParameter.AllowAppend = true;
										for (MimeNode mimeNode2 = mimeNode.FirstChild; mimeNode2 != null; mimeNode2 = nextSibling)
										{
											nextSibling = mimeNode2.NextSibling;
											if (mimeNode2 is MimeParameter)
											{
												mimeNode.RemoveChild(mimeNode2);
												mimeParameter.InternalAppendChild(mimeNode2);
											}
										}
										mimeParameter.AllowAppend = false;
										base.InternalRemoveChild(mimeNode);
									}
									base.InternalAppendChild(mimeParameter);
								}
								else
								{
									if (mimeNode == null)
									{
										MimeParameter mimeParameter3 = new MimeParameter(text);
										mimeParameter3.SegmentNumber = 0;
										base.InternalAppendChild(mimeParameter3);
										mimeNode = mimeParameter3;
									}
									MimeNode mimeNode3;
									MimeNode previousSibling;
									for (mimeNode3 = mimeNode.LastChild; mimeNode3 != null; mimeNode3 = previousSibling)
									{
										previousSibling = mimeNode3.PreviousSibling;
										MimeParameter mimeParameter4 = mimeNode3 as MimeParameter;
										if (mimeParameter4 != null && mimeParameter4.SegmentNumber <= num2)
										{
											break;
										}
									}
									MimeParameter mimeParameter5 = (MimeParameter)mimeNode;
									mimeParameter5.AllowAppend = true;
									mimeParameter5.InternalInsertAfter(mimeParameter, mimeNode3);
									mimeParameter5.AllowAppend = false;
								}
							}
						}
					}
				}
				if (b == 0)
				{
					return;
				}
			}
			return;
			Block_8:
			throw new MimeException(Strings.TooManyParameters(num, countLimit));
			Block_9:
			throw new MimeException(Strings.TooManyTextValueBytes(mimeStringList2.Length, bytesLimit));
		}

		protected bool parsed;

		protected bool handleISO2022 = true;
	}
}
