using System;
using System.Runtime.Serialization;
using System.Security;

namespace System.Text
{
	[Serializable]
	internal sealed class GB18030Encoding : DBCSCodePageEncoding, ISerializable
	{
		[SecurityCritical]
		internal GB18030Encoding() : base(54936, 936)
		{
		}

		[SecurityCritical]
		internal GB18030Encoding(SerializationInfo info, StreamingContext context) : base(54936, 936)
		{
			base.DeserializeEncoding(info, context);
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.SerializeEncoding(info, context);
		}

		[SecurityCritical]
		protected unsafe override void LoadManagedCodePage()
		{
			this.bFlagDataTable = false;
			this.iExtraBytes = 87032;
			base.LoadManagedCodePage();
			byte* ptr = (byte*)((void*)this.safeMemorySectionHandle.DangerousGetHandle());
			this.mapUnicodeTo4BytesFlags = ptr + 262144;
			this.map4BytesToUnicode = (char*)(ptr + 262144 + 8192);
			if (*this.mapCodePageCached == this.CodePage)
			{
				return;
			}
			char c = '\0';
			ushort num = 0;
			for (int i = 0; i < this.tableUnicodeToGBDiffs.Length; i++)
			{
				ushort num2 = this.tableUnicodeToGBDiffs[i];
				if ((num2 & 32768) != 0)
				{
					if (num2 > 36864 && num2 != 53670)
					{
						this.mapBytesToUnicode[num2] = c;
						this.mapUnicodeToBytes[(IntPtr)c] = num2;
						c += '\u0001';
					}
					else
					{
						c += (char)(num2 & 32767);
					}
				}
				else
				{
					while (num2 > 0)
					{
						this.map4BytesToUnicode[num] = c;
						this.mapUnicodeToBytes[(IntPtr)c] = num;
						byte* ptr2 = this.mapUnicodeTo4BytesFlags + c / '\b';
						*ptr2 |= (byte)(1 << (int)(c % '\b'));
						c += '\u0001';
						num += 1;
						num2 -= 1;
					}
				}
			}
			*this.mapCodePageCached = this.CodePage;
		}

		internal override void SetDefaultFallbacks()
		{
			this.encoderFallback = EncoderFallback.ReplacementFallback;
			this.decoderFallback = DecoderFallback.ReplacementFallback;
		}

		[SecurityCritical]
		internal unsafe bool Is4Byte(char charTest)
		{
			byte b = this.mapUnicodeTo4BytesFlags[charTest / '\b'];
			return b != 0 && ((int)b & 1 << (int)(charTest % '\b')) != 0;
		}

		[SecurityCritical]
		internal unsafe override int GetByteCount(char* chars, int count, EncoderNLS encoder)
		{
			return this.GetBytes(chars, count, null, 0, encoder);
		}

		[SecurityCritical]
		internal unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, EncoderNLS encoder)
		{
			char c = '\0';
			if (encoder != null)
			{
				c = encoder.charLeftOver;
			}
			Encoding.EncodingByteBuffer encodingByteBuffer = new Encoding.EncodingByteBuffer(this, encoder, bytes, byteCount, chars, charCount);
			for (;;)
			{
				if (encodingByteBuffer.MoreData)
				{
					char nextChar = encodingByteBuffer.GetNextChar();
					if (c != '\0')
					{
						if (!char.IsLowSurrogate(nextChar))
						{
							encodingByteBuffer.MovePrevious(false);
							if (encodingByteBuffer.Fallback(c))
							{
								c = '\0';
								continue;
							}
							c = '\0';
						}
						else
						{
							int num = (int)(((int)(c - '\ud800') << 10) + (nextChar - '\udc00'));
							byte b = (byte)(num % 10 + 48);
							num /= 10;
							byte b2 = (byte)(num % 126 + 129);
							num /= 126;
							byte b3 = (byte)(num % 10 + 48);
							num /= 10;
							c = '\0';
							if (encodingByteBuffer.AddByte((byte)(num + 144), b3, b2, b))
							{
								c = '\0';
								continue;
							}
							encodingByteBuffer.MovePrevious(false);
						}
					}
					else if (nextChar <= '\u007f')
					{
						if (encodingByteBuffer.AddByte((byte)nextChar))
						{
							continue;
						}
					}
					else
					{
						if (char.IsHighSurrogate(nextChar))
						{
							c = nextChar;
							continue;
						}
						if (char.IsLowSurrogate(nextChar))
						{
							if (encodingByteBuffer.Fallback(nextChar))
							{
								continue;
							}
						}
						else
						{
							ushort num2 = this.mapUnicodeToBytes[(IntPtr)nextChar];
							if (this.Is4Byte(nextChar))
							{
								byte b4 = (byte)(num2 % 10 + 48);
								num2 /= 10;
								byte b5 = (byte)(num2 % 126 + 129);
								num2 /= 126;
								byte b6 = (byte)(num2 % 10 + 48);
								num2 /= 10;
								if (encodingByteBuffer.AddByte((byte)(num2 + 129), b6, b5, b4))
								{
									continue;
								}
							}
							else if (encodingByteBuffer.AddByte((byte)(num2 >> 8), (byte)(num2 & 255)))
							{
								continue;
							}
						}
					}
				}
				if ((encoder != null && !encoder.MustFlush) || c <= '\0')
				{
					break;
				}
				encodingByteBuffer.Fallback(c);
				c = '\0';
			}
			if (encoder != null)
			{
				if (bytes != null)
				{
					encoder.charLeftOver = c;
				}
				encoder.m_charsUsed = encodingByteBuffer.CharsUsed;
			}
			return encodingByteBuffer.Count;
		}

