using System;

namespace Microsoft.Exchange.Data.Globalization.Iso2022Jp
{
	internal class DecodeJisX0208_1983ToCp932 : DecodeToCp932
	{
		public override bool IsEscapeSequenceHandled(Escape escape)
		{
			return escape.Sequence == EscapeSequence.JisX0208_1978 || escape.Sequence == EscapeSequence.JisX0208_1983 || escape.Sequence == EscapeSequence.JisX0208_1990;
		}

		public override ValidationResult GetRunLength(byte[] dataIn, int offsetIn, int lengthIn, Escape escape, out int usedIn, out int usedOut)
		{
			usedIn = 0;
			usedOut = 0;
			bool flag = false;
			bool flag2 = false;
			int i = offsetIn;
			int num = 0;
			int num2 = 0;
			int limit = this.CalculateLoopCountLimit(lengthIn);
			byte b = 0;
			if (escape.IsValidEscapeSequence)
			{
				if (!this.IsEscapeSequenceHandled(escape))
				{
					throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
				}
				i += escape.BytesInCurrentBuffer;
				this.runBeganWithEscape = true;
				this.isKana = false;
				this.leftoverByte = 0;
			}
			while (i < offsetIn + lengthIn)
			{
				this.CheckLoopCount(ref num2, limit);
				uint num3;
				uint num4;
				int num5;
				if (this.leftoverByte != 0 && i == offsetIn)
				{
					num3 = (uint)this.leftoverByte;
					num4 = (uint)dataIn[i];
					num5 = 1;
					b = this.leftoverByte;
					this.leftoverByte = 0;
				}
				else if (i + 2 <= offsetIn + lengthIn)
				{
					num3 = (uint)dataIn[i];
					num4 = (uint)dataIn[i + 1];
					num5 = 2;
				}
				else
				{
					num3 = (uint)dataIn[i];
					if (num3 == 27U || (!this.runBeganWithEscape && (num3 == 14U || num3 == 15U)) || num3 == 0U)
					{
						flag2 = true;
						break;
					}
					if (this.isKana && num3 != 14U && num3 != 15U)
					{
						i++;
						num++;
						break;
					}
					break;
				}
				if (num3 == 27U)
				{
					flag2 = true;
					break;
				}
				if (num3 == 0U)
				{
					flag2 = true;
					break;
				}
				if (num4 == 27U)
				{
					if (num3 == 14U || num3 == 15U)
					{
						if (this.runBeganWithEscape)
						{
							i++;
						}
					}
					else
					{
						i++;
						if (this.isKana)
						{
							num++;
						}
					}
					flag2 = true;
					break;
				}
				if (num4 == 0U)
				{
					if (num3 == 14U || num3 == 15U)
					{
						if (this.runBeganWithEscape)
						{
							i++;
						}
					}
					else
					{
						i++;
						if (this.isKana)
						{
							num++;
						}
					}
					flag2 = true;
					break;
				}
				if (!this.runBeganWithEscape)
				{
					if (num3 == 14U || num3 == 15U || num4 == 14U || num4 == 15U)
					{
						flag2 = true;
						break;
					}
				}
				else if (num3 == 14U)
				{
					this.isKana = true;
					i += num5;
					if (num4 != 14U && num4 != 15U)
					{
						num++;
						continue;
					}
					continue;
				}
				else
				{
					if (num3 == 15U)
					{
						this.isKana = false;
						i += num5 - 1;
						continue;
					}
					if (num4 == 14U)
					{
						i += num5;
						num++;
						this.isKana = true;
						continue;
					}
					if (num4 == 15U)
					{
						this.isKana = false;
						i += num5;
						num++;
						continue;
					}
				}
				if (num3 >= 128U)
				{
					flag = true;
					if (!this.runBeganWithEscape)
					{
						break;
					}
					if (this.isRunContainsIbmExtension == JisX0208PairClass.Unrecognized)
					{
						JisX0208PairClass jisX0208PairClass = this.ClassifyPair(num3, num4);
						if (jisX0208PairClass == JisX0208PairClass.IbmExtension)
						{
							this.isRunContainsIbmExtension = JisX0208PairClass.IbmExtension;
						}
					}
				}
				i += num5;
				num += 2;
			}
			if (!flag2 && i + 1 == offsetIn + lengthIn)
			{
				i++;
			}
			usedIn = i - offsetIn;
			usedOut = num;
			if (b != 0)
			{
				this.leftoverByte = b;
			}
			if (!flag || this.runBeganWithEscape)
			{
				return ValidationResult.Valid;
			}
			return ValidationResult.Invalid;
		}

