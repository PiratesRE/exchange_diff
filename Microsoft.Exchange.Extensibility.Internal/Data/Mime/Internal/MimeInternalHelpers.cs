using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data.Mime.Internal
{
	internal sealed class MimeInternalHelpers
	{
		public static byte[] GetHeaderRawValue(Header header)
		{
			return header.RawValue;
		}

		public static void SetHeaderRawValue(Header header, byte[] rawValue)
		{
			header.RawValue = rawValue;
		}

		public static void SetDocumentDecodingOptions(MimeDocument mimeDocument, DecodingOptions decodingOptions)
		{
			mimeDocument.HeaderDecodingOptions = decodingOptions;
		}

		public static void SetDecodingOptionsDecodingFlags(ref DecodingOptions decodingOptions, DecodingFlags decodingFlags)
		{
			decodingOptions.DecodingFlags = decodingFlags;
		}

		public static long GetDocumentPosition(MimeDocument document)
		{
			return document.Position;
		}

		public static Stream GetLoadStream(MimeDocument document, bool expectBinaryContent)
		{
			return document.GetLoadStream(expectBinaryContent);
		}

		public static int IndexOf(byte[] buffer, byte val, int offset, int count)
		{
			return ByteString.IndexOf(buffer, val, offset, count);
		}

		public static bool IsValidSmtpAddress(string address, bool checkLength, out int domainStart, bool allowUTF8 = false)
		{
			return MimeAddressParser.IsValidSmtpAddress(address, checkLength, out domainStart, allowUTF8);
		}

		public static bool IsValidDomain(string address, int offset, bool checkLength, bool allowUTF8 = false)
		{
			return MimeAddressParser.IsValidDomain(address, offset, checkLength, allowUTF8);
		}

		public static bool IsEaiEnabled()
		{
			bool result;
			try
			{
				result = InternalConfiguration.IsEaiEnabled();
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		internal static bool IsEncodingRequired(string value, bool allowUTF8 = false)
		{
			return MimeCommon.IsEncodingRequired(value, allowUTF8);
		}

		public static string DomainFromSmtpAddress(string address)
		{
			int startIndex;
			if (MimeAddressParser.IsValidSmtpAddress(address, false, out startIndex, false))
			{
				return address.Substring(startIndex);
			}
			return null;
		}

		public static string Rfc2047Encode(string value, Encoding encoding)
		{
			EncodingOptions encodingOptions = new EncodingOptions(Charset.GetCharset(encoding));
			return MimeCommon.EncodeValue(value, encodingOptions, ValueEncodingStyle.Normal).ToString();
		}

		public static string Rfc2047Decode(string encodedValue)
		{
			if (encodedValue == null)
			{
				return null;
			}
			if (!encodedValue.Contains("=?"))
			{
				return encodedValue;
			}
			MimeString str = new MimeString(encodedValue.Trim());
			MimeStringList lines = new MimeStringList(str);
			DecodingOptions decodingOptions = new DecodingOptions(DecodingFlags.Rfc2047);
			DecodingResults decodingResults;
			string result;
			if (!MimeCommon.TryDecodeValue(lines, 4026531840U, decodingOptions, out decodingResults, out result))
			{
				return encodedValue;
			}
			return result;
		}

		public static void SetDateHeaderValue(DateHeader header, DateTime value, TimeSpan timeZoneOffset)
		{
			header.SetValue(value, timeZoneOffset);
		}

		public static void CopyHeaderBetweenList(HeaderList sourceHeaderList, HeaderList targetHeaderList, string headerName, bool onlyWriteFirstHeader)
		{
			Header header = sourceHeaderList.FindFirst(headerName);
			while (header != null)
			{
				if (onlyWriteFirstHeader)
				{
					Header header2 = targetHeaderList.FindFirst(headerName);
					if (header2 != null)
					{
						header2.Value = header.Value;
						return;
					}
					targetHeaderList.AppendChild(header.Clone());
					return;
				}
				else
				{
					targetHeaderList.AppendChild(header.Clone());
					header = sourceHeaderList.FindNext(header);
				}
			}
		}

		public static void CopyHeaderBetweenList(HeaderList sourceHeaderList, HeaderList targetHeaderList, string headerName)
		{
			MimeInternalHelpers.CopyHeaderBetweenList(sourceHeaderList, targetHeaderList, headerName, false);
		}

		public static string BytesToString(byte[] bytes, bool allowUTF8 = true)
		{
			return ByteString.BytesToString(bytes, allowUTF8);
		}

		public static string BytesToString(byte[] bytes, int offset, int count, bool allowUTF8 = true)
		{
			return ByteString.BytesToString(bytes, offset, count, allowUTF8);
		}

		public static byte[] StringToBytes(string value, bool allowUTF8 = true)
		{
			return ByteString.StringToBytes(value, allowUTF8);
		}

		public static int StringToBytes(string value, int valueOffset, int valueCount, byte[] bytes, int bytesOffset, bool allowUTF8 = true)
		{
			return ByteString.StringToBytes(value, valueOffset, valueCount, bytes, bytesOffset, allowUTF8);
		}

		public static bool IsPureASCII(string value)
		{
			return MimeString.IsPureASCII(value);
		}

		public const int MaxEmailName = 315;

		public const int MaxX400EmailName = 1604;

		public const int MaxDomainName = 255;

		public const int MaxInternetName = 571;

		public const int MaxX400InternetName = 1860;
	}
}
