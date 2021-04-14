using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class ZipFileAttachments
	{
		internal ZipFileAttachments(BlockStatus blockStatus, string zipFileName)
		{
			this.files = new SortedDictionary<string, ZipEntryAttachment>(StringComparer.OrdinalIgnoreCase);
			this.blockStatus = blockStatus;
			this.zipFileName = ZipFileAttachments.ConditionZipFileNameForMimeHeader(zipFileName);
		}

		public int Count
		{
			get
			{
				return this.files.Count;
			}
		}

		public void AddAttachmentToZip(Attachment attachment, AttachmentPolicyLevel policyLevel, ConfigurationContextBase configurationContextBase)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("attachment");
			}
			if (configurationContextBase == null)
			{
				throw new ArgumentNullException("configurationContextBase");
			}
			if (attachment.AttachmentType == AttachmentType.EmbeddedMessage)
			{
				this.hasEmbeddedItem = true;
			}
			string contentType = AttachmentUtilities.GetContentType(attachment);
			string fileExtension = attachment.FileExtension;
			bool doNeedToFilterHtml = AttachmentUtilities.NeedToFilterHtml(contentType, fileExtension, policyLevel, configurationContextBase);
			bool isNotHtmlandNotXml = !AttachmentUtilities.GetIsHtmlOrXml(contentType, fileExtension);
			bool doNotSniff = AttachmentUtilities.GetDoNotSniff(policyLevel, configurationContextBase);
			string text = string.IsNullOrEmpty(attachment.FileName) ? attachment.DisplayName : attachment.FileName;
			if (attachment.AttachmentType == AttachmentType.EmbeddedMessage)
			{
				text += fileExtension;
			}
			string text2 = this.ConditionZipEntryFileName(text);
			attachment.FileName = text2;
			ZipEntryAttachment value = new ZipEntryAttachment(text2, attachment, doNeedToFilterHtml, doNotSniff, isNotHtmlandNotXml);
			this.files.Add(text2, value);
		}

		public Stream WriteArchive(ConfigurationContextBase configurationContext, IAttachmentWebOperationContext webOperationContext, OutboundConversionOptions outboundConversionOptions, AttachmentCollection attachmentCollection)
		{
			Stream stream = new MemoryStream();
			ZipFileAttachments.SetResponseHeadersForZipAttachments(webOperationContext, this.zipFileName);
			List<string> list = new List<string>();
			if (this.hasEmbeddedItem)
			{
				foreach (KeyValuePair<string, ZipEntryAttachment> keyValuePair in this.files)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (ZipEntryAttachment zipEntryAttachment in this.files.Values)
			{
				this.bytesWritten += zipEntryAttachment.WriteToStream(configurationContext, stream, outboundConversionOptions, this.blockStatus, this.bytesWritten, list, attachmentCollection);
			}
			long startOfDirectory = this.bytesWritten;
			foreach (ZipEntryAttachment zipEntryAttachment2 in this.files.Values)
			{
				this.bytesWritten += (long)((ulong)zipEntryAttachment2.WriteCentralDirectoryStructure(stream));
			}
			long endOfDirectory = this.bytesWritten;
			this.WriteTrailer(stream, startOfDirectory, endOfDirectory);
			return stream;
		}

		private static void SetResponseHeadersForZipAttachments(IAttachmentWebOperationContext webOperationContext, string fileName)
		{
			if (webOperationContext == null)
			{
				throw new ArgumentNullException("webOperationContext");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("Argument fileName may not be null or empty string");
			}
			webOperationContext.ContentType = "application/zip; authoritative=true;";
			ZipFileAttachments.SetZipContentDispositionResponseHeader(webOperationContext, fileName);
			DateHeader dateHeader = new DateHeader("Date", DateTime.UtcNow.AddDays(-1.0));
			webOperationContext.Headers["Expires"] = dateHeader.Value;
		}

		private static void SetZipContentDispositionResponseHeader(IAttachmentWebOperationContext webOperationContext, string fileName)
		{
			bool chrome = webOperationContext.UserAgent.IsBrowserChrome();
			string str = AttachmentUtilities.ToHexString(fileName, chrome);
			webOperationContext.Headers["Content-Disposition"] = "filename=" + str;
		}

		private static string ToSafeFileNameString(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("filename", "Argument fileName may not be null or empty string");
			}
			StringBuilder stringBuilder = new StringBuilder(fileName.Length);
			for (int i = 0; i < fileName.Length; i++)
			{
				if (!ZipFileAttachments.IsFilenameOrPathInvalidChar(fileName[i]))
				{
					stringBuilder.Append(fileName[i]);
				}
				else
				{
					stringBuilder.Append('_');
				}
			}
			return stringBuilder.ToString();
		}

		private static bool IsFilenameOrPathInvalidChar(char c)
		{
			if (char.IsControl(c))
			{
				return true;
			}
			if (c <= '/')
			{
				if (c != '"' && c != '*' && c != '/')
				{
					return false;
				}
			}
			else
			{
				switch (c)
				{
				case ':':
				case '<':
				case '>':
				case '?':
					break;
				case ';':
				case '=':
					return false;
				default:
					if (c != '\\' && c != '|')
					{
						return false;
					}
					break;
				}
			}
			return true;
		}

		private static string ConditionZipFileNameForMimeHeader(string fileName)
		{
			string text = fileName;
			if (fileName.Length > 148)
			{
				text = fileName.Substring(0, 148);
			}
			text = text.Replace(' ', '_');
			if (ZipFileAttachments.regexWindowsReservedNames.IsMatch(text))
			{
				text = text.Substring(0, text.Length - 1) + '_';
			}
			return text + ".zip";
		}

		private bool FileNameExists(string fileName)
		{
			return this.files.ContainsKey(fileName);
		}

		private void WriteTrailer(Stream outputStream, long startOfDirectory, long endOfDirectory)
		{
			ByteBuffer byteBuffer = new ByteBuffer(22);
			byteBuffer.WriteUInt32(101010256U);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteUInt16((ushort)this.files.Count);
			byteBuffer.WriteUInt16((ushort)this.files.Count);
			byteBuffer.WriteUInt32((uint)(endOfDirectory - startOfDirectory));
			byteBuffer.WriteUInt32((uint)startOfDirectory);
			byteBuffer.WriteUInt16(0);
			byteBuffer.WriteContentsTo(outputStream);
		}

		private string ConditionZipEntryFileName(string fileName)
		{
			string text = ZipFileAttachments.ToSafeFileNameString(fileName);
			string text2 = Path.GetFileNameWithoutExtension(text);
			bool flag = false;
			if (text.Length > 148)
			{
				int length = 148 - (text.Length - text2.Length);
				text2 = text2.Substring(0, length);
				flag = true;
			}
			int num = text2.Length - 1;
			if (text2[0] == '.')
			{
				text2 = '_' + text2.Substring(1, num);
				flag = true;
			}
			if (ZipFileAttachments.regexWindowsReservedNames.IsMatch(text2) || text2[num] == '.')
			{
				text2 = text2.Substring(0, num) + '_';
				flag = true;
			}
			string extension = Path.GetExtension(text);
			string text3 = string.Empty;
			if (!flag)
			{
				text3 = text;
			}
			else
			{
				text3 = text2 + extension;
			}
			int num2 = 1;
			while (this.FileNameExists(text3))
			{
				text3 = string.Format("{0}[{1}]{2}", text2, num2, extension);
				num2++;
			}
			return text3;
		}

		private const int MaxFileNameLength = 148;

		private const int MaxZipFileNameLength = 148;

		private const string ContentDispositionHeader = "Content-Disposition";

		private const string WindowsReservedNames = "(?i)^(com\\d$|lpt\\d$|con$|nul$|prn$|aux$)";

		private const char Underscore = '_';

		private static Regex regexWindowsReservedNames = new Regex("(?i)^(com\\d$|lpt\\d$|con$|nul$|prn$|aux$)");

		private readonly string zipFileName;

		private SortedDictionary<string, ZipEntryAttachment> files;

		private BlockStatus blockStatus;

		private long bytesWritten;

		private bool hasEmbeddedItem;
	}
}
