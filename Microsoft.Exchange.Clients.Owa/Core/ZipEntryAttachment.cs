using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ZipEntryAttachment
	{
		internal ZipEntryAttachment(string fileName, AttachmentWellInfo attachmentWellInfo, bool doNeedToFilterHtml, bool doNotSniff, bool isNotHtmlandNotXml)
		{
			this.fileName = fileName;
			this.encodedfileNameBytes = this.GetEncodedString(fileName);
			this.attachmentWellInfo = attachmentWellInfo;
			this.doNeedToFilterHtml = doNeedToFilterHtml;
			this.doNotSniff = doNotSniff;
			this.isNotHtmlandNotXml = isNotHtmlandNotXml;
			if (this.NeedsCompression(attachmentWellInfo))
			{
				this.compressionMethod = 8;
				this.generalPurposeFlag = 2056;
			}
			else
			{
				this.compressionMethod = 0;
				this.generalPurposeFlag = 2048;
			}
			this.currentDateTime = this.CurrentDosDateTime();
		}

		internal long WriteToStream(Stream outputStream, HttpContext httpContext, BlockStatus blockStatus, long outputStreamPosition, IList<string> fileNames)
		{
			this.fileNames = fileNames;
			this.headerOffset = outputStreamPosition;
			Stream stream = null;
			using (Attachment attachment = this.attachmentWellInfo.OpenAttachment())
			{
				try
				{
					if (attachment.AttachmentType == AttachmentType.EmbeddedMessage)
					{
						stream = this.GetItemAttachmentStream(attachment, httpContext);
					}
					else
					{
						stream = this.GetStreamAttachmentStream(attachment);
					}
					if (stream.Length > 0L)
					{
						if (this.doNeedToFilterHtml)
						{
							if (this.CompressionMethod != 0)
							{
								stream = this.GetfilteredStreamForCompression(httpContext, stream, this.attachmentWellInfo.TextCharset, blockStatus);
							}
							else
							{
								stream = this.GetfilteredStream(httpContext, stream, this.attachmentWellInfo.TextCharset, blockStatus);
							}
						}
						else
						{
							stream = this.GetUnfilteredStream(httpContext, stream);
						}
					}
					if (this.CompressionMethod == 0)
					{
						if (!stream.CanSeek)
						{
							throw new OwaInvalidInputException("Stream is required to support Seek operations, and does not");
						}
						this.attachmentSize = stream.Length;
					}
					this.WriteZipFileHeader(stream, outputStream);
					this.WriteFileData(stream, outputStream, httpContext, blockStatus);
					if (this.CompressionMethod != 0)
					{
						this.WriteZipFileDescriptor(outputStream);
					}
				}
				finally
				{
					if (stream != null)
					{
						stream.Close();
						stream = null;
					}
				}
			}
			return (long)((ulong)(this.headerBytesWritten + this.attachmentBytesWritten + this.descriptorBytesWritten));
		}

		private void WriteZipFileHeader(Stream inputStream, Stream outputStream)
		{
			ByteBuffer byteBuffer = new ByteBuffer(30);
			byteBuffer.WriteUInt32(67324752U);
			byteBuffer.WriteUInt16(20);
			byteBuffer.WriteUInt16(this.GeneralPurposeBitFlag);
			byteBuffer.WriteUInt16(this.CompressionMethod);
			byteBuffer.WriteUInt32(this.CurrentDateTime);
			if (this.CompressionMethod != 0)
			{
				byteBuffer.WriteUInt32(0U);
				byteBuffer.WriteUInt32(0U);
				byteBuffer.WriteUInt32(0U);
			}
			else
			{
				this.ComputeCrc32(inputStream);
				byteBuffer.WriteUInt32(this.CheckSum);
				byteBuffer.WriteUInt32((uint)this.attachmentSize);
				byteBuffer.WriteUInt32((uint)this.attachmentSize);
			}
			byteBuffer.WriteUInt16((ushort)this.encodedfileNameBytes.Length);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteContentsTo(outputStream);
			outputStream.Write(this.encodedfileNameBytes, 0, this.encodedfileNameBytes.Length);
			this.headerBytesWritten = (uint)(byteBuffer.Length + this.encodedfileNameBytes.Length);
		}

		private byte[] GetEncodedString(string stringToEncode)
		{
			return Encoding.UTF8.GetBytes(stringToEncode);
		}

		private bool NeedsCompression(AttachmentWellInfo attachmentWellInfo)
		{
			return !ZipEntryAttachment.alreadyCompressedRegex.IsMatch(attachmentWellInfo.FileExtension) && attachmentWellInfo.Size >= 1000L;
		}

		private void ComputeCrc32(Stream stream)
		{
			uint num = AttachmentUtility.ComputeCrc32FromStream(stream);
			this.checkSum = num;
		}

		private void WriteFileData(Stream inputStream, Stream outputStream, HttpContext httpContext, BlockStatus blockStatus)
		{
			uint[] array = new uint[3];
			if (this.CompressionMethod == 8)
			{
				array = AttachmentUtility.CompressAndWriteOutputStream(httpContext.Response.OutputStream, inputStream, true);
				this.attachmentBytesWritten = array[0];
				this.checkSum = array[1];
				this.attachmentSize = (long)((ulong)array[2]);
				return;
			}
			this.attachmentBytesWritten = AttachmentUtility.WriteOutputStream(httpContext.Response.OutputStream, inputStream);
		}

		internal Stream GetStreamAttachmentStream(Attachment attachment)
		{
			StreamAttachmentBase streamAttachmentBase = attachment as StreamAttachmentBase;
			if (streamAttachmentBase == null)
			{
				throw new OwaInvalidRequestException("Attachment is not a stream attachment");
			}
			OleAttachment oleAttachment = streamAttachmentBase as OleAttachment;
			Stream stream;
			if (oleAttachment != null)
			{
				stream = oleAttachment.TryConvertToImage(ImageFormat.Jpeg);
				if (stream == null)
				{
					stream = new MemoryStream();
				}
			}
			else
			{
				stream = streamAttachmentBase.GetContentStream(PropertyOpenMode.ReadOnly);
			}
			return stream;
		}

		internal Stream GetItemAttachmentStream(Attachment attachment, HttpContext httpContext)
		{
			OwaContext owaContext = OwaContext.Get(httpContext);
			UserContext userContext = owaContext.UserContext;
			OutboundConversionOptions outboundConversionOptions = Utilities.CreateOutboundConversionOptions(userContext);
			Stream stream = Streams.CreateTemporaryStorageStream();
			string text = null;
			ItemAttachment itemAttachment = attachment as ItemAttachment;
			using (Item item = itemAttachment.GetItem(StoreObjectSchema.ContentConversionProperties))
			{
				try
				{
					if (ItemConversion.IsItemClassConvertibleToMime(item.ClassName))
					{
						ItemConversion.ConvertItemToMime(item, stream, outboundConversionOptions);
						text = ".eml";
					}
					else if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(item.ClassName))
					{
						(item as CalendarItemBase).ExportAsICAL(stream, "UTF-8", outboundConversionOptions);
						text = ".ics";
					}
					else if (ObjectClass.IsContact(item.ClassName))
					{
						Contact.ExportVCard(item as Contact, stream, outboundConversionOptions);
						text = ".vcf";
					}
					else
					{
						ItemConversion.ConvertItemToMsgStorage(item, stream, outboundConversionOptions);
					}
				}
				catch (Exception)
				{
					stream = this.GetContentsReplacementStream(-1706159495);
					text = ".txt";
				}
			}
			if (text != null)
			{
				this.fileName = this.GetConvertedItemFileName(this.fileName, text);
				this.encodedfileNameBytes = this.GetEncodedString(this.fileName);
			}
			stream.Position = 0L;
			return stream;
		}

		private string GetConvertedItemFileName(string fileName, string newfileExtension)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			int num = 1;
			string text = fileNameWithoutExtension + newfileExtension;
			while (this.fileNames.Contains(text))
			{
				text = string.Concat(new string[]
				{
					fileNameWithoutExtension,
					"[",
					num.ToString(),
					"]",
					newfileExtension
				});
				num++;
			}
			return text;
		}

		private void WriteZipFileDescriptor(Stream outputStream)
		{
			ByteBuffer byteBuffer = new ByteBuffer(16);
			byteBuffer.WriteUInt32(134695760U);
			byteBuffer.WriteUInt32(this.CheckSum);
			byteBuffer.WriteUInt32(this.attachmentBytesWritten);
			byteBuffer.WriteUInt32((uint)this.attachmentSize);
			byteBuffer.WriteContentsTo(outputStream);
			this.descriptorBytesWritten = (uint)byteBuffer.Length;
		}

		internal uint WriteCentralDirectoryStructure(Stream outputStream)
		{
			ByteBuffer byteBuffer = new ByteBuffer(46);
			byteBuffer.WriteUInt32(33639248U);
			byteBuffer.WriteUInt16(20);
			byteBuffer.WriteUInt16(20);
			byteBuffer.WriteUInt16(this.GeneralPurposeBitFlag);
			byteBuffer.WriteUInt16(this.CompressionMethod);
			byteBuffer.WriteUInt32(this.CurrentDateTime);
			byteBuffer.WriteUInt32(this.CheckSum);
			byteBuffer.WriteUInt32(this.attachmentBytesWritten);
			byteBuffer.WriteUInt32((uint)this.attachmentSize);
			byteBuffer.WriteUInt16((ushort)this.encodedfileNameBytes.Length);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt32(0U);
			byteBuffer.WriteUInt32((uint)this.headerOffset);
			byteBuffer.WriteContentsTo(outputStream);
			outputStream.Write(this.encodedfileNameBytes, 0, this.encodedfileNameBytes.Length);
			this.directoryBytesWritten = (uint)(byteBuffer.Length + this.encodedfileNameBytes.Length);
			return this.directoryBytesWritten;
		}

		public Stream GetfilteredStreamForCompression(HttpContext httpContext, Stream stream, Charset charset, BlockStatus blockStatus)
		{
			return AttachmentUtility.GetFilteredStream(httpContext, stream, charset, blockStatus);
		}

		internal Stream GetfilteredStream(HttpContext httpContext, Stream inputStream, Charset charset, BlockStatus blockStatus)
		{
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(AttachmentUtility.CopyBufferSize);
			byte[] array = bufferPool.Acquire();
			Stream stream = new MemoryStream();
			try
			{
				using (Stream filteredStream = AttachmentUtility.GetFilteredStream(httpContext, inputStream, charset, blockStatus))
				{
					try
					{
						int count;
						while ((count = filteredStream.Read(array, 0, array.Length)) > 0)
						{
							stream.Write(array, 0, count);
						}
					}
					finally
					{
						if (array != null)
						{
							bufferPool.Release(array);
						}
					}
				}
				stream.Seek(0L, SeekOrigin.Begin);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceError<string>((long)this.GetHashCode(), "ZipEntryAttachment.GetfilteredStream: Safe HTML converter failed with exception {0}", ex.Message);
				stream = new MemoryStream();
			}
			return stream;
		}

		public Stream GetUnfilteredStream(HttpContext httpContext, Stream stream)
		{
			if (!stream.CanSeek)
			{
				throw new OwaInvalidInputException("Stream is required to support Seek operations, and does not");
			}
			if (!this.doNotSniff && this.isNotHtmlandNotXml && AttachmentUtility.CheckShouldRemoveContents(stream))
			{
				if (ExTraceGlobals.AttachmentHandlingTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.AttachmentHandlingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ZipEntryAttachment.GetUnfilteredStream: Return contents removed for attachment {0}: mailbox {1}", this.fileName, AttachmentUtility.TryGetMailboxIdentityName(httpContext));
				}
				return this.GetContentsReplacementStream(-1868113279);
			}
			if (ExTraceGlobals.AttachmentHandlingTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ZipEntryAttachment.GetUnfilteredStream: Return original contents for attachment {0}: mailbox {1}", this.fileName, AttachmentUtility.TryGetMailboxIdentityName(httpContext));
			}
			return stream;
		}

		private Stream GetContentsReplacementStream(Strings.IDs resource)
		{
			byte[] encodedString = this.GetEncodedString(LocalizedStrings.GetNonEncoded(resource));
			return new MemoryStream(encodedString, 0, encodedString.Length);
		}

		public AttachmentWellInfo AttachmentWellInfo
		{
			get
			{
				return this.attachmentWellInfo;
			}
		}

		private ushort CompressionMethod
		{
			get
			{
				return this.compressionMethod;
			}
			set
			{
				this.compressionMethod = value;
			}
		}

		private ushort GeneralPurposeBitFlag
		{
			get
			{
				return this.generalPurposeFlag;
			}
			set
			{
				this.generalPurposeFlag = value;
			}
		}

		private uint CheckSum
		{
			get
			{
				return this.checkSum;
			}
			set
			{
				this.checkSum = value;
			}
		}

		private uint CurrentDateTime
		{
			get
			{
				return this.currentDateTime;
			}
			set
			{
				this.currentDateTime = value;
			}
		}

		private uint CurrentDosDateTime()
		{
			ExDateTime localTime = DateTimeUtilities.GetLocalTime();
			int num = localTime.Year - 1980 & 127;
			num = (num << 4) + localTime.Month;
			num = (num << 5) + localTime.Day;
			num = (num << 5) + localTime.Hour;
			num = (num << 6) + localTime.Minute;
			return (uint)((num << 5) + localTime.Second / 2);
		}

		private const int MinimumSizeForCompression = 1000;

		private const ushort VersionNeededToExtract = 20;

		private const ushort GeneralPurposeBitFlagDescriptor = 8;

		private const ushort GeneralPurposeBitFlagUtf8Encoded = 2048;

		private const ushort CompressionMethodStore = 0;

		private const ushort CompressionMethodDeflate = 8;

		private const ushort ExtraFieldLength = 0;

		private const string CompressedExts = "(?i)^\\.(mp3|png|docx|xlsx|pptx|jpg|zip|pdf|gif|mpg|aac|wma|wmv|mov)$";

		private static Regex alreadyCompressedRegex = new Regex("(?i)^\\.(mp3|png|docx|xlsx|pptx|jpg|zip|pdf|gif|mpg|aac|wma|wmv|mov)$");

		private string fileName;

		private long headerOffset;

		private uint headerBytesWritten;

		private uint directoryBytesWritten;

		private uint attachmentBytesWritten;

		private uint descriptorBytesWritten;

		private AttachmentWellInfo attachmentWellInfo;

		private bool doNeedToFilterHtml;

		private bool doNotSniff;

		private bool isNotHtmlandNotXml;

		private long attachmentSize;

		private uint checkSum;

		private ushort compressionMethod;

		private ushort generalPurposeFlag;

		private uint currentDateTime;

		private IList<string> fileNames;

		private byte[] encodedfileNameBytes;
	}
}
