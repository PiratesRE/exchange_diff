using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Encoders;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ItemToMimeConverter : DisposableObject
	{
		internal ItemToMimeConverter(Item item, OutboundConversionOptions options, ConverterFlags flags) : this(item, options, flags, null)
		{
		}

		private ItemToMimeConverter(Item item, OutboundConversionOptions options, ConverterFlags flags, OutboundAddressCache addressCache)
		{
			bool[] array = new bool[2];
			this.journalAttachmentNeedsSave = array;
			bool[] array2 = new bool[2];
			this.journalAttachedItemNeedsSave = array2;
			this.ownsMimeDocuments = true;
			this.ownedEmbeddedMimeDocuments = new List<MimeDocument>();
			base..ctor();
			Util.ThrowOnNullArgument(options, "options");
			Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			if (options.FilterBodyHandler != null)
			{
				throw new NotSupportedException("FilterBodyHandler is not supported in ItemToMimeConverter");
			}
			this.item = item;
			this.addressCache = addressCache;
			this.options = (OutboundConversionOptions)options.Clone();
			this.flags = flags;
			this.itemType = ItemToMimeConverter.GetItemType(this.item);
			this.resolveAddresses = true;
		}

		internal MimeItemType ItemType
		{
			get
			{
				this.CheckDisposed(null);
				return this.itemType;
			}
		}

		internal OutboundAddressCache AddressCache
		{
			get
			{
				this.CheckDisposed(null);
				if (this.addressCache == null)
				{
					this.addressCache = new OutboundAddressCache(this.options, this.limitsTracker);
					this.addressCache.CopyDataFromItem(this.item);
					if (this.resolveAddresses)
					{
						this.addressCache.Resolve();
					}
				}
				return this.addressCache;
			}
		}

		private bool IsJournalingMessage
		{
			get
			{
				return this.itemType == MimeItemType.MimeMessageJournalMsg || this.itemType == MimeItemType.MimeMessageJournalTnef || this.itemType == MimeItemType.MimeMessageSecondaryJournal;
			}
		}

		private MimeDocument Skeleton
		{
			get
			{
				if (!this.skeletonInitialized)
				{
					PropertyError propertyError = this.item.TryGetProperty(InternalSchema.MimeSkeleton) as PropertyError;
					if (propertyError == null || propertyError.PropertyErrorCode != PropertyErrorCode.NotFound)
					{
						using (Stream stream = this.item.OpenPropertyStream(InternalSchema.MimeSkeleton, PropertyOpenMode.ReadOnly))
						{
							using (DisposeGuard disposeGuard = default(DisposeGuard))
							{
								this.skeleton = disposeGuard.Add<MimeDocument>(new MimeDocument());
								this.skeleton.Load(stream, CachingMode.Copy);
								disposeGuard.Success();
							}
						}
					}
					this.skeletonInitialized = true;
				}
				return this.skeleton;
			}
		}

		private MimeDocument SmimeDocument
		{
			get
			{
				if (!this.smimeDocInitialized && this.SmimeAttachment != null)
				{
					this.smimeDocument = new MimeDocument();
					this.smimeDocInitialized = true;
					using (Stream stream = this.SmimeAttachment.TryGetRawContentStream(PropertyOpenMode.ReadOnly))
					{
						if (stream == null)
						{
							StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::SmimeDocument: SMIME attachment is empty.");
							throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, null);
						}
						this.smimeDocument.Load(stream, CachingMode.Copy);
					}
				}
				return this.smimeDocument;
			}
		}

		private Attachment[] JournalAttachments
		{
			get
			{
				if (this.journalAttachments == null)
				{
					if (this.item.AttachmentCollection.Count != 1 && this.item.AttachmentCollection.Count != 2)
					{
						StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::JournalAttachment::get: attachment count not equal to 1 or 2.");
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ExInvalidJournalReportFormat, null);
					}
					this.journalAttachments = new Attachment[this.item.AttachmentCollection.Count];
					int num = 0;
					foreach (AttachmentHandle handle in this.item.AttachmentCollection)
					{
						this.journalAttachments[num] = this.item.AttachmentCollection.Open(handle);
						num++;
					}
				}
				return this.journalAttachments;
			}
		}

		private StreamAttachment SmimeAttachment
		{
			get
			{
				if (this.smimeAttachment == null && (this.ItemType == MimeItemType.MimeMessageSmime || this.ItemType == MimeItemType.MimeMessageSmimeMultipartSigned))
				{
					if (this.item.AttachmentCollection.Count != 1)
					{
						StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateSmimeMimeStructure: attachment count not equal 1.");
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, null);
					}
					this.smimeAttachment = (this.item.AttachmentCollection.TryOpenFirstAttachment(AttachmentType.Stream) as StreamAttachment);
					if (this.smimeAttachment == null)
					{
						StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateSmimeMimeStructure: attachment is not a stream attachment.");
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, null);
					}
				}
				return this.smimeAttachment;
			}
		}

		private Item[] JournalAttachedItems
		{
			get
			{
				if (this.journalAttachedItems == null)
				{
					this.journalAttachedItems = new Item[this.JournalAttachments.Length];
					for (int i = 0; i < this.journalAttachedItems.Length; i++)
					{
						ItemAttachment itemAttachment = this.JournalAttachments[i] as ItemAttachment;
						if (itemAttachment != null)
						{
							this.journalAttachedItems[i] = ConvertUtils.OpenAttachedItem(itemAttachment);
							if (this.journalAttachedItems[i] == null)
							{
								StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::JournalAttachedItem: unable to open journal message.");
								throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionCannotOpenJournalMessage, null);
							}
						}
					}
				}
				return this.journalAttachedItems;
			}
		}

		private OutboundAddressCache[] JournalAttachedItemAddressCaches
		{
			get
			{
				if (this.journalAttachedItemAddressCaches == null)
				{
					this.journalAttachedItemAddressCaches = new OutboundAddressCache[this.JournalAttachedItems.Length];
					for (int i = 0; i < this.journalAttachedItemAddressCaches.Length; i++)
					{
						if (this.JournalAttachedItems[i] != null)
						{
							this.journalAttachedItemAddressCaches[i] = new OutboundAddressCache(this.options, new ConversionLimitsTracker(this.options.Limits));
							this.journalAttachedItemAddressCaches[i].CopyDataFromItem(this.JournalAttachedItems[i]);
						}
					}
				}
				return this.journalAttachedItemAddressCaches;
			}
		}

		internal static string TryGetParticipantSmtpAddress(Participant participant)
		{
			if (participant.RoutingType == null || participant.EmailAddress == null)
			{
				return string.Empty;
			}
			if (participant.RoutingType.Equals("SMTP", StringComparison.OrdinalIgnoreCase))
			{
				return participant.EmailAddress;
			}
			string text = participant.TryGetProperty(ParticipantSchema.SmtpAddress) as string;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}

		internal static string GetParticipantSmtpAddress(Participant participant, OutboundConversionOptions options)
		{
			Util.ThrowOnNullArgument(options, "options");
			Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			if (participant.RoutingType == null || participant.EmailAddress == null)
			{
				return string.Empty;
			}
			return ItemToMimeConverter.TryGetParticipantSmtpAddress(participant) ?? ImceaAddress.Encode(participant.RoutingType, participant.EmailAddress, options.ImceaEncapsulationDomain);
		}

		internal static bool IsValidHeader(string headerName)
		{
			if (string.IsNullOrEmpty(headerName))
			{
				return false;
			}
			for (int num = 0; num != headerName.Length; num++)
			{
				char c = headerName[num];
				if (c < '!' || c > '~' || c == ':')
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsReservedHeader(string headerName)
		{
			string text;
			return ItemToMimeConverter.GetReservedMimeHeadersList().TryGetValue(headerName.ToLowerInvariant(), out text);
		}

		internal static string GetAttachmentContentId(Attachment attachment, IList<AttachmentLink> attachmentLinks, out AttachmentLink link)
		{
			link = AttachmentLink.Find(attachment.Id, attachmentLinks);
			string text = (link == null) ? attachment.ContentId : link.ContentId;
			if (text != null)
			{
				return text.Trim();
			}
			return string.Empty;
		}

		internal static string GetAttachmentContentId(Attachment attachment, IList<AttachmentLink> attachmentLinks)
		{
			AttachmentLink attachmentLink;
			return ItemToMimeConverter.GetAttachmentContentId(attachment, attachmentLinks, out attachmentLink);
		}

		internal ConversionResult ConvertItemToSummaryTnef(MimeStreamWriter writer, ConversionLimitsTracker limits, bool base64Encode)
		{
			this.CheckDisposed(null);
			ConversionResult result = new ConversionResult();
			if (this.options.FilterAttachmentHandler != null)
			{
				throw new NotSupportedException("FilterAttachmentHandler is not supported in ItemToMimeConverter.ConvertItemToSummaryTnef");
			}
			using (StorageGlobals.SetTraceContext(this.item))
			{
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting item (ItemToMimeConverter.ConvertItemToSummaryTnef)");
				this.mimeWriter = writer;
				this.limitsTracker = limits;
				this.messageFormat = ItemToMimeConverter.MessageFormatType.SummaryTnef;
				if (this.HandleTnefConversionAsMime())
				{
					StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing item (ItemToMimeConverter.ConvertItemToSummaryTnef)");
					return result;
				}
				this.StartPart();
				this.WriteReceivedAndXHeaders();
				this.mimeWriter.WriteHeader(HeaderId.ContentType, "application/ms-tnef");
				this.mimeWriter.WriteHeaderParameter("name", "winmail.dat");
				if (base64Encode)
				{
					this.mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, "base64");
				}
				else
				{
					this.mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, "binary");
				}
				this.WriteMimeHeaders(ItemToMimeConverter.MimeFlags.ProduceTnefCorrelationKey);
				this.WriteExtendedHeaders();
				ByteEncoder byteEncoder = null;
				Stream stream = null;
				Stream stream2 = null;
				try
				{
					stream2 = this.mimeWriter.GetContentStream(false);
					Stream stream3 = stream2;
					if (base64Encode)
					{
						byteEncoder = new Base64Encoder();
						stream = new EncoderStream(stream3, byteEncoder, EncoderStreamAccess.Write);
						stream3 = stream;
					}
					using (ItemToTnefConverter itemToTnefConverter = new ItemToTnefConverter(this.item, this.AddressCache, stream3, this.options, this.limitsTracker, TnefType.SummaryTnef, this.tnefCorrelationKey, false))
					{
						result = itemToTnefConverter.Convert();
					}
				}
				finally
				{
					if (stream != null)
					{
						stream.Dispose();
					}
					if (byteEncoder != null)
					{
						byteEncoder.Dispose();
					}
					if (stream2 != null)
					{
						stream2.Dispose();
					}
				}
				this.EndPart();
				this.mimeWriter.Flush();
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing item (ItemToMimeConverter.ConvertItemToSummaryTnef)");
			}
			return result;
		}

		internal void ConvertItemToLegacyTnef(MimeStreamWriter writer, ConversionLimitsTracker limits)
		{
			this.CheckDisposed(null);
			if (this.options.FilterAttachmentHandler != null)
			{
				throw new NotSupportedException("FilterAttachmentHandler is not supported in ItemToMimeConverter.ConvertItemToLegacyTnef");
			}
			using (StorageGlobals.SetTraceContext(this.item))
			{
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting item (ItemToMimeConverter.ConvertItemToLegacyTnef)");
				this.mimeWriter = writer;
				this.limitsTracker = limits;
				this.messageFormat = ItemToMimeConverter.MessageFormatType.LegacyTnef;
				if (this.HandleTnefConversionAsMime())
				{
					StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing item (ItemToMimeConverter.ConvertItemToLegacyTnef)");
				}
				else
				{
					this.StartPart();
					this.WriteReceivedAndXHeaders();
					this.WriteMultipartContentTypeHeader("multipart/mixed", 0);
					this.WriteMimeHeaders(ItemToMimeConverter.MimeFlags.ProduceTnefCorrelationKey);
					this.WriteExtendedHeaders();
					this.StartPart();
					this.WriteMimeBody(MimePartContentType.TextPlain, ItemToMimeConverter.MimeFlags.SkipMessageHeaders, false);
					this.EndPart();
					this.StartPart();
					this.WriteTnefHeaders();
					this.WriteTnefAttachment();
					this.EndPart();
					this.EndPart();
					this.mimeWriter.Flush();
					StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing item (ItemToMimeConverter.ConvertItemToLegacyTnef)");
				}
			}
		}

		internal ConversionResult ConvertItemToMime(MimeStreamWriter writer, ConversionLimitsTracker limits)
		{
			this.CheckDisposed(null);
			if (this.options.AllowPartialStnefConversion && !ItemConversion.IsItemClassConvertibleToMime(this.item.ClassName))
			{
				this.ConvertItemToSummaryTnef(writer, limits, true);
				return new ConversionResult();
			}
			return this.ConvertItemToMimeInternal(writer, limits);
		}

		internal ConversionResult WriteMimePart(MimeStreamWriter mimeWriter, ConversionLimitsTracker limits, MimePartInfo part, ItemToMimeConverter.MimeFlags mimeFlags)
		{
			this.CheckDisposed(null);
			this.mimeWriter = mimeWriter;
			this.limitsTracker = limits;
			ConversionResult result = this.WriteMimePart(part, mimeFlags);
			mimeWriter.Flush();
			return result;
		}

		internal MimePartInfo CalculateMimeStructure(Charset itemCharset)
		{
			this.CheckDisposed(null);
			int num = 0;
			if (this.itemType == MimeItemType.MimeMessageSmime || this.itemType == MimeItemType.MimeMessageSmimeMultipartSigned)
			{
				if (this.options.FilterAttachmentHandler != null)
				{
					StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateMimeStructure: the item type is SMIME, FilterAttachmentHandler is ignored.");
				}
				return this.CalculateSmimeMimeStructure(itemCharset, ref num);
			}
			if ((this.options.UseSkeleton || this.options.InternetTextFormat == InternetTextFormat.BestBody) && this.Skeleton != null)
			{
				return this.CalculateMimeStructureForSkeleton(itemCharset, ref num);
			}
			if (this.IsJournalingMessage)
			{
				return this.CalculateJournalingMimeStructure(itemCharset, ref num);
			}
			MimePartInfo mimePartInfo = null;
			MimePartInfo newChild = null;
			MimePartInfo mimePartInfo2 = null;
			BodyFormat bodyFormat = this.item.Body.Format;
			MimePartInfo.Callback writerCallback = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMimeBody);
			if (this.itemType == MimeItemType.MimeMessageDsn)
			{
				mimePartInfo = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMultipartContent), MimePartContentType.MultipartReportDsn, ref num);
				newChild = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteDsnReportBody), MimePartContentType.DsnReportBody, ref num);
			}
			else if (this.itemType == MimeItemType.MimeMessageMdn)
			{
				mimePartInfo = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMultipartContent), MimePartContentType.MultipartReportMdn, ref num);
				newChild = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMdnReportBody), MimePartContentType.MdnReportBody, ref num);
			}
			if (mimePartInfo != null && !this.item.Body.IsBodyDefined)
			{
				writerCallback = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteHumanReadableReportBody);
				bodyFormat = BodyFormat.TextHtml;
			}
			if (mimePartInfo2 == null && (!CalendarUtil.CanConvertToMeetingMessage(this.item) || !this.item.GetValueOrDefault<bool>(InternalSchema.IsSingleBodyICal) || (!string.IsNullOrEmpty(this.options.OwaServer) && this.item.Id != null)))
			{
				if (this.options.InternetMessageFormat == InternetMessageFormat.Tnef)
				{
					mimePartInfo2 = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMultipartContent), MimePartContentType.MultipartMixed, ref num);
					mimePartInfo2.IsBodyToRemoveFromSkeleton = this.item.Body.IsBodyDefined;
					mimePartInfo2.AddChild(new MimePartInfo(itemCharset, writerCallback, MimePartContentType.TextPlain, ref num)
					{
						IsBodyToRemoveFromSkeleton = (this.item.Body.Format == BodyFormat.TextPlain && this.item.Body.IsBodyDefined)
					});
					mimePartInfo2.AddChild(new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteTnefPart), MimePartContentType.Tnef, ref num)
					{
						IsBodyToRemoveFromSkeleton = (this.item.Body.Format != BodyFormat.TextPlain)
					});
					return mimePartInfo2;
				}
				if (bodyFormat == BodyFormat.TextPlain && (this.options.BlockPlainTextConversion || this.options.InternetTextFormat == InternetTextFormat.BestBody))
				{
					mimePartInfo2 = new MimePartInfo(itemCharset, writerCallback, MimePartContentType.TextPlain, ref num);
					mimePartInfo2.IsBodyToRemoveFromSkeleton = this.item.Body.IsBodyDefined;
				}
				else
				{
					switch (this.options.InternetTextFormat)
					{
					case InternetTextFormat.TextOnly:
						mimePartInfo2 = new MimePartInfo(itemCharset, writerCallback, MimePartContentType.TextPlain, ref num);
						mimePartInfo2.IsBodyToRemoveFromSkeleton = this.item.Body.IsBodyDefined;
						break;
					case InternetTextFormat.HtmlOnly:
					case InternetTextFormat.BestBody:
						mimePartInfo2 = new MimePartInfo(itemCharset, writerCallback, MimePartContentType.TextHtml, ref num);
						mimePartInfo2.IsBodyToRemoveFromSkeleton = this.item.Body.IsBodyDefined;
						break;
					case InternetTextFormat.HtmlAndTextAlternative:
						mimePartInfo2 = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMultipartContent), MimePartContentType.FirstMultipartType, ref num);
						mimePartInfo2.AddChild(new MimePartInfo(itemCharset, writerCallback, MimePartContentType.TextPlain, ref num)
						{
							IsBodyToRemoveFromSkeleton = (this.item.Body.Format == BodyFormat.TextPlain && this.item.Body.IsBodyDefined)
						});
						mimePartInfo2.AddChild(new MimePartInfo(itemCharset, writerCallback, MimePartContentType.TextHtml, ref num)
						{
							IsBodyToRemoveFromSkeleton = (this.item.Body.Format != BodyFormat.TextPlain)
						});
						break;
					case InternetTextFormat.TextEnrichedOnly:
						mimePartInfo2 = new MimePartInfo(itemCharset, writerCallback, MimePartContentType.TextEnriched, ref num);
						mimePartInfo2.IsBodyToRemoveFromSkeleton = this.item.Body.IsBodyDefined;
						break;
					case InternetTextFormat.TextEnrichedAndTextAlternative:
						mimePartInfo2 = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMultipartContent), MimePartContentType.FirstMultipartType, ref num);
						mimePartInfo2.AddChild(new MimePartInfo(itemCharset, writerCallback, MimePartContentType.TextPlain, ref num)
						{
							IsBodyToRemoveFromSkeleton = (this.item.Body.Format == BodyFormat.TextPlain && this.item.Body.IsBodyDefined)
						});
						mimePartInfo2.AddChild(new MimePartInfo(itemCharset, writerCallback, MimePartContentType.TextEnriched, ref num)
						{
							IsBodyToRemoveFromSkeleton = (this.item.Body.Format != BodyFormat.TextPlain)
						});
						break;
					}
				}
			}
			if (CalendarUtil.CanConvertToMeetingMessage(this.item))
			{
				if (!string.IsNullOrEmpty(this.options.OwaServer) && this.item.Id != null)
				{
					StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateMimeStructure: Generating OWA link for ICal part instead of ICAL");
				}
				else if (this.item.GetValueOrDefault<bool>(InternalSchema.IsSingleBodyICal))
				{
					mimePartInfo2 = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMimeBody), MimePartContentType.Calendar, ref num);
				}
				else
				{
					mimePartInfo2 = this.AddICalBodyPart(mimePartInfo2, ref num, Charset.UTF8);
				}
			}
			List<MimePartInfo> list = new List<MimePartInfo>();
			List<MimePartInfo> list2 = new List<MimePartInfo>();
			MimePartInfo mimePartInfo3 = null;
			foreach (AttachmentHandle handle in this.item.AttachmentCollection)
			{
				using (Attachment attachment = this.item.AttachmentCollection.Open(handle, null))
				{
					MimePartInfo mimePartInfo4 = null;
					StreamAttachment streamAttachment = attachment as StreamAttachment;
					ItemAttachment itemAttachment = attachment as ItemAttachment;
					ReferenceAttachment referenceAttachment = attachment as ReferenceAttachment;
					bool flag = itemAttachment != null;
					bool flag2 = flag && ObjectClass.IsContact(itemAttachment.GetValueOrDefault<string>(InternalSchema.ItemClass));
					bool flag3 = false;
					if (this.options.FilterAttachmentHandler != null && !this.options.FilterAttachmentHandler(this.item, attachment))
					{
						StorageGlobals.ContextTraceDebug<Attachment>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateMimeStructure: Skipping attachment {0}", attachment);
						flag3 = true;
					}
					if (streamAttachment != null && streamAttachment.IsMacAttachment)
					{
						mimePartInfo4 = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMultipartAppleDouble), MimePartContentType.MultipartAppleDouble, attachment.Id, ref num);
						MimePartInfo.Callback writerCallback2;
						if (flag3)
						{
							writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteEmptyMacAppleFile);
						}
						else
						{
							writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMacAppleFile);
						}
						mimePartInfo4.AddChild(new MimePartInfo(itemCharset, writerCallback2, MimePartContentType.AppleFile, attachment.Id, ref num));
						if (flag3)
						{
							writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteEmptyMimeAttachment);
						}
						else
						{
							writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMimeAttachment);
						}
						mimePartInfo4.AddChild(new MimePartInfo(itemCharset, writerCallback2, MimePartContentType.Attachment, attachment.Id, ref num));
					}
					else if (flag2)
					{
						MimePartInfo.Callback writerCallback2;
						if (flag3)
						{
							writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteEmptyMimeAttachment);
						}
						else
						{
							writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMimeAttachment);
						}
						mimePartInfo4 = new MimePartInfo(Charset.UTF8, writerCallback2, MimePartContentType.Attachment, attachment.Id, ref num);
					}
					else if (referenceAttachment != null)
					{
						StorageGlobals.ContextTraceDebug<Attachment>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateMimeStructure: Skipping reference attachment {0}", attachment);
					}
					else
					{
						bool flag4 = false;
						if (streamAttachment != null && ObjectClass.IsFailedInboundICal(this.item.ClassName))
						{
							flag4 = streamAttachment.GetValueOrDefault<bool>(AttachmentSchema.FailedInboundICalAsAttachment);
						}
						if (!flag4)
						{
							MimePartInfo.Callback writerCallback2;
							if (flag3)
							{
								writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteEmptyMimeAttachment);
							}
							else
							{
								writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMimeAttachment);
							}
							mimePartInfo4 = new MimePartInfo(itemCharset, writerCallback2, flag ? MimePartContentType.ItemAttachment : MimePartContentType.Attachment, attachment.Id, ref num);
						}
					}
					if (mimePartInfo4 != null)
					{
						if (attachment.IsInline && !flag)
						{
							list.Add(mimePartInfo4);
						}
						else
						{
							list2.Add(mimePartInfo4);
							if (flag && !flag2 && mimePartInfo3 == null)
							{
								mimePartInfo3 = mimePartInfo4;
							}
						}
					}
				}
			}
			if (list.Count != 0)
			{
				MimePartInfo mimePartInfo5 = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMultipartContent), MimePartContentType.MultipartRelated, ref num);
				mimePartInfo5.AddChild(mimePartInfo2);
				mimePartInfo5.AddChildren(list);
				mimePartInfo2 = mimePartInfo5;
			}
			if (mimePartInfo != null)
			{
				mimePartInfo.AddChild(mimePartInfo2);
				mimePartInfo.AddChild(newChild);
				if (mimePartInfo3 != null)
				{
					mimePartInfo.AddChild(mimePartInfo3);
				}
				mimePartInfo2 = mimePartInfo;
			}
			else if (list2.Count != 0)
			{
				MimePartInfo mimePartInfo6 = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMultipartContent), MimePartContentType.MultipartMixed, ref num);
				mimePartInfo6.AddChild(mimePartInfo2);
				mimePartInfo6.AddChildren(list2);
				mimePartInfo2 = mimePartInfo6;
			}
			return mimePartInfo2;
		}

		internal EncodingOptions GetItemMimeEncodingOptions(OutboundConversionOptions options)
		{
			Item item = this.item;
			if (ObjectClass.IsOfClass(this.item.ClassName, "IPM.Note.JournalReport.Msg") || ObjectClass.IsOfClass(this.item.ClassName, "IPM.Note.JournalReport.Tnef"))
			{
				if (this.JournalAttachedItems == null || this.JournalAttachedItems[0] == null)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::GetMimeEncodingOptions::JournalAttachedItems[0] == null.");
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionCannotOpenJournalMessage, null);
				}
				item = this.JournalAttachedItems[0];
			}
			Charset itemOutboundMimeCharset = ConvertUtils.GetItemOutboundMimeCharset(item, options);
			CultureInfo itemCultureInfo = ConvertUtils.GetItemCultureInfo(item);
			EncodingFlags encodingFlags = EncodingFlags.None;
			if (options.UseRFC2231Encoding)
			{
				encodingFlags |= EncodingFlags.EnableRfc2231;
			}
			if (options.QuoteDisplayNameBeforeRfc2047Encoding)
			{
				encodingFlags |= EncodingFlags.QuoteDisplayNameBeforeRfc2047Encoding;
			}
			if (options.AllowUTF8Headers)
			{
				encodingFlags |= EncodingFlags.AllowUTF8;
			}
			return new EncodingOptions(itemOutboundMimeCharset.Name, itemCultureInfo.Name, encodingFlags);
		}

		internal void SetSkeletonAndSmimeDoc(MimeDocument skeleton, MimeDocument smime)
		{
			if (this.smimeDocument != null || this.skeleton != null)
			{
				throw new InvalidOperationException();
			}
			if (skeleton != null)
			{
				this.skeleton = skeleton;
				this.skeletonInitialized = true;
				this.ownsMimeDocuments = false;
			}
			if (smime != null)
			{
				this.smimeDocument = smime;
				this.smimeDocInitialized = true;
				this.ownsMimeDocuments = false;
			}
		}

		internal List<MimeDocument> ExtractSkeletons()
		{
			List<MimeDocument> list = this.ownedEmbeddedMimeDocuments;
			this.ownedEmbeddedMimeDocuments = new List<MimeDocument>();
			if (this.ownsMimeDocuments)
			{
				if (this.Skeleton != null)
				{
					list.Add(this.Skeleton);
				}
				if (this.SmimeDocument != null)
				{
					list.Add(this.SmimeDocument);
				}
				this.ownsMimeDocuments = false;
			}
			return list;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ItemToMimeConverter>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
			if (disposing)
			{
				if (this.journalAttachedItems != null)
				{
					for (int i = 0; i < this.journalAttachedItems.Length; i++)
					{
						if (this.journalAttachedItems[i] != null)
						{
							if (this.journalAttachedItemNeedsSave[i])
							{
								this.journalAttachedItems[i].Save(SaveMode.ResolveConflicts);
							}
							this.journalAttachedItems[i].Dispose();
						}
					}
					this.journalAttachedItems = null;
				}
				if (this.journalAttachments != null)
				{
					for (int j = 0; j < this.journalAttachments.Length; j++)
					{
						if (this.journalAttachments[j] != null)
						{
							if (this.journalAttachmentNeedsSave[j])
							{
								this.journalAttachments[j].Save();
							}
							this.journalAttachments[j].Dispose();
						}
					}
					this.journalAttachments = null;
				}
				if (this.smimeAttachment != null)
				{
					this.smimeAttachment.Dispose();
				}
				if (this.ownsMimeDocuments)
				{
					if (this.smimeDocument != null)
					{
						this.smimeDocument.Dispose();
					}
					if (this.skeleton != null)
					{
						this.skeleton.Dispose();
					}
				}
				foreach (MimeDocument mimeDocument in this.ownedEmbeddedMimeDocuments)
				{
					mimeDocument.Dispose();
				}
				this.ownedEmbeddedMimeDocuments.Clear();
			}
		}

		private static string FilenameFromSubject(string subject, string extension, LocalizedString defaultStr)
		{
			if (string.IsNullOrEmpty(subject))
			{
				return defaultStr;
			}
			StringBuilder stringBuilder = new StringBuilder(subject.Length);
			int num = Math.Min(subject.Length, 64);
			int num2 = 0;
			bool flag = false;
			while (num-- > 0)
			{
				char c = subject[num2++];
				if (char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' ')
				{
					if (flag)
					{
						stringBuilder.Append('-');
					}
					stringBuilder.Append(c);
					flag = false;
				}
				else
				{
					flag = true;
				}
			}
			string text = stringBuilder.ToString().Trim() + extension;
			if (text.Length != 4)
			{
				return text;
			}
			return defaultStr;
		}

		private static MimeItemType GetItemType(Item item)
		{
			string className = item.ClassName;
			if (ObjectClass.IsSmimeClearSigned(className))
			{
				return MimeItemType.MimeMessageSmimeMultipartSigned;
			}
			if (ObjectClass.IsSmime(className))
			{
				return MimeItemType.MimeMessageSmime;
			}
			if (ObjectClass.IsDsn(className))
			{
				return MimeItemType.MimeMessageDsn;
			}
			if (ObjectClass.IsMdn(className))
			{
				return MimeItemType.MimeMessageMdn;
			}
			if (ObjectClass.IsOfClass(className, "IPM.Appointment") || ObjectClass.IsOfClass(className, "IPM.Schedule.Meeting"))
			{
				return MimeItemType.MimeMessageCalendar;
			}
			if (ObjectClass.IsOfClass(className, "IPM.Replication"))
			{
				return MimeItemType.MimeMessageReplication;
			}
			if (ObjectClass.IsOfClass(className, "IPM.Note.JournalReport.Msg"))
			{
				return MimeItemType.MimeMessageJournalMsg;
			}
			if (ObjectClass.IsOfClass(className, "IPM.Note.JournalReport.Tnef"))
			{
				return MimeItemType.MimeMessageJournalTnef;
			}
			object obj = item.TryGetProperty(InternalSchema.XMSJournalReport);
			if (!(obj is PropertyError))
			{
				return MimeItemType.MimeMessageSecondaryJournal;
			}
			return MimeItemType.MimeMessageGeneric;
		}

		private static ConversionResult StaticSkeletonSmimeMultipartToMime(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
			{
				foreach (MimePart part2 in converter.SmimeDocument.RootPart)
				{
					converter.mimeWriter.WritePartWithHeaders(part2, true);
				}
			}
			return new ConversionResult();
		}

		private static ConversionResult StaticSkeletonSmimeBlobToMime(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
			{
				using (Stream stream = converter.SmimeAttachment.TryGetRawContentStream(PropertyOpenMode.ReadOnly))
				{
					if (stream == null)
					{
						StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::StaticWriteSmimeBlob - Unable to open attachment stream.");
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, null);
					}
					using (Stream stream2 = converter.OpenMimeWriteStreamForSkeletonPart(part.SkeletonPart))
					{
						Util.StreamHandler.CopyStreamData(stream, stream2);
					}
				}
			}
			return new ConversionResult();
		}

		private static ConversionResult StaticMultipartSkeletonToMime(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			ConversionResult result = new ConversionResult();
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
			{
				return converter.WriteMimeParts(part.Children, flags);
			}
			return result;
		}

		private static ConversionResult StaticSkeletonBodyToMime(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			converter.limitsTracker.CountMessageBody();
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) == ItemToMimeConverter.MimeFlags.SkipContent)
			{
				return new ConversionResult();
			}
			Charset charset = MimeHelpers.GetCharsetFromMime(part.SkeletonPart);
			using (Stream writeStream = converter.OpenMimeWriteStreamForSkeletonPart(part.SkeletonPart))
			{
				string contentType = part.SkeletonPart.ContentType;
				if (string.Equals(contentType, "text/plain", StringComparison.OrdinalIgnoreCase))
				{
					using (Stream stream = converter.item.Body.OpenReadStream(new BodyReadConfiguration(BodyFormat.TextPlain, charset.Name)))
					{
						Util.StreamHandler.CopyStreamData(stream, writeStream);
						goto IL_185;
					}
				}
				if (string.Equals(contentType, "text/html", StringComparison.OrdinalIgnoreCase))
				{
					using (Stream stream2 = converter.item.Body.OpenReadStream(new BodyReadConfiguration(BodyFormat.TextHtml, charset.Name)))
					{
						Util.StreamHandler.CopyStreamData(stream2, writeStream);
						goto IL_185;
					}
				}
				if (string.Equals(contentType, "text/enriched", StringComparison.OrdinalIgnoreCase))
				{
					using (Stream bodyStream = converter.item.Body.OpenReadStream(new BodyReadConfiguration(BodyFormat.TextHtml, charset.Name)))
					{
						ConvertUtils.CallCts(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter.StaticSkeletonLeafToMime", ServerStrings.ConversionBodyConversionFailed, delegate
						{
							Encoding encoding = charset.GetEncoding();
							HtmlToEnriched htmlToEnriched = new HtmlToEnriched();
							htmlToEnriched.OutputEncoding = encoding;
							htmlToEnriched.InputEncoding = encoding;
							using (Stream stream3 = new ConverterStream(bodyStream, htmlToEnriched, ConverterStreamAccess.Read))
							{
								Util.StreamHandler.CopyStreamData(stream3, writeStream);
							}
						});
					}
				}
				IL_185:;
			}
			return new ConversionResult();
		}

		private static ConversionResult StaticSkeletonCopyToMime(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) == ItemToMimeConverter.MimeFlags.SkipContent)
			{
				return new ConversionResult();
			}
			using (Stream rawContentReadStream = part.SkeletonPart.GetRawContentReadStream())
			{
				using (Stream contentStream = converter.mimeWriter.GetContentStream(false))
				{
					Util.StreamHandler.CopyStreamData(rawContentReadStream, contentStream);
				}
			}
			return new ConversionResult();
		}

		private static ConversionResult StaticSkeletonEmptyAttachmentToMime(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			flags |= ItemToMimeConverter.MimeFlags.SkipContent;
			return ItemToMimeConverter.StaticSkeletonAttachmentToMime(converter, part, flags);
		}

		private static ConversionResult StaticSkeletonAttachmentToMime(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			converter.limitsTracker.CountMessageAttachment();
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) == ItemToMimeConverter.MimeFlags.SkipContent)
			{
				return new ConversionResult();
			}
			using (Stream stream = converter.OpenMimeWriteStreamForSkeletonPart(part.SkeletonPart))
			{
				Attachment attachment = null;
				if (converter.IsJournalingMessage)
				{
					attachment = converter.JournalAttachments[converter.GetJournalAttachmentIndex(part.AttachmentId)];
				}
				else
				{
					attachment = converter.item.AttachmentCollection.Open(part.AttachmentId);
				}
				try
				{
					ItemAttachment itemAttachment = attachment as ItemAttachment;
					if (itemAttachment != null)
					{
						ItemToMimeConverter.StaticSkeletonItemAttachmentToMime(converter, part, itemAttachment, stream, flags);
					}
					else
					{
						ItemToMimeConverter.StaticSkeletonStreamAttachmentToMime(converter, part, attachment as StreamAttachmentBase, stream, flags);
					}
				}
				finally
				{
					if (!converter.IsJournalingMessage)
					{
						attachment.Dispose();
					}
				}
			}
			return new ConversionResult();
		}

		private static void StaticSkeletonStreamAttachmentToMime(ItemToMimeConverter converter, MimePartInfo part, StreamAttachmentBase streamAttach, Stream writeStream, ItemToMimeConverter.MimeFlags flags)
		{
			OleAttachment oleAttachment = streamAttach as OleAttachment;
			if (oleAttachment != null)
			{
				if (!oleAttachment.TryConvertToImage(writeStream, ImageFormat.Jpeg))
				{
					ConvertUtils.SaveDefaultImage(writeStream);
					return;
				}
			}
			else
			{
				StreamAttachment streamAttachment = streamAttach as StreamAttachment;
				string contentType = part.SkeletonPart.ContentType;
				if (string.Equals(contentType, "application/mac-binhex40"))
				{
					bool isMacAttachment = streamAttachment.IsMacAttachment;
					using (Stream stream = streamAttachment.TryGetRawContentStream(PropertyOpenMode.ReadOnly))
					{
						if (stream != null)
						{
							BinHexEncoder encoder;
							if (isMacAttachment)
							{
								encoder = new BinHexEncoder();
							}
							else
							{
								encoder = new BinHexEncoder(new MacBinaryHeader
								{
									DataForkLength = stream.Length
								});
							}
							using (Stream stream2 = new EncoderStream(stream, encoder, EncoderStreamAccess.Read))
							{
								Util.StreamHandler.CopyStreamData(stream2, writeStream);
							}
						}
						return;
					}
				}
				if (string.Equals(contentType, "application/applefile"))
				{
					bool isMacAttachment2 = streamAttachment.IsMacAttachment;
					using (Stream stream3 = streamAttachment.TryGetRawContentStream(PropertyOpenMode.ReadOnly))
					{
						if (stream3 != null)
						{
							if (!isMacAttachment2)
							{
								MimeAppleTranscoder.WriteWholeApplefile(stream3, writeStream);
							}
							else
							{
								using (Stream readOnlyApplefileStream = streamAttachment.GetReadOnlyApplefileStream())
								{
									MimeAppleTranscoder.WriteWholeApplefile(readOnlyApplefileStream, stream3, writeStream);
								}
							}
						}
						return;
					}
				}
				using (Stream stream4 = streamAttachment.TryGetContentStream(PropertyOpenMode.ReadOnly))
				{
					if (stream4 != null)
					{
						Util.StreamHandler.CopyStreamData(stream4, writeStream);
					}
				}
			}
		}

		private static void StaticSkeletonItemAttachmentToMime(ItemToMimeConverter converter, MimePartInfo part, ItemAttachment itemAttach, Stream writeStream, ItemToMimeConverter.MimeFlags flags)
		{
			Item item = converter.IsJournalingMessage ? converter.JournalAttachedItems[converter.GetJournalAttachmentIndex(part.AttachmentId)] : ConvertUtils.OpenAttachedItem(itemAttach);
			if (item == null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::StaticSkeletonLeafToMime: unable to opend attached item.");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
			}
			try
			{
				if (converter.itemType == MimeItemType.MimeMessageJournalMsg)
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						StoreSession session = item.Session;
						bool flag = false;
						try
						{
							if (session != null)
							{
								session.BeginMapiCall();
								session.BeginServerHealthCall();
								flag = true;
							}
							if (StorageGlobals.MapiTestHookBeforeCall != null)
							{
								StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
							}
							item.MapiMessage.SaveMessageToStream(memoryStream, SaveMessageFlags.Unicode | SaveMessageFlags.BestBody);
						}
						catch (MapiPermanentException ex)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCopyItem, ex, session, converter, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("ItemToMimeConverter.StaticSkeletonLeafToMime", new object[0]),
								ex
							});
						}
						catch (MapiRetryableException ex2)
						{
							throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCopyItem, ex2, session, converter, "{0}. MapiException = {1}.", new object[]
							{
								string.Format("ItemToMimeConverter.StaticSkeletonLeafToMime", new object[0]),
								ex2
							});
						}
						finally
						{
							try
							{
								if (session != null)
								{
									session.EndMapiCall();
									if (flag)
									{
										session.EndServerHealthCall();
									}
								}
							}
							finally
							{
								if (StorageGlobals.MapiTestHookAfterCall != null)
								{
									StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
								}
							}
						}
						memoryStream.Position = 0L;
						Util.StreamHandler.CopyStreamData(memoryStream, writeStream);
						goto IL_2FF;
					}
				}
				if (ObjectClass.IsContact(item.ClassName))
				{
					if (!string.Equals(part.SkeletonPart.ContentType, "text/directory", StringComparison.OrdinalIgnoreCase))
					{
						StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::StaticSkeletonLeafToMime: invalid attachment type.");
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
					}
					OutboundVCardConverter.Convert(writeStream, Encoding.UTF8, item as Contact, converter.options, converter.limitsTracker);
				}
				else
				{
					if (!string.Equals(part.SkeletonPart.ContentType, "message/rfc822", StringComparison.OrdinalIgnoreCase))
					{
						StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::StaticSkeletonLeafToMime: invalid attachment type.");
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
					}
					converter.limitsTracker.StartEmbeddedMessage();
					using (ItemToMimeConverter itemToMimeConverter = new ItemToMimeConverter(item, converter.options, converter.flags | ConverterFlags.IsEmbeddedMessage))
					{
						using (MimeStreamWriter embeddedWriter = converter.mimeWriter.GetEmbeddedWriter(itemToMimeConverter.GetItemMimeEncodingOptions(itemToMimeConverter.options), null, itemToMimeConverter.options))
						{
							ConversionLimitsTracker limits = converter.IsJournalingMessage ? new ConversionLimitsTracker(converter.options.Limits) : converter.limitsTracker;
							if (converter.options.AllowPartialStnefConversion && !ItemConversion.IsItemClassConvertibleToMime(item.ClassName))
							{
								itemToMimeConverter.ConvertItemToSummaryTnef(embeddedWriter, limits, true);
							}
							else
							{
								MimePartInfo mimePartInfo;
								if (part.AttachedItemStructure != null)
								{
									mimePartInfo = part.AttachedItemStructure;
									itemToMimeConverter.SetSkeletonAndSmimeDoc(mimePartInfo.Skeleton, mimePartInfo.SmimeDocument);
								}
								else
								{
									mimePartInfo = itemToMimeConverter.CalculateMimeStructure(embeddedWriter.WriterCharset);
									part.AttachedItemStructure = mimePartInfo;
								}
								itemToMimeConverter.WriteMimePart(embeddedWriter, limits, mimePartInfo, flags | ItemToMimeConverter.MimeFlags.WriteMessageHeaders);
							}
						}
						converter.ownedEmbeddedMimeDocuments.AddRange(itemToMimeConverter.ExtractSkeletons());
					}
					converter.limitsTracker.EndEmbeddedMessage();
				}
				IL_2FF:;
			}
			finally
			{
				if (!converter.IsJournalingMessage)
				{
					item.Dispose();
				}
			}
		}

		private static void WriteJournalMailbox(StringBuilder sb, Participant participant)
		{
			if (participant != null)
			{
				if (participant.RoutingType == "SMTP" && participant.EmailAddress != null)
				{
					sb.Append(participant.EmailAddress);
					return;
				}
				string routingType = participant.RoutingType;
				string emailAddress = participant.EmailAddress;
				if (routingType != null && emailAddress != null)
				{
					sb.AppendFormat("[{0}:{1}]", routingType, emailAddress);
				}
			}
		}

		private static string BuildOwaLink(Item item, MimePartContentType contentType, string owaServer)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			switch (contentType)
			{
			default:
				stringBuilder.AppendFormat("{0}: ", "Microsoft Outlook Web Access");
				break;
			case MimePartContentType.TextHtml:
				stringBuilder.AppendFormat("\r\n<br>{0}: <a href=\"", "Microsoft Outlook Web Access");
				break;
			}
			string text = string.Format("{0}?ae=Item&a=open&t={1}&id={2}", owaServer, item.ClassName, Uri.EscapeDataString(item.Id.ObjectId.ToBase64String()));
			stringBuilder.Append(text);
			switch (contentType)
			{
			default:
				stringBuilder.Append("\r\n\r\n");
				break;
			case MimePartContentType.TextHtml:
				stringBuilder.AppendFormat("\">{0}</a><br><br>\r\n", text);
				break;
			}
			return stringBuilder.ToString();
		}

		private static ConversionResult StaticWriteSmimeLeaf(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			using (IImapMimeProvider imapMimeProvider = IImapMimeProvider.CreateInstance(part.SmimePart, part.SmimeDocument))
			{
				imapMimeProvider.WriteMimePart(converter.mimeWriter, converter.limitsTracker, part, flags);
			}
			return new ConversionResult();
		}

		private static ConversionResult StaticWriteSmimeBlob(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			string fileName = converter.SmimeAttachment.FileName;
			string valueOrDefault = converter.item.GetValueOrDefault<string>(InternalSchema.NamedContentType);
			if (valueOrDefault != null && MimeStreamWriter.CheckAsciiHeaderValue(valueOrDefault))
			{
				try
				{
					converter.WriteComplexContentTypeHeader(valueOrDefault);
					goto IL_C7;
				}
				catch (ArgumentException ex)
				{
					StorageGlobals.ContextTraceDebug<ArgumentException>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::StaticWriteSmimeBlob - Invalid content-type: {0}.", ex);
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, ex);
				}
			}
			string contentType = converter.SmimeAttachment.ContentType;
			if (!MimeStreamWriter.CheckAsciiHeaderValue(contentType))
			{
				StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::StaticWriteSmimeBlob - Invalid content-type.");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, null);
			}
			try
			{
				converter.mimeWriter.WriteHeader(HeaderId.ContentType, contentType);
				if (fileName != null)
				{
					converter.mimeWriter.WriteHeaderParameter("name", fileName);
				}
			}
			catch (ArgumentException ex2)
			{
				StorageGlobals.ContextTraceDebug<ArgumentException>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::StaticWriteSmimeBlob - Invalid content-type: {0}.", ex2);
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, ex2);
			}
			IL_C7:
			converter.mimeWriter.WriteHeader(HeaderId.ContentDisposition, "attachment");
			if (fileName != null)
			{
				converter.mimeWriter.WriteHeaderParameter("filename", fileName);
			}
			converter.mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, "base64");
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
			{
				using (Stream stream = converter.SmimeAttachment.TryGetRawContentStream(PropertyOpenMode.ReadOnly))
				{
					if (stream == null)
					{
						StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::StaticWriteSmimeBlob - Unable to open attachment stream.");
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, null);
					}
					using (Stream contentStream = converter.mimeWriter.GetContentStream(false))
					{
						ByteEncoder encoder = new Base64Encoder();
						using (Stream stream2 = new EncoderStream(stream, encoder, EncoderStreamAccess.Read))
						{
							Util.StreamHandler.CopyStreamData(stream2, contentStream);
						}
					}
				}
			}
			return new ConversionResult();
		}

		private static ConversionResult StaticWriteSmimeMultipart(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			MimePart rootPart = converter.SmimeDocument.RootPart;
			Header[] array = rootPart.Headers.FindAll(HeaderId.ContentType);
			bool flag = false;
			foreach (Header header in array)
			{
				ContentTypeHeader contentTypeHeader = (ContentTypeHeader)header;
				if (contentTypeHeader.Value.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase))
				{
					converter.mimeWriter.WriteHeader(contentTypeHeader);
					flag = true;
				}
			}
			if (!flag)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::StaticWriteSmimeMultipart: no content type.");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeClearSignedContent, null);
			}
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
			{
				foreach (MimePart part2 in rootPart)
				{
					converter.mimeWriter.WritePartWithHeaders(part2, true);
				}
			}
			return new ConversionResult();
		}

		private static ConversionResult StaticWriteDsnReportBody(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			return converter.WriteDsnReportBody(part, flags);
		}

		private static ConversionResult StaticWriteMdnReportBody(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			return converter.WriteMdnReportBody(part, flags);
		}

		private static ConversionResult StaticWriteHumanReadableReportBody(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			return converter.WriteHumanReadableReportBody(part, flags);
		}

		private static ConversionResult StaticWriteMacAppleFile(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			return converter.WriteMacAppleFile(part, flags);
		}

		private static ConversionResult StaticWriteEmptyMacAppleFile(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			flags |= ItemToMimeConverter.MimeFlags.SkipContent;
			return ItemToMimeConverter.StaticWriteMacAppleFile(converter, part, flags);
		}

		private static ConversionResult StaticWriteMimeAttachment(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			return converter.WriteMimeAttachment(part, flags);
		}

		private static ConversionResult StaticWriteEmptyMimeAttachment(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			flags |= ItemToMimeConverter.MimeFlags.SkipContent;
			return ItemToMimeConverter.StaticWriteMimeAttachment(converter, part, flags);
		}

		private static ConversionResult StaticWriteMultipartContent(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			return converter.MultipartContentCallback(part, flags);
		}

		private static ConversionResult StaticWriteMultipartAppleDouble(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			return converter.MultipartAppleDoubleCallback(part, flags);
		}

		private static ConversionResult StaticWriteMimeBody(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			return converter.WriteMimeBody(part, flags);
		}

		private static ConversionResult StaticWriteTnefPart(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			return converter.WriteTnefPart(part, flags);
		}

		private static ConversionResult StaticWriteEmptyJournalMsgItemAttachment(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			flags |= ItemToMimeConverter.MimeFlags.SkipContent;
			return ItemToMimeConverter.StaticWriteJournalMsgItemAttachment(converter, part, flags);
		}

		private static ConversionResult StaticWriteJournalMsgItemAttachment(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			Attachment attachment = converter.JournalAttachments[0];
			Item item = converter.JournalAttachedItems[0];
			MimeStreamWriter mimeStreamWriter = converter.mimeWriter;
			ConversionResult conversionResult = new ConversionResult();
			if (item.MapiMessage == null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::StaticWriteJournalMsgItemAttachment: primary journal message conversion is not supported for in memory items.");
				throw new ConversionFailedException(ConversionFailureReason.ConverterInternalFailure, ServerStrings.ConversionCannotOpenJournalMessage, null);
			}
			bool flag = false;
			string subject = item.TryGetProperty(InternalSchema.Subject) as string;
			string text = ItemToMimeConverter.FilenameFromSubject(subject, ".msg", ServerStrings.ExDefaultJournalFilename);
			mimeStreamWriter.WriteHeader(HeaderId.ContentType, "application/octet-stream");
			mimeStreamWriter.WriteHeaderParameter("name", text);
			flag = converter.WriteMimeAttachmentContentId(attachment);
			mimeStreamWriter.WriteHeader(HeaderId.ContentTransferEncoding, "base64");
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) == ItemToMimeConverter.MimeFlags.SkipMessageHeaders)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					StoreSession session = item.Session;
					bool flag2 = false;
					try
					{
						if (session != null)
						{
							session.BeginMapiCall();
							session.BeginServerHealthCall();
							flag2 = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						item.MapiMessage.SaveMessageToStream(memoryStream, SaveMessageFlags.Unicode | SaveMessageFlags.BestBody);
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCopyItem, ex, session, converter, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("ItemToMimeConverter.StaticWriteJournalMsgItemAttachment", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCopyItem, ex2, session, converter, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("ItemToMimeConverter.StaticWriteJournalMsgItemAttachment", new object[0]),
							ex2
						});
					}
					finally
					{
						try
						{
							if (session != null)
							{
								session.EndMapiCall();
								if (flag2)
								{
									session.EndServerHealthCall();
								}
							}
						}
						finally
						{
							if (StorageGlobals.MapiTestHookAfterCall != null)
							{
								StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
							}
						}
					}
					memoryStream.Position = 0L;
					converter.WriteContentDisposition("attachment", attachment, memoryStream.Length, text);
					using (Stream contentStream = mimeStreamWriter.GetContentStream(false))
					{
						ByteEncoder encoder = new Base64Encoder();
						using (Stream stream = new EncoderStream(contentStream, encoder, EncoderStreamAccess.Write))
						{
							Util.StreamHandler.CopyStreamData(memoryStream, stream);
						}
					}
				}
			}
			converter.journalAttachmentNeedsSave[0] = flag;
			conversionResult.ItemWasModified = flag;
			return conversionResult;
		}

		private static ConversionResult StaticWriteEmptyJournalEmlItemAttachment(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			flags |= ItemToMimeConverter.MimeFlags.SkipContent;
			return ItemToMimeConverter.StaticWriteJournalEmlItemAttachment(converter, part, flags);
		}

		private static ConversionResult StaticWriteJournalEmlItemAttachment(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			MimeStreamWriter mimeStreamWriter = converter.mimeWriter;
			int journalAttachmentIndex = converter.GetJournalAttachmentIndex(part.AttachmentId);
			Item item = converter.JournalAttachedItems[journalAttachmentIndex];
			Attachment attachment = converter.JournalAttachments[journalAttachmentIndex];
			OutboundAddressCache outboundAddressCache = converter.JournalAttachedItemAddressCaches[journalAttachmentIndex];
			ConversionResult conversionResult = new ConversionResult();
			mimeStreamWriter.WriteHeader(HeaderId.ContentType, "message/rfc822");
			converter.WriteContentDisposition("attachment", attachment, 0L, null);
			bool flag = converter.WriteMimeAttachmentContentId(attachment);
			Stream stream = null;
			bool flag2 = converter.messageFormat == ItemToMimeConverter.MessageFormatType.Mime && (converter.flags & ConverterFlags.GenerateSkeleton) == ConverterFlags.GenerateSkeleton;
			try
			{
				if (flag2)
				{
					stream = converter.OpenSkeletonStream(item, conversionResult);
				}
				using (ItemToMimeConverter itemToMimeConverter = new ItemToMimeConverter(item, converter.options, converter.flags | ConverterFlags.IsEmbeddedMessage, outboundAddressCache))
				{
					using (MimeStreamWriter embeddedWriter = mimeStreamWriter.GetEmbeddedWriter(itemToMimeConverter.GetItemMimeEncodingOptions(converter.options), stream, converter.options))
					{
						ConversionLimitsTracker limits = new ConversionLimitsTracker(converter.options.Limits);
						if (converter.messageFormat == ItemToMimeConverter.MessageFormatType.Mime)
						{
							MimePartInfo mimePartInfo;
							if (part.AttachedItemStructure != null)
							{
								mimePartInfo = part.AttachedItemStructure;
								itemToMimeConverter.SetSkeletonAndSmimeDoc(part.Skeleton, part.SmimeDocument);
							}
							else
							{
								mimePartInfo = itemToMimeConverter.CalculateMimeStructure(embeddedWriter.WriterCharset);
								part.AttachedItemStructure = mimePartInfo;
							}
							conversionResult.AddSubResult(itemToMimeConverter.WriteMimePart(embeddedWriter, limits, mimePartInfo, ItemToMimeConverter.MimeFlags.WriteMessageHeaders | (flags & ItemToMimeConverter.MimeFlags.SkipContent)));
						}
						else
						{
							itemToMimeConverter.ConvertItemToSummaryTnef(embeddedWriter, limits, true);
						}
					}
					converter.ownedEmbeddedMimeDocuments.AddRange(itemToMimeConverter.ExtractSkeletons());
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
			converter.journalAttachedItemNeedsSave[journalAttachmentIndex] |= conversionResult.ItemWasModified;
			converter.journalAttachmentNeedsSave[journalAttachmentIndex] |= (converter.journalAttachedItemNeedsSave[journalAttachmentIndex] || flag);
			conversionResult.ItemWasModified = flag;
			return conversionResult;
		}

		private static ConversionResult StaticWriteJournalBody(ItemToMimeConverter converter, MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			Item item = converter.JournalAttachedItems[0];
			string writerCharsetName = converter.mimeWriter.WriterCharsetName;
			Item item2 = converter.item;
			MimeStreamWriter mimeStreamWriter = converter.mimeWriter;
			ConversionResult result = new ConversionResult();
			mimeStreamWriter.WriteHeader(HeaderId.ContentType, "text/plain");
			mimeStreamWriter.WriteHeaderParameter("charset", writerCharsetName);
			if (item2.Body.IsBodyDefined)
			{
				converter.WriteMimeBodyContentId(result);
			}
			using (ItemToMimeConverter.MimeBodyOutputStream mimeBodyOutputStream = new ItemToMimeConverter.MimeBodyOutputStream(converter.options, mimeStreamWriter, MimePartContentType.TextPlain, writerCharsetName, !item2.Body.IsBodyDefined))
			{
				if (item2.Body.IsBodyDefined)
				{
					BodyReadConfiguration configuration = new BodyReadConfiguration(BodyFormat.TextPlain, writerCharsetName);
					using (Stream stream = item2.Body.OpenReadStream(configuration))
					{
						Util.StreamHandler.CopyStreamData(stream, mimeBodyOutputStream);
						goto IL_3D8;
					}
				}
				StringBuilder stringBuilder = new StringBuilder(1024);
				OutboundAddressCache outboundAddressCache = converter.JournalAttachedItemAddressCaches[0];
				Participant participant = outboundAddressCache.Participants[ConversionItemParticipants.ParticipantIndex.From];
				Participant participant2 = outboundAddressCache.Participants[ConversionItemParticipants.ParticipantIndex.Sender];
				if (participant2 != null && participant != null)
				{
					stringBuilder.Append("Sender: ");
					ItemToMimeConverter.WriteJournalMailbox(stringBuilder, participant2);
					stringBuilder.Append("\r\n");
					if (participant.EmailAddress != participant2.EmailAddress)
					{
						stringBuilder.Append("On-Behalf-Of: ");
						ItemToMimeConverter.WriteJournalMailbox(stringBuilder, participant);
						stringBuilder.Append("\r\n");
					}
				}
				else if (participant != null)
				{
					stringBuilder.Append("Sender: ");
					ItemToMimeConverter.WriteJournalMailbox(stringBuilder, participant);
					stringBuilder.Append("\r\n");
				}
				else if (participant2 != null)
				{
					stringBuilder.Append("Sender: ");
					ItemToMimeConverter.WriteJournalMailbox(stringBuilder, participant2);
					stringBuilder.Append("\r\n");
				}
				string text = item.TryGetProperty(InternalSchema.InternetMessageId) as string;
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.AppendFormat("Message-ID: {0}\r\n", text);
				}
				string text2 = item.TryGetProperty(ItemSchema.Subject) as string;
				if (!string.IsNullOrEmpty(text2))
				{
					stringBuilder.AppendFormat("Subject: {0}\r\n", text2);
				}
				string text3 = item2.TryGetProperty(InternalSchema.ElcAutoCopyLabel) as string;
				string text4 = null;
				string text5 = null;
				if (!string.IsNullOrEmpty(text3))
				{
					int num = text3.IndexOf(':');
					if (num != -1)
					{
						text5 = text3.Substring(0, num);
						text4 = text3.Substring(num + 1);
					}
					else
					{
						text4 = text3;
					}
				}
				if (text4 != null)
				{
					stringBuilder.AppendFormat("Label: {0}\r\n", text4);
				}
				if (text5 != null)
				{
					stringBuilder.AppendFormat("Mailbox: {0}\r\n", text5);
				}
				foreach (ConversionRecipientEntry conversionRecipientEntry in outboundAddressCache.Recipients)
				{
					string value;
					switch (conversionRecipientEntry.RecipientItemType)
					{
					case RecipientItemType.To:
						value = "To: ";
						break;
					case RecipientItemType.Cc:
						value = "Cc: ";
						break;
					case RecipientItemType.Bcc:
						value = "Bcc: ";
						break;
					default:
						value = "Recipient: ";
						break;
					}
					stringBuilder.Append(value);
					ItemToMimeConverter.WriteJournalMailbox(stringBuilder, conversionRecipientEntry.Participant);
					stringBuilder.Append("\r\n");
				}
				ExDateTime valueOrDefault = item.GetValueOrDefault<ExDateTime>(ItemSchema.SentTime, ExDateTime.MinValue);
				if (valueOrDefault != ExDateTime.MinValue)
				{
					stringBuilder.AppendFormat("SentUtc: {0}\r\n", valueOrDefault.UniversalTime);
				}
				ExDateTime valueOrDefault2 = item.GetValueOrDefault<ExDateTime>(ItemSchema.ReceivedTime, ExDateTime.MinValue);
				if (valueOrDefault2 != ExDateTime.MinValue)
				{
					stringBuilder.AppendFormat("ReceivedUtc: {0}\r\n", valueOrDefault2.UniversalTime);
				}
				char[] buffer = stringBuilder.ToString().ToCharArray();
				using (ConverterWriter converterWriter = new ConverterWriter(mimeBodyOutputStream, new TextToText
				{
					OutputEncoding = Charset.GetCharset(writerCharsetName).GetEncoding()
				}))
				{
					converterWriter.Write(buffer);
				}
				IL_3D8:;
			}
			return result;
		}

		private static Dictionary<string, string> GetReservedMimeHeadersList()
		{
			if (ItemToMimeConverter.reservedMimeHeadersList == null)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				for (int num = 0; num != ItemToMimeConverter.forbiddenNames.Length; num++)
				{
					string text = ItemToMimeConverter.forbiddenNames[num].ToLowerInvariant();
					dictionary.Add(text, text);
				}
				ItemToMimeConverter.reservedMimeHeadersList = dictionary;
			}
			return ItemToMimeConverter.reservedMimeHeadersList;
		}

		private static bool IsDlpHeaderAllowedToPenetrateFirewall(string propertyName)
		{
			return propertyName.StartsWith("X-Ms-Exchange-Organization-Dlp-", StringComparison.OrdinalIgnoreCase);
		}

		private void StartPart()
		{
			this.mimeWriter.StartPart(null);
		}

		private void StartPart(MimePartInfo part)
		{
			this.mimeWriter.StartPart(part);
		}

		private ConversionResult MultipartContentCallback(MimePartInfo part, ItemToMimeConverter.MimeFlags mimeFlags)
		{
			this.WriteMultipartContentTypeHeader(part);
			if ((mimeFlags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
			{
				return this.WriteMimeParts(part.Children, mimeFlags);
			}
			return new ConversionResult();
		}

		private ConversionResult MultipartAppleDoubleCallback(MimePartInfo part, ItemToMimeConverter.MimeFlags mimeFlags)
		{
			ConversionResult conversionResult = new ConversionResult();
			this.WriteMultipartContentTypeHeader(part);
			if ((mimeFlags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
			{
				using (Attachment attachment = this.item.AttachmentCollection.Open(part.AttachmentId, InternalSchema.ContentConversionProperties))
				{
					StreamAttachment streamAttachment = attachment as StreamAttachment;
					if (streamAttachment == null)
					{
						StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteMacAppleFile: not a mac attachment.");
						throw new ConversionFailedException(ConversionFailureReason.ConverterInternalFailure);
					}
					this.StartPart(part.Children[0]);
					this.WriteMacAppleFile(streamAttachment, mimeFlags);
					this.EndPart();
					this.StartPart(part.Children[1]);
					if (this.WriteMimeStreamAttachment(streamAttachment, mimeFlags))
					{
						attachment.Save();
						conversionResult.ItemWasModified = true;
					}
					this.EndPart();
				}
			}
			return conversionResult;
		}

		private void WriteMultipartContentTypeHeader(MimePartInfo part)
		{
			switch (part.ContentType)
			{
			case MimePartContentType.FirstMultipartType:
				this.WriteMultipartContentTypeHeader("multipart/alternative", part.PartIndex);
				return;
			case MimePartContentType.MultipartRelated:
				this.WriteMultipartContentTypeHeader("multipart/related", part.PartIndex);
				this.mimeWriter.WriteHeaderParameter("type", part.SubpartContentType);
				return;
			case MimePartContentType.MultipartMixed:
				this.WriteMultipartContentTypeHeader("multipart/mixed", part.PartIndex);
				return;
			case MimePartContentType.MultipartReportDsn:
				this.WriteMultipartContentTypeHeader("multipart/report", part.PartIndex);
				this.mimeWriter.WriteHeaderParameter("report-type", "delivery-status");
				return;
			case MimePartContentType.MultipartReportMdn:
				this.WriteMultipartContentTypeHeader("multipart/report", part.PartIndex);
				this.mimeWriter.WriteHeaderParameter("report-type", "disposition-notification");
				return;
			case MimePartContentType.MultipartDigest:
				this.WriteMultipartContentTypeHeader("multipart/digest", part.PartIndex);
				return;
			case MimePartContentType.MultipartFormData:
				this.WriteMultipartContentTypeHeader("multipart/form-data", part.PartIndex);
				return;
			case MimePartContentType.MultipartParallel:
				this.WriteMultipartContentTypeHeader("multipart/parallel", part.PartIndex);
				return;
			case MimePartContentType.MultipartAppleDouble:
				this.WriteMultipartContentTypeHeader("multipart/appledouble", part.PartIndex);
				return;
			default:
				return;
			}
		}

		private void WriteMultipartContentTypeHeader(string contentType, int index)
		{
			this.mimeWriter.WriteHeader(HeaderId.ContentType, contentType);
			this.mimeWriter.WriteHeaderParameter("boundary", this.GeneratePartBoundary(index));
		}

		private string GeneratePartBoundary(int boundaryIndex)
		{
			if (this.boundarySuffix == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string text = this.item.TryGetProperty(InternalSchema.InternetMessageId) as string;
				if (text != null)
				{
					int length = text.Length;
					for (int num = 0; num != length; num++)
					{
						char c = text[num];
						if (char.IsLetterOrDigit(c))
						{
							stringBuilder.Append(c);
						}
					}
				}
				if (stringBuilder.Length == 0)
				{
					byte[] array = this.item.TryGetProperty(InternalSchema.SearchKey) as byte[];
					if (array != null)
					{
						for (int num2 = 0; num2 != array.Length; num2++)
						{
							byte b = array[num2];
							stringBuilder.Append(65 + (b >> 4));
							stringBuilder.Append((int)(65 + (b & 15)));
						}
					}
				}
				if (stringBuilder.Length != 0)
				{
					if (stringBuilder.Length > 55)
					{
						stringBuilder.Length = 55;
					}
					this.boundarySuffix = stringBuilder.ToString();
				}
				else
				{
					this.boundarySuffix = Guid.NewGuid().ToString();
				}
			}
			return string.Format("_{0:000}_{1}_", boundaryIndex, this.boundarySuffix);
		}

		private void EndPart()
		{
			this.mimeWriter.EndPart();
		}

		private bool HandleTnefConversionAsMime()
		{
			if (this.itemType == MimeItemType.MimeMessageDsn || this.IsJournalingMessage)
			{
				MimePartInfo part = this.CalculateMimeStructure(this.mimeWriter.WriterCharset);
				this.WriteMimePart(part, ItemToMimeConverter.MimeFlags.WriteMessageHeaders);
				this.mimeWriter.Flush();
				return true;
			}
			return false;
		}

		private ConversionResult ConvertItemToMimeInternal(MimeStreamWriter writer, ConversionLimitsTracker limits)
		{
			ConversionResult result;
			using (StorageGlobals.SetTraceContext(this.item))
			{
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting item (ItemToMimeConverter.ConvertItemToMimeInternal)");
				this.mimeWriter = writer;
				this.limitsTracker = limits;
				this.messageFormat = ItemToMimeConverter.MessageFormatType.Mime;
				MimePartInfo part = this.CalculateMimeStructure(writer.WriterCharset);
				ConversionResult conversionResult = this.WriteMimePart(part, ItemToMimeConverter.MimeFlags.WriteMessageHeaders);
				this.mimeWriter.Flush();
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing item (ItemToMimeConverter.ConvertItemToMimeInternal)");
				result = conversionResult;
			}
			return result;
		}

		private void WriteComplexContentTypeHeader(string headerValue)
		{
			byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(headerValue);
			Header header = Header.Create(HeaderId.ContentType);
			MimeInternalHelpers.SetHeaderRawValue(header, bytes);
			this.mimeWriter.WriteHeader(header);
		}

		private void WriteJournalSpecificHeaders(Item attachedItem, OutboundAddressCache attachedAddressCache)
		{
			Participant participant = this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.From];
			if (participant != null)
			{
				this.mimeWriter.WriteHeader(HeaderId.From, null);
				this.WriteParticipant(participant);
			}
			if (attachedItem is MessageItem || attachedItem is CalendarItemBase)
			{
				this.WriteMimeRecipients(attachedAddressCache, HeaderId.To, RecipientItemType.To);
				this.WriteMimeRecipients(attachedAddressCache, HeaderId.Cc, RecipientItemType.Cc);
				this.WriteMimeRecipients(attachedAddressCache, HeaderId.Bcc, RecipientItemType.Bcc);
			}
			string valueOrDefault = attachedItem.GetValueOrDefault<string>(InternalSchema.Subject, string.Empty);
			this.mimeWriter.WriteHeader(HeaderId.Subject, valueOrDefault);
		}

		private ConversionResult WriteMimeParts(List<MimePartInfo> parts, ItemToMimeConverter.MimeFlags mimeFlags)
		{
			ConversionResult conversionResult = new ConversionResult();
			foreach (MimePartInfo part in parts)
			{
				conversionResult.AddSubResult(this.WriteMimePart(part, mimeFlags));
			}
			return conversionResult;
		}

		private ConversionResult WriteMimePart(MimePartInfo part, ItemToMimeConverter.MimeFlags mimeFlags)
		{
			if (part.SmimePart != null && part.SmimePart != part.SmimeDocument.RootPart)
			{
				mimeFlags &= ~ItemToMimeConverter.MimeFlags.WriteMessageHeaders;
				return part.WriterCallback(this, part, mimeFlags);
			}
			bool flag = part.Headers != null;
			this.StartPart(part);
			if (!flag)
			{
				if (part.SkeletonPart != null)
				{
					this.CopyHeadersFromSkeleton(part.SkeletonPart);
					if ((mimeFlags & ItemToMimeConverter.MimeFlags.WriteMessageHeaders) == ItemToMimeConverter.MimeFlags.WriteMessageHeaders)
					{
						this.WriteXMessageFlagAndReplyBy();
						this.WriteImportanceHeaders();
						this.WriteKeywords();
					}
				}
				else if ((mimeFlags & ItemToMimeConverter.MimeFlags.WriteMessageHeaders) == ItemToMimeConverter.MimeFlags.WriteMessageHeaders && !this.item.GetValueOrDefault<bool>(InternalSchema.IsSingleBodyICal))
				{
					this.WriteReceivedAndXHeaders();
					if (this.itemType == MimeItemType.MimeMessageJournalMsg || this.itemType == MimeItemType.MimeMessageJournalTnef)
					{
						this.WriteMimeHeaders(mimeFlags | ItemToMimeConverter.MimeFlags.SkipFromRecipientsAndSubject);
						this.WriteJournalSpecificHeaders(this.JournalAttachedItems[0], this.JournalAttachedItemAddressCaches[0]);
					}
					else
					{
						this.WriteMimeHeaders(mimeFlags);
					}
					this.WriteExtendedHeaders();
				}
			}
			mimeFlags &= ~ItemToMimeConverter.MimeFlags.WriteMessageHeaders;
			ConversionResult result = part.WriterCallback(this, part, mimeFlags);
			this.EndPart();
			return result;
		}

		private MimePartInfo CalculateSmimeMimeStructure(Charset itemCharset, ref int partIndex)
		{
			string contentType = this.SmimeAttachment.ContentType;
			if (contentType == null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateSmimeMimeStructure: content-type is missing.");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, null);
			}
			if (contentType.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase))
			{
				MimePartInfo mimePartInfo = (this.Skeleton == null) ? new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteSmimeMultipart), MimePartContentType.Attachment, ref partIndex) : new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticSkeletonSmimeMultipartToMime), MimePartContentType.Attachment, null, this.Skeleton.RootPart, this.Skeleton, ref partIndex);
				if (this.SmimeDocument == null || !this.SmimeDocument.RootPart.IsMultipart)
				{
					StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateSmimeMimeStructure - unable to open SMime attachment stream.");
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, null);
				}
				MimeStreamWriter.CalculateBodySize(mimePartInfo, this.SmimeDocument.RootPart);
				int num = 0;
				foreach (MimePart part in this.SmimeDocument.RootPart)
				{
					mimePartInfo.AddChild(this.CalculateSmimePartStructure(part, itemCharset, ref partIndex));
					num++;
				}
				if (num != 2)
				{
					StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateSmimeMimeStructure - corrupt multipart/signed content.");
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, null);
				}
				return mimePartInfo;
			}
			else
			{
				if (this.Skeleton != null)
				{
					return new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticSkeletonSmimeBlobToMime), MimePartContentType.Attachment, null, this.Skeleton.RootPart, this.Skeleton, ref partIndex);
				}
				string valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.NamedContentType);
				if (valueOrDefault != null && MimeStreamWriter.CheckAsciiHeaderValue(valueOrDefault))
				{
					byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(valueOrDefault);
					ContentTypeHeader contentTypeHeader = (ContentTypeHeader)Header.Create(HeaderId.ContentType);
					MimeInternalHelpers.SetHeaderRawValue(contentTypeHeader, bytes);
					if (ConvertUtils.MimeStringEquals(contentTypeHeader.MediaType, "multipart"))
					{
						StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateSmimeMimeStructure - multipart NamedContentType.");
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionInvalidSmimeContent, null);
					}
				}
				return new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteSmimeBlob), MimePartContentType.Attachment, ref partIndex);
			}
		}

		private MimePartInfo CalculateSmimePartStructure(MimePart part, Charset itemCharset, ref int partIndex)
		{
			MimePartInfo mimePartInfo = null;
			if (part.IsEmbeddedMessage)
			{
				mimePartInfo = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteSmimeLeaf), MimePartContentType.ItemAttachment, null, null, null, part, this.SmimeDocument, ref partIndex);
				using (IImapMimeProvider imapMimeProvider = IImapMimeProvider.CreateInstance((MimePart)part.FirstChild, this.smimeDocument))
				{
					mimePartInfo.AttachedItemStructure = imapMimeProvider.CalculateMimeStructure(itemCharset);
					goto IL_BC;
				}
			}
			mimePartInfo = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteSmimeLeaf), MimePartInfo.GetContentType(part.ContentType), null, null, null, part, this.SmimeDocument, ref partIndex);
			foreach (MimePart part2 in part)
			{
				mimePartInfo.AddChild(this.CalculateSmimePartStructure(part2, itemCharset, ref partIndex));
			}
			IL_BC:
			foreach (Header header in part.Headers)
			{
				mimePartInfo.AddHeader(header);
			}
			MimeStreamWriter.CalculateBodySize(mimePartInfo, part);
			return mimePartInfo;
		}

		private MimePartInfo CalculateJournalingMimeStructure(Charset itemCharset, ref int partIndex)
		{
			int num = 0;
			MimePartInfo mimePartInfo = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMultipartContent), MimePartContentType.MultipartMixed, ref partIndex);
			MimePartInfo.Callback writerCallback = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMimeBody);
			if (this.itemType != MimeItemType.MimeMessageSecondaryJournal)
			{
				writerCallback = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteJournalBody);
			}
			MimePartInfo mimePartInfo2 = new MimePartInfo(itemCharset, writerCallback, MimePartContentType.TextPlain, ref partIndex);
			if (this.itemType == MimeItemType.MimeMessageSecondaryJournal)
			{
				mimePartInfo2.IsBodyToRemoveFromSkeleton = true;
			}
			mimePartInfo.AddChild(mimePartInfo2);
			if (this.itemType == MimeItemType.MimeMessageSecondaryJournal)
			{
				foreach (Attachment attachment in this.JournalAttachments)
				{
					num++;
					bool flag = false;
					if (this.options.FilterAttachmentHandler != null && !this.options.FilterAttachmentHandler(this.item, attachment))
					{
						StorageGlobals.ContextTraceDebug<Attachment>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateJournalingMimeStructure: Skipping attachment {0}", attachment);
						flag = true;
					}
					if (attachment is StreamAttachment)
					{
						MimePartInfo.Callback writerCallback2;
						if (flag)
						{
							writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteEmptyMimeAttachment);
						}
						else
						{
							writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMimeAttachment);
						}
						mimePartInfo.AddChild(new MimePartInfo(itemCharset, writerCallback2, MimePartContentType.Attachment, attachment.Id, ref partIndex));
					}
					else if (attachment is ItemAttachment)
					{
						MimePartInfo.Callback writerCallback2;
						if (flag)
						{
							writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteEmptyJournalEmlItemAttachment);
						}
						else
						{
							writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteJournalEmlItemAttachment);
						}
						mimePartInfo.AddChild(new MimePartInfo(itemCharset, writerCallback2, MimePartContentType.ItemAttachment, attachment.Id, ref partIndex));
					}
				}
			}
			else
			{
				if (this.JournalAttachments.Length != 1)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateMimeStructure: unable to open journal message.");
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionCannotOpenJournalMessage, null);
				}
				Attachment attachment2 = this.JournalAttachments[0];
				num = 1;
				bool flag2 = false;
				if (this.options.FilterAttachmentHandler != null && !this.options.FilterAttachmentHandler(this.item, attachment2))
				{
					StorageGlobals.ContextTraceDebug<Attachment>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateJournalingMimeStructure: Skipping journal attachment {0}", attachment2);
					flag2 = true;
				}
				if (this.itemType == MimeItemType.MimeMessageJournalTnef && attachment2 is ItemAttachment)
				{
					MimePartInfo.Callback writerCallback2;
					if (flag2)
					{
						writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteEmptyJournalEmlItemAttachment);
					}
					else
					{
						writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteJournalEmlItemAttachment);
					}
					mimePartInfo.AddChild(new MimePartInfo(itemCharset, writerCallback2, MimePartContentType.ItemAttachment, attachment2.Id, ref partIndex));
				}
				else if (this.itemType == MimeItemType.MimeMessageJournalMsg && attachment2 is ItemAttachment)
				{
					MimePartInfo.Callback writerCallback2;
					if (flag2)
					{
						writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteEmptyJournalMsgItemAttachment);
					}
					else
					{
						writerCallback2 = new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteJournalMsgItemAttachment);
					}
					mimePartInfo.AddChild(new MimePartInfo(itemCharset, writerCallback2, MimePartContentType.Attachment, attachment2.Id, ref partIndex));
				}
			}
			if (num != 1 && num != 2)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::CalculateMimeStructure: unable to open journal message.");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionCannotOpenJournalMessage, null);
			}
			return mimePartInfo;
		}

		private Stream OpenMimeWriteStreamForSkeletonPart(MimePart skeletonPart)
		{
			Stream result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Stream stream = disposeGuard.Add<Stream>(this.mimeWriter.GetContentStream(false));
				switch (skeletonPart.ContentTransferEncoding)
				{
				case ContentTransferEncoding.QuotedPrintable:
					stream = new EncoderStream(stream, new QPEncoder(), EncoderStreamAccess.Write);
					break;
				case ContentTransferEncoding.Base64:
				case ContentTransferEncoding.BinHex:
					stream = new EncoderStream(stream, new Base64Encoder(), EncoderStreamAccess.Write);
					break;
				case ContentTransferEncoding.UUEncode:
				{
					string fileName = "unknown";
					int lengthLimit = 48;
					ContentDispositionHeader header = skeletonPart.Headers.FindFirst(HeaderId.ContentDisposition) as ContentDispositionHeader;
					string headerValue = MimeHelpers.GetHeaderValue(header, "attachment".Length);
					if (headerValue != null && headerValue.Equals("attachment", StringComparison.OrdinalIgnoreCase))
					{
						string headerParameter = MimeHelpers.GetHeaderParameter(header, "filename", lengthLimit);
						if (headerParameter != null)
						{
							fileName = headerParameter;
						}
					}
					stream = new EncoderStream(stream, new UUEncoder(fileName), EncoderStreamAccess.Write);
					break;
				}
				}
				disposeGuard.Success();
				result = stream;
			}
			return result;
		}

		private void CopyHeadersFromSkeleton(MimePart skeletonPart)
		{
			foreach (Header header in skeletonPart.Headers)
			{
				if (header.HeaderId != HeaderId.Keywords && header.HeaderId != HeaderId.Importance && header.HeaderId != HeaderId.Priority && header.HeaderId != HeaderId.XMSMailPriority && header.HeaderId != HeaderId.XPriority && header.HeaderId != HeaderId.MimeVersion && !string.Equals(header.Name, "X-Exchange-Mime-Skeleton-Content-Id", StringComparison.OrdinalIgnoreCase) && !string.Equals(header.Name, "X-Message-Flag", StringComparison.OrdinalIgnoreCase))
				{
					if (header.HeaderId == HeaderId.ContentTransferEncoding && skeletonPart.ContentTransferEncoding == ContentTransferEncoding.BinHex)
					{
						Header header2 = Header.Create(HeaderId.ContentTransferEncoding);
						header2.Value = "base64";
						this.mimeWriter.WriteHeader(header2);
					}
					else
					{
						if (header.HeaderId == HeaderId.To || header.HeaderId == HeaderId.Cc || header.HeaderId == HeaderId.Bcc)
						{
							AddressHeader addressHeader = header as AddressHeader;
							foreach (AddressItem addressItem in addressHeader)
							{
								if (addressItem is MimeRecipient)
								{
									this.limitsTracker.CountRecipient();
								}
							}
						}
						if (header.HeaderId != HeaderId.Bcc || this.options.DemoteBcc)
						{
							this.mimeWriter.WriteHeader(header);
						}
					}
				}
			}
		}

		private MimePartInfo CalculateMimeStructureForSkeleton(Charset itemCharset, ref int partIndex)
		{
			Dictionary<string, AttachmentLink> dictionary = new Dictionary<string, AttachmentLink>(this.item.AttachmentCollection.Count);
			Dictionary<string, bool> dictionary2 = new Dictionary<string, bool>(this.item.AttachmentCollection.Count);
			if (this.IsJournalingMessage)
			{
				foreach (Attachment attachment in this.JournalAttachments)
				{
					AttachmentLink attachmentLink = new AttachmentLink(attachment);
					dictionary[attachmentLink.ContentId] = attachmentLink;
					if (this.options.FilterAttachmentHandler == null || this.options.FilterAttachmentHandler(this.item, attachment))
					{
						dictionary2[attachmentLink.ContentId] = false;
					}
					else
					{
						dictionary2[attachmentLink.ContentId] = true;
					}
				}
			}
			else
			{
				foreach (AttachmentHandle handle in this.item.AttachmentCollection)
				{
					using (Attachment attachment2 = this.item.AttachmentCollection.Open(handle))
					{
						AttachmentLink attachmentLink2 = new AttachmentLink(attachment2);
						dictionary[attachmentLink2.ContentId] = attachmentLink2;
						if (this.options.FilterAttachmentHandler == null || this.options.FilterAttachmentHandler(this.item, attachment2))
						{
							dictionary2[attachmentLink2.ContentId] = false;
						}
						else
						{
							dictionary2[attachmentLink2.ContentId] = true;
						}
					}
				}
			}
			return this.CalculateMimeStructureForSkeleton(itemCharset, this.Skeleton.RootPart, dictionary, dictionary2, ref partIndex);
		}

		private MimePartInfo CalculateMimeStructureForSkeleton(Charset itemCharset, MimePart part, Dictionary<string, AttachmentLink> attachments, Dictionary<string, bool> skipAttachments, ref int partIndex)
		{
			if (part.IsMultipart)
			{
				MimePartInfo mimePartInfo = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticMultipartSkeletonToMime), MimePartInfo.GetContentType(part.ContentType), null, part, this.Skeleton, ref partIndex);
				foreach (MimePart part2 in part)
				{
					mimePartInfo.AddChild(this.CalculateMimeStructureForSkeleton(itemCharset, part2, attachments, skipAttachments, ref partIndex));
				}
				return mimePartInfo;
			}
			string text = null;
			bool flag;
			using (Stream rawContentReadStream = part.GetRawContentReadStream())
			{
				if (part.IsEmbeddedMessage)
				{
					flag = (rawContentReadStream.Length <= 2L);
				}
				else
				{
					flag = (rawContentReadStream.Length == 0L);
				}
			}
			if (flag)
			{
				text = MimeHelpers.GetHeaderValue(part.Headers, HeaderId.ContentId, null);
				if (!string.IsNullOrWhiteSpace(text))
				{
					text = ConvertUtils.ExtractMimeContentId(text);
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					text = null;
					Header header = part.Headers.FindFirst("X-Exchange-Mime-Skeleton-Content-Id");
					if (header != null)
					{
						text = ConvertUtils.ExtractMimeContentId(header.Value);
					}
				}
			}
			AttachmentLink attachmentLink;
			bool flag2;
			MimePartInfo.Callback writerCallback;
			if (text != null && attachments.TryGetValue(text, out attachmentLink) && skipAttachments.TryGetValue(text, out flag2))
			{
				if (flag2)
				{
					writerCallback = new MimePartInfo.Callback(ItemToMimeConverter.StaticSkeletonEmptyAttachmentToMime);
				}
				else
				{
					writerCallback = new MimePartInfo.Callback(ItemToMimeConverter.StaticSkeletonAttachmentToMime);
				}
				return new MimePartInfo(itemCharset, writerCallback, (attachmentLink.AttachmentType == AttachmentType.EmbeddedMessage && this.itemType != MimeItemType.MimeMessageJournalMsg) ? MimePartContentType.ItemAttachment : MimePartContentType.Attachment, attachmentLink.AttachmentId, part, this.Skeleton, ref partIndex);
			}
			string text2 = this.item.TryGetProperty(InternalSchema.BodyContentId) as string;
			writerCallback = new MimePartInfo.Callback(ItemToMimeConverter.StaticSkeletonCopyToMime);
			if (text2 != null && text != null && text2 == text)
			{
				writerCallback = new MimePartInfo.Callback(ItemToMimeConverter.StaticSkeletonBodyToMime);
			}
			return new MimePartInfo(itemCharset, writerCallback, MimePartInfo.GetContentType(part.ContentType), null, part, this.Skeleton, ref partIndex);
		}

		private MimePartInfo AddICalBodyPart(MimePartInfo bodyPart, ref int partIndex, Charset itemCharset)
		{
			MimePartInfo newChild = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMimeBody), MimePartContentType.Calendar, ref partIndex);
			if (!bodyPart.IsMultipart)
			{
				MimePartInfo mimePartInfo = new MimePartInfo(itemCharset, new MimePartInfo.Callback(ItemToMimeConverter.StaticWriteMultipartContent), MimePartContentType.FirstMultipartType, ref partIndex);
				mimePartInfo.AddChild(bodyPart);
				mimePartInfo.AddChild(newChild);
				bodyPart = mimePartInfo;
			}
			else
			{
				bodyPart.AddChild(newChild);
			}
			return bodyPart;
		}

		private int GetJournalAttachmentIndex(AttachmentId id)
		{
			if (!this.IsJournalingMessage)
			{
				throw new InvalidOperationException("Non-journaling message.");
			}
			if (this.JournalAttachments.Length == 1 || this.JournalAttachments[0].Id.Equals(id))
			{
				return 0;
			}
			return 1;
		}

		private ConversionResult WriteMimeAttachment(MimePartInfo part, ItemToMimeConverter.MimeFlags flags)
		{
			this.limitsTracker.CountMessageAttachment();
			ConversionResult conversionResult = new ConversionResult();
			Attachment attachment = null;
			try
			{
				if (this.IsJournalingMessage)
				{
					attachment = this.JournalAttachments[this.GetJournalAttachmentIndex(part.AttachmentId)];
				}
				else
				{
					attachment = this.item.AttachmentCollection.Open(part.AttachmentId, InternalSchema.ContentConversionProperties);
				}
				if (attachment == null)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteMimeAttachment: unable to open attachment.");
					throw new ConversionFailedException(ConversionFailureReason.ConverterInternalFailure, null);
				}
				using (StorageGlobals.SetTraceContext(attachment))
				{
					StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting attachment (ItemToMimeConverter.WriteMimeAttachment)");
					bool flag = false;
					StreamAttachmentBase streamAttachmentBase = attachment as StreamAttachmentBase;
					if (streamAttachmentBase != null)
					{
						flag = this.WriteMimeStreamAttachment(streamAttachmentBase, flags);
					}
					else
					{
						ItemAttachment itemAttachment = attachment as ItemAttachment;
						if (itemAttachment != null)
						{
							this.WriteMimeItemAttachment(part, itemAttachment, flags, out flag);
						}
					}
					if (flag)
					{
						if (this.ItemType == MimeItemType.MimeMessageSecondaryJournal)
						{
							this.journalAttachmentNeedsSave[this.GetJournalAttachmentIndex(part.AttachmentId)] = true;
						}
						else
						{
							attachment.Save();
						}
						conversionResult.ItemWasModified = true;
					}
					StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing attachment (ItemToMimeConverter.WriteMimeAttachment)");
				}
			}
			finally
			{
				if (this.ItemType != MimeItemType.MimeMessageSecondaryJournal && attachment != null)
				{
					attachment.Dispose();
				}
			}
			return conversionResult;
		}

		private bool WriteMimeAttachmentContentId(Attachment attachment)
		{
			bool result = false;
			bool skeletonContentId = false;
			AttachmentLink attachmentLink;
			string text = ItemToMimeConverter.GetAttachmentContentId(attachment, this.attachmentLinks, out attachmentLink);
			if ((this.flags & ConverterFlags.GenerateSkeleton) == ConverterFlags.GenerateSkeleton)
			{
				if (text.Length == 0)
				{
					text = AttachmentLink.CreateContentId(this.item.CoreItem, attachment.Id, this.options.ImceaEncapsulationDomain);
					attachment.ContentId = text;
					skeletonContentId = true;
					result = true;
				}
				if (attachmentLink != null)
				{
					attachmentLink.ContentId = text;
					attachmentLink.MakeAttachmentChanges(attachment, true);
					result = true;
				}
			}
			this.WriteContentId(text, skeletonContentId);
			return result;
		}

		private void WriteContentId(string contentId, bool skeletonContentId)
		{
			contentId = contentId.Trim();
			if (contentId.Length != 0)
			{
				if (contentId[0] != '<' || contentId[contentId.Length - 1] != '>')
				{
					contentId = '<' + contentId + '>';
				}
				if (skeletonContentId)
				{
					this.mimeWriter.WriteHeader("X-Exchange-Mime-Skeleton-Content-Id", contentId);
					return;
				}
				this.mimeWriter.WriteHeader(HeaderId.ContentId, contentId);
			}
		}

		private bool WriteMimeContactAttachment(Contact contact, Attachment attachment, ItemToMimeConverter.MimeFlags flags)
		{
			this.mimeWriter.WriteHeader(HeaderId.ContentType, "text/directory");
			this.mimeWriter.WriteHeaderParameter("charset", "utf-8");
			this.mimeWriter.WriteHeaderParameter("profile", "vCard");
			this.mimeWriter.WriteHeader(HeaderId.ContentDescription, contact.DisplayName);
			this.WriteContentDisposition("attachment", attachment, 0L, ItemToMimeConverter.FilenameFromSubject(contact.DisplayName, ".vcf", ServerStrings.ExDefaultContactFilename));
			bool result = this.WriteMimeAttachmentContentId(attachment);
			ByteEncoder byteEncoder = null;
			try
			{
				if (this.options.EncodeAttachmentsAsBinhex)
				{
					this.mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, "binhex");
					byteEncoder = new BinHexEncoder();
				}
				else
				{
					this.mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, "quoted-printable");
					byteEncoder = new QPEncoder();
				}
				if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) == ItemToMimeConverter.MimeFlags.SkipContent)
				{
					return result;
				}
				using (Stream contentStream = this.mimeWriter.GetContentStream(false))
				{
					using (Stream stream = new EncoderStream(contentStream, byteEncoder, EncoderStreamAccess.Write))
					{
						OutboundVCardConverter.Convert(stream, Encoding.UTF8, contact, this.options, this.limitsTracker);
					}
				}
			}
			finally
			{
				if (byteEncoder != null)
				{
					byteEncoder.Dispose();
					byteEncoder = null;
				}
			}
			return result;
		}

		private ConversionResult WriteMimeItemAttachment(MimePartInfo part, ItemAttachment attachment, ItemToMimeConverter.MimeFlags flags, out bool attachmentNeedsSave)
		{
			attachmentNeedsSave = false;
			ConversionResult conversionResult = new ConversionResult();
			Item item = ConvertUtils.OpenAttachedItem(attachment);
			if (item != null)
			{
				try
				{
					if (ObjectClass.IsContact(item.ClassName))
					{
						attachmentNeedsSave = this.WriteMimeContactAttachment(item as Contact, attachment, flags);
					}
					else
					{
						this.limitsTracker.StartEmbeddedMessage();
						this.mimeWriter.WriteHeader(HeaderId.ContentType, "message/rfc822");
						this.WriteContentDisposition("attachment", attachment, 0L, null);
						attachmentNeedsSave = this.WriteMimeAttachmentContentId(attachment);
						Stream stream = null;
						bool flag = (this.flags & ConverterFlags.GenerateSkeleton) == ConverterFlags.GenerateSkeleton && ItemConversion.IsItemClassConvertibleToMime(item.ClassName);
						try
						{
							if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
							{
								using (ItemToMimeConverter itemToMimeConverter = new ItemToMimeConverter(item, this.options, this.flags | ConverterFlags.IsEmbeddedMessage))
								{
									if (this.resolveAddresses && ObjectClass.IsTaskRequest(this.item.ClassName) && ObjectClass.IsTask(item.ClassName))
									{
										itemToMimeConverter.resolveAddresses = true;
									}
									if (flag)
									{
										stream = this.OpenSkeletonStream(item, conversionResult);
									}
									using (MimeStreamWriter embeddedWriter = this.mimeWriter.GetEmbeddedWriter(itemToMimeConverter.GetItemMimeEncodingOptions(this.options), stream, this.options))
									{
										if (this.options.AllowPartialStnefConversion && !ItemConversion.IsItemClassConvertibleToMime(item.ClassName))
										{
											itemToMimeConverter.ConvertItemToSummaryTnef(embeddedWriter, this.limitsTracker, true);
										}
										else
										{
											MimePartInfo mimePartInfo;
											if (part.AttachedItemStructure != null)
											{
												mimePartInfo = part.AttachedItemStructure;
												itemToMimeConverter.SetSkeletonAndSmimeDoc(part.Skeleton, part.SmimeDocument);
											}
											else
											{
												mimePartInfo = itemToMimeConverter.CalculateMimeStructure(embeddedWriter.WriterCharset);
												part.AttachedItemStructure = mimePartInfo;
											}
											flags |= ItemToMimeConverter.MimeFlags.WriteMessageHeaders;
											conversionResult.AddSubResult(itemToMimeConverter.WriteMimePart(embeddedWriter, this.limitsTracker, mimePartInfo, flags));
										}
									}
									this.ownedEmbeddedMimeDocuments.AddRange(itemToMimeConverter.ExtractSkeletons());
								}
							}
						}
						finally
						{
							if (stream != null)
							{
								stream.Dispose();
							}
						}
						if (conversionResult.ItemWasModified)
						{
							item.Save(SaveMode.ResolveConflicts);
							attachmentNeedsSave = true;
						}
						this.limitsTracker.EndEmbeddedMessage();
					}
					goto IL_22C;
				}
				finally
				{
					item.Dispose();
				}
			}
			this.mimeWriter.WriteHeader(HeaderId.ContentType, "text/plain");
			this.mimeWriter.WriteHeaderParameter("charset", "us-ascii");
			this.mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, "7-bit");
			IL_22C:
			conversionResult.ItemWasModified |= attachmentNeedsSave;
			return conversionResult;
		}

		private ConversionResult WriteMacAppleFile(MimePartInfo partInfo, ItemToMimeConverter.MimeFlags flags)
		{
			using (Attachment attachment = this.item.AttachmentCollection.Open(partInfo.AttachmentId, InternalSchema.ContentConversionProperties))
			{
				StreamAttachment streamAttachment = attachment as StreamAttachment;
				if (streamAttachment == null)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteMacAppleFile: not a mac attachment.");
					throw new ConversionFailedException(ConversionFailureReason.ConverterInternalFailure);
				}
				this.WriteMacAppleFile(streamAttachment, flags);
			}
			return new ConversionResult();
		}

		private void WriteMacAppleFile(StreamAttachment streamAttachment, ItemToMimeConverter.MimeFlags flags)
		{
			Stream stream = null;
			try
			{
				object obj = streamAttachment.TryGetProperty(InternalSchema.AttachmentMacInfo);
				if (obj is byte[])
				{
					stream = new MemoryStream(obj as byte[], false);
				}
				else if (PropertyError.IsPropertyValueTooBig(obj))
				{
					stream = streamAttachment.OpenPropertyStream(InternalSchema.AttachmentMacInfo, PropertyOpenMode.ReadOnly);
				}
				this.mimeWriter.WriteHeader(HeaderId.ContentType, "application/applefile");
				this.mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, "base64");
				if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
				{
					ByteEncoder encoder = new Base64Encoder();
					using (Stream contentStream = this.mimeWriter.GetContentStream(true))
					{
						using (Stream stream2 = new EncoderStream(contentStream, encoder, EncoderStreamAccess.Write))
						{
							if (stream != null)
							{
								Util.StreamHandler.CopyStreamData(stream, stream2);
							}
							else
							{
								using (Stream rawContentStream = streamAttachment.GetRawContentStream(PropertyOpenMode.ReadOnly))
								{
									string text = null;
									byte[] array = null;
									MimeAppleTranscoder.MacBinToApplefile(rawContentStream, stream2, out text, out array);
								}
							}
						}
					}
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
		}

		private bool WriteMimeStreamAttachment(StreamAttachmentBase attachment, ItemToMimeConverter.MimeFlags flags)
		{
			OleAttachment oleAttachment = attachment as OleAttachment;
			AttachmentLink attachmentLink = AttachmentLink.Find(attachment.Id, this.attachmentLinks);
			bool flag = (attachmentLink == null) ? attachment.IsInline : attachmentLink.IsInline(true);
			if (oleAttachment == null)
			{
				string text = (!string.IsNullOrEmpty(attachment.ContentType)) ? attachment.ContentType : attachment.CalculatedContentType;
				if (text.Equals("message/rfc822", StringComparison.OrdinalIgnoreCase) || text.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase) || text.Equals("application/applefile", StringComparison.OrdinalIgnoreCase) || text.Equals("application/mac-binhex40", StringComparison.OrdinalIgnoreCase))
				{
					text = "application/octet-stream";
				}
				if (!MimeStreamWriter.CheckAsciiHeaderValue(text))
				{
					text = "application/octet-stream";
				}
				ArgumentException ex = null;
				try
				{
					this.mimeWriter.WriteHeader(HeaderId.ContentType, text);
					this.mimeWriter.WriteHeaderParameter("name", attachment.FileName);
				}
				catch (ArgumentException ex2)
				{
					ex = ex2;
				}
				if (ex != null)
				{
					string text2 = attachment.CalculatedContentType;
					text2 = (text2 ?? "application/octet-stream");
					try
					{
						this.mimeWriter.WriteHeader(HeaderId.ContentType, text2);
						this.mimeWriter.WriteHeaderParameter("name", attachment.FileName);
					}
					catch (ArgumentException)
					{
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ex);
					}
				}
				this.mimeWriter.WriteHeader(HeaderId.ContentDescription, attachment.DisplayName);
				this.WriteContentDisposition(flag ? "inline" : "attachment", attachment, attachment.Size, attachment.FileName);
			}
			else
			{
				string text3 = this.CreateOleAttachmentFilename(attachment.DisplayName);
				this.mimeWriter.WriteHeader(HeaderId.ContentType, "image/jpeg");
				this.mimeWriter.WriteHeaderParameter("name", text3);
				this.mimeWriter.WriteHeader(HeaderId.ContentDescription, text3);
				this.WriteContentDisposition(flag ? "inline" : "attachment", attachment, 0L, text3);
			}
			bool flag2 = this.WriteMimeAttachmentContentId(attachment);
			if (attachment.ContentLocation != null)
			{
				this.mimeWriter.WriteHeader(HeaderId.ContentLocation, attachment.ContentLocation.ToString());
			}
			if (attachment.ContentBase != null)
			{
				this.mimeWriter.WriteHeader(HeaderId.ContentBase, attachment.ContentBase.ToString());
			}
			ByteEncoder byteEncoder = null;
			bool result;
			try
			{
				if (this.options.EncodeAttachmentsAsBinhex)
				{
					this.mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, "binhex");
					byteEncoder = new BinHexEncoder();
				}
				else
				{
					this.mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, "base64");
					byteEncoder = new Base64Encoder();
				}
				if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) == ItemToMimeConverter.MimeFlags.SkipContent)
				{
					result = flag2;
				}
				else
				{
					using (Stream contentStream = this.mimeWriter.GetContentStream(false))
					{
						using (Stream stream = new EncoderStream(contentStream, byteEncoder, EncoderStreamAccess.Write))
						{
							try
							{
								if (oleAttachment != null)
								{
									if (!oleAttachment.TryConvertToImage(stream, ImageFormat.Jpeg))
									{
										ConvertUtils.SaveDefaultImage(stream);
									}
								}
								else
								{
									using (Stream contentStream2 = attachment.GetContentStream(PropertyOpenMode.ReadOnly))
									{
										Util.StreamHandler.CopyStreamData(contentStream2, stream);
									}
								}
							}
							catch (ObjectNotFoundException arg)
							{
								StorageGlobals.ContextTraceDebug<ObjectNotFoundException>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteMimeStreamAttachment - Unable to open attachment stream: {0}", arg);
							}
							catch (NoSupportException arg2)
							{
								StorageGlobals.ContextTraceDebug<NoSupportException>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteMimeStreamAttachment - Unable to open attachment stream: {0}", arg2);
							}
						}
					}
					result = flag2;
				}
			}
			finally
			{
				if (byteEncoder != null)
				{
					byteEncoder.Dispose();
					byteEncoder = null;
				}
			}
			return result;
		}

		private Stream OpenSkeletonStream(Item item, ConversionResult result)
		{
			Stream result2 = null;
			PropertyError propertyError = item.TryGetProperty(InternalSchema.MimeSkeleton) as PropertyError;
			if (propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.NotFound)
			{
				result2 = item.OpenPropertyStream(InternalSchema.MimeSkeleton, PropertyOpenMode.Create);
				result.ItemWasModified = true;
			}
			return result2;
		}

		private ConversionResult WriteMimeBody(MimePartInfo partInfo, ItemToMimeConverter.MimeFlags flags)
		{
			return this.WriteMimeBody(partInfo.ContentType, flags, !partInfo.IsBodyToRemoveFromSkeleton);
		}

		private ConversionResult WriteMimeBody(MimePartContentType contentType, ItemToMimeConverter.MimeFlags flags, bool copyToSkeleton)
		{
			ConversionResult result = new ConversionResult();
			this.limitsTracker.CountMessageBody();
			if (contentType == MimePartContentType.Calendar)
			{
				object obj = this.item.TryGetProperty(InternalSchema.InboundICalStream);
				if (obj is byte[] || PropertyError.IsPropertyValueTooBig(obj))
				{
					using (Stream stream = this.item.OpenPropertyStream(InternalSchema.InboundICalStream, PropertyOpenMode.ReadOnly))
					{
						using (MimeReader mimeReader = new MimeReader(stream))
						{
							mimeReader.ReadNextPart();
							MimeHeaderReader headerReader = mimeReader.HeaderReader;
							while (headerReader.ReadNextHeader())
							{
								this.mimeWriter.WriteHeader(Header.ReadFrom(headerReader));
							}
							if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
							{
								using (Stream contentStream = this.mimeWriter.GetContentStream(true))
								{
									using (Stream rawContentReadStream = mimeReader.GetRawContentReadStream())
									{
										Util.StreamHandler.CopyStreamData(rawContentReadStream, contentStream);
									}
								}
							}
							return result;
						}
					}
				}
			}
			this.mimeWriter.WriteHeader(HeaderId.ContentType, MimePartInfo.GetContentTypeName(contentType));
			string text = (contentType == MimePartContentType.Calendar) ? "utf-8" : this.mimeWriter.WriterCharsetName;
			this.mimeWriter.WriteHeaderParameter("charset", text);
			CalendarMethod icalMethod = CalendarUtil.GetICalMethod(this.item);
			string text2 = null;
			if (contentType == MimePartContentType.Calendar)
			{
				if (icalMethod == CalendarMethod.None)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcOutboundMimeTracer, "InetToMimeConverter::WriteMimeBody: unknown calendar method.");
					throw new ConversionFailedException(ConversionFailureReason.ConverterInternalFailure);
				}
				string parameterValue = CalendarUtil.CalendarMethodToString(icalMethod);
				this.mimeWriter.WriteHeaderParameter("method", parameterValue);
			}
			bool flag = (flags & ItemToMimeConverter.MimeFlags.SkipContent) == ItemToMimeConverter.MimeFlags.SkipContent;
			if (!copyToSkeleton)
			{
				this.WriteMimeBodyContentId(result);
			}
			if (!string.IsNullOrEmpty(this.options.OwaServer) && icalMethod != CalendarMethod.None && contentType != MimePartContentType.Calendar && this.item.Id != null)
			{
				text2 = ItemToMimeConverter.BuildOwaLink(this.item, contentType, this.options.OwaServer);
				copyToSkeleton = true;
			}
			using (ItemToMimeConverter.MimeBodyOutputStream mimeBodyOutputStream = new ItemToMimeConverter.MimeBodyOutputStream(this.options, this.mimeWriter, contentType, text, copyToSkeleton))
			{
				if (!flag || !mimeBodyOutputStream.IsReadyToStream)
				{
					switch (contentType)
					{
					case MimePartContentType.TextPlain:
						break;
					case MimePartContentType.TextHtml:
					{
						BodyReadConfiguration bodyReadConfiguration = new BodyReadConfiguration(BodyFormat.TextHtml, text);
						DefaultHtmlCallbacks defaultHtmlCallbacks = new DefaultHtmlCallbacks(this.item.CoreItem, false);
						defaultHtmlCallbacks.RemoveLinksToNonImageAttachments = true;
						defaultHtmlCallbacks.SetContentIdDomain(this.options.ImceaEncapsulationDomain);
						bodyReadConfiguration.ConversionCallback = defaultHtmlCallbacks;
						bodyReadConfiguration.ConversionCallback.InitializeAttachmentLinks(this.attachmentLinks);
						if (text2 != null)
						{
							bodyReadConfiguration.AddInjectedText(text2, null, BodyInjectionFormat.Html);
						}
						using (BodyReadStream bodyReadStream = (BodyReadStream)this.item.Body.OpenReadStream(bodyReadConfiguration))
						{
							Util.StreamHandler.CopyStreamData(bodyReadStream, mimeBodyOutputStream);
							this.attachmentLinks = bodyReadStream.AttachmentLinks;
							goto IL_33E;
						}
						break;
					}
					case MimePartContentType.TextEnriched:
						goto IL_2EC;
					case MimePartContentType.Tnef:
						goto IL_323;
					case MimePartContentType.Calendar:
						this.attachmentLinks = CalendarDocument.ItemToICal(this.item, this.attachmentLinks, this.AddressCache, mimeBodyOutputStream, text, this.options);
						goto IL_33E;
					default:
						goto IL_323;
					}
					BodyReadConfiguration bodyReadConfiguration2 = new BodyReadConfiguration(BodyFormat.TextPlain, text);
					if (text2 != null)
					{
						bodyReadConfiguration2.AddInjectedText(text2, null, BodyInjectionFormat.Text);
					}
					using (Stream stream2 = this.item.Body.OpenReadStream(bodyReadConfiguration2))
					{
						Util.StreamHandler.CopyStreamData(stream2, mimeBodyOutputStream);
						goto IL_33E;
					}
					IL_2EC:
					this.WriteMimeBodyAsEtf(mimeBodyOutputStream, text, text2);
					goto IL_33E;
					IL_323:
					StorageGlobals.ContextTraceError<MimePartContentType>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteMimeBody: wrong content-type, {0}.", contentType);
					throw new InvalidOperationException("format not one of text,html,or enriched");
				}
				IL_33E:;
			}
			return result;
		}

		private void WriteMimeBodyContentId(ConversionResult result)
		{
			string text = this.item.TryGetProperty(InternalSchema.BodyContentId) as string;
			if ((this.flags & ConverterFlags.GenerateSkeleton) == ConverterFlags.GenerateSkeleton && text == null)
			{
				text = AttachmentLink.CreateContentId(this.item.CoreItem, null, this.options.ImceaEncapsulationDomain);
				this.item[InternalSchema.BodyContentId] = text;
				result.ItemWasModified = true;
			}
			if (text != null)
			{
				this.WriteContentId(text, false);
			}
		}

		private void WriteMimeBodyAsEtf(Stream outputStream, string charsetName, string prefix)
		{
			ItemToMimeConverter.<>c__DisplayClass9 CS$<>8__locals1 = new ItemToMimeConverter.<>c__DisplayClass9();
			CS$<>8__locals1.outputStream = outputStream;
			CS$<>8__locals1.charsetName = charsetName;
			CS$<>8__locals1.prefix = prefix;
			BodyReadConfiguration bodyReadConfiguration = new BodyReadConfiguration(BodyFormat.TextHtml, CS$<>8__locals1.charsetName);
			DefaultHtmlCallbacks defaultHtmlCallbacks = new DefaultHtmlCallbacks(this.item.CoreItem, false);
			defaultHtmlCallbacks.SetContentIdDomain(this.options.ImceaEncapsulationDomain);
			bodyReadConfiguration.ConversionCallback = defaultHtmlCallbacks;
			bodyReadConfiguration.ConversionCallback.InitializeAttachmentLinks(this.attachmentLinks);
			using (TextReader reader = this.item.Body.OpenTextReader(bodyReadConfiguration))
			{
				ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "ItemToMimeConverter.WriteMimeBodyAsEtf", ServerStrings.ConversionBodyConversionFailed, delegate
				{
					Encoding encoding = Charset.GetEncoding(CS$<>8__locals1.charsetName);
					HtmlToEnriched htmlToEnriched = new HtmlToEnriched();
					htmlToEnriched.OutputEncoding = encoding;
					htmlToEnriched.Header = CS$<>8__locals1.prefix;
					htmlToEnriched.HeaderFooterFormat = HeaderFooterFormat.Html;
					using (TextWriter textWriter = new ConverterWriter(CS$<>8__locals1.outputStream, htmlToEnriched))
					{
						Util.StreamHandler.CopyText(reader, textWriter);
					}
				});
				BodyTextReader bodyTextReader = reader as BodyTextReader;
				if (bodyTextReader != null)
				{
					this.attachmentLinks = bodyTextReader.AttachmentLinks;
				}
			}
		}

		private ConversionResult WriteTnefPart(MimePartInfo partInfo, ItemToMimeConverter.MimeFlags flags)
		{
			return this.WriteTnefPart(partInfo.ContentType, flags);
		}

		private ConversionResult WriteTnefPart(MimePartContentType contentType, ItemToMimeConverter.MimeFlags flags)
		{
			ConversionResult result = new ConversionResult();
			this.WriteTnefHeaders();
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) != ItemToMimeConverter.MimeFlags.SkipContent)
			{
				this.WriteTnefAttachment();
			}
			return result;
		}

		private void WriteTnefHeaders()
		{
			this.mimeWriter.WriteHeader(HeaderId.ContentDisposition, "attachment");
			this.mimeWriter.WriteHeaderParameter("filename", "winmail.dat");
			this.mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, "base64");
			this.mimeWriter.WriteHeader(HeaderId.ContentType, "application/ms-tnef");
			this.mimeWriter.WriteHeaderParameter("name", "winmail.dat");
		}

		private void WriteTnefAttachment()
		{
			using (Stream stream = new EncoderStream(this.mimeWriter.GetContentStream(false), new Base64Encoder(), EncoderStreamAccess.Write))
			{
				using (ItemToTnefConverter itemToTnefConverter = new ItemToTnefConverter(this.item, this.AddressCache, stream, this.options, this.limitsTracker, TnefType.LegacyTnef, this.tnefCorrelationKey, false))
				{
					itemToTnefConverter.Convert();
				}
			}
		}

		private string CreateOleAttachmentFilename(string originalFilename)
		{
			if (originalFilename == null)
			{
				return string.Format("{0}.jpg", ++this.untitledOleAttachCounter);
			}
			int num = originalFilename.LastIndexOf('.');
			string arg;
			if (num <= 0)
			{
				arg = originalFilename;
			}
			else
			{
				arg = originalFilename.Substring(0, num);
			}
			return string.Format("{0} {1}.jpg", arg, ++this.untitledOleAttachCounter);
		}

		private void WriteParticipant(Participant participant)
		{
			if (participant == null)
			{
				return;
			}
			string participantSmtpAddress = ItemToMimeConverter.GetParticipantSmtpAddress(participant, this.options);
			string displayName = participant.DisplayName;
			if (this.options.SuppressDisplayName)
			{
				this.mimeWriter.WriteMailBox(null, participantSmtpAddress);
				return;
			}
			this.mimeWriter.WriteMailBox(displayName, participantSmtpAddress);
		}

		private string GetDualPropertyValue(StorePropertyDefinition primaryPropertyDefinition, StorePropertyDefinition secondaryPropertyDefinition)
		{
			object obj = this.item.TryGetProperty(primaryPropertyDefinition);
			if (obj != null && !(obj is PropertyError))
			{
				return obj.ToString();
			}
			string text = this.item.TryGetProperty(secondaryPropertyDefinition) as string;
			if (text != null)
			{
				return text;
			}
			return null;
		}

		private void WriteContentClass()
		{
			string valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.ItemClass);
			string text = null;
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Microsoft.Fax"))
				{
					text = "fax";
				}
				else if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Microsoft.Fax.CA"))
				{
					text = "fax-ca";
				}
				else if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Microsoft.Voicemail.UM"))
				{
					text = "voice";
				}
				else if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Microsoft.Voicemail.UM.CA"))
				{
					text = "voice-ca";
				}
				else if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Microsoft.Conversation.Voice"))
				{
					text = "voice-uc";
				}
				else if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Microsoft.Partner.UM"))
				{
					text = "MS-Exchange-UM-Partner";
				}
				else if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Microsoft.Missed.Voice"))
				{
					text = "missedcall";
				}
				else if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Post.RSS"))
				{
					text = "RSS";
				}
				else if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Sharing"))
				{
					text = "Sharing";
				}
				else if (ObjectClass.IsOfClass(valueOrDefault, "IPM.Note.Custom") && valueOrDefault.Length > "IPM.Note.Custom".Length + 1)
				{
					string str = valueOrDefault.Substring("IPM.Note.Custom".Length + 1);
					text = "urn:content-class:custom." + str;
				}
				else if (ObjectClass.IsOfClass(valueOrDefault, "IPM.InfoPathForm") && valueOrDefault.Length > "IPM.InfoPathForm".Length + 1)
				{
					string valueOrDefault2 = this.item.GetValueOrDefault<string>(InternalSchema.InfoPathFormName);
					if (valueOrDefault2 != null)
					{
						int num = valueOrDefault.IndexOf('.', "IPM.InfoPathForm".Length + 1);
						string str2 = (num > 0) ? valueOrDefault.Substring("IPM.InfoPathForm".Length + 1, num - ("IPM.InfoPathForm".Length + 1)) : valueOrDefault.Substring("IPM.InfoPathForm".Length + 1);
						text = "InfoPathForm." + str2 + "." + valueOrDefault2;
					}
				}
			}
			if (text == null)
			{
				text = this.item.GetValueOrDefault<string>(InternalSchema.ContentClass);
			}
			if (text != null)
			{
				this.mimeWriter.WriteHeader(HeaderId.ContentClass, text);
			}
		}

		private void WriteUMHeaders()
		{
			string text = this.GetDualPropertyValue(InternalSchema.SenderTelephoneNumber, InternalSchema.XSenderTelephoneNumber);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-CallingTelephoneNumber", text);
			}
			text = this.GetDualPropertyValue(InternalSchema.VoiceMessageDuration, InternalSchema.XVoiceMessageDuration);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-VoiceMessageDuration", text);
			}
			text = this.GetDualPropertyValue(InternalSchema.VoiceMessageSenderName, InternalSchema.XVoiceMessageSenderName);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-VoiceMessageSenderName", text);
			}
			text = this.GetDualPropertyValue(InternalSchema.FaxNumberOfPages, InternalSchema.XFaxNumberOfPages);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-FaxNumberOfPages", text);
			}
			text = this.GetDualPropertyValue(InternalSchema.VoiceMessageAttachmentOrder, InternalSchema.XVoiceMessageAttachmentOrder);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-AttachmentOrder", text);
			}
			text = this.GetDualPropertyValue(InternalSchema.CallId, InternalSchema.XCallId);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-CallID", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.XMsExchangeUMPartnerContent) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-UM-PartnerContent", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.XMsExchangeUMPartnerContext) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-UM-PartnerContext", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.XMsExchangeUMPartnerStatus) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-UM-PartnerStatus", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.XMsExchangeUMPartnerAssignedID) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-UM-PartnerAssignedID", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.XMsExchangeUMDialPlanLanguage) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-UM-DialPlanLanguage", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.XMsExchangeUMCallerInformedOfAnalysis) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-UM-CallerInformedOfAnalysis", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.XRequireProtectedPlayOnPhone) as string);
			if (!string.IsNullOrEmpty(text) && text.Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				this.mimeWriter.WriteHeader("X-RequireProtectedPlayOnPhone", "true");
			}
			text = (this.item.TryGetProperty(InternalSchema.XMSExchangeOutlookProtectionRuleVersion) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-Outlook-Protection-Rule-Addin-Version", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.XMSExchangeOutlookProtectionRuleConfigTimestamp) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-Outlook-Protection-Rule-Config-Timestamp", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.XMSExchangeOutlookProtectionRuleOverridden) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-Outlook-Protection-Rule-Overridden", text);
			}
		}

		private void WriteExtendedHeaders()
		{
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in this.item.AllNativeProperties)
			{
				if (nativeStorePropertyDefinition.SpecifiedWith == PropertyTypeSpecifier.GuidString)
				{
					GuidNamePropertyDefinition guidNamePropertyDefinition = (GuidNamePropertyDefinition)nativeStorePropertyDefinition;
					if (guidNamePropertyDefinition.Guid == WellKnownPropertySet.InternetHeaders && ItemToMimeConverter.IsValidHeader(guidNamePropertyDefinition.PropertyName) && !ItemToMimeConverter.IsReservedHeader(guidNamePropertyDefinition.PropertyName) && !this.IsExcludedHeaderPropertyName(guidNamePropertyDefinition.PropertyName))
					{
						string text = this.item.TryGetProperty(guidNamePropertyDefinition) as string;
						if (text != null)
						{
							this.mimeWriter.WriteHeader(guidNamePropertyDefinition.PropertyName, text);
						}
					}
				}
			}
		}

		private bool IsExcludedHeaderPropertyName(string propertyName)
		{
			return (this.flags & (ConverterFlags.IsEmbeddedMessage | ConverterFlags.IsStreamToStreamConversion)) == ConverterFlags.None && (!this.options.AllowDlpHeadersToPenetrateFirewall || !ItemToMimeConverter.IsDlpHeaderAllowedToPenetrateFirewall(propertyName)) && MimeConstants.IsInReservedHeaderNamespace(propertyName);
		}

		private void WriteCalendarHeaders()
		{
			if (!this.options.EnableCalendarHeaderGeneration)
			{
				return;
			}
			if (!this.item.GetValueOrDefault<bool>(InternalSchema.HasBeenSubmitted))
			{
				return;
			}
			MailboxSession mailboxSession = this.item.Session as MailboxSession;
			if (mailboxSession == null)
			{
				return;
			}
			string valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.ItemClass);
			if (string.IsNullOrEmpty(valueOrDefault) || (!ObjectClass.IsMeetingRequest(valueOrDefault) && !ObjectClass.IsMeetingCancellation(valueOrDefault)))
			{
				return;
			}
			this.WriteCalendarOriginatorIdHeader(mailboxSession);
			this.WriteCalendarItemSeriesMeetingMessageHeaders();
		}

		private void WriteCalendarOriginatorIdHeader(MailboxSession mailboxSession)
		{
			string valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.CalendarOriginatorId);
			int valueOrDefault2 = this.item.GetValueOrDefault<int>(MeetingMessageSchema.AppointmentAuxiliaryFlags, 0);
			if ((valueOrDefault2 & 32) != 0)
			{
				if (!string.IsNullOrEmpty(valueOrDefault))
				{
					StorageGlobals.ContextTraceDebug<string>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteCalendarOriginatorIdHeader: Writing calendar originator Id header = {0} to RUM.", valueOrDefault);
					this.mimeWriter.WriteHeader("X-MS-Exchange-Calendar-Originator-Id", valueOrDefault);
				}
				return;
			}
			string valueOrDefault3 = this.item.GetValueOrDefault<string>(InternalSchema.SenderEmailAddress);
			string valueOrDefault4 = this.item.GetValueOrDefault<string>(InternalSchema.SenderAddressType);
			string valueOrDefault5 = this.item.GetValueOrDefault<string>(InternalSchema.SentRepresentingEmailAddress);
			string valueOrDefault6 = this.item.GetValueOrDefault<string>(InternalSchema.SentRepresentingType);
			if (string.IsNullOrEmpty(valueOrDefault3) || string.IsNullOrEmpty(valueOrDefault4) || valueOrDefault4 != "EX")
			{
				StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteCalendarOriginatorIdHeader: senderAddressType is not EX, not writing the header.");
				return;
			}
			Guid mailboxGuid = mailboxSession.MailboxGuid;
			string mailboxOwnerLegacyDN = valueOrDefault3;
			string text;
			CalendarOriginatorIdProperty.TryCreate(mailboxGuid, mailboxOwnerLegacyDN, out text);
			Participant participant = this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.Sender];
			Participant participant2 = this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.From];
			if (participant != null && participant2 != null && participant.EmailAddress != participant2.EmailAddress)
			{
				text = null;
				ADRawEntry cachedRecipient = this.AddressCache.GetCachedRecipient(participant2);
				Guid guid = Guid.Empty;
				object obj;
				if (cachedRecipient != null && cachedRecipient.TryGetValueWithoutDefault(ADMailboxRecipientSchema.ExchangeGuid, out obj))
				{
					guid = (Guid)obj;
				}
				bool? flag = null;
				bool flag2 = (valueOrDefault2 & 4) != 0;
				if (!flag2 && cachedRecipient != null)
				{
					StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteCalendarOriginatorIdHeader: Meeting request looks like it came from a delegate. Checking for delegates.");
					flag = this.AddressCache.IsDelegateOfPrincipal(participant2, participant);
				}
				if ((flag != null && flag.Value) || flag2)
				{
					if (guid != Guid.Empty && !string.IsNullOrEmpty(valueOrDefault5) && !string.IsNullOrEmpty(valueOrDefault6) && valueOrDefault6 == "EX")
					{
						CalendarOriginatorIdProperty.TryCreate(guid, valueOrDefault5, out text);
					}
					else
					{
						text = null;
					}
				}
				else if (!flag2 && flag == null)
				{
					text = null;
				}
			}
			StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteCalendarOriginatorIdHeader: Calculated originator Id = {0}, Meeting request originator Id = {1}.", text, valueOrDefault);
			string preferredCalendarOriginatorId = CalendarOriginatorIdProperty.GetPreferredCalendarOriginatorId(text, valueOrDefault);
			StorageGlobals.ContextTraceDebug<string>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteCalendarOriginatorIdHeader: Preferred calendar originator Id header = {0}.", preferredCalendarOriginatorId);
			if (!string.IsNullOrEmpty(preferredCalendarOriginatorId))
			{
				StorageGlobals.ContextTraceDebug<string>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteCalendarOriginatorIdHeader: Writing calendar originator Id header = {0}.", preferredCalendarOriginatorId);
				this.mimeWriter.WriteHeader("X-MS-Exchange-Calendar-Originator-Id", preferredCalendarOriginatorId);
			}
		}

		private void WriteCalendarItemSeriesMeetingMessageHeaders()
		{
			int valueOrDefault = this.item.GetValueOrDefault<int>(MeetingMessageSchema.SeriesSequenceNumber, -1);
			string valueOrDefault2 = this.item.GetValueOrDefault<string>(MeetingMessageSchema.SeriesId, null);
			byte[] valueOrDefault3 = this.item.GetValueOrDefault<byte[]>(MeetingMessageInstanceSchema.GlobalObjectId, null);
			byte[] valueOrDefault4 = this.item.GetValueOrDefault<byte[]>(MeetingMessageInstanceSchema.MasterGlobalObjectId, null);
			if (valueOrDefault > 0)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Calendar-Series-Sequence-Number", valueOrDefault.ToString());
				if (!string.IsNullOrEmpty(valueOrDefault2))
				{
					this.mimeWriter.WriteHeader("X-MS-Exchange-Calendar-Series-Id", valueOrDefault2);
				}
				if (valueOrDefault3 != null)
				{
					this.mimeWriter.WriteHeader("X-MS-Exchange-Calendar-Series-Instance-Id", Convert.ToBase64String(valueOrDefault3));
				}
				if (valueOrDefault4 != null)
				{
					this.mimeWriter.WriteHeader("X-MS-Exchange-Calendar-Series-Master-Id", Convert.ToBase64String(valueOrDefault4));
				}
				return;
			}
		}

		private void WriteMimeRecipients(OutboundAddressCache addressCache, HeaderId header, RecipientItemType recipientsToWrite)
		{
			bool flag = false;
			foreach (ConversionRecipientEntry conversionRecipientEntry in addressCache.Recipients)
			{
				if (conversionRecipientEntry.RecipientItemType == recipientsToWrite)
				{
					if (!flag)
					{
						this.mimeWriter.WriteHeader(header, null);
						flag = true;
					}
					this.WriteParticipant(conversionRecipientEntry.Participant);
				}
			}
		}

		private void WriteSharingHeaders()
		{
			for (int i = 0; i < ItemToMimeConverter.XSharingProperties.Length; i++)
			{
				string valueOrDefault = this.item.GetValueOrDefault<string>(ItemToMimeConverter.XSharingProperties[i], null);
				if (valueOrDefault != null)
				{
					this.mimeWriter.WriteHeader(ItemToMimeConverter.XSharingMimeNames[i], valueOrDefault);
				}
			}
		}

		private void WriteModernGroupHeaders()
		{
			for (int i = 0; i < ItemToMimeConverter.XModernGroupProperties.Length; i++)
			{
				string valueOrDefault = this.item.GetValueOrDefault<string>(ItemToMimeConverter.XModernGroupProperties[i], null);
				if (valueOrDefault != null)
				{
					this.mimeWriter.WriteHeader(ItemToMimeConverter.XModernGroupMimeNames[i], valueOrDefault);
				}
			}
		}

		private void WriteMimeHeaders(ItemToMimeConverter.MimeFlags flags)
		{
			MessageItem messageItem = this.item as MessageItem;
			PostItem postItem = this.item as PostItem;
			if ((flags & ItemToMimeConverter.MimeFlags.SkipFromRecipientsAndSubject) != ItemToMimeConverter.MimeFlags.SkipFromRecipientsAndSubject)
			{
				this.WriteFromHeader();
				if (this.itemType != MimeItemType.MimeMessageDsn)
				{
					this.WriteMimeRecipients(this.AddressCache, HeaderId.To, RecipientItemType.To);
					this.WriteMimeRecipients(this.AddressCache, HeaderId.Cc, RecipientItemType.Cc);
					if (this.options.DemoteBcc)
					{
						this.WriteMimeRecipients(this.AddressCache, HeaderId.Bcc, RecipientItemType.Bcc);
					}
				}
				else
				{
					Participant participant = this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.ReceivedRepresenting] ?? this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.ReceivedBy];
					if (participant != null)
					{
						this.mimeWriter.WriteHeader(HeaderId.To, null);
						this.WriteParticipant(participant);
					}
				}
				string valueOrDefault = this.item.GetValueOrDefault<string>(ItemSchema.Subject, string.Empty);
				if (valueOrDefault.Length != 0)
				{
					SubjectProperty.TruncateSubject(ref valueOrDefault, this.options.Limits.MaxMimeSubjectLength);
					this.mimeWriter.WriteHeader(HeaderId.Subject, valueOrDefault);
				}
				else
				{
					this.mimeWriter.WriteHeader(HeaderId.Subject, string.Empty);
				}
			}
			if (messageItem != null && !string.IsNullOrEmpty(messageItem.ConversationTopic))
			{
				this.mimeWriter.WriteHeader("Thread-Topic", messageItem.ConversationTopic);
			}
			else if (postItem != null && !string.IsNullOrEmpty(postItem.ConversationTopic))
			{
				this.mimeWriter.WriteHeader("Thread-Topic", postItem.ConversationTopic);
			}
			this.WriteThreadIndexHeader();
			if (this.IsJournalingMessage)
			{
				this.mimeWriter.WriteHeader("X-MS-Journal-Report", string.Empty);
			}
			this.WriteImportanceHeaders();
			this.WriteReceiptHeaders();
			this.WriteSenderHeader();
			this.WriteSensitivityHeader();
			this.WriteContentClass();
			this.WriteUMHeaders();
			this.WriteSharingHeaders();
			this.WriteModernGroupHeaders();
			this.WriteCalendarHeaders();
			ExDateTime valueOrDefault2 = this.item.GetValueOrDefault<ExDateTime>(InternalSchema.OriginalSentTimeForEscalation, ExDateTime.MinValue);
			ExDateTime exDateTime;
			if (valueOrDefault2 != ExDateTime.MinValue)
			{
				StorageGlobals.ContextTraceDebug<ExDateTime>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteMimeHeaders: Sent time for escalation found, using it as date header: {0}.", valueOrDefault2);
				exDateTime = valueOrDefault2;
			}
			else
			{
				exDateTime = this.item.GetValueOrDefault<ExDateTime>(InternalSchema.SentTime, ExDateTime.MinValue);
			}
			if (exDateTime != ExDateTime.MinValue)
			{
				this.mimeWriter.WriteHeader(HeaderId.Date, exDateTime);
			}
			ExDateTime valueOrDefault3 = this.item.GetValueOrDefault<ExDateTime>(InternalSchema.DeferredDeliveryTime, ExDateTime.MinValue);
			if (valueOrDefault3 != ExDateTime.MinValue)
			{
				this.mimeWriter.WriteHeader(HeaderId.DeferredDelivery, valueOrDefault3);
			}
			string text = this.item.TryGetProperty(InternalSchema.InternetMessageId) as string;
			if (text != null)
			{
				this.mimeWriter.WriteHeader(HeaderId.MessageId, text);
			}
			if (messageItem != null && !string.IsNullOrEmpty(messageItem.References))
			{
				this.mimeWriter.WriteHeader(HeaderId.References, messageItem.References);
			}
			else if (postItem != null && !string.IsNullOrEmpty(postItem.References))
			{
				this.mimeWriter.WriteHeader(HeaderId.References, postItem.References);
			}
			this.WriteKeywords();
			this.WriteListHeaders();
			if (messageItem != null)
			{
				if (!string.IsNullOrEmpty(messageItem.InReplyTo))
				{
					this.mimeWriter.WriteHeader(HeaderId.InReplyTo, messageItem.InReplyTo);
				}
				this.WriteReplyToHeader();
			}
			this.WriteAcceptAndContentLanguage();
			this.WriteClassificationMimeHeaders();
			this.WritePayloadMimeHeaders();
			this.WriteAuthHeaders();
			this.WriteModeratedTransportHeaders();
			this.WriteProtectedMessageHeaders();
			bool produceTnefCorrelationKey = (flags & ItemToMimeConverter.MimeFlags.ProduceTnefCorrelationKey) != ItemToMimeConverter.MimeFlags.SkipMessageHeaders;
			bool flag = this.CheckHasAttachments(produceTnefCorrelationKey);
			this.mimeWriter.WriteHeader("X-MS-Has-Attach", flag ? "yes" : string.Empty);
			if (messageItem != null && messageItem.AutoResponseSuppress != AutoResponseSuppress.None)
			{
				this.mimeWriter.WriteHeader("X-Auto-Response-Suppress", messageItem.AutoResponseSuppress.ToString());
			}
			bool? valueAsNullable = this.item.GetValueAsNullable<bool>(InternalSchema.IsAutoForwarded);
			if (valueAsNullable != null && valueAsNullable.Value)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-AutoForwarded", "true");
			}
			int? valueAsNullable2 = this.item.GetValueAsNullable<int>(InternalSchema.SenderIdStatus);
			if (valueAsNullable2 != null)
			{
				SenderIdStatus value = (SenderIdStatus)valueAsNullable2.Value;
				if (EnumValidator.IsValidValue<SenderIdStatus>(value))
				{
					this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-SenderIdResult", value.ToString());
				}
			}
			int? valueAsNullable3 = this.item.GetValueAsNullable<int>(InternalSchema.SpamConfidenceLevel);
			if (valueAsNullable3 != null && valueAsNullable3 <= 10 && valueAsNullable3 >= -1)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-SCL", valueAsNullable3.ToString());
			}
			int? valueAsNullable4 = this.item.GetValueAsNullable<int>(InternalSchema.OriginalScl);
			if (valueAsNullable4 != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-Original-SCL", valueAsNullable4.ToString());
			}
			int valueOrDefault4 = this.item.GetValueOrDefault<int>(InternalSchema.ContentFilterPcl, -1);
			if (ConvertUtils.IsValidPCL(valueOrDefault4))
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-PCL", valueOrDefault4.ToString());
			}
			string valueOrDefault5 = this.item.GetValueOrDefault<string>(InternalSchema.PurportedSenderDomain);
			if (valueOrDefault5 != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-PRD", valueOrDefault5);
			}
			this.WriteXMessageFlagAndReplyBy();
			if ((this.flags & ConverterFlags.IsStreamToStreamConversion) == ConverterFlags.IsStreamToStreamConversion)
			{
				string[] valueOrDefault6 = this.item.GetValueOrDefault<string[]>(InternalSchema.XMsExchOrganizationAVStampMailbox, Array<string>.Empty);
				if (valueOrDefault6 != null)
				{
					foreach (string text2 in valueOrDefault6)
					{
						if (text2 != null)
						{
							this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-AVStamp-Mailbox", text2);
						}
					}
				}
			}
			string[] valueOrDefault7 = this.item.GetValueOrDefault<string[]>(InternalSchema.XLoop, null);
			if (valueOrDefault7 != null)
			{
				foreach (string text3 in valueOrDefault7)
				{
					if (!string.IsNullOrEmpty(text3))
					{
						this.mimeWriter.WriteHeader("X-MS-Exchange-Inbox-Rules-Loop", text3);
					}
				}
			}
			if ((flags & ItemToMimeConverter.MimeFlags.ProduceTnefCorrelationKey) != ItemToMimeConverter.MimeFlags.SkipMessageHeaders)
			{
				this.tnefCorrelationKey = this.item.GetValueOrDefault<string>(InternalSchema.InternetMessageId, Guid.NewGuid().ToString());
			}
			this.mimeWriter.WriteHeader("X-MS-TNEF-Correlator", this.tnefCorrelationKey);
		}

		private bool CheckHasAttachments(bool produceTnefCorrelationKey)
		{
			if (produceTnefCorrelationKey)
			{
				return this.item.AttachmentCollection.Count != 0;
			}
			int num = 0;
			foreach (AttachmentHandle handle in this.item.AttachmentCollection)
			{
				using (Attachment attachment = this.item.AttachmentCollection.Open(handle))
				{
					ReferenceAttachment referenceAttachment = attachment as ReferenceAttachment;
					if (referenceAttachment != null)
					{
						num++;
					}
				}
			}
			return this.item.AttachmentCollection.Count - num != 0;
		}

		private void WriteXMessageFlagAndReplyBy()
		{
			string valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.FlagRequest);
			if (!string.IsNullOrEmpty(valueOrDefault))
			{
				this.mimeWriter.WriteHeader("X-Message-Flag", valueOrDefault);
				ExDateTime valueOrDefault2 = this.item.GetValueOrDefault<ExDateTime>(InternalSchema.ReplyTime, ExDateTime.MinValue);
				if (valueOrDefault2 != ExDateTime.MinValue)
				{
					this.mimeWriter.WriteHeader(HeaderId.ReplyBy, valueOrDefault2);
				}
			}
		}

		private void WriteAuthHeaders()
		{
			string valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.XMsExchOrganizationAuthAs);
			if (valueOrDefault != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-AuthAs", valueOrDefault);
			}
			valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.XMsExchOrganizationAuthDomain);
			if (valueOrDefault != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-AuthDomain", valueOrDefault);
			}
			valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.XMsExchOrganizationAuthMechanism);
			if (valueOrDefault != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-AuthMechanism", valueOrDefault);
			}
			valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.XMsExchOrganizationAuthSource);
			if (valueOrDefault != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-AuthSource", valueOrDefault);
			}
		}

		private void WriteProtectedMessageHeaders()
		{
			string valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.XMsExchangeOrganizationRightsProtectMessage);
			if (valueOrDefault != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-RightsProtectMessage", valueOrDefault);
			}
		}

		private void WriteModeratedTransportHeaders()
		{
			string valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.ApprovalAllowedDecisionMakers);
			if (valueOrDefault != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers", valueOrDefault);
			}
			valueOrDefault = this.item.GetValueOrDefault<string>(InternalSchema.ApprovalRequestor);
			if (valueOrDefault != null)
			{
				this.mimeWriter.WriteHeader("X-MS-Exchange-Organization-Approval-Requestor", valueOrDefault);
			}
		}

		private void WritePayloadMimeHeaders()
		{
			string text = this.item.TryGetProperty(InternalSchema.AttachPayloadProviderGuidString) as string;
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-Payload-Provider-Guid", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.AttachPayloadClass) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("X-Payload-Class", text);
			}
		}

		private void WriteClassificationMimeHeaders()
		{
			object obj = this.item.TryGetProperty(InternalSchema.IsClassified);
			if (!(obj is bool) || !(bool)obj)
			{
				return;
			}
			this.mimeWriter.WriteHeader("x-microsoft-classified", "true");
			string text = this.item.TryGetProperty(InternalSchema.Classification) as string;
			if (text != null)
			{
				this.mimeWriter.WriteHeader("x-microsoft-classification", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.ClassificationDescription) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("x-microsoft-classDesc", text);
			}
			text = (this.item.TryGetProperty(InternalSchema.ClassificationGuid) as string);
			if (text != null)
			{
				this.mimeWriter.WriteHeader("x-microsoft-classID", text);
			}
			obj = this.item.TryGetProperty(InternalSchema.ClassificationKeep);
			if (obj is bool && (bool)obj)
			{
				this.mimeWriter.WriteHeader("X-microsoft-classKeep", "true");
			}
		}

		private void WriteKeywords()
		{
			if (this.IsJournalingMessage || this.itemType == MimeItemType.MimeMessageReplication || (!this.options.ClearCategories && this.item.Categories.Count > 0))
			{
				this.options.ClearCategories = false;
				StringBuilder stringBuilder = new StringBuilder(128);
				foreach (string value in this.item.Categories)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(value);
				}
				this.mimeWriter.WriteHeader(HeaderId.Keywords, stringBuilder.ToString());
			}
		}

		private void WriteThreadIndexHeader()
		{
			byte[] valueOrDefault = this.item.GetValueOrDefault<byte[]>(InternalSchema.ConversationIndex);
			if (valueOrDefault != null)
			{
				string data = Convert.ToBase64String(valueOrDefault);
				this.mimeWriter.WriteHeader("Thread-Index", data);
			}
		}

		private void WriteListHeaders()
		{
			string text = this.item.TryGetProperty(InternalSchema.ListHelp) as string;
			if (text != null)
			{
				this.mimeWriter.WriteHeader(HeaderId.ListHelp, text);
			}
			string text2 = this.item.TryGetProperty(InternalSchema.ListSubscribe) as string;
			if (text2 != null)
			{
				this.mimeWriter.WriteHeader(HeaderId.ListSubscribe, text2);
			}
			string text3 = this.item.TryGetProperty(InternalSchema.ListUnsubscribe) as string;
			if (text3 != null)
			{
				this.mimeWriter.WriteHeader(HeaderId.ListUnsubscribe, text3);
			}
		}

		private void WriteImportanceHeaders()
		{
			if (this.item.Importance != Importance.Normal)
			{
				switch (this.item.Importance)
				{
				case Importance.Low:
					this.mimeWriter.WriteHeader(HeaderId.Importance, "low");
					this.mimeWriter.WriteHeader(HeaderId.XPriority, "5");
					return;
				case Importance.High:
					this.mimeWriter.WriteHeader(HeaderId.Importance, "high");
					this.mimeWriter.WriteHeader(HeaderId.XPriority, "1");
					return;
				}
				StorageGlobals.ContextTraceDebug<Importance>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteImportanceHeader: invalid importance value found, {0}.", this.item.Importance);
			}
		}

		private void WriteFromHeader()
		{
			Participant participant = this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.From];
			if (participant != null)
			{
				this.mimeWriter.WriteHeader(HeaderId.From, null);
				this.WriteParticipant(participant);
			}
		}

		private void WriteSensitivityHeader()
		{
			switch (this.item.Sensitivity)
			{
			case Sensitivity.Personal:
				this.mimeWriter.WriteHeader(HeaderId.Sensitivity, "personal");
				return;
			case Sensitivity.Private:
				this.mimeWriter.WriteHeader(HeaderId.Sensitivity, "private");
				return;
			case Sensitivity.CompanyConfidential:
				this.mimeWriter.WriteHeader(HeaderId.Sensitivity, "company-confidential");
				return;
			default:
				return;
			}
		}

		private void WriteContentDisposition(string value, Attachment attach, long size, string filename)
		{
			this.mimeWriter.WriteHeader(HeaderId.ContentDisposition, value);
			if (filename != null)
			{
				this.mimeWriter.WriteHeaderParameter("filename", filename);
			}
			if (size > 0L)
			{
				this.mimeWriter.WriteHeaderParameter("size", size.ToString());
			}
			this.mimeWriter.WriteHeaderParameter("creation-date", attach.CreationTime.UniversalTime.ToString("r"));
			this.mimeWriter.WriteHeaderParameter("modification-date", attach.LastModifiedTime.UniversalTime.ToString("r"));
			object obj = attach.TryGetProperty(InternalSchema.OriginalMimeReadTime);
			if (obj is ExDateTime)
			{
				this.mimeWriter.WriteHeaderParameter("read-date", ((ExDateTime)obj).UniversalTime.ToString("r"));
			}
		}

		private void WriteSenderHeader()
		{
			MessageItem messageItem = this.item as MessageItem;
			Participant participant = this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.From];
			Participant participant2 = this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.Sender];
			if (messageItem != null)
			{
				if (participant2 != null && (participant == null || participant2.EmailAddress != participant.EmailAddress))
				{
					this.mimeWriter.WriteHeader(HeaderId.Sender, null);
					this.WriteParticipant(participant2);
				}
				IMailboxSession mailboxSession = messageItem.Session as MailboxSession;
				if (mailboxSession != null && mailboxSession.MailboxOwner != null && mailboxSession.MailboxOwner.GetConfiguration().SharedMailbox.SharedMailboxSentItemCopy.Enabled)
				{
					MessageSentRepresentingFlags valueOrDefault = this.item.GetValueOrDefault<MessageSentRepresentingFlags>(InternalSchema.MessageSentRepresentingType, MessageSentRepresentingFlags.None);
					if (valueOrDefault != MessageSentRepresentingFlags.None)
					{
						MimeStreamWriter mimeStreamWriter = this.mimeWriter;
						string name = "X-MS-Exchange-MessageSentRepresentingType";
						int num = (int)valueOrDefault;
						mimeStreamWriter.WriteHeader(name, num.ToString());
					}
				}
			}
		}

		private void WriteReplyToHeader()
		{
			if (this.AddressCache.ReplyTo == null || this.AddressCache.ReplyTo.Count == 0)
			{
				return;
			}
			this.mimeWriter.WriteHeader(HeaderId.ReplyTo, null);
			foreach (Participant participant in this.AddressCache.ReplyTo)
			{
				this.WriteParticipant(participant);
			}
		}

		private void WriteReceiptHeaders()
		{
			MessageItem messageItem = this.item as MessageItem;
			if (messageItem != null)
			{
				bool isReadReceiptRequested = messageItem.IsReadReceiptRequested;
				bool isDeliveryReceiptRequested = messageItem.IsDeliveryReceiptRequested;
				if (isReadReceiptRequested || isDeliveryReceiptRequested)
				{
					Participant participant;
					if ((participant = this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.ReadReceipt]) == null)
					{
						participant = (this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.Sender] ?? this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.From]);
					}
					Participant participant2 = participant;
					if (participant2 != null)
					{
						if (isReadReceiptRequested)
						{
							this.mimeWriter.WriteHeader(HeaderId.DispositionNotificationTo, null);
							this.WriteParticipant(participant2);
						}
						if (isDeliveryReceiptRequested)
						{
							this.mimeWriter.WriteHeader(HeaderId.ReturnReceiptTo, null);
							this.mimeWriter.WriteMailBox(null, ItemToMimeConverter.GetParticipantSmtpAddress(participant2, this.options));
						}
					}
				}
			}
		}

		private void WriteReceivedAndXHeaders()
		{
			Stream headersStream = null;
			Stream storeStream = null;
			try
			{
				ConvertUtils.CallCts(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteXAndReceivedHeaders", ServerStrings.ConversionCorruptContent, delegate
				{
					object obj = this.item.TryGetProperty(InternalSchema.TransportMessageHeaders);
					string text = obj as string;
					if (text != null)
					{
						byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(text);
						headersStream = new MemoryStream(bytes);
					}
					else if (PropertyError.IsPropertyValueTooBig(obj))
					{
						storeStream = this.item.OpenPropertyStream(InternalSchema.TransportMessageHeaders, PropertyOpenMode.ReadOnly);
						TextToText textToText = new TextToText(TextToTextConversionMode.ConvertCodePageOnly);
						textToText.InputEncoding = Encoding.Unicode;
						textToText.OutputEncoding = CTSGlobals.AsciiEncoding;
						headersStream = new ConverterStream(storeStream, textToText, ConverterStreamAccess.Read);
					}
					if (headersStream == null)
					{
						return;
					}
					using (MimeReader mimeReader = new MimeReader(headersStream))
					{
						mimeReader.ReadNextPart();
						MimeHeaderReader headerReader = mimeReader.HeaderReader;
						while (headerReader.ReadNextHeader())
						{
							if (this.CanWriteXHeader(headerReader))
							{
								this.mimeWriter.WriteHeader(Header.ReadFrom(headerReader));
							}
						}
					}
				});
			}
			catch (ConversionFailedException arg)
			{
				StorageGlobals.ContextTraceError<ConversionFailedException>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::WriteXAndReceivedHeaders - ConversionFailedException: {0}", arg);
			}
			finally
			{
				if (headersStream != null)
				{
					headersStream.Dispose();
				}
				if (storeStream != null)
				{
					storeStream.Dispose();
				}
			}
		}

		private void WriteAcceptAndContentLanguage()
		{
			string text = this.item.GetValueOrDefault<string>(InternalSchema.AcceptLanguage);
			if (text == null)
			{
				bool valueOrDefault = this.item.GetValueOrDefault<bool>(InternalSchema.HasBeenSubmitted);
				if (valueOrDefault)
				{
					MailboxSession mailboxSession = this.item.Session as MailboxSession;
					if (mailboxSession == null)
					{
						return;
					}
					StringBuilder stringBuilder = new StringBuilder();
					foreach (CultureInfo cultureInfo in mailboxSession.InternalGetMailboxCultures())
					{
						if (!string.IsNullOrEmpty(cultureInfo.Name))
						{
							if (stringBuilder.Length != 0)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.Append(cultureInfo.Name);
						}
					}
					text = stringBuilder.ToString();
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.mimeWriter.WriteHeader("Accept-Language", text);
			}
			object obj = this.item.TryGetProperty(InternalSchema.MessageLocaleId);
			int num = 0;
			if (!PropertyError.IsPropertyError(obj))
			{
				num = (int)obj;
			}
			if (num != 0)
			{
				Culture culture = null;
				if (Culture.TryGetCulture(num, out culture))
				{
					this.mimeWriter.WriteHeader(HeaderId.ContentLanguage, culture.Name);
				}
			}
		}

		private bool CanWriteXHeader(MimeHeaderReader reader)
		{
			return reader.HeaderId == HeaderId.Received;
		}

		private static string FormatMimeTypeValueHeader(string type, string value)
		{
			return string.Format("{0}; {1}", type, value);
		}

		private static string GetDsnMdnActionFromMessageClass(KeyValuePair<string, string>[] actionToClassMapping, string itemClass)
		{
			int num = 0;
			string text = null;
			foreach (KeyValuePair<string, string> keyValuePair in actionToClassMapping)
			{
				if (ObjectClass.IsReport(itemClass, keyValuePair.Value))
				{
					int num2 = keyValuePair.Value.Split(new char[]
					{
						'.'
					}).Length;
					if (num2 > num)
					{
						text = keyValuePair.Key;
						num = num2;
					}
				}
			}
			if (text == null)
			{
				text = actionToClassMapping[actionToClassMapping.Length - 1].Key;
				StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcOutboundMimeTracer, "ItemToMimeConverter::GetDsnMdnActionFromMessageClass: none of the action-to-suffix mappings match the item class \"{0}\". Going with a default choice: \"{1}\"", itemClass, text);
			}
			return text;
		}

		private static void WriteHeaderSectionBreak(Stream stm)
		{
			stm.Write(ItemToMimeConverter.headerSectionBreak, 0, ItemToMimeConverter.headerSectionBreak.Length);
		}

		private string EncodeParticipant(Participant participant, out string displayName)
		{
			string value = participant.EmailAddress ?? participant.DisplayName;
			displayName = participant.DisplayName;
			string routingType;
			string type;
			if ((routingType = participant.RoutingType) != null)
			{
				if (!(routingType == "SMTP"))
				{
					if (!(routingType == "EX"))
					{
						type = "X-" + participant.RoutingType;
					}
					else
					{
						value = ItemToMimeConverter.GetParticipantSmtpAddress(participant, this.options);
						type = "RFC822";
					}
				}
				else
				{
					type = "RFC822";
				}
			}
			else
			{
				type = "unknown";
			}
			return ItemToMimeConverter.FormatMimeTypeValueHeader(type, value);
		}

		private ConversionResult WriteHumanReadableReportBody(MimePartInfo partInfo, ItemToMimeConverter.MimeFlags flags)
		{
			ConversionResult result = new ConversionResult();
			bool flag;
			switch (partInfo.ContentType)
			{
			case MimePartContentType.TextPlain:
				flag = false;
				break;
			case MimePartContentType.TextHtml:
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			CultureInfo cultureInfo;
			Charset charset;
			using (Stream stream = ReportMessage.GenerateReportBody((MessageItem)this.item, this.options.DsnHumanReadableWriter ?? DsnHumanReadableWriter.DefaultDsnHumanReadableWriter, null, out cultureInfo, out charset))
			{
				Charset charset2 = charset;
				if (!ConvertUtils.TryTransformCharset(ref charset2) && this.options.DetectionOptions.PreferredCharset != null)
				{
					charset2 = this.options.DetectionOptions.PreferredCharset;
					if (!ConvertUtils.TryTransformCharset(ref charset2))
					{
						charset2 = Culture.Default.MimeCharset;
					}
				}
				this.mimeWriter.WriteHeader(HeaderId.ContentType, flag ? "text/html" : "text/plain");
				this.mimeWriter.WriteHeaderParameter("charset", charset2.Name);
				using (ItemToMimeConverter.MimeBodyOutputStream mimeBodyOutputStream = new ItemToMimeConverter.MimeBodyOutputStream(this.options, this.mimeWriter, partInfo.ContentType, charset2.Name, true))
				{
					if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) == ItemToMimeConverter.MimeFlags.SkipContent)
					{
						return result;
					}
					stream.Position = 0L;
					if (charset.Name != charset2.Name || !flag)
					{
						Encoding encoding = charset.GetEncoding();
						if (flag)
						{
							new HtmlToHtml
							{
								InputEncoding = encoding,
								OutputEncoding = charset2.GetEncoding()
							}.Convert(stream, mimeBodyOutputStream);
						}
						else
						{
							new HtmlToText
							{
								InputEncoding = encoding,
								OutputEncoding = charset2.GetEncoding()
							}.Convert(stream, mimeBodyOutputStream);
						}
					}
					else
					{
						Util.StreamHandler.CopyStreamData(stream, mimeBodyOutputStream);
					}
				}
			}
			return result;
		}

		private ConversionResult WriteDsnReportBody(MimePartInfo partInfo, ItemToMimeConverter.MimeFlags flags)
		{
			ConversionResult result = new ConversionResult();
			this.mimeWriter.WriteHeader(HeaderId.ContentType, "message/delivery-status");
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) == ItemToMimeConverter.MimeFlags.SkipContent)
			{
				return result;
			}
			MessageItem messageItem = (MessageItem)this.item;
			using (Stream contentStream = this.mimeWriter.GetContentStream(true))
			{
				using (MimeWriter mimeWriter = new MimeWriter(contentStream, false, this.GetItemMimeEncodingOptions(this.options)))
				{
					mimeWriter.StartPart();
					mimeWriter.WriteHeader("Reporting-MTA", ItemToMimeConverter.FormatMimeTypeValueHeader("dns", messageItem.GetValueOrDefault<string>(InternalSchema.ReportingMta, this.InternalGetDefaultMta())));
					mimeWriter.Flush();
					foreach (Recipient recipient in messageItem.Recipients)
					{
						if (!(recipient.Participant == null))
						{
							ItemToMimeConverter.WriteHeaderSectionBreak(contentStream);
							string text;
							mimeWriter.WriteHeader("Final-recipient", this.EncodeParticipant(recipient.Participant, out text));
							mimeWriter.WriteHeader("Action", ItemToMimeConverter.GetDsnMdnActionFromMessageClass(InboundMimeConverter.DsnActionToClass, this.item.ClassName));
							mimeWriter.WriteHeader("Status", DsnMdnUtil.GetMimeDsnRecipientStatusCode(recipient).Value);
							string valueOrDefault = recipient.GetValueOrDefault<string>(InternalSchema.RemoteMta);
							if (valueOrDefault != null)
							{
								mimeWriter.WriteHeader("Remote-MTA", ItemToMimeConverter.FormatMimeTypeValueHeader("dns", valueOrDefault));
							}
							string valueOrDefault2 = recipient.GetValueOrDefault<string>(InternalSchema.SupplementaryInfo);
							if (valueOrDefault2 != null)
							{
								mimeWriter.WriteHeader("X-Supplementary-Info", valueOrDefault2);
							}
							if (text != recipient.Participant.EmailAddress)
							{
								mimeWriter.WriteHeader("X-Display-Name", text);
							}
							mimeWriter.Flush();
						}
					}
				}
			}
			return result;
		}

		private ConversionResult WriteMdnReportBody(MimePartInfo partInfo, ItemToMimeConverter.MimeFlags flags)
		{
			ConversionResult result = new ConversionResult();
			this.mimeWriter.WriteHeader(HeaderId.ContentType, "message/disposition-notification");
			if ((flags & ItemToMimeConverter.MimeFlags.SkipContent) == ItemToMimeConverter.MimeFlags.SkipContent)
			{
				return result;
			}
			MessageItem messageItem = (MessageItem)this.item;
			using (Stream contentStream = this.mimeWriter.GetContentStream(true))
			{
				using (MimeWriter mimeWriter = new MimeWriter(contentStream, false, this.GetItemMimeEncodingOptions(this.options)))
				{
					mimeWriter.StartPart();
					Participant participant = this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.Sender];
					Participant participant2 = this.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.From];
					Participant participant3 = participant ?? participant2;
					string text = null;
					if (participant3 != null)
					{
						mimeWriter.WriteHeader("Final-recipient", this.EncodeParticipant(participant3, out text));
						if (text == participant3.EmailAddress)
						{
							text = null;
						}
					}
					else
					{
						mimeWriter.WriteHeader("Final-recipient", string.Empty);
					}
					mimeWriter.WriteHeader("Disposition", string.Format("{0}; {1}", "automatic-action/MDN-sent-automatically", ItemToMimeConverter.GetDsnMdnActionFromMessageClass(InboundMimeConverter.MdnActionToClass, this.item.ClassName)));
					byte[] valueOrDefault = messageItem.GetValueOrDefault<byte[]>(InternalSchema.ParentKey);
					if (valueOrDefault != null)
					{
						mimeWriter.WriteHeader("X-MSExch-Correlation-Key", Convert.ToBase64String(valueOrDefault));
					}
					string valueOrDefault2 = messageItem.GetValueOrDefault<string>(InternalSchema.OriginalMessageId);
					if (valueOrDefault2 != null)
					{
						mimeWriter.WriteHeader("Original-Message-ID", valueOrDefault2);
					}
					if (text != null)
					{
						mimeWriter.WriteHeader("X-Display-Name", text);
					}
				}
			}
			return result;
		}

		private string InternalGetDefaultMta()
		{
			return Environment.MachineName;
		}

		private const int AttachMethodOLE = 6;

		private const string DnsAddressType = "dns";

		private static Dictionary<string, string> reservedMimeHeadersList;

		private static string[] forbiddenNames = new string[]
		{
			"acceptlanguage",
			"bcc",
			"cc",
			"content-base",
			"content-class",
			"content-description",
			"content-disposition",
			"content-id",
			"content-location",
			"content-md5",
			"content-transfer-encoding",
			"content-type",
			"date",
			"expires",
			"expiry-date",
			"from",
			"importance",
			"in-reply-to",
			"keywords",
			"list-help",
			"list-subscribe",
			"list-unsubscribe",
			"message-id",
			"mime-version",
			"priority",
			"received",
			"references",
			"reply-to",
			"resent-bcc",
			"resent-cc",
			"resent-date",
			"resent-message-id",
			"resent-reply-to",
			"resent-sender",
			"resent-to",
			"sender",
			"sensitivity",
			"subject",
			"thread-index",
			"thread-topic",
			"to",
			"X-Auto-Response-Suppress",
			"x-list-help",
			"x-list-subscribe",
			"x-list-unsubscribe",
			"X-MS-Has-Attach",
			"x-msmail-priority",
			"X-MS-TNEF-Correlator",
			"x-priority",
			"X-MimeOle",
			"x-microsoft-classified",
			"x-microsoft-classification",
			"x-microsoft-classDesc",
			"x-microsoft-classID",
			"X-Payload-Provider-Guid",
			"X-Payload-Class",
			"X-CallingTelephoneNumber",
			"X-VoiceMessageDuration",
			"X-VoiceMessageSenderName",
			"X-FaxNumberOfPages",
			"X-MS-Exchange-UM-PartnerContent",
			"X-MS-Exchange-UM-PartnerContext",
			"X-MS-Exchange-UM-PartnerStatus",
			"X-MS-Exchange-UM-PartnerAssignedID",
			"X-MS-Exchange-UM-DialPlanLanguage",
			"X-MS-Exchange-UM-CallerInformedOfAnalysis",
			"X-AttachmentOrder",
			"X-CallID",
			"X-MS-Journal-Report",
			"X-Message-Flag",
			"Accept-Language",
			"X-Accept-Language",
			"X-MS-Exchange-Organization-PCL",
			"X-MS-Exchange-Organization-SCL",
			"X-MS-Exchange-Organization-Original-SCL",
			"X-MS-Exchange-Organization-SenderIdResult",
			"X-MS-Exchange-Organization-AuthAs",
			"X-MS-Exchange-Organization-AuthDomain",
			"X-MS-Exchange-Organization-AuthMechanism",
			"X-MS-Exchange-Organization-AuthSource",
			"X-MS-Exchange-Organization-AVStamp-Mailbox",
			"X-MS-Exchange-ApplicationFlags"
		};

		private Item item;

		private ReadOnlyCollection<AttachmentLink> attachmentLinks;

		private OutboundAddressCache addressCache;

		private MimeStreamWriter mimeWriter;

		private OutboundConversionOptions options;

		private ConversionLimitsTracker limitsTracker;

		private int untitledOleAttachCounter;

		private MimeItemType itemType;

		private string tnefCorrelationKey;

		private string boundarySuffix;

		private ItemToMimeConverter.MessageFormatType messageFormat;

		private Attachment[] journalAttachments;

		private bool[] journalAttachmentNeedsSave;

		private Item[] journalAttachedItems;

		private bool[] journalAttachedItemNeedsSave;

		private OutboundAddressCache[] journalAttachedItemAddressCaches;

		private StreamAttachment smimeAttachment;

		private MimeDocument smimeDocument;

		private ConverterFlags flags;

		private MimeDocument skeleton;

		private bool skeletonInitialized;

		private bool smimeDocInitialized;

		private bool ownsMimeDocuments;

		private bool resolveAddresses;

		private List<MimeDocument> ownedEmbeddedMimeDocuments;

		private static PropertyDefinition[] XSharingProperties = new PropertyDefinition[]
		{
			InternalSchema.XSharingBrowseUrl,
			InternalSchema.XSharingCapabilities,
			InternalSchema.XSharingFlavor,
			InternalSchema.XSharingInstanceGuid,
			InternalSchema.XSharingLocalType,
			InternalSchema.XSharingProviderGuid,
			InternalSchema.XSharingProviderName,
			InternalSchema.XSharingProviderUrl,
			InternalSchema.XSharingRemoteName,
			InternalSchema.XSharingRemotePath,
			InternalSchema.XSharingRemoteType
		};

		private static string[] XSharingMimeNames = new string[]
		{
			"x-sharing-browse-url",
			"x-sharing-capabilities",
			"x-sharing-flavor",
			"x-sharing-instance-guid",
			"x-sharing-local-type",
			"x-sharing-provider-guid",
			"x-sharing-provider-name",
			"x-sharing-provider-url",
			"x-sharing-remote-name",
			"x-sharing-remote-path",
			"x-sharing-remote-type"
		};

		private static PropertyDefinition[] XModernGroupProperties = new PropertyDefinition[]
		{
			InternalSchema.XGroupMailboxSmtpAddressId
		};

		private static string[] XModernGroupMimeNames = new string[]
		{
			"X-MS-Exchange-GroupMailbox-Id"
		};

		private static readonly byte[] headerSectionBreak = CTSGlobals.AsciiEncoding.GetBytes("\r\n");

		[Flags]
		internal enum MimeFlags
		{
			SkipMessageHeaders = 0,
			WriteMessageHeaders = 1,
			SkipFromRecipientsAndSubject = 2,
			ProduceTnefCorrelationKey = 4,
			SkipContent = 8
		}

		private enum MessageFormatType
		{
			Mime,
			SummaryTnef,
			LegacyTnef
		}

		internal static class ReportConstants
		{
			internal const string Sender = "Sender: ";

			internal const string SentOnBehalf = "On-Behalf-Of: ";

			internal const string Subject = "Subject: {0}\r\n";

			internal const string MessageId = "Message-ID: {0}\r\n";

			internal const string To = "To: ";

			internal const string Cc = "Cc: ";

			internal const string Bcc = "Bcc: ";

			internal const string Recipient = "Recipient: ";

			internal const string Mailbox = "Mailbox: {0}\r\n";

			internal const string Label = "Label: {0}\r\n";

			internal const string SentTime = "SentUtc: {0}\r\n";

			internal const string ReceivedTime = "ReceivedUtc: {0}\r\n";
		}

		internal class MimeBodyOutputStream : StreamBase
		{
			internal MimeBodyOutputStream(OutboundConversionOptions options, MimeStreamWriter writer, MimePartContentType contentType, string charset, bool copyToSkeleton, int detectionBufferSize) : base(StreamBase.Capabilities.Writable)
			{
				this.options = options;
				this.writer = writer;
				this.charset = charset;
				this.detectionBufferSize = detectionBufferSize;
				this.contentType = contentType;
				this.copyToSkeleton = copyToSkeleton;
				if (!this.TryInitializeStream())
				{
					this.detectionBuffer = new byte[this.detectionBufferSize];
					this.detectionBytesWritten = 0;
					this.isLongStringDetected = false;
					this.isNonAsciiDetected = false;
				}
			}

			internal MimeBodyOutputStream(OutboundConversionOptions options, MimeStreamWriter writer, MimePartContentType contentType, string charset, bool copyToSkeleton) : this(options, writer, contentType, charset, copyToSkeleton, 32768)
			{
			}

			public override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<ItemToMimeConverter.MimeBodyOutputStream>(this);
			}

			private bool TryInitializeStream()
			{
				if (this.PrepareEncoder())
				{
					if (this.encoderName != null)
					{
						this.writer.WriteHeader(HeaderId.ContentTransferEncoding, this.encoderName);
						this.mimeStream = this.writer.GetContentStream(this.copyToSkeleton);
						this.outputStream = (this.encoderStream = new EncoderStream(this.mimeStream, this.encoder, EncoderStreamAccess.Write));
					}
					else
					{
						this.outputStream = (this.mimeStream = this.writer.GetContentStream(this.copyToSkeleton));
					}
					if (this.detectionBytesWritten != 0)
					{
						this.outputStream.Write(this.detectionBuffer, 0, this.detectionBytesWritten);
					}
					this.detectionBytesWritten = 0;
					this.detectionBuffer = null;
					return true;
				}
				return false;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				base.CheckDisposed("Write");
				if (this.outputStream != null)
				{
					this.outputStream.Write(buffer, offset, count);
					return;
				}
				while (count > 0 && this.detectionBytesWritten < this.detectionBufferSize)
				{
					byte b = buffer[offset++];
					count--;
					this.detectionBuffer[this.detectionBytesWritten++] = b;
					if (b == 13)
					{
						this.stringBytes++;
						this.hasCR = true;
						if (this.stringBytes > 999)
						{
							this.isLongStringDetected = true;
							break;
						}
					}
					else if (b == 10 && this.hasCR)
					{
						this.stringBytes = 0;
						this.hasCR = false;
					}
					else
					{
						if (b > 127)
						{
							this.isNonAsciiDetected = true;
							break;
						}
						this.hasCR = false;
						this.stringBytes++;
						if (this.stringBytes > 998)
						{
							this.isLongStringDetected = true;
							break;
						}
					}
				}
				if (this.isLongStringDetected || this.isNonAsciiDetected || count > 0)
				{
					this.TryInitializeStream();
					this.outputStream.Write(buffer, offset, count);
				}
			}

			public override void Flush()
			{
				base.CheckDisposed("Flush");
				if (this.outputStream != null)
				{
					this.outputStream.Flush();
					return;
				}
				if (this.detectionBytesWritten != 0)
				{
					this.TryInitializeStream();
					this.outputStream.Flush();
				}
			}

			protected override void Dispose(bool disposing)
			{
				try
				{
					if (disposing && !base.IsClosed)
					{
						this.Flush();
						if (this.encoderStream != null)
						{
							this.encoderStream.Dispose();
							this.encoderStream = null;
						}
						if (this.encoder != null)
						{
							this.encoder.Dispose();
							this.encoder = null;
						}
						if (this.mimeStream != null)
						{
							this.mimeStream.Dispose();
							this.mimeStream = null;
						}
					}
				}
				finally
				{
					base.Dispose(disposing);
				}
			}

			private void CreateBase64Encoder()
			{
				this.encoder = new Base64Encoder();
				this.encoderName = "base64";
			}

			private void CreateQPEncoder()
			{
				this.encoder = new QPEncoder();
				this.encoderName = "quoted-printable";
			}

			private bool PrepareEncoder()
			{
				int internetEncodingFromCharset = ConvertUtils.GetInternetEncodingFromCharset(this.charset);
				int num = internetEncodingFromCharset;
				if (num <= 50932)
				{
					if (num <= 950)
					{
						if (num != 932 && num != 936)
						{
							switch (num)
							{
							case 949:
							case 950:
								break;
							default:
								goto IL_115;
							}
						}
						if (this.contentType == MimePartContentType.TextHtml)
						{
							this.CreateQPEncoder();
							return true;
						}
						this.CreateBase64Encoder();
						return true;
					}
					else if (num != 20127)
					{
						switch (num)
						{
						case 50220:
						case 50221:
						case 50222:
						case 50225:
							break;
						case 50223:
						case 50224:
						case 50226:
						case 50228:
							goto IL_115;
						case 50227:
						case 50229:
							goto IL_10D;
						default:
							if (num != 50932)
							{
								goto IL_115;
							}
							goto IL_10D;
						}
					}
				}
				else if (num <= 51936)
				{
					if (num != 50949 && num != 51932 && num != 51936)
					{
						goto IL_115;
					}
					goto IL_10D;
				}
				else
				{
					switch (num)
					{
					case 51949:
					case 51950:
						goto IL_10D;
					default:
						if (num == 52936)
						{
							goto IL_10D;
						}
						switch (num)
						{
						case 65000:
							break;
						case 65001:
							goto IL_10D;
						default:
							goto IL_115;
						}
						break;
					}
				}
				return this.SelectEncodingFor7BitCharset();
				IL_10D:
				this.CreateBase64Encoder();
				return true;
				IL_115:
				this.CreateQPEncoder();
				return true;
			}

			private bool SelectEncodingFor7BitCharset()
			{
				ByteEncoderTypeFor7BitCharsets byteEncoderTypeFor7BitCharsets = this.options.ByteEncoderTypeFor7BitCharsets;
				switch (byteEncoderTypeFor7BitCharsets)
				{
				case ByteEncoderTypeFor7BitCharsets.UseQP:
					this.CreateQPEncoder();
					return true;
				case ByteEncoderTypeFor7BitCharsets.UseBase64:
					this.CreateBase64Encoder();
					return true;
				case (ByteEncoderTypeFor7BitCharsets)3:
				case (ByteEncoderTypeFor7BitCharsets)4:
					break;
				case ByteEncoderTypeFor7BitCharsets.UseQPHtmlDetectTextPlain:
					if (this.contentType == MimePartContentType.TextHtml)
					{
						this.CreateQPEncoder();
						return true;
					}
					if (this.detectionBytesWritten == 0)
					{
						return false;
					}
					if (this.isLongStringDetected || this.isNonAsciiDetected)
					{
						this.CreateQPEncoder();
						return true;
					}
					return true;
				case ByteEncoderTypeFor7BitCharsets.UseBase64HtmlDetectTextPlain:
					if (this.contentType == MimePartContentType.TextHtml)
					{
						this.CreateBase64Encoder();
						return true;
					}
					if (this.detectionBytesWritten == 0)
					{
						return false;
					}
					if (this.isLongStringDetected || this.isNonAsciiDetected)
					{
						this.CreateBase64Encoder();
						return true;
					}
					return true;
				default:
					switch (byteEncoderTypeFor7BitCharsets)
					{
					case ByteEncoderTypeFor7BitCharsets.UseQPHtml7BitTextPlain:
						if (this.contentType == MimePartContentType.TextHtml)
						{
							this.CreateQPEncoder();
							return true;
						}
						return true;
					case ByteEncoderTypeFor7BitCharsets.UseBase64Html7BitTextPlain:
						if (this.contentType == MimePartContentType.TextHtml)
						{
							this.CreateBase64Encoder();
							return true;
						}
						return true;
					}
					break;
				}
				return true;
			}

			internal bool IsReadyToStream
			{
				get
				{
					return this.outputStream != null;
				}
			}

			internal int DetectionBufferSize
			{
				get
				{
					return this.detectionBufferSize;
				}
			}

			private const int DefaultDetectionBufferSize = 32768;

			private const int MaxStringLength = 998;

			private readonly MimeStreamWriter writer;

			private readonly OutboundConversionOptions options;

			private readonly MimePartContentType contentType;

			private readonly string charset;

			private readonly int detectionBufferSize;

			private bool copyToSkeleton;

			private ByteEncoder encoder;

			private Stream encoderStream;

			private Stream mimeStream;

			private Stream outputStream;

			private string encoderName;

			private byte[] detectionBuffer;

			private int detectionBytesWritten;

			private bool isLongStringDetected;

			private bool isNonAsciiDetected;

			private bool hasCR;

			private int stringBytes;
		}
	}
}
