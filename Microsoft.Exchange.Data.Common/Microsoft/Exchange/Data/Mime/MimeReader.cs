using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.Mime
{
	public class MimeReader : IDisposable
	{
		public MimeReader(Stream mime) : this(mime, true, DecodingOptions.Default, MimeLimits.Unlimited, false, true)
		{
			if (mime == null)
			{
				throw new ArgumentNullException("mime");
			}
		}

		public MimeReader(Stream mime, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits) : this(mime, inferMime, decodingOptions, mimeLimits, false, true)
		{
			if (mime == null)
			{
				throw new ArgumentNullException("mime");
			}
		}

		internal MimeReader(Stream mime, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits, bool parseEmbeddedMessages, bool parseInline) : this(mime, inferMime, decodingOptions, mimeLimits, parseEmbeddedMessages, parseInline, true)
		{
		}

		internal MimeReader(Stream mime, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits, bool parseEmbeddedMessages, bool parseInline, bool expectBinaryContent)
		{
			this.FixBadMimeBoundary = true;
			this.state = MimeReaderState.Start;
			this.partCount = 1;
			this.eofMeansEndOfFile = true;
			base..ctor();
			if (mime != null && !mime.CanRead)
			{
				throw new ArgumentException(Strings.StreamMustAllowRead, "mime");
			}
			this.mimeStream = mime;
			this.parser = new MimeParser(true, parseInline, expectBinaryContent);
			this.data = new byte[5120];
			this.inferMime = inferMime;
			this.decodingOptions = decodingOptions;
			this.limits = mimeLimits;
			this.parseEmbeddedMessages = parseEmbeddedMessages;
		}

		private MimeReader(MimeReader parent)
		{
			this.FixBadMimeBoundary = true;
			this.state = MimeReaderState.Start;
			this.partCount = 1;
			this.eofMeansEndOfFile = true;
			base..ctor();
			this.parentReader = parent;
			this.parentReader.childReader = this;
			this.mimeStream = parent.mimeStream;
			this.parser = parent.parser;
			this.data = parent.data;
			this.dataOffset = parent.dataOffset;
			this.dataCount = parent.dataCount;
			this.dataEOF = parent.dataEOF;
			this.outerContentStream = parent.outerContentStream;
			this.outerContentDepth = -1;
			this.inferMime = parent.inferMime;
			this.limits = parent.limits;
			this.decodingOptions = parent.decodingOptions;
			this.partCount = parent.partCount;
			this.headerCount = parent.headerCount;
			this.cumulativeHeaderBytes = parent.cumulativeHeaderBytes;
			this.embeddedDepth = parent.embeddedDepth + 1;
		}

		internal MimeReader(IMimeHandlerInternal handler, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits, bool parseInline)
		{
			this.FixBadMimeBoundary = true;
			this.state = MimeReaderState.Start;
			this.partCount = 1;
			this.eofMeansEndOfFile = true;
			base..ctor();
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			this.handler = handler;
			this.parser = new MimeParser(true, parseInline, true);
			this.data = new byte[5120];
			this.inferMime = inferMime;
			this.decodingOptions = decodingOptions;
			this.limits = mimeLimits;
			this.parseEmbeddedMessages = true;
		}

		public MimeLimits MimeLimits
		{
			get
			{
				this.AssertGoodToUse(false, false);
				return this.limits;
			}
		}

		public DecodingOptions HeaderDecodingOptions
		{
			get
			{
				return this.decodingOptions;
			}
		}

		public MimeComplianceStatus ComplianceStatus
		{
			get
			{
				this.AssertGoodToUse(false, false);
				return this.parser.ComplianceStatus;
			}
		}

		public long StreamOffset
		{
			get
			{
				this.AssertGoodToUse(false, true);
				return (long)this.parser.Position;
			}
		}

		internal int Depth
		{
			get
			{
				this.AssertGoodToUse(false, true);
				return this.depth;
			}
		}

		public int PartDepth
		{
			get
			{
				this.AssertGoodToUse(false, true);
				if (this.depth != 0)
				{
					return this.parser.PartDepth + 1;
				}
				return 0;
			}
		}

		public int EmbeddedDepth
		{
			get
			{
				this.AssertGoodToUse(false, true);
				return this.embeddedDepth;
			}
		}

		internal MimeReaderState ReaderState
		{
			get
			{
				return this.state;
			}
		}

		internal bool DataExhausted
		{
			get
			{
				return this.dataExhausted;
			}
		}

		internal bool EndOfFile
		{
			get
			{
				return this.state == MimeReaderState.End;
			}
		}

		public MimeHeaderReader HeaderReader
		{
			get
			{
				this.AssertGoodToUse(true, true);
				if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete | MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				return new MimeHeaderReader(this);
			}
		}

		internal HeaderId HeaderId
		{
			get
			{
				this.AssertGoodToUse(false, true);
				if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete))
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				return this.currentHeaderId;
			}
		}

		internal string HeaderName
		{
			get
			{
				this.AssertGoodToUse(false, true);
				if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete))
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				return this.currentHeaderName;
			}
		}

		public bool IsMultipart
		{
			get
			{
				this.AssertGoodToUse(false, true);
				if (this.state == MimeReaderState.InlineStart)
				{
					return false;
				}
				if (this.state != MimeReaderState.EndOfHeaders)
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				return this.currentPartMajorType == MajorContentType.Multipart && this.parser.IsMime;
			}
		}

		public bool IsEmbeddedMessage
		{
			get
			{
				this.AssertGoodToUse(false, true);
				if (this.state == MimeReaderState.InlineStart)
				{
					return false;
				}
				if (this.state != MimeReaderState.EndOfHeaders)
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				return this.currentPartMajorType == MajorContentType.MessageRfc822 && this.parser.IsMime;
			}
		}

		public string ContentType
		{
			get
			{
				this.AssertGoodToUse(false, true);
				if (this.state == MimeReaderState.InlineStart)
				{
					return "application/octet-stream";
				}
				if (this.state != MimeReaderState.EndOfHeaders)
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				return this.currentPartContentType;
			}
		}

		internal ContentTransferEncoding ContentTransferEncoding
		{
			get
			{
				this.AssertGoodToUse(false, true);
				if (this.state != MimeReaderState.EndOfHeaders && this.state != MimeReaderState.InlineStart)
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				return this.currentPartContentTransferEncoding;
			}
		}

		public bool IsInline
		{
			get
			{
				this.AssertGoodToUse(false, true);
				if (MimeReader.StateIsOneOf(this.state, MimeReaderState.Start | MimeReaderState.End))
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				return MimeReader.StateIsOneOf(this.state, MimeReaderState.InlineStart | MimeReaderState.InlineBody | MimeReaderState.InlineEnd);
			}
		}

		public string InlineFileName
		{
			get
			{
				this.AssertGoodToUse(false, true);
				if (!this.IsInline)
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				return this.inlineFileName.ToString();
			}
		}

		internal LineTerminationState LineTerminationState
		{
			get
			{
				return this.currentLineTerminationState;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.parser == null)
			{
				return;
			}
			if (disposing)
			{
				if (this.childReader != null)
				{
					throw new InvalidOperationException(Strings.EmbeddedMessageReaderNeedsToBeClosedFirst);
				}
				if (this.parentReader == null)
				{
					if (this.mimeStream != null)
					{
						this.mimeStream.Dispose();
					}
				}
				else
				{
					this.parentReader.partCount = this.partCount;
					this.parentReader.headerCount = this.headerCount;
					this.parentReader.cumulativeHeaderBytes = this.cumulativeHeaderBytes;
					this.parentReader.dataOffset = this.dataOffset;
					this.parentReader.dataCount = this.dataCount;
					this.parentReader.dataEOF = this.dataEOF;
					this.parentReader.currentToken = this.currentToken;
					this.parentReader.cleanupDepth = this.depth + this.cleanupDepth;
					this.parentReader.state = MimeReaderState.EmbeddedEnd;
					this.parentReader.childReader = null;
					this.parentReader = null;
				}
				if (this.contentStream != null)
				{
					this.contentStream.Dispose();
				}
				if (this.outerContentStream != null)
				{
					int num = this.outerContentDepth;
				}
			}
			this.state = MimeReaderState.End;
			this.mimeStream = null;
			this.handler = null;
			this.contentStream = null;
			this.outerContentStream = null;
			this.data = null;
			this.parser = null;
			this.currentHeader = null;
			this.currentChild = null;
			this.currentGrandChild = null;
		}

		public void Close()
		{
			this.Dispose();
		}

		internal void DisconnectInputStream()
		{
			this.mimeStream = null;
		}

		public bool ReadNextPart()
		{
			this.AssertGoodToUse(true, true);
			this.TrySkipToNextPartBoundary(false);
			return MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart);
		}

		public bool ReadFirstChildPart()
		{
			this.AssertGoodToUse(true, true);
			if (this.state == MimeReaderState.InlineStart)
			{
				return false;
			}
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.Start | MimeReaderState.EndOfHeaders))
			{
				if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				do
				{
					this.TryReachNextState();
				}
				while (this.state != MimeReaderState.EndOfHeaders);
			}
			if (this.state == MimeReaderState.EndOfHeaders && !this.IsMultipart && (!this.IsEmbeddedMessage || !this.parseEmbeddedMessages))
			{
				return false;
			}
			this.TrySkipToNextPartBoundary(true);
			return this.state == MimeReaderState.PartStart;
		}

		public bool ReadNextSiblingPart()
		{
			this.AssertGoodToUse(true, true);
			if (this.state == MimeReaderState.End)
			{
				return false;
			}
			if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete | MimeReaderState.EndOfHeaders))
			{
				this.createHeader = false;
				while (this.state != MimeReaderState.EndOfHeaders)
				{
					this.TryReachNextState();
				}
				this.parser.SetContentType(MajorContentType.Other, default(MimeString));
			}
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.Start | MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
			{
				int num = this.depth;
				do
				{
					this.TrySkipToNextPartBoundary(true);
				}
				while (this.depth > num || !MimeReader.StateIsOneOf(this.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd));
			}
			this.TrySkipToNextPartBoundary(true);
			return MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart);
		}

		public void EnableReadingUnparsedHeaders()
		{
			this.enableReadingOuterContent = true;
		}

		public void ReadHeaders()
		{
			this.AssertGoodToUse(true, true);
			if (MimeReader.StateIsOneOf(this.state, MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
			{
				return;
			}
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
			{
				throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
			}
			this.createHeader = false;
			do
			{
				this.TryReachNextState();
			}
			while (this.state != MimeReaderState.EndOfHeaders);
		}

		internal bool ReadNextHeader()
		{
			this.AssertGoodToUse(true, true);
			if (MimeReader.StateIsOneOf(this.state, MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
			{
				return false;
			}
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
			{
				throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
			}
			do
			{
				this.TryReachNextState();
			}
			while (!MimeReader.StateIsOneOf(this.state, MimeReaderState.HeaderStart | MimeReaderState.EndOfHeaders));
			return this.state == MimeReaderState.HeaderStart;
		}

		internal Header ReadHeaderObject()
		{
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
			{
				throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
			}
			if (this.state == MimeReaderState.HeaderStart)
			{
				this.TryCompleteCurrentHeader(true);
			}
			return this.currentHeader;
		}

		internal bool TryCompleteCurrentHeader(bool createHeader)
		{
			this.createHeader = (this.createHeader || createHeader);
			if (!this.TryReachNextState())
			{
				return false;
			}
			this.currentHeaderConsumed = false;
			this.currentChildConsumed = false;
			this.currentChild = null;
			this.currentGrandChild = null;
			return true;
		}

		internal Header CurrentHeaderObject
		{
			get
			{
				return this.currentHeader;
			}
		}

		internal void Reset(Stream stream)
		{
			this.mimeStream = stream;
			this.parser.Reset();
			this.state = MimeReaderState.Start;
			this.depth = 0;
			this.cleanupDepth = 0;
			this.embeddedDepth = 0;
			this.dataExhausted = false;
			this.dataEOF = false;
			this.dataOffset = 0;
			this.dataCount = 0;
			this.currentToken = default(MimeToken);
			this.currentHeader = null;
			this.currentChild = null;
			this.currentGrandChild = null;
			this.decoder = null;
			this.readRawData = false;
			this.contentStream = null;
			this.enableReadingOuterContent = false;
			this.outerContentStream = null;
			this.outerContentDepth = 0;
			this.childReader = null;
			this.parentReader = null;
			this.skipPart = false;
			this.skipHeaders = false;
			this.skipHeader = false;
			this.partCount = 0;
			this.headerCount = 0;
			this.cumulativeHeaderBytes = 0;
			this.currentTextHeaderBytes = 0;
		}

		internal void DangerousSetFixBadMimeBoundary(bool value)
		{
			this.FixBadMimeBoundary = value;
		}

		internal HeaderList ReadHeaderList()
		{
			this.AssertGoodToUse(true, true);
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart))
			{
				throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
			}
			HeaderList headerList = new HeaderList(null);
			if (this.state == MimeReaderState.InlineStart)
			{
				return headerList;
			}
			do
			{
				this.TryReachNextState();
				if (this.state == MimeReaderState.HeaderStart)
				{
					this.createHeader = true;
				}
				else if (this.state == MimeReaderState.HeaderComplete && this.currentHeader != null)
				{
					headerList.InternalAppendChild(this.currentHeader);
				}
			}
			while (this.state != MimeReaderState.EndOfHeaders);
			return headerList;
		}

		internal bool ReadNextDescendant(bool topLevel)
		{
			this.AssertGoodToUse(true, true);
			if (this.state != MimeReaderState.HeaderComplete)
			{
				throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
			}
			if (topLevel)
			{
				if (this.currentHeaderConsumed)
				{
					return false;
				}
				if (this.currentChild == null)
				{
					this.currentChild = this.currentHeader.FirstChild;
				}
				else
				{
					this.currentGrandChild = null;
					this.currentChild = this.currentChild.NextSibling;
				}
				if (this.currentChild == null)
				{
					this.currentHeaderConsumed = true;
				}
				this.currentChildConsumed = false;
				this.currentGrandChild = null;
				return this.currentChild != null;
			}
			else
			{
				if (this.currentChild == null)
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				if (this.currentChildConsumed)
				{
					return false;
				}
				if (this.currentGrandChild == null)
				{
					this.currentGrandChild = this.currentChild.FirstChild;
				}
				else
				{
					this.currentGrandChild = this.currentGrandChild.NextSibling;
				}
				if (this.currentGrandChild == null)
				{
					this.currentChildConsumed = true;
				}
				return this.currentGrandChild != null;
			}
		}

		internal bool IsCurrentChildValid(bool topLevel)
		{
			if (!topLevel)
			{
				return this.currentGrandChild != null;
			}
			return this.currentChild != null;
		}

		public void CopyOuterContentTo(Stream stream)
		{
			this.AssertGoodToUse(false, true);
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException(Strings.StreamMustSupportWriting, "stream");
			}
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart))
			{
				throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
			}
			if (this.outerContentStream != null)
			{
				throw new NotSupportedException(Strings.OnlyOneOuterContentPushStreamAllowed);
			}
			this.outerContentStream = stream;
			this.outerContentDepth = this.depth;
		}

		public int ReadRawContent(byte[] buffer, int offset, int count)
		{
			this.AssertGoodToUse(true, true);
			MimeCommon.CheckBufferArguments(buffer, offset, count);
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartBody | MimeReaderState.InlineBody))
			{
				if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
				{
					return 0;
				}
				this.TryInitializeReadContent(false);
			}
			if (this.contentStream != null)
			{
				throw new NotSupportedException(Strings.CannotReadContentWhileStreamIsActive);
			}
			if (!this.readRawData)
			{
				throw new NotSupportedException(Strings.CannotMixReadRawContentAndReadContent);
			}
			int result;
			this.ReadPartData(buffer, offset, count, out result);
			return result;
		}

		public int ReadContent(byte[] buffer, int offset, int count)
		{
			this.AssertGoodToUse(true, true);
			MimeCommon.CheckBufferArguments(buffer, offset, count);
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartBody | MimeReaderState.InlineBody))
			{
				if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
				{
					return 0;
				}
				if (!this.TryInitializeReadContent(true))
				{
					throw new MimeException(Strings.CannotDecodeContentStream);
				}
			}
			if (this.contentStream != null)
			{
				throw new NotSupportedException(Strings.CannotReadContentWhileStreamIsActive);
			}
			if (this.readRawData)
			{
				throw new NotSupportedException(Strings.CannotMixReadRawContentAndReadContent);
			}
			int result;
			bool flag = this.ReadPartData(buffer, offset, count, out result);
			return result;
		}

		public Stream GetContentReadStream()
		{
			Stream result;
			if (!this.TryGetContentReadStream(out result))
			{
				throw new MimeException(Strings.CannotDecodeContentStream);
			}
			return result;
		}

		public bool TryGetContentReadStream(out Stream result)
		{
			this.AssertGoodToUse(true, true);
			if (!this.TryInitializeReadContent(true))
			{
				result = null;
				return false;
			}
			this.contentStream = new MimeReader.ContentReadStream(this);
			result = this.contentStream;
			return true;
		}

		public Stream GetRawContentReadStream()
		{
			this.AssertGoodToUse(true, true);
			this.TryInitializeReadContent(false);
			this.contentStream = new MimeReader.ContentReadStream(this);
			return this.contentStream;
		}

		private bool TryInitializeReadContent(bool decode)
		{
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
			{
				if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				if (this.state == MimeReaderState.PartStart && this.enableReadingOuterContent)
				{
					this.parser.SetStreamMode();
					this.state = MimeReaderState.PartBody;
					this.decoder = null;
					this.readRawData = !decode;
					return true;
				}
				while (this.TryReachNextState())
				{
					if (this.state == MimeReaderState.EndOfHeaders)
					{
						goto IL_7A;
					}
				}
				return false;
			}
			IL_7A:
			MimeReaderState mimeReaderState = this.state;
			if (this.state == MimeReaderState.EndOfHeaders)
			{
				this.parser.SetContentType(MajorContentType.Other, default(MimeString));
				mimeReaderState = MimeReaderState.PartBody;
			}
			else
			{
				mimeReaderState = MimeReaderState.InlineBody;
			}
			if (decode)
			{
				ContentTransferEncoding transferEncoding = this.parser.TransferEncoding;
				if (transferEncoding == ContentTransferEncoding.SevenBit || transferEncoding == ContentTransferEncoding.EightBit || transferEncoding == ContentTransferEncoding.Binary)
				{
					this.decoder = null;
				}
				else
				{
					this.decoder = MimePart.CreateDecoder(transferEncoding);
					if (this.decoder == null)
					{
						return false;
					}
				}
				this.readRawData = false;
			}
			else
			{
				this.decoder = null;
				this.readRawData = true;
			}
			this.state = mimeReaderState;
			return true;
		}

		public MimeReader GetEmbeddedMessageReader()
		{
			this.AssertGoodToUse(true, true);
			if (this.state != MimeReaderState.EndOfHeaders)
			{
				if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				do
				{
					this.TryReachNextState();
				}
				while (this.state != MimeReaderState.EndOfHeaders);
			}
			if (this.currentPartMajorType != MajorContentType.MessageRfc822 || !this.parser.IsMime)
			{
				throw new InvalidOperationException(Strings.CurrentPartIsNotEmbeddedMessage);
			}
			this.parser.SetContentType(MajorContentType.MessageRfc822, default(MimeString));
			this.TryReachNextState();
			this.childReader = new MimeReader(this);
			return this.childReader;
		}

		public void ResetComplianceStatus()
		{
			this.parser.ComplianceStatus = MimeComplianceStatus.Compliant;
		}

		private int TrimEndOfLine(int offset, int length)
		{
			if (length >= 1 && this.data[offset + length - 1] == 10)
			{
				length--;
				while (length >= 1 && this.data[offset + length - 1] == 13)
				{
					length--;
				}
			}
			return length;
		}

		private void ParseAndCheckSize()
		{
			this.currentToken = this.parser.Parse(this.data, this.dataOffset, this.dataOffset + this.dataCount, this.dataEOF);
			if (this.parser.Position > this.MimeLimits.MaxSize)
			{
				throw new MimeException(Strings.InputStreamTooLong(this.parser.Position, this.MimeLimits.MaxSize));
			}
		}

		private void CheckHeaderBytesLimits()
		{
			this.cumulativeHeaderBytes += (int)this.currentToken.Length;
			if (this.cumulativeHeaderBytes > this.MimeLimits.MaxHeaderBytes)
			{
				throw new MimeException(Strings.TooManyHeaderBytes(this.cumulativeHeaderBytes, this.MimeLimits.MaxHeaderBytes));
			}
			if (this.currentToken.Id == MimeTokenId.Header)
			{
				this.currentTextHeaderBytes = 0;
			}
			this.currentTextHeaderBytes += (int)this.currentToken.Length;
			Type left = Header.TypeFromHeaderId(this.currentHeaderId);
			if ((left == typeof(TextHeader) || left == typeof(AsciiTextHeader)) && this.currentTextHeaderBytes > this.MimeLimits.MaxTextValueBytesPerValue)
			{
				throw new MimeException(Strings.TooManyTextValueBytes(this.currentTextHeaderBytes, this.MimeLimits.MaxTextValueBytesPerValue));
			}
		}

		private void CheckPartsLimit()
		{
			if (++this.partCount > this.MimeLimits.MaxParts)
			{
				throw new MimeException(Strings.TooManyParts(this.partCount, this.MimeLimits.MaxParts));
			}
			if (this.PartDepth > this.MimeLimits.MaxPartDepth)
			{
				throw new MimeException(Strings.PartNestingTooDeep(this.PartDepth, this.MimeLimits.MaxPartDepth));
			}
			if (this.embeddedDepth > this.MimeLimits.MaxEmbeddedDepth)
			{
				throw new MimeException(Strings.EmbeddedNestingTooDeep(this.embeddedDepth, this.MimeLimits.MaxEmbeddedDepth));
			}
		}

		internal bool GroupInProgress
		{
			get
			{
				return this.currentChild != null && this.currentChild is MimeGroup;
			}
		}

		internal string ReadRecipientEmail(bool topLevel)
		{
			string result = null;
			MimeRecipient mimeRecipient;
			if (topLevel)
			{
				mimeRecipient = (this.currentChild as MimeRecipient);
				if (mimeRecipient == null)
				{
					throw new NotSupportedException(Strings.CurrentAddressIsGroupAndCannotHaveEmail);
				}
			}
			else
			{
				mimeRecipient = (this.currentGrandChild as MimeRecipient);
			}
			if (mimeRecipient != null)
			{
				result = mimeRecipient.Email;
			}
			return result;
		}

		internal bool TryReadDisplayName(bool topLevel, DecodingOptions decodingOptions, out DecodingResults decodingResults, out string displayName)
		{
			AddressItem addressItem;
			if (topLevel)
			{
				addressItem = (this.currentChild as AddressItem);
			}
			else
			{
				addressItem = (this.currentGrandChild as AddressItem);
			}
			return addressItem.TryGetDisplayName(decodingOptions, out decodingResults, out displayName);
		}

		internal string ReadParameterName()
		{
			MimeParameter mimeParameter = this.currentChild as MimeParameter;
			return mimeParameter.Name;
		}

		internal bool TryReadParameterValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
		{
			MimeParameter mimeParameter = this.currentChild as MimeParameter;
			return mimeParameter.TryGetValue(decodingOptions, out decodingResults, out value);
		}

		internal int AddMoreData(byte[] buffer, int offset, int length, bool endOfFile)
		{
			this.CompactDataBuffer();
			int num;
			if (length != 0)
			{
				num = Math.Min(length, this.data.Length - (this.dataOffset + this.dataCount) - 2);
				Buffer.BlockCopy(buffer, offset, this.data, this.dataOffset + this.dataCount, num);
				length -= num;
				this.dataCount += num;
			}
			else
			{
				num = 0;
			}
			this.dataEOF = (length == 0 && endOfFile);
			return num;
		}

		private bool ReadMoreData()
		{
			this.CompactDataBuffer();
			int num = this.dataOffset + this.dataCount;
			int num2 = this.mimeStream.Read(this.data, num, this.data.Length - num - 2);
			if (num2 != 0)
			{
				this.dataCount += num2;
				return true;
			}
			if (this.eofMeansEndOfFile)
			{
				this.dataEOF = true;
				return true;
			}
			return false;
		}

		private void CompactDataBuffer()
		{
			if (this.dataCount == 0)
			{
				this.dataOffset = 0;
				return;
			}
			if (this.data.Length - this.dataOffset + this.dataCount < this.data.Length / 2)
			{
				Buffer.BlockCopy(this.data, this.dataOffset, this.data, 0, this.dataCount);
				this.dataOffset = 0;
			}
		}

		internal void Write(byte[] buffer, int offset, int length)
		{
			if (this.dataEOF)
			{
				throw new InvalidOperationException(Strings.CannotWriteAfterFlush);
			}
			for (;;)
			{
				if (this.currentToken.Id == MimeTokenId.None && this.state != MimeReaderState.Start)
				{
					this.ParseAndCheckSize();
					if (this.currentToken.Id == MimeTokenId.None)
					{
						if (length == 0)
						{
							break;
						}
						int num = this.AddMoreData(buffer, offset, length, false);
						offset += num;
						length -= num;
						continue;
					}
				}
				this.HandleTokenInPushMode();
			}
		}

		internal void Flush()
		{
			if (this.dataEOF)
			{
				return;
			}
			this.dataEOF = true;
			do
			{
				if (this.currentToken.Id == MimeTokenId.None && this.state != MimeReaderState.Start)
				{
					this.ParseAndCheckSize();
				}
				this.HandleTokenInPushMode();
			}
			while (this.state != MimeReaderState.End);
		}

		private void HandleTokenInPushMode()
		{
			if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartBody | MimeReaderState.InlineBody) && !this.skipPart && (this.currentToken.Id == MimeTokenId.PartData || (this.state == MimeReaderState.InlineBody && (this.currentToken.Id == MimeTokenId.InlineStart || this.currentToken.Id == MimeTokenId.InlineEnd))))
			{
				this.handler.PartContent(this.data, this.dataOffset, (int)this.currentToken.Length);
			}
			if (this.RunStateMachine())
			{
				MimeReaderState mimeReaderState = this.state;
				if (mimeReaderState > MimeReaderState.PartBody)
				{
					if (mimeReaderState <= MimeReaderState.InlineStart)
					{
						if (mimeReaderState != MimeReaderState.PartEnd)
						{
							if (mimeReaderState != MimeReaderState.InlineStart)
							{
								return;
							}
							goto IL_10F;
						}
					}
					else
					{
						if (mimeReaderState == MimeReaderState.InlineBody)
						{
							return;
						}
						if (mimeReaderState != MimeReaderState.InlineEnd)
						{
							if (mimeReaderState != MimeReaderState.End)
							{
								return;
							}
							this.handler.EndOfFile();
							return;
						}
					}
					this.handler.PartEnd();
					return;
				}
				if (mimeReaderState <= MimeReaderState.HeaderComplete)
				{
					switch (mimeReaderState)
					{
					case MimeReaderState.PartStart:
						break;
					case MimeReaderState.Start | MimeReaderState.PartStart:
						return;
					case MimeReaderState.HeaderStart:
					{
						if (this.skipHeaders)
						{
							this.skipHeader = true;
							return;
						}
						HeaderParseOptionInternal headerParseOptionInternal;
						this.handler.HeaderStart(this.currentHeaderId, this.currentHeaderName, out headerParseOptionInternal);
						this.skipHeader = (headerParseOptionInternal == HeaderParseOptionInternal.Skip);
						if (!this.skipHeader)
						{
							this.createHeader = true;
							return;
						}
						return;
					}
					default:
						if (mimeReaderState != MimeReaderState.HeaderComplete)
						{
							return;
						}
						if (this.currentHeader != null && !this.skipHeader)
						{
							this.handler.Header(this.currentHeader);
							return;
						}
						return;
					}
				}
				else
				{
					if (mimeReaderState == MimeReaderState.EndOfHeaders)
					{
						goto IL_275;
					}
					if (mimeReaderState != MimeReaderState.PartBody)
					{
						return;
					}
					return;
				}
				IL_10F:
				this.skipPart = false;
				this.skipHeaders = false;
				PartParseOptionInternal partParseOptionInternal;
				Stream stream;
				this.handler.PartStart(this.state == MimeReaderState.InlineStart, (this.state == MimeReaderState.InlineStart) ? this.InlineFileName : null, out partParseOptionInternal, out stream);
				if (stream != null)
				{
					if (this.outerContentStream != null)
					{
						throw new NotSupportedException(Strings.MimeHandlerErrorMoreThanOneOuterContentPushStream);
					}
					this.outerContentStream = stream;
					this.outerContentDepth = this.depth;
				}
				if (partParseOptionInternal == PartParseOptionInternal.Skip)
				{
					this.skipPart = true;
					this.parser.SetStreamMode();
					this.state = ((this.state == MimeReaderState.InlineStart) ? MimeReaderState.InlineBody : MimeReaderState.PartBody);
				}
				else if (partParseOptionInternal == PartParseOptionInternal.ParseSkipHeaders)
				{
					this.skipHeaders = true;
				}
				else if (partParseOptionInternal == PartParseOptionInternal.ParseRawOuterContent)
				{
					this.parser.SetStreamMode();
					this.state = ((this.state == MimeReaderState.InlineStart) ? MimeReaderState.InlineBody : MimeReaderState.PartBody);
				}
				if (this.skipPart || this.state != MimeReaderState.InlineStart)
				{
					return;
				}
				IL_275:
				PartContentParseOptionInternal partContentParseOptionInternal;
				this.handler.EndOfHeaders(this.parser.IsMime ? this.currentPartContentType : ((this.state == MimeReaderState.InlineStart) ? "application/octet-stream" : "text/plain"), this.parser.TransferEncoding, out partContentParseOptionInternal);
				if (partContentParseOptionInternal == PartContentParseOptionInternal.Skip)
				{
					if (this.state != MimeReaderState.InlineStart)
					{
						this.parser.SetContentType(MajorContentType.Other, default(MimeString));
					}
					this.state = ((this.state == MimeReaderState.InlineStart) ? MimeReaderState.InlineBody : MimeReaderState.PartBody);
					this.skipPart = true;
					return;
				}
				if (partContentParseOptionInternal == PartContentParseOptionInternal.ParseRawContent)
				{
					if (this.state != MimeReaderState.InlineStart)
					{
						this.parser.SetContentType(MajorContentType.Other, default(MimeString));
					}
					this.state = ((this.state == MimeReaderState.InlineStart) ? MimeReaderState.InlineBody : MimeReaderState.PartBody);
					return;
				}
				if (partContentParseOptionInternal == PartContentParseOptionInternal.ParseEmbeddedMessage)
				{
					if (this.currentPartMajorType != MajorContentType.MessageRfc822)
					{
						throw new NotSupportedException(Strings.MimeHandlerErrorNotEmbeddedMessage);
					}
				}
				else
				{
					if (this.currentPartMajorType == MajorContentType.MessageRfc822)
					{
						this.parser.SetContentType(MajorContentType.Other, default(MimeString));
					}
					if (this.currentPartMajorType != MajorContentType.Multipart || !this.parser.IsMime)
					{
						this.state = ((this.state == MimeReaderState.InlineStart) ? MimeReaderState.InlineBody : MimeReaderState.PartBody);
						return;
					}
				}
			}
		}

		private bool TrySkipToNextPartBoundary(bool stopAtPartEnd)
		{
			while (this.state != MimeReaderState.End)
			{
				if (!this.TryReachNextState())
				{
					return false;
				}
				if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart) || (stopAtPartEnd && MimeReader.StateIsOneOf(this.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd)))
				{
					break;
				}
			}
			return true;
		}

		internal bool TryReachNextState()
		{
			while (this.state != MimeReaderState.End)
			{
				if (this.currentToken.Id == MimeTokenId.None)
				{
					if (this.state == MimeReaderState.Start)
					{
						this.RunStateMachine();
						break;
					}
					this.ParseAndCheckSize();
					if (this.currentToken.Id == MimeTokenId.None)
					{
						if (this.mimeStream == null || !this.ReadMoreData())
						{
							this.dataExhausted = true;
							return false;
						}
						continue;
					}
				}
				if (this.RunStateMachine())
				{
					break;
				}
			}
			this.dataExhausted = false;
			return true;
		}

		private bool RunStateMachine()
		{
			MimeReaderState mimeReaderState = this.state;
			if (mimeReaderState <= MimeReaderState.PartBody)
			{
				if (mimeReaderState <= MimeReaderState.HeaderComplete)
				{
					switch (mimeReaderState)
					{
					case MimeReaderState.Start:
						this.depth++;
						this.StartPart();
						this.state = MimeReaderState.PartStart;
						return true;
					case MimeReaderState.PartStart:
						break;
					case MimeReaderState.Start | MimeReaderState.PartStart:
						goto IL_57D;
					case MimeReaderState.HeaderStart:
						this.CreateHeader();
						this.ContinueHeader();
						if (this.parser.IsHeaderComplete)
						{
							this.EndHeader();
							this.ConsumeCurrentToken();
							this.state = MimeReaderState.HeaderComplete;
							return true;
						}
						this.ConsumeCurrentToken();
						this.state = MimeReaderState.HeaderIncomplete;
						return false;
					default:
						if (mimeReaderState != MimeReaderState.HeaderIncomplete)
						{
							if (mimeReaderState != MimeReaderState.HeaderComplete)
							{
								goto IL_57D;
							}
						}
						else
						{
							if (this.currentToken.Id != MimeTokenId.HeaderContinuation)
							{
								this.EndHeader();
								this.state = MimeReaderState.HeaderComplete;
								return true;
							}
							this.ContinueHeader();
							this.ConsumeCurrentToken();
							if (this.parser.IsHeaderComplete)
							{
								this.EndHeader();
								this.state = MimeReaderState.HeaderComplete;
								return true;
							}
							return false;
						}
						break;
					}
					if (this.currentToken.Id == MimeTokenId.Header)
					{
						this.StartHeader();
						this.state = MimeReaderState.HeaderStart;
						return true;
					}
					this.ConsumeCurrentToken();
					this.state = MimeReaderState.EndOfHeaders;
					return true;
				}
				else
				{
					if (mimeReaderState != MimeReaderState.EndOfHeaders)
					{
						if (mimeReaderState != MimeReaderState.PartPrologue)
						{
							if (mimeReaderState != MimeReaderState.PartBody)
							{
								goto IL_57D;
							}
							if (this.currentToken.Id == MimeTokenId.PartData)
							{
								this.ConsumeCurrentToken();
								return false;
							}
							this.EndPart();
							this.state = MimeReaderState.PartEnd;
							return true;
						}
					}
					else if (this.currentToken.Id == MimeTokenId.EmbeddedStart)
					{
						this.ConsumeCurrentToken();
						if (this.parseEmbeddedMessages)
						{
							this.depth++;
							this.embeddedDepth++;
							this.StartPart();
							this.state = MimeReaderState.PartStart;
							return true;
						}
						this.state = MimeReaderState.Embedded;
						return true;
					}
					else
					{
						if (this.currentToken.Id == MimeTokenId.NestedStart)
						{
							this.depth++;
							this.StartPart();
							this.ConsumeCurrentToken();
							this.state = MimeReaderState.PartStart;
							return true;
						}
						if (this.currentToken.Id != MimeTokenId.PartData)
						{
							this.EndPart();
							this.state = MimeReaderState.PartEnd;
							return true;
						}
						if (this.parser.ContentType != MajorContentType.Multipart)
						{
							this.state = MimeReaderState.PartBody;
							return true;
						}
						this.state = MimeReaderState.PartPrologue;
					}
					if (this.currentToken.Id == MimeTokenId.NestedStart)
					{
						this.depth++;
						this.StartPart();
						this.ConsumeCurrentToken();
						this.state = MimeReaderState.PartStart;
						return true;
					}
					if (this.currentToken.Id == MimeTokenId.PartData)
					{
						this.ConsumeCurrentToken();
						return false;
					}
					this.EndPart();
					this.state = MimeReaderState.PartEnd;
					return true;
				}
			}
			else if (mimeReaderState <= MimeReaderState.InlineStart)
			{
				if (mimeReaderState != MimeReaderState.PartEpilogue)
				{
					if (mimeReaderState != MimeReaderState.PartEnd)
					{
						if (mimeReaderState == MimeReaderState.InlineStart)
						{
							this.state = MimeReaderState.InlineBody;
							return true;
						}
					}
					else
					{
						if (this.currentToken.Id == MimeTokenId.NestedNext)
						{
							this.StartPart();
							this.ConsumeCurrentToken();
							this.state = MimeReaderState.PartStart;
							return true;
						}
						if (this.currentToken.Id == MimeTokenId.NestedEnd)
						{
							this.ConsumeCurrentToken();
							this.depth--;
							this.state = MimeReaderState.PartEpilogue;
							return false;
						}
						if (this.currentToken.Id == MimeTokenId.InlineStart)
						{
							this.StartPart();
							this.ParseInlineFileName();
							this.state = MimeReaderState.InlineStart;
							return true;
						}
						if (this.currentToken.Id == MimeTokenId.EmbeddedEnd && this.parseEmbeddedMessages)
						{
							this.ConsumeCurrentToken();
							this.depth--;
							this.embeddedDepth--;
							this.EndPart();
							return true;
						}
						this.depth--;
						this.state = MimeReaderState.End;
						return true;
					}
				}
				else
				{
					if (this.currentToken.Id == MimeTokenId.PartData)
					{
						this.ConsumeCurrentToken();
						return false;
					}
					this.EndPart();
					this.state = MimeReaderState.PartEnd;
					return true;
				}
			}
			else
			{
				if (mimeReaderState <= MimeReaderState.InlineEnd)
				{
					if (mimeReaderState != MimeReaderState.InlineBody)
					{
						if (mimeReaderState != MimeReaderState.InlineEnd)
						{
							goto IL_57D;
						}
						this.depth--;
						this.state = MimeReaderState.InlineJunk;
					}
					else
					{
						if (this.currentToken.Id == MimeTokenId.InlineEnd)
						{
							this.ConsumeCurrentToken();
							this.EndPart();
							this.state = MimeReaderState.InlineEnd;
							return true;
						}
						this.ConsumeCurrentToken();
						return false;
					}
				}
				else if (mimeReaderState != MimeReaderState.InlineJunk)
				{
					if (mimeReaderState != MimeReaderState.EmbeddedEnd)
					{
						goto IL_57D;
					}
					if (this.currentToken.Id == MimeTokenId.EmbeddedEnd)
					{
						if (this.cleanupDepth == 0)
						{
							this.ConsumeCurrentToken();
							this.EndPart();
							this.state = MimeReaderState.PartEnd;
							return true;
						}
						this.cleanupDepth--;
					}
					else if (this.currentToken.Id == MimeTokenId.InlineStart || this.currentToken.Id == MimeTokenId.NestedStart)
					{
						this.cleanupDepth++;
					}
					else if (this.currentToken.Id == MimeTokenId.InlineEnd || this.currentToken.Id == MimeTokenId.NestedEnd)
					{
						this.cleanupDepth--;
					}
					this.ConsumeCurrentToken();
					return false;
				}
				if (this.currentToken.Id == MimeTokenId.PartData)
				{
					this.ConsumeCurrentToken();
					return false;
				}
				if (this.currentToken.Id == MimeTokenId.InlineStart)
				{
					this.depth++;
					this.StartPart();
					this.ParseInlineFileName();
					this.state = MimeReaderState.InlineStart;
					return true;
				}
				if (this.currentToken.Id == MimeTokenId.EmbeddedEnd && this.parseEmbeddedMessages)
				{
					this.ConsumeCurrentToken();
					this.EndPart();
					this.state = MimeReaderState.PartEnd;
					return true;
				}
				this.state = MimeReaderState.End;
				return true;
			}
			IL_57D:
			throw new InvalidOperationException();
		}

		private void ConsumeCurrentToken()
		{
			if (this.currentToken.Length != 0)
			{
				if (this.outerContentStream != null)
				{
					this.outerContentStream.Write(this.data, this.dataOffset, (int)this.currentToken.Length);
				}
				this.currentLineTerminationState = MimeCommon.AdvanceLineTerminationState(this.currentLineTerminationState, this.data, this.dataOffset, (int)this.currentToken.Length);
				this.parser.ReportConsumedData((int)this.currentToken.Length);
				this.dataOffset += (int)this.currentToken.Length;
				this.dataCount -= (int)this.currentToken.Length;
				this.currentToken.Length = 0;
			}
			this.currentToken.Id = MimeTokenId.None;
		}

		private void StartPart()
		{
			this.CheckPartsLimit();
			this.currentPartMajorType = MajorContentType.Other;
			this.currentPartContentType = null;
			this.currentPartContentTransferEncoding = ContentTransferEncoding.SevenBit;
			this.enableReadingOuterContent = false;
			this.currentHeader = null;
			this.createHeader = false;
			this.inlineFileName = default(MimeString);
			this.decoder = null;
			this.contentStream = null;
		}

		private void EndPart()
		{
			if (this.outerContentStream != null && this.depth == this.outerContentDepth)
			{
				this.outerContentStream.Flush();
				this.outerContentStream = null;
			}
		}

		private void ParseInlineFileName()
		{
			this.currentPartContentType = "application/octet-stream";
			this.currentPartContentTransferEncoding = this.parser.InlineFormat;
			if (this.parser.InlineFormat == ContentTransferEncoding.UUEncode)
			{
				int num = this.dataOffset + 6;
				while (num < this.dataOffset + (int)this.currentToken.Length && this.data[num] >= 48 && this.data[num] <= 55)
				{
					num++;
				}
				num += MimeScan.SkipLwsp(this.data, num, this.dataOffset + (int)this.currentToken.Length - num);
				int count = this.TrimEndOfLine(num, this.dataOffset + (int)this.currentToken.Length - num);
				this.inlineFileName = new MimeString(this.data, num, count);
				return;
			}
			this.inlineFileName = default(MimeString);
		}

		private void StartHeader()
		{
			if (++this.headerCount > this.MimeLimits.MaxHeaders)
			{
				throw new MimeException(Strings.TooManyHeaders(this.headerCount, this.MimeLimits.MaxHeaders));
			}
			this.currentHeaderId = ((this.parser.HeaderNameLength == 0) ? HeaderId.Unknown : Header.GetHeaderId(this.data, this.dataOffset, this.parser.HeaderNameLength));
			this.currentHeaderName = ((this.parser.HeaderNameLength == 0) ? null : ByteString.BytesToString(this.data, this.dataOffset, this.parser.HeaderNameLength, false));
			this.currentHeader = null;
			bool flag = false;
			if (this.currentHeaderId == HeaderId.ContentType || this.currentHeaderId == HeaderId.ContentTransferEncoding || this.currentHeaderId == HeaderId.MimeVersion)
			{
				flag = true;
			}
			else if (this.MimeLimits.MaxAddressItemsPerHeader < 2147483647 || this.MimeLimits.MaxParametersPerHeader < 2147483647 || this.MimeLimits.MaxTextValueBytesPerValue < 2147483647)
			{
				Type left = Header.TypeFromHeaderId(this.currentHeaderId);
				if ((left == typeof(AddressHeader) && (this.MimeLimits.MaxParametersPerHeader < 2147483647 || this.MimeLimits.MaxAddressItemsPerHeader < 2147483647)) || ((left == typeof(ContentTypeHeader) || left == typeof(ContentDispositionHeader)) && (this.MimeLimits.MaxParametersPerHeader < 2147483647 || this.MimeLimits.MaxAddressItemsPerHeader < 2147483647)))
				{
					flag = true;
				}
			}
			this.createHeader = flag;
		}

		private void CreateHeader()
		{
			if (this.createHeader && this.parser.HeaderNameLength != 0)
			{
				if (this.currentHeaderId != HeaderId.Unknown)
				{
					this.currentHeader = Header.Create(this.currentHeaderName, this.currentHeaderId);
				}
				else
				{
					this.currentHeader = Header.CreateGeneralHeader(this.currentHeaderName);
				}
			}
			else
			{
				this.currentHeader = null;
			}
			this.currentHeaderEmpty = true;
		}

		private void ContinueHeader()
		{
			this.CheckHeaderBytesLimits();
			if (this.currentHeader != null)
			{
				int num = this.dataOffset + this.parser.HeaderDataOffset;
				int num2 = (int)this.currentToken.Length - this.parser.HeaderDataOffset;
				num2 = this.TrimEndOfLine(num, num2);
				if (this.currentHeaderEmpty)
				{
					int num3 = MimeScan.SkipLwsp(this.data, num, num2);
					num += num3;
					num2 -= num3;
				}
				if (num2 != 0)
				{
					this.currentHeaderEmpty = false;
					this.currentHeader.AppendLine(MimeString.CopyData(this.data, num, num2), false);
				}
			}
		}

		private void EndHeader()
		{
			if (this.currentHeader != null)
			{
				if (this.currentHeader is ComplexHeader)
				{
					if (this.MimeLimits.MaxParametersPerHeader < 2147483647 || this.MimeLimits.MaxTextValueBytesPerValue < 2147483647)
					{
						this.currentHeader.CheckChildrenLimit(this.MimeLimits.MaxParametersPerHeader, this.MimeLimits.MaxTextValueBytesPerValue);
					}
				}
				else if (this.currentHeader is AddressHeader && (this.MimeLimits.MaxAddressItemsPerHeader < 2147483647 || this.MimeLimits.MaxTextValueBytesPerValue < 2147483647))
				{
					this.currentHeader.CheckChildrenLimit(this.MimeLimits.MaxAddressItemsPerHeader, this.MimeLimits.MaxTextValueBytesPerValue);
				}
				if (this.currentHeader.HeaderId == HeaderId.MimeVersion && this.PartDepth == 1)
				{
					this.parser.SetMIME();
					return;
				}
				if (this.currentHeader.HeaderId == HeaderId.ContentTransferEncoding)
				{
					if (this.inferMime && this.PartDepth == 1)
					{
						this.parser.SetMIME();
					}
					ContentTransferEncoding encodingType = MimePart.GetEncodingType(this.currentHeader.FirstRawToken);
					this.parser.SetTransferEncoding(encodingType);
					this.currentPartContentTransferEncoding = encodingType;
					return;
				}
				if (this.currentHeader.HeaderId == HeaderId.ContentType)
				{
					MajorContentType majorContentType = MajorContentType.Other;
					string text = null;
					MimeString boundaryValue = default(MimeString);
					ContentTypeHeader contentTypeHeader = this.currentHeader as ContentTypeHeader;
					if (contentTypeHeader != null)
					{
						if (contentTypeHeader.IsMultipart)
						{
							MimeParameter mimeParameter = contentTypeHeader["boundary"];
							if (mimeParameter != null)
							{
								byte[] rawValue = mimeParameter.RawValue;
								int num = 0;
								if (rawValue != null && (num = rawValue.Length) != 0)
								{
									while (num != 0 && MimeScan.IsLWSP(rawValue[num - 1]))
									{
										num--;
									}
									if (num != 0 && num <= 994)
									{
										if (this.FixBadMimeBoundary)
										{
											int num2 = 0;
											if (num == rawValue.Length && num <= 70)
											{
												while (num2 < num && MimeScan.IsBChar(rawValue[num2]))
												{
													num2++;
												}
											}
											if (num2 != num)
											{
												this.parser.ComplianceStatus |= MimeComplianceStatus.InvalidBoundary;
												mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
											}
										}
										majorContentType = MajorContentType.Multipart;
										boundaryValue = new MimeString(rawValue, 0, num);
									}
								}
								if (rawValue == null || num == 0 || num > 994)
								{
									this.parser.ComplianceStatus |= MimeComplianceStatus.InvalidBoundary;
									contentTypeHeader.Value = "text/plain";
								}
							}
							else
							{
								this.parser.ComplianceStatus |= MimeComplianceStatus.MissingBoundaryParameter;
								contentTypeHeader.Value = "text/plain";
							}
						}
						else if (contentTypeHeader.IsEmbeddedMessage)
						{
							majorContentType = MajorContentType.MessageRfc822;
						}
						else if (contentTypeHeader.IsAnyMessage)
						{
							majorContentType = MajorContentType.Message;
						}
						text = contentTypeHeader.Value;
					}
					if (this.inferMime && this.PartDepth == 1)
					{
						this.parser.SetMIME();
					}
					this.currentPartMajorType = majorContentType;
					this.currentPartContentType = text;
					if (majorContentType == MajorContentType.Multipart || this.parseEmbeddedMessages)
					{
						this.parser.SetContentType(majorContentType, boundaryValue);
						return;
					}
					this.parser.SetContentType(MajorContentType.Other, default(MimeString));
				}
			}
		}

		internal bool ReadPartData(byte[] buffer, int offset, int count, out int readCount)
		{
			readCount = 0;
			this.dataExhausted = false;
			bool flag = true;
			while (count != 0)
			{
				if (this.currentToken.Id == MimeTokenId.None)
				{
					this.ParseAndCheckSize();
					if (this.currentToken.Id == MimeTokenId.None)
					{
						if (this.mimeStream == null || !this.ReadMoreData())
						{
							this.dataExhausted = true;
							return false;
						}
						continue;
					}
				}
				int num;
				if ((this.currentToken.Id != MimeTokenId.PartData && this.currentToken.Id != MimeTokenId.InlineStart && this.currentToken.Id != MimeTokenId.InlineEnd) || (this.state == MimeReaderState.PartBody && this.currentToken.Id == MimeTokenId.InlineStart))
				{
					if (this.decoder != null)
					{
						int num2;
						this.decoder.Convert(this.data, 0, 0, buffer, offset, count, true, out num, out num2, out flag);
						count -= num2;
						offset += num2;
						readCount += num2;
						if (!flag)
						{
							break;
						}
					}
					this.EndPart();
					this.state = MimeReaderState.PartEnd;
					break;
				}
				if (this.decoder != null)
				{
					int num2;
					this.decoder.Convert(this.data, this.dataOffset, (int)this.currentToken.Length, buffer, offset, count, this.currentToken.Id == MimeTokenId.InlineEnd, out num, out num2, out flag);
					count -= num2;
					offset += num2;
					readCount += num2;
				}
				else
				{
					int num2 = num = Math.Min(count, (int)this.currentToken.Length);
					if (num2 != 0)
					{
						if (buffer != null)
						{
							Buffer.BlockCopy(this.data, this.dataOffset, buffer, offset, num2);
							count -= num2;
							offset += num2;
						}
						readCount += num2;
					}
				}
				if (num != 0)
				{
					if (this.outerContentStream != null)
					{
						this.outerContentStream.Write(this.data, this.dataOffset, num);
					}
					this.currentLineTerminationState = MimeCommon.AdvanceLineTerminationState(this.currentLineTerminationState, this.data, this.dataOffset, num);
					this.parser.ReportConsumedData(num);
					this.dataOffset += num;
					this.dataCount -= num;
					this.currentToken.Length = this.currentToken.Length - (short)num;
				}
				if (this.currentToken.Length != 0)
				{
					break;
				}
				if (this.currentToken.Id == MimeTokenId.InlineEnd)
				{
					if (flag)
					{
						this.EndPart();
						this.currentToken.Id = MimeTokenId.None;
						this.state = MimeReaderState.InlineEnd;
						break;
					}
					break;
				}
				else
				{
					this.currentToken.Id = MimeTokenId.None;
				}
			}
			return true;
		}

		internal static bool StateIsOneOf(MimeReaderState state, MimeReaderState set)
		{
			return (state & set) != (MimeReaderState)0;
		}

		internal void AssertGoodToUse(bool pullModeOnly, bool noEmbeddedReader)
		{
			if (this.parser == null)
			{
				throw new ObjectDisposedException("MimeReader");
			}
			if (pullModeOnly && this.mimeStream == null)
			{
				throw new ObjectDisposedException("MimeReader");
			}
			if (noEmbeddedReader && this.childReader != null)
			{
				throw new InvalidOperationException(Strings.EmbeddedMessageReaderNeedsToBeClosedFirst);
			}
		}

		internal void SetEofMeansEndOfFile(bool eofMeansEndOfFile)
		{
			this.eofMeansEndOfFile = eofMeansEndOfFile;
		}

		internal bool TryReadNextPart()
		{
			this.AssertGoodToUse(false, true);
			return this.TrySkipToNextPartBoundary(false) && MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart);
		}

		internal bool TryReadFirstChildPart()
		{
			this.AssertGoodToUse(false, true);
			if (this.state == MimeReaderState.InlineStart)
			{
				return false;
			}
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.Start | MimeReaderState.EndOfHeaders | MimeReaderState.PartPrologue))
			{
				if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete))
				{
					throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
				}
				while (this.TryReachNextState())
				{
					if (this.state == MimeReaderState.EndOfHeaders)
					{
						goto IL_54;
					}
				}
				return false;
			}
			IL_54:
			return (this.state != MimeReaderState.EndOfHeaders || this.IsMultipart || (this.IsEmbeddedMessage && this.parseEmbeddedMessages)) && this.TrySkipToNextPartBoundary(true) && this.state == MimeReaderState.PartStart;
		}

		internal bool TryReadNextSiblingPart()
		{
			this.AssertGoodToUse(false, true);
			if (this.state == MimeReaderState.End)
			{
				return false;
			}
			if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete | MimeReaderState.EndOfHeaders))
			{
				this.createHeader = false;
				while (this.state != MimeReaderState.EndOfHeaders)
				{
					if (!this.TryReachNextState())
					{
						return false;
					}
				}
				this.parser.SetContentType(MajorContentType.Other, default(MimeString));
			}
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.Start | MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
			{
				int num = this.depth;
				while (this.TrySkipToNextPartBoundary(true))
				{
					if (this.depth <= num && MimeReader.StateIsOneOf(this.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
					{
						goto IL_97;
					}
				}
				return false;
			}
			IL_97:
			return this.TrySkipToNextPartBoundary(true) && MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart);
		}

		internal HeaderList TryReadHeaderList()
		{
			this.AssertGoodToUse(false, true);
			if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart) && (!MimeReader.StateIsOneOf(this.state, MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete) || this.headerList == null))
			{
				throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
			}
			if (this.state == MimeReaderState.InlineStart)
			{
				return new HeaderList(null);
			}
			HeaderList headerList;
			if (this.headerList == null)
			{
				headerList = new HeaderList(null);
			}
			else
			{
				headerList = this.headerList;
				this.headerList = null;
			}
			while (this.TryReachNextState())
			{
				if (this.state == MimeReaderState.HeaderStart)
				{
					this.createHeader = true;
				}
				else if (this.state == MimeReaderState.HeaderComplete && this.currentHeader != null)
				{
					headerList.InternalAppendChild(this.currentHeader);
				}
				if (this.state == MimeReaderState.EndOfHeaders)
				{
					return headerList;
				}
			}
			this.headerList = headerList;
			return null;
		}

		internal Stream TryGetRawContentReadStream()
		{
			this.AssertGoodToUse(false, true);
			if (!this.TryInitializeReadContent(false))
			{
				return null;
			}
			this.contentStream = new MimeReader.ContentReadStream(this);
			return this.contentStream;
		}

		private const int DataBufferSize = 5120;

		private bool FixBadMimeBoundary;

		private Stream mimeStream;

		private IMimeHandlerInternal handler;

		private bool inferMime;

		private bool parseEmbeddedMessages;

		private DecodingOptions decodingOptions;

		private MimeLimits limits;

		private MimeParser parser;

		private MimeReaderState state;

		private int depth;

		private int cleanupDepth;

		private int embeddedDepth;

		private bool dataExhausted;

		private bool dataEOF;

		private byte[] data;

		private int dataOffset;

		private int dataCount;

		private MimeToken currentToken;

		private HeaderId currentHeaderId;

		private string currentHeaderName;

		private bool createHeader;

		private Header currentHeader;

		private bool currentHeaderEmpty;

		private bool currentHeaderConsumed;

		private bool currentChildConsumed;

		private MimeNode currentChild;

		private MimeNode currentGrandChild;

		private MajorContentType currentPartMajorType;

		private string currentPartContentType;

		private ContentTransferEncoding currentPartContentTransferEncoding;

		private LineTerminationState currentLineTerminationState;

		private MimeString inlineFileName;

		private ByteEncoder decoder;

		private bool readRawData;

		private Stream contentStream;

		private bool enableReadingOuterContent;

		private Stream outerContentStream;

		private int outerContentDepth;

		private MimeReader childReader;

		private MimeReader parentReader;

		private int partCount;

		private int headerCount;

		private int cumulativeHeaderBytes;

		private int currentTextHeaderBytes;

		private bool skipPart;

		private bool skipHeaders;

		private bool skipHeader;

		private HeaderList headerList;

		private bool eofMeansEndOfFile;

		private class ContentReadStream : Stream
		{
			public ContentReadStream(MimeReader reader)
			{
				this.reader = reader;
			}

			public override bool CanRead
			{
				get
				{
					return this.reader != null;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return false;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			public override long Length
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public override long Position
			{
				get
				{
					throw new NotSupportedException();
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				MimeCommon.CheckBufferArguments(buffer, offset, count);
				if (this.reader.contentStream != this)
				{
					throw new NotSupportedException(Strings.StreamNoLongerValid);
				}
				if (MimeReader.StateIsOneOf(this.reader.state, MimeReaderState.PartBody | MimeReaderState.InlineBody))
				{
					int result;
					this.reader.ReadPartData(buffer, offset, count, out result);
					return result;
				}
				if (MimeReader.StateIsOneOf(this.reader.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
				{
					return 0;
				}
				throw new NotSupportedException(Strings.StreamNoLongerValid);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void Flush()
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			private MimeReader reader;
		}
	}
}