		public override void ConvertToCp932(byte[] dataIn, int offsetIn, int lengthIn, byte[] dataOut, int offsetOut, int lengthOut, bool flush, Escape escape, out int usedIn, out int usedOut, out bool complete)
		{
			usedIn = 0;
			usedOut = 0;
			complete = false;
			bool flag = false;
			int i = offsetIn;
			int num = offsetOut;
			int num2 = 0;
			int num3 = 0;
			int limit = this.CalculateLoopCountLimit(lengthIn);
			if (escape.IsValidEscapeSequence)
			{
				if (!this.IsEscapeSequenceHandled(escape))
				{
					throw new InvalidOperationException(string.Format("unhandled escape sequence: {0}", escape.Sequence));
				}
				i += escape.BytesInCurrentBuffer;
				this.isKana = false;
				this.leftoverByte = 0;
			}
			while (i < offsetIn + lengthIn)
			{
				this.CheckLoopCount(ref num3, limit);
				uint num4;
				uint num5;
				int num6;
				if (this.leftoverByte != 0)
				{
					if (i != offsetIn)
					{
						string message = string.Format("DecodeJisX0208_1983ToCp932.ConvertToCp932: leftover byte processed at offset {0}, should have been {1}", i, offsetIn);
						throw new InvalidOperationException(message);
					}
					num4 = (uint)this.leftoverByte;
					num5 = (uint)dataIn[i];
					num6 = 1;
					this.leftoverByte = 0;
				}
				else if (i + 2 <= offsetIn + lengthIn)
				{
					num4 = (uint)dataIn[i];
					num5 = (uint)dataIn[i + 1];
					num6 = 2;
				}
				else
				{
					num4 = (uint)dataIn[i];
					if (num4 == 27U || (!this.runBeganWithEscape && (num4 == 14U || num4 == 15U)) || num4 == 0U)
					{
						flag = true;
						break;
					}
					if (this.isKana && num4 != 14U && num4 != 15U)
					{
						dataOut[num++] = this.DecodeKana(num4);
						num2++;
						i++;
						break;
					}
					break;
				}
				if (num4 == 27U)
				{
					this.runBeganWithEscape = false;
					flag = true;
					break;
				}
				if (num4 == 0U)
				{
					flag = true;
					break;
				}
				if (num5 == 27U)
				{
					this.runBeganWithEscape = false;
					flag = true;
					i++;
					if (!this.isKana || num4 == 14U)
					{
						break;
					}
					if (num4 == 15U)
					{
						this.isKana = false;
						break;
					}
					dataOut[num++] = this.DecodeKana(num4);
					num2++;
					break;
				}
				else if (num5 == 0U)
				{
					flag = true;
					i++;
					if (!this.isKana || num4 == 14U)
					{
						break;
					}
					if (num4 == 15U)
					{
						this.isKana = false;
						break;
					}
					dataOut[num++] = this.DecodeKana(num4);
					num2++;
					break;
				}
				else
				{
					if (!this.runBeganWithEscape)
					{
						if (num4 == 14U || num4 == 15U || num5 == 14U || num5 == 15U)
						{
							flag = true;
							break;
						}
					}
					else if (num4 == 14U)
					{
						this.isKana = true;
						i += num6;
						switch (num5)
						{
						case 14U:
							continue;
						case 15U:
							this.isKana = false;
							continue;
						default:
							dataOut[num++] = this.DecodeKana(num5);
							num2++;
							continue;
						}
					}
					else
					{
						if (num4 == 15U)
						{
							this.isKana = false;
							i += num6 - 1;
							continue;
						}
						if (num5 == 14U)
						{
							i += num6;
							dataOut[num] = (this.isKana ? this.DecodeKana(num4) : 165);
							num++;
							num2++;
							this.isKana = true;
							continue;
						}
						if (num5 == 15U)
						{
							dataOut[num] = (this.isKana ? this.DecodeKana(num4) : 165);
							i += num6;
							num2++;
							num++;
							this.isKana = false;
							continue;
						}
					}
					if (num + 2 > offsetOut + lengthOut)
					{
						break;
					}
					if (this.isKana)
					{
						dataOut[num] = this.DecodeKana(num4);
						dataOut[num + 1] = this.DecodeKana(num5);
					}
					else if (num4 >= 128U)
					{
						switch (this.ClassifyPair(num4, num5))
						{
						case JisX0208PairClass.Unrecognized:
						case JisX0208PairClass.Cp932:
							goto IL_37C;
						case JisX0208PairClass.IbmExtension:
							break;
						default:
							throw new InvalidOperationException("unrecognized pair class, update DecodeJisX0208_1983.DecodeToCp932");
						case JisX0208PairClass.IbmExtension | JisX0208PairClass.Cp932:
							if (this.isRunContainsIbmExtension != JisX0208PairClass.IbmExtension)
							{
								goto IL_37C;
							}
							break;
						}
						this.isRunContainsIbmExtension = JisX0208PairClass.IbmExtension;
						byte b;
						byte b2;
						this.MapIbmExtensionToCp932(num4, num5, out b, out b2);
						dataOut[num] = b;
						dataOut[num + 1] = b2;
						goto IL_413;
						IL_37C:
						dataOut[num] = (byte)num4;
						dataOut[num + 1] = (byte)num5;
					}
					else
					{
						if ((num4 & 1U) == 1U)
						{
							num5 += 31U;
						}
						else
						{
							num5 += 125U;
						}
						if (num5 >= 127U)
						{
							num5 += 1U;
						}
						num4 = (num4 - 33U >> 1) + 129U;
						if (num4 > 159U)
						{
							num4 += 64U;
						}
						dataOut[num] = (byte)num4;
						dataOut[num + 1] = (byte)num5;
					}
					IL_413:
					i += num6;
					num += 2;
					num2 += 2;
				}
			}
			if (!flag && i + 1 == offsetIn + lengthIn)
			{
				this.leftoverByte = dataIn[i++];
			}
			usedIn = i - offsetIn;
			usedOut = num2;
			complete = (i == offsetIn + lengthIn);
		}

