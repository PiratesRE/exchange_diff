using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ItemCharsetDetector
	{
		internal BodyCharsetFlags CharsetFlags
		{
			get
			{
				return this.charsetFlags;
			}
			set
			{
				this.charsetFlags = value;
			}
		}

		internal bool NoMessageDecoding
		{
			get
			{
				return this.noMessageDecoding;
			}
			set
			{
				this.noMessageDecoding = value;
			}
		}

		public ItemCharsetDetector(CoreItem coreItem)
		{
			this.coreItem = coreItem;
		}

		public void ResetCachedBody()
		{
			this.cachedBodyData = null;
		}

		public void SetCachedBody(char[] cachedBodyData)
		{
			this.cachedBodyData = cachedBodyData;
		}

		internal CharsetDetectionOptions DetectionOptions
		{
			get
			{
				return this.detectionOptions;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.detectionOptions = new CharsetDetectionOptions(value);
			}
		}

		public Charset SetCachedBodyDataAndDetectCharset(char[] cachedBodyData, Charset userCharset, BodyCharsetFlags charsetFlags)
		{
			this.cachedBodyData = cachedBodyData;
			this.CharsetFlags = charsetFlags;
			if (userCharset == null)
			{
				userCharset = this.detectionOptions.PreferredCharset;
			}
			Charset charset;
			if (this.IsItemCharsetKnownWithoutDetection(charsetFlags, userCharset, out charset))
			{
				this.SetItemCharset(charset);
				return charset;
			}
			MemoryStream memoryStream;
			int num = this.DetectCpidWithOptions(userCharset, out memoryStream);
			if (!Charset.TryGetCharset(num, out charset) || !charset.IsAvailable)
			{
				ExTraceGlobals.StorageTracer.TraceError<int>((long)this.GetHashCode(), "SetCachedBodyAndDetectCharset: stamping codepage {0} not a valid charset", num);
			}
			this.SetItemCharset(charset);
			return charset;
		}

		private void SetItemCharset(Charset charset)
		{
			this.coreItem.LocationIdentifierHelperInstance.SetLocationIdentifier(34677U);
			this.coreItem.PropertyBag.SetProperty(InternalSchema.InternetCpid, charset.CodePage);
			this.coreItem.PropertyBag.SetProperty(InternalSchema.Codepage, ConvertUtils.MapItemWindowsCharset(charset).CodePage);
		}

		private bool IsCharsetDetectionDisabled(BodyCharsetFlags flags)
		{
			return this.DetectionOptions.RequiredCoverage == 0 || (flags & BodyCharsetFlags.CharsetDetectionMask) == BodyCharsetFlags.DisableCharsetDetection;
		}

		public void ValidateItemCharset()
		{
			if (this.IsCharsetDetectionDisabled(this.CharsetFlags))
			{
				return;
			}
			int valueOrDefault = this.coreItem.PropertyBag.GetValueOrDefault<int>(InternalSchema.InternetCpid);
			if (ItemCharsetDetector.IsMultipleLanguageCodePage(valueOrDefault))
			{
				if (this.coreItem.Body.RawFormat == BodyFormat.TextHtml && this.coreItem.Body.ForceRedetectHtmlBodyCharset)
				{
					Charset charset;
					Charset targetCharset;
					if (this.TryGetHtmlCharsetFromMetaTag(4096U, out charset) && charset.CodePage != valueOrDefault && Charset.TryGetCharset(valueOrDefault, out targetCharset))
					{
						this.RestampItemCharset(targetCharset, null, charset);
					}
					this.coreItem.Body.ForceRedetectHtmlBodyCharset = false;
				}
				return;
			}
			Charset charset2;
			if (ConvertUtils.TryGetValidCharset(valueOrDefault, out charset2) && charset2.IsDetectable)
			{
				StringBuilder stringBuilder = new StringBuilder();
				this.coreItem.GetCharsetDetectionData(stringBuilder, CharsetDetectionDataFlags.None);
				if (stringBuilder.Length == 0)
				{
					return;
				}
				OutboundCodePageDetector outboundCodePageDetector = new OutboundCodePageDetector();
				outboundCodePageDetector.AddText(stringBuilder.ToString());
				if (outboundCodePageDetector.GetCodePageCoverage(valueOrDefault) >= this.DetectionOptions.RequiredCoverage)
				{
					return;
				}
			}
			MemoryStream cachedHtml;
			int num = this.DetectCpidWithOptions(null, out cachedHtml);
			Charset charset3;
			if (Charset.TryGetCharset(num, out charset3) && charset3.IsAvailable)
			{
				this.RestampItemCharset(charset3, cachedHtml, null);
				return;
			}
			ExTraceGlobals.StorageTracer.TraceError<int>((long)this.GetHashCode(), "ValidateItemCharset: detected codepage {0} is not valid", num);
		}

		public static bool IsMultipleLanguageCodePage(int internetCpid)
		{
			return internetCpid == 65001 || internetCpid == 65000 || internetCpid == 1200 || internetCpid == 1201 || internetCpid == 54936;
		}

		private int DetectCpidWithOptions(Charset userCharset, out MemoryStream cachedHtmlBody)
		{
			cachedHtmlBody = null;
			int cpid;
			if (this.IsCharsetDetectionDisabled(this.CharsetFlags) && userCharset != null)
			{
				cpid = userCharset.CodePage;
			}
			else
			{
				cpid = this.DetectCpid(userCharset, out cachedHtmlBody);
			}
			return this.OverrideCodePage(this.CharsetFlags, cpid);
		}

		private OutboundCodePageDetector BuildCodePageDetector(out MemoryStream cachedHtmlBody)
		{
			cachedHtmlBody = null;
			OutboundCodePageDetector outboundCodePageDetector = new OutboundCodePageDetector();
			StringBuilder stringBuilder = new StringBuilder();
			CharsetDetectionDataFlags charsetDetectionDataFlags = CharsetDetectionDataFlags.Complete;
			if (this.coreItem.CharsetDetector.NoMessageDecoding && ItemBuilder.ReadStoreObjectTypeFromPropertyBag(this.coreItem.PropertyBag) == StoreObjectType.RightsManagedMessage)
			{
				charsetDetectionDataFlags |= CharsetDetectionDataFlags.NoMessageDecoding;
			}
			this.coreItem.GetCharsetDetectionData(stringBuilder, charsetDetectionDataFlags);
			outboundCodePageDetector.AddText(stringBuilder.ToString());
			if (this.cachedBodyData == null)
			{
				this.cachedBodyData = this.LoadCachedBodyData(out cachedHtmlBody);
			}
			if (this.cachedBodyData != null)
			{
				outboundCodePageDetector.AddText(this.cachedBodyData);
			}
			return outboundCodePageDetector;
		}

		private int DetectCpid(Charset userCharset, out MemoryStream cachedHtmlBody)
		{
			OutboundCodePageDetector outboundCodePageDetector = this.BuildCodePageDetector(out cachedHtmlBody);
			int codePage;
			if (userCharset != null && userCharset.IsDetectable && outboundCodePageDetector.GetCodePageCoverage(userCharset.CodePage) >= this.DetectionOptions.RequiredCoverage)
			{
				codePage = userCharset.CodePage;
			}
			else
			{
				codePage = outboundCodePageDetector.GetCodePage(userCharset, false);
			}
			return codePage;
		}

		private void RestampItemCharset(Charset targetCharset, MemoryStream cachedHtml, Charset htmlCharsetDetectedFromMetaTag = null)
		{
			Charset charset = htmlCharsetDetectedFromMetaTag ?? ConvertUtils.GetItemMimeCharset(this.coreItem.PropertyBag);
			BodyFormat rawFormat = this.coreItem.Body.RawFormat;
			if (rawFormat == BodyFormat.TextHtml)
			{
				using (MemoryStream memoryStream = cachedHtml ?? this.LoadHtmlBodyInMemory())
				{
					memoryStream.Position = 0L;
					using (Stream stream = this.coreItem.Body.InternalOpenBodyStream(InternalSchema.HtmlBody, PropertyOpenMode.Create))
					{
						using (Stream stream2 = new ConverterStream(stream, new HtmlToHtml
						{
							InputEncoding = charset.GetEncoding(),
							OutputEncoding = targetCharset.GetEncoding(),
							DetectEncodingFromMetaTag = false
						}, ConverterStreamAccess.Write))
						{
							Util.StreamHandler.CopyStreamData(memoryStream, stream2);
						}
					}
				}
			}
			this.SetItemCharset(targetCharset);
		}

		private bool TryGetHtmlCharsetFromMetaTag(uint maxBytesToSearch, out Charset htmlBodyCharset)
		{
			htmlBodyCharset = null;
			using (MemoryStream memoryStream = this.LoadHtmlBodyInMemory())
			{
				memoryStream.Position = 0L;
				Charset itemMimeCharset = ConvertUtils.GetItemMimeCharset(this.coreItem.PropertyBag);
				using (HtmlReader htmlReader = new HtmlReader(memoryStream, itemMimeCharset.GetEncoding()))
				{
					while (htmlReader.ReadNextToken())
					{
						if ((long)htmlReader.CurrentOffset > (long)((ulong)maxBytesToSearch))
						{
							return false;
						}
						if (htmlReader.TokenKind == HtmlTokenKind.EmptyElementTag && htmlReader.TagId == HtmlTagId.Meta)
						{
							bool flag = false;
							bool flag2 = false;
							string text = string.Empty;
							HtmlAttributeReader attributeReader = htmlReader.AttributeReader;
							while (!flag && attributeReader.ReadNext())
							{
								if (attributeReader.Id == HtmlAttributeId.Charset && attributeReader.HasValue)
								{
									text = attributeReader.ReadValue();
									flag = true;
								}
								else if (attributeReader.Id == HtmlAttributeId.Content && attributeReader.HasValue)
								{
									text = attributeReader.ReadValue();
									if (flag2)
									{
										flag = true;
									}
								}
								else if (attributeReader.Id == HtmlAttributeId.HttpEquiv && attributeReader.HasValue)
								{
									string a = attributeReader.ReadValue();
									if (string.Equals(a, "content-type", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "charset", StringComparison.OrdinalIgnoreCase))
									{
										flag2 = true;
										if (!string.IsNullOrEmpty(text))
										{
											flag = true;
										}
									}
								}
							}
							if (flag)
							{
								text = HtmlReader.CharsetFromString(text, flag2);
								if (!string.IsNullOrEmpty(text) && Charset.TryGetCharset(text, out htmlBodyCharset))
								{
									return true;
								}
								return false;
							}
						}
					}
				}
			}
			return false;
		}

		private char[] LoadCachedBodyData(out MemoryStream htmlBody)
		{
			htmlBody = null;
			if (!this.coreItem.Body.IsBodyDefined)
			{
				return null;
			}
			BodyReadConfiguration configuration = new BodyReadConfiguration(BodyFormat.TextPlain);
			if (this.coreItem.Body.RawFormat == BodyFormat.TextHtml)
			{
				htmlBody = this.LoadHtmlBodyInMemory();
				htmlBody.Position = 0L;
				using (TextReader textReader = new BodyTextReader(this.coreItem, configuration, new StreamWrapper(htmlBody, false)))
				{
					return Util.StreamHandler.ReadCharBuffer(textReader, 16384);
				}
			}
			char[] result;
			using (TextReader textReader2 = new BodyTextReader(this.coreItem, configuration, null))
			{
				result = Util.StreamHandler.ReadCharBuffer(textReader2, 16384);
			}
			return result;
		}

		private MemoryStream LoadHtmlBodyInMemory()
		{
			MemoryStream memoryStream = new MemoryStream();
			using (Stream stream = this.coreItem.Body.InternalOpenBodyStream(InternalSchema.HtmlBody, PropertyOpenMode.ReadOnly))
			{
				Util.StreamHandler.CopyStreamData(stream, memoryStream);
			}
			memoryStream.Position = 0L;
			return memoryStream;
		}

		private int OverrideCodePage(BodyCharsetFlags charsetFlags, int cpid)
		{
			if (cpid == 936)
			{
				if ((charsetFlags & BodyCharsetFlags.PreferGB18030) != BodyCharsetFlags.PreferGB18030)
				{
					return 936;
				}
				return 54936;
			}
			else if (cpid == 28591)
			{
				if ((charsetFlags & BodyCharsetFlags.PreferIso885915) != BodyCharsetFlags.PreferIso885915)
				{
					return 28591;
				}
				return 28605;
			}
			else if (cpid == 932)
			{
				if ((charsetFlags & BodyCharsetFlags.DoNotPreferIso2022jp) == BodyCharsetFlags.DoNotPreferIso2022jp)
				{
					return 932;
				}
				return this.detectionOptions.PreferredInternetCodePageForShiftJis;
			}
			else
			{
				if (cpid == 1200 || cpid == 1201)
				{
					return 65001;
				}
				return cpid;
			}
		}

		private Charset OverrideCharset(BodyCharsetFlags flags, Charset charset)
		{
			int num = this.OverrideCodePage(flags, charset.CodePage);
			if (charset.CodePage != num)
			{
				return Charset.GetCharset(num);
			}
			return charset;
		}

		internal bool IsItemCharsetKnownWithoutDetection(BodyCharsetFlags flags, Charset userCharset, out Charset targetCharset)
		{
			targetCharset = null;
			if (userCharset == null)
			{
				userCharset = this.detectionOptions.PreferredCharset;
			}
			if (userCharset == null)
			{
				return false;
			}
			if (this.IsCharsetDetectionDisabled(flags))
			{
				targetCharset = this.OverrideCharset(flags, userCharset);
				return true;
			}
			if (userCharset.CodePage == 54936)
			{
				targetCharset = userCharset;
				return true;
			}
			if (BodyCharsetFlags.PreserveUnicode == (BodyCharsetFlags.PreserveUnicode & flags))
			{
				Charset charset = this.OverrideCharset(flags, userCharset);
				if (charset.CodePage == 65000 || charset.CodePage == 65001)
				{
					targetCharset = charset;
					return true;
				}
			}
			return false;
		}

		private const int DefaultDetectionBufferCharacters = 16384;

		private readonly CoreItem coreItem;

		private char[] cachedBodyData;

		private BodyCharsetFlags charsetFlags;

		private CharsetDetectionOptions detectionOptions = new CharsetDetectionOptions();

		private bool noMessageDecoding;
	}
}
