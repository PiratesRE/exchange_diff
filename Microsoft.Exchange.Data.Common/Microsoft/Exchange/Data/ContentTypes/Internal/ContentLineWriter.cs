using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Data.ContentTypes.Internal
{
	internal class ContentLineWriter : IDisposable
	{
		public ContentLineWriter(Stream s, Encoding encoding)
		{
			this.foldingTextWriter = new ContentLineWriter.FoldingTextWriter(s, encoding, "\r\n ");
		}

		public ContentLineWriteState State
		{
			get
			{
				return this.state;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.foldingTextWriter != null)
			{
				this.Flush();
				this.foldingTextWriter.Dispose();
				this.foldingTextWriter = null;
			}
			this.state = ContentLineWriteState.Closed;
		}

		public void Flush()
		{
			this.foldingTextWriter.Flush();
		}

		public void WriteProperty(string property, string data)
		{
			this.AssertValidState(ContentLineWriteState.Start | ContentLineWriteState.PropertyEnd);
			this.WriteToStream(property + ":" + data + "\r\n");
			this.state = ContentLineWriteState.PropertyEnd;
		}

		public void StartProperty(string property)
		{
			this.AssertValidState(ContentLineWriteState.Start | ContentLineWriteState.PropertyEnd);
			this.WriteToStream(property);
			this.state = ContentLineWriteState.Property;
		}

		public void EndProperty()
		{
			this.AssertValidState(ContentLineWriteState.PropertyValue);
			this.WriteToStream("\r\n");
			this.state = ContentLineWriteState.PropertyEnd;
		}

		public void StartParameter(string parameter)
		{
			this.AssertValidState(ContentLineWriteState.Property | ContentLineWriteState.ParameterEnd);
			this.WriteToStream(";");
			if (parameter != null)
			{
				this.WriteToStream(parameter);
			}
			this.emptyParamName = (parameter == null);
			this.state = ContentLineWriteState.Parameter;
		}

		public void EndParameter()
		{
			this.AssertValidState(ContentLineWriteState.Parameter | ContentLineWriteState.ParameterValue);
			this.state = ContentLineWriteState.ParameterEnd;
		}

		public void WriteNextValue(ContentLineParser.Separators separator)
		{
			string data;
			if (separator == ContentLineParser.Separators.Comma)
			{
				data = ",";
			}
			else
			{
				if (separator != ContentLineParser.Separators.SemiColon)
				{
					throw new ArgumentException();
				}
				data = ";";
			}
			ContentLineWriteState contentLineWriteState = this.state;
			if (contentLineWriteState == ContentLineWriteState.PropertyValue || contentLineWriteState == ContentLineWriteState.ParameterValue)
			{
				this.WriteToStream(data);
				return;
			}
			throw new InvalidOperationException(CalendarStrings.InvalidState);
		}

		public void WriteStartValue()
		{
			ContentLineWriteState contentLineWriteState = this.state;
			if (contentLineWriteState <= ContentLineWriteState.Parameter)
			{
				switch (contentLineWriteState)
				{
				case ContentLineWriteState.Property:
					break;
				case ContentLineWriteState.Start | ContentLineWriteState.Property:
				case ContentLineWriteState.PropertyValue:
					goto IL_60;
				default:
					if (contentLineWriteState != ContentLineWriteState.Parameter)
					{
						goto IL_60;
					}
					if (!this.emptyParamName)
					{
						this.WriteToStream("=");
					}
					this.state = ContentLineWriteState.ParameterValue;
					return;
				}
			}
			else if (contentLineWriteState == ContentLineWriteState.ParameterValue || contentLineWriteState != ContentLineWriteState.ParameterEnd)
			{
				goto IL_60;
			}
			this.WriteToStream(":");
			this.state = ContentLineWriteState.PropertyValue;
			return;
			IL_60:
			throw new InvalidOperationException(CalendarStrings.InvalidState);
		}

		public void WriteChars(char[] data, int offset, int size)
		{
			this.AssertValidState(ContentLineWriteState.PropertyValue | ContentLineWriteState.ParameterValue);
			this.foldingTextWriter.Write(data, offset, size);
		}

		internal void WriteToStream(byte[] data)
		{
			this.AssertValidState(ContentLineWriteState.PropertyValue);
			this.foldingTextWriter.Write(data, 0, data.Length);
		}

		internal void WriteToStream(byte[] data, int offset, int length)
		{
			this.AssertValidState(ContentLineWriteState.PropertyValue);
			this.foldingTextWriter.Write(data, offset, length);
		}

		internal void WriteToStream(byte data)
		{
			this.AssertValidState(ContentLineWriteState.PropertyValue | ContentLineWriteState.ParameterValue);
			this.foldingTextWriter.WriteByte(data);
		}

		internal void WriteToStream(string data)
		{
			this.foldingTextWriter.Write(data);
		}

		private void AssertValidState(ContentLineWriteState state)
		{
			if ((state & this.state) == (ContentLineWriteState)0)
			{
				throw new InvalidOperationException(CalendarStrings.InvalidStateForOperation);
			}
		}

		private const string FoldingTagString = "\r\n ";

		private ContentLineWriter.FoldingTextWriter foldingTextWriter;

		private ContentLineWriteState state = ContentLineWriteState.Start;

		private bool emptyParamName;

		private class FoldingTextWriter : TextWriter
		{
			public FoldingTextWriter(Stream s, Encoding encoding, string foldingString)
			{
				this.baseStream = s;
				this.encoding = encoding;
				this.encoder = this.encoding.GetEncoder();
				this.decoder = this.encoding.GetDecoder();
				this.foldingBytes = CTSGlobals.AsciiEncoding.GetBytes(foldingString);
			}

			private FoldingTextWriter()
			{
			}

			public override Encoding Encoding
			{
				get
				{
					return this.encoding;
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && this.baseStream != null)
				{
					this.baseStream.Dispose();
					this.baseStream = null;
				}
				base.Dispose(disposing);
			}

			public override void Flush()
			{
				this.baseStream.Flush();
			}

			public void Write(byte[] buffer, int offset, int count)
			{
				int charCount = this.decoder.GetCharCount(buffer, offset, count, false);
				char[] array = new char[charCount];
				this.decoder.GetChars(buffer, offset, count, array, 0);
				this.Write(array, 0, array.Length, buffer, count);
			}

			public override void Write(string data)
			{
				byte[] bytes = this.encoding.GetBytes(data);
				int charCount = this.decoder.GetCharCount(bytes, 0, bytes.Length, false);
				char[] array = new char[charCount];
				this.decoder.GetChars(bytes, 0, bytes.Length, array, 0);
				this.Write(array, 0, array.Length, bytes, bytes.Length);
			}

			public override void Write(char[] buffer, int offset, int count)
			{
				this.Write(buffer, offset, count, null, -1);
			}

			public void Write(char[] charBuffer, int charOffset, int charCount, byte[] buffer, int byteCount)
			{
				bool flag = false;
				int num = 1;
				int num2 = 0;
				int num3 = 0;
				bool flag2 = false;
				if (buffer == null)
				{
					buffer = new byte[this.encoder.GetByteCount(charBuffer, charOffset, charCount, false)];
					this.encoder.GetBytes(charBuffer, charOffset, charCount, buffer, 0, false);
				}
				if (byteCount == -1)
				{
					byteCount = buffer.Length;
				}
				while (byteCount > 0 && num3 < charBuffer.Length)
				{
					switch (this.state)
					{
					case ContentLineWriter.FoldingTextWriter.States.Normal:
					{
						if (flag || this.linePosition == 75)
						{
							if (flag)
							{
								flag = false;
							}
							if (charBuffer[num3] == '\r')
							{
								num3++;
								this.state = ContentLineWriter.FoldingTextWriter.States.CR;
								this.baseStream.WriteByte(buffer[num2++]);
								byteCount--;
								break;
							}
							this.baseStream.Write(this.foldingBytes, 0, this.foldingBytes.Length);
							this.linePosition = 1;
						}
						int num4 = Math.Min(75 - this.linePosition, byteCount);
						int num5 = num2;
						int codePage = CodePageMap.GetCodePage(this.encoding);
						while (num2 - num5 < num4)
						{
							bool flag3 = false;
							if (num3 == charBuffer.Length)
							{
								break;
							}
							int num6 = codePage;
							if (num6 != 1200)
							{
								switch (num6)
								{
								case 65000:
									if (!flag2)
									{
										if (num3 == charBuffer.Length - 1)
										{
											flag2 = true;
										}
										num = this.WriteCharIntoBytes(charBuffer[num3], flag2);
									}
									break;
								case 65001:
									if (charBuffer[num3] < '\u0080')
									{
										num = 1;
									}
									else if (charBuffer[num3] < 'ࠀ')
									{
										num = 2;
									}
									else if (char.IsHighSurrogate(charBuffer[num3]))
									{
										if (num3 < charBuffer.Length - 1 && char.IsLowSurrogate(charBuffer[num3 + 1]))
										{
											flag3 = true;
											if (buffer[num2] < 248)
											{
												num = 4;
											}
											else if (buffer[num2] < 252)
											{
												num = 5;
											}
											else
											{
												num = 6;
											}
										}
									}
									else if (ContentLineWriter.FoldingTextWriter.IsInvalidUTF8Byte(charBuffer[num3], buffer, num2))
									{
										num = 1;
									}
									else
									{
										num = 3;
									}
									break;
								default:
									num = this.WriteCharIntoBytes(charBuffer[num3], false);
									break;
								}
							}
							else if (this.linePosition + num2 - num5 > 71 && char.IsHighSurrogate(charBuffer[num3]))
							{
								flag = true;
							}
							else
							{
								num = 2;
							}
							if (flag)
							{
								break;
							}
							if (this.linePosition + (num2 - num5) + num > 75)
							{
								flag = true;
								break;
							}
							num2 += num;
							if (flag3)
							{
								num3++;
							}
							if (charBuffer[num3++] == '\r')
							{
								this.state = ContentLineWriter.FoldingTextWriter.States.CR;
								break;
							}
						}
						int num7 = num2 - num5;
						this.baseStream.Write(buffer, num5, num7);
						this.linePosition += num7;
						byteCount -= num7;
						break;
					}
					case ContentLineWriter.FoldingTextWriter.States.CR:
						this.baseStream.WriteByte(buffer[num2]);
						if (charBuffer[num3++] == '\n')
						{
							this.linePosition = 0;
							this.state = ContentLineWriter.FoldingTextWriter.States.Normal;
						}
						else
						{
							if (this.linePosition == 75 || flag)
							{
								if (flag)
								{
									flag = false;
								}
								this.baseStream.Write(this.foldingBytes, 0, this.foldingBytes.Length);
								this.linePosition = 1;
							}
							else
							{
								this.linePosition++;
							}
							if (num3 < charBuffer.Length && charBuffer[num3] != '\r')
							{
								this.state = ContentLineWriter.FoldingTextWriter.States.Normal;
							}
						}
						num2++;
						byteCount--;
						break;
					}
				}
			}

			public void WriteByte(byte byteToWrite)
			{
				this.baseStream.WriteByte(byteToWrite);
			}

			public override void Write(char charToWrite)
			{
				throw new NotSupportedException();
			}

			private static bool IsInvalidUTF8Byte(char inputChar, byte[] buffer, int offset)
			{
				return inputChar == '�' && (offset >= buffer.Length - 2 || buffer[offset] != 239 || buffer[offset + 1] != 191 || buffer[offset + 2] != 189);
			}

			private int WriteCharIntoBytes(char ch, bool flush)
			{
				this.charCheckerArray[0] = ch;
				return this.encoder.GetBytes(this.charCheckerArray, 0, 1, this.byteBuffer, 0, flush);
			}

			private const char CR = '\r';

			private const char LF = '\n';

			private const int MaxTextLength = 75;

			private Stream baseStream;

			private int linePosition;

			private byte[] foldingBytes;

			private byte[] byteBuffer = new byte[10];

			private char[] charCheckerArray = new char[1];

			private Encoding encoding;

			private Encoder encoder;

			private Decoder decoder;

			private ContentLineWriter.FoldingTextWriter.States state;

			private enum States
			{
				Normal,
				CR
			}
		}
	}
}
