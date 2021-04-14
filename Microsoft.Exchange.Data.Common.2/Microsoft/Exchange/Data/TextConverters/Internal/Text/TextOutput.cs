using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
	internal class TextOutput : IRestartable, IReusable, IFallback, IDisposable
	{
		public TextOutput(ConverterOutput output, bool lineWrapping, bool flowed, int wrapBeforePosition, int longestNonWrappedParagraph, ImageRenderingCallbackInternal imageRenderingCallback, bool fallbacks, bool htmlEscape, bool preserveSpace, Stream testTraceStream)
		{
			this.rfc2646 = flowed;
			this.lineWrapping = lineWrapping;
			this.wrapBeforePosition = wrapBeforePosition;
			this.longestNonWrappedParagraph = longestNonWrappedParagraph;
			if (!this.lineWrapping)
			{
				this.preserveTrailingSpace = preserveSpace;
				this.preserveTabulation = preserveSpace;
				this.preserveNbsp = preserveSpace;
			}
			this.output = output;
			this.fallbacks = fallbacks;
			this.htmlEscape = htmlEscape;
			this.imageRenderingCallback = imageRenderingCallback;
			this.wrapBuffer = new char[(this.longestNonWrappedParagraph + 1) * 5];
		}

		public bool OutputCodePageSameAsInput
		{
			get
			{
				return this.output is ConverterEncodingOutput && (this.output as ConverterEncodingOutput).CodePageSameAsInput;
			}
		}

		public Encoding OutputEncoding
		{
			set
			{
				if (this.output is ConverterEncodingOutput)
				{
					(this.output as ConverterEncodingOutput).Encoding = value;
					return;
				}
				throw new InvalidOperationException();
			}
		}

		public bool LineEmpty
		{
			get
			{
				return this.lineLength == 0 && this.tailSpace == 0;
			}
		}

		public bool ImageRenderingCallbackDefined
		{
			get
			{
				return this.imageRenderingCallback != null;
			}
		}

		public void OpenDocument()
		{
		}

		public void CloseDocument()
		{
			if (!this.anyNewlines)
			{
				this.output.Write("\r\n");
			}
			this.endParagraph = false;
		}

		public void SetQuotingLevel(int quotingLevel)
		{
			this.quotingLevel = Math.Min(quotingLevel, this.wrapBeforePosition / 2);
		}

		public void CloseParagraph()
		{
			if (this.lineLength != 0 || this.tailSpace != 0)
			{
				this.OutputNewLine();
			}
			this.endParagraph = true;
		}

		public void OutputNewLine()
		{
			if (this.lineWrapping)
			{
				this.FlushLine('\n');
				if (this.signaturePossible && this.lineLength == 2 && this.tailSpace == 1)
				{
					this.output.Write(' ');
					this.lineLength++;
				}
			}
			else if (this.preserveTrailingSpace && this.tailSpace != 0)
			{
				this.FlushTailSpace();
			}
			if (!this.endParagraph)
			{
				this.output.Write("\r\n");
				this.anyNewlines = true;
				this.linePosition += 2;
			}
			this.linePosition += this.lineLength;
			this.lineLength = 0;
			this.lineLengthBeforeSoftWrap = 0;
			this.flushedLength = 0;
			this.tailSpace = 0;
			this.breakOpportunity = 0;
			this.nextBreakOpportunity = 0;
			this.wrapped = false;
			this.seenSpace = false;
			this.signaturePossible = true;
		}

		public void OutputTabulation(int count)
		{
			if (this.preserveTabulation)
			{
				while (count != 0)
				{
					this.OutputNonspace("\t", TextMapping.Unicode);
					count--;
				}
				return;
			}
			int num = (this.lineLengthBeforeSoftWrap + this.lineLength + this.tailSpace) / 8 * 8 + 8 * count;
			count = num - (this.lineLengthBeforeSoftWrap + this.lineLength + this.tailSpace);
			this.OutputSpace(count);
		}

		public void OutputSpace(int count)
		{
			if (this.lineWrapping)
			{
				if (this.breakOpportunity == 0 || this.lineLength + this.tailSpace <= this.WrapBeforePosition())
				{
					this.breakOpportunity = this.lineLength + this.tailSpace;
					if (this.lineLength + this.tailSpace < this.WrapBeforePosition() && count > 1)
					{
						this.breakOpportunity += Math.Min(this.WrapBeforePosition() - (this.lineLength + this.tailSpace), count - 1);
					}
					if (this.breakOpportunity < this.lineLength + this.tailSpace + count - 1)
					{
						this.nextBreakOpportunity = this.lineLength + this.tailSpace + count - 1;
					}
					if (this.lineLength > this.flushedLength)
					{
						this.FlushLine(' ');
					}
				}
				else
				{
					this.nextBreakOpportunity = this.lineLength + this.tailSpace + count - 1;
				}
			}
			this.tailSpace += count;
		}

		public void OutputNbsp(int count)
		{
			if (this.preserveNbsp)
			{
				while (count != 0)
				{
					this.OutputNonspace("\u00a0", TextMapping.Unicode);
					count--;
				}
				return;
			}
			this.tailSpace += count;
		}

		public void OutputNonspace(char[] buffer, int offset, int count, TextMapping textMapping)
		{
			if (!this.lineWrapping && !this.endParagraph && textMapping == TextMapping.Unicode)
			{
				if (this.tailSpace != 0)
				{
					this.FlushTailSpace();
				}
				this.output.Write(buffer, offset, count, this.fallbacks ? this : null);
				this.lineLength += count;
				return;
			}
			this.OutputNonspaceImpl(buffer, offset, count, textMapping);
		}

		public void OutputNonspace(string text, TextMapping textMapping)
		{
			this.OutputNonspace(text, 0, text.Length, textMapping);
		}

		public void OutputNonspace(string text, int offset, int length, TextMapping textMapping)
		{
			if (textMapping != TextMapping.Unicode)
			{
				for (int i = offset; i < length; i++)
				{
					this.MapAndOutputSymbolCharacter(text[i], textMapping);
				}
				return;
			}
			if (this.endParagraph)
			{
				this.output.Write("\r\n");
				this.linePosition += 2;
				this.anyNewlines = true;
				this.endParagraph = false;
			}
			if (this.lineWrapping)
			{
				if (length != 0)
				{
					this.WrapPrepareToAppendNonspace(length);
					if (this.breakOpportunity == 0)
					{
						this.FlushLine(text[offset]);
						this.output.Write(text, offset, length, this.fallbacks ? this : null);
						this.flushedLength += length;
					}
					else
					{
						text.CopyTo(offset, this.wrapBuffer, this.lineLength - this.flushedLength, length);
					}
					this.lineLength += length;
					if (this.lineLength > 2 || text[offset] != '-' || (length == 2 && text[offset + 1] != '-'))
					{
						this.signaturePossible = false;
						return;
					}
				}
			}
			else
			{
				if (this.tailSpace != 0)
				{
					this.FlushTailSpace();
				}
				this.output.Write(text, offset, length, this.fallbacks ? this : null);
				this.lineLength += length;
			}
		}

		public void OutputNonspace(int ucs32Literal, TextMapping textMapping)
		{
			if (textMapping != TextMapping.Unicode)
			{
				this.MapAndOutputSymbolCharacter((char)ucs32Literal, textMapping);
				return;
			}
			if (this.endParagraph)
			{
				this.output.Write("\r\n");
				this.linePosition += 2;
				this.anyNewlines = true;
				this.endParagraph = false;
			}
			if (this.lineWrapping)
			{
				int num = Token.LiteralLength(ucs32Literal);
				this.WrapPrepareToAppendNonspace(num);
				if (this.breakOpportunity == 0)
				{
					this.FlushLine(Token.LiteralFirstChar(ucs32Literal));
					this.output.Write(ucs32Literal, this.fallbacks ? this : null);
					this.flushedLength += num;
				}
				else
				{
					this.wrapBuffer[this.lineLength - this.flushedLength] = Token.LiteralFirstChar(ucs32Literal);
					if (num != 1)
					{
						this.wrapBuffer[this.lineLength - this.flushedLength + 1] = Token.LiteralLastChar(ucs32Literal);
					}
				}
				this.lineLength += num;
				if (this.lineLength > 2 || num != 1 || (ushort)ucs32Literal != 45)
				{
					this.signaturePossible = false;
					return;
				}
			}
			else
			{
				if (this.tailSpace != 0)
				{
					this.FlushTailSpace();
				}
				this.output.Write(ucs32Literal, this.fallbacks ? this : null);
				this.lineLength += Token.LiteralLength(ucs32Literal);
			}
		}

		public void OpenAnchor(string anchorUrl)
		{
			this.anchorUrl = anchorUrl;
		}

		public void CloseAnchor()
		{
			if (this.anchorUrl != null)
			{
				bool flag = this.tailSpace != 0;
				string text = this.anchorUrl;
				if (text.IndexOf(' ') != -1)
				{
					text = text.Replace(" ", "%20");
				}
				this.OutputNonspace("<", TextMapping.Unicode);
				this.OutputNonspace(text, TextMapping.Unicode);
				this.OutputNonspace(">", TextMapping.Unicode);
				if (flag)
				{
					this.OutputSpace(1);
				}
				this.anchorUrl = null;
			}
		}

		public void CancelAnchor()
		{
			this.anchorUrl = null;
		}

		public void OutputImage(string imageUrl, string imageAltText, int wdthPixels, int heightPixels)
		{
			if (this.imageRenderingCallback != null && this.imageRenderingCallback(imageUrl, this.RenderingPosition()))
			{
				this.OutputSpace(1);
				return;
			}
			if ((wdthPixels == 0 || wdthPixels >= 8) && (heightPixels == 0 || heightPixels >= 8))
			{
				bool flag = this.tailSpace != 0;
				this.OutputNonspace("[", TextMapping.Unicode);
				if (!string.IsNullOrEmpty(imageAltText))
				{
					int num2;
					for (int num = 0; num != imageAltText.Length; num = num2 + 1)
					{
						num2 = imageAltText.IndexOfAny(TextOutput.Whitespaces, num);
						if (num2 == -1)
						{
							this.OutputNonspace(imageAltText, num, imageAltText.Length - num, TextMapping.Unicode);
							break;
						}
						if (num2 != num)
						{
							this.OutputNonspace(imageAltText, num, num2 - num, TextMapping.Unicode);
						}
						if (imageAltText[num] == '\t')
						{
							this.OutputTabulation(1);
						}
						else
						{
							this.OutputSpace(1);
						}
					}
				}
				else if (!string.IsNullOrEmpty(imageUrl))
				{
					if (imageUrl.Contains("/") && !imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
					{
						imageUrl = "X";
					}
					else if (imageUrl.IndexOf(' ') != -1)
					{
						imageUrl = imageUrl.Replace(" ", "%20");
					}
					this.OutputNonspace(imageUrl, TextMapping.Unicode);
				}
				else
				{
					this.OutputNonspace("X", TextMapping.Unicode);
				}
				this.OutputNonspace("]", TextMapping.Unicode);
				if (flag)
				{
					this.OutputSpace(1);
				}
			}
		}

		public int RenderingPosition()
		{
			return this.linePosition + this.lineLength + this.tailSpace;
		}

		public void Flush()
		{
			if (this.lineWrapping)
			{
				if (this.lineLength != 0)
				{
					this.FlushLine('\r');
					this.OutputNewLine();
				}
			}
			else if (this.lineLength != 0)
			{
				this.OutputNewLine();
			}
			this.output.Flush();
		}

		byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
		{
			if (this.htmlEscape)
			{
				unsafeAsciiMask = 1;
				return HtmlSupport.UnsafeAsciiMap;
			}
			unsafeAsciiMask = 0;
			return null;
		}

		bool IFallback.HasUnsafeUnicode()
		{
			return this.htmlEscape;
		}

		bool IFallback.TreatNonAsciiAsUnsafe(string charset)
		{
			return false;
		}

		bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
		{
			return this.htmlEscape && (ch < '\ud800' || ch >= '') && ((byte)(ch & 'ÿ') == 60 || (byte)(ch >> 8 & 'ÿ') == 60);
		}

		bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
		{
			if (this.htmlEscape)
			{
				HtmlEntityIndex htmlEntityIndex = (HtmlEntityIndex)0;
				if (ch <= '>')
				{
					if (ch == '>')
					{
						htmlEntityIndex = HtmlEntityIndex.gt;
					}
					else if (ch == '<')
					{
						htmlEntityIndex = HtmlEntityIndex.lt;
					}
					else if (ch == '&')
					{
						htmlEntityIndex = HtmlEntityIndex.amp;
					}
					else if (ch == '"')
					{
						htmlEntityIndex = HtmlEntityIndex.quot;
					}
				}
				else if ('\u00a0' <= ch && ch <= 'ÿ')
				{
					htmlEntityIndex = HtmlSupport.EntityMap[(int)(ch - '\u00a0')];
				}
				if (htmlEntityIndex != (HtmlEntityIndex)0)
				{
					string name = HtmlNameData.entities[(int)htmlEntityIndex].Name;
					if (outputEnd - outputBufferCount < name.Length + 2)
					{
						return false;
					}
					outputBuffer[outputBufferCount++] = '&';
					name.CopyTo(0, outputBuffer, outputBufferCount, name.Length);
					outputBufferCount += name.Length;
					outputBuffer[outputBufferCount++] = ';';
				}
				else
				{
					uint num = (uint)ch;
					int num2 = (num < 16U) ? 1 : ((num < 256U) ? 2 : ((num < 4096U) ? 3 : 4));
					if (outputEnd - outputBufferCount < num2 + 4)
					{
						return false;
					}
					outputBuffer[outputBufferCount++] = '&';
					outputBuffer[outputBufferCount++] = '#';
					outputBuffer[outputBufferCount++] = 'x';
					int num3 = outputBufferCount + num2;
					while (num != 0U)
					{
						uint num4 = num & 15U;
						outputBuffer[--num3] = (char)((ulong)num4 + (ulong)((num4 < 10U) ? 48L : 55L));
						num >>= 4;
					}
					outputBufferCount += num2;
					outputBuffer[outputBufferCount++] = ';';
				}
			}
			else
			{
				string substitute = TextOutput.GetSubstitute(ch);
				if (substitute != null)
				{
					if (outputEnd - outputBufferCount < substitute.Length)
					{
						return false;
					}
					substitute.CopyTo(0, outputBuffer, outputBufferCount, substitute.Length);
					outputBufferCount += substitute.Length;
				}
				else
				{
					outputBuffer[outputBufferCount++] = ch;
				}
			}
			return true;
		}

		void IDisposable.Dispose()
		{
			if (this.output != null)
			{
				((IDisposable)this.output).Dispose();
			}
			this.output = null;
			this.wrapBuffer = null;
			GC.SuppressFinalize(this);
		}

		bool IRestartable.CanRestart()
		{
			return this.output is IRestartable && ((IRestartable)this.output).CanRestart();
		}

		void IRestartable.Restart()
		{
			((IRestartable)this.output).Restart();
			this.Reinitialize();
		}

		void IRestartable.DisableRestart()
		{
			if (this.output is IRestartable)
			{
				((IRestartable)this.output).DisableRestart();
			}
		}

		void IReusable.Initialize(object newSourceOrDestination)
		{
			((IReusable)this.output).Initialize(newSourceOrDestination);
			this.Reinitialize();
		}

		private void Reinitialize()
		{
			this.anchorUrl = null;
			this.linePosition = 0;
			this.lineLength = 0;
			this.lineLengthBeforeSoftWrap = 0;
			this.flushedLength = 0;
			this.tailSpace = 0;
			this.breakOpportunity = 0;
			this.nextBreakOpportunity = 0;
			this.quotingLevel = 0;
			this.seenSpace = false;
			this.wrapped = false;
			this.signaturePossible = true;
			this.anyNewlines = false;
			this.endParagraph = false;
		}

		private void OutputNonspaceImpl(char[] buffer, int offset, int count, TextMapping textMapping)
		{
			if (count != 0)
			{
				if (textMapping != TextMapping.Unicode)
				{
					for (int i = 0; i < count; i++)
					{
						this.MapAndOutputSymbolCharacter(buffer[offset++], textMapping);
					}
					return;
				}
				if (this.endParagraph)
				{
					this.output.Write("\r\n");
					this.linePosition += 2;
					this.anyNewlines = true;
					this.endParagraph = false;
				}
				if (this.lineWrapping)
				{
					this.WrapPrepareToAppendNonspace(count);
					if (this.breakOpportunity == 0)
					{
						this.FlushLine(buffer[offset]);
						this.output.Write(buffer, offset, count, this.fallbacks ? this : null);
						this.flushedLength += count;
					}
					else
					{
						Buffer.BlockCopy(buffer, offset * 2, this.wrapBuffer, (this.lineLength - this.flushedLength) * 2, count * 2);
					}
					this.lineLength += count;
					if (this.lineLength > 2 || buffer[offset] != '-' || (count == 2 && buffer[offset + 1] != '-'))
					{
						this.signaturePossible = false;
						return;
					}
				}
				else
				{
					if (this.tailSpace != 0)
					{
						this.FlushTailSpace();
					}
					this.output.Write(buffer, offset, count, this.fallbacks ? this : null);
					this.lineLength += count;
				}
			}
		}

		private int WrapBeforePosition()
		{
			return this.wrapBeforePosition - (this.rfc2646 ? (this.quotingLevel + 1) : 0);
		}

		private int LongestNonWrappedParagraph()
		{
			return this.longestNonWrappedParagraph - (this.rfc2646 ? (this.quotingLevel + 1) : 0);
		}

		private void WrapPrepareToAppendNonspace(int count)
		{
			while (this.breakOpportunity != 0 && this.lineLength + this.tailSpace + count > (this.wrapped ? this.WrapBeforePosition() : this.LongestNonWrappedParagraph()))
			{
				if (this.flushedLength == 0 && this.rfc2646)
				{
					for (int i = 0; i < this.quotingLevel; i++)
					{
						this.output.Write('>');
					}
					if (this.quotingLevel != 0 || this.wrapBuffer[0] == '>' || this.wrapBuffer[0] == ' ')
					{
						this.output.Write(' ');
					}
				}
				if (this.breakOpportunity >= this.lineLength)
				{
					do
					{
						if (this.lineLength - this.flushedLength == this.wrapBuffer.Length)
						{
							this.output.Write(this.wrapBuffer, 0, this.wrapBuffer.Length, this.fallbacks ? this : null);
							this.flushedLength += this.wrapBuffer.Length;
						}
						this.wrapBuffer[this.lineLength - this.flushedLength] = ' ';
						this.lineLength++;
						this.tailSpace--;
					}
					while (this.lineLength != this.breakOpportunity + 1);
				}
				this.output.Write(this.wrapBuffer, 0, this.breakOpportunity + 1 - this.flushedLength, this.fallbacks ? this : null);
				this.anyNewlines = true;
				this.output.Write("\r\n");
				this.wrapped = true;
				this.lineLengthBeforeSoftWrap += this.breakOpportunity + 1;
				this.linePosition += this.breakOpportunity + 1 + 2;
				this.lineLength -= this.breakOpportunity + 1;
				int num = this.flushedLength;
				this.flushedLength = 0;
				if (this.lineLength != 0)
				{
					if (this.nextBreakOpportunity == 0 || this.nextBreakOpportunity - (this.breakOpportunity + 1) >= this.lineLength || this.nextBreakOpportunity - (this.breakOpportunity + 1) == 0)
					{
						if (this.rfc2646)
						{
							for (int j = 0; j < this.quotingLevel; j++)
							{
								this.output.Write('>');
							}
							if (this.quotingLevel != 0 || this.wrapBuffer[this.breakOpportunity + 1 - num] == '>' || this.wrapBuffer[this.breakOpportunity + 1 - num] == ' ')
							{
								this.output.Write(' ');
							}
						}
						this.output.Write(this.wrapBuffer, this.breakOpportunity + 1 - num, this.lineLength, this.fallbacks ? this : null);
						this.flushedLength = this.lineLength;
					}
					else
					{
						Buffer.BlockCopy(this.wrapBuffer, (this.breakOpportunity + 1 - num) * 2, this.wrapBuffer, 0, this.lineLength * 2);
					}
				}
				if (this.nextBreakOpportunity != 0)
				{
					this.breakOpportunity = this.nextBreakOpportunity - (this.breakOpportunity + 1);
					if (this.breakOpportunity > this.WrapBeforePosition())
					{
						if (this.lineLength < this.WrapBeforePosition())
						{
							this.nextBreakOpportunity = this.breakOpportunity;
							this.breakOpportunity = this.WrapBeforePosition();
						}
						else if (this.breakOpportunity > this.lineLength)
						{
							this.nextBreakOpportunity = this.breakOpportunity;
							this.breakOpportunity = this.lineLength;
						}
						else
						{
							this.nextBreakOpportunity = 0;
						}
					}
					else
					{
						this.nextBreakOpportunity = 0;
					}
				}
				else
				{
					this.breakOpportunity = 0;
				}
			}
			if (this.tailSpace != 0)
			{
				if (this.breakOpportunity == 0)
				{
					if (this.flushedLength == 0 && this.rfc2646)
					{
						for (int k = 0; k < this.quotingLevel; k++)
						{
							this.output.Write('>');
						}
						this.output.Write(' ');
					}
					this.flushedLength += this.tailSpace;
					this.FlushTailSpace();
					return;
				}
				do
				{
					this.wrapBuffer[this.lineLength - this.flushedLength] = ' ';
					this.lineLength++;
					this.tailSpace--;
				}
				while (this.tailSpace != 0);
			}
		}

		private void FlushLine(char nextChar)
		{
			if (this.flushedLength == 0 && this.rfc2646)
			{
				for (int i = 0; i < this.quotingLevel; i++)
				{
					this.output.Write('>');
				}
				char c = (this.lineLength != 0) ? this.wrapBuffer[0] : nextChar;
				if (this.quotingLevel != 0 || c == '>' || c == ' ')
				{
					this.output.Write(' ');
				}
			}
			if (this.lineLength != this.flushedLength)
			{
				this.output.Write(this.wrapBuffer, 0, this.lineLength - this.flushedLength, this.fallbacks ? this : null);
				this.flushedLength = this.lineLength;
			}
		}

		private void FlushTailSpace()
		{
			this.lineLength += this.tailSpace;
			do
			{
				this.output.Write(' ');
				this.tailSpace--;
			}
			while (this.tailSpace != 0);
		}

		private void MapAndOutputSymbolCharacter(char ch, TextMapping textMapping)
		{
			if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
			{
				this.OutputNonspace((int)ch, TextMapping.Unicode);
				return;
			}
			string text = null;
			if (textMapping == TextMapping.Wingdings)
			{
				if (ch <= 'Ø')
				{
					switch (ch)
					{
					case 'J':
						text = "☺";
						break;
					case 'K':
						text = ":|";
						break;
					case 'L':
						text = "☹";
						break;
					default:
						if (ch == 'Ø')
						{
							text = ">";
						}
						break;
					}
				}
				else
				{
					switch (ch)
					{
					case 'ß':
						text = "<--";
						break;
					case 'à':
						text = "-->";
						break;
					default:
						switch (ch)
						{
						case 'ç':
							text = "<==";
							break;
						case 'è':
							text = "==>";
							break;
						default:
							switch (ch)
							{
							case 'ï':
								text = "<=";
								break;
							case 'ð':
								text = "=>";
								break;
							case 'ó':
								text = "<=>";
								break;
							}
							break;
						}
						break;
					}
				}
			}
			if (text == null)
			{
				text = "•";
			}
			this.OutputNonspace(text, TextMapping.Unicode);
		}

		private static string GetSubstitute(char ch)
		{
			return AsciiEncoderFallback.GetCharacterFallback(ch);
		}

		protected ConverterOutput output;

		protected bool lineWrapping;

		protected bool rfc2646;

		protected int longestNonWrappedParagraph;

		protected int wrapBeforePosition;

		protected bool preserveTrailingSpace;

		protected bool preserveTabulation;

		protected bool preserveNbsp;

		protected int lineLength;

		protected int lineLengthBeforeSoftWrap;

		protected int flushedLength;

		protected int tailSpace;

		protected int breakOpportunity;

		protected int nextBreakOpportunity;

		protected int quotingLevel;

		protected bool seenSpace;

		protected bool wrapped;

		protected char[] wrapBuffer;

		protected bool signaturePossible = true;

		protected bool anyNewlines;

		protected bool endParagraph;

		private static readonly char[] Whitespaces = new char[]
		{
			' ',
			'\t',
			'\r',
			'\n',
			'\f'
		};

		private bool fallbacks;

		private bool htmlEscape;

		private string anchorUrl;

		private int linePosition;

		private ImageRenderingCallbackInternal imageRenderingCallback;
	}
}
