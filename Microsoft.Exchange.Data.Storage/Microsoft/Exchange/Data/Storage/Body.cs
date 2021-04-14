using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.TextConverters.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Body : IBody
	{
		internal Body(ICoreItem coreItem)
		{
			Util.ThrowOnNullArgument(coreItem, "coreItem");
			this.coreItem = coreItem;
			this.bodyReadStreams = new List<Body.IBodyStream>(1);
			this.ForceRedetectHtmlBodyCharset = false;
		}

		public string Charset
		{
			get
			{
				string result = string.Empty;
				switch (this.CheckBody())
				{
				case BodyFormat.TextPlain:
				case BodyFormat.TextHtml:
				case BodyFormat.ApplicationRtf:
					result = ConvertUtils.GetItemMimeCharset(this.coreItem.PropertyBag).Name;
					break;
				}
				return result;
			}
		}

		public Uri ContentBase
		{
			get
			{
				string valueOrDefault = this.coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.BodyContentBase, null);
				Uri result = null;
				if (Uri.TryCreate(valueOrDefault, UriKind.Absolute, out result))
				{
					return result;
				}
				return null;
			}
		}

		public Uri ContentLocation
		{
			get
			{
				string valueOrDefault = this.coreItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.BodyContentLocation, null);
				Uri result = null;
				if (Uri.TryCreate(valueOrDefault, UriKind.RelativeOrAbsolute, out result))
				{
					return result;
				}
				return null;
			}
		}

		public BodyFormat Format
		{
			get
			{
				return this.CheckBody();
			}
		}

		public string PreviewText
		{
			get
			{
				try
				{
					if (this.cachedPreviewText == null)
					{
						BodyReadConfiguration configuration = new BodyReadConfiguration(BodyFormat.TextPlain)
						{
							ShouldOutputAnchorLinks = false,
							ShouldOutputImageLinks = false
						};
						using (TextReader textReader = this.OpenTextReader(configuration))
						{
							this.cachedPreviewText = new string(Util.StreamHandler.ReadCharBuffer(textReader, 255));
						}
					}
				}
				catch (ConversionFailedException arg)
				{
					ExTraceGlobals.CcBodyTracer.TraceError<ConversionFailedException>((long)this.GetHashCode(), "Body.PreviewText: throwing {0}", arg);
					this.cachedPreviewText = string.Empty;
				}
				return this.cachedPreviewText;
			}
		}

		public long Size
		{
			get
			{
				long result = 0L;
				this.ChooseBestBody();
				BodyFormat rawFormat = this.RawFormat;
				if (this.noBody)
				{
					return result;
				}
				StorePropertyDefinition propertyDefinition = null;
				try
				{
					switch (rawFormat)
					{
					case BodyFormat.TextPlain:
						propertyDefinition = InternalSchema.TextBody;
						break;
					case BodyFormat.TextHtml:
						propertyDefinition = InternalSchema.HtmlBody;
						break;
					case BodyFormat.ApplicationRtf:
						propertyDefinition = InternalSchema.RtfBody;
						break;
					}
					using (Stream stream = this.coreItem.PropertyBag.OpenPropertyStream(propertyDefinition, PropertyOpenMode.ReadOnly))
					{
						result = stream.Length;
					}
				}
				catch (ObjectNotFoundException)
				{
				}
				return result;
			}
		}

		public bool IsBodyChanged
		{
			get
			{
				return this.isBodyChanged;
			}
		}

		public bool IsPreviewInvalid
		{
			get
			{
				return this.isPreviewInvalid;
			}
		}

		public bool ForceRedetectHtmlBodyCharset { get; set; }

		public void NotifyPreviewNeedsUpdated()
		{
			this.isPreviewInvalid = true;
			this.cachedPreviewText = null;
			this.ResetBodyFormat();
		}

		internal Charset RawCharset
		{
			get
			{
				switch (this.RawFormat)
				{
				case BodyFormat.TextPlain:
					return ConvertUtils.UnicodeCharset;
				case BodyFormat.TextHtml:
				case BodyFormat.ApplicationRtf:
					return ConvertUtils.GetItemMimeCharset(this.coreItem.PropertyBag);
				default:
					return ConvertUtils.UnicodeCharset;
				}
			}
		}

		public bool IsBodyDefined
		{
			get
			{
				this.CheckBody();
				return !this.noBody;
			}
		}

		internal BodyFormat RawFormat
		{
			get
			{
				this.CalculateRawFormat();
				return (BodyFormat)this.rawBodyFormat;
			}
		}

		internal bool IsRtfEmbeddedBody
		{
			get
			{
				this.CheckBody();
				return this.isEmbeddedPlainText;
			}
		}

		public static void CopyBody(Item source, Item target, bool disableCharsetDetection = false)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			bool removeMungageData = target is CalendarItemBase;
			Body.CopyBody(source.Body, target.Body, source.Session.PreferedCulture, removeMungageData, disableCharsetDetection);
		}

		public static void CopyBody(Body source, Body target, CultureInfo cultureInfo, bool removeMungageData, bool disableCharsetDetection = false)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			Body.InternalCopyBody(source, target, cultureInfo, removeMungageData, null, BodyInjectionFormat.Text, disableCharsetDetection);
		}

		public void CopyBodyInjectingText(IBody targetBody, BodyInjectionFormat injectionFormat, string prefixInjectionText, string suffixInjectionText)
		{
			if (string.IsNullOrEmpty(prefixInjectionText) && string.IsNullOrEmpty(suffixInjectionText))
			{
				return;
			}
			BodyFormat bodyFormat = this.Format;
			if (bodyFormat == BodyFormat.ApplicationRtf)
			{
				bodyFormat = BodyFormat.TextHtml;
			}
			using (Stream stream = this.OpenReadStream(new BodyReadConfiguration(bodyFormat, this.Charset)))
			{
				BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(bodyFormat, this.Charset);
				bodyWriteConfiguration.SetTargetFormat(this.Format, this.Charset);
				bodyWriteConfiguration.AddInjectedText(prefixInjectionText, suffixInjectionText, injectionFormat);
				using (Stream stream2 = targetBody.OpenWriteStream(bodyWriteConfiguration))
				{
					Util.StreamHandler.CopyStreamData(stream, stream2, null, 0, 65536);
				}
			}
		}

		public int GetLastNBytesAsString(int lastNBytesToRead, out string readString)
		{
			int result;
			using (Stream stream = BodyReadStream.OpenBodyStream(this.coreItem))
			{
				readString = null;
				byte[] bytes;
				int num = Util.StreamHandler.ReadLastBytesOfStream(stream, lastNBytesToRead, out bytes);
				if (num > 0)
				{
					readString = this.GetBodyEncoding().GetString(bytes, 0, num);
				}
				result = num;
			}
			return result;
		}

		public Stream OpenReadStream(BodyReadConfiguration configuration)
		{
			return this.InternalOpenReadStream(configuration, true);
		}

		public TextReader OpenTextReader(BodyFormat bodyFormat)
		{
			BodyReadConfiguration configuration = new BodyReadConfiguration(bodyFormat);
			return this.OpenTextReader(configuration);
		}

		public TextReader OpenTextReader(BodyReadConfiguration configuration)
		{
			Body.CheckNull(configuration, "configuration");
			this.CheckStreamingExceptions();
			TextReader result;
			lock (this.bodyStreamsLock)
			{
				this.CheckOpenBodyStreamForRead();
				BodyTextReader bodyTextReader = new BodyTextReader(this.coreItem, configuration, null);
				this.bodyReadStreams.Add(bodyTextReader);
				result = bodyTextReader;
			}
			return result;
		}

		public Stream OpenWriteStream(BodyWriteConfiguration configuration)
		{
			Body.CheckNull(configuration, "configuration");
			if ((configuration.SourceFormat == BodyFormat.TextPlain || configuration.SourceFormat == BodyFormat.TextHtml) && configuration.SourceCharset == null)
			{
				throw new InvalidOperationException("Body.OpenWriteStream - source charset is undefined");
			}
			return this.InternalOpenWriteStream(configuration, null);
		}

		public TextWriter OpenTextWriter(BodyFormat bodyFormat)
		{
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(bodyFormat);
			return this.OpenTextWriter(configuration);
		}

		public TextWriter OpenTextWriter(BodyWriteConfiguration configuration)
		{
			Body.CheckNull(configuration, "configuration");
			BodyTextWriter result;
			lock (this.bodyStreamsLock)
			{
				this.CheckOpenBodyStreamForWrite();
				result = new BodyTextWriter(this.coreItem, configuration, null);
				this.bodyWriteStream = result;
			}
			this.BodyChanged(configuration);
			this.bodyStreamingException = null;
			this.isBodyChanged = true;
			this.cachedPreviewText = null;
			return result;
		}

		public byte[] CalculateBodyTag()
		{
			int num;
			return this.CalculateBodyTag(out num);
		}

		public byte[] CalculateBodyTag(out int latestMessagePartWordCount)
		{
			latestMessagePartWordCount = int.MinValue;
			if (this.IsBodyDefined && this.Size / 2048L <= 2048L)
			{
				try
				{
					ConversationBodyScanner conversationBodyScanner = this.GetConversationBodyScanner();
					latestMessagePartWordCount = conversationBodyScanner.CalculateLatestMessagePartWordCount();
					BodyFragmentInfo bodyFragmentInfo = new BodyFragmentInfo(conversationBodyScanner);
					return bodyFragmentInfo.BodyTag.ToByteArray();
				}
				catch (TextConvertersException)
				{
					return new byte[Body.BodyTagLength];
				}
			}
			if (ObjectClass.IsSmime(this.coreItem.ClassName()) && !ObjectClass.IsSmimeClearSigned(this.coreItem.ClassName()))
			{
				Item item = null;
				try
				{
					InboundConversionOptions inboundConversionOptions = ConvertUtils.GetInboundConversionOptions();
					if (ItemConversion.TryOpenSMimeContent(this.coreItem, inboundConversionOptions, out item))
					{
						return item.Body.CalculateBodyTag(out latestMessagePartWordCount);
					}
				}
				finally
				{
					if (item != null)
					{
						item.Dispose();
					}
				}
			}
			return new byte[12];
		}

		public void ResetBodyFormat()
		{
			this.bodyFormat = -1;
			this.rawBodyFormat = -1;
			this.bodyFormatDecision = -1;
			this.coreItem.CharsetDetector.ResetCachedBody();
		}

		private static void InternalCopyBody(Body source, Body target, CultureInfo cultureInfo, bool removeMungageData, string prefix, BodyInjectionFormat prefixFormat, bool disableCharsetDetection = false)
		{
			BodyReadConfiguration bodyReadConfiguration = new BodyReadConfiguration(source.RawFormat, source.RawCharset.Name);
			BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(source.RawFormat, source.RawCharset.Name);
			if (disableCharsetDetection)
			{
				bodyWriteConfiguration.SetTargetFormat(source.RawFormat, source.Charset, BodyCharsetFlags.DisableCharsetDetection);
			}
			else
			{
				bodyWriteConfiguration.SetTargetFormat(source.RawFormat, source.Charset);
			}
			if (!string.IsNullOrEmpty(prefix))
			{
				bodyWriteConfiguration.AddInjectedText(prefix, null, prefixFormat);
			}
			bool flag = false;
			if (removeMungageData)
			{
				flag = Body.CopyBodyWithoutMungage(source, target, cultureInfo, bodyReadConfiguration, bodyWriteConfiguration);
			}
			if (!flag)
			{
				using (Stream stream = source.OpenReadStream(bodyReadConfiguration))
				{
					using (Stream stream2 = target.OpenWriteStream(bodyWriteConfiguration))
					{
						Util.StreamHandler.CopyStreamData(stream, stream2, null, 0, 16384);
					}
				}
			}
		}

		internal static Stream GetEmptyStream()
		{
			byte[] buffer = new byte[1];
			return new MemoryStream(buffer, 0, 0, false);
		}

		internal static bool IsUtfCpid(int codepage)
		{
			return codepage == 1200 || codepage == 1201 || codepage == 65000 || codepage == 65001 || codepage == 54936;
		}

		internal static StorePropertyDefinition GetBodyProperty(BodyFormat bodyFormat)
		{
			switch (bodyFormat)
			{
			case BodyFormat.TextPlain:
				return InternalSchema.TextBody;
			case BodyFormat.TextHtml:
				return InternalSchema.HtmlBody;
			case BodyFormat.ApplicationRtf:
				return InternalSchema.RtfBody;
			default:
				throw new ArgumentOutOfRangeException("bodyFormat", string.Format("Invalid body format: {0}.", bodyFormat));
			}
		}

		internal static int ReadChars(TextReader reader, char[] buffer, int length)
		{
			int num;
			int num2;
			for (num = 0; num != length; num += num2)
			{
				num2 = reader.Read(buffer, num, length - num);
				if (num2 == 0)
				{
					break;
				}
			}
			return num;
		}

		internal void Reset()
		{
			this.ResetBodyFormat();
			this.isBodyChanged = false;
			this.isPreviewInvalid = false;
		}

		internal Stream TryOpenReadStream(BodyReadConfiguration configuration)
		{
			return this.InternalOpenReadStream(configuration, false);
		}

		internal ConversationBodyTextReader OpenConversationTextReader(BodyReadConfiguration configuration, long bytesLoadedForConversation, long maxBytesForConversation)
		{
			Body.CheckNull(configuration, "configuration");
			this.CheckStreamingExceptions();
			ConversationBodyTextReader result;
			lock (this.bodyStreamsLock)
			{
				this.CheckOpenBodyStreamForRead();
				ConversationBodyTextReader conversationBodyTextReader = new ConversationBodyTextReader(this.coreItem, configuration, null, bytesLoadedForConversation, maxBytesForConversation);
				this.bodyReadStreams.Add(conversationBodyTextReader);
				result = conversationBodyTextReader;
			}
			return result;
		}

		internal Stream InternalOpenWriteStream(BodyWriteConfiguration configuration, Stream outputStream)
		{
			Stream result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				BodyWriteStream bodyWriteStream;
				lock (this.bodyStreamsLock)
				{
					this.CheckOpenBodyStreamForWrite();
					bodyWriteStream = new BodyWriteStream(this.coreItem, configuration, outputStream);
					disposeGuard.Add<BodyWriteStream>(bodyWriteStream);
					this.bodyWriteStream = bodyWriteStream;
				}
				this.BodyChanged(configuration);
				this.bodyStreamingException = null;
				this.isBodyChanged = true;
				this.cachedPreviewText = null;
				disposeGuard.Success();
				result = bodyWriteStream;
			}
			return result;
		}

		internal void SetBodyStreamingException(ExchangeDataException exc)
		{
			this.bodyStreamingException = exc;
		}

		internal void CheckStreamingExceptions()
		{
			if (this.bodyStreamingException != null)
			{
				Exception ex = new CorruptDataException(ServerStrings.ConversionBodyCorrupt, this.bodyStreamingException);
				ExTraceGlobals.CcBodyTracer.TraceError<Exception>((long)this.GetHashCode(), "Body.CheckStreamingExceptions: throwing {0}", ex);
				throw ex;
			}
		}

		internal void ValidateBody()
		{
			this.coreItem.Body.CheckStreamingExceptions();
			this.coreItem.CharsetDetector.ValidateItemCharset();
		}

		internal Stream InternalOpenBodyStream(StorePropertyDefinition bodyProperty, PropertyOpenMode openMode)
		{
			if (openMode != PropertyOpenMode.ReadOnly)
			{
				IDirectPropertyBag directPropertyBag = (IDirectPropertyBag)this.coreItem.PropertyBag;
				directPropertyBag.SetValue(InternalSchema.TextBody, Body.TextNotFoundPropertyError);
				directPropertyBag.SetValue(InternalSchema.HtmlBody, Body.HtmlNotFoundPropertyError);
				directPropertyBag.SetValue(InternalSchema.RtfBody, Body.RtfNotFoundPropertyError);
			}
			return this.coreItem.PropertyBag.OpenPropertyStream(bodyProperty, openMode);
		}

		internal Encoding GetBodyEncoding()
		{
			Charset itemMimeCharset = ConvertUtils.GetItemMimeCharset(this.coreItem.PropertyBag);
			Encoding result = null;
			itemMimeCharset.TryGetEncoding(out result);
			return result;
		}

		public ConversationBodyScanner GetConversationBodyScanner()
		{
			long num = 0L;
			return this.GetConversationBodyScanner(null, -1L, 0L, false, false, out num);
		}

		public ConversationBodyScanner GetConversationBodyScanner(HtmlCallbackBase callback, long maxBytes, long bytesLoaded, bool fixCharset, bool filterHtml, out long bytesRead)
		{
			ConversationBodyScanner conversationBodyScanner = null;
			StorePropertyDefinition bodyProperty = null;
			BodyFormat bodyFormat;
			if (fixCharset)
			{
				bodyFormat = this.RawFormat;
			}
			else
			{
				bodyFormat = this.Format;
			}
			switch (bodyFormat)
			{
			case BodyFormat.TextPlain:
			{
				TextConversationBodyScanner textConversationBodyScanner = new TextConversationBodyScanner();
				conversationBodyScanner = textConversationBodyScanner;
				if (fixCharset)
				{
					textConversationBodyScanner.InputEncoding = ConvertUtils.UnicodeEncoding;
				}
				else
				{
					textConversationBodyScanner.InputEncoding = this.GetBodyEncoding();
				}
				if (this.IsRtfEmbeddedBody)
				{
					bodyProperty = ItemSchema.RtfBody;
				}
				else
				{
					bodyProperty = ItemSchema.TextBody;
				}
				break;
			}
			case BodyFormat.TextHtml:
			{
				HtmlConversationBodyScanner htmlConversationBodyScanner = new HtmlConversationBodyScanner();
				conversationBodyScanner = htmlConversationBodyScanner;
				htmlConversationBodyScanner.InputEncoding = this.GetBodyEncoding();
				htmlConversationBodyScanner.DetectEncodingFromMetaTag = false;
				bodyProperty = ItemSchema.HtmlBody;
				break;
			}
			case BodyFormat.ApplicationRtf:
				conversationBodyScanner = new RtfConversationBodyScanner();
				bodyProperty = ItemSchema.RtfBody;
				break;
			}
			conversationBodyScanner.FilterHtml = filterHtml;
			if (callback != null)
			{
				conversationBodyScanner.HtmlCallback = new HtmlTagCallback(callback.ProcessTag);
			}
			if (this.IsBodyDefined)
			{
				using (Stream stream = this.InternalOpenBodyStream(bodyProperty, PropertyOpenMode.ReadOnly))
				{
					bytesRead = stream.Length;
					if (maxBytes > -1L && bytesRead + bytesLoaded > maxBytes)
					{
						throw new MessageLoadFailedInConversationException(new LocalizedString("Message body size exceeded the conversation threshold for loading"));
					}
					if (this.RawFormat == BodyFormat.ApplicationRtf)
					{
						using (Stream stream2 = new ConverterStream(stream, new RtfCompressedToRtf(), ConverterStreamAccess.Read))
						{
							conversationBodyScanner.Load(stream2);
							goto IL_12F;
						}
					}
					conversationBodyScanner.Load(stream);
					IL_12F:
					return conversationBodyScanner;
				}
			}
			bytesRead = 0L;
			MemoryStream sourceStream = new MemoryStream(0);
			conversationBodyScanner.Load(sourceStream);
			return conversationBodyScanner;
		}

		private static bool CopyBodyWithoutMungage(Body source, Body target, CultureInfo cultureInfo, BodyReadConfiguration readConfiguration, BodyWriteConfiguration writeConfiguration)
		{
			bool result = false;
			switch (source.RawFormat)
			{
			case BodyFormat.TextPlain:
			{
				bool flag = true;
				goto IL_2B;
			}
			case BodyFormat.ApplicationRtf:
			{
				bool flag = false;
				goto IL_2B;
			}
			}
			return false;
			IL_2B:
			using (Stream stream = source.OpenReadStream(readConfiguration))
			{
				Stream stream2 = null;
				try
				{
					bool flag;
					stream2 = (flag ? stream : new ConverterStream(stream, new RtfCompressedToRtf(), ConverterStreamAccess.Read));
					byte[] array = new byte[16384];
					int num = stream2.Read(array, 0, array.Length);
					if (num > 0)
					{
						string @string = readConfiguration.Encoding.GetString(array, 0, num);
						string pattern = string.Format("{0}[\\s\\S]+(^.*{1}[\\s\\S]+)?^.*\\*~\\*~\\*~\\*~\\*~\\*~\\*~\\*~\\*~\\*(\\\\line)?\\r\\n", Regex.Escape(ClientStrings.WhenPart.ToString(cultureInfo)), Regex.Escape(ClientStrings.WherePart.ToString(cultureInfo)));
						Match match = Regex.Match(@string, pattern, RegexOptions.Multiline);
						if (match.Success)
						{
							string s = @string.Remove(match.Index, match.Length);
							byte[] bytes = writeConfiguration.SourceEncoding.GetBytes(s);
							using (Stream stream3 = target.OpenWriteStream(writeConfiguration))
							{
								Stream stream4 = null;
								try
								{
									stream4 = (flag ? stream3 : new ConverterStream(stream3, new RtfToRtfCompressed(), ConverterStreamAccess.Write));
									stream4.Write(bytes, 0, bytes.Length);
									if (num == array.Length)
									{
										Util.StreamHandler.CopyStreamData(stream2, stream4, null, 0, 16384);
									}
									result = true;
								}
								finally
								{
									if (!flag && stream4 != null)
									{
										stream4.Dispose();
									}
								}
							}
						}
					}
				}
				finally
				{
					bool flag;
					if (!flag && stream2 != null)
					{
						stream2.Dispose();
					}
				}
			}
			return result;
		}

		private static void CheckNull(object arg, string argName)
		{
			if (arg == null)
			{
				throw new ArgumentNullException(argName);
			}
		}

		private bool IsAnyBodyPropDirty()
		{
			foreach (StorePropertyDefinition propertyDefinition in Body.BodyProps)
			{
				if (this.coreItem.PropertyBag.IsPropertyDirty(propertyDefinition))
				{
					return true;
				}
			}
			return false;
		}

		private void CheckOpenBodyStreamForRead()
		{
			if (this.bodyWriteStream != null && !this.bodyWriteStream.IsDisposed())
			{
				throw new NoSupportException(ServerStrings.ExTooManyObjects("BodyConversionStream", 1, 1));
			}
			if (this.bodyReadStreams.Count > 0)
			{
				this.bodyReadStreams = (from x in this.bodyReadStreams
				where x != null && !x.IsDisposed()
				select x).ToList<Body.IBodyStream>();
			}
		}

		private void CheckOpenBodyStreamForWrite()
		{
			if (this.bodyWriteStream != null && !this.bodyWriteStream.IsDisposed())
			{
				throw new NoSupportException(ServerStrings.ExTooManyObjects("BodyConversionStream", 1, 1));
			}
			foreach (Body.IBodyStream bodyStream in this.bodyReadStreams)
			{
				if (bodyStream != null && !bodyStream.IsDisposed())
				{
					throw new NoSupportException(ServerStrings.ExTooManyObjects("BodyConversionStream", 1, 1));
				}
			}
			this.bodyReadStreams.Clear();
		}

		private void BodyChanged(BodyWriteConfiguration configuration)
		{
			if (this.coreItem.Schema is CalendarItemBaseSchema)
			{
				this.coreItem.LocationIdentifierHelperInstance.SetLocationIdentifier(65525U, LastChangeAction.SetBody);
			}
			if (configuration.TargetFormat == BodyFormat.ApplicationRtf && configuration.SourceFormat == BodyFormat.TextPlain && string.IsNullOrEmpty(configuration.InjectPrefix) && string.IsNullOrEmpty(configuration.InjectSuffix))
			{
				this.bodyFormat = 1;
				this.rawBodyFormat = 3;
				this.isEmbeddedPlainText = true;
			}
			else
			{
				this.bodyFormat = (int)configuration.TargetFormat;
				this.rawBodyFormat = (int)configuration.TargetFormat;
				this.isEmbeddedPlainText = false;
			}
			this.noBody = false;
		}

		private Stream InternalOpenReadStream(BodyReadConfiguration configuration, bool createEmptyStreamIfNotFound)
		{
			Body.CheckNull(configuration, "configuration");
			this.CheckStreamingExceptions();
			Stream result;
			lock (this.bodyStreamsLock)
			{
				this.CheckOpenBodyStreamForRead();
				BodyReadStream bodyReadStream = BodyReadStream.TryCreateBodyReadStream(this.coreItem, configuration, createEmptyStreamIfNotFound);
				this.bodyReadStreams.Add(bodyReadStream);
				result = bodyReadStream;
			}
			return result;
		}

		private BodyFormat CheckBody()
		{
			this.ChooseBestBody();
			return (BodyFormat)this.bodyFormat;
		}

		private void CalculateRawFormat()
		{
			if (this.rawBodyFormat != -1)
			{
				return;
			}
			BodyFormat bodyFormat = BodyFormat.TextPlain;
			bool flag = false;
			bool flag2 = false;
			BodyFormat bodyFormat2 = BodyFormat.TextPlain;
			bool flag3 = false;
			bool flag4 = false;
			int num = 0;
			object obj = this.coreItem.PropertyBag.TryGetProperty(InternalSchema.TextBody);
			object obj2 = this.coreItem.PropertyBag.TryGetProperty(InternalSchema.HtmlBody);
			object obj3 = this.coreItem.PropertyBag.TryGetProperty(InternalSchema.RtfBody);
			object obj4 = this.coreItem.PropertyBag.TryGetProperty(InternalSchema.RtfInSync);
			PropertyErrorCode propertyErrorCode = (PropertyErrorCode)(-1);
			PropertyError propertyError;
			if ((propertyError = (obj as PropertyError)) != null)
			{
				propertyErrorCode = ((propertyError.PropertyErrorCode == PropertyErrorCode.RequireStreamed) ? PropertyErrorCode.NotEnoughMemory : propertyError.PropertyErrorCode);
			}
			PropertyErrorCode propertyErrorCode2 = (PropertyErrorCode)(-1);
			if ((propertyError = (obj2 as PropertyError)) != null)
			{
				propertyErrorCode2 = ((propertyError.PropertyErrorCode == PropertyErrorCode.RequireStreamed) ? PropertyErrorCode.NotEnoughMemory : propertyError.PropertyErrorCode);
			}
			PropertyErrorCode propertyErrorCode3 = (PropertyErrorCode)(-1);
			if ((propertyError = (obj3 as PropertyError)) != null)
			{
				propertyErrorCode3 = ((propertyError.PropertyErrorCode == PropertyErrorCode.RequireStreamed) ? PropertyErrorCode.NotEnoughMemory : propertyError.PropertyErrorCode);
			}
			bool flag5 = false;
			if (!(obj4 is PropertyError))
			{
				flag5 = (bool)obj4;
			}
			if (!this.IsAnyBodyPropDirty())
			{
				object obj5 = this.coreItem.PropertyBag.TryGetProperty(InternalSchema.NativeBodyInfo);
				if (!(obj5 is PropertyError))
				{
					flag2 = true;
					switch ((int)obj5)
					{
					case 0:
						flag = true;
						bodyFormat = BodyFormat.TextPlain;
						num = 1;
						break;
					case 1:
						if (propertyErrorCode == PropertyErrorCode.NotFound)
						{
							flag2 = false;
						}
						else
						{
							bodyFormat = BodyFormat.TextPlain;
							num = 3;
						}
						break;
					case 2:
						if (propertyErrorCode3 == PropertyErrorCode.NotFound)
						{
							flag2 = false;
						}
						else
						{
							bodyFormat = BodyFormat.ApplicationRtf;
							num = 4;
						}
						break;
					case 3:
						if (propertyErrorCode2 == PropertyErrorCode.NotFound)
						{
							flag2 = false;
						}
						else
						{
							bodyFormat = BodyFormat.TextHtml;
							num = 2;
						}
						break;
					default:
						flag2 = false;
						break;
					}
				}
			}
			if (!flag2)
			{
				if (propertyErrorCode == PropertyErrorCode.NotFound && propertyErrorCode2 == PropertyErrorCode.NotFound && propertyErrorCode3 == PropertyErrorCode.NotFound)
				{
					flag3 = true;
					bodyFormat2 = BodyFormat.TextPlain;
					num = 11;
					flag4 = true;
				}
				else if (propertyErrorCode == PropertyErrorCode.NotEnoughMemory && propertyErrorCode2 == PropertyErrorCode.NotFound && propertyErrorCode3 == PropertyErrorCode.NotFound)
				{
					bodyFormat2 = BodyFormat.TextPlain;
					num = 12;
					flag4 = true;
				}
				else if ((propertyErrorCode == PropertyErrorCode.NotEnoughMemory && propertyErrorCode3 == PropertyErrorCode.NotEnoughMemory && propertyErrorCode2 == PropertyErrorCode.NotFound) || (propertyErrorCode == PropertyErrorCode.NotEnoughMemory && propertyErrorCode3 == PropertyErrorCode.NotEnoughMemory && propertyErrorCode2 == PropertyErrorCode.NotEnoughMemory && flag5))
				{
					bodyFormat2 = BodyFormat.ApplicationRtf;
					num = 13;
					flag4 = true;
				}
				else if ((propertyErrorCode == PropertyErrorCode.NotEnoughMemory || propertyErrorCode == (PropertyErrorCode)(-1)) && propertyErrorCode3 == PropertyErrorCode.NotEnoughMemory && propertyErrorCode2 == PropertyErrorCode.NotEnoughMemory && !flag5)
				{
					bodyFormat2 = BodyFormat.TextHtml;
					num = 14;
					flag4 = true;
				}
				if (!flag4)
				{
					if (propertyErrorCode2 == (PropertyErrorCode)(-1) || propertyErrorCode2 == PropertyErrorCode.NotEnoughMemory)
					{
						if ((propertyErrorCode3 == (PropertyErrorCode)(-1) || propertyErrorCode3 == PropertyErrorCode.NotEnoughMemory) && flag5)
						{
							bodyFormat2 = BodyFormat.ApplicationRtf;
							num = 21;
						}
						else
						{
							bodyFormat2 = BodyFormat.TextHtml;
							num = 22;
						}
						flag4 = true;
					}
					else if (propertyErrorCode3 == (PropertyErrorCode)(-1) || propertyErrorCode3 == PropertyErrorCode.NotEnoughMemory)
					{
						if ((propertyErrorCode == (PropertyErrorCode)(-1) || propertyErrorCode == PropertyErrorCode.NotEnoughMemory) && !flag5)
						{
							bodyFormat2 = BodyFormat.TextPlain;
							num = 23;
						}
						else
						{
							bodyFormat2 = BodyFormat.ApplicationRtf;
							num = 24;
						}
						flag4 = true;
					}
					else
					{
						bodyFormat2 = BodyFormat.TextPlain;
						num = 25;
						flag4 = true;
					}
				}
			}
			this.bodyFormatDecision = num;
			if (!flag4)
			{
				this.noBody = flag;
				this.rawBodyFormat = (int)bodyFormat;
			}
			else
			{
				this.noBody = flag3;
				this.rawBodyFormat = (int)bodyFormat2;
			}
			ExTraceGlobals.CcBodyTracer.TraceDebug<int, bool, int>((long)this.GetHashCode(), "Body.CalculateRawFormat: BodyFormat={0}, missing={1}, decision point={2}", this.rawBodyFormat, this.noBody, this.bodyFormatDecision);
		}

		private void ChooseBestBody()
		{
			if (this.bodyFormat != -1)
			{
				return;
			}
			this.CalculateRawFormat();
			this.bodyFormat = this.rawBodyFormat;
			this.isEmbeddedPlainText = false;
			if (this.bodyFormat == 3)
			{
				ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "Body.ChooseBestBody", ServerStrings.ConversionBodyConversionFailed, delegate
				{
					using (Stream stream = this.coreItem.PropertyBag.OpenPropertyStream(InternalSchema.RtfBody, PropertyOpenMode.ReadOnly))
					{
						using (Stream stream2 = new ConverterStream(stream, new RtfCompressedToRtf(), ConverterStreamAccess.Read))
						{
							using (Stream stream3 = TextConvertersInternalHelpers.CreateRtfPreviewStream(stream2, 4096))
							{
								if (TextConvertersInternalHelpers.RtfHasEncapsulatedText(stream3))
								{
									this.bodyFormat = 1;
									this.isEmbeddedPlainText = true;
									this.bodyFormatDecision = 31;
									ExTraceGlobals.CcBodyTracer.TraceDebug<BodyFormat, bool, int>((long)this.GetHashCode(), "Body.ChooseBestBody: BodyFormat={0}, missing={1}, decision point={2}", BodyFormat.TextPlain, false, this.bodyFormatDecision);
								}
							}
						}
					}
				});
			}
		}

		public string GetPartialHtmlBody(int length, HtmlCallbackBase htmlCallbacks, bool filterHtml, int styleSheetLimit)
		{
			BodyReadConfiguration bodyReadConfiguration = new BodyReadConfiguration(BodyFormat.TextHtml);
			bodyReadConfiguration.SetHtmlOptions(filterHtml ? HtmlStreamingFlags.FilterHtml : HtmlStreamingFlags.None, htmlCallbacks, new int?(styleSheetLimit));
			string result;
			using (TextReader textReader = this.OpenTextReader(bodyReadConfiguration))
			{
				char[] array = new char[length];
				int length2 = Body.ReadChars(textReader, array, length);
				result = new string(array, 0, length2);
			}
			return result;
		}

		public byte[] GetPartialRtfCompressedBody(int nBytes)
		{
			BodyReadConfiguration configuration = new BodyReadConfiguration(BodyFormat.ApplicationRtf);
			byte[] result;
			using (Stream stream = this.OpenReadStream(configuration))
			{
				byte[] array = new byte[nBytes];
				int num;
				int num2;
				for (num = 0; num != nBytes; num += num2)
				{
					num2 = stream.Read(array, num, nBytes - num);
					if (num2 == 0)
					{
						break;
					}
				}
				if (num == nBytes)
				{
					result = array;
				}
				else
				{
					byte[] array2 = new byte[num];
					Array.Copy(array, array2, num);
					result = array2;
				}
			}
			return result;
		}

		public string GetPartialTextBody(int length)
		{
			BodyReadConfiguration configuration = new BodyReadConfiguration(BodyFormat.TextPlain);
			string result;
			using (TextReader textReader = this.OpenTextReader(configuration))
			{
				char[] array = new char[length];
				int length2 = Body.ReadChars(textReader, array, length);
				result = new string(array, 0, length2);
			}
			return result;
		}

		internal const string OutlookHeader = "*~*~*~*~*~*~*~*~*~*";

		private const int PreviewSize = 255;

		private const int DefaultEmbeddedRtfPreviewBufferSize = 4096;

		private const int CopyBufferSize = 16384;

		private const int InvalidBodyFormat = -1;

		private const PropertyErrorCode NoPropertyError = (PropertyErrorCode)(-1);

		internal static int BodyTagLength = 12;

		internal static StorePropertyDefinition[] BodyProps = new StorePropertyDefinition[]
		{
			InternalSchema.TextBody,
			InternalSchema.HtmlBody,
			InternalSchema.RtfBody,
			InternalSchema.RtfInSync
		};

		internal static HashSet<StorePropertyDefinition> BodyPropSet = new HashSet<StorePropertyDefinition>(Body.BodyProps);

		private static readonly PropertyError TextNotFoundPropertyError = new PropertyError(InternalSchema.TextBody, PropertyErrorCode.NotFound);

		private static readonly PropertyError HtmlNotFoundPropertyError = new PropertyError(InternalSchema.HtmlBody, PropertyErrorCode.NotFound);

		private static readonly PropertyError RtfNotFoundPropertyError = new PropertyError(InternalSchema.RtfBody, PropertyErrorCode.NotFound);

		private readonly object bodyStreamsLock = new object();

		private ICoreItem coreItem;

		private int bodyFormat = -1;

		private int rawBodyFormat = -1;

		private bool isEmbeddedPlainText;

		private bool noBody;

		private bool isBodyChanged;

		private bool isPreviewInvalid;

		private string cachedPreviewText;

		private int bodyFormatDecision = -1;

		private List<Body.IBodyStream> bodyReadStreams;

		private Body.IBodyStream bodyWriteStream;

		private ExchangeDataException bodyStreamingException;

		internal interface IBodyStream
		{
			bool IsDisposed();
		}
	}
}
