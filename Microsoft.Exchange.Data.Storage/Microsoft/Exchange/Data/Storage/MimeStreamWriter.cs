using System;
using System.IO;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MimeStreamWriter : IDisposeTrackable, IDisposable
	{
		internal MimeStreamWriter(MimeStreamWriter.Flags flags, EncodingOptions encodingOptions) : this(null, null, encodingOptions, flags)
		{
		}

		internal MimeStreamWriter(Stream mimeOut, EncodingOptions options, MimeStreamWriter.Flags flags) : this(mimeOut, null, options, flags)
		{
		}

		internal MimeStreamWriter(Stream mimeOut, Stream mimeSkeletonOut, EncodingOptions options, MimeStreamWriter.Flags flags)
		{
			this.flags = flags;
			this.mimeWriter = null;
			this.encodingOptions = options;
			if (mimeOut != null)
			{
				if ((flags & MimeStreamWriter.Flags.SkipHeaders) == MimeStreamWriter.Flags.SkipHeaders)
				{
					this.mimeTextStream = new MimeStreamWriter.MimeTextStream(mimeOut);
					mimeOut = this.mimeTextStream;
				}
				this.mimeWriter = new MimeWriter(mimeOut, (flags & MimeStreamWriter.Flags.ForceMime) == MimeStreamWriter.Flags.ForceMime, options);
			}
			if (mimeSkeletonOut != null)
			{
				this.mimeSkeletonWriter = new MimeWriter(mimeSkeletonOut, (flags & MimeStreamWriter.Flags.ForceMime) == MimeStreamWriter.Flags.ForceMime, options);
			}
			this.disposeTracker = this.GetDisposeTracker();
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MimeStreamWriter>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal void StartPart(MimePartInfo part)
		{
			this.StartPart(part, true);
		}

		internal void StartPart(MimePartInfo part, bool outputToSkeleton)
		{
			this.FlushCachedHeader();
			if (this.currentPart != null)
			{
				this.StartWriting();
				if ((this.flags & MimeStreamWriter.Flags.ProduceMimeStructure) == MimeStreamWriter.Flags.ProduceMimeStructure && this.currentPart.IsMultipart)
				{
					this.currentPart.ChildrenWrittenOut();
				}
			}
			MimePartHeaders mimePartHeaders = (part == null) ? null : part.Headers;
			this.currentPart = part;
			this.hasAllHeaders = (mimePartHeaders != null);
			if (this.mimeWriter != null)
			{
				this.mimeWriter.StartPart();
				if (this.hasAllHeaders)
				{
					this.CopyHeadersToWriter(mimePartHeaders, this.mimeWriter);
				}
			}
			if (this.mimeSkeletonWriter != null && outputToSkeleton)
			{
				this.mimeSkeletonWriter.StartPart();
				if (mimePartHeaders != null)
				{
					this.CopyHeadersToWriter(mimePartHeaders, this.mimeSkeletonWriter);
				}
			}
			this.assembleHeaders = false;
			if (this.currentPart != null && (this.flags & MimeStreamWriter.Flags.ProduceMimeStructure) == MimeStreamWriter.Flags.ProduceMimeStructure)
			{
				if (mimePartHeaders == null)
				{
					this.assembleHeaders = true;
					if ((this.flags & MimeStreamWriter.Flags.ForceMime) == MimeStreamWriter.Flags.ForceMime)
					{
						Header header = Header.Create(HeaderId.MimeVersion);
						header.Value = "1.0";
						this.currentPart.AddHeader(header);
					}
				}
				this.flags &= (MimeStreamWriter.Flags)(-5);
			}
		}

		private void CopyHeadersToWriter(MimePartHeaders headers, MimeWriter writer)
		{
			Header header = null;
			foreach (Header header2 in headers)
			{
				if (header2.HeaderId == HeaderId.MimeVersion)
				{
					header = header2;
				}
				else
				{
					header2.WriteTo(writer);
				}
			}
			if (header != null)
			{
				header.WriteTo(writer);
			}
		}

		internal void EndPart()
		{
			this.EndPart(true);
		}

		internal void EndPart(bool outputToSkeleton)
		{
			this.FlushCachedHeader();
			if (this.mimeWriter != null)
			{
				this.mimeWriter.EndPart();
			}
			if (this.mimeSkeletonWriter != null && outputToSkeleton)
			{
				this.mimeSkeletonWriter.EndPart();
			}
			this.currentPart = null;
		}

		private void FlushCachedHeader()
		{
			if (this.currentHeader == null || this.hasAllHeaders)
			{
				return;
			}
			if (!string.Equals(this.currentHeader.Name, "X-Exchange-Mime-Skeleton-Content-Id", StringComparison.OrdinalIgnoreCase))
			{
				if (this.mimeWriter != null)
				{
					this.currentHeader.WriteTo(this.mimeWriter);
				}
				if (this.assembleHeaders && this.currentPart != null)
				{
					this.currentPart.AddHeader(this.currentHeader);
				}
			}
			if (this.mimeSkeletonWriter != null)
			{
				this.currentHeader.WriteTo(this.mimeSkeletonWriter);
			}
			this.currentHeader = null;
		}

		internal static void CalculateBodySize(MimePartInfo partInfo, MimePart part)
		{
			if (partInfo.IsBodySizeComputed)
			{
				return;
			}
			using (Stream stream = new MimeStreamWriter.MimeBodySizeCounter(null, partInfo))
			{
				using (Stream rawContentReadStream = part.GetRawContentReadStream())
				{
					Util.StreamHandler.CopyStreamData(rawContentReadStream, stream);
				}
			}
		}

		internal void WritePartWithHeaders(MimePart part, bool copyBoundaryToSkeleton)
		{
			this.StartPart(null, copyBoundaryToSkeleton);
			if ((this.flags & MimeStreamWriter.Flags.SkipHeaders) == MimeStreamWriter.Flags.SkipHeaders && this.mimeTextStream != null)
			{
				this.mimeTextStream.StartWriting();
				this.flags &= (MimeStreamWriter.Flags)(-2);
				if (this.mimeWriter == null)
				{
					goto IL_98;
				}
				using (Stream rawContentWriteStream = this.mimeWriter.GetRawContentWriteStream())
				{
					using (Stream rawContentReadStream = part.GetRawContentReadStream())
					{
						Util.StreamHandler.CopyStreamData(rawContentReadStream, rawContentWriteStream);
					}
					goto IL_98;
				}
			}
			if (this.mimeWriter != null)
			{
				using (Stream rawContentWriteStream2 = this.mimeWriter.GetRawContentWriteStream())
				{
					part.WriteTo(rawContentWriteStream2);
				}
			}
			IL_98:
			this.EndPart(copyBoundaryToSkeleton);
		}

		internal void WriteHeadersFromPart(MimePart part)
		{
			this.StartPart(null, false);
			if (this.mimeWriter != null)
			{
				foreach (Header header in part.Headers)
				{
					header.WriteTo(this.mimeWriter);
				}
			}
			this.EndPart(false);
		}

		internal void WriteHeader(Header header)
		{
			if (this.hasAllHeaders)
			{
				return;
			}
			this.FlushCachedHeader();
			this.currentHeader = header;
		}

		internal void WriteHeader(string name, string data)
		{
			if (this.hasAllHeaders)
			{
				return;
			}
			Header header = Header.Create(name);
			if (MimeStreamWriter.CheckHeaderValue(header, data))
			{
				header = MimeStreamWriter.CopyHeader(header, data);
				this.WriteHeader(header);
			}
		}

		internal void WriteHeader(HeaderId type, string data)
		{
			if (this.hasAllHeaders)
			{
				return;
			}
			Header header = Header.Create(type);
			if (MimeStreamWriter.CheckHeaderValue(header, data))
			{
				header = MimeStreamWriter.CopyHeader(header, data);
				this.WriteHeader(header);
			}
		}

		internal void WriteHeader(HeaderId id, ExDateTime data)
		{
			if (this.hasAllHeaders)
			{
				return;
			}
			DateHeader header = (DateHeader)Header.Create(id);
			MimeInternalHelpers.SetDateHeaderValue(header, data.UniversalTime, data.Bias);
			this.WriteHeader(header);
		}

		internal void WriteHeaderParameter(string parameterName, string parameterValue)
		{
			if (this.hasAllHeaders)
			{
				return;
			}
			ComplexHeader complexHeader = (ComplexHeader)this.currentHeader;
			MimeParameter mimeParameter = complexHeader[parameterName];
			if (mimeParameter == null)
			{
				mimeParameter = new MimeParameter(parameterName);
				complexHeader.AppendChild(mimeParameter);
			}
			mimeParameter.Value = parameterValue;
		}

		private static Header CopyHeader(Header header, string data)
		{
			if (data != null)
			{
				if (header is AddressHeader)
				{
					header = AddressHeader.Parse(header.Name, data, AddressParserFlags.IgnoreComments);
				}
				else
				{
					header.Value = data;
				}
			}
			return header;
		}

		internal MimeStreamWriter GetEmbeddedWriter(EncodingOptions encodingOptions, Stream embeddedSkeleton, OutboundConversionOptions options)
		{
			return new MimeStreamWriter(this.GetContentStream(false), embeddedSkeleton, encodingOptions, (this.flags & MimeStreamWriter.Flags.ProduceMimeStructure) | MimeStreamWriter.Flags.ForceMime);
		}

		internal void WriteMailBox(string displayName, string address)
		{
			if (this.hasAllHeaders)
			{
				return;
			}
			if (address == null)
			{
				address = string.Empty;
			}
			AddressHeader addressHeader = (AddressHeader)this.currentHeader;
			MimeRecipient newChild = new MimeRecipient(displayName, address);
			addressHeader.AppendChild(newChild);
		}

		internal Stream GetContentStream(bool writeToSkeleton)
		{
			this.StartWriting();
			this.Flush();
			Stream stream = null;
			bool flag = false;
			try
			{
				if (this.mimeWriter != null)
				{
					stream = this.mimeWriter.GetRawContentWriteStream();
				}
				if (writeToSkeleton && this.mimeSkeletonWriter != null)
				{
					Stream rawContentWriteStream = this.mimeSkeletonWriter.GetRawContentWriteStream();
					stream = ((stream == null) ? rawContentWriteStream : new MimeStreamWriter.SplitterWriteStream(stream, rawContentWriteStream));
				}
				if (this.currentPart != null && (this.flags & MimeStreamWriter.Flags.ProduceMimeStructure) == MimeStreamWriter.Flags.ProduceMimeStructure && !this.currentPart.IsBodySizeComputed)
				{
					stream = new MimeStreamWriter.MimeBodySizeCounter(stream, this.currentPart);
				}
				if (stream == null)
				{
					stream = new MimeStreamWriter.MimeTextStream(null);
				}
				flag = true;
			}
			finally
			{
				if (!flag && stream != null)
				{
					stream.Dispose();
				}
			}
			return stream;
		}

		private void StartWriting()
		{
			this.FlushCachedHeader();
			this.Flush();
			if ((this.flags & MimeStreamWriter.Flags.SkipHeaders) == MimeStreamWriter.Flags.SkipHeaders && this.mimeTextStream != null)
			{
				this.flags &= (MimeStreamWriter.Flags)(-2);
				if (this.mimeWriter != null)
				{
					this.mimeTextStream.StartWriting();
				}
			}
		}

		internal void Flush()
		{
			this.FlushCachedHeader();
			if (this.mimeWriter != null)
			{
				this.mimeWriter.Flush();
			}
			if (this.mimeSkeletonWriter != null)
			{
				this.mimeSkeletonWriter.Flush();
			}
		}

		private void Dispose(bool isDisposing)
		{
			this.FlushCachedHeader();
			if (isDisposing)
			{
				if (this.mimeWriter != null)
				{
					this.mimeWriter.Close();
				}
				if (this.mimeSkeletonWriter != null)
				{
					this.mimeSkeletonWriter.Close();
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal static bool CheckAsciiHeaderValue(string value)
		{
			if (value == null)
			{
				return true;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] >= '\u0080')
				{
					return false;
				}
			}
			return true;
		}

		internal static bool CheckHeaderValue(Header header, string value)
		{
			if (header is TextHeader)
			{
				return true;
			}
			if (header is AddressHeader)
			{
				return true;
			}
			if (header is AsciiTextHeader)
			{
				return MimeStreamWriter.CheckAsciiHeaderValue(value);
			}
			if (!(header is ComplexHeader))
			{
				return value == null;
			}
			if (!MimeStreamWriter.CheckAsciiHeaderValue(value))
			{
				throw new InvalidOperationException(string.Format("ComplexHeader {0} value {1} is not ASCII", header.Name, value));
			}
			return true;
		}

		internal string WriterCharsetName
		{
			get
			{
				return this.encodingOptions.CharsetName;
			}
		}

		internal Charset WriterCharset
		{
			get
			{
				return Charset.GetCharset(this.encodingOptions.CharsetName);
			}
		}

		private MimeWriter mimeWriter;

		private MimeWriter mimeSkeletonWriter;

		private MimeStreamWriter.MimeTextStream mimeTextStream;

		private EncodingOptions encodingOptions;

		private MimeStreamWriter.Flags flags;

		private bool assembleHeaders;

		private bool hasAllHeaders;

		private MimePartInfo currentPart;

		private Header currentHeader;

		private DisposeTracker disposeTracker;

		internal enum Flags
		{
			None,
			SkipHeaders,
			ProduceMimeStructure,
			ForceMime = 4
		}

		internal class MimeTextStream : StreamWrapper
		{
			internal MimeTextStream(Stream targetStream) : base(targetStream, false, StreamBase.Capabilities.Writable)
			{
				this.isWriting = false;
				this.skippedCR = false;
				this.skippedLF = false;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				if (this.isWriting && count > 0)
				{
					if (!this.skippedCR && buffer[offset] == 13)
					{
						this.skippedCR = true;
						this.Write(buffer, offset + 1, count - 1);
						return;
					}
					if (this.skippedCR && !this.skippedLF && buffer[offset] == 10)
					{
						this.skippedLF = true;
						this.Write(buffer, offset + 1, count - 1);
						return;
					}
					this.skippedCR = true;
					this.skippedLF = true;
					base.InternalStream.Write(buffer, offset, count);
				}
			}

			public override void Flush()
			{
				if (this.isWriting)
				{
					base.InternalStream.Flush();
				}
			}

			internal void StartWriting()
			{
				this.isWriting = true;
			}

			private bool isWriting;

			private bool skippedCR;

			private bool skippedLF;
		}

		internal class SplitterWriteStream : StreamWrapper
		{
			internal SplitterWriteStream(Stream targetStream1, Stream targetStream2) : base(targetStream1, true, StreamBase.Capabilities.Writable)
			{
				this.targetStream2 = targetStream2;
			}

			protected override void Dispose(bool disposing)
			{
				try
				{
					if (disposing && !base.IsClosed && this.targetStream2 != null)
					{
						this.targetStream2.Dispose();
						this.targetStream2 = null;
					}
				}
				finally
				{
					base.Dispose(disposing);
				}
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				base.InternalStream.Write(buffer, offset, count);
				this.targetStream2.Write(buffer, offset, count);
			}

			public override void Flush()
			{
				base.InternalStream.Flush();
				this.targetStream2.Flush();
			}

			private Stream targetStream2;
		}

		internal class MimeBodySizeCounter : StreamWrapper
		{
			internal MimeBodySizeCounter(Stream stream, MimePartInfo part) : base(stream, true, StreamBase.Capabilities.Writable)
			{
				this.part = part;
				this.lineCount = 0;
				this.byteCount = 0;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				if (base.InternalStream != null)
				{
					base.InternalStream.Write(buffer, offset, count);
				}
				this.byteCount += count;
				for (int num = 0; num != count; num++)
				{
					if (buffer[offset + num] == 10)
					{
						this.lineCount++;
					}
				}
			}

			public override void Flush()
			{
				if (base.InternalStream != null)
				{
					base.InternalStream.Flush();
				}
			}

			protected override void Dispose(bool disposing)
			{
				try
				{
					if (disposing)
					{
						this.part.SetBodySize(this.byteCount, this.lineCount);
					}
				}
				finally
				{
					base.Dispose(disposing);
				}
			}

			private MimePartInfo part;

			private int lineCount;

			private int byteCount;
		}
	}
}
