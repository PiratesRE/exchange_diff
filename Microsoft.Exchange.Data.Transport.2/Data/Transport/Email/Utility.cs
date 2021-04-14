using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class Utility
	{
		internal static bool IsBodyContentType(string contentType)
		{
			BodyTypes bodyType = Utility.GetBodyType(contentType);
			return bodyType != BodyTypes.None;
		}

		internal static BodyTypes GetBodyType(string contentType)
		{
			if (contentType == "text/plain")
			{
				return BodyTypes.Text;
			}
			if (contentType == "text/html")
			{
				return BodyTypes.Html;
			}
			if (contentType == "text/enriched")
			{
				return BodyTypes.Enriched;
			}
			if (contentType == "text/calendar")
			{
				return BodyTypes.Calendar;
			}
			return BodyTypes.None;
		}

		internal static Charset TranslateWriteStreamCharset(Charset charset)
		{
			if (charset.Kind == CodePageKind.Unicode)
			{
				charset = charset.Culture.MimeCharset;
			}
			else if (charset.CodePage == 20127)
			{
				charset = Charset.GetCharset(28591);
				if (!charset.IsAvailable)
				{
					charset = Charset.GetCharset(1252);
				}
				if (!charset.IsAvailable)
				{
					charset = Charset.ASCII;
				}
			}
			return charset;
		}

		internal static InternalAttachmentType CheckContentDisposition(MimePart part)
		{
			string headerValue = Utility.GetHeaderValue(part, HeaderId.ContentDisposition);
			if (string.IsNullOrEmpty(headerValue) || !headerValue.Equals("inline", StringComparison.OrdinalIgnoreCase))
			{
				return InternalAttachmentType.Regular;
			}
			string headerValue2 = Utility.GetHeaderValue(part, HeaderId.ContentId);
			string headerValue3 = Utility.GetHeaderValue(part, HeaderId.ContentLocation);
			if (string.IsNullOrEmpty(headerValue2) && string.IsNullOrEmpty(headerValue3))
			{
				return InternalAttachmentType.Regular;
			}
			return InternalAttachmentType.Inline;
		}

		internal static string GetHeaderValue(MimePart part, HeaderId headerId)
		{
			Header header = part.Headers.FindFirst(headerId);
			if (header == null)
			{
				return null;
			}
			return Utility.GetHeaderValue(header);
		}

		internal static string GetHeaderValue(Header header)
		{
			TextHeader textHeader = header as TextHeader;
			if (textHeader != null)
			{
				DecodingResults decodingResults;
				string result;
				textHeader.TryGetValue(Utility.DecodeOrFallBack, out decodingResults, out result);
				return result;
			}
			if (header != null)
			{
				return header.Value;
			}
			return null;
		}

		internal static string GetParameterValue(ComplexHeader header, string parameterName)
		{
			string result = null;
			MimeParameter mimeParameter = header[parameterName];
			if (mimeParameter != null)
			{
				DecodingResults decodingResults;
				mimeParameter.TryGetValue(Utility.DecodeOrFallBack, out decodingResults, out result);
				return result;
			}
			return result;
		}

		internal static string GetParameterValue(MimePart part, HeaderId headerId, string parameterName)
		{
			string result = null;
			ComplexHeader complexHeader = part.Headers.FindFirst(headerId) as ComplexHeader;
			if (complexHeader != null)
			{
				result = Utility.GetParameterValue(complexHeader, parameterName);
			}
			return result;
		}

		internal static void SetParameterValue(MimePart part, HeaderId headerId, string parameterName, string value)
		{
			ComplexHeader complexHeader = part.Headers.FindFirst(headerId) as ComplexHeader;
			if (complexHeader == null)
			{
				complexHeader = (Header.Create(headerId) as ComplexHeader);
				complexHeader.AppendChild(new MimeParameter(parameterName, value));
				part.Headers.AppendChild(complexHeader);
				return;
			}
			MimeParameter mimeParameter = complexHeader[parameterName];
			if (mimeParameter != null)
			{
				mimeParameter.Value = value;
				return;
			}
			complexHeader.AppendChild(new MimeParameter(parameterName, value));
		}

		internal static MimePart CreateBodyPart(string type, string charsetName)
		{
			MimePart mimePart = new MimePart();
			ContentTypeHeader contentTypeHeader = new ContentTypeHeader(type);
			MimeParameter newChild = new MimeParameter("charset", charsetName);
			contentTypeHeader.AppendChild(newChild);
			mimePart.Headers.AppendChild(contentTypeHeader);
			return mimePart;
		}

		internal static MimePart GetStartChild(MimePart part)
		{
			string parameterValue = Utility.GetParameterValue(part, HeaderId.ContentType, "start");
			foreach (MimePart mimePart in part)
			{
				if (string.IsNullOrEmpty(parameterValue))
				{
					return mimePart;
				}
				Header header = mimePart.Headers.FindFirst(HeaderId.ContentId);
				if (header != null && Utility.CompareIdentifiers(header.Value, parameterValue))
				{
					return mimePart;
				}
			}
			return part.FirstChild as MimePart;
		}

		internal static bool HasExactlyOneChild(MimePart part)
		{
			return part.IsMultipart && part.FirstChild != null && null == part.FirstChild.NextSibling;
		}

		internal static bool HasExactlyTwoChildren(MimePart part)
		{
			return part.IsMultipart && part.FirstChild != null && part.FirstChild.NextSibling != null && null == part.FirstChild.NextSibling.NextSibling;
		}

		internal static bool HasAtLeastTwoChildren(MimePart part)
		{
			return part.IsMultipart && part.FirstChild != null && part.FirstChild.NextSibling != null;
		}

		internal static bool Get2047CharsetName(AddressItem addressItem, out string charsetName)
		{
			DecodingOptions decodingOptions = new DecodingOptions(DecodingFlags.Rfc2047, null);
			DecodingResults decodingResults;
			string text;
			if (addressItem.TryGetDisplayName(decodingOptions, out decodingResults, out text) && EncodingScheme.Rfc2047 == decodingResults.EncodingScheme)
			{
				charsetName = decodingResults.CharsetName;
				return true;
			}
			charsetName = null;
			return false;
		}

		internal static bool Get2047CharsetName(TextHeader textHeader, out string charsetName)
		{
			DecodingOptions decodingOptions = new DecodingOptions(DecodingFlags.Rfc2047, null);
			DecodingResults decodingResults;
			string text;
			if (textHeader.TryGetValue(decodingOptions, out decodingResults, out text) && EncodingScheme.Rfc2047 == decodingResults.EncodingScheme)
			{
				charsetName = decodingResults.CharsetName;
				return true;
			}
			charsetName = null;
			return false;
		}

		internal static Header FindLastHeader(MimePart part, HeaderId headerId)
		{
			Header result = null;
			for (Header header = part.Headers.FindFirst(headerId); header != null; header = part.Headers.FindNext(header))
			{
				result = header;
			}
			return result;
		}

		internal static Header FindLastHeader(MimePart part, string name)
		{
			Header result = null;
			for (Header header = part.Headers.FindFirst(name); header != null; header = part.Headers.FindNext(header))
			{
				result = header;
			}
			return result;
		}

		internal static bool TryGet822Subject(MimePart mimePart, ref string subject)
		{
			if (mimePart.FirstChild == null)
			{
				return false;
			}
			MimePart mimePart2 = mimePart.FirstChild as MimePart;
			if (mimePart2 == null)
			{
				return false;
			}
			Header header = Utility.FindLastHeader(mimePart2, HeaderId.Subject);
			if (header == null)
			{
				return false;
			}
			string headerValue = Utility.GetHeaderValue(header);
			if (string.IsNullOrEmpty(headerValue))
			{
				return false;
			}
			subject = headerValue;
			return true;
		}

		internal static bool CompareIdentifiers(string a, string b)
		{
			if (Utility.IsBracketed(a) != Utility.IsBracketed(b))
			{
				if (Utility.IsBracketed(a))
				{
					a = a.Substring(1, a.Length - 2);
				}
				if (Utility.IsBracketed(b))
				{
					b = b.Substring(1, b.Length - 2);
				}
			}
			return a.Equals(b, StringComparison.Ordinal);
		}

		private static bool IsBracketed(string str)
		{
			return 2 <= str.Length && str[0] == '<' && str[str.Length - 1] == '>';
		}

		internal static void SynchronizeEncoding(BodyData body, MimePart part)
		{
			Encoding encoding = body.Encoding;
			Encoding encoding2 = Utility.GetEncoding(part);
			if (!encoding.Equals(encoding2))
			{
				Utility.SetEncoding(part, encoding);
			}
		}

		internal static void UpdateTransferEncoding(MimePart part, ContentTransferEncoding embeddedCte)
		{
			if (ContentTransferEncoding.EightBit == embeddedCte || ContentTransferEncoding.Binary == embeddedCte)
			{
				string value = (ContentTransferEncoding.EightBit == embeddedCte) ? "8bit" : "binary";
				do
				{
					ContentTransferEncoding contentTransferEncoding = part.ContentTransferEncoding;
					if (ContentTransferEncoding.Binary != contentTransferEncoding && embeddedCte != contentTransferEncoding)
					{
						Header header = part.Headers.FindFirst(HeaderId.ContentTransferEncoding);
						if (header == null)
						{
							header = Header.Create(HeaderId.ContentTransferEncoding);
							part.Headers.AppendChild(header);
						}
						header.Value = value;
					}
					part = (part.Parent as MimePart);
				}
				while (part != null);
				return;
			}
			Header header2 = part.Headers.FindFirst(HeaderId.ContentTransferEncoding);
			if (header2 != null)
			{
				header2.RemoveFromParent();
			}
		}

		private static bool IsRestrictedHeader(HeaderId headerId)
		{
			switch (headerId)
			{
			case HeaderId.Unknown:
			case HeaderId.Received:
			case HeaderId.To:
			case HeaderId.Cc:
			case HeaderId.Bcc:
			case HeaderId.Comments:
			case HeaderId.Keywords:
			case HeaderId.ResentDate:
			case HeaderId.ResentSender:
			case HeaderId.ResentFrom:
			case HeaderId.ResentBcc:
			case HeaderId.ResentCc:
			case HeaderId.ResentTo:
			case HeaderId.ResentReplyTo:
			case HeaderId.ResentMessageId:
				return false;
			}
			return true;
		}

		internal static void MoveChildToNewParent(MimeNode newParent, MimeNode child)
		{
			if (newParent == child.Parent)
			{
				return;
			}
			child.RemoveFromParent();
			newParent.AppendChild(child);
		}

		internal static void NormalizeHeaders(MimePart part, Utility.HeaderNormalization normalizationFlags)
		{
			Dictionary<HeaderId, Header> dictionary = new Dictionary<HeaderId, Header>(Utility.HeaderIdComparerInstance);
			foreach (Header header in part.Headers)
			{
				if (!dictionary.ContainsKey(header.HeaderId))
				{
					dictionary.Add(header.HeaderId, header);
				}
				else
				{
					Header header2 = dictionary[header.HeaderId];
					if ((normalizationFlags & Utility.HeaderNormalization.MergeAddressHeaders) != Utility.HeaderNormalization.None && header is AddressHeader)
					{
						foreach (MimeNode child in header)
						{
							Utility.MoveChildToNewParent(header2, child);
						}
						header.RemoveFromParent();
					}
					else if ((normalizationFlags & Utility.HeaderNormalization.PruneRestrictedHeaders) != Utility.HeaderNormalization.None && Utility.IsRestrictedHeader(header.HeaderId))
					{
						header2.RemoveFromParent();
						dictionary[header2.HeaderId] = header;
					}
				}
			}
		}

		public static void CopyStream(Stream src, Stream dst, ref byte[] scratchBuffer)
		{
			if (scratchBuffer == null)
			{
				scratchBuffer = new byte[4096];
			}
			int count;
			while (0 < (count = src.Read(scratchBuffer, 0, scratchBuffer.Length)))
			{
				dst.Write(scratchBuffer, 0, count);
			}
			dst.Flush();
		}

		internal static Exception InternalError()
		{
			return new Exception("EmailMessage internal error.");
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				throw new Exception(message);
			}
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
		}

		internal static bool TryGetAppleDoubleParts(MimePart doublePart, out MimePart dataPart, out MimePart resourcePart)
		{
			dataPart = null;
			resourcePart = null;
			if (!Utility.HasExactlyTwoChildren(doublePart))
			{
				return false;
			}
			if (doublePart.ContentType != "multipart/appledouble")
			{
				return false;
			}
			MimePart mimePart = doublePart.FirstChild as MimePart;
			MimePart mimePart2 = doublePart.FirstChild.NextSibling as MimePart;
			if (mimePart.ContentType == "application/applefile" && mimePart2.ContentType != "application/applefile" && !mimePart2.IsMultipart)
			{
				resourcePart = mimePart;
				dataPart = mimePart2;
				return true;
			}
			if (mimePart.ContentType != "application/applefile" && !mimePart.IsMultipart && mimePart2.ContentType == "application/applefile")
			{
				dataPart = mimePart;
				resourcePart = mimePart2;
				return true;
			}
			return false;
		}

		internal static string GetRawFileName(MimePart part)
		{
			string result = null;
			if (Utility.TryGetFileNameFromHeader(part, HeaderId.ContentDisposition, "filename", ref result))
			{
				return result;
			}
			if (Utility.TryGetFileNameFromHeader(part, HeaderId.ContentType, "name", ref result))
			{
				return result;
			}
			string contentType = part.ContentType;
			if (contentType.Equals("message/rfc822", StringComparison.OrdinalIgnoreCase))
			{
				string result2 = "No Subject";
				Utility.TryGet822Subject(part, ref result2);
				return result2;
			}
			if (contentType.Equals("multipart/appledouble", StringComparison.OrdinalIgnoreCase))
			{
				Utility.TryGetFileNameFromAppleDouble(part, ref result);
				return result;
			}
			if (contentType.Equals("application/applefile", StringComparison.OrdinalIgnoreCase))
			{
				Utility.TryGetFileNameFromAppleFile(part, ref result);
				return result;
			}
			ContentTransferEncoding contentTransferEncoding = part.ContentTransferEncoding;
			if (contentType.Equals("application/mac-binhex40", StringComparison.OrdinalIgnoreCase) || ContentTransferEncoding.BinHex == contentTransferEncoding)
			{
				Utility.TryGetFileNameFromBinHex(part, ref result);
				return result;
			}
			if (ContentTransferEncoding.UUEncode == contentTransferEncoding)
			{
				Utility.TryGetFileNameFromUuencode(part, ref result);
				return result;
			}
			if (Utility.TryGetFileNameFromHeader(part, HeaderId.ContentDescription, ref result))
			{
				return result;
			}
			return null;
		}

		internal static Stream GetContentReadStream(MimePart part)
		{
			Stream rawContentReadStream;
			if (!part.TryGetContentReadStream(out rawContentReadStream))
			{
				rawContentReadStream = part.GetRawContentReadStream();
			}
			return rawContentReadStream;
		}

		private static bool TryGetFileNameFromAppleDouble(MimePart part, ref string fileName)
		{
			MimePart mimePart;
			MimePart part2;
			if (Utility.TryGetAppleDoubleParts(part, out mimePart, out part2))
			{
				if (Utility.TryGetFileNameFromHeader(mimePart, HeaderId.ContentDisposition, "filename", ref fileName) || Utility.TryGetFileNameFromHeader(mimePart, HeaderId.ContentType, "name", ref fileName))
				{
					return true;
				}
				if (Utility.TryGetFileNameFromAppleFile(part2, ref fileName))
				{
					return true;
				}
			}
			return false;
		}

		private static bool TryGetFileNameFromAppleFile(MimePart part, ref string fileName)
		{
			string fileNameFromResourceFork;
			using (Stream contentReadStream = Utility.GetContentReadStream(part))
			{
				fileNameFromResourceFork = MimeAppleTranscoder.GetFileNameFromResourceFork(contentReadStream);
			}
			if (string.IsNullOrEmpty(fileNameFromResourceFork))
			{
				return false;
			}
			fileName = fileNameFromResourceFork;
			return true;
		}

		private static bool TryGetFileNameFromUuencode(MimePart part, ref string fileName)
		{
			byte[] array = new byte[1050];
			int inputSize;
			using (Stream rawContentReadStream = part.GetRawContentReadStream())
			{
				inputSize = rawContentReadStream.Read(array, 0, array.Length);
			}
			bool result;
			using (UUDecoder uudecoder = new UUDecoder())
			{
				byte[] array2 = new byte[1050];
				int num;
				int num2;
				bool flag;
				uudecoder.Convert(array, 0, inputSize, array2, 0, array2.Length, false, out num, out num2, out flag);
				string fileName2 = uudecoder.FileName;
				if (string.IsNullOrEmpty(fileName2))
				{
					result = false;
				}
				else
				{
					fileName = fileName2;
					result = true;
				}
			}
			return result;
		}

		internal static bool TryGetFileNameFromHeader(MimePart mimePart, HeaderId headerId, ref string fileName)
		{
			Header header = mimePart.Headers.FindFirst(headerId);
			string headerValue = Utility.GetHeaderValue(header);
			if (string.IsNullOrEmpty(headerValue))
			{
				return false;
			}
			fileName = headerValue;
			return true;
		}

		internal static bool TryGetFileNameFromHeader(MimePart mimePart, HeaderId headerId, string parameterName, ref string fileName)
		{
			string parameterValue = Utility.GetParameterValue(mimePart, headerId, parameterName);
			if (string.IsNullOrEmpty(parameterValue))
			{
				return false;
			}
			fileName = parameterValue;
			return true;
		}

		internal static bool TryGetFileNameFromBinHex(MimePart mimePart, ref string fileName)
		{
			string text;
			using (Stream rawContentReadStream = mimePart.GetRawContentReadStream())
			{
				try
				{
					BinHexDecoder binHexDecoder = new BinHexDecoder();
					using (EncoderStream encoderStream = new EncoderStream(rawContentReadStream, binHexDecoder, EncoderStreamAccess.Read))
					{
						byte[] array = new byte[256];
						encoderStream.Read(array, 0, array.Length);
						text = binHexDecoder.MacBinaryHeader.FileName;
					}
				}
				catch (ByteEncoderException)
				{
					text = null;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			fileName = text;
			return true;
		}

		internal static AttachmentMethod GetAttachmentMethod(TnefPropertyBag properties)
		{
			AttachmentMethod result = AttachmentMethod.AttachByValue;
			object obj = properties[TnefPropertyId.AttachMethod];
			if (obj is int)
			{
				result = (AttachmentMethod)((int)obj);
			}
			return result;
		}

		internal static string GetRawFileName(TnefPropertyBag properties, bool stnef)
		{
			AttachmentMethod attachmentMethod = Utility.GetAttachmentMethod(properties);
			if (attachmentMethod == AttachmentMethod.EmbeddedMessage)
			{
				return properties.GetProperty(TnefPropertyTag.DisplayNameA, stnef) as string;
			}
			string text = properties.GetProperty(TnefPropertyTag.AttachLongFilenameA, stnef) as string;
			if (string.IsNullOrEmpty(text))
			{
				text = (properties.GetProperty(TnefPropertyTag.AttachFilenameA, stnef) as string);
			}
			return text;
		}

		internal static string SanitizeOrRegenerateFileName(string rawName, ref int sequenceNumber)
		{
			string result;
			if (Utility.TrySanitizeAttachmentFileName(rawName, out result))
			{
				return result;
			}
			return Attachment.FileNameGenerator.GenerateFileName(ref sequenceNumber);
		}

		internal static void SetFileName(MimePart attachmentPart, AttachmentType attachmentType, string value)
		{
			TextHeader textHeader = attachmentPart.Headers.FindFirst(HeaderId.ContentDescription) as TextHeader;
			if (textHeader == null)
			{
				textHeader = (Header.Create(HeaderId.ContentDescription) as TextHeader);
				attachmentPart.Headers.AppendChild(textHeader);
			}
			textHeader.Value = value;
			Utility.StoreFileNameInHeader(attachmentPart, HeaderId.ContentType, () => attachmentPart.ContentType, "name", value);
			Utility.StoreFileNameInHeader(attachmentPart, HeaderId.ContentDisposition, delegate
			{
				if (attachmentType != AttachmentType.Inline)
				{
					return "attachment";
				}
				return "inline";
			}, "filename", value);
		}

		internal static void StoreFileNameInHeader(MimePart attachmentPart, HeaderId headerId, GetDefaultValue getDefaultValue, string parameterName, string value)
		{
			ComplexHeader complexHeader = attachmentPart.Headers.FindFirst(headerId) as ComplexHeader;
			if (complexHeader == null)
			{
				complexHeader = (Header.Create(headerId) as ComplexHeader);
				complexHeader.Value = getDefaultValue();
				attachmentPart.Headers.AppendChild(complexHeader);
			}
			MimeParameter mimeParameter = complexHeader[parameterName];
			if (mimeParameter == null)
			{
				mimeParameter = new MimeParameter(parameterName);
				complexHeader.AppendChild(mimeParameter);
			}
			mimeParameter.Value = value;
		}

		internal static bool TrySanitizeAttachmentFileName(string rawFileName, out string fileName)
		{
			fileName = rawFileName;
			if (string.IsNullOrEmpty(rawFileName))
			{
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder(fileName.Length);
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			int num = 0;
			for (int i = 0; i < fileName.Length; i++)
			{
				bool flag = false;
				char c = fileName[i];
				for (int j = 0; j < invalidFileNameChars.Length; j++)
				{
					if (c == invalidFileNameChars[j])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					stringBuilder.Append(c);
					num++;
				}
			}
			fileName = ((num == fileName.Length) ? fileName : stringBuilder.ToString());
			return !string.IsNullOrEmpty(fileName);
		}

		internal static string GetTnefCorrelator(MimePart part)
		{
			Header header = part.Headers.FindFirst("X-MS-TNEF-Correlator");
			return Utility.GetHeaderValue(header);
		}

		internal static string RemoveMimeHeaderComments(string headerValue)
		{
			if (headerValue == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(headerValue.Length);
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < headerValue.Length; i++)
			{
				bool flag3 = true;
				if (!flag2)
				{
					char c = headerValue[i];
					if (c != '"')
					{
						switch (c)
						{
						case '(':
							if (!flag)
							{
								num++;
							}
							break;
						case ')':
							if (!flag && num > 0)
							{
								num--;
								flag3 = false;
							}
							break;
						default:
							if (c == '\\')
							{
								flag2 = true;
							}
							break;
						}
					}
					else if (num == 0)
					{
						flag = !flag;
					}
					flag3 &= (num == 0);
				}
				else
				{
					flag2 = false;
				}
				if (flag3)
				{
					stringBuilder.Append(headerValue[i]);
				}
			}
			return stringBuilder.ToString().Trim();
		}

		private static Encoding GetEncoding(MimePart part)
		{
			string parameterValue = Utility.GetParameterValue(part, HeaderId.ContentType, "charset");
			Charset charset;
			Encoding result;
			if (Charset.TryGetCharset(parameterValue, out charset) && charset.TryGetEncoding(out result))
			{
				return result;
			}
			return Charset.DefaultMimeCharset.GetEncoding();
		}

		private static void SetEncoding(MimePart part, Encoding encoding)
		{
			Charset defaultMimeCharset;
			if (!Charset.TryGetCharset(encoding, out defaultMimeCharset))
			{
				defaultMimeCharset = Charset.DefaultMimeCharset;
			}
			Utility.SetParameterValue(part, HeaderId.ContentType, "charset", defaultMimeCharset.Name);
		}

		internal static DecodingOptions DecodeOrFallBack = new DecodingOptions((DecodingFlags)131071);

		private static readonly Utility.HeaderIdComparer HeaderIdComparerInstance = new Utility.HeaderIdComparer();

		[Flags]
		internal enum HeaderNormalization
		{
			None = 0,
			PruneRestrictedHeaders = 1,
			MergeAddressHeaders = 2,
			All = 255
		}

		private class HeaderIdComparer : IEqualityComparer<HeaderId>
		{
			public bool Equals(HeaderId x, HeaderId y)
			{
				return x == y;
			}

			public int GetHashCode(HeaderId obj)
			{
				return (int)obj;
			}
		}
	}
}
