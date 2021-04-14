using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInDataStreamBuilder : SmtpInStreamBuilderBase
	{
		public SmtpInDataStreamBuilder()
		{
			base.EohPos = -1L;
		}

		public ParserState State
		{
			get
			{
				return this.parserState;
			}
		}

		public override bool IsEodSeen
		{
			get
			{
				return this.parserState == ParserState.EOD;
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.parserState = ParserState.LF1;
		}

		public override bool Write(byte[] data, int offset, int numBytes, out int numBytesConsumed)
		{
			bool result = false;
			int num = offset;
			int i = offset;
			int num2 = offset + numBytes;
			if (this.parserState > ParserState.EOHCR2)
			{
				throw new InvalidOperationException("SmtpInDataParser is in an unknown state");
			}
			if (this.parserState == ParserState.EOD)
			{
				throw new InvalidOperationException("SmtpInDataParser received data after EOD");
			}
			while (i < num2)
			{
				switch (this.parserState)
				{
				case ParserState.NONE:
				{
					int num3 = Array.IndexOf<byte>(data, 13, i, num2 - i);
					if (num3 >= 0)
					{
						this.parserState = ParserState.CR1;
						i = num3 + 1;
						continue;
					}
					i = num2;
					continue;
				}
				case ParserState.CR1:
					if (data[i] == 10)
					{
						this.parserState = ParserState.LF1;
					}
					else if (data[i] == 13)
					{
						this.parserState = ParserState.CR1;
					}
					else
					{
						this.parserState = ParserState.NONE;
					}
					i++;
					continue;
				case ParserState.LF1:
					if (data[i] == 46)
					{
						this.parserState = ParserState.DOT;
					}
					else if (data[i] == 13)
					{
						if (base.EohPos != -1L)
						{
							this.parserState = ParserState.CR1;
						}
						else
						{
							this.parserState = ParserState.EOHCR2;
						}
					}
					else
					{
						this.parserState = ParserState.NONE;
					}
					i++;
					continue;
				case ParserState.DOT:
					if (data[i] == 13)
					{
						this.parserState = ParserState.CR2;
					}
					else if (!base.IsDiscardingData)
					{
						int num4 = i - offset - 1;
						if (num4 > 0)
						{
							base.Write(data, offset, num4);
						}
						this.parserState = ParserState.NONE;
						offset = i;
					}
					else
					{
						this.parserState = ParserState.NONE;
					}
					i++;
					continue;
				case ParserState.CR2:
					if (data[i] == 10)
					{
						this.parserState = ParserState.EOD;
						result = true;
						num2 = i;
					}
					else if (!base.IsDiscardingData)
					{
						int num5 = i - offset - 2;
						if (num5 > 0)
						{
							base.Write(data, offset, num5);
						}
						base.Write(SmtpInParser.EodSequence, 3, 1);
						if (data[i] == 13)
						{
							this.parserState = ParserState.CR1;
						}
						else
						{
							this.parserState = ParserState.NONE;
						}
						offset = i;
					}
					else
					{
						this.parserState = ParserState.NONE;
					}
					i++;
					continue;
				case ParserState.EOHCR2:
					if (data[i] == 10)
					{
						base.EohPos = base.TotalBytesRead + (long)i - (long)num - 1L;
						this.parserState = ParserState.LF1;
					}
					else if (data[i] == 13)
					{
						this.parserState = ParserState.CR1;
					}
					else
					{
						this.parserState = ParserState.NONE;
					}
					i++;
					continue;
				}
				throw new InvalidOperationException("SmtpInDataParser got into an unknown state");
			}
			numBytesConsumed = i - num;
			base.TotalBytesRead += (long)numBytesConsumed;
			if (!base.IsDiscardingData)
			{
				int num6 = i - offset;
				if (this.parserState >= ParserState.DOT && this.parserState <= ParserState.EOD)
				{
					num6 -= this.parserState - ParserState.DOT + 1;
				}
				if (num6 > 0)
				{
					base.Write(data, offset, num6);
				}
			}
			return result;
		}

		private ParserState parserState = ParserState.LF1;
	}
}