		private JisX0208PairClass ClassifyPair(uint high, uint low)
		{
			if ((high >= 147U && high < 151U && low > 32U && low < 127U) || (high == 151U && low > 32U && low < 45U))
			{
				if (low >= 64U && low != 127U && low < 253U)
				{
					return JisX0208PairClass.IbmExtension | JisX0208PairClass.Cp932;
				}
				return JisX0208PairClass.IbmExtension;
			}
			else
			{
				if (high < 240U && high > 128U && high != 160U && low >= 64U && low != 127U && low < 253U)
				{
					return JisX0208PairClass.Cp932;
				}
				return JisX0208PairClass.Unrecognized;
			}
		}

		private void MapIbmExtensionToCp932(uint highIn, uint lowIn, out byte highOut, out byte lowOut)
		{
			highOut = 0;
			lowOut = 0;
			switch (highIn)
			{
			case 147U:
				highOut = 250;
				lowOut = (byte)(lowIn + 31U);
				return;
			case 148U:
				highOut = 250;
				lowOut = (byte)(lowIn + 126U);
				return;
			case 149U:
				highOut = 251;
				lowOut = (byte)(lowIn + 31U);
				return;
			case 150U:
				highOut = 251;
				lowOut = (byte)(lowIn + 126U);
				return;
			case 151U:
				highOut = 252;
				lowOut = (byte)(lowIn + 31U);
				return;
			default:
			{
				string message = string.Format("ClassifyPair and MapIbmExtensionToCp932 disagree on {0},{1}", highIn.ToString("X2"), lowIn.ToString("X2"));
				throw new InvalidOperationException(message);
			}
			}
		}

		private byte DecodeKana(uint current)
		{
			if (!this.isKana || current < 33U || current > 95U)
			{
				return (byte)current;
			}
			return (byte)(current + 128U);
		}

		public override void Reset()
		{
			this.isKana = false;
			this.leftoverByte = 0;
			this.runBeganWithEscape = false;
			this.isRunContainsIbmExtension = JisX0208PairClass.Unrecognized;
		}

		public override char Abbreviation
		{
			get
			{
				return 'e';
			}
		}

		private bool isKana;

		private byte leftoverByte;

		private bool runBeganWithEscape;

		private JisX0208PairClass isRunContainsIbmExtension = JisX0208PairClass.Unrecognized;
	}
}
