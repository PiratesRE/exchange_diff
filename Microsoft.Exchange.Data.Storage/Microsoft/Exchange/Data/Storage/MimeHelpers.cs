using System;
using System.IO;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MimeHelpers
	{
		internal static Charset GetCharsetFromMime(MimePart part)
		{
			Charset result = Charset.ASCII;
			Header header = part.Headers.FindFirst(HeaderId.ContentType);
			if (header != null)
			{
				MimeParameter mimeParameter = (header as ComplexHeader)["charset"];
				if (mimeParameter != null)
				{
					Charset charset = null;
					if (Charset.TryGetCharset(mimeParameter.Value, out charset))
					{
						result = charset;
					}
				}
			}
			return result;
		}

		internal static string GetHeaderValue(HeaderList headers, string headerName, InboundConversionOptions options)
		{
			return MimeHelpers.GetHeaderValue(headers.FindFirst(headerName), options);
		}

		internal static string GetHeaderValue(HeaderList headers, HeaderId headerId, InboundConversionOptions options)
		{
			return MimeHelpers.GetHeaderValue(headers.FindFirst(headerId), options);
		}

		internal static string GetHeaderValue(Header header, InboundConversionOptions options)
		{
			int lengthLimit = (options != null) ? options.Limits.MaxMimeTextHeaderLength : 2000;
			return MimeHelpers.GetHeaderValue(header, lengthLimit);
		}

		internal static string GetHeaderValue(Header header, int lengthLimit)
		{
			if (header == null)
			{
				return null;
			}
			string text = null;
			try
			{
				text = header.Value;
			}
			catch (InvalidCharsetException)
			{
				StorageGlobals.ContextTraceDebug<string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetHeaderValue: Header[{0}].Value throws InvalidCharsetException", header.Name);
				return null;
			}
			if (text != null && text.Length > lengthLimit)
			{
				StorageGlobals.ContextTraceDebug<string, int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetHeaderValue: Header[{0}].Value exceeds length limit ({1})", header.Name, lengthLimit);
				return null;
			}
			return text;
		}

		internal static string GetHeaderParameter(ComplexHeader header, string parameterName, InboundConversionOptions options)
		{
			return MimeHelpers.GetHeaderParameter(header, parameterName, options.Limits.MaxMimeTextHeaderLength);
		}

		internal static string GetHeaderParameter(ComplexHeader header, string parameterName, int lengthLimit)
		{
			if (header == null)
			{
				return null;
			}
			MimeParameter mimeParameter = header[parameterName];
			if (mimeParameter == null)
			{
				return null;
			}
			string text = null;
			try
			{
				text = mimeParameter.Value;
			}
			catch (InvalidCharsetException)
			{
				StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetHeaderParameter: Header[{0},{1}].Value throws InvalidCharsetException", header.Name, parameterName);
				return null;
			}
			if (text != null && text.Length > lengthLimit)
			{
				StorageGlobals.ContextTraceDebug<string, string, int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetHeaderParameter: Header[{0},{1}].Value exceeds length limit ({2})", header.Name, parameterName, lengthLimit);
				return null;
			}
			return text;
		}

		internal static Charset GetValidCharset(string charsetName)
		{
			Charset charset;
			if (charsetName != null && Charset.TryGetCharset(charsetName, out charset) && charset.IsAvailable)
			{
				return charset;
			}
			return null;
		}

		internal static Charset GetValidAndTrustedCharset(string charsetName, InboundConversionOptions options)
		{
			Charset validCharset = MimeHelpers.GetValidCharset(charsetName);
			if (validCharset != null && (options.TrustAsciiCharsets || !ConvertUtils.MimeStringEquals(charsetName, "us-ascii")))
			{
				return validCharset;
			}
			return null;
		}

		public static Charset ChooseCharset(string partCharset, string messageCharset, InboundConversionOptions conversionOptions)
		{
			Charset charset = MimeHelpers.GetValidAndTrustedCharset(partCharset, conversionOptions);
			if (charset != null)
			{
				return charset;
			}
			charset = MimeHelpers.GetValidAndTrustedCharset(messageCharset, conversionOptions);
			if (charset != null)
			{
				return charset;
			}
			charset = conversionOptions.DefaultCharset;
			if (charset != null && charset.IsAvailable)
			{
				return charset;
			}
			return Culture.Default.MimeCharset;
		}

		public static bool GetOpaqueContent(Stream inputStream, Stream outputStream)
		{
			bool result = false;
			int num = 0;
			using (Asn1Reader asn1Reader = new Asn1Reader(inputStream))
			{
				while (asn1Reader.ReadNext() && asn1Reader.EncodingType == EncodingType.Sequence)
				{
					if (!asn1Reader.ReadFirstChild() || asn1Reader.EncodingType != EncodingType.ObjectIdentifier)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected ContentType=EncodingType.ObjectIdentifier", ++num);
					}
					else if (asn1Reader.ReadValueAsOID() != OID.RSASignedData)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected RSA_signedData OID", ++num);
					}
					else if (!asn1Reader.ReadNextSibling() || asn1Reader.TagClass != TagClass.Context || asn1Reader.TagNumber != 0)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected content tag", ++num);
					}
					else if (!asn1Reader.ReadFirstChild() || asn1Reader.EncodingType != EncodingType.Sequence)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected EncodingType.Sequence", ++num);
					}
					else if (!asn1Reader.ReadFirstChild() || asn1Reader.EncodingType != EncodingType.Integer)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected version number", ++num);
					}
					else if (!asn1Reader.ReadNextSibling() || asn1Reader.EncodingType != EncodingType.Set)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected digest algorithm set", ++num);
					}
					else if (!asn1Reader.ReadNextSibling() || asn1Reader.EncodingType != EncodingType.Sequence)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected content tag", ++num);
					}
					else if (!asn1Reader.ReadFirstChild() || asn1Reader.EncodingType != EncodingType.ObjectIdentifier)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected ContentType = EncodingType.ObjectIdentifier for SignedData", ++num);
					}
					else if (asn1Reader.ReadValueAsOID() != OID.RSAData)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected RSA_Data OID", ++num);
					}
					else if (!asn1Reader.ReadNextSibling())
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find any content in S/MIME blob", ++num);
					}
					else if (asn1Reader.TagClass != TagClass.Context || asn1Reader.TagNumber != 0)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected tag class or value for S/MIME blob", ++num);
					}
					else if (!asn1Reader.ReadFirstChild() || asn1Reader.EncodingType != EncodingType.OctetString)
					{
						StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected EncodingType for S/MIME blob", ++num);
					}
					else
					{
						BufferPoolCollection.BufferSize bufferSize = BufferPoolCollection.BufferSize.Size20K;
						BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(bufferSize);
						byte[] array = null;
						try
						{
							array = bufferPool.Acquire();
							do
							{
								if (!asn1Reader.IsConstructedTag)
								{
									for (;;)
									{
										int num2 = asn1Reader.ReadBytesValue(array, 0, array.Length);
										if (num2 == 0)
										{
											break;
										}
										outputStream.Write(array, 0, num2);
										result = true;
									}
								}
								asn1Reader.ReadNext();
							}
							while (asn1Reader.EncodingType == EncodingType.OctetString);
							continue;
						}
						finally
						{
							if (array != null)
							{
								bufferPool.Release(array);
							}
						}
					}
					return result;
				}
				StorageGlobals.ContextTraceDebug<int>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter.GetOpaqueContent ({0}): did not find expected ContentInfo=EncodingType.Sequence", ++num);
			}
			return result;
		}
	}
}
