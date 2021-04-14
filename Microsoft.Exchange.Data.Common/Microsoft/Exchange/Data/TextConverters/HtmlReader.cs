using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class HtmlReader : IRestartable, IResultsFeedback, IDisposable
	{
		public HtmlReader(Stream input, Encoding inputEncoding)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (!input.CanRead)
			{
				throw new ArgumentException("input stream must support reading");
			}
			this.input = input;
			this.inputEncoding = inputEncoding;
			this.state = HtmlReader.State.Begin;
		}

		public HtmlReader(TextReader input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			this.input = input;
			this.inputEncoding = Encoding.Unicode;
			this.state = HtmlReader.State.Begin;
		}

		public Encoding InputEncoding
		{
			get
			{
				return this.inputEncoding;
			}
			set
			{
				this.AssertNotLocked();
				this.inputEncoding = value;
			}
		}

		public bool DetectEncodingFromByteOrderMark
		{
			get
			{
				return this.detectEncodingFromByteOrderMark;
			}
			set
			{
				this.AssertNotLocked();
				this.detectEncodingFromByteOrderMark = value;
			}
		}

		public bool NormalizeHtml
		{
			get
			{
				return this.normalizeInputHtml;
			}
			set
			{
				this.AssertNotLocked();
				this.normalizeInputHtml = value;
			}
		}

		public HtmlTokenKind TokenKind
		{
			get
			{
				this.AssertInToken();
				return this.tokenKind;
			}
		}

		public bool ReadNextToken()
		{
			this.AssertNotDisposed();
			if (this.state == HtmlReader.State.EndOfFile)
			{
				return false;
			}
			if (!this.locked)
			{
				this.InitializeAndLock();
			}
			if (this.state == HtmlReader.State.Text)
			{
				for (;;)
				{
					this.ParseToken();
					if (this.parserTokenId != HtmlTokenId.Text)
					{
						if (!this.literalTags || this.parserTokenId != HtmlTokenId.Tag)
						{
							break;
						}
						if (this.parserToken.TagIndex >= HtmlTagIndex.Unknown)
						{
							break;
						}
					}
				}
			}
			else if (this.state >= HtmlReader.State.SpecialTag)
			{
				while (!this.parserToken.IsTagEnd)
				{
					this.ParseToken();
				}
				if (this.parserToken.TagIndex > HtmlTagIndex.Unknown && !this.parserToken.IsEndTag && HtmlDtd.tags[(int)this.parserToken.TagIndex].Scope != HtmlDtd.TagScope.EMPTY && !this.parserToken.IsEmptyScope)
				{
					this.depth++;
				}
				if (!this.parserToken.IsEndTag && (byte)(HtmlDtd.tags[(int)this.parserToken.TagIndex].Literal & HtmlDtd.Literal.Tags) != 0)
				{
					this.literalTags = true;
				}
				this.ParseToken();
			}
			else
			{
				if (this.state == HtmlReader.State.OverlappedClose)
				{
					this.depth -= this.parserToken.Argument;
				}
				this.ParseToken();
			}
			for (;;)
			{
				switch (this.parserTokenId)
				{
				case HtmlTokenId.Text:
					goto IL_15A;
				case HtmlTokenId.EncodingChange:
					this.ParseToken();
					continue;
				case HtmlTokenId.Tag:
					if (this.parserToken.TagIndex < HtmlTagIndex.Unknown)
					{
						goto Block_17;
					}
					if (this.parserToken.TagIndex == HtmlTagIndex.TC)
					{
						this.ParseToken();
						continue;
					}
					goto IL_1E6;
				case HtmlTokenId.Restart:
					continue;
				case HtmlTokenId.OverlappedClose:
					goto IL_2DC;
				case HtmlTokenId.OverlappedReopen:
					goto IL_2EC;
				}
				break;
			}
			this.state = HtmlReader.State.EndOfFile;
			return false;
			IL_15A:
			this.state = HtmlReader.State.Text;
			this.tokenKind = HtmlTokenKind.Text;
			this.parserToken.Text.Rewind();
			return true;
			Block_17:
			if (this.literalTags)
			{
				this.state = HtmlReader.State.Text;
				this.tokenKind = HtmlTokenKind.Text;
			}
			else
			{
				this.state = HtmlReader.State.SpecialTag;
				this.tokenKind = HtmlTokenKind.SpecialTag;
			}
			this.parserToken.Text.Rewind();
			return true;
			IL_1E6:
			if (this.parserToken.IsTagNameEmpty && this.parserToken.TagIndex == HtmlTagIndex.Unknown)
			{
				this.state = HtmlReader.State.SpecialTag;
				this.tokenKind = HtmlTokenKind.SpecialTag;
				this.parserToken.Text.Rewind();
				return true;
			}
			this.state = HtmlReader.State.BeginTag;
			if (this.parserToken.IsEndTag)
			{
				this.tokenKind = HtmlTokenKind.EndTag;
				if ((byte)(HtmlDtd.tags[(int)this.parserToken.TagIndex].Literal & HtmlDtd.Literal.Tags) != 0)
				{
					this.literalTags = false;
				}
			}
			else if (this.parserToken.TagIndex > HtmlTagIndex.Unknown && HtmlDtd.tags[(int)this.parserToken.TagIndex].Scope == HtmlDtd.TagScope.EMPTY)
			{
				this.tokenKind = HtmlTokenKind.EmptyElementTag;
			}
			else
			{
				this.tokenKind = HtmlTokenKind.StartTag;
			}
			this.parserToken.Text.Rewind();
			if (this.parserToken.IsEndTag && this.parserToken.TagIndex != HtmlTagIndex.Unknown)
			{
				this.depth--;
				return true;
			}
			return true;
			IL_2DC:
			this.state = HtmlReader.State.OverlappedClose;
			this.tokenKind = HtmlTokenKind.OverlappedClose;
			return true;
			IL_2EC:
			this.depth += this.parserToken.Argument;
			this.state = HtmlReader.State.OverlappedReopen;
			this.tokenKind = HtmlTokenKind.OverlappedReopen;
			return true;
		}

		public int Depth
		{
			get
			{
				this.AssertNotDisposed();
				return this.depth;
			}
		}

		public int CurrentOffset
		{
			get
			{
				this.AssertNotDisposed();
				return this.parser.CurrentOffset;
			}
		}

		public int OverlappedDepth
		{
			get
			{
				if (this.state != HtmlReader.State.OverlappedClose && this.state != HtmlReader.State.OverlappedReopen)
				{
					this.AssertInToken();
					throw new InvalidOperationException("Reader must be positioned on OverlappedClose or OverlappedReopen token");
				}
				return this.parserToken.Argument;
			}
		}

		public HtmlTagId TagId
		{
			get
			{
				this.AssertInTag();
				return HtmlNameData.Names[(int)this.parserToken.NameIndex].PublicTagId;
			}
		}

		public bool TagInjectedByNormalizer
		{
			get
			{
				this.AssertInTag();
				if (this.state != HtmlReader.State.BeginTag)
				{
					throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
				}
				return this.parserToken.Argument == 1;
			}
		}

		public bool TagNameIsLong
		{
			get
			{
				this.AssertInTag();
				if (this.state != HtmlReader.State.BeginTag)
				{
					throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
				}
				return this.parserToken.NameIndex == HtmlNameIndex.Unknown && this.parserToken.IsTagNameBegin && !this.parserToken.IsTagNameEnd;
			}
		}

		public string ReadTagName()
		{
			if (this.state != HtmlReader.State.BeginTag)
			{
				this.AssertInTag();
				throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
			}
			string result;
			if (this.parserToken.NameIndex != HtmlNameIndex.Unknown)
			{
				result = HtmlNameData.Names[(int)this.parserToken.NameIndex].Name;
			}
			else
			{
				if (this.parserToken.IsTagNameEnd)
				{
					return this.parserToken.Name.GetString(int.MaxValue);
				}
				StringBuildSink stringBuildSink = this.GetStringBuildSink();
				this.parserToken.Name.WriteTo(stringBuildSink);
				do
				{
					this.ParseToken();
					this.parserToken.Name.WriteTo(stringBuildSink);
				}
				while (!this.parserToken.IsTagNameEnd);
				result = stringBuildSink.ToString();
			}
			this.state = HtmlReader.State.EndTagName;
			return result;
		}

		public int ReadTagName(char[] buffer, int offset, int count)
		{
			this.AssertInTag();
			if (this.state > HtmlReader.State.EndTagName)
			{
				throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", TextConvertersStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
			}
			int num = 0;
			if (this.state != HtmlReader.State.EndTagName)
			{
				if (this.state == HtmlReader.State.BeginTag)
				{
					this.state = HtmlReader.State.ReadTagName;
					this.parserToken.Name.Rewind();
				}
				while (count != 0)
				{
					int num2 = this.parserToken.Name.Read(buffer, offset, count);
					if (num2 == 0)
					{
						if (this.parserToken.IsTagNameEnd)
						{
							this.state = HtmlReader.State.EndTagName;
							break;
						}
						this.ParseToken();
						this.parserToken.Name.Rewind();
					}
					else
					{
						offset += num2;
						count -= num2;
						num += num2;
					}
				}
			}
			return num;
		}

		public static string CharsetFromString(string arg, bool lookForWordCharset)
		{
			for (int i = 0; i < arg.Length; i++)
			{
				while (i < arg.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[i])))
				{
					i++;
				}
				if (i == arg.Length)
				{
					break;
				}
				if (!lookForWordCharset || (arg.Length - i >= 7 && string.Equals(arg.Substring(i, 7), "charset", StringComparison.OrdinalIgnoreCase)))
				{
					if (lookForWordCharset)
					{
						i = arg.IndexOf('=', i + 7);
						if (i < 0)
						{
							break;
						}
						i++;
						while (i < arg.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[i])))
						{
							i++;
						}
						if (i == arg.Length)
						{
							break;
						}
					}
					int num = i;
					while (num < arg.Length && arg[num] != ';' && !ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(arg[num])))
					{
						num++;
					}
					return arg.Substring(i, num - i);
				}
				i = arg.IndexOf(';', i);
				if (i < 0)
				{
					break;
				}
			}
			return null;
		}

		internal void WriteTagNameTo(ITextSink sink)
		{
			if (this.state != HtmlReader.State.BeginTag)
			{
				this.AssertInTag();
				throw new InvalidOperationException("Reader must be positioned at the beginning of a StartTag, EndTag or EmptyElementTag token");
			}
			for (;;)
			{
				this.parserToken.Name.WriteTo(sink);
				if (this.parserToken.IsTagNameEnd)
				{
					break;
				}
				this.ParseToken();
			}
			this.state = HtmlReader.State.EndTagName;
		}

		public HtmlAttributeReader AttributeReader
		{
			get
			{
				this.AssertInTag();
				if (this.state == HtmlReader.State.ReadTag)
				{
					throw new InvalidOperationException("Cannot read attributes after reading tag as a markup text");
				}
				return new HtmlAttributeReader(this);
			}
		}

		public int ReadText(char[] buffer, int offset, int count)
		{
			if (this.state == HtmlReader.State.EndText)
			{
				return 0;
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", TextConvertersStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
			}
			if (this.state != HtmlReader.State.Text)
			{
				this.AssertInToken();
				throw new InvalidOperationException("Reader must be positioned on a Text token");
			}
			int num = 0;
			while (count != 0)
			{
				int num2 = this.parserToken.Text.Read(buffer, offset, count);
				if (num2 == 0)
				{
					HtmlTokenId htmlTokenId = this.PreviewNextToken();
					if (htmlTokenId != HtmlTokenId.Text && (!this.literalTags || htmlTokenId != HtmlTokenId.Tag || this.nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
					{
						this.state = HtmlReader.State.EndText;
						break;
					}
					this.ParseToken();
					this.parserToken.Text.Rewind();
				}
				else
				{
					offset += num2;
					count -= num2;
					num += num2;
				}
			}
			return num;
		}

		internal void WriteTextTo(ITextSink sink)
		{
			if (this.state != HtmlReader.State.Text)
			{
				this.AssertInToken();
				throw new InvalidOperationException("Reader must be positioned on a Text token");
			}
			for (;;)
			{
				this.parserToken.Text.WriteTo(sink);
				HtmlTokenId htmlTokenId = this.PreviewNextToken();
				if (htmlTokenId != HtmlTokenId.Text && (!this.literalTags || htmlTokenId != HtmlTokenId.Tag || this.nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
				{
					break;
				}
				this.ParseToken();
			}
			this.state = HtmlReader.State.EndText;
		}

		public int ReadMarkupText(char[] buffer, int offset, int count)
		{
			if (this.state == HtmlReader.State.EndTag || this.state == HtmlReader.State.EndSpecialTag || this.state == HtmlReader.State.EndText)
			{
				return 0;
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", TextConvertersStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
			}
			if (this.state == HtmlReader.State.BeginTag)
			{
				this.state = HtmlReader.State.ReadTag;
			}
			else if (this.state != HtmlReader.State.SpecialTag && this.state != HtmlReader.State.ReadTag && this.state != HtmlReader.State.Text)
			{
				this.AssertInToken();
				if (this.state > HtmlReader.State.BeginTag)
				{
					throw new InvalidOperationException("Cannot read tag content as markup text after accessing tag name or attributes");
				}
				throw new InvalidOperationException("Reader must be positioned on Text, StartTag, EndTag, EmptyElementTag or SpecialTag token");
			}
			int num = 0;
			while (count != 0)
			{
				int num2 = this.parserToken.Text.ReadOriginal(buffer, offset, count);
				if (num2 == 0)
				{
					if (this.state == HtmlReader.State.SpecialTag)
					{
						if (this.parserToken.IsTagEnd)
						{
							this.state = HtmlReader.State.EndSpecialTag;
							break;
						}
					}
					else if (this.state == HtmlReader.State.ReadTag)
					{
						if (this.parserToken.IsTagEnd)
						{
							this.state = HtmlReader.State.EndTag;
							break;
						}
					}
					else
					{
						HtmlTokenId htmlTokenId = this.PreviewNextToken();
						if (htmlTokenId != HtmlTokenId.Text && (!this.literalTags || htmlTokenId != HtmlTokenId.Tag || this.nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
						{
							this.state = HtmlReader.State.EndText;
							break;
						}
					}
					this.ParseToken();
					this.parserToken.Text.Rewind();
				}
				else
				{
					offset += num2;
					count -= num2;
					num += num2;
				}
			}
			return num;
		}

		internal void WriteMarkupTextTo(ITextSink sink)
		{
			if (this.state == HtmlReader.State.BeginTag)
			{
				this.state = HtmlReader.State.ReadTag;
			}
			else if (this.state != HtmlReader.State.SpecialTag && this.state != HtmlReader.State.ReadTag && this.state != HtmlReader.State.Text)
			{
				this.AssertInToken();
				if (this.state > HtmlReader.State.BeginTag)
				{
					throw new InvalidOperationException("Cannot read tag content as markup text after accessing tag name or attributes");
				}
				throw new InvalidOperationException("Reader must be positioned on Text, StartTag, EndTag, EmptyElementTag or SpecialTag token");
			}
			for (;;)
			{
				this.parserToken.Text.WriteOriginalTo(sink);
				if (this.state == HtmlReader.State.SpecialTag)
				{
					if (this.parserToken.IsTagEnd)
					{
						break;
					}
				}
				else if (this.state == HtmlReader.State.ReadTag)
				{
					if (this.parserToken.IsTagEnd)
					{
						goto Block_9;
					}
				}
				else
				{
					HtmlTokenId htmlTokenId = this.PreviewNextToken();
					if (htmlTokenId != HtmlTokenId.Text && (!this.literalTags || htmlTokenId != HtmlTokenId.Tag || this.nextParserToken.TagIndex >= HtmlTagIndex.Unknown))
					{
						goto IL_CD;
					}
				}
				this.ParseToken();
			}
			this.state = HtmlReader.State.EndSpecialTag;
			return;
			Block_9:
			this.state = HtmlReader.State.EndTag;
			return;
			IL_CD:
			this.state = HtmlReader.State.EndText;
		}

		public void Close()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.parser != null && this.parser is IDisposable)
				{
					((IDisposable)this.parser).Dispose();
				}
				if (this.input != null && this.input is IDisposable)
				{
					((IDisposable)this.input).Dispose();
				}
			}
			this.parser = null;
			this.input = null;
			this.stringBuildSink = null;
			this.parserToken = null;
			this.nextParserToken = null;
			this.state = HtmlReader.State.Disposed;
		}

		internal HtmlReader SetInputEncoding(Encoding value)
		{
			this.InputEncoding = value;
			return this;
		}

		internal HtmlReader SetDetectEncodingFromByteOrderMark(bool value)
		{
			this.DetectEncodingFromByteOrderMark = value;
			return this;
		}

		internal HtmlReader SetNormalizeHtml(bool value)
		{
			this.NormalizeHtml = value;
			return this;
		}

		internal HtmlReader SetTestBoundaryConditions(bool value)
		{
			this.testBoundaryConditions = value;
			return this;
		}

		internal HtmlReader SetTestTraceStream(Stream value)
		{
			this.testTraceStream = value;
			return this;
		}

		internal HtmlReader SetTestTraceShowTokenNum(bool value)
		{
			this.testTraceShowTokenNum = value;
			return this;
		}

		internal HtmlReader SetTestTraceStopOnTokenNum(int value)
		{
			this.testTraceStopOnTokenNum = value;
			return this;
		}

		internal HtmlReader SetTestNormalizerTraceStream(Stream value)
		{
			this.testNormalizerTraceStream = value;
			return this;
		}

		internal HtmlReader SetTestNormalizerTraceShowTokenNum(bool value)
		{
			this.testNormalizerTraceShowTokenNum = value;
			return this;
		}

		internal HtmlReader SetTestNormalizerTraceStopOnTokenNum(int value)
		{
			this.testNormalizerTraceStopOnTokenNum = value;
			return this;
		}

		private void InitializeAndLock()
		{
			this.locked = true;
			ConverterInput converterInput;
			if (this.input is Stream)
			{
				if (this.inputEncoding == null)
				{
					throw new InvalidOperationException(TextConvertersStrings.InputEncodingRequired);
				}
				converterInput = new ConverterDecodingInput((Stream)this.input, false, this.inputEncoding, this.detectEncodingFromByteOrderMark, TextConvertersDefaults.MaxTokenSize(this.testBoundaryConditions), TextConvertersDefaults.MaxHtmlMetaRestartOffset(this.testBoundaryConditions), 16384, this.testBoundaryConditions, this, null);
			}
			else
			{
				converterInput = new ConverterUnicodeInput(this.input, false, TextConvertersDefaults.MaxTokenSize(this.testBoundaryConditions), this.testBoundaryConditions, null);
			}
			HtmlParser htmlParser = new HtmlParser(converterInput, false, false, TextConvertersDefaults.MaxTokenRuns(this.testBoundaryConditions), TextConvertersDefaults.MaxHtmlAttributes(this.testBoundaryConditions), this.testBoundaryConditions);
			if (this.normalizeInputHtml)
			{
				this.parser = new HtmlNormalizingParser(htmlParser, null, false, TextConvertersDefaults.MaxHtmlNormalizerNesting(this.testBoundaryConditions), this.testBoundaryConditions, this.testNormalizerTraceStream, this.testNormalizerTraceShowTokenNum, this.testNormalizerTraceStopOnTokenNum);
				return;
			}
			this.parser = htmlParser;
		}

		bool IRestartable.CanRestart()
		{
			return false;
		}

		void IRestartable.Restart()
		{
		}

		void IRestartable.DisableRestart()
		{
		}

		void IResultsFeedback.Set(ConfigParameter parameterId, object val)
		{
			if (parameterId != ConfigParameter.InputEncoding)
			{
				return;
			}
			this.inputEncoding = (Encoding)val;
		}

		private void ParseToken()
		{
			if (this.nextParserToken != null)
			{
				this.parserToken = this.nextParserToken;
				this.parserTokenId = this.parserToken.HtmlTokenId;
				this.nextParserToken = null;
				return;
			}
			this.parserTokenId = this.parser.Parse();
			this.parserToken = this.parser.Token;
		}

		private HtmlTokenId PreviewNextToken()
		{
			if (this.nextParserToken == null)
			{
				this.parser.Parse();
				this.nextParserToken = this.parser.Token;
			}
			return this.nextParserToken.HtmlTokenId;
		}

		private StringBuildSink GetStringBuildSink()
		{
			if (this.stringBuildSink == null)
			{
				this.stringBuildSink = new StringBuildSink();
			}
			this.stringBuildSink.Reset(int.MaxValue);
			return this.stringBuildSink;
		}

		internal bool AttributeReader_ReadNextAttribute()
		{
			if (this.state == HtmlReader.State.EndTag)
			{
				return false;
			}
			this.AssertInTag();
			if (this.state == HtmlReader.State.ReadTag)
			{
				throw new InvalidOperationException("Cannot read attributes after reading tag as markup text");
			}
			for (;;)
			{
				if (this.state >= HtmlReader.State.BeginTag && this.state < HtmlReader.State.BeginAttribute)
				{
					while (this.parserToken.Attributes.Count == 0 && !this.parserToken.IsTagEnd)
					{
						this.ParseToken();
					}
					if (this.parserToken.Attributes.Count == 0)
					{
						break;
					}
					this.currentAttribute = 0;
					this.state = HtmlReader.State.BeginAttribute;
				}
				else if (++this.currentAttribute == this.parserToken.Attributes.Count)
				{
					if (this.parserToken.IsTagEnd)
					{
						goto Block_8;
					}
					for (;;)
					{
						this.ParseToken();
						if (this.parserToken.Attributes.Count != 0 && (this.parserToken.Attributes[0].IsAttrBegin || this.parserToken.Attributes.Count > 1))
						{
							break;
						}
						if (this.parserToken.IsTagEnd)
						{
							goto Block_11;
						}
					}
					this.currentAttribute = 0;
					if (!this.parserToken.Attributes[0].IsAttrBegin)
					{
						this.currentAttribute++;
					}
				}
				if (!this.parserToken.Attributes[this.currentAttribute].IsAttrEmptyName)
				{
					goto Block_13;
				}
			}
			this.state = HtmlReader.State.EndTag;
			return false;
			Block_8:
			this.state = HtmlReader.State.EndTag;
			return false;
			Block_11:
			this.state = HtmlReader.State.EndTag;
			return false;
			Block_13:
			this.state = HtmlReader.State.BeginAttribute;
			return true;
		}

		internal HtmlAttributeId AttributeReader_GetCurrentAttributeId()
		{
			this.AssertInAttribute();
			return HtmlNameData.Names[(int)this.parserToken.Attributes[this.currentAttribute].NameIndex].PublicAttributeId;
		}

		internal bool AttributeReader_CurrentAttributeNameIsLong()
		{
			if (this.state != HtmlReader.State.BeginAttribute)
			{
				this.AssertInAttribute();
				throw new InvalidOperationException();
			}
			return this.parserToken.Attributes[this.currentAttribute].NameIndex == HtmlNameIndex.Unknown && this.parserToken.Attributes[this.currentAttribute].IsAttrBegin && !this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd;
		}

		internal string AttributeReader_ReadCurrentAttributeName()
		{
			if (this.state != HtmlReader.State.BeginAttribute)
			{
				this.AssertInAttribute();
				throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
			}
			string result;
			if (this.parserToken.Attributes[this.currentAttribute].NameIndex != HtmlNameIndex.Unknown)
			{
				result = HtmlNameData.Names[(int)this.parserToken.Attributes[this.currentAttribute].NameIndex].Name;
			}
			else
			{
				if (this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd)
				{
					return this.parserToken.Attributes[this.currentAttribute].Name.GetString(int.MaxValue);
				}
				StringBuildSink stringBuildSink = this.GetStringBuildSink();
				this.parserToken.Attributes[this.currentAttribute].Name.WriteTo(stringBuildSink);
				do
				{
					this.ParseToken();
					this.currentAttribute = 0;
					this.parserToken.Attributes[this.currentAttribute].Name.WriteTo(stringBuildSink);
				}
				while (!this.parserToken.Attributes[0].IsAttrNameEnd);
				result = stringBuildSink.ToString();
			}
			this.state = HtmlReader.State.EndAttributeName;
			return result;
		}

		internal int AttributeReader_ReadCurrentAttributeName(char[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", TextConvertersStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
			}
			if (this.state < HtmlReader.State.BeginAttribute || this.state > HtmlReader.State.EndAttributeName)
			{
				this.AssertInAttribute();
				throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
			}
			int num = 0;
			if (this.state != HtmlReader.State.EndAttributeName)
			{
				if (this.state == HtmlReader.State.BeginAttribute)
				{
					this.state = HtmlReader.State.ReadAttributeName;
					this.parserToken.Attributes[this.currentAttribute].Name.Rewind();
				}
				while (count != 0)
				{
					int num2 = this.parserToken.Attributes[this.currentAttribute].Name.Read(buffer, offset, count);
					if (num2 == 0)
					{
						if (this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd)
						{
							this.state = HtmlReader.State.EndAttributeName;
							break;
						}
						this.ParseToken();
						this.currentAttribute = 0;
						this.parserToken.Attributes[this.currentAttribute].Name.Rewind();
					}
					else
					{
						offset += num2;
						count -= num2;
						num += num2;
					}
				}
			}
			return num;
		}

		internal void AttributeReader_WriteCurrentAttributeNameTo(ITextSink sink)
		{
			if (this.state != HtmlReader.State.BeginAttribute)
			{
				this.AssertInAttribute();
				throw new InvalidOperationException("Reader must be positioned at the beginning of attribute.");
			}
			for (;;)
			{
				this.parserToken.Attributes[this.currentAttribute].Name.WriteTo(sink);
				if (this.parserToken.Attributes[this.currentAttribute].IsAttrNameEnd)
				{
					break;
				}
				this.ParseToken();
				this.currentAttribute = 0;
			}
			this.state = HtmlReader.State.EndAttributeName;
		}

		internal bool AttributeReader_CurrentAttributeHasValue()
		{
			if (this.state != HtmlReader.State.BeginAttributeValue)
			{
				this.AssertInAttribute();
				if (this.state > HtmlReader.State.BeginAttributeValue)
				{
					throw new InvalidOperationException("Reader must be positioned before attribute value");
				}
				if (!this.SkipToAttributeValue())
				{
					this.state = HtmlReader.State.EndAttributeName;
					return false;
				}
				this.state = HtmlReader.State.BeginAttributeValue;
			}
			return true;
		}

		internal bool AttributeReader_CurrentAttributeValueIsLong()
		{
			if (this.state != HtmlReader.State.BeginAttributeValue)
			{
				this.AssertInAttribute();
				if (this.state > HtmlReader.State.BeginAttributeValue)
				{
					throw new InvalidOperationException("Reader must be positioned before attribute value");
				}
				if (!this.SkipToAttributeValue())
				{
					this.state = HtmlReader.State.EndAttributeName;
					return false;
				}
				this.state = HtmlReader.State.BeginAttributeValue;
			}
			return this.parserToken.Attributes[this.currentAttribute].IsAttrValueBegin && !this.parserToken.Attributes[this.currentAttribute].IsAttrEnd;
		}

		internal string AttributeReader_ReadCurrentAttributeValue()
		{
			if (this.state != HtmlReader.State.BeginAttributeValue)
			{
				this.AssertInAttribute();
				if (this.state > HtmlReader.State.BeginAttributeValue)
				{
					throw new InvalidOperationException("Reader must be positioned before attribute value");
				}
				if (!this.SkipToAttributeValue())
				{
					this.state = HtmlReader.State.EndAttribute;
					return null;
				}
			}
			if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
			{
				return this.parserToken.Attributes[this.currentAttribute].Value.GetString(int.MaxValue);
			}
			StringBuildSink stringBuildSink = this.GetStringBuildSink();
			this.parserToken.Attributes[this.currentAttribute].Value.WriteTo(stringBuildSink);
			do
			{
				this.ParseToken();
				this.currentAttribute = 0;
				this.parserToken.Attributes[0].Value.WriteTo(stringBuildSink);
			}
			while (!this.parserToken.Attributes[0].IsAttrEnd);
			this.state = HtmlReader.State.EndAttribute;
			return stringBuildSink.ToString();
		}

		internal int AttributeReader_ReadCurrentAttributeValue(char[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", TextConvertersStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", TextConvertersStrings.CountTooLarge);
			}
			this.AssertInAttribute();
			if (this.state < HtmlReader.State.BeginAttributeValue)
			{
				if (!this.SkipToAttributeValue())
				{
					this.state = HtmlReader.State.EndAttribute;
					return 0;
				}
				this.state = HtmlReader.State.BeginAttributeValue;
			}
			int num = 0;
			if (this.state != HtmlReader.State.EndAttribute)
			{
				if (this.state == HtmlReader.State.BeginAttributeValue)
				{
					this.state = HtmlReader.State.ReadAttributeValue;
					this.parserToken.Attributes[this.currentAttribute].Value.Rewind();
				}
				while (count != 0)
				{
					int num2 = this.parserToken.Attributes[this.currentAttribute].Value.Read(buffer, offset, count);
					if (num2 == 0)
					{
						if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
						{
							this.state = HtmlReader.State.EndAttribute;
							break;
						}
						this.ParseToken();
						this.currentAttribute = 0;
						this.parserToken.Attributes[this.currentAttribute].Value.Rewind();
					}
					else
					{
						offset += num2;
						count -= num2;
						num += num2;
					}
				}
			}
			return num;
		}

		internal void AttributeReader_WriteCurrentAttributeValueTo(ITextSink sink)
		{
			if (this.state != HtmlReader.State.BeginAttributeValue)
			{
				this.AssertInAttribute();
				if (this.state > HtmlReader.State.BeginAttributeValue)
				{
					throw new InvalidOperationException("Reader must be positioned before attribute value");
				}
				if (!this.SkipToAttributeValue())
				{
					this.state = HtmlReader.State.EndAttribute;
					return;
				}
			}
			for (;;)
			{
				this.parserToken.Attributes[this.currentAttribute].Value.WriteTo(sink);
				if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
				{
					break;
				}
				this.ParseToken();
				this.currentAttribute = 0;
			}
			this.state = HtmlReader.State.EndAttribute;
		}

		private bool SkipToAttributeValue()
		{
			if (!this.parserToken.Attributes[this.currentAttribute].IsAttrValueBegin)
			{
				if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
				{
					return false;
				}
				do
				{
					this.ParseToken();
				}
				while (!this.parserToken.Attributes[0].IsAttrValueBegin && !this.parserToken.Attributes[0].IsAttrEnd);
				if (this.parserToken.Attributes[this.currentAttribute].IsAttrEnd)
				{
					return false;
				}
			}
			return true;
		}

		private void AssertNotLocked()
		{
			this.AssertNotDisposed();
			if (this.locked)
			{
				throw new InvalidOperationException("Cannot set reader properties after reading a first token");
			}
		}

		private void AssertNotDisposed()
		{
			if (this.state == HtmlReader.State.Disposed)
			{
				throw new ObjectDisposedException("HtmlReader");
			}
		}

		private void AssertInToken()
		{
			if (this.state <= HtmlReader.State.Begin)
			{
				this.AssertNotDisposed();
				throw new InvalidOperationException("Reader must be positioned inside a valid token");
			}
		}

		private void AssertInTag()
		{
			if (this.state < HtmlReader.State.BeginTag)
			{
				this.AssertInToken();
				throw new InvalidOperationException("Reader must be positioned inside a StartTag, EndTag or EmptyElementTag token");
			}
		}

		private void AssertInAttribute()
		{
			if (this.state < HtmlReader.State.BeginAttribute || this.state > HtmlReader.State.EndAttribute)
			{
				this.AssertInTag();
				throw new InvalidOperationException("Reader must be positioned inside attribute");
			}
		}

		private const int InputBufferSize = 16384;

		private Encoding inputEncoding;

		private bool detectEncodingFromByteOrderMark;

		private bool normalizeInputHtml;

		private bool testBoundaryConditions;

		private Stream testTraceStream;

		private bool testTraceShowTokenNum = true;

		private int testTraceStopOnTokenNum;

		private Stream testNormalizerTraceStream;

		private bool testNormalizerTraceShowTokenNum = true;

		private int testNormalizerTraceStopOnTokenNum;

		private bool locked;

		private object input;

		private IHtmlParser parser;

		private HtmlTokenId parserTokenId;

		private HtmlToken parserToken;

		private HtmlToken nextParserToken;

		private int depth;

		private StringBuildSink stringBuildSink;

		private int currentAttribute;

		private bool literalTags;

		private HtmlReader.State state;

		private HtmlTokenKind tokenKind;

		private enum State : byte
		{
			Disposed,
			EndOfFile,
			Begin,
			Text,
			EndText,
			OverlappedClose,
			OverlappedReopen,
			SpecialTag,
			EndSpecialTag,
			BeginTag,
			ReadTagName,
			EndTagName,
			BeginAttribute,
			ReadAttributeName,
			EndAttributeName,
			BeginAttributeValue,
			ReadAttributeValue,
			EndAttribute,
			ReadTag,
			EndTag
		}
	}
}
