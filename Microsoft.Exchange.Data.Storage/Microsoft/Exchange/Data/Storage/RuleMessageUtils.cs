using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage.MailboxRules;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RuleMessageUtils
	{
		public static MessageItem CreateReply(MessageItem originalItem, MessageItem template, CultureInfo culture, string xLoop, IRuleEvaluationContext context)
		{
			Util.ThrowOnNullArgument(originalItem, "originalItem");
			Util.ThrowOnNullArgument(template, "template");
			ExTraceGlobals.StorageTracer.Information(0L, "RuleMessageUtils::CreateReply.");
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				messageItem = context.CreateMessageItem(InternalSchema.ContentConversionProperties);
				RuleReplyCreation ruleReplyCreation = new RuleReplyCreation(originalItem, messageItem, template, new ReplyForwardConfiguration(culture)
				{
					XLoop = xLoop
				}, context.StoreSession is PublicFolderSession);
				ruleReplyCreation.PopulateProperties();
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		public static MessageItem CreateStockReply(MessageItem originalItem, string body, CultureInfo culture, string xLoop)
		{
			Util.ThrowOnNullArgument(originalItem, "originalItem");
			Util.ThrowOnNullArgument(body, "body");
			ExTraceGlobals.StorageTracer.Information(0L, "RuleMessageUtils::CreateStockReply.");
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				messageItem = MessageItem.CreateInMemory(InternalSchema.ContentConversionProperties);
				using (MessageItem messageItem2 = MessageItem.CreateInMemory(InternalSchema.ContentConversionProperties))
				{
					messageItem2[InternalSchema.ItemClass] = "IPM.Note";
					using (TextWriter textWriter = messageItem2.Body.OpenTextWriter(BodyFormat.TextPlain))
					{
						textWriter.Write(body);
						textWriter.Flush();
					}
					messageItem.Save(SaveMode.NoConflictResolution);
					messageItem.Load(InternalSchema.ContentConversionProperties);
					RuleReplyCreation ruleReplyCreation = new RuleReplyCreation(originalItem, messageItem, messageItem2, new ReplyForwardConfiguration(culture)
					{
						XLoop = xLoop
					});
					ruleReplyCreation.PopulateProperties();
					flag = true;
					result = messageItem;
				}
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		public static MessageItem CreateOofReply(MessageItem originalItem, MessageItem template, CultureInfo culture, InboundConversionOptions conversionOptions, string xLoop)
		{
			Util.ThrowOnNullArgument(originalItem, "originalItem");
			Util.ThrowOnNullArgument(template, "template");
			Util.ThrowOnNullArgument(conversionOptions, "conversionOptions");
			ExTraceGlobals.StorageTracer.Information(0L, "RuleMessageUtils::CreateOofReply.");
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				messageItem = MessageItem.CreateInMemory(InternalSchema.ContentConversionProperties);
				OofReplyCreation oofReplyCreation = new OofReplyCreation(originalItem, messageItem, template, new ReplyForwardConfiguration(culture)
				{
					XLoop = xLoop,
					ConversionOptionsForSmime = conversionOptions
				});
				oofReplyCreation.PopulateProperties();
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		public static MessageItem CreateForward(MessageItem originalItem, bool asAttachment, CultureInfo culture, string imceaDomain, string xLoop, ExTimeZone timeZone, IRuleEvaluationContext context)
		{
			Util.ThrowOnNullArgument(originalItem, "originalItem");
			Util.ThrowOnNullArgument(culture, "culture");
			Util.ThrowOnNullOrEmptyArgument(imceaDomain, "imceaDomain");
			ExTraceGlobals.StorageTracer.Information(0L, "RuleMessageUtils::CreateForward.");
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				ForwardCreationFlags forwardCreationFlags = ForwardCreationFlags.None;
				string className = originalItem.ClassName;
				if (ObjectClass.IsMeetingMessage(className))
				{
					forwardCreationFlags |= ForwardCreationFlags.TreatAsMeetingMessage;
				}
				messageItem = context.CreateMessageItem(InternalSchema.ContentConversionProperties);
				messageItem[InternalSchema.ItemClass] = "IPM.Note";
				StoreSession storeSession = context.StoreSession ?? originalItem.Session;
				if (asAttachment)
				{
					ForwardAsAttachmentCreation forwardAsAttachmentCreation = new ForwardAsAttachmentCreation(originalItem, messageItem, new ReplyForwardConfiguration(forwardCreationFlags, culture)
					{
						XLoop = xLoop,
						TimeZone = timeZone,
						ConversionOptionsForSmime = new InboundConversionOptions(storeSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), imceaDomain)
					});
					forwardAsAttachmentCreation.PopulateProperties();
				}
				else
				{
					bool flag2 = ObjectClass.IsMeetingCancellation(className);
					bool flag3 = ObjectClass.IsMeetingRequest(className);
					bool isRestricted = originalItem.IsRestricted;
					if (flag2)
					{
						messageItem[InternalSchema.ItemClass] = "IPM.Schedule.Meeting.Canceled";
						messageItem[InternalSchema.IconIndex] = IconIndex.AppointmentMeetCancel;
					}
					else if (flag3)
					{
						messageItem[InternalSchema.ItemClass] = "IPM.Schedule.Meeting.Request";
						messageItem[InternalSchema.IsResponseRequested] = true;
						messageItem[InternalSchema.IsReplyRequested] = true;
					}
					else if (isRestricted)
					{
						messageItem[StoreObjectSchema.ContentClass] = "rpmsg.message";
						messageItem.IconIndex = IconIndex.MailIrm;
					}
					BodyFormat format = originalItem.Body.Format;
					ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(format, forwardCreationFlags, culture);
					replyForwardConfiguration.XLoop = xLoop;
					replyForwardConfiguration.TimeZone = timeZone;
					replyForwardConfiguration.ConversionOptionsForSmime = new InboundConversionOptions(storeSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), imceaDomain);
					RuleMessageUtils.GenerateHeader(replyForwardConfiguration, originalItem);
					ForwardCreation forwardCreation = new ForwardCreation(originalItem, messageItem, replyForwardConfiguration);
					forwardCreation.PopulateProperties();
					if (flag2 || flag3)
					{
						AppointmentStateFlags appointmentStateFlags = messageItem.GetValueOrDefault<AppointmentStateFlags>(InternalSchema.AppointmentState);
						appointmentStateFlags |= (AppointmentStateFlags.Meeting | AppointmentStateFlags.Received);
						messageItem[InternalSchema.AppointmentState] = appointmentStateFlags;
						int num = messageItem.GetValueOrDefault<int>(InternalSchema.AppointmentAuxiliaryFlags, 0);
						num |= 4;
						messageItem[InternalSchema.AppointmentAuxiliaryFlags] = num;
						if (flag3)
						{
							List<BlobRecipient> list = BlobRecipientParser.ReadRecipients(originalItem, InternalSchema.UnsendableRecipients);
							list = MeetingRequest.FilterBlobRecipientList(list);
							list = MeetingRequest.MergeRecipientLists(originalItem.Recipients, list);
							list = MeetingRequest.FilterBlobRecipientList(list);
							BlobRecipientParser.WriteRecipients(messageItem, InternalSchema.UnsendableRecipients, list);
						}
					}
				}
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		public static MessageItem CreateRedirect(MessageItem originalItem, CultureInfo culture, string imceaDomain, string xLoop, IRuleEvaluationContext context)
		{
			Util.ThrowOnNullArgument(originalItem, "originalItem");
			ExTraceGlobals.StorageTracer.Information(0L, "RuleMessageUtils::CreateRedirect.");
			MessageItem messageItem = null;
			bool flag = false;
			MessageItem result;
			try
			{
				messageItem = context.CreateMessageItem(InternalSchema.ContentConversionProperties);
				Item.CopyItemContent(originalItem, messageItem);
				messageItem.Delete(InternalSchema.InternetMessageId);
				messageItem.Delete(InternalSchema.MimeSkeleton);
				messageItem.CharsetDetector.DetectionOptions = originalItem.CharsetDetector.DetectionOptions;
				messageItem.SaveFlags |= (originalItem.SaveFlags | PropertyBagSaveFlags.IgnoreMapiComputedErrors);
				ReplyForwardCommon.UpdateXLoop(originalItem, messageItem, xLoop);
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		public static MessageItem CreateSmsAlert(MessageItem originalItem, CultureInfo culture, string imceaDomain, string xLoop, ExTimeZone timeZone, IRuleEvaluationContext context)
		{
			Util.ThrowOnNullArgument(originalItem, "originalItem");
			ExTraceGlobals.StorageTracer.Information(0L, "RuleMessageUtils::CreateSmsAlert.");
			MessageItem messageItem = RuleMessageUtils.CreateForward(originalItem, true, culture, imceaDomain, xLoop, timeZone, context);
			if (messageItem != null)
			{
				messageItem[InternalSchema.ItemClass] = "IPM.Note.Mobile.SMS.Alert";
			}
			return messageItem;
		}

		public static MessageItem CreateDelegateForward(MessageItem originalItem, CultureInfo culture, string imceaDomain, string xLoop, IRuleEvaluationContext context)
		{
			Util.ThrowOnNullArgument(originalItem, "originalItem");
			ExTraceGlobals.StorageTracer.Information(0L, "RuleMessageUtils::CreateDelegateForward.");
			MessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MessageItem messageItem = RuleMessageUtils.CreateRedirect(originalItem, culture, imceaDomain, xLoop, context);
				disposeGuard.Add<MessageItem>(messageItem);
				for (int i = 0; i < RuleMessageUtils.rcvdRepresentingProps.Length; i++)
				{
					if (originalItem.GetValueOrDefault<object>(RuleMessageUtils.rcvdRepresentingProps[i]) == null)
					{
						object valueOrDefault = originalItem.GetValueOrDefault<object>(RuleMessageUtils.rcvdByProps[i]);
						if (valueOrDefault != null)
						{
							messageItem[RuleMessageUtils.rcvdRepresentingProps[i]] = valueOrDefault;
						}
					}
					messageItem.Delete(RuleMessageUtils.rcvdByProps[i]);
				}
				object valueOrDefault2 = originalItem.GetValueOrDefault<object>(InternalSchema.SentRepresentingEntryId);
				if (valueOrDefault2 != null)
				{
					messageItem[InternalSchema.ReadReceiptEntryId] = valueOrDefault2;
					messageItem[InternalSchema.ReportEntryId] = valueOrDefault2;
				}
				disposeGuard.Success();
				result = messageItem;
			}
			return result;
		}

		public static MessageItem CreateNotReadNotification(MessageItem originalItem)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			MessageItem messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties);
			messageItem.ClassName = ObjectClass.MakeReportClassName(originalItem.ClassName, "IPNNRN");
			messageItem.SafeSetProperty(InternalSchema.ReportTime, utcNow);
			Participant participant = originalItem.ReadReceiptAddressee;
			if (null == participant)
			{
				if (null != originalItem.Sender)
				{
					participant = originalItem.Sender;
				}
				else
				{
					participant = originalItem.From;
				}
			}
			messageItem.Recipients.Add(participant, RecipientItemType.To);
			foreach (KeyValuePair<PropertyDefinition, PropertyDefinition> keyValuePair in RuleMessageUtils.NrnPropertyMap)
			{
				messageItem.SafeSetProperty(keyValuePair.Key, originalItem.TryGetProperty(keyValuePair.Value));
			}
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(BodyFormat.TextHtml, originalItem.Body.RawCharset.Name);
			CultureInfo formatProvider;
			using (Stream stream = messageItem.Body.OpenWriteStream(configuration))
			{
				Charset charset;
				ReportMessage.GenerateReportBody(messageItem, stream, out formatProvider, out charset);
			}
			messageItem.SafeSetProperty(InternalSchema.Subject, ServerStrings.NotRead.ToString(formatProvider) + originalItem.TryGetProperty(InternalSchema.Subject));
			byte[] parentBytes = originalItem.TryGetProperty(InternalSchema.ConversationIndex) as byte[];
			messageItem.SafeSetProperty(InternalSchema.ConversationIndex, ConversationIndex.CreateFromParent(parentBytes).ToByteArray());
			messageItem.SafeSetProperty(InternalSchema.ConversationTopic, StoreObject.SafePropertyValue(originalItem.TryGetProperty(InternalSchema.ConversationTopic), typeof(string), StoreObject.SafePropertyValue(originalItem.TryGetProperty(InternalSchema.NormalizedSubjectInternal), typeof(string), null)));
			messageItem.SafeSetProperty(InternalSchema.IsReadReceiptRequested, false);
			messageItem.SafeSetProperty(InternalSchema.IsDeliveryReceiptRequested, false);
			messageItem.SafeSetProperty(InternalSchema.IsNonDeliveryReceiptRequested, false);
			messageItem.SafeSetProperty(InternalSchema.NonReceiptReason, 0);
			messageItem.SafeSetProperty(InternalSchema.DiscardReason, 1);
			messageItem.SafeSetProperty(InternalSchema.OriginalDeliveryTime, utcNow);
			return messageItem;
		}

		public static DeferredAction CreateDAM(MailboxSession session, StoreObjectId ruleFolderId, string providerName)
		{
			return DeferredAction.Create(session, ruleFolderId, providerName);
		}

		public static DeferredError CreateDAE(MailboxSession session, StoreObjectId ruleFolderId, string providerName, long ruleId, RuleAction.Type actionType, int actionNumber, DeferredError.RuleError ruleError)
		{
			return DeferredError.Create(session, ruleFolderId, providerName, ruleId, actionType, actionNumber, ruleError);
		}

		private static void GenerateHeader(ReplyForwardConfiguration replyForwardConfiguration, Item originalItem)
		{
			ReplyForwardHeader.CreateForwardReplyHeader(replyForwardConfiguration, originalItem, RuleMessageUtils.headerOptions);
		}

		private static ForwardReplyHeaderOptions headerOptions = new ForwardReplyHeaderOptions();

		private static readonly Dictionary<PropertyDefinition, PropertyDefinition> NrnPropertyMap = new Dictionary<PropertyDefinition, PropertyDefinition>
		{
			{
				InternalSchema.MapiImportance,
				InternalSchema.MapiImportance
			},
			{
				InternalSchema.MapiPriority,
				InternalSchema.MapiPriority
			},
			{
				InternalSchema.ReportTag,
				InternalSchema.ReportTag
			},
			{
				InternalSchema.MapiInternetCpid,
				InternalSchema.MapiInternetCpid
			},
			{
				InternalSchema.Codepage,
				InternalSchema.Codepage
			},
			{
				InternalSchema.ConversationKey,
				InternalSchema.ConversationKey
			},
			{
				InternalSchema.ConversationId,
				InternalSchema.ConversationId
			},
			{
				InternalSchema.OriginalMessageId,
				InternalSchema.InternetMessageId
			},
			{
				InternalSchema.OriginalSenderDisplayName,
				InternalSchema.SenderDisplayName
			},
			{
				InternalSchema.OriginalSenderEntryId,
				InternalSchema.SenderEntryId
			},
			{
				InternalSchema.OriginalSenderAddressType,
				InternalSchema.SenderAddressType
			},
			{
				InternalSchema.OriginalSenderEmailAddress,
				InternalSchema.SenderEmailAddress
			},
			{
				InternalSchema.OriginalSentRepresentingDisplayName,
				InternalSchema.SentRepresentingDisplayName
			},
			{
				InternalSchema.OriginalSentRepresentingEntryId,
				InternalSchema.SentRepresentingEntryId
			},
			{
				InternalSchema.OriginalSentRepresentingAddressType,
				InternalSchema.SentRepresentingType
			},
			{
				InternalSchema.OriginalSentRepresentingEmailAddress,
				InternalSchema.SentRepresentingEmailAddress
			},
			{
				InternalSchema.OriginalDisplayTo,
				InternalSchema.DisplayToInternal
			},
			{
				InternalSchema.OriginalDisplayCc,
				InternalSchema.DisplayCcInternal
			},
			{
				InternalSchema.OriginalDisplayBcc,
				InternalSchema.DisplayBccInternal
			},
			{
				InternalSchema.OriginallyIntendedRecipientName,
				InternalSchema.OriginallyIntendedRecipEntryId
			},
			{
				InternalSchema.OriginalSentTime,
				InternalSchema.SentTime
			},
			{
				InternalSchema.OriginalSubject,
				InternalSchema.MapiSubject
			},
			{
				InternalSchema.OriginalSearchKey,
				InternalSchema.SearchKey
			},
			{
				InternalSchema.ParentKey,
				InternalSchema.SearchKey
			}
		};

		private static StorePropertyDefinition[] sentRepresentingProps = new StorePropertyDefinition[]
		{
			InternalSchema.SentRepresentingEntryId,
			InternalSchema.SentRepresentingDisplayName,
			InternalSchema.SentRepresentingEmailAddress,
			InternalSchema.SentRepresentingSearchKey,
			InternalSchema.SentRepresentingType,
			InternalSchema.SentRepresentingSmtpAddress,
			InternalSchema.SentRepresentingFlags
		};

		private static StorePropertyDefinition[] rcvdRepresentingProps = new StorePropertyDefinition[]
		{
			InternalSchema.ReceivedRepresentingEntryId,
			InternalSchema.ReceivedRepresentingDisplayName,
			InternalSchema.ReceivedRepresentingEmailAddress,
			InternalSchema.ReceivedRepresentingSearchKey,
			InternalSchema.ReceivedRepresentingAddressType,
			InternalSchema.ReceivedRepresentingSmtpAddress
		};

		private static StorePropertyDefinition[] rcvdByProps = new StorePropertyDefinition[]
		{
			InternalSchema.ReceivedByEntryId,
			InternalSchema.ReceivedByName,
			InternalSchema.ReceivedByEmailAddress,
			InternalSchema.ReceivedBySearchKey,
			InternalSchema.ReceivedByAddrType,
			InternalSchema.ReceivedBySmtpAddress
		};
	}
}
