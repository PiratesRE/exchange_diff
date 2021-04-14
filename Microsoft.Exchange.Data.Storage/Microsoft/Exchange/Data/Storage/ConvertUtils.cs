using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ConvertUtils
	{
		internal static T CallCtsWithReturnValue<T>(Trace tracer, string methodName, LocalizedString exceptionString, ConvertUtils.CtsCallWithReturnValue<T> ctsCall)
		{
			T returnValue = default(T);
			ConvertUtils.CallCts(tracer, methodName, exceptionString, delegate
			{
				returnValue = ctsCall();
			});
			return returnValue;
		}

		internal static void CallCts(Trace tracer, string methodName, LocalizedString exceptionString, ConvertUtils.CtsCall ctsCall)
		{
			try
			{
				ctsCall();
			}
			catch (ExchangeDataException ex)
			{
				StorageGlobals.ContextTraceError<string, ExchangeDataException>(tracer, "{0}: ExchangeDataException, {1}", methodName, ex);
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, exceptionString, ex);
			}
			catch (IOException ex2)
			{
				StorageGlobals.ContextTraceError<string, IOException>(tracer, "{0}: IOException, {1}", methodName, ex2);
				if (StorageGlobals.IsDiskFullException(ex2))
				{
					throw new StorageTransientException(exceptionString, ex2);
				}
				throw new StoragePermanentException(exceptionString, ex2);
			}
		}

		internal static string ExtractMimeContentId(string value)
		{
			if (value.Length >= 2 && value[0] == '<' && value[value.Length - 1] == '>')
			{
				return value.Substring(1, value.Length - 2);
			}
			return value;
		}

		internal static int GetCodepageFromCharset(string charset)
		{
			int result;
			if (ConvertUtils.TryGetCodepageFromCharset(charset, out result))
			{
				return result;
			}
			throw new NotSupportedException(ServerStrings.ExUnsupportedCharset(charset));
		}

		internal static int GetInternetEncodingFromCharset(string charset)
		{
			int result;
			if (ConvertUtils.TryGetInternetEncodingFromCharset(charset, out result))
			{
				return result;
			}
			throw new NotSupportedException(ServerStrings.ExUnsupportedCharset(charset));
		}

		internal static bool TryGetCodepageFromCharset(string charset, out int codepage)
		{
			return ConvertUtils.TryGetCodepageFromCharset(charset, false, out codepage);
		}

		internal static bool TryGetInternetEncodingFromCharset(string charset, out int codepage)
		{
			return ConvertUtils.TryGetCodepageFromCharset(charset, true, out codepage);
		}

		internal static Charset GetCharsetFromCharsetName(string charsetName)
		{
			Charset result;
			if (ConvertUtils.TryGetCharsetFromCharsetName(charsetName, out result))
			{
				return result;
			}
			throw new NotSupportedException(ServerStrings.ExUnsupportedCharset(charsetName));
		}

		internal static bool TryGetCharsetFromCharsetName(string charsetName, out Charset charset)
		{
			charset = null;
			if (string.IsNullOrEmpty(charsetName))
			{
				return false;
			}
			if (charsetName.StartsWith("InternalXsoCodepage-"))
			{
				int length = "InternalXsoCodepage-".Length;
				int codePage = int.Parse(charsetName.Substring(length, charsetName.Length - length));
				if (Charset.TryGetCharset(codePage, out charset) && charset.IsAvailable)
				{
					return true;
				}
			}
			else if (Charset.TryGetCharset(charsetName, out charset) && charset.IsAvailable)
			{
				return true;
			}
			return false;
		}

		private static bool TryGetCodepageFromCharset(string charsetName, bool isInternetEncoding, out int codepage)
		{
			codepage = 0;
			if (charsetName == null)
			{
				return false;
			}
			if (charsetName.StartsWith("InternalXsoCodepage-"))
			{
				int length = "InternalXsoCodepage-".Length;
				codepage = int.Parse(charsetName.Substring(length, charsetName.Length - length));
				return true;
			}
			Charset charset;
			if (!Charset.TryGetCharset(charsetName, out charset))
			{
				return false;
			}
			if (isInternetEncoding)
			{
				codepage = charset.CodePage;
				return true;
			}
			codepage = ConvertUtils.MapItemWindowsCharset(charset).CodePage;
			return true;
		}

		public static string GetCharsetFromCodepage(int codepage)
		{
			string result;
			if (ConvertUtils.TryGetCharsetFromCodepage(codepage, out result))
			{
				return result;
			}
			throw new NotSupportedException(ServerStrings.ExUnsupportedCodepage(codepage));
		}

		private static bool TryGetCharsetFromCodepage(int codepage, out string charsetName)
		{
			Charset charset;
			if (Charset.TryGetCharset(codepage, out charset))
			{
				charsetName = charset.Name;
				return true;
			}
			charsetName = null;
			return false;
		}

		internal static string WrapCodepageToCharset(int codepage)
		{
			return "InternalXsoCodepage-" + codepage.ToString();
		}

		private static int TransformCodepage(int codepage)
		{
			if (codepage <= 50932)
			{
				switch (codepage)
				{
				case 1200:
				case 1201:
					return 65000;
				default:
					if (codepage != 50225)
					{
						if (codepage != 50932)
						{
							return codepage;
						}
						return 50220;
					}
					break;
				}
			}
			else
			{
				if (codepage == 50936)
				{
					return 936;
				}
				switch (codepage)
				{
				case 50949:
					break;
				case 50950:
					return 950;
				default:
					if (codepage != 51256)
					{
						return codepage;
					}
					return 28596;
				}
			}
			return 51949;
		}

		internal static bool TryGetValidCharset(int codepage, out Charset charset)
		{
			if (Charset.TryGetCharset(codepage, out charset) && charset.IsAvailable)
			{
				return true;
			}
			charset = null;
			return false;
		}

		internal static bool TryTransformCharset(ref Charset charset)
		{
			int num = ConvertUtils.TransformCodepage(charset.CodePage);
			if (num == charset.CodePage)
			{
				return true;
			}
			Charset charset2;
			if (ConvertUtils.TryGetValidCharset(num, out charset2))
			{
				charset = charset2;
				return true;
			}
			return false;
		}

		internal static Charset GetItemOutboundMimeCharset(Item item, OutboundConversionOptions options)
		{
			Charset charset = ConvertUtils.GetItemOutboundMimeCharsetInternal(item, options);
			string className = item.ClassName;
			if (options.DetectionOptions.PreferredCharset == null && charset.CodePage != 54936 && (ObjectClass.IsTaskRequest(className) || ObjectClass.IsMeetingMessage(className) || ObjectClass.IsCalendarItem(className)))
			{
				charset = Charset.GetCharset(65001);
			}
			return charset;
		}

		private static Charset DetectOutboundCharset(Item item, OutboundConversionOptions options, object internetCpidObj, bool trustInternetCpid)
		{
			if (options != null && options.DetectionOptions.PreferredCharset != null && (options.DetectionOptions.RequiredCoverage == 0 || (!(internetCpidObj is PropertyError) && trustInternetCpid && options.DetectionOptions.PreferredCharset.CodePage == (int)internetCpidObj)))
			{
				Charset preferredCharset = options.DetectionOptions.PreferredCharset;
				if (ConvertUtils.TryTransformCharset(ref preferredCharset))
				{
					return preferredCharset;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			item.CoreItem.GetCharsetDetectionData(stringBuilder, CharsetDetectionDataFlags.Complete | CharsetDetectionDataFlags.NoMessageDecoding);
			if (item.Body.IsBodyDefined)
			{
				using (TextReader textReader = item.Body.OpenTextReader(BodyFormat.TextPlain))
				{
					char[] array = new char[32768];
					int charCount = textReader.ReadBlock(array, 0, array.Length);
					stringBuilder.Append(array, 0, charCount);
				}
			}
			OutboundCodePageDetector outboundCodePageDetector = new OutboundCodePageDetector();
			outboundCodePageDetector.AddText(stringBuilder.ToString());
			if (options != null && options.DetectionOptions.PreferredCharset != null && outboundCodePageDetector.GetCodePageCoverage(options.DetectionOptions.PreferredCharset.CodePage) >= options.DetectionOptions.RequiredCoverage)
			{
				Charset preferredCharset = options.DetectionOptions.PreferredCharset;
				if (ConvertUtils.TryTransformCharset(ref preferredCharset))
				{
					return preferredCharset;
				}
			}
			if (!(internetCpidObj is PropertyError) && !trustInternetCpid)
			{
				int num = (options != null) ? options.DetectionOptions.RequiredCoverage : 100;
				Charset preferredCharset;
				if (Charset.TryGetCharset((int)internetCpidObj, out preferredCharset) && preferredCharset.IsDetectable && outboundCodePageDetector.GetCodePageCoverage((int)internetCpidObj) >= num && ConvertUtils.TryTransformCharset(ref preferredCharset))
				{
					return preferredCharset;
				}
			}
			if (!trustInternetCpid || (internetCpidObj is PropertyError && !item.Body.IsBodyDefined))
			{
				int codePage = outboundCodePageDetector.GetCodePage();
				Charset preferredCharset;
				if (Charset.TryGetCharset(codePage, out preferredCharset) && ConvertUtils.TryTransformCharset(ref preferredCharset))
				{
					return preferredCharset;
				}
			}
			return null;
		}

		private static Charset GetItemOutboundMimeCharsetInternal(Item item, OutboundConversionOptions options)
		{
			object obj = item.TryGetProperty(InternalSchema.InternetCpid);
			bool valueOrDefault = item.GetValueOrDefault<bool>(InternalSchema.IsAutoForwarded, false);
			string className = item.ClassName;
			Charset charset = null;
			if (valueOrDefault || (obj is PropertyError && !item.Body.IsBodyDefined) || (options != null && options.DetectionOptions.PreferredCharset != null && (options.DetectionOptions.RequiredCoverage < 100 || ObjectClass.IsTaskRequest(className) || ObjectClass.IsMeetingMessage(className))))
			{
				charset = ConvertUtils.DetectOutboundCharset(item, options, obj, !valueOrDefault);
				if (charset != null)
				{
					if (!item.CharsetDetector.IsItemCharsetKnownWithoutDetection(BodyCharsetFlags.DisableCharsetDetection, charset, out charset))
					{
						throw new InvalidOperationException();
					}
					return charset;
				}
			}
			if (!(obj is PropertyError) && Charset.TryGetCharset((int)obj, out charset) && ConvertUtils.TryTransformCharset(ref charset))
			{
				return charset;
			}
			object obj2 = item.TryGetProperty(InternalSchema.Codepage);
			if (!(obj2 is PropertyError) && Charset.TryGetCharset((int)obj2, out charset))
			{
				charset = charset.Culture.MimeCharset;
				if (ConvertUtils.TryTransformCharset(ref charset))
				{
					return charset;
				}
			}
			return Charset.GetCharset(65001);
		}

		internal static bool TryGetWindowsCodepageFromInternetCpid(int internetCpid, out int windowsCodepage)
		{
			Charset charset;
			if (Charset.TryGetCharset(internetCpid, out charset))
			{
				charset = ConvertUtils.MapItemWindowsCharset(charset);
				if (charset != null && charset.IsAvailable)
				{
					windowsCodepage = charset.CodePage;
					return true;
				}
			}
			windowsCodepage = 0;
			return false;
		}

		internal static Charset GetItemMimeCharset(ICorePropertyBag propertyBag)
		{
			int valueOrDefault = propertyBag.GetValueOrDefault<int>(InternalSchema.InternetCpid, 0);
			Charset mimeCharset;
			if (ConvertUtils.TryGetValidCharset(valueOrDefault, out mimeCharset))
			{
				return mimeCharset;
			}
			int num = propertyBag.GetValueOrDefault<int>(InternalSchema.Codepage, 0);
			bool flag = false;
			if (num == 1252)
			{
				num = 28605;
				flag = true;
			}
			if (Charset.TryGetCharset(num, out mimeCharset))
			{
				if (!flag)
				{
					mimeCharset = mimeCharset.Culture.MimeCharset;
				}
				if (mimeCharset != null && mimeCharset.IsAvailable)
				{
					return mimeCharset;
				}
			}
			return Culture.Default.MimeCharset;
		}

		internal static Charset GetItemWindowsCharset(Item item)
		{
			return ConvertUtils.GetItemWindowsCharset(item, null);
		}

		public static Charset GetItemWindowsCharset(Item item, OutboundConversionOptions options)
		{
			int valueOrDefault = item.GetValueOrDefault<int>(InternalSchema.InternetCpid, 0);
			Charset charset;
			if (Charset.TryGetCharset(valueOrDefault, out charset))
			{
				charset = ConvertUtils.MapItemWindowsCharset(charset);
				if (charset != null && charset.IsAvailable)
				{
					return charset;
				}
			}
			int valueOrDefault2 = item.GetValueOrDefault<int>(InternalSchema.Codepage, 0);
			if (ConvertUtils.TryGetValidCharset(valueOrDefault2, out charset))
			{
				return charset;
			}
			if (options != null && options.DetectionOptions.PreferredCharset != null)
			{
				charset = ConvertUtils.MapItemWindowsCharset(options.DetectionOptions.PreferredCharset);
				if (charset != null && charset.IsAvailable)
				{
					return charset;
				}
			}
			return Charset.DefaultWindowsCharset;
		}

		public static Charset MapItemWindowsCharset(Charset charset)
		{
			Charset windowsCharset = charset.Culture.WindowsCharset;
			if (windowsCharset.CodePage == 1200)
			{
				return Charset.DefaultWindowsCharset;
			}
			return windowsCharset;
		}

		public static int GetSystemDefaultCodepage()
		{
			return CodePageMap.GetWindowsCodePage(ConvertUtils.GetSystemDefaultEncoding());
		}

		public static Charset UnicodeCharset
		{
			get
			{
				if (ConvertUtils.unicodeCharset == null)
				{
					Charset charset;
					Charset.TryGetCharset(1200, out charset);
					return ConvertUtils.unicodeCharset = charset;
				}
				return ConvertUtils.unicodeCharset;
			}
		}

		public static Encoding GetSystemDefaultEncoding()
		{
			Encoding result = null;
			Culture.Default.MimeCharset.TryGetEncoding(out result);
			return result;
		}

		internal static CultureInfo GetItemCultureInfo(Item item)
		{
			if (item.Session != null)
			{
				return item.Session.InternalCulture;
			}
			return CultureInfo.CurrentCulture;
		}

		internal static string GetFailedInboundConversionsDirectory(string logDirectoryPath)
		{
			return ConvertUtils.GetConversionsDirectory(logDirectoryPath, "ContentConversionTracing\\InboundFailures\\", true);
		}

		internal static string GetFailedOutboundConversionsDirectory(string logDirectoryPath)
		{
			return ConvertUtils.GetConversionsDirectory(logDirectoryPath, "ContentConversionTracing\\OutboundFailures\\", true);
		}

		internal static string ExchangeSetupPath
		{
			get
			{
				string result = null;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
				{
					if (registryKey != null)
					{
						result = (registryKey.GetValue("MsiInstallPath") as string);
					}
				}
				return result;
			}
		}

		internal static string GetOleConversionsDirectory(string subdir, bool checkIfFull)
		{
			string exchangeSetupPath = ConvertUtils.ExchangeSetupPath;
			if (exchangeSetupPath != null)
			{
				return ConvertUtils.GetConversionsDirectory(exchangeSetupPath, subdir, checkIfFull);
			}
			return null;
		}

		internal static string GetConversionsDirectory(string parentDirectory, string subdir, bool checkIfFull)
		{
			if (parentDirectory != null && Directory.Exists(parentDirectory))
			{
				try
				{
					string text = Path.Combine(parentDirectory, subdir);
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					else if (checkIfFull)
					{
						DirectoryInfo directoryInfo = new DirectoryInfo(text);
						FileInfo[] files = directoryInfo.GetFiles();
						long num = 0L;
						foreach (FileInfo fileInfo in files)
						{
							num += fileInfo.Length;
						}
						if (num > StorageLimits.Instance.ConversionsFolderMaxTotalMessageSize)
						{
							text = null;
						}
					}
					return text;
				}
				catch (UnauthorizedAccessException arg)
				{
					ExTraceGlobals.CcGenericTracer.TraceError<UnauthorizedAccessException>(0L, "Exception hit when accessing ContentConversion trace logging directory: {0}", arg);
				}
				catch (AccessDeniedException arg2)
				{
					ExTraceGlobals.CcGenericTracer.TraceError<AccessDeniedException>(0L, "Exception hit when accessing ContentConversion trace logging directory: {0}", arg2);
				}
				catch (IOException arg3)
				{
					ExTraceGlobals.CcGenericTracer.TraceError<IOException>(0L, "Exception hit when accessing ContentConversion trace logging directory: {0}", arg3);
				}
			}
			return null;
		}

		private static bool TryGetHexVal(char ch, out int value)
		{
			if (ch >= '0' && ch <= '9')
			{
				value = Convert.ToInt32((int)(ch - '0'));
				return true;
			}
			ch = char.ToUpperInvariant(ch);
			if (ch >= 'A' && ch <= 'F')
			{
				value = Convert.ToInt32((int)(ch - 'A')) + 10;
				return true;
			}
			value = 0;
			return false;
		}

		internal static bool TryDecodeHexByte(string source, int offset, out byte value)
		{
			int num;
			int num2;
			if (ConvertUtils.TryGetHexVal(source[offset], out num) && ConvertUtils.TryGetHexVal(source[offset + 1], out num2))
			{
				value = (byte)(num << 4 | num2);
				return true;
			}
			value = 0;
			return false;
		}

		internal static bool IsString8Property(TnefPropertyType propertyType)
		{
			return propertyType == TnefPropertyType.String8 || propertyType == (TnefPropertyType)4126;
		}

		internal static PropType GetPropertyBaseType(TnefPropertyTag propertyTag)
		{
			return ConvertUtils.GetPropertyType(propertyTag.ValueTnefType);
		}

		internal static PropType GetPropertyType(TnefPropertyType type)
		{
			return (PropType)type;
		}

		internal static bool IsSmimeMessage(ICoreItem coreItem, out bool isMultipartSigned, out bool isOpaqueSigned, out StreamAttachment attachment)
		{
			isMultipartSigned = false;
			isOpaqueSigned = false;
			attachment = null;
			string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (!ObjectClass.IsSmime(valueOrDefault))
			{
				return false;
			}
			IList<AttachmentHandle> allHandles = coreItem.AttachmentCollection.GetAllHandles();
			if (allHandles.Count != 1)
			{
				return false;
			}
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreAttachment coreAttachment = coreItem.AttachmentCollection.Open(allHandles[0]);
				disposeGuard.Add<CoreAttachment>(coreAttachment);
				StreamAttachment streamAttachment = (StreamAttachment)AttachmentCollection.CreateTypedAttachment(coreAttachment, new AttachmentType?(AttachmentType.Stream));
				if (streamAttachment == null)
				{
					return false;
				}
				disposeGuard.Add<StreamAttachment>(streamAttachment);
				if (ObjectClass.IsSmimeClearSigned(valueOrDefault))
				{
					isMultipartSigned = ConvertUtils.IsMessageMultipartSigned(coreItem, streamAttachment);
					if (isMultipartSigned)
					{
						attachment = streamAttachment;
						disposeGuard.Success();
						return true;
					}
				}
				isOpaqueSigned = ConvertUtils.IsMessageOpaqueSigned(coreItem, streamAttachment);
				if (isOpaqueSigned)
				{
					attachment = streamAttachment;
					disposeGuard.Success();
					return true;
				}
			}
			return false;
		}

		private static bool IsMessageMultipartSigned(ICoreItem coreItem, StreamAttachment attachment)
		{
			string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (!ObjectClass.IsSmimeClearSigned(valueOrDefault))
			{
				return false;
			}
			string contentType = attachment.ContentType;
			return string.Equals(attachment.ContentType, "multipart/signed", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsMessageOpaqueSigned(Item item)
		{
			Util.ThrowOnNullArgument(item, "item");
			ICoreItem coreItem = item.CoreItem;
			string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (!ObjectClass.IsSmime(valueOrDefault))
			{
				return false;
			}
			if (coreItem.AttachmentCollection.Count != 1)
			{
				return false;
			}
			IList<AttachmentHandle> allHandles = coreItem.AttachmentCollection.GetAllHandles();
			bool result;
			using (CoreAttachment coreAttachment = coreItem.AttachmentCollection.Open(allHandles[0]))
			{
				using (StreamAttachment streamAttachment = (StreamAttachment)AttachmentCollection.CreateTypedAttachment(coreAttachment, new AttachmentType?(AttachmentType.Stream)))
				{
					if (streamAttachment == null)
					{
						result = false;
					}
					else
					{
						result = ConvertUtils.IsMessageOpaqueSigned(coreItem, streamAttachment);
					}
				}
			}
			return result;
		}

		private static bool IsMessageOpaqueSigned(ICoreItem coreItem, StreamAttachment attachment)
		{
			Util.ThrowOnNullArgument(coreItem, "coreItem");
			string valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass, string.Empty);
			if (!ObjectClass.IsSmime(valueOrDefault))
			{
				return false;
			}
			coreItem.PropertyBag.Load(new PropertyDefinition[]
			{
				InternalSchema.NamedContentType
			});
			string valueOrDefault2 = coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.NamedContentType);
			if (valueOrDefault2 != null)
			{
				byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(valueOrDefault2);
				ContentTypeHeader header = (ContentTypeHeader)Header.Create(HeaderId.ContentType);
				MimeInternalHelpers.SetHeaderRawValue(header, bytes);
				string headerParameter = MimeHelpers.GetHeaderParameter(header, "smime-type", 100);
				if (headerParameter == null || (!ConvertUtils.MimeStringEquals(headerParameter, "signed-data") && !ConvertUtils.MimeStringEquals(headerParameter, "certs-only")))
				{
					return false;
				}
			}
			string contentType = attachment.ContentType;
			if (string.Compare(contentType, "application/pkcs7-mime", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(contentType, "application/x-pkcs7-mime", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			if (string.Compare(contentType, "application/octet-stream", StringComparison.OrdinalIgnoreCase) == 0)
			{
				string fileName = attachment.FileName;
				string strA = string.Empty;
				if (fileName != null)
				{
					string[] array = fileName.Split(new char[]
					{
						'.'
					});
					if (array.Length > 0)
					{
						strA = array[array.Length - 1];
					}
				}
				return string.Compare(strA, "p7m", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA, "p7c", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA, "p7s", StringComparison.OrdinalIgnoreCase) == 0;
			}
			return false;
		}

		internal static Item OpenAttachedItem(ItemAttachment attachment)
		{
			Item result;
			try
			{
				result = attachment.GetItem(InternalSchema.ContentConversionProperties);
			}
			catch (ObjectNotFoundException arg)
			{
				StorageGlobals.ContextTraceDebug<ObjectNotFoundException>(ExTraceGlobals.CcOutboundGenericTracer, "ConvertUtils::OpenAttachedItem - unable to open embedded message, exc = {0}", arg);
				result = null;
			}
			return result;
		}

		internal static bool IsValidPCL(int pcl)
		{
			return pcl <= 8 && pcl >= 1;
		}

		internal static bool MimeStringEquals(string string1, string string2)
		{
			return string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsRecipientTransmittable(RecipientItemType recipientItemType)
		{
			return recipientItemType == RecipientItemType.To || recipientItemType == RecipientItemType.Cc || recipientItemType == RecipientItemType.Bcc;
		}

		internal static bool IsPropertyTransmittable(NativeStorePropertyDefinition property)
		{
			if (property is NamedPropertyDefinition)
			{
				return true;
			}
			PropertyTagPropertyDefinition propertyTagPropertyDefinition = (PropertyTagPropertyDefinition)property;
			return propertyTagPropertyDefinition.IsTransmittable;
		}

		internal static UnicodeEncoding UnicodeEncoding
		{
			get
			{
				if (ConvertUtils.unicodeEncoding == null)
				{
					ConvertUtils.unicodeEncoding = new UnicodeEncoding(false, false);
				}
				return ConvertUtils.unicodeEncoding;
			}
		}

		public static void SaveDefaultImage(Stream contentStream)
		{
			using (Bitmap bitmap = new Bitmap(1, 1))
			{
				bitmap.Save(contentStream, ImageFormat.Jpeg);
			}
		}

		public static double GetOADate(DateTime dateTime)
		{
			return dateTime.ToOADate();
		}

		public static DateTime GetDateTimeFromOADate(double dateTime)
		{
			return DateTime.FromOADate(dateTime);
		}

		public static bool TryGetImageThumbnail(Stream imageStream, int maxSideInPixels, out byte[] thumbnailByteArray)
		{
			thumbnailByteArray = null;
			Image image;
			try
			{
				image = Image.FromStream(imageStream, true, false);
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.StorageTracer.Information<ArgumentException>(0L, "StreamAttachmentBase::TryGetImageThumbnail. Failed to generate thumbnail due to argument exception {0}", arg);
				return false;
			}
			catch (Exception arg2)
			{
				ExTraceGlobals.StorageTracer.TraceError<Exception>(0L, "StreamAttachmentBase::TryGetImageThumbnail. Failed to generate thumbnail due to exception {0}", arg2);
				return false;
			}
			bool result;
			try
			{
				int height = image.Height;
				int width = image.Width;
				int num = Math.Max(height, width);
				int num2 = Math.Min(height, width);
				if (num2 < maxSideInPixels)
				{
					result = false;
				}
				else
				{
					bool flag = height == num;
					int thumbWidth;
					int thumbHeight;
					if (flag)
					{
						thumbWidth = maxSideInPixels;
						thumbHeight = num / num2 * maxSideInPixels;
					}
					else
					{
						thumbHeight = maxSideInPixels;
						thumbWidth = num / num2 * maxSideInPixels;
					}
					using (MemoryStream memoryStream = new MemoryStream())
					{
						try
						{
							using (Image thumbnailImage = image.GetThumbnailImage(thumbWidth, thumbHeight, () => false, IntPtr.Zero))
							{
								thumbnailImage.Save(memoryStream, ImageFormat.Jpeg);
							}
							thumbnailByteArray = memoryStream.ToArray();
							result = true;
						}
						catch (Exception arg3)
						{
							ExTraceGlobals.StorageTracer.TraceError<Exception>(0L, "StreamAttachmentBase::TryGetImageThumbnail. Failed to generate thumbnail due to exception {0}", arg3);
							result = false;
						}
					}
				}
			}
			finally
			{
				if (image != null)
				{
					image.Dispose();
				}
			}
			return result;
		}

		public static InboundConversionOptions GetInboundConversionOptions()
		{
			return new InboundConversionOptions(new EmptyRecipientCache());
		}

		public static DateTime GetDateTimeFromXml(string date)
		{
			return XmlConvert.ToDateTime(date, XmlDateTimeSerializationMode.Utc);
		}

		public static string GetXmlFromDateTime(DateTime date)
		{
			return XmlConvert.ToString(date, XmlDateTimeSerializationMode.Utc);
		}

		internal const int CodePageInvalid = 0;

		internal const int CodePageShiftJIS = 932;

		internal const int CodePageGb2312 = 936;

		internal const int CodePageKSC5601 = 949;

		internal const int CodePageBig5 = 950;

		internal const int CodePageUnicode = 1200;

		internal const int CodePageUnicodeFEFF = 1201;

		internal const int CodePageWindowsAnsi = 1252;

		internal const int CodePageUsAscii = 20127;

		internal const int CodePageIso88591 = 28591;

		internal const int CodePageIso88956 = 28596;

		internal const int CodePageIso885915 = 28605;

		internal const int CodePageIso2022Jp = 50220;

		internal const int CodePageEscJp = 50221;

		internal const int CodePageSioJp = 50222;

		internal const int CodePageIso2022Kr = 50225;

		internal const int CodePageIso2022Chs = 50227;

		internal const int CodePageIso2022Cht = 50229;

		internal const int CodePageAutodetectSJis = 50932;

		internal const int CodePageAutodetectChs = 50936;

		internal const int CodePageAutodetectKr = 50949;

		internal const int CodePageAutodetectCht = 50950;

		internal const int CodePageAutodetectArabic = 51256;

		internal const int CodePageEucJp = 51932;

		internal const int CodePageEucChs = 51936;

		internal const int CodePageEucKr = 51949;

		internal const int CodePageEucCht = 51950;

		internal const int CodePageHzChs = 52936;

		internal const int CodePageGb18030 = 54936;

		internal const int CodePageUtf7 = 65000;

		internal const int CodePageUtf8 = 65001;

		private const string TargetDirDataKeyPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		private const string TargetDirDataKeyName = "MsiInstallPath";

		private const string InboundFailedConversionsSubdir = "ContentConversionTracing\\InboundFailures\\";

		private const string OutboundFailedConversionsSubdir = "ContentConversionTracing\\OutboundFailures\\";

		private const string CodepagePrefix = "InternalXsoCodepage-";

		private const string CodePage = "Codepage";

		internal const uint PropertyTypeMask = 4095U;

		internal const int MaxAttachmentDataCacheSize = 4096;

		internal const int MinimalCtsCoverterBufferSize = 1024;

		private const int TnefPriorityHigh = 3;

		private const int TnefPriorityNormal = 2;

		private const int TnefPriorityLow = 1;

		private const int MapiPriorityHigh = 1;

		private const int MapiPriorityNormal = 0;

		private const int MapiPriorityLow = -1;

		private static Charset unicodeCharset = null;

		private static UnicodeEncoding unicodeEncoding = null;

		internal static byte[] OidMacBinary = new byte[]
		{
			42,
			134,
			72,
			134,
			247,
			20,
			3,
			11,
			1
		};

		internal delegate void CtsCall();

		internal delegate T CtsCallWithReturnValue<T>();
	}
}
