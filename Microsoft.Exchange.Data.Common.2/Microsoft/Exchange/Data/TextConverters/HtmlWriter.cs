using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters
{
	public class HtmlWriter : IRestartable, IFallback, IDisposable, ITextSinkEx, ITextSink
	{
		public HtmlWriter(Stream output, Encoding outputEncoding)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (outputEncoding == null)
			{
				throw new ArgumentNullException("outputEncoding");
			}
			this.output = new ConverterEncodingOutput(output, true, false, outputEncoding, false, false, null);
			this.autoNewLines = true;
		}

		public HtmlWriter(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			this.output = new ConverterUnicodeOutput(output, true, false);
			this.autoNewLines = true;
		}

		internal HtmlWriter(ConverterOutput output, bool filterHtml, bool autoNewLines)
		{
			this.output = output;
			this.filterHtml = filterHtml;
			this.autoNewLines = autoNewLines;
		}

		public HtmlWriterState WriterState
		{
			get
			{
				if (this.outputState == HtmlWriter.OutputState.OutsideTag)
				{
					return HtmlWriterState.Default;
				}
				if (this.outputState >= HtmlWriter.OutputState.WritingAttributeName)
				{
					return HtmlWriterState.Attribute;
				}
				return HtmlWriterState.Tag;
			}
		}

		bool ITextSink.IsEnough
		{
			get
			{
				return false;
			}
		}

		internal bool HasEncoding
		{
			get
			{
				return this.output is ConverterEncodingOutput;
			}
		}

		internal bool CodePageSameAsInput
		{
			get
			{
				return (this.output as ConverterEncodingOutput).CodePageSameAsInput;
			}
		}

		internal Encoding Encoding
		{
			get
			{
				return (this.output as ConverterEncodingOutput).Encoding;
			}
			set
			{
				(this.output as ConverterEncodingOutput).Encoding = value;
			}
		}

		internal bool CanAcceptMore
		{
			get
			{
				return this.output.CanAcceptMore;
			}
		}

		internal bool IsTagOpen
		{
			get
			{
				return this.outputState != HtmlWriter.OutputState.OutsideTag;
			}
		}

		internal int LineLength
		{
			get
			{
				return this.lineLength;
			}
		}

		internal int LiteralWhitespaceNesting
		{
			get
			{
				return this.literalWhitespaceNesting;
			}
		}

		internal bool IsCopyPending
		{
			get
			{
				return this.copyPending;
			}
		}

		public void Flush()
		{
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			this.output.Flush();
		}

		public void WriteTag(HtmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (reader.TagId != HtmlTagId.Unknown)
			{
				this.WriteTagBegin(HtmlNameData.TagIndex[(int)reader.TagId], null, reader.TokenKind == HtmlTokenKind.EndTag, false, false);
			}
			else
			{
				this.WriteTagBegin(HtmlNameIndex.Unknown, null, reader.TokenKind == HtmlTokenKind.EndTag, false, false);
				reader.WriteTagNameTo(this.WriteTagName());
			}
			this.isEmptyScopeTag = (reader.TokenKind == HtmlTokenKind.EmptyElementTag);
			if (reader.TokenKind == HtmlTokenKind.StartTag || reader.TokenKind == HtmlTokenKind.EmptyElementTag)
			{
				HtmlAttributeReader attributeReader = reader.AttributeReader;
				while (attributeReader.ReadNext())
				{
					if (attributeReader.Id != HtmlAttributeId.Unknown)
					{
						this.OutputAttributeName(HtmlNameData.Names[(int)HtmlNameData.attributeIndex[(int)attributeReader.Id]].Name);
					}
					else
					{
						attributeReader.WriteNameTo(this.WriteAttributeName());
					}
					if (attributeReader.HasValue)
					{
						attributeReader.WriteValueTo(this.WriteAttributeValue());
					}
					this.OutputAttributeEnd();
					this.outputState = HtmlWriter.OutputState.BeforeAttribute;
				}
			}
		}

		public void WriteStartTag(HtmlTagId id)
		{
			this.WriteTag(id, false);
		}

		public void WriteStartTag(string name)
		{
			this.WriteTag(name, false);
		}

		public void WriteEndTag(HtmlTagId id)
		{
			this.WriteTag(id, true);
			this.WriteTagEnd();
		}

		public void WriteEndTag(string name)
		{
			this.WriteTag(name, true);
			this.WriteTagEnd();
		}

		public void WriteEmptyElementTag(HtmlTagId id)
		{
			this.WriteTag(id, false);
			this.isEmptyScopeTag = true;
		}

		public void WriteEmptyElementTag(string name)
		{
			this.WriteTag(name, false);
			this.isEmptyScopeTag = true;
		}

		public void WriteAttribute(HtmlAttributeId id, string value)
		{
			if (id < HtmlAttributeId.Unknown || (int)id >= HtmlNameData.attributeIndex.Length)
			{
				throw new ArgumentException(TextConvertersStrings.AttributeIdInvalid, "id");
			}
			if (id == HtmlAttributeId.Unknown)
			{
				throw new ArgumentException(TextConvertersStrings.AttributeIdIsUnknown, "id");
			}
			if (this.outputState < HtmlWriter.OutputState.WritingTagName)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.isEndTag)
			{
				throw new InvalidOperationException(TextConvertersStrings.EndTagCannotHaveAttributes);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
			{
				this.OutputAttributeEnd();
			}
			this.OutputAttributeName(HtmlNameData.Names[(int)HtmlNameData.attributeIndex[(int)id]].Name);
			if (value != null)
			{
				this.OutputAttributeValue(value);
				this.OutputAttributeEnd();
			}
			this.outputState = HtmlWriter.OutputState.BeforeAttribute;
		}

		public void WriteAttribute(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(TextConvertersStrings.AttributeNameIsEmpty, "name");
			}
			if (this.outputState < HtmlWriter.OutputState.WritingTagName)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.isEndTag)
			{
				throw new InvalidOperationException(TextConvertersStrings.EndTagCannotHaveAttributes);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
			{
				this.OutputAttributeEnd();
			}
			this.OutputAttributeName(name);
			if (value != null)
			{
				this.OutputAttributeValue(value);
				this.OutputAttributeEnd();
			}
			this.outputState = HtmlWriter.OutputState.BeforeAttribute;
		}

		public void WriteAttribute(HtmlAttributeId id, char[] buffer, int index, int count)
		{
			if (id < HtmlAttributeId.Unknown || (int)id >= HtmlNameData.attributeIndex.Length)
			{
				throw new ArgumentException(TextConvertersStrings.AttributeIdInvalid, "id");
			}
			if (id == HtmlAttributeId.Unknown)
			{
				throw new ArgumentException(TextConvertersStrings.AttributeIdIsUnknown, "id");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || index > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0 || count > buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.outputState < HtmlWriter.OutputState.WritingTagName)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.isEndTag)
			{
				throw new InvalidOperationException(TextConvertersStrings.EndTagCannotHaveAttributes);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
			{
				this.OutputAttributeEnd();
			}
			this.OutputAttributeName(HtmlNameData.Names[(int)HtmlNameData.attributeIndex[(int)id]].Name);
			this.OutputAttributeValue(buffer, index, count);
			this.OutputAttributeEnd();
			this.outputState = HtmlWriter.OutputState.BeforeAttribute;
		}

		public void WriteAttribute(string name, char[] buffer, int index, int count)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(TextConvertersStrings.AttributeNameIsEmpty, "name");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || index > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0 || count > buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.outputState < HtmlWriter.OutputState.WritingTagName)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.isEndTag)
			{
				throw new InvalidOperationException(TextConvertersStrings.EndTagCannotHaveAttributes);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
			{
				this.OutputAttributeEnd();
			}
			this.OutputAttributeName(name);
			this.OutputAttributeValue(buffer, index, count);
			this.OutputAttributeEnd();
			this.outputState = HtmlWriter.OutputState.BeforeAttribute;
		}

		public void WriteAttribute(HtmlAttributeReader attributeReader)
		{
			if (this.outputState < HtmlWriter.OutputState.WritingTagName)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.isEndTag)
			{
				throw new InvalidOperationException(TextConvertersStrings.EndTagCannotHaveAttributes);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			attributeReader.WriteNameTo(this.WriteAttributeName());
			if (attributeReader.HasValue)
			{
				attributeReader.WriteValueTo(this.WriteAttributeValue());
			}
			this.OutputAttributeEnd();
			this.outputState = HtmlWriter.OutputState.BeforeAttribute;
		}

		public void WriteAttributeName(HtmlAttributeId id)
		{
			if (id < HtmlAttributeId.Unknown || (int)id >= HtmlNameData.attributeIndex.Length)
			{
				throw new ArgumentException(TextConvertersStrings.AttributeIdInvalid, "id");
			}
			if (id == HtmlAttributeId.Unknown)
			{
				throw new ArgumentException(TextConvertersStrings.AttributeIdIsUnknown, "id");
			}
			if (this.outputState < HtmlWriter.OutputState.WritingTagName)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.isEndTag)
			{
				throw new InvalidOperationException(TextConvertersStrings.EndTagCannotHaveAttributes);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
			{
				this.OutputAttributeEnd();
			}
			this.OutputAttributeName(HtmlNameData.Names[(int)HtmlNameData.attributeIndex[(int)id]].Name);
		}

		public void WriteAttributeName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(TextConvertersStrings.AttributeNameIsEmpty, "name");
			}
			if (this.outputState < HtmlWriter.OutputState.WritingTagName)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.isEndTag)
			{
				throw new InvalidOperationException(TextConvertersStrings.EndTagCannotHaveAttributes);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
			{
				this.OutputAttributeEnd();
			}
			this.OutputAttributeName(name);
		}

		public void WriteAttributeName(HtmlAttributeReader attributeReader)
		{
			if (this.outputState < HtmlWriter.OutputState.WritingTagName)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.isEndTag)
			{
				throw new InvalidOperationException(TextConvertersStrings.EndTagCannotHaveAttributes);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			attributeReader.WriteNameTo(this.WriteAttributeName());
		}

		public void WriteAttributeValue(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this.outputState < HtmlWriter.OutputState.TagStarted)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.outputState < HtmlWriter.OutputState.WritingAttributeName)
			{
				throw new InvalidOperationException(TextConvertersStrings.AttributeNotStarted);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			this.OutputAttributeValue(value);
		}

		public void WriteAttributeValue(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || index > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0 || count > buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.outputState < HtmlWriter.OutputState.TagStarted)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.outputState < HtmlWriter.OutputState.WritingAttributeName)
			{
				throw new InvalidOperationException(TextConvertersStrings.AttributeNotStarted);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			this.OutputAttributeValue(buffer, index, count);
		}

		public void WriteAttributeValue(HtmlAttributeReader attributeReader)
		{
			if (this.outputState < HtmlWriter.OutputState.TagStarted)
			{
				throw new InvalidOperationException(TextConvertersStrings.TagNotStarted);
			}
			if (this.outputState < HtmlWriter.OutputState.WritingAttributeName)
			{
				throw new InvalidOperationException(TextConvertersStrings.AttributeNotStarted);
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (attributeReader.HasValue)
			{
				attributeReader.WriteValueTo(this.WriteAttributeValue());
			}
		}

		public void WriteText(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			if (value.Length != 0)
			{
				if (this.lastWhitespace)
				{
					this.OutputLastWhitespace(value[0]);
				}
				this.output.Write(value, this);
				this.lineLength += value.Length;
				this.textLineLength += value.Length;
				this.allowWspBeforeFollowingTag = false;
			}
		}

		public void WriteText(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || index > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0 || count > buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			this.WriteTextInternal(buffer, index, count);
		}

		public void WriteText(HtmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			reader.WriteTextTo(this.WriteText());
		}

		public void WriteMarkupText(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			if (this.lastWhitespace)
			{
				this.OutputLastWhitespace(value[0]);
			}
			this.output.Write(value, null);
			this.lineLength += value.Length;
			this.allowWspBeforeFollowingTag = false;
		}

		public void WriteMarkupText(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || index > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0 || count > buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			if (this.lastWhitespace)
			{
				this.OutputLastWhitespace(buffer[index]);
			}
			this.output.Write(buffer, index, count, null);
			this.lineLength += count;
			this.allowWspBeforeFollowingTag = false;
		}

		public void WriteMarkupText(HtmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			reader.WriteMarkupTextTo(this.WriteMarkupText());
		}

		bool IRestartable.CanRestart()
		{
			return this.output is IRestartable && ((IRestartable)this.output).CanRestart();
		}

		void IRestartable.Restart()
		{
			if (this.output is IRestartable)
			{
				((IRestartable)this.output).Restart();
			}
			this.allowWspBeforeFollowingTag = false;
			this.lastWhitespace = false;
			this.lineLength = 0;
			this.longestLineLength = 0;
			this.literalWhitespaceNesting = 0;
			this.literalTags = false;
			this.literalEntities = false;
			this.cssEscaping = false;
			this.tagNameIndex = HtmlNameIndex._NOTANAME;
			this.previousTagNameIndex = HtmlNameIndex._NOTANAME;
			this.isEndTag = false;
			this.isEmptyScopeTag = false;
			this.copyPending = false;
			this.outputState = HtmlWriter.OutputState.OutsideTag;
		}

		void IRestartable.DisableRestart()
		{
			if (this.output is IRestartable)
			{
				((IRestartable)this.output).DisableRestart();
			}
		}

		byte[] IFallback.GetUnsafeAsciiMap(out byte unsafeAsciiMask)
		{
			if (this.literalEntities)
			{
				unsafeAsciiMask = 0;
				return null;
			}
			if (this.filterHtml)
			{
				unsafeAsciiMask = 1;
			}
			else
			{
				unsafeAsciiMask = 1;
			}
			return HtmlSupport.UnsafeAsciiMap;
		}

		bool IFallback.HasUnsafeUnicode()
		{
			return this.filterHtml;
		}

		bool IFallback.TreatNonAsciiAsUnsafe(string charset)
		{
			return this.filterHtml && charset.StartsWith("x-", StringComparison.OrdinalIgnoreCase);
		}

		bool IFallback.IsUnsafeUnicode(char ch, bool isFirstChar)
		{
			return this.filterHtml && (ch < '\ud800' || ch >= '') && ((byte)(ch & 'ÿ') == 60 || (byte)(ch >> 8 & 'ÿ') == 60 || (!isFirstChar && ch == '﻿') || CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.PrivateUse);
		}

		bool IFallback.FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int outputEnd)
		{
			if (this.literalEntities)
			{
				if (this.cssEscaping)
				{
					uint num = (uint)ch;
					int num2 = (num < 16U) ? 1 : ((num < 256U) ? 2 : ((num < 4096U) ? 3 : 4));
					if (outputEnd - outputBufferCount < num2 + 2)
					{
						return false;
					}
					outputBuffer[outputBufferCount++] = '\\';
					int num3 = outputBufferCount + num2;
					while (num != 0U)
					{
						uint num4 = num & 15U;
						outputBuffer[--num3] = (char)((ulong)num4 + (ulong)((num4 < 10U) ? 48L : 55L));
						num >>= 4;
					}
					outputBufferCount += num2;
					outputBuffer[outputBufferCount++] = ' ';
				}
				else
				{
					if (outputEnd - outputBufferCount < 1)
					{
						return false;
					}
					outputBuffer[outputBufferCount++] = (this.filterHtml ? '?' : ch);
				}
			}
			else
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
					uint num5 = (uint)ch;
					int num6 = (num5 < 10U) ? 1 : ((num5 < 100U) ? 2 : ((num5 < 1000U) ? 3 : ((num5 < 10000U) ? 4 : 5)));
					if (outputEnd - outputBufferCount < num6 + 3)
					{
						return false;
					}
					outputBuffer[outputBufferCount++] = '&';
					outputBuffer[outputBufferCount++] = '#';
					int num7 = outputBufferCount + num6;
					while (num5 != 0U)
					{
						uint num8 = num5 % 10U;
						outputBuffer[--num7] = (char)(num8 + 48U);
						num5 /= 10U;
					}
					outputBufferCount += num6;
					outputBuffer[outputBufferCount++] = ';';
				}
			}
			return true;
		}

		void ITextSink.Write(char[] buffer, int offset, int count)
		{
			this.lineLength += count;
			this.textLineLength += count;
			this.output.Write(buffer, offset, count, this.fallback);
		}

		void ITextSink.Write(int ucs32Char)
		{
			this.lineLength++;
			this.textLineLength++;
			this.output.Write(ucs32Char, this.fallback);
		}

		void ITextSinkEx.Write(string text)
		{
			this.lineLength += text.Length;
			this.textLineLength += text.Length;
			this.output.Write(text, this.fallback);
		}

		void ITextSinkEx.WriteNewLine()
		{
			if (this.lineLength > this.longestLineLength)
			{
				this.longestLineLength = this.lineLength;
			}
			this.output.Write("\r\n");
			this.lineLength = 0;
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

		internal static HtmlNameIndex LookupName(string name)
		{
			if (name.Length <= 14)
			{
				short num = (short)((ulong)(HashCode.CalculateLowerCase(name) ^ 221) % 601UL);
				int num2 = (int)HtmlNameData.nameHashTable[(int)num];
				if (num2 > 0)
				{
					for (;;)
					{
						string name2 = HtmlNameData.Names[num2].Name;
						if (name2.Length == name.Length && name2[0] == ParseSupport.ToLowerCase(name[0]) && (name.Length == 1 || name2.Equals(name, StringComparison.OrdinalIgnoreCase)))
						{
							break;
						}
						if (HtmlNameData.Names[++num2].Hash != num)
						{
							return HtmlNameIndex.Unknown;
						}
					}
					return (HtmlNameIndex)num2;
				}
			}
			return HtmlNameIndex.Unknown;
		}

		internal void SetCopyPending(bool copyPending)
		{
			this.copyPending = copyPending;
		}

		internal void WriteStartTag(HtmlNameIndex nameIndex)
		{
			this.WriteTagBegin(nameIndex, null, false, false, false);
		}

		internal void WriteEndTag(HtmlNameIndex nameIndex)
		{
			this.WriteTagBegin(nameIndex, null, true, false, false);
			this.WriteTagEnd();
		}

		internal void WriteEmptyElementTag(HtmlNameIndex nameIndex)
		{
			this.WriteTagBegin(nameIndex, null, true, false, false);
			this.isEmptyScopeTag = true;
		}

		internal void WriteTagBegin(HtmlNameIndex nameIndex, string name, bool isEndTag, bool allowWspLeft, bool allowWspRight)
		{
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			if (this.literalTags && nameIndex >= HtmlNameIndex.Unknown && (!isEndTag || nameIndex != this.tagNameIndex))
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteOtherTagsInsideElement(HtmlNameData.Names[(int)this.tagNameIndex].Name));
			}
			HtmlTagIndex tagIndex = HtmlNameData.Names[(int)nameIndex].TagIndex;
			if (nameIndex > HtmlNameIndex.Unknown)
			{
				this.isEmptyScopeTag = (HtmlDtd.tags[(int)tagIndex].Scope == HtmlDtd.TagScope.EMPTY);
				if (isEndTag && this.isEmptyScopeTag)
				{
					if (HtmlDtd.tags[(int)tagIndex].UnmatchedSubstitute != HtmlTagIndex._IMPLICIT_BEGIN)
					{
						this.output.Write("<!-- </");
						this.lineLength += 7;
						if (nameIndex > HtmlNameIndex.Unknown)
						{
							this.output.Write(HtmlNameData.Names[(int)nameIndex].Name);
							this.lineLength += HtmlNameData.Names[(int)nameIndex].Name.Length;
						}
						else
						{
							this.output.Write((name != null) ? name : "???");
							this.lineLength += ((name != null) ? name.Length : 3);
						}
						this.output.Write("> ");
						this.lineLength += 2;
						this.tagNameIndex = HtmlNameIndex._COMMENT;
						this.outputState = HtmlWriter.OutputState.WritingUnstructuredTagContent;
						return;
					}
					isEndTag = false;
				}
			}
			if (this.autoNewLines && this.literalWhitespaceNesting == 0)
			{
				bool flag = this.lastWhitespace;
				HtmlDtd.TagFill fill = HtmlDtd.tags[(int)tagIndex].Fill;
				if (this.lineLength != 0)
				{
					HtmlDtd.TagFmt fmt = HtmlDtd.tags[(int)tagIndex].Fmt;
					if ((!isEndTag && fmt.LB == HtmlDtd.FmtCode.BRK) || (isEndTag && fmt.LE == HtmlDtd.FmtCode.BRK) || (this.lineLength > 80 && (this.lastWhitespace || this.allowWspBeforeFollowingTag || (!isEndTag && fill.LB == HtmlDtd.FillCode.EAT) || (isEndTag && fill.LE == HtmlDtd.FillCode.EAT))))
					{
						if (this.lineLength > this.longestLineLength)
						{
							this.longestLineLength = this.lineLength;
						}
						this.output.Write("\r\n");
						this.lineLength = 0;
						this.lastWhitespace = false;
					}
				}
				this.allowWspBeforeFollowingTag = (((!isEndTag && fill.RB == HtmlDtd.FillCode.EAT) || (isEndTag && fill.RE == HtmlDtd.FillCode.EAT) || (flag && ((!isEndTag && fill.RB == HtmlDtd.FillCode.NUL) || (isEndTag && fill.RE == HtmlDtd.FillCode.NUL)))) && (nameIndex != HtmlNameIndex.Body || !isEndTag));
			}
			if (this.lastWhitespace)
			{
				this.output.Write(' ');
				this.lineLength++;
				this.lastWhitespace = false;
			}
			if (HtmlDtd.tags[(int)tagIndex].BlockElement || tagIndex == HtmlTagIndex.BR)
			{
				this.textLineLength = 0;
			}
			this.output.Write('<');
			this.lineLength++;
			if (nameIndex >= HtmlNameIndex.Unknown)
			{
				if (isEndTag)
				{
					if ((byte)(HtmlDtd.tags[(int)tagIndex].Literal & HtmlDtd.Literal.Tags) != 0)
					{
						this.literalTags = false;
						this.literalEntities = false;
						this.cssEscaping = false;
					}
					if (HtmlDtd.tags[(int)tagIndex].ContextTextType == HtmlDtd.ContextTextType.Literal)
					{
						this.literalWhitespaceNesting--;
					}
					this.output.Write('/');
					this.lineLength++;
				}
				if (nameIndex != HtmlNameIndex.Unknown)
				{
					this.output.Write(HtmlNameData.Names[(int)nameIndex].Name);
					this.lineLength += HtmlNameData.Names[(int)nameIndex].Name.Length;
					this.outputState = HtmlWriter.OutputState.BeforeAttribute;
				}
				else
				{
					if (name != null)
					{
						this.output.Write(name);
						this.lineLength += name.Length;
						this.outputState = HtmlWriter.OutputState.BeforeAttribute;
					}
					else
					{
						this.outputState = HtmlWriter.OutputState.TagStarted;
					}
					this.isEmptyScopeTag = false;
				}
			}
			else
			{
				this.previousTagNameIndex = this.tagNameIndex;
				if (nameIndex == HtmlNameIndex._COMMENT)
				{
					this.output.Write("!--");
					this.lineLength += 3;
				}
				else if (nameIndex == HtmlNameIndex._ASP)
				{
					this.output.Write('%');
					this.lineLength++;
				}
				else if (nameIndex == HtmlNameIndex._CONDITIONAL)
				{
					this.output.Write("!--[");
					this.lineLength += 4;
				}
				else if (nameIndex == HtmlNameIndex._DTD)
				{
					this.output.Write('?');
					this.lineLength++;
				}
				else
				{
					this.output.Write('!');
					this.lineLength++;
				}
				this.outputState = HtmlWriter.OutputState.WritingUnstructuredTagContent;
				this.isEmptyScopeTag = true;
			}
			this.tagNameIndex = nameIndex;
			this.isEndTag = isEndTag;
		}

		internal void WriteTagEnd()
		{
			this.WriteTagEnd(this.isEmptyScopeTag);
		}

		internal void WriteTagEnd(bool emptyScopeTag)
		{
			HtmlTagIndex tagIndex = HtmlNameData.Names[(int)this.tagNameIndex].TagIndex;
			if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
			{
				this.OutputAttributeEnd();
			}
			if (this.tagNameIndex > HtmlNameIndex.Unknown)
			{
				this.output.Write('>');
				this.lineLength++;
			}
			else
			{
				if (this.tagNameIndex == HtmlNameIndex._COMMENT)
				{
					this.output.Write("-->");
					this.lineLength += 3;
				}
				else if (this.tagNameIndex == HtmlNameIndex._ASP)
				{
					this.output.Write("%>");
					this.lineLength += 2;
				}
				else if (this.tagNameIndex == HtmlNameIndex._CONDITIONAL)
				{
					this.output.Write("]-->");
					this.lineLength += 4;
				}
				else if (this.tagNameIndex == HtmlNameIndex.Unknown && emptyScopeTag)
				{
					this.output.Write(" />");
					this.lineLength += 3;
				}
				else
				{
					this.output.Write('>');
					this.lineLength++;
				}
				this.tagNameIndex = this.previousTagNameIndex;
			}
			if (this.isEndTag && (tagIndex == HtmlTagIndex.LI || tagIndex == HtmlTagIndex.DD || tagIndex == HtmlTagIndex.DT))
			{
				this.lineLength = 0;
			}
			if (this.autoNewLines && this.literalWhitespaceNesting == 0)
			{
				HtmlDtd.TagFmt fmt = HtmlDtd.tags[(int)tagIndex].Fmt;
				HtmlDtd.TagFill fill = HtmlDtd.tags[(int)tagIndex].Fill;
				if ((!this.isEndTag && fmt.RB == HtmlDtd.FmtCode.BRK) || (this.isEndTag && fmt.RE == HtmlDtd.FmtCode.BRK) || (this.lineLength > 80 && (this.allowWspBeforeFollowingTag || (!this.isEndTag && fill.RB == HtmlDtd.FillCode.EAT) || (this.isEndTag && fill.RE == HtmlDtd.FillCode.EAT))))
				{
					if (this.lineLength > this.longestLineLength)
					{
						this.longestLineLength = this.lineLength;
					}
					this.output.Write("\r\n");
					this.lineLength = 0;
				}
			}
			if (!this.isEndTag && !emptyScopeTag)
			{
				HtmlDtd.Literal literal = HtmlDtd.tags[(int)tagIndex].Literal;
				if ((byte)(literal & HtmlDtd.Literal.Tags) != 0)
				{
					this.literalTags = true;
					this.literalEntities = (0 != (byte)(literal & HtmlDtd.Literal.Entities));
					this.cssEscaping = (tagIndex == HtmlTagIndex.Style);
				}
				if (HtmlDtd.tags[(int)tagIndex].ContextTextType == HtmlDtd.ContextTextType.Literal)
				{
					this.literalWhitespaceNesting++;
				}
			}
			this.outputState = HtmlWriter.OutputState.OutsideTag;
		}

		internal void WriteAttribute(HtmlNameIndex nameIndex, string value)
		{
			if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
			{
				this.OutputAttributeEnd();
			}
			this.OutputAttributeName(HtmlNameData.Names[(int)nameIndex].Name);
			if (value != null)
			{
				this.OutputAttributeValue(value);
				this.OutputAttributeEnd();
			}
			this.outputState = HtmlWriter.OutputState.BeforeAttribute;
		}

		internal void WriteAttribute(HtmlNameIndex nameIndex, BufferString value)
		{
			if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
			{
				this.OutputAttributeEnd();
			}
			this.OutputAttributeName(HtmlNameData.Names[(int)nameIndex].Name);
			this.OutputAttributeValue(value.Buffer, value.Offset, value.Length);
			this.OutputAttributeEnd();
			this.outputState = HtmlWriter.OutputState.BeforeAttribute;
		}

		internal void WriteAttributeName(HtmlNameIndex nameIndex)
		{
			if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
			{
				this.OutputAttributeEnd();
			}
			this.OutputAttributeName(HtmlNameData.Names[(int)nameIndex].Name);
		}

		internal void WriteAttributeValue(BufferString value)
		{
			this.OutputAttributeValue(value.Buffer, value.Offset, value.Length);
		}

		internal void WriteAttributeValueInternal(string value)
		{
			this.OutputAttributeValue(value);
		}

		internal void WriteAttributeValueInternal(char[] buffer, int index, int count)
		{
			this.OutputAttributeValue(buffer, index, count);
		}

		internal void WriteText(char ch)
		{
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			if (this.lastWhitespace)
			{
				this.OutputLastWhitespace(ch);
			}
			this.output.Write(ch, this);
			this.lineLength++;
			this.textLineLength++;
			this.allowWspBeforeFollowingTag = false;
		}

		internal void WriteMarkupText(char ch)
		{
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			if (this.lastWhitespace)
			{
				this.OutputLastWhitespace(ch);
			}
			this.output.Write(ch, null);
			this.lineLength++;
			this.allowWspBeforeFollowingTag = false;
		}

		internal ITextSinkEx WriteUnstructuredTagContent()
		{
			this.fallback = null;
			return this;
		}

		internal ITextSinkEx WriteTagName()
		{
			this.outputState = HtmlWriter.OutputState.WritingTagName;
			this.fallback = null;
			return this;
		}

		internal ITextSinkEx WriteAttributeName()
		{
			if (this.outputState != HtmlWriter.OutputState.WritingAttributeName)
			{
				if (this.outputState > HtmlWriter.OutputState.BeforeAttribute)
				{
					this.OutputAttributeEnd();
				}
				this.output.Write(' ');
				this.lineLength++;
			}
			this.outputState = HtmlWriter.OutputState.WritingAttributeName;
			this.fallback = null;
			return this;
		}

		internal ITextSinkEx WriteAttributeValue()
		{
			if (this.outputState != HtmlWriter.OutputState.WritingAttributeValue)
			{
				this.output.Write("=\"");
				this.lineLength += 2;
			}
			this.outputState = HtmlWriter.OutputState.WritingAttributeValue;
			this.fallback = this;
			return this;
		}

		internal ITextSinkEx WriteText()
		{
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			this.allowWspBeforeFollowingTag = false;
			if (this.lastWhitespace)
			{
				this.OutputLastWhitespace('\u3000');
			}
			this.fallback = this;
			return this;
		}

		internal ITextSinkEx WriteMarkupText()
		{
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			if (this.lastWhitespace)
			{
				this.output.Write(' ');
				this.lineLength++;
				this.lastWhitespace = false;
			}
			this.fallback = null;
			return this;
		}

		internal void WriteNewLine()
		{
			this.WriteNewLine(false);
		}

		internal void WriteNewLine(bool optional)
		{
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			if (!optional || (this.lineLength != 0 && this.literalWhitespaceNesting == 0))
			{
				if (this.lineLength > this.longestLineLength)
				{
					this.longestLineLength = this.lineLength;
				}
				this.output.Write("\r\n");
				this.lineLength = 0;
				this.lastWhitespace = false;
				this.allowWspBeforeFollowingTag = false;
			}
		}

		internal void WriteAutoNewLine()
		{
			this.WriteNewLine(false);
		}

		internal void WriteAutoNewLine(bool optional)
		{
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			if (this.autoNewLines && (!optional || (this.lineLength != 0 && this.literalWhitespaceNesting == 0)))
			{
				if (this.lineLength > this.longestLineLength)
				{
					this.longestLineLength = this.lineLength;
				}
				this.output.Write("\r\n");
				this.lineLength = 0;
				this.lastWhitespace = false;
				this.allowWspBeforeFollowingTag = false;
			}
		}

		internal void WriteTabulation(int count)
		{
			this.WriteSpace(this.textLineLength / 8 * 8 + 8 * count - this.textLineLength);
		}

		internal void WriteSpace(int count)
		{
			if (this.literalWhitespaceNesting != 0)
			{
				while (count-- != 0)
				{
					this.output.Write(' ');
				}
				this.lineLength += count;
				this.textLineLength += count;
				this.lastWhitespace = false;
				this.allowWspBeforeFollowingTag = false;
				return;
			}
			if (this.lineLength == 0 && count == 1)
			{
				this.output.Write('\u00a0', this);
				return;
			}
			if (this.lastWhitespace)
			{
				this.lineLength++;
				this.output.Write('\u00a0', this);
			}
			this.lineLength += count - 1;
			this.textLineLength += count - 1;
			while (--count != 0)
			{
				this.output.Write('\u00a0', this);
			}
			this.lastWhitespace = true;
			this.allowWspBeforeFollowingTag = false;
		}

		internal void WriteNbsp(int count)
		{
			if (this.lastWhitespace)
			{
				this.OutputLastWhitespace('\u00a0');
			}
			this.lineLength += count;
			this.textLineLength += count;
			while (count-- != 0)
			{
				this.output.Write('\u00a0', this);
			}
			this.allowWspBeforeFollowingTag = false;
		}

		internal void WriteTextInternal(char[] buffer, int index, int count)
		{
			if (count != 0)
			{
				if (this.lastWhitespace)
				{
					this.OutputLastWhitespace(buffer[index]);
				}
				this.output.Write(buffer, index, count, this);
				this.lineLength += count;
				this.textLineLength += count;
				this.allowWspBeforeFollowingTag = false;
			}
		}

		internal void StartTextChunk()
		{
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			this.lastWhitespace = false;
		}

		internal void EndTextChunk()
		{
			if (this.lastWhitespace)
			{
				this.OutputLastWhitespace('\n');
			}
		}

		internal void WriteCollapsedWhitespace()
		{
			if (this.outputState != HtmlWriter.OutputState.OutsideTag)
			{
				this.WriteTagEnd();
			}
			this.lastWhitespace = true;
			this.allowWspBeforeFollowingTag = false;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.output != null)
			{
				if (!this.copyPending)
				{
					this.Flush();
				}
				if (this.output != null)
				{
					((IDisposable)this.output).Dispose();
				}
			}
			this.output = null;
		}

		private void WriteTag(HtmlTagId id, bool isEndTag)
		{
			if (id < HtmlTagId.Unknown || (int)id >= HtmlNameData.TagIndex.Length)
			{
				throw new ArgumentException(TextConvertersStrings.TagIdInvalid, "id");
			}
			if (id == HtmlTagId.Unknown)
			{
				throw new ArgumentException(TextConvertersStrings.TagIdIsUnknown, "id");
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			this.WriteTagBegin(HtmlNameData.TagIndex[(int)id], null, isEndTag, false, false);
		}

		private void WriteTag(string name, bool isEndTag)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(TextConvertersStrings.TagNameIsEmpty, "name");
			}
			if (this.copyPending)
			{
				throw new InvalidOperationException(TextConvertersStrings.CannotWriteWhileCopyPending);
			}
			HtmlNameIndex htmlNameIndex = HtmlWriter.LookupName(name);
			if (htmlNameIndex != HtmlNameIndex.Unknown)
			{
				name = null;
			}
			this.WriteTagBegin(htmlNameIndex, name, isEndTag, false, false);
		}

		private void OutputLastWhitespace(char nextChar)
		{
			if (this.lineLength > 255 && this.autoNewLines)
			{
				if (this.lineLength > this.longestLineLength)
				{
					this.longestLineLength = this.lineLength;
				}
				this.output.Write("\r\n");
				this.lineLength = 0;
				if (ParseSupport.FarEastNonHanguelChar(nextChar))
				{
					this.output.Write(' ');
					this.lineLength++;
				}
			}
			else
			{
				this.output.Write(' ');
				this.lineLength++;
			}
			this.textLineLength++;
			this.lastWhitespace = false;
		}

		private void OutputAttributeName(string name)
		{
			this.output.Write(' ');
			this.output.Write(name);
			this.lineLength += name.Length + 1;
			this.outputState = HtmlWriter.OutputState.AfterAttributeName;
		}

		private void OutputAttributeValue(string value)
		{
			if (this.outputState < HtmlWriter.OutputState.WritingAttributeValue)
			{
				this.output.Write("=\"");
				this.lineLength += 2;
			}
			this.output.Write(value, this);
			this.lineLength += value.Length;
			this.outputState = HtmlWriter.OutputState.WritingAttributeValue;
		}

		private void OutputAttributeValue(char[] value, int index, int count)
		{
			if (this.outputState < HtmlWriter.OutputState.WritingAttributeValue)
			{
				this.output.Write("=\"");
				this.lineLength += 2;
			}
			this.output.Write(value, index, count, this);
			this.lineLength += count;
			this.outputState = HtmlWriter.OutputState.WritingAttributeValue;
		}

		private void OutputAttributeEnd()
		{
			if (this.outputState < HtmlWriter.OutputState.WritingAttributeValue)
			{
				this.output.Write("=\"");
				this.lineLength += 2;
			}
			this.output.Write('"');
			this.lineLength++;
		}

		private ConverterOutput output;

		private HtmlWriter.OutputState outputState;

		private bool filterHtml;

		private bool autoNewLines;

		private bool allowWspBeforeFollowingTag;

		private bool lastWhitespace;

		private int lineLength;

		private int longestLineLength;

		private int textLineLength;

		private int literalWhitespaceNesting;

		private bool literalTags;

		private bool literalEntities;

		private bool cssEscaping;

		private IFallback fallback;

		private HtmlNameIndex tagNameIndex;

		private HtmlNameIndex previousTagNameIndex;

		private bool isEndTag;

		private bool isEmptyScopeTag;

		private bool copyPending;

		internal enum OutputState
		{
			OutsideTag,
			TagStarted,
			WritingUnstructuredTagContent,
			WritingTagName,
			BeforeAttribute,
			WritingAttributeName,
			AfterAttributeName,
			WritingAttributeValue
		}
	}
}
