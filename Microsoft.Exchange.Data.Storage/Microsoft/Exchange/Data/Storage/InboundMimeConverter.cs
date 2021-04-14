using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Encoders;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.TextConverters.Internal;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class InboundMimeConverter
	{
		private static string DecodeMimeTypeValueHeader(string headerValue, out string value)
		{
			int num = headerValue.IndexOf(';');
			string text = (num != -1) ? headerValue.Substring(0, num).Trim() : string.Empty;
			if (text.Length == 0)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::DecodeMimeTypeValueHeader: type is empty");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
			}
			if (num + 1 < headerValue.Length)
			{
				value = headerValue.Substring(num + 1).Trim();
			}
			else
			{
				value = string.Empty;
			}
			return text;
		}

		private static int GetDsnActionIndex(string action)
		{
			return Array.FindIndex<KeyValuePair<string, string>>(InboundMimeConverter.DsnActionToClass, (KeyValuePair<string, string> element) => 0 == string.Compare(element.Key, action, StringComparison.CurrentCultureIgnoreCase));
		}

		private static string GetRequiredHeader(IDictionary<string, string> headers, string header)
		{
			string result;
			if (headers.TryGetValue(header, out result))
			{
				return result;
			}
			StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetRequiredHeader: header not found, {0}.", header);
			throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
		}

		private static bool IsDsnPositive(int actionIndex)
		{
			if (actionIndex < 0)
			{
				return false;
			}
			string itemClass = ObjectClass.MakeReportClassName("IPM.Note", InboundMimeConverter.DsnActionToClass[actionIndex].Value);
			return ObjectClass.IsDsnPositive(itemClass);
		}

		private static bool WillStoreRemoveOriginalMessageAttachment(string reportClassNameOrSuffix)
		{
			return ObjectClass.IsReport(reportClassNameOrSuffix, "DR");
		}

		private static Participant GetDsnMdnParticipant(IDictionary<string, string> headers)
		{
			string displayName;
			headers.TryGetValue("X-Display-Name", out displayName);
			string requiredHeader;
			if (!headers.TryGetValue("Original-recipient", out requiredHeader))
			{
				requiredHeader = InboundMimeConverter.GetRequiredHeader(headers, "Final-recipient");
			}
			return InboundMimeConverter.PromoteTypedParticipant(displayName, requiredHeader);
		}

		private static Participant PromoteTypedParticipant(string displayName, string unparsedParticipant)
		{
			string emailAddress;
			string text = InboundMimeConverter.DecodeMimeTypeValueHeader(unparsedParticipant, out emailAddress);
			if (string.IsNullOrEmpty(emailAddress))
			{
				StorageGlobals.ContextTraceError<string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::PromoteTypedParticipant: empty email address, participant {0}.", unparsedParticipant);
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
			}
			if (InboundMimeConverter.MimeStringEquals(text, "RFC822"))
			{
				Participant participant = Participant.Parse(emailAddress);
				emailAddress = participant.EmailAddress;
				if (participant.RoutingType == null)
				{
					return participant;
				}
				if (participant.RoutingType != "SMTP")
				{
					text = null;
				}
				else
				{
					text = "SMTP";
				}
			}
			else
			{
				text = Util.SubstringAfterPrefix(text, "X-", StringComparison.OrdinalIgnoreCase);
			}
			if (text != null)
			{
				return new Participant(displayName, emailAddress, text);
			}
			return new Participant(emailAddress, null, null);
		}

		private static Recipient PromoteDsnRecipientHeaders(MessageItem message, Dictionary<string, string> recipientHeaders, out int actionIndex)
		{
			string text = EmailMessageHelpers.RemoveMimeHeaderComments(InboundMimeConverter.GetRequiredHeader(recipientHeaders, "Status"));
			actionIndex = InboundMimeConverter.GetDsnActionIndex(EmailMessageHelpers.RemoveMimeHeaderComments(InboundMimeConverter.GetRequiredHeader(recipientHeaders, "Action")));
			if (actionIndex == -1)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::PromoteDsnRecipientHeaders: unknown DSN action.");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
			}
			string baseClass = ObjectClass.MakeReportClassName("IPM.Note", InboundMimeConverter.DsnActionToClass[actionIndex].Value);
			if (!ObjectClass.IsOfClass(message.ClassName, baseClass))
			{
				return null;
			}
			Recipient recipient = message.Recipients.Add(InboundMimeConverter.GetDsnMdnParticipant(recipientHeaders));
			string text2;
			if (recipientHeaders.TryGetValue("Remote-MTA", out text2))
			{
				InboundMimeConverter.DecodeMimeTypeValueHeader(text2, out text2);
				recipient[InternalSchema.RemoteMta] = text2;
			}
			string text3;
			if (!recipientHeaders.TryGetValue("X-Supplementary-Info", out text3))
			{
				string text4;
				recipientHeaders.TryGetValue("Diagnostic-code", out text4);
				text3 = string.Format("<{0} #{1}{2}{3}>", new object[]
				{
					text2,
					text,
					(text4 != null) ? " " : string.Empty,
					text4
				});
			}
			if (text3 != null)
			{
				recipient[InternalSchema.SupplementaryInfo] = text3;
			}
			if (!InboundMimeConverter.IsDsnPositive(actionIndex))
			{
				MapiDiagnosticCode mapiDiagnosticCode;
				NdrReasonCode ndrReasonCode;
				int num;
				DsnMdnUtil.GetMapiDsnRecipientStatusCode(text, out mapiDiagnosticCode, out ndrReasonCode, out num);
				recipient[InternalSchema.NdrDiagnosticCode] = mapiDiagnosticCode;
				recipient[InternalSchema.NdrReasonCode] = ndrReasonCode;
				recipient[InternalSchema.NdrStatusCode] = num;
				recipient[InternalSchema.ReportText] = string.Format("Diagnostic code = {0}; Reason code = {1}; Status code = {2}", mapiDiagnosticCode, ndrReasonCode, num);
			}
			return recipient;
		}

		private bool TryPromoteDsnMdnOriginalMessage(MessageItem message, Attachment mimeOriginalMessageAttachment, IList<KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>> promoteFromOriginal, out string originalItemClass, out bool? isMessageHeaders)
		{
			if (EmailMessageHelpers.IsEmbeddedMessageAttachment(mimeOriginalMessageAttachment))
			{
				isMessageHeaders = new bool?(false);
			}
			else
			{
				if (!ConvertUtils.MimeStringEquals(mimeOriginalMessageAttachment.ContentType, "text/rfc822-headers"))
				{
					StorageGlobals.ContextTraceDebug<string>(ExTraceGlobals.CcInboundMimeTracer, "Can't promote an original message (type = \"{0}\") for DSN/MDN, because it's neither RFC822 nor RFC822-HEADERS", mimeOriginalMessageAttachment.ContentType);
					isMessageHeaders = null;
					originalItemClass = null;
					return false;
				}
				isMessageHeaders = new bool?(true);
			}
			using (ItemAttachment itemAttachment = (ItemAttachment)this.item.AttachmentCollection.Create(AttachmentType.EmbeddedMessage))
			{
				this.PromoteAttachedMessage(itemAttachment, mimeOriginalMessageAttachment, false);
				itemAttachment.Save();
				itemAttachment.Load(null);
				StorePropertyDefinition[] pairListKeys = Util.GetPairListKeys<StorePropertyDefinition, StorePropertyDefinition>(promoteFromOriginal);
				using (Item itemAsReadOnly = itemAttachment.GetItemAsReadOnly(InternalSchema.Combine<PropertyDefinition>(MessageItemSchema.Instance.AutoloadProperties, pairListKeys)))
				{
					for (int i = 0; i < promoteFromOriginal.Count; i++)
					{
						object propertyValue = itemAsReadOnly.TryGetProperty(pairListKeys[i]);
						message.PropertyBag.SetOrDeleteProperty(promoteFromOriginal[i].Value, propertyValue);
					}
					message.PropertyBag.SetOrDeleteProperty(InternalSchema.NormalizedSubject, itemAsReadOnly.TryGetProperty(InternalSchema.NormalizedSubject));
					originalItemClass = itemAsReadOnly.ClassName;
				}
			}
			if (PropertyError.IsPropertyNotFound(message.TryGetProperty(InternalSchema.OriginalSentTime)))
			{
				ExDateTime? valueAsNullable = message.GetValueAsNullable<ExDateTime>(InternalSchema.LastModifiedTime);
				if (valueAsNullable == null)
				{
					valueAsNullable = new ExDateTime?(ExDateTime.GetNow(message.PropertyBag.ExTimeZone));
				}
				message.PropertyBag.SetOrDeleteProperty(InternalSchema.OriginalSentTime, valueAsNullable.Value);
			}
			return true;
		}

		private void PromoteDsnReportBody(MessageItem message, MimePart mimePart)
		{
			if (this.mimeAddressCache.Recipients.Count == 1 && null == this.mimeAddressCache.Participants[ConversionItemParticipants.ParticipantIndex.ReceivedRepresenting])
			{
				this.mimeAddressCache.Participants[ConversionItemParticipants.ParticipantIndex.ReceivedRepresenting] = this.mimeAddressCache.Recipients[0].Participant;
			}
			this.mimeAddressCache.ClearRecipients();
			Stream stream = null;
			try
			{
				if (!mimePart.TryGetContentReadStream(out stream))
				{
					stream = mimePart.GetRawContentReadStream();
				}
				bool fallbackToRaw = false;
				MimeLimits mimeLimits = MimeLimits.Unlimited;
				if (this.emailMessage != null && this.emailMessage.MimeDocument != null)
				{
					mimeLimits = this.emailMessage.MimeDocument.MimeLimits;
					if ((this.emailMessage.MimeDocument.HeaderDecodingOptions.DecodingFlags & DecodingFlags.FallbackToRaw) != DecodingFlags.None)
					{
						fallbackToRaw = true;
					}
				}
				Dictionary<string, string> dictionary = InboundMimeConverter.ReadMimeHeaders(stream, fallbackToRaw, mimeLimits);
				if (dictionary == null)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::PromoteDsnReportBody: wrong report herders.");
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
				}
				string value;
				InboundMimeConverter.DecodeMimeTypeValueHeader(InboundMimeConverter.GetRequiredHeader(dictionary, "Reporting-MTA"), out value);
				message[InternalSchema.ReportingMta] = value;
				for (int i = 0; i < this.conversionOptions.Limits.MaxMimeRecipients; i++)
				{
					Dictionary<string, string> dictionary2 = InboundMimeConverter.ReadMimeHeaders(stream, fallbackToRaw, mimeLimits);
					if (dictionary2 == null)
					{
						break;
					}
					if (dictionary2.Count != 0)
					{
						int actionIndex;
						Recipient recipient = InboundMimeConverter.PromoteDsnRecipientHeaders(message, dictionary2, out actionIndex);
						if (recipient != null)
						{
							recipient[InternalSchema.ReportTime] = message.SentTime;
							if (InboundMimeConverter.IsDsnPositive(actionIndex))
							{
								recipient[InternalSchema.DeliveryTime] = message.SentTime;
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
			if (message.Recipients.Count == 0)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::PromoteDsnReportBody: no recipients.");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
			}
		}

		private void PromoteMdnReportBody(MessageItem message, MimePart mimePart)
		{
			Stream stream = null;
			try
			{
				if (!mimePart.TryGetContentReadStream(out stream))
				{
					stream = mimePart.GetRawContentReadStream();
				}
				Dictionary<string, string> dictionary = InboundMimeConverter.ReadMimeHeaders(stream, false, MimeLimits.Unlimited);
				if (dictionary == null || InboundMimeConverter.ReadMimeHeaders(stream, false, MimeLimits.Unlimited) != null)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::PromoteMdnReportBody: wrong report headers.");
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
				}
				Participant dsnMdnParticipant = InboundMimeConverter.GetDsnMdnParticipant(dictionary);
				message[InternalSchema.OriginalDisplayTo] = dsnMdnParticipant.DisplayName;
				Participant v = this.mimeAddressCache.Participants[ConversionItemParticipants.ParticipantIndex.From];
				Participant v2 = this.mimeAddressCache.Participants[ConversionItemParticipants.ParticipantIndex.Sender];
				if (v == null && v2 == null)
				{
					this.mimeAddressCache.Participants[ConversionItemParticipants.ParticipantIndex.From] = dsnMdnParticipant;
					this.mimeAddressCache.Participants[ConversionItemParticipants.ParticipantIndex.Sender] = dsnMdnParticipant;
				}
				message[InternalSchema.OriginalDeliveryTime] = message.SentTime;
				message[InternalSchema.ReceiptTime] = message.SentTime;
				message[InternalSchema.ReportTime] = message.SentTime;
				string requiredHeader = InboundMimeConverter.GetRequiredHeader(dictionary, "Disposition");
				message[InternalSchema.ReportText] = requiredHeader;
				string text;
				if (dictionary.TryGetValue("X-MSExch-Correlation-Key", out text))
				{
					try
					{
						message[InternalSchema.ParentKey] = Convert.FromBase64String(text.Trim());
					}
					catch (FormatException)
					{
						StorageGlobals.ContextTraceDebug<string>(ExTraceGlobals.CcInboundTnefTracer, "InboundMimeConverter::PromoteMdnReportBody: error in correlation key, {0}.", text);
					}
				}
				string value;
				if (dictionary.TryGetValue("Original-Message-ID", out value))
				{
					message[InternalSchema.OriginalMessageId] = value;
					message[InternalSchema.InternetReferences] = value;
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

		private bool TryPromoteDsn()
		{
			Header header = this.MessageRoot.Headers.FindFirst("X-MS-Exchange-Organization-Dsn-Version");
			string headerValue = MimeHelpers.GetHeaderValue(header, this.conversionOptions);
			int num;
			bool preserveBody = this.conversionOptions.PreserveReportBody || (int.TryParse(headerValue, NumberStyles.Any, CultureInfo.InvariantCulture, out num) && num >= 12);
			return this.TryPromoteDsnMdn("message/delivery-status", preserveBody, new InboundMimeConverter.ReportBodyPartPromoter(this.PromoteDsnReportBody), InboundMimeConverter.dsnPromoteFromOriginalProperties);
		}

		private bool TryPromoteDsnMdn(string expectedReportContentType, bool preserveBody, InboundMimeConverter.ReportBodyPartPromoter reportPartPromoter, IList<KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>> promoteFromOriginal)
		{
			MessageItem message = this.item as MessageItem;
			if (message == null)
			{
				return false;
			}
			preserveBody = (preserveBody || this.IsStreamToStreamConversion || this.MessageLevel == MimeMessageLevel.AttachedMessage);
			try
			{
				List<Attachment> list = this.FindDsnMdnAttachments(this.MessageRoot);
				if (list.Count > 0 && list.Count < 3 && InboundMimeConverter.MimeStringEquals(list[0].ContentType, expectedReportContentType))
				{
					Attachment attachment = list[0];
					Attachment originalMessageAttachment = (list.Count == 2) ? (originalMessageAttachment = list[1]) : null;
					this.HeadersParser.PromoteMessageHeaders();
					string originalItemClass = "IPM.Note";
					bool? originalMessageContainsOnlyHeaders = null;
					reportPartPromoter(message, attachment.MimePart);
					if (originalMessageAttachment != null)
					{
						try
						{
							ConvertUtils.CallCts(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::TryPromoteDsnMdn(original message)", ServerStrings.ConversionCorruptContent, delegate
							{
								string originalItemClass;
								if (this.TryPromoteDsnMdnOriginalMessage(message, originalMessageAttachment, promoteFromOriginal, out originalItemClass, out originalMessageContainsOnlyHeaders))
								{
									originalItemClass = originalItemClass;
								}
							});
						}
						catch (StoragePermanentException ex)
						{
							StorageGlobals.ContextTraceDebug<StoragePermanentException>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::TryPromoteDsnMdn(original message): StoragePermanentException: {0}", ex);
							ItemConversion.SaveFailedMimeDocument(this.MessageRoot, this.conversionOptions, ex);
						}
					}
					string className = message.ClassName;
					if (!ObjectClass.ReportClasses.IsReportOfSpecialCasedClass(className))
					{
						string text = null;
						foreach (KeyValuePair<string, string> keyValuePair in Util.MergeArrays<KeyValuePair<string, string>>(new ICollection<KeyValuePair<string, string>>[]
						{
							InboundMimeConverter.DsnActionToClass,
							InboundMimeConverter.MdnActionToClass
						}))
						{
							if ((text == null || text.Length <= keyValuePair.Value.Length) && className.EndsWith(keyValuePair.Value, StringComparison.Ordinal))
							{
								text = keyValuePair.Value;
							}
						}
						if (text != null)
						{
							message.ClassName = ObjectClass.MakeReportClassName(originalItemClass, text);
						}
					}
					message.AutoResponseSuppress = AutoResponseSuppress.All;
					message[InternalSchema.Flags] = MessageFlags.None;
					if (!ObjectClass.IsReport(message.ClassName, "NDR") || originalMessageContainsOnlyHeaders == true)
					{
						this.promotedMimeParts.Clear();
						if (!InboundMimeConverter.WillStoreRemoveOriginalMessageAttachment(message.ClassName))
						{
							message.AttachmentCollection.RemoveAll();
						}
					}
					if (!preserveBody)
					{
						MimeDocument mimeDocument = null;
						Stream stream = null;
						try
						{
							HeaderList headerList;
							if (originalMessageContainsOnlyHeaders == false)
							{
								headerList = originalMessageAttachment.EmbeddedMessage.RootPart.Headers;
							}
							else if (originalMessageContainsOnlyHeaders == true)
							{
								stream = originalMessageAttachment.GetContentReadStream();
								mimeDocument = ItemConversion.LoadInboundMimeDocument(stream, this.ConversionOptions);
								headerList = mimeDocument.RootPart.Headers;
							}
							else
							{
								headerList = null;
							}
							CultureInfo culture;
							Charset charset;
							using (Stream stream2 = ReportMessage.GenerateReportBody(message, this.conversionOptions.DsnHumanReadableWriter ?? DsnHumanReadableWriter.DefaultDsnHumanReadableWriter, headerList, out culture, out charset))
							{
								stream2.Position = 0L;
								BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextHtml, charset.Name);
								using (Stream stream3 = message.Body.OpenWriteStream(configuration))
								{
									Util.StreamHandler.CopyStreamData(stream2, stream3);
								}
								int lcidFromCulture = LocaleMap.GetLcidFromCulture(culture);
								if (lcidFromCulture != 0)
								{
									message[MessageItemSchema.MessageLocaleId] = lcidFromCulture;
								}
								else
								{
									message.Delete(MessageItemSchema.MessageLocaleId);
								}
							}
							goto IL_428;
						}
						finally
						{
							if (mimeDocument != null)
							{
								mimeDocument.Dispose();
							}
							if (stream != null)
							{
								stream.Dispose();
							}
						}
					}
					StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting MIME part (InboundMimeConverter.TryPromoteDsnMdn)");
					this.PromoteMessageBody();
					StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing MIME part (InboundMimeConverter.TryPromoteDsnMdn)");
					IL_428:
					return true;
				}
				StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::TryPromoteDsnMdn: wrong mime structure for DSN, promoting as generic message.");
				return false;
			}
			catch (ConversionFailedException ex2)
			{
				if (ex2.ConversionFailureReason != ConversionFailureReason.CorruptContent)
				{
					throw;
				}
				StorageGlobals.ContextTraceDebug(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::TryPromoteDsnMdn: DSN/MDN was corrupted. Falling back: " + ((ex2.InnerException != null) ? ex2.InnerException : ex2).Message);
			}
			message.AttachmentCollection.RemoveAll();
			message.Recipients.Clear();
			message.From = null;
			message.Sender = null;
			this.mimeAddressCache.Cleanup();
			return false;
		}

		private bool TryPromoteMdn()
		{
			return this.TryPromoteDsnMdn("message/disposition-notification", this.conversionOptions.PreserveReportBody, new InboundMimeConverter.ReportBodyPartPromoter(this.PromoteMdnReportBody), InboundMimeConverter.dsnPromoteFromOriginalProperties);
		}

		private List<Attachment> FindDsnMdnAttachments(MimePart parentPart)
		{
			List<Attachment> list = new List<Attachment>();
			foreach (Attachment attachment in this.EmailMessage.Attachments)
			{
				if (attachment.MimePart.Parent == this.MessageRoot)
				{
					list.Add(attachment);
				}
			}
			return list;
		}

		public InboundMimeConverter(Item item, EmailMessage emailMessage, InboundConversionOptions options, ConversionLimitsTracker limitsTracker, ConverterFlags flags)
		{
			Util.ThrowOnNullArgument(options, "options");
			if (!options.IgnoreImceaDomain)
			{
				Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			}
			this.item = item;
			this.conversionOptions = options;
			this.converterFlags = flags;
			this.limitsTracker = limitsTracker;
			this.emailMessage = emailMessage;
			this.item.CharsetDetector.DetectionOptions = options.DetectionOptions;
			if (this.IsReplicationMessage)
			{
				this.item.DisableConstraintValidation();
			}
		}

		public void ConvertToItem(MimePromotionFlags promotionFlags)
		{
			using (StorageGlobals.SetTraceContext(new InboundMimeConverter.MimePartTraceContext(0, this.emailMessage.RootPart)))
			{
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting MIME part (InboundMimeConverter.ConvertToItem)");
				if (!this.IsReplicationMessage && this.emailMessage.Attachments.Count + this.limitsTracker.PartCount > this.ConversionLimits.MaxBodyPartsTotal)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::ConvertToItem - Attachments limit exceeded");
					throw new ConversionFailedException(ConversionFailureReason.ExceedsLimit);
				}
				ConvertUtils.CallCts(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::ConvertToItem", ServerStrings.ConversionCorruptContent, delegate
				{
					this.ConvertToItemInternal(promotionFlags);
				});
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing MIME part (InboundMimeConverter.ConvertToItem)");
			}
		}

		private void ConvertToItemInternal(MimePromotionFlags promotionFlags)
		{
			bool flag = false;
			bool flag2 = false;
			this.limitsTracker.CountMessageBody();
			this.item.ClassName = this.emailMessage.MapiMessageClass;
			MessageItem messageItem = this.Item as MessageItem;
			if (messageItem != null)
			{
				messageItem[InternalSchema.ClientSubmittedSecurely] = this.ConversionOptions.ClientSubmittedSecurely;
				messageItem[InternalSchema.ServerSubmittedSecurely] = this.ConversionOptions.ServerSubmittedSecurely;
			}
			this.item.DeleteProperties(new PropertyDefinition[]
			{
				InternalSchema.MessageLocaleId
			});
			this.mimeAddressCache = new InboundAddressCache(this.conversionOptions, this.limitsTracker, this.MessageLevel);
			if (promotionFlags != MimePromotionFlags.Default)
			{
				this.PromoteIpmNote(promotionFlags);
			}
			else
			{
				InboundMimeConverter.MimeMessageClass mimeMessageClass = this.GetMimeMessageClass();
				if ((mimeMessageClass == InboundMimeConverter.MimeMessageClass.ClassDSN || mimeMessageClass == InboundMimeConverter.MimeMessageClass.ClassMDN) && this.ConversionOptions.ConvertReportToMessage)
				{
					this.Item.ClassName = "IPM.Note";
					mimeMessageClass = InboundMimeConverter.MimeMessageClass.ClassIpmNote;
				}
				this.initialClass = mimeMessageClass;
				switch (mimeMessageClass)
				{
				case InboundMimeConverter.MimeMessageClass.ClassIpmNote:
					this.PromoteIpmNote(MimePromotionFlags.Default);
					this.SaveSkeletonData(false);
					goto IL_262;
				case InboundMimeConverter.MimeMessageClass.ClassIpmNoteSummaryTnef:
				{
					bool flag4;
					bool flag3 = this.PromoteTnef(this.EmailMessage.TnefPart, 0, true, out flag4);
					if (flag3)
					{
						this.HeadersParser.PromoteMessageHeaders();
						flag2 = (!flag4 && ObjectClass.IsMdn(this.Item.ClassName));
						goto IL_262;
					}
					this.Item.ClassName = "IPM.Note";
					this.PromoteIpmNote(MimePromotionFlags.Default);
					this.SaveSkeletonData(false);
					goto IL_262;
				}
				case InboundMimeConverter.MimeMessageClass.ClassIpmNoteLegacyTnef:
				{
					bool flag5;
					this.PromoteLegacyTnef(out flag5);
					flag2 = (!flag5 && ObjectClass.IsMdn(this.Item.ClassName));
					goto IL_262;
				}
				case InboundMimeConverter.MimeMessageClass.ClassIpmNoteSmime:
				case InboundMimeConverter.MimeMessageClass.ClassIpmNoteSmimeMultipartSigned:
					this.PromoteSmimeMessage(mimeMessageClass);
					this.SaveSkeletonData(true);
					goto IL_262;
				case InboundMimeConverter.MimeMessageClass.ClassCalendar:
					this.PromoteCalendarMessage();
					flag = ObjectClass.IsMeetingRequest(this.item.ClassName);
					this.SaveSkeletonData(false);
					goto IL_262;
				case InboundMimeConverter.MimeMessageClass.ClassDSN:
					if (!this.TryPromoteDsn())
					{
						this.Item.ClassName = "IPM.Note";
						this.PromoteIpmNote(MimePromotionFlags.Default);
					}
					this.SaveSkeletonData(false);
					goto IL_262;
				case InboundMimeConverter.MimeMessageClass.ClassMDN:
					if (!this.TryPromoteMdn())
					{
						this.Item.ClassName = "IPM.Note";
						this.PromoteIpmNote(MimePromotionFlags.Default);
					}
					this.SaveSkeletonData(false);
					goto IL_262;
				case InboundMimeConverter.MimeMessageClass.Journal:
					this.conversionOptions.ClearCategories = false;
					this.PromoteJournalReport();
					this.SaveSkeletonData(false);
					goto IL_262;
				}
				throw new InvalidOperationException();
				IL_262:
				if (ObjectClass.IsOutlookRecall(this.item.ClassName))
				{
					bool valueOrDefault = this.item.GetValueOrDefault<bool>(InternalSchema.IsReadReceiptRequestedInternal);
					if (valueOrDefault)
					{
						this.item[InternalSchema.IsReadReceiptRequested] = false;
					}
				}
			}
			this.ResolveAddresses();
			if (flag)
			{
				CalendarItemBase.CopyPropertiesTo(this.item, this.item, new StorePropertyDefinition[]
				{
					InternalSchema.DisplayAll
				}, new StorePropertyDefinition[]
				{
					InternalSchema.DisplayAttendeesAll
				});
			}
			if (flag2)
			{
				this.GenerateMdnBody();
			}
			bool promoteEmptyBody = !ObjectClass.IsReport(this.item.ClassName);
			this.PromoteDelayedBody(this.item, promoteEmptyBody);
			this.Item.SaveFlags |= this.ConversionOptions.GetSaveFlags(this.MessageLevel == MimeMessageLevel.TopLevelMessage);
		}

		private void SaveSkeletonData(bool isSmime)
		{
			using (Stream stream = this.Item.OpenPropertyStream(InternalSchema.MimeSkeleton, PropertyOpenMode.Create))
			{
				if (isSmime)
				{
					using (MimeWriter mimeWriter = new MimeWriter(stream, false, null))
					{
						mimeWriter.StartPart();
						foreach (Header header in this.EmailMessage.RootPart.Headers)
						{
							string name = header.Name;
							if (this.IsStreamToStreamConversion || !MimeConstants.IsInReservedHeaderNamespace(name) || (!this.ConversionOptions.ApplyHeaderFirewall && MimeConstants.IsReservedHeaderAllowedOnDelivery(name)))
							{
								header.WriteTo(mimeWriter);
							}
						}
						if (this.EmailMessage.RootPart.IsMultipart)
						{
							foreach (MimePart mimePart in this.EmailMessage.RootPart)
							{
								mimeWriter.StartPart();
								mimeWriter.EndPart();
							}
						}
						mimeWriter.EndPart();
						goto IL_123;
					}
				}
				this.EmailMessage.RootPart.WriteTo(stream, null, new InboundMimeConverter.SkeletonOutputFilter(this.promotedMimeParts, this.IsStreamToStreamConversion, this.ConversionOptions));
				IL_123:;
			}
		}

		private void ResolveAddresses()
		{
			string className = this.Item.ClassName;
			bool importResourceFromTnef = false;
			InboundAddressCache inboundAddressCache;
			InboundAddressCache inboundAddressCache2;
			if (ObjectClass.IsNonSendableWithRecipients(className) && this.tnefAddressCache != null)
			{
				inboundAddressCache = this.tnefAddressCache;
				inboundAddressCache2 = null;
			}
			else if (this.MimeRecipientCacheHasHigherPriority() || this.tnefAddressCache == null)
			{
				inboundAddressCache = this.mimeAddressCache;
				inboundAddressCache2 = this.tnefAddressCache;
				importResourceFromTnef = (this.tnefAddressCache != null && this.IsStreamToStreamConversion && ObjectClass.IsMeetingMessage(this.Item.ClassName));
			}
			else
			{
				inboundAddressCache = this.tnefAddressCache;
				inboundAddressCache2 = this.mimeAddressCache;
			}
			if (inboundAddressCache2 != null)
			{
				inboundAddressCache.AddDependentAddressCache(inboundAddressCache2);
			}
			if (this.isResolvingParticipants)
			{
				inboundAddressCache.Resolve();
			}
			else
			{
				inboundAddressCache.ReplyTo.Resync(true);
			}
			inboundAddressCache.CopyDataToItem(this.Item.CoreItem, importResourceFromTnef);
		}

		private bool MimeRecipientCacheHasHigherPriority()
		{
			return !ObjectClass.IsDsn(this.Item.ClassName);
		}

		private bool IsTnefPart(MimePart mimePart)
		{
			ContentTypeHeader header = (ContentTypeHeader)mimePart.Headers.FindFirst(HeaderId.ContentType);
			string headerValue = MimeHelpers.GetHeaderValue(header, this.ConversionOptions);
			if (headerValue != null)
			{
				if (InboundMimeConverter.MimeStringEquals(headerValue, "application/ms-tnef") || headerValue.StartsWith("application/x-openmail", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				if (InboundMimeConverter.MimeStringEquals(headerValue, "application/octet-stream"))
				{
					string headerParameter = MimeHelpers.GetHeaderParameter(header, "name", this.ConversionOptions);
					if (headerParameter != null && headerParameter.StartsWith("winmail.dat", StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		private InboundMimeConverter.MimeMessageClass GetMimeMessageClass()
		{
			if (this.EmailMessage.TnefPart != null)
			{
				if (this.EmailMessage.TnefPart != this.MessageRoot)
				{
					return InboundMimeConverter.MimeMessageClass.ClassIpmNoteLegacyTnef;
				}
				return InboundMimeConverter.MimeMessageClass.ClassIpmNoteSummaryTnef;
			}
			else
			{
				if (this.MessageLevel == MimeMessageLevel.TopLevelMessage && this.IsTnefPart(this.MessageRoot))
				{
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.ConversionCorruptSummaryTnef, null);
				}
				if (this.EmailMessage.CalendarPart != null)
				{
					return InboundMimeConverter.MimeMessageClass.ClassCalendar;
				}
				string mapiMessageClass = this.EmailMessage.MapiMessageClass;
				if (ObjectClass.IsSmimeClearSigned(mapiMessageClass))
				{
					return InboundMimeConverter.MimeMessageClass.ClassIpmNoteSmimeMultipartSigned;
				}
				if (ObjectClass.IsSmime(mapiMessageClass))
				{
					return InboundMimeConverter.MimeMessageClass.ClassIpmNoteSmime;
				}
				if (ObjectClass.IsDsn(mapiMessageClass))
				{
					return InboundMimeConverter.MimeMessageClass.ClassDSN;
				}
				if (ObjectClass.IsMdn(mapiMessageClass))
				{
					return InboundMimeConverter.MimeMessageClass.ClassMDN;
				}
				if (!this.ConversionOptions.IsSenderTrusted || this.MessageRoot.Headers.FindFirst("X-MS-Journal-Report") == null)
				{
					return InboundMimeConverter.MimeMessageClass.ClassIpmNote;
				}
				int count = this.EmailMessage.Attachments.Count;
				if (count != 1 && count != 2)
				{
					return InboundMimeConverter.MimeMessageClass.ClassIpmNote;
				}
				if (count != 1)
				{
					foreach (Attachment attachment in this.EmailMessage.Attachments)
					{
						if (!EmailMessageHelpers.IsEmbeddedMessageAttachment(attachment))
						{
							return InboundMimeConverter.MimeMessageClass.ClassIpmNote;
						}
					}
					return InboundMimeConverter.MimeMessageClass.Journal;
				}
				Attachment attachment2 = this.EmailMessage.Attachments[0];
				if (EmailMessageHelpers.IsEmbeddedMessageAttachment(attachment2))
				{
					return InboundMimeConverter.MimeMessageClass.Journal;
				}
				if (attachment2.FileName.EndsWith(".msg", StringComparison.OrdinalIgnoreCase))
				{
					return InboundMimeConverter.MimeMessageClass.Journal;
				}
				return InboundMimeConverter.MimeMessageClass.ClassIpmNote;
			}
		}

		private void PromoteIpmNote(MimePromotionFlags promotionFlags)
		{
			this.PromoteIpmNote(promotionFlags, InboundMimeConverter.MimeBodyWrapper.RtfOptions.None);
		}

		private void PromoteIpmNote(MimePromotionFlags promotionFlags, InboundMimeConverter.MimeBodyWrapper.RtfOptions bodyOptions)
		{
			using (StorageGlobals.SetTraceContext(new InboundMimeConverter.MimePartTraceContext(0, this.MessageRoot)))
			{
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting MIME part (InboundMimeConverter.PromoteIpmNote)");
				if (!InboundMimeConverter.IsSkippingHeaders(promotionFlags))
				{
					this.HeadersParser.PromoteMessageHeaders();
				}
				bool flag = (bodyOptions & InboundMimeConverter.MimeBodyWrapper.RtfOptions.ConvertToRtf) != InboundMimeConverter.MimeBodyWrapper.RtfOptions.ConvertToRtf && this.EmailMessage.Body.BodyFormat == BodyFormat.Html;
				if (!InboundMimeConverter.IsSkippingBody(promotionFlags) && this.emailMessage.Body != null && this.emailMessage.Body.MimePart != null)
				{
					this.PromoteMessageBody(bodyOptions);
				}
				if (!InboundMimeConverter.IsSkippingAllAttachments(promotionFlags))
				{
					int count = this.emailMessage.Attachments.Count;
					for (int i = 0; i < count; i++)
					{
						Attachment attachment = this.emailMessage.Attachments[i];
						if (attachment.MimePart != null && attachment.MimePart != this.skipAttachment)
						{
							if (flag && attachment.AttachmentType == AttachmentType.Inline)
							{
								if (InboundMimeConverter.IsSkippingInlineAttachments(promotionFlags))
								{
									goto IL_F6;
								}
							}
							else if (InboundMimeConverter.IsSkippingRegularAttachments(promotionFlags))
							{
								goto IL_F6;
							}
							this.PromoteAttachment(attachment, flag);
						}
						IL_F6:;
					}
				}
			}
		}

		private void PromoteJournalReport()
		{
			using (StorageGlobals.SetTraceContext(new InboundMimeConverter.MimePartTraceContext(0, this.MessageRoot)))
			{
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting MIME part (InboundMimeConverter.PromoteJournalReport)");
				this.HeadersParser.PromoteMessageHeaders();
				this.item[InternalSchema.XMSJournalReport] = string.Empty;
				Attachment attachment = this.EmailMessage.Attachments[0];
				if (EmailMessageHelpers.IsEmbeddedMessageAttachment(attachment))
				{
					this.PromoteAttachedJournalMessage(attachment);
					if (this.EmailMessage.Attachments.Count == 2)
					{
						this.PromoteAttachedJournalMessage(this.EmailMessage.Attachments[1]);
					}
				}
				else
				{
					using (StreamAttachment streamAttachment = (StreamAttachment)this.item.AttachmentCollection.Create(AttachmentType.Stream))
					{
						this.PromoteStreamAttachment(streamAttachment, attachment, false);
						streamAttachment.Save();
					}
				}
				this.PromoteMessageBody();
			}
		}

		private void PromoteAttachedJournalMessage(Attachment journalAttachment)
		{
			bool flag = false;
			using (ItemAttachment itemAttachment = (ItemAttachment)this.item.AttachmentCollection.Create(AttachmentType.EmbeddedMessage))
			{
				try
				{
					this.PromoteAttachedMessage(itemAttachment, journalAttachment, false);
					itemAttachment.Save();
					flag = true;
				}
				catch (ConversionFailedException arg)
				{
					StorageGlobals.ContextTraceDebug<ConversionFailedException>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::PromoteAttachedJournalMessage: got ConversionFailedException, {0}.", arg);
				}
				catch (ExchangeDataException arg2)
				{
					StorageGlobals.ContextTraceDebug<ExchangeDataException>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::PromoteAttachedJournalMessage: got ExchangeDataException, {0}.", arg2);
				}
				if (!flag && itemAttachment.Id != null)
				{
					this.Item.AttachmentCollection.Remove(itemAttachment.Id);
				}
			}
			if (!flag)
			{
				using (StreamAttachment streamAttachment = (StreamAttachment)this.item.AttachmentCollection.Create(AttachmentType.Stream))
				{
					streamAttachment.FileName = "corrupt.eml";
					streamAttachment.ContentType = "application/octet-stream";
					string headerValue = MimeHelpers.GetHeaderValue(journalAttachment.MimePart.Headers, HeaderId.ContentId, this.ConversionOptions);
					if (!string.IsNullOrEmpty(headerValue))
					{
						streamAttachment.ContentId = ConvertUtils.ExtractMimeContentId(headerValue);
					}
					using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.Create))
					{
						journalAttachment.EmbeddedMessage.RootPart.WriteTo(contentStream);
					}
					this.MarkPromotedAttachment(streamAttachment, journalAttachment.MimePart);
					streamAttachment.Save();
				}
			}
		}

		private void PromoteCalendarMessage()
		{
			this.HeadersParser.PromoteMessageHeaders();
			bool flag = this.MessageRoot.Headers.FindFirst("X-Notes-Item") == null && this.EmailMessage.Body.MimePart != this.EmailMessage.CalendarPart;
			MimePart calendarPart = this.EmailMessage.CalendarPart;
			MimePromotionFlags mimePromotionFlags = MimePromotionFlags.SkipMessageHeaders;
			ContentTypeHeader header = calendarPart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
			string headerParameter = MimeHelpers.GetHeaderParameter(header, "charset", this.conversionOptions);
			Charset charset = MimeHelpers.ChooseCharset(headerParameter, "utf-8", this.ConversionOptions);
			bool flag2 = false;
			LocalizedString localizedString;
			using (Stream contentReadStream = calendarPart.GetContentReadStream())
			{
				this.calendarPartCRC = new uint?(ComputeCRC.Compute(contentReadStream));
				try
				{
					flag2 = CalendarDocument.ICalToItem(contentReadStream, this.item, this.mimeAddressCache, flag, charset.Name, out localizedString);
				}
				catch (ConversionFailedException ex)
				{
					localizedString = ex.LocalizedString;
				}
			}
			if (flag2)
			{
				this.skipAttachment = calendarPart;
				if (!flag)
				{
					mimePromotionFlags |= MimePromotionFlags.SkipMessageBody;
				}
			}
			else
			{
				this.Item.ClassName = "IPM.Note.NotSupportedICal";
				if (this.EmailMessage.MimeDocument != null)
				{
					ItemConversion.SaveFailedMimeDocument(this.MessageRoot, this.conversionOptions.ToString(), localizedString, this.conversionOptions.LogDirectoryPath);
				}
			}
			InboundMimeConverter.MimeBodyWrapper.RtfOptions bodyOptions = InboundMimeConverter.MimeBodyWrapper.RtfOptions.None;
			if (flag2)
			{
				if (this.HeadersParser.IsSentByLegacyExchange())
				{
					bodyOptions = InboundMimeConverter.MimeBodyWrapper.RtfOptions.ConvertToRtfAndFixSquigglieInHtml;
				}
				else
				{
					bodyOptions = InboundMimeConverter.MimeBodyWrapper.RtfOptions.ConvertToRtf;
				}
			}
			this.PromoteIpmNote(mimePromotionFlags, bodyOptions);
		}

		private bool PromoteMessageBody()
		{
			return this.PromoteMessageBody(InboundMimeConverter.MimeBodyWrapper.RtfOptions.None);
		}

		private bool PromoteMessageBody(InboundMimeConverter.MimeBodyWrapper.RtfOptions wrapperOptions)
		{
			if (this.EmailMessage.Body.MimePart == null || this.EmailMessage.Body.BodyFormat == BodyFormat.None)
			{
				return false;
			}
			InboundMimeConverter.MimeBodyWrapper mimeBodyWrapper = new InboundMimeConverter.MimeBodyWrapper(this.Item, this.EmailMessage.Body, this.conversionOptions, wrapperOptions);
			using (StorageGlobals.SetTraceContext(new InboundMimeConverter.MimePartTraceContext(-1, this.EmailMessage.Body.MimePart)))
			{
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting (InboundMimeConverter.PromoteMimeBody)");
				string value = mimeBodyWrapper.ContentId;
				if (this.EmailMessage.Body.MimePart == this.EmailMessage.CalendarPart || mimeBodyWrapper.GetCharset().CodePage != MimeHelpers.GetCharsetFromMime(this.EmailMessage.Body.MimePart).CodePage)
				{
					if (!string.IsNullOrEmpty(value))
					{
						this.item[InternalSchema.BodyContentId] = ConvertUtils.ExtractMimeContentId(value);
					}
				}
				else
				{
					if (string.IsNullOrEmpty(value))
					{
						if (this.conversionOptions.IgnoreImceaDomain)
						{
							value = AttachmentLink.CreateContentId(this.item.CoreItem, null, null);
						}
						else
						{
							value = AttachmentLink.CreateContentId(this.item.CoreItem, null, this.conversionOptions.ImceaEncapsulationDomain);
						}
					}
					else
					{
						value = ConvertUtils.ExtractMimeContentId(value);
					}
					this.item[InternalSchema.BodyContentId] = value;
					this.promotedMimeParts[this.EmailMessage.Body.MimePart] = value;
				}
				string contentBase = mimeBodyWrapper.ContentBase;
				if (!string.IsNullOrEmpty(contentBase))
				{
					this.item[InternalSchema.BodyContentBase] = contentBase;
				}
				string contentLocation = mimeBodyWrapper.ContentLocation;
				if (!string.IsNullOrEmpty(contentLocation))
				{
					this.item[InternalSchema.BodyContentLocation] = contentLocation;
				}
				string contentLanguage = mimeBodyWrapper.ContentLanguage;
				if (!string.IsNullOrEmpty(contentLanguage))
				{
					this.HeadersParser.SetMessageLocaleId(contentLanguage);
				}
				this.delayedBody = mimeBodyWrapper;
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing MIME part (InboundMimeConverter.PromoteMimeBody)");
			}
			return true;
		}

		private void PromoteDelayedBody(Item item, bool promoteEmptyBody)
		{
			if (this.delayedBody != null)
			{
				BodyFormat bodyFormat = this.delayedBody.BodyFormat;
				Charset charset = this.delayedBody.GetCharset();
				ConversionCallbackBase conversionCallbackBase = null;
				using (Stream stream = this.delayedBody.OpenContentStream(out conversionCallbackBase))
				{
					if (promoteEmptyBody || !this.delayedBody.IsEmpty)
					{
						BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(bodyFormat, charset);
						bodyWriteConfiguration.SetTargetFormat(bodyFormat, this.conversionOptions.DetectionOptions.PreferredCharset ?? charset, BodyCharsetFlags.PreserveUnicode);
						if (!this.delayedBody.IsCharsetPresentInMime && bodyFormat == BodyFormat.TextHtml)
						{
							bodyWriteConfiguration.TrustHtmlMetaTag = true;
						}
						using (Stream stream2 = item.Body.OpenWriteStream(bodyWriteConfiguration))
						{
							Util.StreamHandler.CopyStreamData(stream, stream2, null, 0, 65536);
						}
					}
				}
				if (conversionCallbackBase != null)
				{
					conversionCallbackBase.SaveChanges();
				}
			}
		}

		private bool IsContactAttachment(Attachment mimeAttach, out Encoding defaultEncoding)
		{
			defaultEncoding = null;
			if (ConvertUtils.MimeStringEquals(mimeAttach.ContentType, "text/directory"))
			{
				ContentTypeHeader contentTypeHeader = mimeAttach.MimePart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
				if (contentTypeHeader != null)
				{
					string headerParameter = MimeHelpers.GetHeaderParameter(contentTypeHeader, "profile", this.conversionOptions);
					if (headerParameter != null && ConvertUtils.MimeStringEquals(headerParameter, "vCard"))
					{
						defaultEncoding = Encoding.UTF8;
						return true;
					}
				}
			}
			else
			{
				ConvertUtils.MimeStringEquals(mimeAttach.ContentType, "text/x-vcard");
			}
			return false;
		}

		private void PromoteContactAttachment(Attachment mimeAttach, Encoding defaultEncoding)
		{
			Encoding encoding = null;
			ContentTypeHeader contentTypeHeader = mimeAttach.MimePart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
			if (contentTypeHeader != null)
			{
				string headerParameter = MimeHelpers.GetHeaderParameter(contentTypeHeader, "charset", this.conversionOptions);
				if (!string.IsNullOrEmpty(headerParameter))
				{
					Charset.TryGetEncoding(headerParameter, out encoding);
				}
			}
			if (encoding == null)
			{
				encoding = defaultEncoding;
			}
			using (ItemAttachment itemAttachment = this.item.AttachmentCollection.Create(StoreObjectType.Contact))
			{
				using (Contact contact = itemAttachment.GetItem(InternalSchema.ContentConversionProperties) as Contact)
				{
					using (Stream contentReadStream = mimeAttach.GetContentReadStream())
					{
						InboundVCardConverter.Convert(contentReadStream, encoding, contact, this.conversionOptions);
					}
					contact.Save(SaveMode.ResolveConflicts);
				}
				itemAttachment.Save();
			}
		}

		private void PromoteAttachment(Attachment mimeAttach, bool setInlineFlagOnAttachments)
		{
			this.limitsTracker.CountMessageAttachment();
			string text = mimeAttach.ContentType;
			if (text == null)
			{
				text = string.Empty;
			}
			if (ConvertUtils.MimeStringEquals(text, "message/partial"))
			{
				throw new ConversionFailedException(ConversionFailureReason.ConverterUnsupportedContent);
			}
			Encoding defaultEncoding = null;
			if (EmailMessageHelpers.IsEmbeddedMessageAttachment(mimeAttach) || ConvertUtils.MimeStringEquals(text, "text/rfc822-headers"))
			{
				using (ItemAttachment itemAttachment = (ItemAttachment)this.item.AttachmentCollection.Create(AttachmentType.EmbeddedMessage))
				{
					this.PromoteAttachedMessage(itemAttachment, mimeAttach, setInlineFlagOnAttachments);
					itemAttachment.Save();
					return;
				}
			}
			if (this.IsContactAttachment(mimeAttach, out defaultEncoding))
			{
				this.PromoteContactAttachment(mimeAttach, defaultEncoding);
				return;
			}
			bool flag = false;
			if (this.EmailMessage.CalendarPart != null && ConvertUtils.MimeStringEquals(text, "text/calendar"))
			{
				using (Stream contentReadStream = mimeAttach.GetContentReadStream())
				{
					long num = (long)((ulong)ComputeCRC.Compute(contentReadStream));
					bool flag2;
					if (this.calendarPartCRC != null)
					{
						uint? num2 = this.calendarPartCRC;
						long num3 = num;
						flag2 = ((ulong)num2.GetValueOrDefault() == (ulong)num3 && num2 != null);
					}
					else
					{
						flag2 = false;
					}
					flag = flag2;
				}
			}
			if (!flag)
			{
				using (StreamAttachment streamAttachment = (StreamAttachment)this.item.AttachmentCollection.Create(AttachmentType.Stream))
				{
					if (ConvertUtils.MimeStringEquals(mimeAttach.ContentType, "message/external-body"))
					{
						this.PromoteMimeMessageExternalBody(streamAttachment, mimeAttach);
					}
					else
					{
						this.PromoteStreamAttachment(streamAttachment, mimeAttach, setInlineFlagOnAttachments);
					}
					streamAttachment.Save();
				}
			}
		}

		private void MarkPromotedAttachment(Attachment attach, MimePart part)
		{
			string text = attach.ContentId;
			if (string.IsNullOrEmpty(text))
			{
				if (this.conversionOptions.IgnoreImceaDomain)
				{
					text = AttachmentLink.CreateContentId(this.item.CoreItem, attach.Id, null);
				}
				else
				{
					text = AttachmentLink.CreateContentId(this.item.CoreItem, attach.Id, this.conversionOptions.ImceaEncapsulationDomain);
				}
				attach.ContentId = text;
			}
			this.promotedMimeParts[part] = text;
		}

		private void PromoteAttachedMessage(ItemAttachment attachment, Attachment mimeAttach, bool allowInlineAttachments)
		{
			this.limitsTracker.StartEmbeddedMessage();
			Item item = null;
			MimeDocument mimeDocument = null;
			Stream stream = null;
			string text = null;
			try
			{
				ConverterFlags flags = this.ConverterFlags | ConverterFlags.IsEmbeddedMessage;
				item = attachment.GetItemAsMessage(InternalSchema.ContentConversionProperties);
				this.PromoteAttachmentProperties(attachment, mimeAttach, mimeAttach.MimePart, null, allowInlineAttachments && mimeAttach.AttachmentType == AttachmentType.Inline);
				MimePromotionFlags mimePromotionFlags = MimePromotionFlags.Default;
				EmailMessage emailMessage;
				if (EmailMessageHelpers.IsEmbeddedMessageAttachment(mimeAttach))
				{
					emailMessage = mimeAttach.EmbeddedMessage;
				}
				else
				{
					stream = mimeAttach.GetContentReadStream();
					mimeDocument = ItemConversion.LoadInboundMimeDocument(stream, this.ConversionOptions);
					emailMessage = EmailMessage.Create(mimeDocument);
					mimePromotionFlags = MimePromotionFlags.PromoteHeadersOnly;
				}
				ConversionLimitsTracker conversionLimitsTracker = (this.initialClass == InboundMimeConverter.MimeMessageClass.Journal) ? new ConversionLimitsTracker(this.ConversionOptions.Limits) : this.limitsTracker;
				new InboundMimeConverter(item, emailMessage, this.conversionOptions, conversionLimitsTracker, flags)
				{
					IsResolvingParticipants = this.conversionOptions.ResolveRecipientsInAttachedMessages
				}.ConvertToItem(mimePromotionFlags);
				text = item.GetValueOrDefault<string>(InternalSchema.Subject, string.Empty);
				int num = item.GetValueOrDefault<int>(InternalSchema.Flags);
				num &= -9;
				num |= 1;
				item[InternalSchema.Flags] = num;
				if (mimePromotionFlags == MimePromotionFlags.Default)
				{
					this.MarkPromotedAttachment(attachment, mimeAttach.MimePart);
				}
				item.Save(SaveMode.ResolveConflicts);
			}
			finally
			{
				if (mimeDocument != null)
				{
					mimeDocument.Dispose();
					mimeDocument = null;
				}
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
			}
			if (string.IsNullOrEmpty(attachment.DisplayName) && text != null)
			{
				attachment[InternalSchema.DisplayName] = text;
			}
			this.limitsTracker.EndEmbeddedMessage();
		}

		private void PromoteStreamAttachment(StreamAttachment attachment, Attachment mimeAttach, bool allowInlineAttachments)
		{
			bool isMhtml = allowInlineAttachments && mimeAttach.AttachmentType == AttachmentType.Inline;
			Stream stream = null;
			Stream stream2 = null;
			Stream stream3 = null;
			try
			{
				string text = null;
				byte[] array = null;
				if (EmailMessageHelpers.IsAppleDoubleAttachment(mimeAttach))
				{
					MimePart mimePart = mimeAttach.MimePart.Parent as MimePart;
					this.PromoteAttachmentProperties(attachment, null, mimePart, null, isMhtml);
					MimePart mimePart2 = null;
					MimePart mimePart3 = null;
					this.GetAppleDoubleParts(mimePart, out mimePart2, out mimePart3);
					stream2 = mimePart2.GetContentReadStream();
					stream3 = mimePart3.GetContentReadStream();
					stream = attachment.GetRawContentStream(PropertyOpenMode.Create);
					MimeAppleTranscoder.AppledoubleToMacBin(stream3, stream2, stream, out text, out array);
					stream3.Position = 0L;
					using (Stream stream4 = attachment.OpenPropertyStream(InternalSchema.AttachmentMacInfo, PropertyOpenMode.Create))
					{
						Util.StreamHandler.CopyStreamData(stream3, stream4, null, 0, 131072);
					}
					attachment[InternalSchema.AttachEncoding] = ConvertUtils.OidMacBinary;
					this.MarkPromotedAttachment(attachment, mimePart2);
				}
				else
				{
					this.PromoteAttachmentProperties(attachment, mimeAttach, mimeAttach.MimePart, null, isMhtml);
					if (InboundMimeConverter.MimeStringEquals(mimeAttach.MimePart.ContentType, "application/applefile"))
					{
						if (!mimeAttach.MimePart.TryGetContentReadStream(out stream2))
						{
							stream2 = mimeAttach.MimePart.GetRawContentReadStream();
						}
						try
						{
							using (Stream stream5 = attachment.OpenPropertyStream(InternalSchema.AttachmentMacInfo, PropertyOpenMode.Create))
							{
								stream = attachment.GetRawContentStream(PropertyOpenMode.Create);
								MimeAppleTranscoder.ApplesingleToMacBin(stream2, stream5, stream, out text, out array);
							}
							attachment[InternalSchema.AttachEncoding] = ConvertUtils.OidMacBinary;
							goto IL_297;
						}
						catch (MimeException ex)
						{
							this.DowngradeAppleAttachmentToOctetStream(ex, attachment, mimeAttach, ref stream, ref stream2);
							goto IL_297;
						}
					}
					if (InboundMimeConverter.MimeStringEquals(mimeAttach.MimePart.ContentType, "application/mac-binhex40"))
					{
						try
						{
							stream = attachment.GetRawContentStream(PropertyOpenMode.Create);
							if (mimeAttach.MimePart.Headers.FindFirst(HeaderId.ContentTransferEncoding) == null || mimeAttach.MimePart.ContentTransferEncoding == ContentTransferEncoding.BinHex || !mimeAttach.MimePart.TryGetContentReadStream(out stream2))
							{
								stream2 = mimeAttach.MimePart.GetRawContentReadStream();
								BinHexDecoder encoder = new BinHexDecoder(false);
								stream2 = new EncoderStream(stream2, encoder, EncoderStreamAccess.Read);
							}
							Util.StreamHandler.CopyStreamData(stream2, stream, null, 0, 131072);
							stream2.Position = 0L;
							using (Stream stream6 = attachment.OpenPropertyStream(InternalSchema.AttachmentMacInfo, PropertyOpenMode.Create))
							{
								MimeAppleTranscoder.MacBinToApplefile(stream2, stream6, out text, out array);
							}
							attachment[InternalSchema.AttachEncoding] = ConvertUtils.OidMacBinary;
							goto IL_297;
						}
						catch (MimeException ex2)
						{
							this.DowngradeAppleAttachmentToOctetStream(ex2, attachment, mimeAttach, ref stream, ref stream2);
							goto IL_297;
						}
						catch (ByteEncoderException ex3)
						{
							this.DowngradeAppleAttachmentToOctetStream(ex3, attachment, mimeAttach, ref stream, ref stream2);
							goto IL_297;
						}
					}
					stream2 = mimeAttach.GetContentReadStream();
					stream = attachment.GetContentStream(PropertyOpenMode.Create);
					Util.StreamHandler.CopyStreamData(stream2, stream, null, 0, 131072);
					IL_297:
					this.MarkPromotedAttachment(attachment, mimeAttach.MimePart);
				}
				if (array != null)
				{
					attachment[InternalSchema.AttachAdditionalInfo] = array;
				}
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
					stream = null;
				}
				if (stream2 != null)
				{
					stream2.Dispose();
					stream2 = null;
				}
				if (stream3 != null)
				{
					stream3.Dispose();
					stream3 = null;
				}
			}
		}

		private void DowngradeAppleAttachmentToOctetStream(Exception ex, StreamAttachment streamAttachment, Attachment mimeAttach, ref Stream dataStream, ref Stream inputStream)
		{
			StorageGlobals.ContextTraceError<Exception>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::DowngradeAppleAttachmentToOctetStream: attachment content-type invalid, downgrading to application/octet-stream, exception: {0}", ex);
			streamAttachment.Delete(InternalSchema.AttachmentMacInfo);
			streamAttachment.Delete(InternalSchema.AttachEncoding);
			streamAttachment.ContentType = "application/octet-stream";
			if (dataStream != null)
			{
				dataStream.Dispose();
				dataStream = null;
			}
			dataStream = streamAttachment.GetRawContentStream(PropertyOpenMode.Create);
			if (inputStream != null)
			{
				inputStream.Dispose();
				inputStream = null;
			}
			inputStream = mimeAttach.MimePart.GetContentReadStream();
			Util.StreamHandler.CopyStreamData(inputStream, dataStream);
		}

		private void PromoteMimeMessageExternalBody(StreamAttachment attachment, Attachment mimeAttach)
		{
			MimePart mimePart = mimeAttach.MimePart;
			bool isMhtml = mimeAttach.AttachmentType == AttachmentType.Inline;
			this.PromoteAttachmentProperties(attachment, mimeAttach, mimePart, null, isMhtml);
			ContentTypeHeader header = mimePart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
			string headerParameter = MimeHelpers.GetHeaderParameter(header, "access-type", this.conversionOptions);
			if (!string.IsNullOrEmpty(headerParameter) && InboundMimeConverter.MimeStringEquals(headerParameter, "anon-ftp"))
			{
				string headerParameter2 = MimeHelpers.GetHeaderParameter(header, "mode", this.conversionOptions);
				string headerParameter3 = MimeHelpers.GetHeaderParameter(header, "site", this.conversionOptions);
				string headerParameter4 = MimeHelpers.GetHeaderParameter(header, "directory", this.conversionOptions);
				string headerParameter5 = MimeHelpers.GetHeaderParameter(header, "name", this.conversionOptions);
				if (!string.IsNullOrEmpty(headerParameter3) && !string.IsNullOrEmpty(headerParameter5))
				{
					int num = headerParameter5.LastIndexOf('.');
					string text;
					if (num > 0)
					{
						text = headerParameter5.Substring(0, num) + ".url";
					}
					else
					{
						text = headerParameter5 + ".url";
					}
					attachment.FileName = text;
					attachment[InternalSchema.DisplayName] = text;
					StringBuilder stringBuilder = new StringBuilder("[InternetShortcut]\r\nURL=ftp://");
					stringBuilder.Append(headerParameter3);
					stringBuilder.Append('/');
					if (!string.IsNullOrEmpty(headerParameter4))
					{
						stringBuilder.Append(headerParameter4);
						stringBuilder.Append('/');
					}
					stringBuilder.Append(headerParameter5);
					if (!string.IsNullOrEmpty(headerParameter2))
					{
						if (InboundMimeConverter.MimeStringEquals(headerParameter2, "ascii"))
						{
							stringBuilder.Append(";type=a");
						}
						else if (InboundMimeConverter.MimeStringEquals(headerParameter2, "image"))
						{
							stringBuilder.Append(";type=i");
						}
					}
					stringBuilder.Append("\r\n");
					attachment[InternalSchema.AttachDataBin] = CTSGlobals.AsciiEncoding.GetBytes(stringBuilder.ToString());
					return;
				}
			}
			using (Stream contentStream = attachment.GetContentStream(PropertyOpenMode.Create))
			{
				mimePart.WriteTo(contentStream);
			}
		}

		private void PromoteAttachmentProperties(Attachment attachment, Attachment mimeAttach, MimePart part, string defaultFilename, bool isMhtml)
		{
			if (mimeAttach != null)
			{
				part = mimeAttach.MimePart;
			}
			ContentTypeHeader contentTypeHeader = part.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
			ContentDispositionHeader contentDispositionHeader = part.Headers.FindFirst(HeaderId.ContentDisposition) as ContentDispositionHeader;
			bool flag = false;
			string text = null;
			string text2 = null;
			bool flag2 = false;
			if (mimeAttach != null && !string.IsNullOrEmpty(mimeAttach.FileName) && !(attachment is ItemAttachment))
			{
				attachment.FileName = mimeAttach.FileName;
				text2 = attachment.FileName;
				flag = true;
			}
			string text3 = null;
			if (contentTypeHeader != null)
			{
				text3 = MimeHelpers.GetHeaderValue(contentTypeHeader, this.ConversionOptions);
				if (text3 != null)
				{
					if (InboundMimeConverter.MimeStringEquals(text3, "application/ms-tnef"))
					{
						attachment.ContentType = "application/octet-stream";
					}
					else if (InboundMimeConverter.MimeStringEquals(text3, "application/pkcs7-mime") || InboundMimeConverter.MimeStringEquals(text3, "application/x-pkcs7-mime"))
					{
						MemoryStream memoryStream = new MemoryStream(256);
						contentTypeHeader.WriteTo(memoryStream);
						memoryStream.Flush();
						byte[] array = memoryStream.ToArray();
						string text4 = CTSGlobals.AsciiEncoding.GetString(array, 0, array.Length);
						int num = text4.IndexOf(':');
						if (num > 0)
						{
							text4 = text4.Substring(num + 1).Trim();
							this.item[InternalSchema.NamedContentType] = text4;
						}
						attachment.ContentType = text3;
					}
					else if (InboundMimeConverter.MimeStringEquals(text3, "multipart/appledouble"))
					{
						flag2 = true;
					}
					else
					{
						attachment.ContentType = text3;
					}
					if (InboundMimeConverter.MimeStringEquals(contentTypeHeader.MediaType, "text"))
					{
						string headerParameter = MimeHelpers.GetHeaderParameter(contentTypeHeader, "charset", this.conversionOptions);
						if (headerParameter != null)
						{
							attachment[InternalSchema.TextAttachmentCharset] = headerParameter;
						}
					}
				}
				string headerParameter2 = MimeHelpers.GetHeaderParameter(contentTypeHeader, "filename", this.conversionOptions);
				if (headerParameter2 == null)
				{
					headerParameter2 = MimeHelpers.GetHeaderParameter(contentTypeHeader, "name", this.conversionOptions);
				}
				if (!string.IsNullOrEmpty(headerParameter2))
				{
					if (!flag)
					{
						attachment.FileName = headerParameter2;
						attachment[AttachmentSchema.DisplayName] = headerParameter2;
						flag = true;
					}
					if (text == null)
					{
						text = attachment.FileName;
					}
				}
			}
			if (contentDispositionHeader != null)
			{
				this.PromoteDateParameter(attachment, contentDispositionHeader, "creation-date", InternalSchema.CreationTime);
				this.PromoteDateParameter(attachment, contentDispositionHeader, "modification-date", InternalSchema.LastModifiedTime);
				this.PromoteDateParameter(attachment, contentDispositionHeader, "read-date", InternalSchema.OriginalMimeReadTime);
				if (this.IsStreamToStreamConversion)
				{
					string headerParameter3 = MimeHelpers.GetHeaderParameter(contentDispositionHeader, "size", this.conversionOptions);
					int num2;
					if (headerParameter3 != null && int.TryParse(headerParameter3, out num2))
					{
						attachment[InternalSchema.Size] = num2;
					}
				}
				string headerParameter4 = MimeHelpers.GetHeaderParameter(contentDispositionHeader, "filename", this.conversionOptions);
				if (!string.IsNullOrEmpty(headerParameter4))
				{
					if (!flag)
					{
						attachment.FileName = headerParameter4;
						attachment[AttachmentSchema.DisplayName] = headerParameter4;
						flag = true;
					}
					if (text == null)
					{
						text = attachment.FileName;
					}
				}
			}
			string headerValue = MimeHelpers.GetHeaderValue(part.Headers, HeaderId.ContentDescription, this.ConversionOptions);
			if (!string.IsNullOrEmpty(headerValue))
			{
				attachment[AttachmentSchema.DisplayName] = headerValue;
				if (!flag)
				{
					attachment.FileName = headerValue;
					flag = true;
				}
				if (text == null)
				{
					text = attachment.FileName;
				}
			}
			if (!flag && !string.IsNullOrEmpty(defaultFilename))
			{
				attachment.FileName = defaultFilename;
			}
			if (text != text2 && !InboundMimeConverter.MimeStringEquals(text, text2))
			{
				StorageGlobals.ContextTraceError<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::PromoteAttachmentProperties -Edge OM filename '{0}' does not match filename from headers '{1}'", text2, text);
			}
			bool flag3 = false;
			if (flag)
			{
				string fileExtension = null;
				string text5 = null;
				Attachment.TryFindFileExtension(attachment.FileName, out fileExtension, out text5);
				flag3 = DefaultHtmlCallbacks.IsInlineImage(text3, fileExtension);
			}
			string headerValue2 = MimeHelpers.GetHeaderValue(part.Headers, HeaderId.ContentId, this.ConversionOptions);
			if (headerValue2 != null)
			{
				string value = ConvertUtils.ExtractMimeContentId(headerValue2);
				if (!string.IsNullOrWhiteSpace(value))
				{
					attachment[InternalSchema.AttachContentId] = value;
					if (isMhtml && flag3)
					{
						attachment.IsInline = true;
						if (flag && this.conversionOptions.TreatInlineDispositionAsAttachment)
						{
							attachment.PropertyBag.SetOrDeleteProperty(InternalSchema.AttachCalendarHidden, false);
						}
					}
				}
			}
			string headerValue3 = MimeHelpers.GetHeaderValue(part.Headers, HeaderId.ContentLocation, this.ConversionOptions);
			if (headerValue3 != null && !string.IsNullOrWhiteSpace(headerValue3))
			{
				attachment[InternalSchema.AttachContentLocation] = headerValue3;
				if (isMhtml && flag3)
				{
					attachment.IsInline = true;
					if (flag && this.conversionOptions.TreatInlineDispositionAsAttachment)
					{
						attachment.PropertyBag.SetOrDeleteProperty(InternalSchema.AttachCalendarHidden, false);
					}
				}
			}
			string headerValue4 = MimeHelpers.GetHeaderValue(part.Headers, HeaderId.ContentBase, this.ConversionOptions);
			if (headerValue4 != null)
			{
				attachment[InternalSchema.AttachContentBase] = headerValue4;
			}
			if (flag2)
			{
				MimePart part2 = null;
				MimePart part3 = null;
				this.GetAppleDoubleParts(part, out part2, out part3);
				this.PromoteAttachmentProperties(attachment, null, part3, defaultFilename, isMhtml);
				this.PromoteAttachmentProperties(attachment, null, part2, defaultFilename, isMhtml);
				if (attachment.ContentType != null)
				{
					attachment[InternalSchema.AttachmentMacContentType] = attachment.ContentType;
				}
				attachment.ContentType = "multipart/appledouble";
			}
		}

		private void PromoteDateParameter(Attachment attachment, ComplexHeader header, string paramName, PropertyDefinition prop)
		{
			string headerParameter = MimeHelpers.GetHeaderParameter(header, paramName, this.conversionOptions);
			if (headerParameter != null)
			{
				object obj = InboundMimeHeadersParser.ToDateTime(headerParameter);
				if (obj != null)
				{
					attachment[prop] = obj;
				}
			}
		}

		private bool PromoteTnef(MimePart tnefPart, int primaryCodepage, bool isSummaryTnef, out bool bodyFound)
		{
			bodyFound = false;
			ConversionLimitsTracker.State trackerSavedState = this.limitsTracker.SaveState();
			InboundAddressCache addressCache = new InboundAddressCache(this.conversionOptions, this.limitsTracker, this.MessageLevel);
			using (InboundMessageWriter writer = new InboundMessageWriter(this.item.CoreItem, this.ConversionOptions, addressCache, this.limitsTracker, this.MessageLevel))
			{
				Stream contentStream = null;
				InboundTnefConverter tnefconverter = new InboundTnefConverter(writer);
				tnefconverter.IsReplicationMessage = this.IsReplicationMessage;
				tnefconverter.ResolveParticipantsOnAttachments = this.IsReplicationMessage;
				try
				{
					bool internalBodyFound = false;
					ConvertUtils.CallCts(ExTraceGlobals.CcInboundTnefTracer, "InboundMimeConverter::PromoteTnef", ServerStrings.ConversionCorruptContent, delegate
					{
						if (!tnefPart.TryGetContentReadStream(out contentStream))
						{
							contentStream = tnefPart.GetRawContentReadStream();
						}
						tnefconverter.ConvertToItem(contentStream, primaryCodepage, isSummaryTnef);
						writer.Commit();
						internalBodyFound = (tnefconverter.PromotedBodyProperty != null);
						this.tnefAddressCache = addressCache;
					});
					this.limitsTracker.RollbackRecipients(this.tnefAddressCache.Recipients.Count);
					bodyFound = internalBodyFound;
				}
				catch (ConversionFailedException ex)
				{
					StorageGlobals.ContextTraceDebug<ConversionFailedException>(ExTraceGlobals.CcInboundTnefTracer, "InboundMimeConverter::PromoteTnef: got ConversionFailedException, {0}.", ex);
					this.UndoTnef(tnefconverter, trackerSavedState, isSummaryTnef, ex);
					return false;
				}
				finally
				{
					if (contentStream != null)
					{
						contentStream.Dispose();
						contentStream = null;
					}
				}
			}
			return true;
		}

		private void GenerateMdnBody()
		{
			MessageItem messageItem = this.Item as MessageItem;
			if (messageItem == null)
			{
				return;
			}
			MemoryStream memoryStream2;
			MemoryStream memoryStream = memoryStream2 = new MemoryStream(4096);
			try
			{
				CultureInfo cultureInfo;
				Charset sourceCharset;
				ReportMessage.GenerateReportBody(messageItem, memoryStream, out cultureInfo, out sourceCharset);
				memoryStream.Position = 0L;
				BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextHtml, sourceCharset);
				Stream stream;
				Stream writeStream = stream = this.item.CoreItem.Body.OpenWriteStream(configuration);
				try
				{
					Util.StreamHandler.CopyStreamData(memoryStream, writeStream);
				}
				finally
				{
					if (stream != null)
					{
						((IDisposable)stream).Dispose();
					}
				}
			}
			finally
			{
				if (memoryStream2 != null)
				{
					((IDisposable)memoryStream2).Dispose();
				}
			}
		}

		private void UndoTnef(InboundTnefConverter tnefConverter, ConversionLimitsTracker.State trackerSavedState, bool isSummaryTnef, Exception tnefException)
		{
			tnefConverter.Undo();
			this.limitsTracker.RestoreState(trackerSavedState);
			if (isSummaryTnef)
			{
				StorageGlobals.ContextTraceError<Exception>(ExTraceGlobals.CcInboundGenericTracer, "InboundMimeConverter::UndoTnef: summary tnef, {0}.", tnefException);
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, tnefException);
			}
			this.PromoteTnefAttachment(this.EmailMessage.TnefPart);
		}

		private void PromoteTnefAttachment(MimePart tnefPart)
		{
			using (StreamAttachment streamAttachment = (StreamAttachment)this.item.AttachmentCollection.Create(AttachmentType.Stream))
			{
				this.PromoteAttachmentProperties(streamAttachment, null, tnefPart, null, false);
				Stream stream = null;
				Stream stream2 = null;
				try
				{
					stream = streamAttachment.GetContentStream(PropertyOpenMode.Create);
					if (!tnefPart.TryGetContentReadStream(out stream2))
					{
						stream2 = tnefPart.GetRawContentReadStream();
					}
					Util.StreamHandler.CopyStreamData(stream2, stream);
				}
				finally
				{
					if (stream != null)
					{
						stream.Dispose();
						stream = null;
					}
					if (stream2 != null)
					{
						stream2.Dispose();
						stream2 = null;
					}
				}
				streamAttachment.Save();
				this.skipAttachment = tnefPart;
			}
		}

		private void PromoteLegacyTnef(out bool tnefBodyFound)
		{
			MimePart mimePart = null;
			bool flag = false;
			if (this.EmailMessage.TnefPart != null)
			{
				Charset tnefTextCharset = EmailMessageHelpers.GetTnefTextCharset(this.EmailMessage);
				if (this.PromoteTnef(this.EmailMessage.TnefPart, tnefTextCharset.CodePage, false, out flag))
				{
					mimePart = this.EmailMessage.TnefPart;
				}
			}
			if (mimePart != null)
			{
				MimePromotionFlags promotionFlags = flag ? MimePromotionFlags.SkipMessageBody : MimePromotionFlags.Default;
				this.skipAttachment = mimePart;
				this.PromoteIpmNote(promotionFlags);
			}
			else
			{
				this.PromoteIpmNote(MimePromotionFlags.Default);
				this.SaveSkeletonData(false);
			}
			tnefBodyFound = flag;
		}

		private void PromoteSmimeMessage(InboundMimeConverter.MimeMessageClass messageClass)
		{
			MimePart messageRoot = this.MessageRoot;
			using (StorageGlobals.SetTraceContext(new InboundMimeConverter.MimePartTraceContext(0, messageRoot)))
			{
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Starting MIME part (InboundMimeConverter.PromoteSmimeMessage)");
				this.HeadersParser.PromoteMessageHeaders();
				using (Attachment attachment = this.item.AttachmentCollection.Create(AttachmentType.Stream))
				{
					this.PromoteAttachmentProperties(attachment, null, messageRoot, "smime.p7m", false);
					if (messageRoot.IsMultipart)
					{
						this.PromoteMessageBody();
						using (Stream contentStream = ((StreamAttachment)attachment).GetContentStream(PropertyOpenMode.Create))
						{
							InboundMimeConverter.SMimeMultipartOutputFilter filter = new InboundMimeConverter.SMimeMultipartOutputFilter();
							messageRoot.WriteTo(contentStream, null, filter);
							goto IL_D5;
						}
					}
					using (Stream contentStream2 = ((StreamAttachment)attachment).GetContentStream(PropertyOpenMode.Create))
					{
						using (Stream contentReadStream = messageRoot.GetContentReadStream())
						{
							Util.StreamHandler.CopyStreamData(contentReadStream, contentStream2, null, 0, 65536);
						}
					}
					IL_D5:
					attachment.Save();
				}
				StorageGlobals.ContextTracePfd(ExTraceGlobals.CcPFDTracer, "Finishing MIME part (InboundMimeConverter.PromoteSmimeMessage)");
			}
		}

		private static bool IsSkippingHeaders(MimePromotionFlags promotionFlags)
		{
			return MimePromotionFlags.SkipMessageHeaders == (promotionFlags & MimePromotionFlags.SkipMessageHeaders);
		}

		private static bool IsSkippingBody(MimePromotionFlags promotionFlags)
		{
			return MimePromotionFlags.SkipMessageBody == (promotionFlags & MimePromotionFlags.SkipMessageBody);
		}

		private static bool IsSkippingAllAttachments(MimePromotionFlags promotionFlags)
		{
			return MimePromotionFlags.SkipAllAttachments == (promotionFlags & MimePromotionFlags.SkipAllAttachments);
		}

		private static bool IsSkippingRegularAttachments(MimePromotionFlags promotionFlags)
		{
			return MimePromotionFlags.SkipRegularAttachments == (promotionFlags & MimePromotionFlags.SkipRegularAttachments);
		}

		private static bool IsSkippingInlineAttachments(MimePromotionFlags promotionFlags)
		{
			return MimePromotionFlags.SkipInlineAttachments == (promotionFlags & MimePromotionFlags.SkipInlineAttachments);
		}

		private static bool MimeStringEquals(string string1, string string2)
		{
			return ConvertUtils.MimeStringEquals(string1, string2);
		}

		private static Dictionary<string, string> ReadMimeHeaders(Stream mimeStream, bool fallbackToRaw, MimeLimits mimeLimits)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			DecodingOptions @default = DecodingOptions.Default;
			if (fallbackToRaw)
			{
				MimeInternalHelpers.SetDecodingOptionsDecodingFlags(ref @default, @default.DecodingFlags | DecodingFlags.FallbackToRaw);
			}
			using (Stream stream = new StreamWrapper(mimeStream, false))
			{
				using (MimeReader mimeReader = new MimeReader(stream, true, @default, mimeLimits))
				{
					long position = mimeStream.Position;
					if (!mimeReader.ReadNextPart())
					{
						return null;
					}
					MimeHeaderReader headerReader = mimeReader.HeaderReader;
					while (headerReader.ReadNextHeader())
					{
						dictionary[headerReader.Name] = headerReader.Value;
					}
					if (mimeReader.StreamOffset == 0L)
					{
						if (mimeStream.Position == mimeStream.Length)
						{
							return null;
						}
						StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::ReadMimeHeaders: error in MimeHeaderReader.");
						throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
					}
					else
					{
						mimeStream.Position = position + mimeReader.StreamOffset;
					}
				}
			}
			return dictionary;
		}

		private void GetAppleDoubleParts(MimePart parentPart, out MimePart dataPart, out MimePart resourcePart)
		{
			MimeNode mimeNode = parentPart.FirstChild;
			if (mimeNode == null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetAppleDoubleParts, firstChild = null");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
			}
			uint num = 0U;
			dataPart = null;
			resourcePart = null;
			while (mimeNode != null)
			{
				if (!(mimeNode is MimePart))
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetAppleDoubleParts, subPart is not MimePart");
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
				}
				MimePart mimePart = mimeNode as MimePart;
				if (mimePart.IsMultipart)
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetAppleDoubleParts, subPart is multipart");
					throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
				}
				string headerValue = MimeHelpers.GetHeaderValue(mimePart.Headers, HeaderId.ContentType, this.ConversionOptions);
				if (InboundMimeConverter.MimeStringEquals(headerValue, "application/applefile"))
				{
					resourcePart = mimePart;
				}
				else
				{
					dataPart = mimePart;
				}
				mimeNode = mimeNode.NextSibling;
				num += 1U;
				if (num == 2U)
				{
					break;
				}
			}
			if (num != 2U || mimeNode != null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetAppleDoubleParts, wrong number of parts");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
			}
			if (resourcePart == null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetAppleDoubleParts, resource part not found");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
			}
			if (dataPart == null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeConverter::GetAppleDoubleParts, data part not found");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, null);
			}
		}

		internal ConverterFlags ConverterFlags
		{
			get
			{
				return this.converterFlags;
			}
		}

		internal MimePart MessageRoot
		{
			get
			{
				return this.EmailMessage.RootPart;
			}
		}

		internal EmailMessage EmailMessage
		{
			get
			{
				return this.emailMessage;
			}
		}

		public bool IsStreamToStreamConversion
		{
			get
			{
				return ConverterFlags.IsStreamToStreamConversion == (this.converterFlags & ConverterFlags.IsStreamToStreamConversion);
			}
		}

		private bool IsReplicationMessage
		{
			get
			{
				return ObjectClass.IsOfClass(this.emailMessage.MapiMessageClass, "IPM.Replication") && this.MessageLevel == MimeMessageLevel.TopLevelMessage && this.conversionOptions.IsSenderTrusted;
			}
		}

		public MimeMessageLevel MessageLevel
		{
			get
			{
				if (ConverterFlags.IsEmbeddedMessage == (this.converterFlags & ConverterFlags.IsEmbeddedMessage))
				{
					return MimeMessageLevel.AttachedMessage;
				}
				return MimeMessageLevel.TopLevelMessage;
			}
		}

		private InboundMimeHeadersParser HeadersParser
		{
			get
			{
				if (this.headersParser == null)
				{
					this.headersParser = new InboundMimeHeadersParser(this);
				}
				return this.headersParser;
			}
		}

		public ConversionLimits ConversionLimits
		{
			get
			{
				return this.conversionOptions.Limits;
			}
		}

		public InboundConversionOptions ConversionOptions
		{
			get
			{
				return this.conversionOptions;
			}
		}

		public Item Item
		{
			get
			{
				return this.item;
			}
		}

		public InboundAddressCache AddressCache
		{
			get
			{
				return this.mimeAddressCache;
			}
		}

		internal bool IsResolvingParticipants
		{
			get
			{
				return this.isResolvingParticipants;
			}
			set
			{
				this.isResolvingParticipants = value;
			}
		}

		private const int EarliestDsnVersionToPromoteBodyFrom = 12;

		internal static KeyValuePair<string, string>[] DsnActionToClass = new KeyValuePair<string, string>[]
		{
			new KeyValuePair<string, string>("delivered", "DR"),
			new KeyValuePair<string, string>("expanded", "Expanded.DR"),
			new KeyValuePair<string, string>("relayed", "Relayed.DR"),
			new KeyValuePair<string, string>("delayed", "Delayed.DR"),
			new KeyValuePair<string, string>("failed", "NDR")
		};

		internal static KeyValuePair<string, string>[] MdnActionToClass = new KeyValuePair<string, string>[]
		{
			new KeyValuePair<string, string>("displayed", "IPNRN"),
			new KeyValuePair<string, string>("dispatched", "IPNRN"),
			new KeyValuePair<string, string>("processed", "IPNRN"),
			new KeyValuePair<string, string>("deleted", "IPNNRN"),
			new KeyValuePair<string, string>("denied", "IPNNRN"),
			new KeyValuePair<string, string>("failed", "IPNNRN")
		};

		private static KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>[] dsnPromoteFromOriginalProperties = new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>[]
		{
			new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>(InternalSchema.From, InternalSchema.OriginalFrom),
			new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>(InternalSchema.Sender, InternalSchema.OriginalSender),
			new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>(InternalSchema.DisplayTo, InternalSchema.OriginalDisplayTo),
			new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>(InternalSchema.DisplayCc, InternalSchema.OriginalDisplayCc),
			new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>(InternalSchema.DisplayBcc, InternalSchema.OriginalDisplayBcc),
			new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>(InternalSchema.Subject, InternalSchema.OriginalSubject),
			new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>(InternalSchema.SentTime, InternalSchema.OriginalSentTime),
			new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>(InternalSchema.InternetMessageId, InternalSchema.OriginalMessageId),
			new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>(InternalSchema.ContentIdentifier, InternalSchema.ContentIdentifier),
			new KeyValuePair<StorePropertyDefinition, StorePropertyDefinition>(InternalSchema.ReportTag, InternalSchema.ReportTag)
		};

		private InboundConversionOptions conversionOptions;

		private Item item;

		private ConversionLimitsTracker limitsTracker;

		private EmailMessage emailMessage;

		private InboundMimeHeadersParser headersParser;

		private ConverterFlags converterFlags;

		private MimePart skipAttachment;

		private InboundAddressCache mimeAddressCache;

		private InboundAddressCache tnefAddressCache;

		private InboundMimeConverter.MimeMessageClass initialClass;

		private InboundMimeConverter.MimeBodyWrapper delayedBody;

		private uint? calendarPartCRC = null;

		private Dictionary<MimePart, string> promotedMimeParts = new Dictionary<MimePart, string>();

		private bool isResolvingParticipants;

		private delegate void ReportBodyPartPromoter(MessageItem item, MimePart part);

		private class SkeletonOutputFilter : MimeOutputFilter
		{
			internal SkeletonOutputFilter(Dictionary<MimePart, string> excludedParts, bool isStreamToStream, InboundConversionOptions options)
			{
				this.excludedParts = excludedParts;
				this.isStreamToStream = isStreamToStream;
				if (isStreamToStream)
				{
					this.options = null;
					return;
				}
				this.options = options;
			}

			public override bool FilterPart(MimePart part, Stream stream)
			{
				this.lastPartStarted = part;
				return false;
			}

			public override bool FilterHeaderList(HeaderList headerList, Stream stream)
			{
				string value;
				if (this.excludedParts.TryGetValue(this.lastPartStarted, out value))
				{
					string value2 = MimeHelpers.GetHeaderValue(headerList, HeaderId.ContentId, this.options);
					if (!string.IsNullOrWhiteSpace(value2))
					{
						value2 = ConvertUtils.ExtractMimeContentId(value2);
					}
					if (string.IsNullOrWhiteSpace(value2))
					{
						Header header = Header.Create("X-Exchange-Mime-Skeleton-Content-Id");
						header.Value = value;
						header.WriteTo(stream);
					}
				}
				return false;
			}

			public override bool FilterHeader(Header header, Stream stream)
			{
				if (!this.isStreamToStream && (this.lastPartStarted == null || this.lastPartStarted.Parent == null))
				{
					string name = header.Name;
					return MimeConstants.IsInReservedHeaderNamespace(name) && (this.options.ApplyHeaderFirewall || !MimeConstants.IsReservedHeaderAllowedOnDelivery(name));
				}
				return false;
			}

			public override bool FilterPartBody(MimePart part, Stream stream)
			{
				return this.excludedParts.ContainsKey(part);
			}

			private MimePart lastPartStarted;

			private Dictionary<MimePart, string> excludedParts;

			private bool isStreamToStream;

			private InboundConversionOptions options;
		}

		private class SMimeMultipartOutputFilter : MimeOutputFilter
		{
			public override bool FilterPart(MimePart part, Stream stream)
			{
				if (this.seenContentTypeHeader)
				{
					this.shouldFilterOutHeaders = false;
				}
				return false;
			}

			public override bool FilterHeader(Header header, Stream stream)
			{
				if (!this.shouldFilterOutHeaders)
				{
					return false;
				}
				if (header.HeaderId == HeaderId.ContentType)
				{
					this.seenContentTypeHeader = true;
					return false;
				}
				return true;
			}

			private bool shouldFilterOutHeaders = true;

			private bool seenContentTypeHeader;
		}

		private enum MimeMessageClass
		{
			ClassIpmNote,
			ClassIpmNoteSummaryTnef,
			ClassIpmNoteLegacyTnef,
			ClassIpmNoteSmime,
			ClassIpmNoteSmimeMultipartSigned,
			ClassCalendar,
			ClassDSN,
			ClassMDN,
			OutlookRecall,
			Journal
		}

		private class MimePartTraceContext
		{
			public MimePartTraceContext(int order, MimePart part)
			{
				this.order = order;
				this.part = part;
			}

			public override string ToString()
			{
				string arg = "unknown";
				string arg2 = "unknown";
				if (this.part.Headers != null)
				{
					try
					{
						Header header = this.part.Headers.FindFirst(HeaderId.MessageId);
						if (header != null)
						{
							try
							{
								arg = header.Value;
							}
							catch (ExchangeDataException)
							{
							}
						}
						Header header2 = this.part.Headers.FindFirst(HeaderId.ContentType);
						if (header2 != null)
						{
							try
							{
								arg2 = header2.Value;
							}
							catch (ExchangeDataException)
							{
							}
						}
					}
					catch (ExchangeDataException)
					{
					}
				}
				return string.Format("MimePart #{0}, message-id: {1}, content-type: {2}.", this.order, arg, arg2);
			}

			private MimePart part;

			private int order;
		}

		private class MimeBodyWrapper
		{
			public MimeBodyWrapper(Item item, Body body, InboundConversionOptions conversionOptions, InboundMimeConverter.MimeBodyWrapper.RtfOptions wrapperOptions)
			{
				this.item = item;
				this.body = body;
				this.conversionOptions = conversionOptions;
				this.wrapperOptions = wrapperOptions;
			}

			public bool IsCharsetPresentInMime
			{
				get
				{
					if (this.charsetPresentInMime == null)
					{
						this.charsetPresentInMime = new bool?(false);
						ComplexHeader complexHeader = this.body.MimePart.Headers.FindFirst(HeaderId.ContentType) as ComplexHeader;
						if (complexHeader != null)
						{
							string headerParameter = MimeHelpers.GetHeaderParameter(complexHeader, "charset", this.conversionOptions);
							Charset charset;
							if (headerParameter != null && Charset.TryGetCharset(headerParameter, out charset))
							{
								this.charsetPresentInMime = new bool?(true);
							}
						}
					}
					return this.charsetPresentInMime.Value;
				}
			}

			public bool IsEmpty
			{
				get
				{
					return this.body == null || this.body.BodyFormat == Microsoft.Exchange.Data.Transport.Email.BodyFormat.None || InboundMimeConverter.MimeBodyWrapper.HasEmptyBody(this.body);
				}
			}

			public Charset GetCharset()
			{
				if (this.charset == null)
				{
					this.charset = MimeHelpers.ChooseCharset(this.body.CharsetName, null, this.ConversionOptions);
				}
				return this.charset;
			}

			public Stream OpenContentStream(out ConversionCallbackBase conversionCallback)
			{
				Stream stream = null;
				conversionCallback = null;
				bool flag = false;
				try
				{
					stream = (this.ConvertToRtf ? this.CreateRtfConverter(out conversionCallback) : this.body.GetContentReadStream());
					flag = true;
				}
				finally
				{
					if (!flag && stream != null)
					{
						stream.Dispose();
						stream = null;
					}
				}
				return stream;
			}

			public BodyFormat BodyFormat
			{
				get
				{
					if (this.ConvertToRtf)
					{
						return BodyFormat.ApplicationRtf;
					}
					return this.RawBodyFormat;
				}
			}

			public string ContentId
			{
				get
				{
					return MimeHelpers.GetHeaderValue(this.body.MimePart.Headers.FindFirst(HeaderId.ContentId), this.ConversionOptions);
				}
			}

			public string ContentBase
			{
				get
				{
					return MimeHelpers.GetHeaderValue(this.body.MimePart.Headers.FindFirst(HeaderId.ContentBase), this.ConversionOptions);
				}
			}

			public string ContentLanguage
			{
				get
				{
					return MimeHelpers.GetHeaderValue(this.body.MimePart.Headers.FindFirst(HeaderId.ContentLanguage), this.ConversionOptions);
				}
			}

			public string ContentLocation
			{
				get
				{
					return MimeHelpers.GetHeaderValue(this.body.MimePart.Headers.FindFirst(HeaderId.ContentLocation), this.ConversionOptions);
				}
			}

			public InboundConversionOptions ConversionOptions
			{
				get
				{
					return this.conversionOptions;
				}
			}

			private static bool HasEmptyBody(Body body)
			{
				long num = -1L;
				using (Stream contentReadStream = body.GetContentReadStream())
				{
					if (contentReadStream != null)
					{
						num = (long)contentReadStream.ReadByte();
					}
				}
				return num == -1L;
			}

			private Stream CreateRtfConverter(out ConversionCallbackBase conversionCallback)
			{
				Stream stream = this.body.GetContentReadStreamOrNull();
				Stream stream2 = null;
				if (stream == null)
				{
					stream2 = new MemoryStream();
					stream = stream2;
				}
				conversionCallback = null;
				Charset charset = this.GetCharset();
				Stream stream3 = null;
				switch (this.RawBodyFormat)
				{
				case BodyFormat.TextPlain:
					stream3 = new ConverterStream(stream, new TextToRtf
					{
						InputEncoding = charset.GetEncoding()
					}, ConverterStreamAccess.Read);
					break;
				case BodyFormat.TextHtml:
				{
					HtmlToRtf htmlToRtf = new HtmlToRtf();
					if (this.item != null)
					{
						DefaultRtfCallbacks defaultRtfCallbacks = new DefaultRtfCallbacks(this.item.CoreItem, false);
						conversionCallback = defaultRtfCallbacks;
						TextConvertersInternalHelpers.SetImageRenderingCallback(htmlToRtf, new ImageRenderingCallback(defaultRtfCallbacks.ProcessImage));
					}
					if (this.FixSquigglieInHtml)
					{
						IcalSquigglieFixHtmlReader sourceReader = new IcalSquigglieFixHtmlReader(stream, charset, !this.IsCharsetPresentInMime);
						htmlToRtf.DetectEncodingFromMetaTag = false;
						stream3 = new ConverterStream(sourceReader, htmlToRtf);
					}
					else
					{
						if (this.IsCharsetPresentInMime)
						{
							htmlToRtf.DetectEncodingFromMetaTag = false;
						}
						htmlToRtf.InputEncoding = charset.GetEncoding();
						stream3 = new ConverterStream(stream, htmlToRtf, ConverterStreamAccess.Read);
					}
					break;
				}
				default:
					if (stream2 != null)
					{
						stream2.Dispose();
					}
					break;
				}
				RtfToRtfCompressed converter = new RtfToRtfCompressed();
				return new ConverterStream(stream3, converter, ConverterStreamAccess.Read);
			}

			private BodyFormat RawBodyFormat
			{
				get
				{
					switch (this.body.BodyFormat)
					{
					case Microsoft.Exchange.Data.Transport.Email.BodyFormat.Text:
						return BodyFormat.TextPlain;
					case Microsoft.Exchange.Data.Transport.Email.BodyFormat.Html:
						return BodyFormat.TextHtml;
					}
					return BodyFormat.TextPlain;
				}
			}

			private bool ConvertToRtf
			{
				get
				{
					return (this.wrapperOptions & InboundMimeConverter.MimeBodyWrapper.RtfOptions.ConvertToRtf) == InboundMimeConverter.MimeBodyWrapper.RtfOptions.ConvertToRtf;
				}
			}

			private bool FixSquigglieInHtml
			{
				get
				{
					return (this.wrapperOptions & InboundMimeConverter.MimeBodyWrapper.RtfOptions.ConvertToRtfAndFixSquigglieInHtml) == InboundMimeConverter.MimeBodyWrapper.RtfOptions.ConvertToRtfAndFixSquigglieInHtml;
				}
			}

			private Item item;

			private InboundConversionOptions conversionOptions;

			private Charset charset;

			private bool? charsetPresentInMime;

			private InboundMimeConverter.MimeBodyWrapper.RtfOptions wrapperOptions;

			private Body body;

			[Flags]
			public enum RtfOptions
			{
				None = 0,
				ConvertToRtf = 1,
				ConvertToRtfAndFixSquigglieInHtml = 3
			}
		}
	}
}
