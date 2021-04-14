using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.ContentTypes.Internal
{
	internal class ContentLineParser : IDisposable
	{
		public ContentLineParser(Stream stream, Encoding encoding, ComplianceTracker complianceTracker)
		{
			this.reader = new DirectoryReader(stream, encoding, complianceTracker);
			this.complianceTracker = complianceTracker;
		}

		public ContentLineParser.States State
		{
			get
			{
				this.CheckDisposed("State::get");
				return this.state;
			}
		}

		public Encoding CurrentCharsetEncoding
		{
			get
			{
				this.CheckDisposed("CurrentEncoding::get");
				return this.reader.CurrentCharsetEncoding;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool ParseElement(char[] buffer, int offset, int size, out int filled, bool parseAsText, ContentLineParser.Separators separators)
		{
			this.CheckDisposed("ParseElement");
			bool flag = false;
			bool flag2 = false;
			int num = offset;
			int num2 = offset + size;
			filled = 0;
			IL_756:
			while (!flag)
			{
				if (!this.eof && this.state != ContentLineParser.States.ValueStart && this.state != ContentLineParser.States.ValueStartComma && this.state != ContentLineParser.States.ValueStartSemiColon && this.state != ContentLineParser.States.Value && this.state != ContentLineParser.States.ValueEnd)
				{
					char c = this.GetCurrentChar();
				}
				if (this.eof)
				{
					if (this.state != ContentLineParser.States.ValueStartComma && this.state != ContentLineParser.States.ValueStartSemiColon && this.state != ContentLineParser.States.ValueStart && this.state != ContentLineParser.States.Value && this.state != ContentLineParser.States.ValueEnd && this.state != ContentLineParser.States.End && this.state != ContentLineParser.States.PropName)
					{
						this.complianceTracker.SetComplianceStatus(ComplianceStatus.StreamTruncated, CalendarStrings.UnexpectedEndOfStream);
					}
					this.state = ContentLineParser.States.End;
					flag2 = true;
					break;
				}
				if (this.state != ContentLineParser.States.ValueStart && this.state != ContentLineParser.States.ValueStartComma && this.state != ContentLineParser.States.ValueStartSemiColon && this.state != ContentLineParser.States.Value && this.state != ContentLineParser.States.ValueEnd)
				{
					this.lastCharProcessed = false;
				}
				switch (this.state)
				{
				case ContentLineParser.States.PropName:
					this.currentValueCharsetOverride = null;
					this.currentValueEncodingOverride = null;
					for (;;)
					{
						char c = this.GetCurrentChar();
						if (this.eof)
						{
							break;
						}
						if (this.isEndOfLine || c == '\r')
						{
							goto IL_189;
						}
						if (c == ':')
						{
							goto Block_23;
						}
						if (c == ';')
						{
							goto Block_24;
						}
						if ((byte)(ContentLineParser.GetToken((int)c) & ContentLineParser.Tokens.Alpha) == 0 && (byte)(ContentLineParser.GetToken((int)c) & ContentLineParser.Tokens.Digit) == 0 && c != '-')
						{
							this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInPropertyName, CalendarStrings.InvalidCharacterInPropertyName);
						}
						buffer[offset++] = c;
						if (offset == num2)
						{
							goto Block_28;
						}
					}
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.StreamTruncated | ComplianceStatus.PropertyTruncated, CalendarStrings.PropertyTruncated);
					continue;
					IL_189:
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CalendarStrings.PropertyTruncated);
					this.lastCharProcessed = false;
					this.state = ContentLineParser.States.Value;
					flag2 = true;
					flag = true;
					continue;
					Block_23:
					this.state = ContentLineParser.States.ValueStart;
					this.escaped = false;
					flag = true;
					flag2 = true;
					continue;
					Block_24:
					this.state = ContentLineParser.States.ParamName;
					flag = true;
					flag2 = true;
					continue;
					Block_28:
					flag = true;
					continue;
				case ContentLineParser.States.ParamName:
					for (;;)
					{
						char c = this.GetCurrentChar();
						if (this.eof)
						{
							goto IL_756;
						}
						if (this.isEndOfLine || c == '\r')
						{
							break;
						}
						if (c == '=')
						{
							goto Block_31;
						}
						if (this.complianceTracker.Format == FormatType.VCard && (c == ';' || c == ':'))
						{
							goto IL_2A7;
						}
						if ((byte)(ContentLineParser.GetToken((int)c) & ContentLineParser.Tokens.Alpha) == 0 && (byte)(ContentLineParser.GetToken((int)c) & ContentLineParser.Tokens.Digit) == 0 && c != '-')
						{
							this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInParameterName, CalendarStrings.InvalidCharacterInParameterName);
						}
						buffer[offset++] = c;
						if (offset == num2)
						{
							goto Block_37;
						}
					}
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CalendarStrings.PropertyTruncated);
					this.lastCharProcessed = false;
					this.state = ContentLineParser.States.Value;
					flag2 = true;
					flag = true;
					continue;
					Block_31:
					this.state = ContentLineParser.States.ParamValueStart;
					flag2 = true;
					flag = true;
					continue;
					IL_2A7:
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.ParameterNameMissing, CalendarStrings.ParameterNameMissing);
					this.state = ContentLineParser.States.UnnamedParamEnd;
					this.lastCharProcessed = false;
					flag = true;
					flag2 = true;
					continue;
					Block_37:
					flag = true;
					continue;
				case ContentLineParser.States.UnnamedParamEnd:
				{
					char c = this.GetCurrentChar();
					flag = true;
					flag2 = true;
					if (c == ':')
					{
						this.state = ContentLineParser.States.ValueStart;
						continue;
					}
					if (c == ';')
					{
						this.state = ContentLineParser.States.ParamName;
						continue;
					}
					continue;
				}
				case ContentLineParser.States.ParamValueStart:
				{
					char c = this.GetCurrentChar();
					this.lastCharProcessed = false;
					if (this.isEndOfLine || c == '\r')
					{
						this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CalendarStrings.PropertyTruncated);
						this.state = ContentLineParser.States.ParamValueUnquoted;
						flag2 = true;
						flag = true;
						continue;
					}
					if (c == '"')
					{
						this.lastCharProcessed = true;
						this.state = ContentLineParser.States.ParamValueQuoted;
					}
					else
					{
						this.state = ContentLineParser.States.ParamValueUnquoted;
					}
					flag = true;
					flag2 = true;
					continue;
				}
				case ContentLineParser.States.ParamValueUnquoted:
					for (;;)
					{
						char c = this.GetCurrentChar();
						if (this.eof)
						{
							goto IL_756;
						}
						if (this.isEndOfLine || c == '\r')
						{
							break;
						}
						if (c == ':')
						{
							goto Block_44;
						}
						if (c == ';')
						{
							goto Block_45;
						}
						if ((separators & ContentLineParser.Separators.Comma) != ContentLineParser.Separators.None && c == ',')
						{
							goto Block_47;
						}
						if ((byte)(ContentLineParser.GetToken((int)c) & ContentLineParser.Tokens.SafeChar) == 0)
						{
							this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInParameterText, CalendarStrings.InvalidCharacterInParameterText);
						}
						buffer[offset++] = c;
						if (offset == num2)
						{
							goto Block_49;
						}
					}
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CalendarStrings.PropertyTruncated);
					this.lastCharProcessed = false;
					this.state = ContentLineParser.States.Value;
					flag2 = true;
					flag = true;
					continue;
					Block_44:
					this.state = ContentLineParser.States.ValueStart;
					this.escaped = false;
					flag = true;
					flag2 = true;
					continue;
					Block_45:
					this.state = ContentLineParser.States.ParamName;
					flag = true;
					flag2 = true;
					continue;
					Block_47:
					this.state = ContentLineParser.States.ParamValueStart;
					flag = true;
					flag2 = true;
					continue;
					Block_49:
					flag = true;
					continue;
				case ContentLineParser.States.ParamValueQuoted:
					for (;;)
					{
						char c = this.GetCurrentChar();
						if (this.eof)
						{
							goto IL_756;
						}
						if (this.isEndOfLine || c == '\r')
						{
							break;
						}
						if (c == '"')
						{
							goto Block_52;
						}
						if ((byte)(ContentLineParser.GetToken((int)c) & ContentLineParser.Tokens.QSafeChar) == 0)
						{
							this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInQuotedString, CalendarStrings.InvalidCharacterInQuotedString);
						}
						buffer[offset++] = c;
						if (offset == num2)
						{
							goto Block_54;
						}
					}
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CalendarStrings.PropertyTruncated);
					this.lastCharProcessed = false;
					this.state = ContentLineParser.States.ParamValueQuotedEnd;
					continue;
					Block_52:
					this.state = ContentLineParser.States.ParamValueQuotedEnd;
					continue;
					Block_54:
					flag = true;
					continue;
				case ContentLineParser.States.ParamValueQuotedEnd:
				{
					char c = this.GetCurrentChar();
					if (this.isEndOfLine || c == '\r')
					{
						this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CalendarStrings.PropertyTruncated);
						this.lastCharProcessed = false;
						this.state = ContentLineParser.States.Value;
						flag2 = true;
						flag = true;
						continue;
					}
					if (c == ';')
					{
						this.state = ContentLineParser.States.ParamName;
						flag = true;
						flag2 = true;
						continue;
					}
					if (c == ':')
					{
						this.state = ContentLineParser.States.ValueStart;
						this.escaped = false;
						flag = true;
						flag2 = true;
						continue;
					}
					if ((separators & ContentLineParser.Separators.Comma) != ContentLineParser.Separators.None && c == ',')
					{
						this.state = ContentLineParser.States.ParamValueStart;
						flag = true;
						flag2 = true;
						continue;
					}
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidParameterValue, CalendarStrings.InvalidParameterValue);
					this.state = ContentLineParser.States.ParamValueUnquoted;
					continue;
				}
				case ContentLineParser.States.ValueStartComma:
				case ContentLineParser.States.ValueStartSemiColon:
					this.state = ContentLineParser.States.Value;
					this.GetCurrentChar();
					flag = true;
					flag2 = true;
					continue;
				case ContentLineParser.States.ValueStart:
					this.state = ContentLineParser.States.Value;
					flag = true;
					flag2 = true;
					continue;
				case ContentLineParser.States.Value:
					if (this.currentValueCharsetOverride != null)
					{
						this.reader.SwitchCharsetEncoding(this.currentValueCharsetOverride);
						this.currentValueCharsetOverride = null;
					}
					if (this.currentValueEncodingOverride != null)
					{
						this.reader.ApplyValueDecoder(this.currentValueEncodingOverride);
						this.currentValueEncodingOverride = null;
					}
					for (;;)
					{
						if (this.emitLF)
						{
							this.emitLF = false;
							buffer[offset++] = '\n';
							if (offset == num2)
							{
								break;
							}
						}
						char c = this.GetCurrentChar();
						if (this.isEndOfLine || this.eof)
						{
							goto IL_665;
						}
						if (parseAsText && c == '\\' && !this.escaped)
						{
							this.escaped = true;
						}
						else
						{
							if ((separators & ContentLineParser.Separators.Comma) != ContentLineParser.Separators.None && c == ',' && !this.escaped)
							{
								goto Block_70;
							}
							if ((separators & ContentLineParser.Separators.SemiColon) != ContentLineParser.Separators.None && c == ';' && !this.escaped)
							{
								goto Block_73;
							}
							if ((byte)(ContentLineParser.GetToken((int)c) & ContentLineParser.Tokens.ValueChar) == 0)
							{
								this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInPropertyValue, CalendarStrings.InvalidCharacterInPropertyValue);
							}
							if (this.escaped)
							{
								if ('n' == c || 'N' == c)
								{
									c = '\r';
									this.emitLF = true;
								}
								this.escaped = false;
							}
							buffer[offset++] = c;
							if (offset == num2)
							{
								goto Block_77;
							}
						}
					}
					flag = true;
					continue;
					IL_665:
					this.state = ContentLineParser.States.ValueEnd;
					continue;
					Block_70:
					this.state = ContentLineParser.States.ValueStartComma;
					this.lastCharProcessed = false;
					flag = true;
					flag2 = true;
					continue;
					Block_73:
					this.state = ContentLineParser.States.ValueStartSemiColon;
					this.lastCharProcessed = false;
					flag = true;
					flag2 = true;
					continue;
					Block_77:
					flag = true;
					continue;
				case ContentLineParser.States.ValueEnd:
					this.reader.RestoreCharsetEncoding();
					this.state = ContentLineParser.States.PropName;
					flag = true;
					flag2 = true;
					continue;
				}
				flag = true;
				flag2 = true;
			}
			filled = offset - num;
			return !flag2;
		}

		public void ApplyValueOverrides(Encoding charset, ByteEncoder decoder)
		{
			this.CheckDisposed("ApplyValueDecoder");
			if (this.state == ContentLineParser.States.Value)
			{
				throw new InvalidOperationException();
			}
			this.currentValueCharsetOverride = charset;
			this.currentValueEncodingOverride = decoder;
		}

		public Stream GetValueReadStream()
		{
			this.CheckDisposed("GetValueReadStream");
			if (this.state != ContentLineParser.States.Value)
			{
				throw new InvalidOperationException();
			}
			Stream stream = this.reader.GetValueReadStream(delegate
			{
				if (this.state != ContentLineParser.States.Value)
				{
					throw new InvalidOperationException();
				}
				this.state = ContentLineParser.States.ValueEnd;
				char[] buffer = new char[0];
				int num;
				this.ParseElement(buffer, 0, 0, out num, true, ContentLineParser.Separators.None);
			});
			if (this.currentValueEncodingOverride != null)
			{
				stream = new EncoderStream(stream, this.currentValueEncodingOverride, EncoderStreamAccess.Read);
				this.currentValueEncodingOverride = null;
			}
			return stream;
		}

		protected virtual void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException("ContentLineParser", methodName);
			}
		}

		private static ContentLineParser.Tokens GetToken(int ch)
		{
			if (ch > 255)
			{
				return ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar;
			}
			return ContentLineParser.Dictionary[ch];
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing && this.reader != null)
			{
				this.reader.Dispose();
				this.reader = null;
			}
		}

		private char GetCurrentChar()
		{
			if (this.lastCharProcessed)
			{
				if (this.eof)
				{
					throw new InvalidOperationException();
				}
				this.eof = !this.reader.ReadChar(out this.lastChar, out this.isEndOfLine);
			}
			this.lastCharProcessed = true;
			return this.lastChar;
		}

		private const char DQuote = '"';

		private const char CR = '\r';

		private const char LF = '\n';

		private const char SemiColon = ';';

		private const char Colon = ':';

		private const char Comma = ',';

		private const char Dash = '-';

		private const char BackSlash = '\\';

		internal static readonly ContentLineParser.Tokens[] Dictionary = new ContentLineParser.Tokens[]
		{
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.CTL,
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
			ContentLineParser.Tokens.CTL,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
			ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII
		};

		private ContentLineParser.States state;

		private DirectoryReader reader;

		private char lastChar;

		private bool lastCharProcessed = true;

		private bool eof;

		private bool isEndOfLine;

		private bool escaped;

		private bool emitLF;

		private ComplianceTracker complianceTracker;

		private bool isDisposed;

		private Encoding currentValueCharsetOverride;

		private ByteEncoder currentValueEncodingOverride;

		internal enum States
		{
			PropName,
			ParamName,
			UnnamedParamEnd,
			ParamValueStart,
			ParamValueUnquoted,
			ParamValueQuoted,
			ParamValueQuotedEnd,
			ValueStartComma,
			ValueStartSemiColon,
			ValueStart,
			Value,
			ValueEnd,
			End
		}

		[Flags]
		internal enum Tokens : byte
		{
			CTL = 1,
			Alpha = 2,
			Digit = 4,
			SafeChar = 8,
			QSafeChar = 16,
			ValueChar = 32,
			WSP = 64,
			NonASCII = 128
		}

		[Flags]
		internal enum Separators
		{
			None = 0,
			Comma = 1,
			SemiColon = 2
		}
	}
}
