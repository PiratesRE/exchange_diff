using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Common.Sniff;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class ZipEntryAttachment
	{
		internal ZipEntryAttachment(string fileName, Attachment attachment, bool doNeedToFilterHtml, bool doNotSniff, bool isNotHtmlandNotXml)
		{
			this.fileName = fileName;
			this.encodedfileNameBytes = ZipEntryAttachment.GetEncodedString(fileName);
			this.attachmentId = attachment.Id;
			this.doNeedToFilterHtml = doNeedToFilterHtml;
			this.doNotSniff = doNotSniff;
			this.isNotHtmlandNotXml = isNotHtmlandNotXml;
			if (this.NeedsCompression(attachment))
			{
				this.CompressionMethod = 8;
				this.GeneralPurposeBitFlag = 2056;
			}
			else
			{
				this.CompressionMethod = 0;
				this.GeneralPurposeBitFlag = 2048;
			}
			this.CurrentDateTime = this.CurrentDosDateTime();
		}

		internal long WriteToStream(ConfigurationContextBase configurationContext, Stream outputStream, OutboundConversionOptions outboundConversionOptions, BlockStatus blockStatus, long outputStreamPosition, IList<string> fileNames, AttachmentCollection attachmentCollection)
		{
			this.headerOffset = outputStreamPosition;
			Stream stream = null;
			uint num = 0U;
			using (Attachment attachment = attachmentCollection.Open(this.attachmentId))
			{
				try
				{
					if (attachment.AttachmentType == AttachmentType.EmbeddedMessage)
					{
						this.fileNames = fileNames;
						stream = this.GetItemAttachmentStream(attachment, outboundConversionOptions);
					}
					else
					{
						this.fileNames = null;
						stream = ZipEntryAttachment.GetStreamAttachmentStream(attachment);
					}
					if (stream.Length > 0L)
					{
						Stream stream2;
						if (this.doNeedToFilterHtml)
						{
							if (this.CompressionMethod != 0)
							{
								stream2 = AttachmentUtilities.GetFilteredStream(configurationContext, stream, attachment.TextCharset, blockStatus);
							}
							else
							{
								stream2 = this.GetFilteredResponseStream(configurationContext, stream, attachment.TextCharset, blockStatus);
							}
						}
						else
						{
							stream2 = this.GetUnfilteredStream(stream);
						}
						if (stream2 != stream)
						{
							stream.Close();
							stream = stream2;
						}
					}
					if (this.CompressionMethod == 0)
					{
						if (!stream.CanSeek)
						{
							throw new ArgumentException("stream", "Stream is required to support Seek operations, and does not");
						}
						this.attachmentSize = stream.Length;
					}
					this.WriteZipFileHeader(stream, outputStream);
					this.WriteFileData(stream, outputStream, blockStatus);
					if (this.CompressionMethod != 0)
					{
						num = this.WriteZipFileDescriptor(outputStream);
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
			return (long)((ulong)(this.headerBytesWritten + this.attachmentBytesWritten + num));
		}

		internal void WriteZipFileHeader(Stream inputStream, Stream outputStream)
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

		private static bool CheckShouldRemoveContents(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanSeek)
			{
				throw new ArgumentException("Stream is required to support Seek operations, and does not");
			}
			byte[] array = new byte[512];
			int bytesToRead = stream.Read(array, 0, 512);
			bool result = ZipEntryAttachment.CheckShouldRemoveContents(array, bytesToRead);
			stream.Seek(0L, SeekOrigin.Begin);
			return result;
		}

		private static bool CheckShouldRemoveContents(byte[] bytesToSniff, int bytesToRead)
		{
			bool result;
			using (MemoryStream memoryStream = new MemoryStream(bytesToSniff, 0, bytesToRead))
			{
				DataSniff dataSniff = new DataSniff(256);
				string x = dataSniff.FindMimeFromData(memoryStream);
				result = (StringComparer.OrdinalIgnoreCase.Compare(x, "text/xml") == 0 || 0 == StringComparer.OrdinalIgnoreCase.Compare(x, "text/html"));
			}
			return result;
		}

		private static Stream GetContentsReplacementStream(Strings.IDs resource)
		{
			byte[] encodedString = ZipEntryAttachment.GetEncodedString(Strings.GetLocalizedString(resource));
			return new MemoryStream(encodedString, 0, encodedString.Length);
		}

		private static byte[] GetEncodedString(string stringToEncode)
		{
			return Encoding.UTF8.GetBytes(stringToEncode);
		}

		private bool NeedsCompression(Attachment attachment)
		{
			return !ZipEntryAttachment.alreadyCompressedRegex.IsMatch(attachment.FileExtension) && attachment.Size >= 1000L;
		}

		private void ComputeCrc32(Stream stream)
		{
			uint num = ZipEntryAttachment.ComputeCrc32FromStream(stream);
			this.CheckSum = num;
		}

		private static uint ComputeCrc32FromStream(Stream stream)
		{
			if (!stream.CanSeek)
			{
				throw new ArgumentException("Stream is required to support Seek operations, and does not");
			}
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(ZipEntryAttachment.CopyBufferSize);
			byte[] array = bufferPool.Acquire();
			uint num = 0U;
			try
			{
				int bytesToRead;
				while ((bytesToRead = stream.Read(array, 0, array.Length)) > 0)
				{
					num = ZipEntryAttachment.ComputeCrc32FromBytes(array, bytesToRead, num);
				}
			}
			finally
			{
				if (array != null)
				{
					bufferPool.Release(array);
				}
			}
			stream.Seek(0L, SeekOrigin.Begin);
			return num;
		}

		private static uint ComputeCrc32FromBytes(byte[] data, int bytesToRead, uint seed)
		{
			uint num = seed ^ uint.MaxValue;
			for (int i = 0; i < bytesToRead; i++)
			{
				num = (ZipEntryAttachment.CrcTable[(int)((UIntPtr)((num ^ (uint)data[i]) & 255U))] ^ num >> 8);
			}
			return num ^ uint.MaxValue;
		}

		private static uint[] GenerateCrc32Table()
		{
			int num = 256;
			uint[] array = new uint[num];
			for (int i = 0; i < num; i++)
			{
				uint num2 = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					if ((num2 & 1U) != 0U)
					{
						num2 = (3988292384U ^ num2 >> 1);
					}
					else
					{
						num2 >>= 1;
					}
				}
				array[i] = num2;
			}
			return array;
		}

		private void WriteFileData(Stream inputStream, Stream outputStream, BlockStatus blockStatus)
		{
			uint[] array = new uint[3];
			if (this.CompressionMethod == 8)
			{
				array = ZipEntryAttachment.CompressAndWriteOutputStream(outputStream, inputStream, true);
				this.attachmentBytesWritten = array[0];
				this.CheckSum = array[1];
				this.attachmentSize = (long)((ulong)array[2]);
				return;
			}
			this.attachmentBytesWritten = ZipEntryAttachment.WriteOutputStream(outputStream, inputStream);
		}

		private static uint[] CompressAndWriteOutputStream(Stream outputStream, Stream inputStream, bool doComputeCrc)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			uint num = 0U;
			int num2 = 0;
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(ZipEntryAttachment.CopyBufferSize);
			byte[] array = bufferPool.Acquire();
			uint num3 = 0U;
			using (Stream stream = Streams.CreateTemporaryStorageStream())
			{
				try
				{
					int num4;
					using (Stream stream2 = new DeflateStream(stream, CompressionMode.Compress, true))
					{
						while ((num4 = inputStream.Read(array, 0, array.Length)) > 0)
						{
							if (doComputeCrc)
							{
								num3 = ZipEntryAttachment.ComputeCrc32FromBytes(array, num4, num3);
							}
							num2 += num4;
							stream2.Write(array, 0, num4);
						}
						stream2.Flush();
					}
					stream.Seek(0L, SeekOrigin.Begin);
					while ((num4 = stream.Read(array, 0, array.Length)) > 0)
					{
						outputStream.Write(array, 0, num4);
						num += (uint)num4;
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
			return new uint[]
			{
				num,
				num3,
				(uint)num2
			};
		}

		private static uint WriteOutputStream(Stream outputStream, Stream inputStream)
		{
			if (outputStream == null)
			{
				throw new ArgumentNullException("outputStream");
			}
			if (inputStream == null)
			{
				throw new ArgumentNullException("inputStream");
			}
			uint num = 0U;
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(ZipEntryAttachment.CopyBufferSize);
			byte[] array = bufferPool.Acquire();
			try
			{
				int num2;
				while ((num2 = inputStream.Read(array, 0, array.Length)) > 0)
				{
					outputStream.Write(array, 0, num2);
					num += (uint)num2;
				}
			}
			finally
			{
				if (array != null)
				{
					bufferPool.Release(array);
				}
			}
			return num;
		}

		private static Stream GetStreamAttachmentStream(Attachment attachment)
		{
			StreamAttachmentBase streamAttachmentBase = attachment as StreamAttachmentBase;
			if (streamAttachmentBase == null)
			{
				throw new ArgumentNullException("stream", "Attachment is not a stream attachment");
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

		private Stream GetItemAttachmentStream(Attachment attachment, OutboundConversionOptions outboundConversionOptions)
		{
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
					stream = ZipEntryAttachment.GetContentsReplacementStream(-1706159495);
					text = ".txt";
				}
			}
			if (text != null)
			{
				this.fileName = this.GetConvertedItemFileName(this.fileName, text);
				this.encodedfileNameBytes = ZipEntryAttachment.GetEncodedString(this.fileName);
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

		private uint WriteZipFileDescriptor(Stream outputStream)
		{
			ByteBuffer byteBuffer = new ByteBuffer(16);
			byteBuffer.WriteUInt32(134695760U);
			byteBuffer.WriteUInt32(this.CheckSum);
			byteBuffer.WriteUInt32(this.attachmentBytesWritten);
			byteBuffer.WriteUInt32((uint)this.attachmentSize);
			byteBuffer.WriteContentsTo(outputStream);
			return (uint)byteBuffer.Length;
		}

		private Stream GetFilteredResponseStream(ConfigurationContextBase configurationContext, Stream inputStream, Charset charset, BlockStatus blockStatus)
		{
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(ZipEntryAttachment.CopyBufferSize);
			byte[] array = bufferPool.Acquire();
			Stream stream = new MemoryStream();
			try
			{
				using (Stream filteredStream = AttachmentUtilities.GetFilteredStream(configurationContext, inputStream, charset, blockStatus))
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

		private Stream GetUnfilteredStream(Stream stream)
		{
			if (!stream.CanSeek)
			{
				throw new ArgumentException("stream", "Stream is required to support Seek operations, and does not");
			}
			if (!this.doNotSniff && this.isNotHtmlandNotXml && ZipEntryAttachment.CheckShouldRemoveContents(stream))
			{
				if (ExTraceGlobals.AttachmentHandlingTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.AttachmentHandlingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ZipEntryAttachment.GetUnfilteredStream: Return contents removed for attachment {0}: mailbox {1}", this.fileName, AttachmentUtilities.TryGetMailboxIdentityName());
				}
				return ZipEntryAttachment.GetContentsReplacementStream(-1868113279);
			}
			if (ExTraceGlobals.AttachmentHandlingTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ZipEntryAttachment.GetUnfilteredStream: Return original contents for attachment {0}: mailbox {1}", this.fileName, AttachmentUtilities.TryGetMailboxIdentityName());
			}
			return stream;
		}

		private uint CurrentDosDateTime()
		{
			ExDateTime now = ExDateTime.Now;
			int num = now.Year - 1980 & 127;
			num = (num << 4) + now.Month;
			num = (num << 5) + now.Day;
			num = (num << 5) + now.Hour;
			num = (num << 6) + now.Minute;
			return (uint)((num << 5) + now.Second / 2);
		}

		private const int MinimumSizeForCompression = 1000;

		private const ushort VersionNeededToExtract = 20;

		private const ushort GeneralPurposeBitFlagDescriptor = 8;

		private const ushort GeneralPurposeBitFlagUtf8Encoded = 2048;

		private const ushort CompressionMethodStore = 0;

		private const ushort CompressionMethodDeflate = 8;

		private const ushort ExtraFieldLength = 0;

		private const string CompressedExts = "(?i)^\\.(mp3|png|docx|xlsx|pptx|jpg|zip|pdf|gif|mpg|aac|wma|wmv|mov)$";

		private const int DataSniffByteCount = 512;

		private static BufferPoolCollection.BufferSize CopyBufferSize = BufferPoolCollection.BufferSize.Size2K;

		private static Regex alreadyCompressedRegex = new Regex("(?i)^\\.(mp3|png|docx|xlsx|pptx|jpg|zip|pdf|gif|mpg|aac|wma|wmv|mov)$");

		private static uint[] CrcTable = ZipEntryAttachment.GenerateCrc32Table();

		private readonly bool doNeedToFilterHtml;

		private readonly bool doNotSniff;

		private readonly bool isNotHtmlandNotXml;

		private readonly AttachmentId attachmentId;

		private string fileName;

		private long headerOffset;

		private uint headerBytesWritten;

		private uint directoryBytesWritten;

		private uint attachmentBytesWritten;

		private long attachmentSize;

		private uint checkSum;

		private ushort compressionMethod;

		private ushort generalPurposeFlag;

		private uint currentDateTime;

		private IList<string> fileNames;

		private byte[] encodedfileNameBytes;
	}
}
