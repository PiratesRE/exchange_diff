using System;
using System.IO;

namespace Microsoft.Exchange.Data.Mime.Internal
{
	internal class MimePushParser : Stream, IMimeHandlerInternal
	{
		public MimePushParser(IMimeHandler handler) : this(handler, true, DecodingOptions.Default, MimeLimits.Default, true)
		{
		}

		public MimePushParser(IMimeHandler handler, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits, bool parseInline)
		{
			this.handler = handler;
			this.reader = new MimeReader(this, inferMime, decodingOptions, mimeLimits, parseInline);
		}

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.reader != null;
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
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int length)
		{
			if (this.reader == null)
			{
				throw new ObjectDisposedException("MimePushParser");
			}
			this.reader.Write(buffer, offset, length);
		}

		public override void Flush()
		{
			if (this.reader == null)
			{
				throw new ObjectDisposedException("MimePushParser");
			}
			this.reader.Flush();
		}

		protected override void Dispose(bool disposing)
		{
			if (this.reader != null)
			{
				this.reader.Close();
				this.reader = null;
			}
			base.Dispose(disposing);
		}

		void IMimeHandlerInternal.PartStart(bool isInline, string inlineFileName, out PartParseOptionInternal partParseOption, out Stream outerContentWriteStream)
		{
			PartParseOption partParseOption2;
			this.handler.PartStart(isInline, inlineFileName, out partParseOption2, out outerContentWriteStream);
			partParseOption = (PartParseOptionInternal)partParseOption2;
		}

		void IMimeHandlerInternal.HeaderStart(HeaderId headerId, string name, out HeaderParseOptionInternal headerParseOption)
		{
			HeaderParseOption headerParseOption2;
			this.handler.HeaderStart(headerId, name, out headerParseOption2);
			headerParseOption = (HeaderParseOptionInternal)headerParseOption2;
		}

		void IMimeHandlerInternal.Header(Header header)
		{
			this.handler.Header(header);
		}

		void IMimeHandlerInternal.EndOfHeaders(string mediaType, ContentTransferEncoding cte, out PartContentParseOptionInternal partContentParseOption)
		{
			PartContentParseOption partContentParseOption2;
			this.handler.EndOfHeaders(mediaType, cte, out partContentParseOption2);
			partContentParseOption = (PartContentParseOptionInternal)partContentParseOption2;
		}

		void IMimeHandlerInternal.PartContent(byte[] buffer, int offset, int length)
		{
			this.handler.PartContent(buffer, offset, length);
		}

		void IMimeHandlerInternal.PartEnd()
		{
			this.handler.PartEnd();
		}

		void IMimeHandlerInternal.EndOfFile()
		{
			this.handler.EndOfFile();
		}

		private MimeReader reader;

		private IMimeHandler handler;
	}
}