		internal bool IsGBLeadByte(short ch)
		{
			return ch >= 129 && ch <= 254;
		}

		internal bool IsGBTwoByteTrailing(short ch)
		{
			return (ch >= 64 && ch <= 126) || (ch >= 128 && ch <= 254);
		}

		internal bool IsGBFourByteTrailing(short ch)
		{
			return ch >= 48 && ch <= 57;
		}

		internal int GetFourBytesOffset(short offset1, short offset2, short offset3, short offset4)
		{
			return (int)((offset1 - 129) * 10 * 126 * 10 + (offset2 - 48) * 126 * 10 + (offset3 - 129) * 10 + offset4 - 48);
		}

		[SecurityCritical]
		internal unsafe override int GetCharCount(byte* bytes, int count, DecoderNLS baseDecoder)
		{
			return this.GetChars(bytes, count, null, 0, baseDecoder);
		}

		[SecurityCritical]
		internal unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount, DecoderNLS baseDecoder)
		{
			GB18030Encoding.GB18030Decoder gb18030Decoder = (GB18030Encoding.GB18030Decoder)baseDecoder;
			Encoding.EncodingCharBuffer encodingCharBuffer = new Encoding.EncodingCharBuffer(this, gb18030Decoder, chars, charCount, bytes, byteCount);
			short num = -1;
			short num2 = -1;
			short num3 = -1;
			short num4 = -1;
			if (gb18030Decoder != null && gb18030Decoder.bLeftOver1 != -1)
			{
				num = gb18030Decoder.bLeftOver1;
				num2 = gb18030Decoder.bLeftOver2;
				num3 = gb18030Decoder.bLeftOver3;
				num4 = gb18030Decoder.bLeftOver4;
				while (num != -1)
				{
					if (!this.IsGBLeadByte(num))
					{
						if (num <= 127)
						{
							if (!encodingCharBuffer.AddChar((char)num))
							{
								break;
							}
						}
						else if (!encodingCharBuffer.Fallback((byte)num))
						{
							break;
						}
						num = num2;
						num2 = num3;
						num3 = num4;
						num4 = -1;
					}
					else
					{
						while (num2 == -1 || (this.IsGBFourByteTrailing(num2) && num4 == -1))
						{
							if (!encodingCharBuffer.MoreData)
							{
								if (!gb18030Decoder.MustFlush)
								{
									if (chars != null)
									{
										gb18030Decoder.bLeftOver1 = num;
										gb18030Decoder.bLeftOver2 = num2;
										gb18030Decoder.bLeftOver3 = num3;
										gb18030Decoder.bLeftOver4 = num4;
									}
									gb18030Decoder.m_bytesUsed = encodingCharBuffer.BytesUsed;
									return encodingCharBuffer.Count;
								}
								break;
							}
							else if (num2 == -1)
							{
								num2 = (short)encodingCharBuffer.GetNextByte();
							}
							else if (num3 == -1)
							{
								num3 = (short)encodingCharBuffer.GetNextByte();
							}
							else
							{
								num4 = (short)encodingCharBuffer.GetNextByte();
							}
						}
						if (this.IsGBTwoByteTrailing(num2))
						{
							int num5 = (int)num << 8;
							num5 |= (int)((byte)num2);
							if (!encodingCharBuffer.AddChar(this.mapBytesToUnicode[num5], 2))
							{
								break;
							}
							num = -1;
							num2 = -1;
						}
						else if (this.IsGBFourByteTrailing(num2) && this.IsGBLeadByte(num3) && this.IsGBFourByteTrailing(num4))
						{
							int num6 = this.GetFourBytesOffset(num, num2, num3, num4);
							if (num6 <= 39419)
							{
								if (!encodingCharBuffer.AddChar(this.map4BytesToUnicode[num6], 4))
								{
									break;
								}
							}
							else if (num6 >= 189000 && num6 <= 1237575)
							{
								num6 -= 189000;
								if (!encodingCharBuffer.AddChar((char)(55296 + num6 / 1024), (char)(56320 + num6 % 1024), 4))
								{
									break;
								}
							}
							else if (!encodingCharBuffer.Fallback((byte)num, (byte)num2, (byte)num3, (byte)num4))
							{
								break;
							}
							num = -1;
							num2 = -1;
							num3 = -1;
							num4 = -1;
						}
						else
						{
							if (!encodingCharBuffer.Fallback((byte)num))
							{
								break;
							}
							num = num2;
							num2 = num3;
							num3 = num4;
							num4 = -1;
						}
					}
				}
			}
			while (encodingCharBuffer.MoreData)
			{
				byte nextByte = encodingCharBuffer.GetNextByte();
				if (nextByte <= 127)
				{
					if (!encodingCharBuffer.AddChar((char)nextByte))
					{
						break;
					}
				}
				else if (this.IsGBLeadByte((short)nextByte))
				{
					if (encodingCharBuffer.MoreData)
					{
						byte nextByte2 = encodingCharBuffer.GetNextByte();
						if (this.IsGBTwoByteTrailing((short)nextByte2))
						{
							int num7 = (int)nextByte << 8;
							num7 |= (int)nextByte2;
							if (!encodingCharBuffer.AddChar(this.mapBytesToUnicode[num7], 2))
							{
								break;
							}
						}
						else if (this.IsGBFourByteTrailing((short)nextByte2))
						{
							if (encodingCharBuffer.EvenMoreData(2))
							{
								byte nextByte3 = encodingCharBuffer.GetNextByte();
								byte nextByte4 = encodingCharBuffer.GetNextByte();
								if (this.IsGBLeadByte((short)nextByte3) && this.IsGBFourByteTrailing((short)nextByte4))
								{
									int num8 = this.GetFourBytesOffset((short)nextByte, (short)nextByte2, (short)nextByte3, (short)nextByte4);
									if (num8 <= 39419)
									{
										if (!encodingCharBuffer.AddChar(this.map4BytesToUnicode[num8], 4))
										{
											break;
										}
									}
									else if (num8 >= 189000 && num8 <= 1237575)
									{
										num8 -= 189000;
										if (!encodingCharBuffer.AddChar((char)(55296 + num8 / 1024), (char)(56320 + num8 % 1024), 4))
										{
											break;
										}
									}
									else if (!encodingCharBuffer.Fallback(nextByte, nextByte2, nextByte3, nextByte4))
									{
										break;
									}
								}
								else
								{
									encodingCharBuffer.AdjustBytes(-3);
									if (!encodingCharBuffer.Fallback(nextByte))
									{
										break;
									}
								}
							}
							else if (gb18030Decoder != null && !gb18030Decoder.MustFlush)
							{
								if (chars != null)
								{
									num = (short)nextByte;
									num2 = (short)nextByte2;
									if (encodingCharBuffer.MoreData)
									{
										num3 = (short)encodingCharBuffer.GetNextByte();
									}
									else
									{
										num3 = -1;
									}
									num4 = -1;
									break;
								}
								break;
							}
							else if (!encodingCharBuffer.Fallback(nextByte, nextByte2))
							{
								break;
							}
						}
						else
						{
							encodingCharBuffer.AdjustBytes(-1);
							if (!encodingCharBuffer.Fallback(nextByte))
							{
								break;
							}
						}
					}
					else if (gb18030Decoder != null && !gb18030Decoder.MustFlush)
					{
						if (chars != null)
						{
							num = (short)nextByte;
							num2 = -1;
							num3 = -1;
							num4 = -1;
							break;
						}
						break;
					}
					else if (!encodingCharBuffer.Fallback(nextByte))
					{
						break;
					}
				}
				else if (!encodingCharBuffer.Fallback(nextByte))
				{
					break;
				}
			}
			if (gb18030Decoder != null)
			{
				if (chars != null)
				{
					gb18030Decoder.bLeftOver1 = num;
					gb18030Decoder.bLeftOver2 = num2;
					gb18030Decoder.bLeftOver3 = num3;
					gb18030Decoder.bLeftOver4 = num4;
				}
				gb18030Decoder.m_bytesUsed = encodingCharBuffer.BytesUsed;
			}
			return encodingCharBuffer.Count;
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			long num = (long)charCount + 1L;
			if (base.EncoderFallback.MaxCharCount > 1)
			{
				num *= (long)base.EncoderFallback.MaxCharCount;
			}
			num *= 4L;
			if (num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("charCount", Environment.GetResourceString("ArgumentOutOfRange_GetByteCountOverflow"));
			}
			return (int)num;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			long num = (long)byteCount + 3L;
			if (base.DecoderFallback.MaxCharCount > 1)
			{
				num *= (long)base.DecoderFallback.MaxCharCount;
			}
			if (num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("byteCount", Environment.GetResourceString("ArgumentOutOfRange_GetCharCountOverflow"));
			}
			return (int)num;
		}

		public override Decoder GetDecoder()
		{
			return new GB18030Encoding.GB18030Decoder(this);
		}

		private const int GBLast4ByteCode = 39419;

		[SecurityCritical]
		[NonSerialized]
		internal unsafe char* map4BytesToUnicode = null;

		[SecurityCritical]
		[NonSerialized]
		internal unsafe byte* mapUnicodeTo4BytesFlags = null;

		private const int GB18030 = 54936;

		private const int GBSurrogateOffset = 189000;

		private const int GBLastSurrogateOffset = 1237575;

		private readonly ushort[] tableUnicodeToGBDiffs = new ushort[]
		{
			32896,
			36,
			32769,
			2,
			32770,
			7,
			32770,
			5,
			32769,
			31,
			32769,
			8,
			32770,
			6,
			32771,
			1,
			32770,
			4,
			32770,
			3,
			32769,
			1,
			32770,
			1,
			32769,
			4,
			32769,
			17,
			32769,
			7,
			32769,
			15,
			32769,
			24,
			32769,
			3,
			32769,
			4,
			32769,
			29,
			32769,
			98,
			32769,
			1,
			32769,
			1,
			32769,
			1,
			32769,
			1,
			32769,
			1,
			32769,
			1,
			32769,
			1,
			32769,
			28,
			43199,
			87,
			32769,
			15,
			32769,
			101,
			32769,
			1,
			32771,
			13,
			32769,
			183,
			32785,
			1,
			32775,
			7,
			32785,
			1,
			32775,
			55,
			32769,
			14,
			32832,
			1,
			32769,
			7102,
			32769,
			2,
			32772,
			1,
			32770,
			2,
			32770,
			7,
			32770,
			9,
			32769,
			1,
			32770,
			1,
			32769,
			5,
			32769,
			112,
			41699,
			86,
			32769,
			1,
			32769,
			3,
			32769,
			12,
			32769,
			10,
			32769,
			62,
			32780,
			4,
			32778,
			22,
			32772,
			2,
			32772,
			110,
			32769,
			6,
			32769,
			1,
			32769,
			3,
			32769,
			4,
			32769,
			2,
			32772,
			2,
			32769,
			1,
			32769,
			1,
			32773,
			2,
			32769,
			5,
			32772,
			5,
			32769,
			10,
			32769,
			3,
			32769,
			5,
			32769,
			13,
			32770,
			2,
			32772,
			6,
			32770,
			37,
			32769,
			3,
			32769,
			11,
			32769,
			25,
			32769,
			82,
			32769,
			333,
			32778,
			10,
			32808,
			100,
			32844,
			4,
			32804,
			13,
			32783,
			3,
			32771,
			10,
			32770,
			16,
			32770,
			8,
			32770,
			8,
			32770,
			3,
			32769,
			2,
			32770,
			18,
			32772,
			31,
			32770,
			2,
			32769,
			54,
			32769,
			1,
			32769,
			2110,
			65104,
			2,
			65108,
			3,
			65111,
			2,
			65112,
			65117,
			10,
			65118,
			15,
			65131,
			2,
			65134,
			3,
			65137,
			4,
			65139,
			2,
			65140,
			65141,
			3,
			65145,
			14,
			65156,
			293,
			43402,
			43403,
			43404,
			43405,
			43406,
			43407,
			43408,
			43409,
			43410,
			43411,
			43412,
			43413,
			4,
			32772,
			1,
			32787,
			5,
			32770,
			2,
			32777,
			20,
			43401,
			2,
			32851,
			7,
			32772,
			2,
			32854,
			5,
			32771,
			6,
			32805,
			246,
			32778,
			7,
			32769,
			113,
			32769,
			234,
			32770,
			12,
			32771,
			2,
			32769,
			34,
			32769,
			9,
			32769,
			2,
			32770,
			2,
			32769,
			113,
			65110,
			43,
			65109,
			298,
			65114,
			111,
			65116,
			11,
			65115,
			765,
			65120,
			85,
			65119,
			96,
			65122,
			65125,
			14,
			65123,
			147,
			65124,
			218,
			65128,
			287,
			65129,
			113,
			65130,
			885,
			65135,
			264,
			65136,
			471,
			65138,
			116,
			65144,
			4,
			65143,
			43,
			65146,
			248,
			65147,
			373,
			65149,
			20,
			65148,
			193,
			65152,
			5,
			65153,
			82,
			65154,
			16,
			65155,
			441,
			65157,
			50,
			65158,
			2,
			65159,
			4,
			65160,
			65161,
			1,
			65162,
			65163,
			20,
			65165,
			3,
			65164,
			22,
			65167,
			65166,
			703,
			65174,
			39,
			65171,
			65172,
			65173,
			65175,
			65170,
			111,
			65176,
			65177,
			65178,
			65179,
			65180,
			65181,
			65182,
			148,
			65183,
			81,
			53670,
			14426,
			36716,
			1,
			32859,
			1,
			32798,
			13,
			32801,
			1,
			32771,
			5,
			32769,
			7,
			32769,
			4,
			32770,
			4,
			32770,
			8,
			32769,
			7,
			32769,
			16,
			32770,
			14,
			32769,
			4295,
			32769,
			76,
			32769,
			27,
			32769,
			81,
			32769,
			9,
			32769,
			26,
			32772,
			1,
			32769,
			1,
			32770,
			3,
			32769,
			6,
			32771,
			1,
			32770,
			2,
			32771,
			1030,
			32770,
			1,
			32786,
			4,
			32778,
			1,
			32772,
			1,
			32782,
			1,
			32772,
			149,
			32862,
			129,
			32774,
			26
		};

		[Serializable]
		internal sealed class GB18030Decoder : DecoderNLS, ISerializable
		{
			internal GB18030Decoder(EncodingNLS encoding) : base(encoding)
			{
			}

			[SecurityCritical]
			internal GB18030Decoder(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				try
				{
					this.m_encoding = (Encoding)info.GetValue("m_encoding", typeof(Encoding));
					this.m_fallback = (DecoderFallback)info.GetValue("m_fallback", typeof(DecoderFallback));
					this.bLeftOver1 = (short)info.GetValue("bLeftOver1", typeof(short));
					this.bLeftOver2 = (short)info.GetValue("bLeftOver2", typeof(short));
					this.bLeftOver3 = (short)info.GetValue("bLeftOver3", typeof(short));
					this.bLeftOver4 = (short)info.GetValue("bLeftOver4", typeof(short));
				}
				catch (SerializationException)
				{
					this.m_encoding = new GB18030Encoding();
				}
			}

			[SecurityCritical]
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("m_encoding", this.m_encoding);
				info.AddValue("m_fallback", this.m_fallback);
				info.AddValue("bLeftOver1", this.bLeftOver1);
				info.AddValue("bLeftOver2", this.bLeftOver2);
				info.AddValue("bLeftOver3", this.bLeftOver3);
				info.AddValue("bLeftOver4", this.bLeftOver4);
				info.AddValue("m_leftOverBytes", 0);
				info.AddValue("leftOver", new byte[8]);
			}

			public override void Reset()
			{
				this.bLeftOver1 = -1;
				this.bLeftOver2 = -1;
				this.bLeftOver3 = -1;
				this.bLeftOver4 = -1;
				if (this.m_fallbackBuffer != null)
				{
					this.m_fallbackBuffer.Reset();
				}
			}

			internal override bool HasState
			{
				get
				{
					return this.bLeftOver1 >= 0;
				}
			}

			internal short bLeftOver1 = -1;

			internal short bLeftOver2 = -1;

			internal short bLeftOver3 = -1;

			internal short bLeftOver4 = -1;
		}
	}
}
