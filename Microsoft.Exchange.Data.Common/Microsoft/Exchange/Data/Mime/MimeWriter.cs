using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.Mime
{
	public class MimeWriter : IDisposable
	{
		public MimeWriter(Stream data) : this(data, true, MimeCommon.DefaultEncodingOptions)
		{
		}

		public MimeWriter(Stream data, bool forceMime, EncodingOptions encodingOptions)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!data.CanWrite)
			{
				throw new ArgumentException("Stream must support writing", "data");
			}
			this.forceMime = forceMime;
			this.data = data;
			this.encodingOptions = encodingOptions;
			this.shimStream = new MimeWriter.WriterQueueStream(this);
		}

		public int PartDepth
		{
			get
			{
				this.AssertOpen();
				return this.partDepth;
			}
		}

		public int EmbeddedDepth
		{
			get
			{
				this.AssertOpen();
				return this.embeddedDepth;
			}
		}

		public int StreamOffset
		{
			get
			{
				this.AssertOpen();
				return this.bytesWritten;
			}
		}

		public EncodingOptions EncodingOptions
		{
			get
			{
				this.AssertOpen();
				return this.encodingOptions;
			}
		}

		public string PartBoundary
		{
			get
			{
				this.AssertOpen();
				string result = null;
				switch (this.state)
				{
				case MimeWriteState.StartPart:
				case MimeWriteState.Headers:
					if (this.currentPart.IsMultipart)
					{
						result = ByteString.BytesToString(this.currentPart.Boundary, false);
					}
					break;
				}
				return result;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.data == null)
			{
				return;
			}
			try
			{
				if (disposing)
				{
					while (this.partDepth != 0)
					{
						this.EndPart();
					}
					this.FlushWriteQueue();
					if (this.lineTermination != LineTerminationState.CRLF)
					{
						if (this.lineTermination == LineTerminationState.CR)
						{
							this.data.Write(MimeString.CrLf, 1, 1);
						}
						else
						{
							this.data.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
						}
						this.lineTermination = LineTerminationState.CRLF;
					}
				}
			}
			finally
			{
				if (disposing)
				{
					if (this.encodedPartContent != null)
					{
						this.encodedPartContent.Dispose();
					}
					if (this.partContent != null)
					{
						this.partContent.Dispose();
					}
					this.data.Dispose();
				}
				this.state = MimeWriteState.Complete;
				this.encodedPartContent = null;
				this.partContent = null;
				this.data = null;
			}
		}

		public virtual void Close()
		{
			this.Dispose();
		}

		public void WritePart(MimeReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.AssertOpen();
			if (!MimeReader.StateIsOneOf(reader.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart))
			{
				throw new InvalidOperationException(Strings.OperationNotValidInThisReaderState);
			}
			this.StartPart();
			MimeHeaderReader headerReader = reader.HeaderReader;
			while (headerReader.ReadNextHeader())
			{
				this.WriteHeader(headerReader);
			}
			this.WriteContent(reader);
			this.EndPart();
		}

		public void WriteHeader(MimeHeaderReader reader)
		{
			this.AssertOpen();
			Header header = Header.ReadFrom(reader);
			header.WriteTo(this);
		}

		public void WriteAddress(MimeAddressReader reader)
		{
			this.AssertOpen();
			if (reader.IsGroup)
			{
				this.StartGroup(reader.DisplayName);
				MimeAddressReader groupRecipientReader = reader.GroupRecipientReader;
				while (groupRecipientReader.ReadNextAddress())
				{
					string displayName = groupRecipientReader.DisplayName;
					string email = groupRecipientReader.Email;
					if (displayName == null || email == null)
					{
						throw new ExchangeDataException(Strings.AddressReaderIsNotPositionedOnAddress);
					}
					this.WriteRecipient(displayName, email);
				}
				this.EndGroup();
				return;
			}
			this.WriteRecipient(reader.DisplayName, reader.Email);
		}

		public void WriteParameter(MimeParameterReader reader)
		{
			this.AssertOpen();
			this.WriteParameter(reader.Name, reader.Value);
		}

		public void WriteContent(MimeReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.AssertOpen();
			if (this.contentWritten)
			{
				throw new InvalidOperationException(Strings.ContentAlreadyWritten);
			}
			using (Stream rawContentReadStream = reader.GetRawContentReadStream())
			{
				if (rawContentReadStream != null)
				{
					using (Stream rawContentWriteStream = this.GetRawContentWriteStream())
					{
						DataStorage.CopyStreamToStream(rawContentReadStream, rawContentWriteStream, long.MaxValue, ref this.scratchBuffer);
					}
				}
			}
		}

		public void StartHeader(string name)
		{
			this.AssertOpen();
			this.WriteHeader(Header.Create(name));
		}

		public void StartHeader(HeaderId headerId)
		{
			this.AssertOpen();
			this.WriteHeader(Header.Create(headerId));
		}

		public void WriteHeaderValue(string value)
		{
			this.AssertOpen();
			if (this.headerValueWritten)
			{
				throw new InvalidOperationException(Strings.CannotWriteHeaderValueMoreThanOnce);
			}
			if (this.lastHeader == null)
			{
				throw new InvalidOperationException(Strings.CannotWriteHeaderValueHere);
			}
			this.headerValueWritten = true;
			if (value != null)
			{
				if (!(this.lastHeader is TextHeader))
				{
					byte[] rawValue = ByteString.StringToBytes(value, this.encodingOptions.AllowUTF8);
					this.lastHeader.RawValue = rawValue;
					return;
				}
				this.lastHeader.Value = value;
			}
		}

		public void WriteHeaderValue(DateTime value)
		{
			this.AssertOpen();
			if (this.headerValueWritten)
			{
				throw new InvalidOperationException(Strings.CannotWriteHeaderValueMoreThanOnce);
			}
			if (this.lastHeader == null)
			{
				throw new InvalidOperationException(Strings.CannotWriteHeaderValueHere);
			}
			this.headerValueWritten = true;
			TimeSpan timeZoneOffset = TimeSpan.Zero;
			DateTime utcDateTime = value.ToUniversalTime();
			if (value.Kind != DateTimeKind.Utc)
			{
				timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(value);
			}
			Header.WriteName(this.shimStream, this.lastHeader.Name, ref this.scratchBuffer);
			MimeStringLength mimeStringLength = new MimeStringLength(0);
			DateHeader.WriteDateHeaderValue(this.shimStream, utcDateTime, timeZoneOffset, ref mimeStringLength);
			this.lastHeader = null;
		}

		public void WriteContent(byte[] buffer, int offset, int count)
		{
			MimeCommon.CheckBufferArguments(buffer, offset, count);
			this.AssertOpen();
			if (this.contentWritten)
			{
				throw new InvalidOperationException(Strings.ContentAlreadyWritten);
			}
			if (this.encodedPartContent != null)
			{
				this.encodedPartContent.Write(buffer, offset, count);
				return;
			}
			using (Stream contentWriteStream = this.GetContentWriteStream())
			{
				contentWriteStream.Write(buffer, offset, count);
			}
		}

		public void WriteContent(Stream sourceStream)
		{
			if (sourceStream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.AssertOpen();
			if (this.contentWritten)
			{
				throw new InvalidOperationException(Strings.ContentAlreadyWritten);
			}
			Stream stream = this.encodedPartContent;
			Stream stream2 = null;
			try
			{
				if (stream == null)
				{
					stream2 = this.GetContentWriteStream();
					stream = stream2;
				}
				byte[] buffer = new byte[4096];
				int count;
				while (0 < (count = sourceStream.Read(buffer, 0, 4096)))
				{
					stream.Write(buffer, 0, count);
				}
			}
			finally
			{
				if (stream2 != null)
				{
					stream2.Dispose();
					stream2 = null;
				}
			}
		}

		public void WriteRawContent(byte[] buffer, int offset, int count)
		{
			MimeCommon.CheckBufferArguments(buffer, offset, count);
			this.AssertOpen();
			if (this.contentWritten)
			{
				throw new InvalidOperationException(Strings.ContentAlreadyWritten);
			}
			Stream rawContentWriteStream = this.partContent;
			if (rawContentWriteStream == null)
			{
				rawContentWriteStream = this.GetRawContentWriteStream();
			}
			rawContentWriteStream.Write(buffer, offset, count);
		}

		public void WriteRawContent(Stream sourceStream)
		{
			if (sourceStream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.AssertOpen();
			if (this.contentWritten)
			{
				throw new InvalidOperationException(Strings.ContentAlreadyWritten);
			}
			Stream rawContentWriteStream = this.partContent;
			if (rawContentWriteStream == null)
			{
				rawContentWriteStream = this.GetRawContentWriteStream();
			}
			byte[] buffer = new byte[4096];
			int count;
			while (0 < (count = sourceStream.Read(buffer, 0, 4096)))
			{
				rawContentWriteStream.Write(buffer, 0, count);
			}
		}

		public void EndContent()
		{
			this.AssertOpen();
			if (this.encodedPartContent != null)
			{
				this.encodedPartContent.Dispose();
				this.encodedPartContent = null;
				this.contentWritten = true;
				if (this.partContent != null)
				{
					this.partContent.Dispose();
					this.partContent = null;
				}
			}
		}

		public void Flush()
		{
			this.AssertOpen();
			if (this.state == MimeWriteState.Initial)
			{
				return;
			}
			this.FlushHeader();
			this.EndContent();
			this.FlushWriteQueue();
		}

		internal void WriteMimeNode(MimeNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			Header header = node as Header;
			if (header != null)
			{
				this.WriteHeader(header);
				this.FlushHeader();
				return;
			}
			MimePart mimePart = node as MimePart;
			if (mimePart != null)
			{
				this.StartPart();
				mimePart.WriteTo(this.shimStream, this.encodingOptions);
				this.EndPart();
				return;
			}
			HeaderList headerList = node as HeaderList;
			if (headerList != null)
			{
				foreach (Header header2 in headerList)
				{
					this.WriteHeader(header);
				}
				this.FlushHeader();
				return;
			}
			node = node.Clone();
			MimeRecipient mimeRecipient = node as MimeRecipient;
			if (mimeRecipient != null)
			{
				this.WriteRecipient(mimeRecipient);
				return;
			}
			MimeParameter mimeParameter = node as MimeParameter;
			if (mimeParameter != null)
			{
				this.WriteParameter(mimeParameter);
				return;
			}
			MimeGroup mimeGroup = node as MimeGroup;
			if (mimeGroup != null)
			{
				this.StartGroup(mimeGroup);
				this.EndGroup();
			}
		}

		public void WriteHeader(string name, string value)
		{
			this.AssertOpen();
			this.StartHeader(name);
			this.WriteHeaderValue(value);
			this.FlushHeader();
		}

		public void WriteHeader(HeaderId headerId, string value)
		{
			this.AssertOpen();
			this.StartHeader(headerId);
			this.WriteHeaderValue(value);
			this.FlushHeader();
		}

		public void WriteParameter(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.AssertOpen();
			this.WriteParameter(new MimeParameter(name, value));
		}

		public void StartGroup(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.AssertOpen();
			this.StartGroup(new MimeGroup(name));
		}

		public void EndGroup()
		{
			this.AssertOpen();
			MimeWriteState mimeWriteState = this.state;
			if (mimeWriteState != MimeWriteState.GroupRecipients)
			{
				throw new InvalidOperationException(Strings.CannotWriteGroupEndHere);
			}
			this.state = MimeWriteState.Recipients;
		}

		public void WriteRecipient(string displayName, string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			this.AssertOpen();
			this.WriteRecipient(new MimeRecipient(displayName, address));
		}

		public MimeWriter GetEmbeddedMessageWriter()
		{
			this.AssertOpen();
			if (this.contentWritten)
			{
				throw new InvalidOperationException(Strings.ContentAlreadyWritten);
			}
			MimeWriteState mimeWriteState = this.state;
			switch (mimeWriteState)
			{
			case MimeWriteState.Initial:
			case MimeWriteState.Complete:
				break;
			default:
				switch (mimeWriteState)
				{
				case MimeWriteState.PartContent:
				case MimeWriteState.EndPart:
					break;
				default:
					return new MimeWriter(this.GetRawContentWriteStream())
					{
						embeddedDepth = this.embeddedDepth + 1
					};
				}
				break;
			}
			throw new InvalidOperationException(Strings.CannotWritePartContentNow);
		}

		public Stream GetRawContentWriteStream()
		{
			this.AssertOpen();
			if (this.contentWritten)
			{
				throw new InvalidOperationException(Strings.ContentAlreadyWritten);
			}
			switch (this.state)
			{
			case MimeWriteState.Initial:
			case MimeWriteState.Complete:
			case MimeWriteState.EndPart:
				throw new InvalidOperationException(Strings.CannotWritePartContentNow);
			case MimeWriteState.StartPart:
			case MimeWriteState.Headers:
			case MimeWriteState.Parameters:
			case MimeWriteState.Recipients:
			case MimeWriteState.GroupRecipients:
				this.FlushHeader();
				if (!this.foundMimeVersion)
				{
					if (this.forceMime && this.partDepth == 1)
					{
						this.WriteMimeVersion();
					}
					else
					{
						this.currentPart.IsMultipart = false;
					}
				}
				if (MimeWriteState.StartPart != this.state)
				{
					this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
				}
				break;
			case MimeWriteState.PartContent:
				return this.partContent;
			}
			if (this.currentPart.IsMultipart)
			{
				throw new InvalidOperationException(Strings.MultipartCannotContainContent);
			}
			this.state = MimeWriteState.PartContent;
			this.partContent = new MimeWriter.WriterContentStream(this);
			return this.partContent;
		}

		public Stream GetContentWriteStream()
		{
			this.AssertOpen();
			if (this.contentWritten)
			{
				throw new InvalidOperationException(Strings.ContentAlreadyWritten);
			}
			if (this.partContent != null)
			{
				throw new InvalidOperationException(Strings.PartContentIsBeingWritten);
			}
			Stream rawContentWriteStream = this.GetRawContentWriteStream();
			if (this.contentTransferEncoding == ContentTransferEncoding.SevenBit || this.contentTransferEncoding == ContentTransferEncoding.EightBit || this.contentTransferEncoding == ContentTransferEncoding.Binary)
			{
				return rawContentWriteStream;
			}
			if (this.contentTransferEncoding == ContentTransferEncoding.BinHex)
			{
				throw new NotSupportedException(Strings.BinHexNotSupportedForThisMethod);
			}
			ByteEncoder byteEncoder = MimePart.CreateEncoder(null, this.contentTransferEncoding);
			if (byteEncoder == null)
			{
				throw new NotSupportedException(Strings.UnrecognizedTransferEncodingUsed);
			}
			this.encodedPartContent = new EncoderStream(rawContentWriteStream, byteEncoder, EncoderStreamAccess.Write);
			return new SuppressCloseStream(this.encodedPartContent);
		}

		public void StartPart()
		{
			this.AssertOpen();
			MimeWriteState mimeWriteState = this.state;
			if (mimeWriteState == MimeWriteState.Complete || mimeWriteState == MimeWriteState.PartContent)
			{
				throw new InvalidOperationException(Strings.CannotStartPartHere);
			}
			if (this.partDepth != 0)
			{
				this.FlushHeader();
				if (!this.currentPart.IsMultipart)
				{
					throw new InvalidOperationException(Strings.NonMultiPartPartsCannotHaveChildren);
				}
				if (!this.foundMimeVersion && this.forceMime && this.partDepth == 1)
				{
					this.WriteMimeVersion();
				}
				this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
				this.WriteBoundary(this.currentPart.Boundary, false);
			}
			MimeWriter.PartData partData = default(MimeWriter.PartData);
			this.PushPart(ref partData);
			this.state = MimeWriteState.StartPart;
			this.contentWritten = false;
			this.contentTransferEncoding = ContentTransferEncoding.SevenBit;
		}

		public void EndPart()
		{
			this.AssertOpen();
			switch (this.state)
			{
			case MimeWriteState.Initial:
			case MimeWriteState.Complete:
				throw new InvalidOperationException(Strings.CannotEndPartHere);
			case MimeWriteState.StartPart:
			case MimeWriteState.Headers:
			case MimeWriteState.Parameters:
			case MimeWriteState.Recipients:
			case MimeWriteState.GroupRecipients:
				this.FlushHeader();
				if (!this.foundMimeVersion)
				{
					if (this.forceMime && this.partDepth == 1)
					{
						this.WriteMimeVersion();
					}
					else
					{
						this.currentPart.IsMultipart = false;
					}
				}
				this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
				break;
			case MimeWriteState.PartContent:
				if (this.encodedPartContent != null)
				{
					this.encodedPartContent.Dispose();
					this.encodedPartContent = null;
				}
				if (this.partContent != null)
				{
					this.partContent.Dispose();
					this.partContent = null;
				}
				this.contentWritten = true;
				break;
			}
			this.state = MimeWriteState.EndPart;
			if (this.currentPart.IsMultipart)
			{
				this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
				this.WriteBoundary(this.currentPart.Boundary, true);
			}
			this.PopPart();
			if (this.partDepth == 0)
			{
				this.state = MimeWriteState.Complete;
			}
		}

		internal void Write(byte[] data, int offset, int count)
		{
			if (0 >= count)
			{
				return;
			}
			this.QueueWrite(data, offset, count);
		}

		internal void QueueWrite(byte[] data, int offset, int count)
		{
			this.bytesWritten += count;
			this.lineTermination = MimeCommon.AdvanceLineTerminationState(this.lineTermination, data, offset, count);
			int num = (this.writeCount == 1) ? (this.currentWrite.Length - this.currentWrite.Count) : ((this.writeCount == 0) ? MimeWriter.QueuedWrite.QueuedWriteSize : 0);
			if (num >= count)
			{
				if (this.writeCount == 0)
				{
					MimeWriter.QueuedWrite queuedWrite = default(MimeWriter.QueuedWrite);
					this.PushWrite(ref queuedWrite);
				}
				this.currentWrite.Append(data, offset, count);
				return;
			}
			MimeWriter.QueuedWrite queuedWrite2 = default(MimeWriter.QueuedWrite);
			if (count < MimeWriter.QueuedWrite.QueuedWriteSize && this.writeCount > 0)
			{
				queuedWrite2 = this.currentWrite;
			}
			this.FlushWriteQueue();
			if (count < MimeWriter.QueuedWrite.QueuedWriteSize && queuedWrite2.Length > 0)
			{
				queuedWrite2.Reset();
				queuedWrite2.Append(data, offset, count);
				this.PushWrite(ref queuedWrite2);
				return;
			}
			this.data.Write(data, offset, count);
		}

		private void WriteHeader(Header header)
		{
			MimeWriteState mimeWriteState = this.state;
			switch (mimeWriteState)
			{
			case MimeWriteState.Initial:
			case MimeWriteState.Complete:
				break;
			default:
				switch (mimeWriteState)
				{
				case MimeWriteState.PartContent:
				case MimeWriteState.EndPart:
					break;
				default:
					this.state = MimeWriteState.Headers;
					this.FlushHeader();
					this.lastHeader = header;
					return;
				}
				break;
			}
			throw new InvalidOperationException(Strings.CannotWriteHeadersHere);
		}

		private void WriteParameter(MimeParameter parameter)
		{
			if (this.lastHeader == null || !(this.lastHeader is ComplexHeader))
			{
				throw new InvalidOperationException(Strings.CannotWriteParametersOnThisHeader);
			}
			switch (this.state)
			{
			case MimeWriteState.Complete:
			case MimeWriteState.StartPart:
			case MimeWriteState.Recipients:
			case MimeWriteState.PartContent:
			case MimeWriteState.EndPart:
				throw new InvalidOperationException(Strings.CannotWriteParametersHere);
			}
			this.state = MimeWriteState.Parameters;
			ContentTypeHeader contentTypeHeader = this.lastHeader as ContentTypeHeader;
			if (contentTypeHeader != null && contentTypeHeader.IsMultipart && parameter.Name == "boundary")
			{
				string value = parameter.Value;
				if (value.Length == 0)
				{
					throw new ArgumentException(Strings.CannotWriteEmptyOrNullBoundary);
				}
				this.currentPart.Boundary = ByteString.StringToBytes(value, false);
			}
			this.lastHeader.InternalAppendChild(parameter);
		}

		private void WriteRecipient(MimeRecipient recipient)
		{
			if (this.lastHeader == null || !(this.lastHeader is AddressHeader))
			{
				throw new InvalidOperationException(Strings.CannotWriteRecipientsHere);
			}
			MimeNode lastChild;
			switch (this.state)
			{
			case MimeWriteState.Complete:
			case MimeWriteState.StartPart:
			case MimeWriteState.PartContent:
			case MimeWriteState.EndPart:
				throw new InvalidOperationException(Strings.CannotWriteRecipientsHere);
			case MimeWriteState.GroupRecipients:
				lastChild = this.lastHeader.LastChild;
				goto IL_78;
			}
			this.state = MimeWriteState.Recipients;
			lastChild = this.lastHeader;
			IL_78:
			lastChild.InternalAppendChild(recipient);
		}

		private void StartGroup(MimeGroup group)
		{
			MimeWriteState mimeWriteState = this.state;
			switch (mimeWriteState)
			{
			case MimeWriteState.Complete:
			case MimeWriteState.StartPart:
				break;
			default:
				switch (mimeWriteState)
				{
				case MimeWriteState.PartContent:
				case MimeWriteState.EndPart:
					break;
				default:
					if (this.lastHeader == null || !(this.lastHeader is AddressHeader))
					{
						throw new InvalidOperationException(Strings.CannotWriteGroupStartHere);
					}
					this.state = MimeWriteState.GroupRecipients;
					this.lastHeader.InternalAppendChild(group);
					return;
				}
				break;
			}
			throw new InvalidOperationException(Strings.CannotWriteGroupStartHere);
		}

		private void FlushHeader()
		{
			this.headerValueWritten = false;
			if (this.lastHeader != null)
			{
				if (this.lastHeader.HeaderId == HeaderId.MimeVersion && this.partDepth == 1)
				{
					this.foundMimeVersion = true;
				}
				else if (this.lastHeader.HeaderId == HeaderId.ContentTransferEncoding)
				{
					string value = this.lastHeader.Value;
					if (value != null)
					{
						this.contentTransferEncoding = MimePart.GetEncodingType(new MimeString(value));
					}
				}
				else if (this.lastHeader.HeaderId == HeaderId.ContentType)
				{
					ContentTypeHeader contentTypeHeader = this.lastHeader as ContentTypeHeader;
					if (contentTypeHeader.IsMultipart)
					{
						this.currentPart.IsMultipart = true;
						MimeParameter mimeParameter = contentTypeHeader["boundary"];
						this.currentPart.Boundary = mimeParameter.RawValue;
					}
					else
					{
						this.currentPart.IsMultipart = false;
					}
					this.currentPart.HasContentType = true;
				}
				this.lastHeader.WriteTo(this.shimStream, this.encodingOptions);
				this.lastHeader = null;
			}
		}

		private void WriteMimeVersion()
		{
			this.foundMimeVersion = true;
			this.QueueWrite(MimeString.MimeVersion, 0, MimeString.MimeVersion.Length);
			this.state = MimeWriteState.Headers;
		}

		private void FlushWriteQueue()
		{
			if (this.writeCount != 0)
			{
				this.writeQueue[this.writeCount - 1] = this.currentWrite;
			}
			for (int i = 0; i < this.writeCount; i++)
			{
				this.data.Write(this.writeQueue[i].Data, this.writeQueue[i].Offset, this.writeQueue[i].Count);
				this.writeQueue[i] = default(MimeWriter.QueuedWrite);
			}
			this.writeCount = 0;
		}

		private void WriteBoundary(byte[] boundary, bool final)
		{
			byte[] array;
			if (final)
			{
				array = MimeWriter.terminateBoundarySuffix;
			}
			else
			{
				array = MimeWriter.endBoundarySuffix;
			}
			this.Write(MimeWriter.boundaryPrefix, 0, MimeWriter.boundaryPrefix.Length);
			this.Write(boundary, 0, boundary.Length);
			this.Write(array, 0, array.Length);
		}

		private void PushPart(ref MimeWriter.PartData part)
		{
			if (this.partStack == null)
			{
				this.partStack = new MimeWriter.PartData[8];
				this.partDepth = 0;
			}
			else if (this.partStack.Length == this.partDepth)
			{
				MimeWriter.PartData[] destinationArray = new MimeWriter.PartData[this.partStack.Length * 2];
				Array.Copy(this.partStack, 0, destinationArray, 0, this.partStack.Length);
				for (int i = 0; i < this.partDepth; i++)
				{
					this.partStack[i] = default(MimeWriter.PartData);
				}
				this.partStack = destinationArray;
			}
			if (this.partDepth != 0)
			{
				this.partStack[this.partDepth - 1] = this.currentPart;
			}
			this.partStack[this.partDepth++] = part;
			this.currentPart = part;
		}

		private void PopPart()
		{
			this.partDepth--;
			this.partStack[this.partDepth] = default(MimeWriter.PartData);
			this.currentPart = this.partStack[(this.partDepth > 0) ? (this.partDepth - 1) : 0];
		}

		private void AssertOpen()
		{
			if (this.data == null)
			{
				throw new ObjectDisposedException("MimeWriter");
			}
		}

		private void PushWrite(ref MimeWriter.QueuedWrite write)
		{
			if (this.writeQueue == null)
			{
				this.writeQueue = new MimeWriter.QueuedWrite[16];
				this.writeCount = 0;
			}
			else if (this.writeQueue.Length == this.writeCount)
			{
				MimeWriter.QueuedWrite[] destinationArray = new MimeWriter.QueuedWrite[this.writeQueue.Length * 2];
				Array.Copy(this.writeQueue, 0, destinationArray, 0, this.writeQueue.Length);
				for (int i = 0; i < this.writeQueue.Length; i++)
				{
					this.writeQueue[i] = default(MimeWriter.QueuedWrite);
				}
				this.writeQueue = destinationArray;
			}
			if (this.writeCount != 0)
			{
				this.writeQueue[this.writeCount - 1] = this.currentWrite;
			}
			this.writeQueue[this.writeCount++] = write;
			this.currentWrite = write;
		}

		private static readonly byte[] terminateBoundarySuffix = new byte[]
		{
			45,
			45,
			13,
			10
		};

		private static readonly byte[] endBoundarySuffix = MimeString.CrLf;

		private static readonly byte[] boundaryPrefix = new byte[]
		{
			45,
			45
		};

		private MimeWriteState state;

		private Stream data;

		private Header lastHeader;

		private MimeWriter.WriterContentStream partContent;

		private Stream encodedPartContent;

		private ContentTransferEncoding contentTransferEncoding = ContentTransferEncoding.SevenBit;

		private MimeWriter.PartData[] partStack;

		private int partDepth;

		private int embeddedDepth;

		private MimeWriter.QueuedWrite[] writeQueue;

		private int writeCount;

		private EncodingOptions encodingOptions;

		private int bytesWritten;

		private bool forceMime = true;

		private bool headerValueWritten;

		private bool contentWritten;

		private MimeWriter.PartData currentPart;

		private MimeWriter.QueuedWrite currentWrite;

		private bool foundMimeVersion;

		private MimeWriter.WriterQueueStream shimStream;

		private byte[] scratchBuffer;

		private LineTerminationState lineTermination;

		private struct PartData
		{
			public bool IsMultipart
			{
				get
				{
					return this.multipartPart;
				}
				set
				{
					this.multipartPart = value;
				}
			}

			public bool HasContentType
			{
				get
				{
					return this.contentType;
				}
				set
				{
					this.contentType = value;
				}
			}

			public byte[] Boundary
			{
				get
				{
					return this.boundary;
				}
				set
				{
					this.boundary = value;
				}
			}

			private byte[] boundary;

			private bool contentType;

			private bool multipartPart;
		}

		private struct QueuedWrite
		{
			public int Length
			{
				get
				{
					return this.data.Length;
				}
			}

			public byte[] Data
			{
				get
				{
					return this.data;
				}
			}

			public int Offset
			{
				get
				{
					return this.offset;
				}
			}

			public int Count
			{
				get
				{
					return this.count;
				}
			}

			public bool Full
			{
				get
				{
					return this.data != null && this.Count == this.data.Length;
				}
			}

			public void Reset()
			{
				this.count = 0;
				this.offset = 0;
			}

			public int Append(byte[] buffer, int offset, int count)
			{
				if (this.Full)
				{
					return 0;
				}
				if (this.data == null)
				{
					this.data = new byte[MimeWriter.QueuedWrite.QueuedWriteSize];
				}
				int num = Math.Min(count, this.data.Length - this.Count);
				Buffer.BlockCopy(buffer, offset, this.data, this.Count, num);
				this.count += num;
				return num;
			}

			public static int QueuedWriteSize = 4096;

			private byte[] data;

			private int offset;

			private int count;
		}

		internal class WriterQueueStream : Stream
		{
			public WriterQueueStream(MimeWriter writer)
			{
				this.writer = writer;
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
					return true;
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

			public override void Write(byte[] buffer, int offset, int count)
			{
				if (count > 0)
				{
					this.writer.QueueWrite(buffer, offset, count);
				}
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

			private MimeWriter writer;
		}

		private class WriterContentStream : Stream
		{
			public WriterContentStream(MimeWriter writer)
			{
				this.writer = writer;
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
					return true;
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

			public override void Write(byte[] buffer, int offset, int count)
			{
				MimeCommon.CheckBufferArguments(buffer, offset, count);
				if (this.writer.contentWritten)
				{
					throw new InvalidOperationException(Strings.ContentAlreadyWritten);
				}
				this.writer.Write(buffer, offset, count);
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void Flush()
			{
				if (this.writer.contentWritten)
				{
					throw new InvalidOperationException(Strings.ContentAlreadyWritten);
				}
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			private MimeWriter writer;
		}
	}
}
