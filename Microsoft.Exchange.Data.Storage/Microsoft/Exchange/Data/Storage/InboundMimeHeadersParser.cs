using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class InboundMimeHeadersParser
	{
		private static Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule> CreateHeaderRulesTable()
		{
			Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule> dictionary = new Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule>();
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.From, new InboundMimeHeadersParser.AddressHeaderRule(delegate(InboundMimeHeadersParser self, Header header, Participant from)
			{
				self.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.From] = from;
				if (!self.CheckIsHeaderPresent(HeaderId.Sender))
				{
					self.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.Sender] = from;
				}
			}, InboundMimeHeadersParser.AddressHeaderFlags.DeencapsulateIfSenderTrusted));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Sender, new InboundMimeHeadersParser.AddressHeaderRule(delegate(InboundMimeHeadersParser self, Header header, Participant sender)
			{
				self.AddressCache.Participants[ConversionItemParticipants.ParticipantIndex.Sender] = sender;
			}, InboundMimeHeadersParser.AddressHeaderFlags.DeencapsulateIfSenderTrusted));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.To, new InboundMimeHeadersParser.AddressHeaderRule(delegate(InboundMimeHeadersParser self, Header header, List<Participant> toRecipients)
			{
				self.AddressCache.AddRecipients(toRecipients, RecipientItemType.To);
			}));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Cc, new InboundMimeHeadersParser.AddressHeaderRule(delegate(InboundMimeHeadersParser self, Header header, List<Participant> ccRecipients)
			{
				self.AddressCache.AddRecipients(ccRecipients, RecipientItemType.Cc);
			}));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Bcc, new InboundMimeHeadersParser.AddressHeaderRule(delegate(InboundMimeHeadersParser self, Header header, List<Participant> bccRecipients)
			{
				self.AddressCache.AddRecipients(bccRecipients, RecipientItemType.Bcc);
			}));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.ReplyTo, new InboundMimeHeadersParser.AddressHeaderRule(delegate(InboundMimeHeadersParser self, Header header, List<Participant> replyToRecipients)
			{
				self.AddressCache.AddReplyTo(replyToRecipients);
			}));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.MessageId, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.InternetMessageId));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Date, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.SentTime, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToDateTime)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.DeferredDelivery, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.DeferredDeliveryTime, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToDateTime)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.References, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.InternetReferences, 65536));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Sensitivity, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.Sensitivity, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToSensitivity)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Importance, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.Importance, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToImportance)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Priority, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.Importance, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.PriorityToImportance), new InboundMimeHeadersParser.HeaderPriorityList(new HeaderId[]
			{
				HeaderId.Importance
			})));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.XMSMailPriority, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.Importance, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToImportance), new InboundMimeHeadersParser.HeaderPriorityList(new HeaderId[]
			{
				HeaderId.Importance,
				HeaderId.Priority
			})));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.XPriority, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.Importance, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.XPriorityToImportance), new InboundMimeHeadersParser.HeaderPriorityList(new HeaderId[]
			{
				HeaderId.Importance,
				HeaderId.Priority,
				HeaderId.XMSMailPriority
			})));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Subject, new InboundMimeHeadersParser.CustomRule(new InboundMimeHeadersParser.CustomRule.PromotionDelegate(InboundMimeHeadersParser.PromoteSubject)));
			InboundMimeHeadersParser.AddRule(dictionary, "Thread-Topic", new InboundMimeHeadersParser.CustomRule(new InboundMimeHeadersParser.CustomRule.PromotionDelegate(InboundMimeHeadersParser.PromoteThreadTopic)));
			InboundMimeHeadersParser.AddRule(dictionary, "Thread-Index", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ConversationIndex, delegate(InboundMimeHeadersParser self, Header header, string value)
			{
				object result;
				try
				{
					result = Convert.FromBase64String(value);
				}
				catch (FormatException)
				{
					StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser: failed to parse header value ({0}: {1})", header.Name, value);
					result = null;
				}
				return result;
			}));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.InReplyTo, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.InReplyTo));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.ReplyBy, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ReplyTime, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToDateTime)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.ContentLanguage, new InboundMimeHeadersParser.CustomRule(new InboundMimeHeadersParser.CustomRule.PromotionDelegate(InboundMimeHeadersParser.PromoteContentLanguage)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Keywords, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.Categories, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.KeywordsToCategories)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Expires, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ExpiryTime, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToDateTime)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.ExpiryDate, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ExpiryTime, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToDateTime)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.DispositionNotificationTo, new InboundMimeHeadersParser.AddressHeaderRule(new InboundMimeHeadersParser.AddressHeaderRule.PromoteSingleAddress(InboundMimeHeadersParser.PromoteDispositionNotificationTo), InboundMimeHeadersParser.AddressHeaderFlags.AlwaysDeencapsulate));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.ReturnReceiptTo, new InboundMimeHeadersParser.AddressHeaderRule(new InboundMimeHeadersParser.AddressHeaderRule.PromoteSingleAddress(InboundMimeHeadersParser.PromoteReturnReceiptTo), InboundMimeHeadersParser.AddressHeaderFlags.AlwaysDeencapsulate));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.Precedence, new InboundMimeHeadersParser.CustomRule(new InboundMimeHeadersParser.CustomRule.PromotionDelegate(InboundMimeHeadersParser.PromotePrecedence)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.ContentClass, new InboundMimeHeadersParser.CustomRule(new InboundMimeHeadersParser.CustomRule.PromotionDelegate(InboundMimeHeadersParser.PromoteContentClass)));
			InboundMimeHeadersParser.AddRule(dictionary, "X-Message-Flag", new InboundMimeHeadersParser.CustomRule(new InboundMimeHeadersParser.CustomRule.PromotionDelegate(InboundMimeHeadersParser.PromoteXMessageFlag)));
			InboundMimeHeadersParser.AddRule(dictionary, "X-Auto-Response-Suppress", new InboundMimeHeadersParser.CustomRule(new InboundMimeHeadersParser.CustomRule.PromotionDelegate(InboundMimeHeadersParser.PromoteXAutoResponseSuppress)));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.ListHelp, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ListHelp));
			InboundMimeHeadersParser.AddRule(dictionary, "X-List-Help", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ListHelp, new InboundMimeHeadersParser.HeaderPriorityList(new HeaderId[]
			{
				HeaderId.ListHelp
			})));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.ListSubscribe, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ListSubscribe));
			InboundMimeHeadersParser.AddRule(dictionary, "X-List-Subscribe", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ListSubscribe, new InboundMimeHeadersParser.HeaderPriorityList(new HeaderId[]
			{
				HeaderId.ListSubscribe
			})));
			InboundMimeHeadersParser.AddRule(dictionary, HeaderId.ListUnsubscribe, new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ListUnsubscribe));
			InboundMimeHeadersParser.AddRule(dictionary, "X-List-Unsubscribe", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ListUnsubscribe, new InboundMimeHeadersParser.HeaderPriorityList(new HeaderId[]
			{
				HeaderId.ListUnsubscribe
			})));
			InboundMimeHeadersParser.AddRule(dictionary, "Accept-Language", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.AcceptLanguage));
			InboundMimeHeadersParser.AddRule(dictionary, "X-Accept-Language", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.AcceptLanguage, new InboundMimeHeadersParser.HeaderPriorityList(new string[]
			{
				"Accept-Language"
			})));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-SCL", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.SpamConfidenceLevel, delegate(InboundMimeHeadersParser self, Header header, string value)
			{
				int num = 0;
				if (int.TryParse(value, out num) && num >= -1 && num <= 10)
				{
					return num;
				}
				StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser: failed to parse header value ({0}: {1})", header.Name, value);
				return null;
			}));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Original-SCL", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.OriginalScl, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToInt32)));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-PCL", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ContentFilterPcl, delegate(InboundMimeHeadersParser self, Header header, string value)
			{
				int num;
				if (int.TryParse(value, out num) && ConvertUtils.IsValidPCL(num))
				{
					return num;
				}
				StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser: failed to parse header value ({0}: {1})", header.Name, value);
				return null;
			}));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-PRD", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.PurportedSenderDomain));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-SenderIdResult", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.SenderIdStatus, delegate(InboundMimeHeadersParser self, Header header, string value)
			{
				SenderIdStatus senderIdStatus;
				if (EnumValidator<SenderIdStatus>.TryParse(value, EnumParseOptions.IgnoreCase, out senderIdStatus))
				{
					return (int)senderIdStatus;
				}
				StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser: failed to parse header value ({0}: {1})", header.Name, value);
				return null;
			}));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-AVStamp-Mailbox", new InboundMimeHeadersParser.CustomRule(new InboundMimeHeadersParser.CustomRule.PromotionDelegate(InboundMimeHeadersParser.PromoteAVStampMailbox)));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Recipient-P2-Type", new InboundMimeHeadersParser.CustomRule(new InboundMimeHeadersParser.CustomRule.PromotionDelegate(InboundMimeHeadersParser.PromoteRecipientP2Type)));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Outlook-Protection-Rule-Addin-Version", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMSExchangeOutlookProtectionRuleVersion));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Outlook-Protection-Rule-Config-Timestamp", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMSExchangeOutlookProtectionRuleConfigTimestamp));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Outlook-Protection-Rule-Overridden", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMSExchangeOutlookProtectionRuleOverridden));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-DeliverAsRead", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.DeliverAsRead, (InboundMimeHeadersParser self, Header header, string value) => true));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-MailReplied", new InboundMimeHeadersParser.CustomRule(delegate(InboundMimeHeadersParser self, Header header, string value)
			{
				IconIndex valueOrDefault = self.Item.GetValueOrDefault<IconIndex>(InternalSchema.IconIndex, IconIndex.Default);
				IconIndex iconIndex = valueOrDefault;
				if (iconIndex <= IconIndex.MailUnread)
				{
					switch (iconIndex)
					{
					case IconIndex.Default:
					case IconIndex.PostItem:
						break;
					case (IconIndex)0:
						return;
					default:
						switch (iconIndex)
						{
						case IconIndex.BaseMail:
						case IconIndex.MailUnread:
							break;
						default:
							return;
						}
						break;
					}
				}
				else if (iconIndex != IconIndex.MailForwarded)
				{
					switch (iconIndex)
					{
					case IconIndex.MailEncrypted:
					case IconIndex.MailEncryptedForwarded:
					case IconIndex.MailEncryptedRead:
						self.Item[InternalSchema.IconIndex] = IconIndex.MailEncryptedReplied;
						return;
					case IconIndex.MailSmimeSigned:
					case IconIndex.MailSmimeSignedForwarded:
					case IconIndex.MailSmimeSignedRead:
						self.Item[InternalSchema.IconIndex] = IconIndex.MailSmimeSignedReplied;
						return;
					case (IconIndex)274:
					case IconIndex.MailEncryptedReplied:
					case IconIndex.MailSmimeSignedReplied:
						return;
					default:
						switch (iconIndex)
						{
						case IconIndex.MailIrm:
						case IconIndex.MailIrmForwarded:
							self.Item[InternalSchema.IconIndex] = IconIndex.MailIrmReplied;
							return;
						default:
							return;
						}
						break;
					}
				}
				self.Item[InternalSchema.IconIndex] = IconIndex.MailReplied;
			}));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-MailForwarded", new InboundMimeHeadersParser.CustomRule(delegate(InboundMimeHeadersParser self, Header header, string value)
			{
				IconIndex valueOrDefault = self.Item.GetValueOrDefault<IconIndex>(InternalSchema.IconIndex, IconIndex.Default);
				IconIndex iconIndex = valueOrDefault;
				if (iconIndex <= IconIndex.MailUnread)
				{
					switch (iconIndex)
					{
					case IconIndex.Default:
					case IconIndex.PostItem:
						break;
					case (IconIndex)0:
						return;
					default:
						switch (iconIndex)
						{
						case IconIndex.BaseMail:
						case IconIndex.MailUnread:
							break;
						default:
							return;
						}
						break;
					}
				}
				else if (iconIndex != IconIndex.MailReplied)
				{
					switch (iconIndex)
					{
					case IconIndex.MailEncrypted:
					case IconIndex.MailEncryptedReplied:
					case IconIndex.MailEncryptedRead:
						self.Item[InternalSchema.IconIndex] = IconIndex.MailEncryptedForwarded;
						return;
					case IconIndex.MailSmimeSigned:
					case IconIndex.MailSmimeSignedReplied:
					case IconIndex.MailSmimeSignedRead:
						self.Item[InternalSchema.IconIndex] = IconIndex.MailSmimeSignedForwarded;
						return;
					case (IconIndex)274:
					case IconIndex.MailEncryptedForwarded:
					case IconIndex.MailSmimeSignedForwarded:
						return;
					default:
						switch (iconIndex)
						{
						case IconIndex.MailIrm:
						case IconIndex.MailIrmReplied:
							self.Item[InternalSchema.IconIndex] = IconIndex.MailIrmForwarded;
							return;
						case IconIndex.MailIrmForwarded:
							return;
						default:
							return;
						}
						break;
					}
				}
				self.Item[InternalSchema.IconIndex] = IconIndex.MailForwarded;
			}));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Category", new InboundMimeHeadersParser.CustomRule(delegate(InboundMimeHeadersParser self, Header header, string value)
			{
				string[] array = self.Item.GetValueOrDefault<string[]>(InternalSchema.Categories, null);
				if (array == null || array.Length == 0)
				{
					array = new string[]
					{
						value
					};
				}
				else
				{
					string[] array2 = new string[array.Length + 1];
					Array.Copy(array, 0, array2, 0, array.Length);
					array2[array.Length] = value;
					array = array2;
				}
				self.Item[InternalSchema.Categories] = array;
			}));
			InboundMimeHeadersParser.AddRule(dictionary, "X-Payload-Class", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.AttachPayloadClass));
			InboundMimeHeadersParser.AddRule(dictionary, "X-Payload-Provider-Guid", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.AttachPayloadProviderGuidString));
			InboundMimeHeadersParser.AddRule(dictionary, "x-microsoft-classified", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.IsClassified, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToBoolean)));
			InboundMimeHeadersParser.AddRule(dictionary, "X-microsoft-classKeep", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ClassificationKeep, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToBoolean)));
			InboundMimeHeadersParser.AddRule(dictionary, "x-microsoft-classification", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.Classification));
			InboundMimeHeadersParser.AddRule(dictionary, "x-microsoft-classDesc", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ClassificationDescription));
			InboundMimeHeadersParser.AddRule(dictionary, "x-microsoft-classID", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ClassificationGuid));
			InboundMimeHeadersParser.AddRule(dictionary, "X-RequireProtectedPlayOnPhone", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XRequireProtectedPlayOnPhone));
			InboundMimeHeadersParser.AddRule(dictionary, "X-CallingTelephoneNumber", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.SenderTelephoneNumber));
			InboundMimeHeadersParser.AddRule(dictionary, "X-VoiceMessageSenderName", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.VoiceMessageSenderName));
			InboundMimeHeadersParser.AddRule(dictionary, "X-AttachmentOrder", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.VoiceMessageAttachmentOrder));
			InboundMimeHeadersParser.AddRule(dictionary, "X-CallID", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.CallId));
			InboundMimeHeadersParser.AddRule(dictionary, "X-VoiceMessageDuration", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.VoiceMessageDuration, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToInt32)));
			InboundMimeHeadersParser.AddRule(dictionary, "X-FaxNumberOfPages", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.FaxNumberOfPages, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToInt32)));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-UM-PartnerContent", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMsExchangeUMPartnerContent));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-UM-PartnerContext", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMsExchangeUMPartnerContext));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-UM-PartnerStatus", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMsExchangeUMPartnerStatus));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-UM-PartnerAssignedID", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMsExchangeUMPartnerAssignedID));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-UM-DialPlanLanguage", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMsExchangeUMDialPlanLanguage));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-UM-CallerInformedOfAnalysis", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMsExchangeUMCallerInformedOfAnalysis));
			InboundMimeHeadersParser.AddRule(dictionary, "Received-SPF", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ReceivedSPF));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-AuthAs", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMsExchOrganizationAuthAs));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-AuthDomain", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMsExchOrganizationAuthDomain));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-AuthMechanism", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMsExchOrganizationAuthMechanism));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-AuthSource", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XMsExchOrganizationAuthSource));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Sharing-Instance-Guid", new InboundMimeHeadersParser.HeaderPropertyRule(MessageItemSchema.SharingInstanceGuid, delegate(InboundMimeHeadersParser parser, Header header, string value)
			{
				Guid guid;
				if (GuidHelper.TryParseGuid(value, out guid))
				{
					return guid;
				}
				StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser: failed to parse header value ({0}: {1})", header.Name, value);
				return null;
			}));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ApprovalAllowedDecisionMakers));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Approval-Requestor", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ApprovalRequestor));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Original-Sender", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.QuarantineOriginalSender, delegate(InboundMimeHeadersParser self, Header header, string value)
			{
				if (!self.CanPromoteQuarantineHeaders())
				{
					return null;
				}
				return value;
			}));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Organization-Journaling-Remote-Accounts", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.JournalingRemoteAccounts, new InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation(InboundMimeHeadersParser.ToJournalRemoteAccounts)));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Send-Outlook-Recall-Report", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.SendOutlookRecallReport, delegate(InboundMimeHeadersParser self, Header header, string value)
			{
				bool flag = ConvertUtils.MimeStringEquals(value, "false") && ObjectClass.IsOfClass(self.Item.ClassName, "IPM.Outlook.Recall", false);
				bool flag2 = false;
				if (!flag)
				{
					return null;
				}
				return flag2;
			}));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-browse-url", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingBrowseUrl));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-capabilities", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingCapabilities));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-flavor", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingFlavor));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-instance-guid", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingInstanceGuid));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-local-type", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingLocalType));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-provider-guid", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingProviderGuid));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-provider-name", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingProviderName));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-provider-url", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingProviderUrl));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-remote-name", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingRemoteName));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-remote-path", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingRemotePath));
			InboundMimeHeadersParser.AddRule(dictionary, "x-sharing-remote-type", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XSharingRemoteType));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-GroupMailbox-Id", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.XGroupMailboxSmtpAddressId));
			InboundMimeHeadersParser.AddDefaultRule(dictionary, new HeaderId[]
			{
				HeaderId.ContentBase,
				HeaderId.ContentLocation,
				HeaderId.XRef
			});
			InboundMimeHeadersParser.AddNonPromotableRule(dictionary, new HeaderId[]
			{
				HeaderId.Received,
				HeaderId.ResentSender,
				HeaderId.ResentDate,
				HeaderId.ResentMessageId,
				HeaderId.ContentType,
				HeaderId.ContentDisposition,
				HeaderId.ContentDescription,
				HeaderId.ContentTransferEncoding,
				HeaderId.ContentId,
				HeaderId.ContentMD5,
				HeaderId.MimeVersion,
				HeaderId.ReturnPath,
				HeaderId.Comments,
				HeaderId.AdHoc,
				HeaderId.ApparentlyTo,
				HeaderId.Approved,
				HeaderId.Control,
				HeaderId.Distribution,
				HeaderId.Encoding,
				HeaderId.FollowUpTo,
				HeaderId.Lines,
				HeaderId.Bytes,
				HeaderId.Article,
				HeaderId.Supercedes,
				HeaderId.NewsGroups,
				HeaderId.NntpPostingHost,
				HeaderId.Organization,
				HeaderId.Path,
				HeaderId.RR,
				HeaderId.Summary,
				HeaderId.Encrypted
			});
			InboundMimeHeadersParser.AddNonPromotableRule(dictionary, new string[]
			{
				"X-MimeOle",
				"X-MS-TNEF-Correlator",
				"X-MS-Journal-Report",
				"X-MS-Exchange-Inbox-Rules-Loop"
			});
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-ApplicationFlags", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.ExchangeApplicationFlags));
			InboundMimeHeadersParser.AddRule(dictionary, "X-MS-Exchange-Calendar-Originator-Id", new InboundMimeHeadersParser.HeaderPropertyRule(InternalSchema.CalendarOriginatorId));
			return dictionary;
		}

		private static void AddRule(Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule> rulesList, string headerName, InboundMimeHeadersParser.IHeaderPromotionRule rule)
		{
			Header header = Header.Create(headerName);
			object headerKey = InboundMimeHeadersParser.GetHeaderKey(header);
			rulesList.Add(headerKey, rule);
		}

		private static void AddRule(Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule> rulesList, HeaderId headerId, InboundMimeHeadersParser.IHeaderPromotionRule rule)
		{
			object headerKey = InboundMimeHeadersParser.GetHeaderKey(headerId);
			rulesList.Add(headerKey, rule);
		}

		private static void AddDefaultRule(Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule> rulesList, params HeaderId[] headerIds)
		{
			InboundMimeHeadersParser.DefaultHeaderRule rule = new InboundMimeHeadersParser.DefaultHeaderRule();
			foreach (HeaderId headerId in headerIds)
			{
				InboundMimeHeadersParser.AddRule(rulesList, headerId, rule);
			}
		}

		private static void AddDefaultRule(Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule> rulesList, params string[] headerNames)
		{
			InboundMimeHeadersParser.DefaultHeaderRule rule = new InboundMimeHeadersParser.DefaultHeaderRule();
			foreach (string headerName in headerNames)
			{
				InboundMimeHeadersParser.AddRule(rulesList, headerName, rule);
			}
		}

		private static void AddNonPromotableRule(Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule> rulesList, params HeaderId[] headerIds)
		{
			foreach (HeaderId headerId in headerIds)
			{
				InboundMimeHeadersParser.AddRule(rulesList, headerId, null);
			}
		}

		private static void AddNonPromotableRule(Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule> rulesList, params string[] headerNames)
		{
			foreach (string headerName in headerNames)
			{
				InboundMimeHeadersParser.AddRule(rulesList, headerName, null);
			}
		}

		private InboundMimeHeadersParser.IHeaderPromotionRule FindRule(Header header)
		{
			object headerKey = InboundMimeHeadersParser.GetHeaderKey(header);
			InboundMimeHeadersParser.IHeaderPromotionRule result = null;
			if (this.headerRulesCopy.TryGetValue(headerKey, out result))
			{
				return result;
			}
			return this.DefaultRule;
		}

		private static object GetHeaderKey(Header header)
		{
			if (header.HeaderId != HeaderId.Unknown)
			{
				return InboundMimeHeadersParser.GetHeaderKey(header.HeaderId);
			}
			return header.Name.ToLowerInvariant();
		}

		private static object GetHeaderKey(string headerName)
		{
			Header header = Header.Create(headerName);
			return InboundMimeHeadersParser.GetHeaderKey(header);
		}

		private static object GetHeaderKey(HeaderId headerId)
		{
			return headerId;
		}

		internal bool CheckIsHeaderPresent(HeaderId headerId)
		{
			return this.Headers.FindFirst(headerId) != null;
		}

		internal bool CheckIsHeaderPresent(string headerName)
		{
			return this.Headers.FindFirst(headerName) != null;
		}

		internal static object ToDateTime(string dateTimeString)
		{
			DateHeader dateHeader = new DateHeader("<empty>", DateTime.UtcNow);
			if (!dateHeader.IsValueValid(dateTimeString))
			{
				return null;
			}
			dateHeader.Value = dateTimeString;
			ExDateTime exDateTime = ExDateTime.MinValue;
			try
			{
				exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, dateHeader.UtcDateTime);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				StorageGlobals.ContextTraceDebug<DateTime, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser::ToDateTime: failed to parse header value ({0}: {1})", dateHeader.UtcDateTime, ex.Message);
			}
			if (!(exDateTime != ExDateTime.MinValue))
			{
				return null;
			}
			return exDateTime;
		}

		internal static object ToDateTime(InboundMimeHeadersParser self, Header header, string value)
		{
			if (value == null)
			{
				return null;
			}
			object obj = InboundMimeHeadersParser.ToDateTime(value);
			if (obj == null)
			{
				StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser::ToDateTime: failed to parse header value ({0}: {1})", header.Name, value);
			}
			return obj;
		}

		private static object ToInt32(InboundMimeHeadersParser self, Header header, string value)
		{
			if (value == null)
			{
				return null;
			}
			int num;
			if (int.TryParse(value, out num))
			{
				return num;
			}
			StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser::ToInt32: failed to parse header value ({0}: {1})", header.Name, value);
			return null;
		}

		private static object ToBoolean(InboundMimeHeadersParser self, Header header, string value)
		{
			if (value == null)
			{
				return null;
			}
			return ConvertUtils.MimeStringEquals(value, "true");
		}

		private static object ToJournalRemoteAccounts(InboundMimeHeadersParser self, Header header, string value)
		{
			string[] array = value.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			return array;
		}

		private static object KeywordsToCategories(InboundMimeHeadersParser self, Header header, string value)
		{
			object result = null;
			if (!self.ConversionOptions.ClearCategories && value != null)
			{
				string[] array = value.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Trim();
				}
				result = array;
			}
			return result;
		}

		private static object ToSensitivity(InboundMimeHeadersParser self, Header header, string value)
		{
			if (ConvertUtils.MimeStringEquals(value, "normal"))
			{
				return Sensitivity.Normal;
			}
			if (ConvertUtils.MimeStringEquals(value, "personal"))
			{
				return Sensitivity.Personal;
			}
			if (ConvertUtils.MimeStringEquals(value, "private"))
			{
				return Sensitivity.Private;
			}
			if (ConvertUtils.MimeStringEquals(value, "company-confidential"))
			{
				return Sensitivity.CompanyConfidential;
			}
			StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser::ToSensitivity: failed to parse header value ({0}: {1})", header.Name, value);
			return null;
		}

		private static object ToImportance(InboundMimeHeadersParser self, Header header, string value)
		{
			if (ConvertUtils.MimeStringEquals(value, "high"))
			{
				return Importance.High;
			}
			if (ConvertUtils.MimeStringEquals(value, "low"))
			{
				return Importance.Low;
			}
			if (ConvertUtils.MimeStringEquals(value, "normal"))
			{
				return Importance.Normal;
			}
			StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser::ToImportance: failed to parse header value ({0}: {1})", header.Name, value);
			return null;
		}

		private static object PriorityToImportance(InboundMimeHeadersParser self, Header header, string priorityValue)
		{
			if (ConvertUtils.MimeStringEquals(priorityValue, "normal"))
			{
				return Importance.Normal;
			}
			if (ConvertUtils.MimeStringEquals(priorityValue, "urgent"))
			{
				return Importance.High;
			}
			if (ConvertUtils.MimeStringEquals(priorityValue, "non-urgent"))
			{
				return Importance.Low;
			}
			StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser::PriorityToImportance: failed to parse header value ({0}: {1})", header.Name, priorityValue);
			return null;
		}

		private static object XPriorityToImportance(InboundMimeHeadersParser self, Header header, string value)
		{
			if (value.Length == 1 || (value.Length > 1 && !char.IsDigit(value[1])))
			{
				switch (value[0])
				{
				case '1':
				case '2':
					return Importance.High;
				case '3':
					return Importance.Normal;
				case '4':
				case '5':
					return Importance.Low;
				}
			}
			StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser::XPriorityToImportance: failed to parse header value ({0}: {1})", header.Name, value);
			return null;
		}

		private bool CanPromoteQuarantineHeaders()
		{
			return !this.IsTopLevelMessage || this.IsStreamToStreamConversion;
		}

		private static void PromoteAVStampMailbox(InboundMimeHeadersParser self, Header header, string value)
		{
			Item item = self.Item;
			if (self.isTransportAVStampPresent)
			{
				string[] array = item.TryGetProperty(InternalSchema.XMsExchOrganizationAVStampMailbox) as string[];
				if (array != null)
				{
					string[] array2 = new string[array.Length + 1];
					array.CopyTo(array2, 0);
					array2[array.Length] = value;
					item[InternalSchema.XMsExchOrganizationAVStampMailbox] = array2;
					return;
				}
			}
			else
			{
				self.isTransportAVStampPresent = true;
				item[InternalSchema.XMsExchOrganizationAVStampMailbox] = new string[]
				{
					value
				};
			}
		}

		private static void PromoteRecipientP2Type(InboundMimeHeadersParser self, Header header, string value)
		{
			if (ConvertUtils.MimeStringEquals(value, "Bcc"))
			{
				self.Item[InternalSchema.MessageBccMe] = true;
			}
		}

		private static void PromoteContentLanguage(InboundMimeHeadersParser self, Header header, string value)
		{
			self.SetMessageLocaleId(value);
		}

		private static void PromoteSubject(InboundMimeHeadersParser self, Header subjectHeader, string subjectValue)
		{
			self.isSubjectPromotedFromThreadTopic = false;
			SubjectProperty.ModifySubjectProperty(self.Item, InternalSchema.MapiSubject, subjectValue);
			SubjectProperty.TruncateSubject(self.Item, self.ConversionLimits.MaxMimeSubjectLength);
		}

		private static void PromoteThreadTopic(InboundMimeHeadersParser self, Header topicHeader, string topicValue)
		{
			string text = self.Item.TryGetProperty(InternalSchema.SubjectPrefixInternal) as string;
			string text2 = self.Item.TryGetProperty(InternalSchema.MapiSubject) as string;
			if (string.IsNullOrEmpty(text) && text2 != null)
			{
				text = SubjectProperty.ExtractPrefixUsingNormalizedSubject(text2, topicValue);
				if (text != null)
				{
					self.Item[InternalSchema.SubjectPrefix] = text;
					SubjectProperty.TruncateSubject(self.Item, self.ConversionLimits.MaxMimeSubjectLength);
					return;
				}
			}
			else if (text2 == null)
			{
				self.isSubjectPromotedFromThreadTopic = true;
				self.Item[InternalSchema.NormalizedSubject] = topicValue;
				SubjectProperty.TruncateSubject(self.Item, self.ConversionLimits.MaxMimeSubjectLength);
			}
		}

		private void PromoteReceivedTime()
		{
			ReceivedHeader receivedHeader = this.Headers.FindFirst(HeaderId.Received) as ReceivedHeader;
			if (receivedHeader != null && receivedHeader.Date != null)
			{
				object obj = InboundMimeHeadersParser.ToDateTime(receivedHeader.Date);
				if (obj != null)
				{
					this.Item[InternalSchema.ReceivedTime] = obj;
				}
			}
		}

		private void PromoteXLoop()
		{
			List<string> list = null;
			for (TextHeader textHeader = this.Headers.FindFirst("X-MS-Exchange-Inbox-Rules-Loop") as TextHeader; textHeader != null; textHeader = (this.Headers.FindNext(textHeader) as TextHeader))
			{
				if (list == null)
				{
					list = new List<string>(4);
				}
				string text = textHeader.Value;
				if (text.Length > 1000)
				{
					text = text.Substring(0, 1000);
				}
				list.Add(text);
				if (list.Count == 3)
				{
					break;
				}
			}
			if (list != null)
			{
				string[] value = list.ToArray();
				this.Item[InternalSchema.XLoop] = value;
			}
		}

		internal void SetMessageLocaleId(string cultureList)
		{
			if (cultureList != null)
			{
				string[] array = cultureList.Split(new char[]
				{
					','
				});
				foreach (string text in array)
				{
					string text2 = text.Trim();
					Culture culture;
					if (!string.IsNullOrEmpty(text2) && Culture.TryGetCulture(text2, out culture))
					{
						this.Item[InternalSchema.MessageLocaleId] = culture.LCID;
						return;
					}
				}
			}
		}

		private static void PromoteDispositionNotificationTo(InboundMimeHeadersParser self, Header header, Participant participant)
		{
			Item item = self.Item;
			InboundAddressCache addressCache = self.AddressCache;
			item[InternalSchema.IsReadReceiptRequested] = true;
			if (item.GetValueOrDefault<bool>(InternalSchema.IsReadReceiptRequested))
			{
				addressCache.Participants[ConversionItemParticipants.ParticipantIndex.ReadReceipt] = participant;
				item[InternalSchema.IsReadReceiptPendingInternal] = true;
				item[InternalSchema.IsNotReadReceiptPendingInternal] = true;
			}
		}

		private static void PromoteReturnReceiptTo(InboundMimeHeadersParser self, Header header, Participant participant)
		{
			self.Item[InternalSchema.IsDeliveryReceiptRequested] = true;
		}

		private static void PromotePrecedence(InboundMimeHeadersParser self, Header header, string headerValue)
		{
			self.Item[InternalSchema.AutoResponseSuppress] = AutoResponseSuppress.All;
		}

		private static void PromoteContentClass(InboundMimeHeadersParser self, Header header, string value)
		{
			Item item = self.Item;
			self.DefaultRule.PromoteHeader(self, header);
			if (ObjectClass.IsOfClass(item.ClassName, "IPM.InfoPathForm"))
			{
				string text = value.Substring("InfoPathForm.".Length);
				int num = text.IndexOf('.');
				string value2 = text.Substring(num + 1);
				item[InternalSchema.InfoPathFormName] = value2;
			}
		}

		private static void PromoteXMessageFlag(InboundMimeHeadersParser self, Header headerName, string value)
		{
			Item item = self.Item;
			item[InternalSchema.FlagRequest] = value;
			item[InternalSchema.MapiFlagStatus] = FlagStatus.Flagged;
			item[InternalSchema.FlagSubject] = item.GetValueOrDefault<string>(InternalSchema.Subject, string.Empty);
			item[InternalSchema.TaskStatus] = TaskStatus.NotStarted;
			item.Delete(InternalSchema.StartDate);
			item.Delete(InternalSchema.DueDate);
			item[InternalSchema.IsComplete] = false;
			item[InternalSchema.PercentComplete] = 0.0;
			item.Delete(InternalSchema.FlagCompleteTime);
			item.Delete(InternalSchema.CompleteDate);
			item[InternalSchema.IsFlagSetForRecipient] = true;
		}

		private static void PromoteXAutoResponseSuppress(InboundMimeHeadersParser self, Header headerName, string value)
		{
			Item item = self.Item;
			AutoResponseSuppress autoResponseSuppress;
			if (EnumValidator<AutoResponseSuppress>.TryParse(value, EnumParseOptions.IgnoreUnknownValues | EnumParseOptions.IgnoreCase, out autoResponseSuppress))
			{
				AutoResponseSuppress valueOrDefault = item.GetValueOrDefault<AutoResponseSuppress>(InternalSchema.AutoResponseSuppress);
				item[InternalSchema.AutoResponseSuppress] = (valueOrDefault | autoResponseSuppress);
				return;
			}
			StorageGlobals.ContextTraceDebug<string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser::PromoteXAutoResponseSuppress: unable to parse the value for X-Auto-Response-Suppress: {0}", value);
		}

		private void PromoteTransportMessageHeaders()
		{
			if (this.isTransportMessageHeadersPromoted)
			{
				return;
			}
			this.isTransportMessageHeadersPromoted = true;
			InboundMimeHeadersParser.TransportMessageHeadersOutputFilter filter = new InboundMimeHeadersParser.TransportMessageHeadersOutputFilter(this.ConversionOptions);
			using (Stream stream = this.Item.OpenPropertyStream(InternalSchema.TransportMessageHeaders, PropertyOpenMode.Create))
			{
				using (Stream stream2 = new ConverterStream(stream, new TextToText(TextToTextConversionMode.ConvertCodePageOnly)
				{
					InputEncoding = CTSGlobals.AsciiEncoding,
					OutputEncoding = Encoding.Unicode
				}, ConverterStreamAccess.Write))
				{
					this.MessageRoot.WriteTo(stream2, null, filter);
				}
			}
		}

		private Participant ParseAddress(AddressHeader addressHeader, InboundMimeHeadersParser.AddressHeaderFlags flags)
		{
			MimeRecipient mimeRecipient = addressHeader.FirstChild as MimeRecipient;
			if (mimeRecipient == null)
			{
				return null;
			}
			return this.ParseAddress(mimeRecipient, flags);
		}

		private List<Participant> ParseAddressList(AddressHeader addressHeader)
		{
			List<Participant> list = new List<Participant>();
			this.ParseAddressList(list, addressHeader);
			return list;
		}

		private void ParseAddressList(List<Participant> list, IEnumerable collection)
		{
			foreach (object obj in collection)
			{
				MimeRecipient mimeRecipient = obj as MimeRecipient;
				if (mimeRecipient != null)
				{
					Participant participant = this.ParseAddress(mimeRecipient, InboundMimeHeadersParser.AddressHeaderFlags.AlwaysDeencapsulate);
					if (participant != null)
					{
						list.Add(participant);
					}
				}
				else
				{
					MimeGroup mimeGroup = obj as MimeGroup;
					if (mimeGroup != null && InboundMimeHeadersParser.CanPromoteAddressGroup(mimeGroup))
					{
						this.ParseAddressList(list, mimeGroup);
					}
				}
			}
		}

		private Participant ParseAddress(MimeRecipient mimeRecipient, InboundMimeHeadersParser.AddressHeaderFlags flags)
		{
			string displayName = null;
			if (!mimeRecipient.TryGetDisplayName(out displayName))
			{
				StorageGlobals.ContextTraceDebug<string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser::ParseAddress: failed to parse recipient's display name, {0}.", mimeRecipient.Email);
			}
			bool canDeencapsulate = this.ConversionOptions.IsSenderTrusted || (flags & InboundMimeHeadersParser.AddressHeaderFlags.AlwaysDeencapsulate) == InboundMimeHeadersParser.AddressHeaderFlags.AlwaysDeencapsulate;
			return InboundMimeHeadersParser.CreateParticipantFromMime(displayName, mimeRecipient.Email, this.ConversionOptions, canDeencapsulate);
		}

		internal static Participant CreateParticipantFromMime(string displayName, string email, InboundConversionOptions options, bool canDeencapsulate)
		{
			string routingType = "SMTP";
			if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(displayName))
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, ServerStrings.ConversionEmptyAddress);
				return null;
			}
			if (email != null && canDeencapsulate && !options.IgnoreImceaDomain && !string.IsNullOrEmpty(options.ImceaEncapsulationDomain) && ImceaAddress.IsImceaAddress(email))
			{
				ImceaAddress.Decode(ref routingType, ref email, options.ImceaEncapsulationDomain);
			}
			if (string.IsNullOrEmpty(email))
			{
				routingType = null;
			}
			if (displayName != null && displayName.Length > 512)
			{
				int num = 512;
				if (char.IsHighSurrogate(displayName, num - 1))
				{
					num--;
				}
				displayName = displayName.Substring(0, num);
			}
			Participant participant = new Participant(displayName, email, routingType);
			if (string.IsNullOrWhiteSpace(participant.EmailAddress) && string.IsNullOrWhiteSpace(participant.DisplayName))
			{
				return null;
			}
			return participant;
		}

		private static bool CanPromoteAddressGroup(MimeGroup addressGroup)
		{
			return addressGroup.DisplayName != null && !ConvertUtils.MimeStringEquals(addressGroup.DisplayName, "undisclosed recipients");
		}

		internal void PromoteMessageHeaders()
		{
			this.PromoteTransportMessageHeaders();
			this.PromoteReceivedTime();
			this.PromoteXLoop();
			foreach (Header header in this.Headers)
			{
				if (!this.ConversionOptions.ApplyHeaderFirewall || !MimeConstants.IsInReservedHeaderNamespace(header.Name))
				{
					InboundMimeHeadersParser.IHeaderPromotionRule headerPromotionRule = this.FindRule(header);
					if (headerPromotionRule != null)
					{
						headerPromotionRule.PromoteHeader(this, header);
					}
				}
			}
			if (this.isSubjectPromotedFromThreadTopic)
			{
				this.Item.Delete(InternalSchema.Subject);
			}
		}

		internal bool IsSentByLegacyExchange()
		{
			string headerValue = MimeHelpers.GetHeaderValue(this.Headers, "X-MimeOle", this.ConversionOptions);
			return headerValue != null && headerValue.IndexOf("Microsoft Exchange", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public InboundMimeHeadersParser(InboundMimeConverter owner)
		{
			this.owner = owner;
			this.headers = owner.MessageRoot.Headers;
			this.isSubjectPromotedFromThreadTopic = false;
		}

		private InboundConversionOptions ConversionOptions
		{
			get
			{
				return this.owner.ConversionOptions;
			}
		}

		private ConversionLimits ConversionLimits
		{
			get
			{
				return this.ConversionOptions.Limits;
			}
		}

		private InboundAddressCache AddressCache
		{
			get
			{
				return this.owner.AddressCache;
			}
		}

		private Item Item
		{
			get
			{
				return this.owner.Item;
			}
		}

		private HeaderList Headers
		{
			get
			{
				return this.headers;
			}
		}

		private MimePart MessageRoot
		{
			get
			{
				return this.owner.EmailMessage.RootPart;
			}
		}

		private InboundMimeHeadersParser.DefaultHeaderRule DefaultRule
		{
			get
			{
				if (this.defaultRule == null)
				{
					this.defaultRule = new InboundMimeHeadersParser.DefaultHeaderRule();
				}
				return this.defaultRule;
			}
		}

		private bool IsStreamToStreamConversion
		{
			get
			{
				return (this.owner.ConverterFlags & ConverterFlags.IsStreamToStreamConversion) == ConverterFlags.IsStreamToStreamConversion;
			}
		}

		private bool IsTopLevelMessage
		{
			get
			{
				return (this.owner.ConverterFlags & ConverterFlags.IsEmbeddedMessage) != ConverterFlags.IsEmbeddedMessage;
			}
		}

		private static readonly Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule> staticHeaderRules = InboundMimeHeadersParser.CreateHeaderRulesTable();

		private readonly Dictionary<object, InboundMimeHeadersParser.IHeaderPromotionRule> headerRulesCopy = InboundMimeHeadersParser.staticHeaderRules;

		private InboundMimeConverter owner;

		private HeaderList headers;

		private InboundMimeHeadersParser.DefaultHeaderRule defaultRule;

		private bool isTransportAVStampPresent;

		private bool isTransportMessageHeadersPromoted;

		private bool isSubjectPromotedFromThreadTopic;

		private class TransportMessageHeadersOutputFilter : MimeOutputFilter
		{
			public TransportMessageHeadersOutputFilter(InboundConversionOptions options)
			{
				this.options = options;
			}

			public override bool FilterPartBody(MimePart part, Stream stream)
			{
				return true;
			}

			public override bool FilterHeader(Header header, Stream stream)
			{
				string name = header.Name;
				return MimeConstants.IsInReservedHeaderNamespace(name) && (this.options.ApplyHeaderFirewall || !MimeConstants.IsReservedHeaderAllowedOnDelivery(name));
			}

			private InboundConversionOptions options;
		}

		private class HeaderPriorityList
		{
			public HeaderPriorityList(params HeaderId[] dependencyIds)
			{
				this.dependentHeaderIds = dependencyIds;
			}

			public HeaderPriorityList(params string[] dependencyNames)
			{
				List<string> list = new List<string>();
				List<HeaderId> list2 = new List<HeaderId>();
				foreach (string name in dependencyNames)
				{
					Header header = Header.Create(name);
					if (header.HeaderId != HeaderId.Unknown)
					{
						list2.Add(header.HeaderId);
					}
					else
					{
						list.Add(header.Name);
					}
				}
				if (list.Count != 0)
				{
					this.dependentHeaderNames = list.ToArray();
				}
				if (list2.Count != 0)
				{
					this.dependentHeaderIds = list2.ToArray();
				}
			}

			public static bool CanPromoteHeader(InboundMimeHeadersParser parser, Header header, InboundMimeHeadersParser.HeaderPriorityList priorityList)
			{
				if (priorityList == null)
				{
					return true;
				}
				if (priorityList.dependentHeaderIds != null)
				{
					for (int num = 0; num != priorityList.dependentHeaderIds.Length; num++)
					{
						if (parser.CheckIsHeaderPresent(priorityList.dependentHeaderIds[num]))
						{
							return false;
						}
					}
				}
				if (priorityList.dependentHeaderNames != null)
				{
					for (int num2 = 0; num2 != priorityList.dependentHeaderNames.Length; num2++)
					{
						if (parser.CheckIsHeaderPresent(priorityList.dependentHeaderNames[num2]))
						{
							return false;
						}
					}
				}
				return true;
			}

			private string[] dependentHeaderNames;

			private HeaderId[] dependentHeaderIds;
		}

		private interface IHeaderPromotionRule
		{
			void PromoteHeader(InboundMimeHeadersParser parser, Header header);
		}

		private class HeaderPropertyRule : InboundMimeHeadersParser.IHeaderPromotionRule
		{
			public HeaderPropertyRule(StorePropertyDefinition property)
			{
				this.property = property;
				this.lengthLimit = int.MaxValue;
				this.transformation = null;
				this.priorityList = null;
			}

			public HeaderPropertyRule(StorePropertyDefinition property, InboundMimeHeadersParser.HeaderPriorityList priorityList)
			{
				this.property = property;
				this.lengthLimit = int.MaxValue;
				this.transformation = null;
				this.priorityList = priorityList;
			}

			public HeaderPropertyRule(StorePropertyDefinition property, int lengthLimit)
			{
				this.property = property;
				this.lengthLimit = lengthLimit;
				this.transformation = null;
				this.priorityList = null;
			}

			public HeaderPropertyRule(StorePropertyDefinition property, InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation transformation)
			{
				this.property = property;
				this.lengthLimit = int.MaxValue;
				this.transformation = transformation;
				this.priorityList = null;
			}

			public HeaderPropertyRule(StorePropertyDefinition property, InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation transformation, InboundMimeHeadersParser.HeaderPriorityList priorityList)
			{
				this.property = property;
				this.lengthLimit = int.MaxValue;
				this.transformation = transformation;
				this.priorityList = priorityList;
			}

			public void PromoteHeader(InboundMimeHeadersParser parser, Header header)
			{
				if (!InboundMimeHeadersParser.HeaderPriorityList.CanPromoteHeader(parser, header, this.priorityList))
				{
					return;
				}
				int num = Math.Min(this.lengthLimit, parser.ConversionLimits.MaxMimeTextHeaderLength);
				string headerValue = MimeHelpers.GetHeaderValue(header, num);
				if (headerValue == null)
				{
					return;
				}
				object obj;
				if (this.transformation != null)
				{
					obj = this.transformation(parser, header, headerValue);
				}
				else
				{
					obj = headerValue;
				}
				if (obj != null)
				{
					parser.Item[this.property] = obj;
				}
			}

			private StorePropertyDefinition property;

			private int lengthLimit;

			private InboundMimeHeadersParser.HeaderPropertyRule.HeaderValueTransformation transformation;

			private InboundMimeHeadersParser.HeaderPriorityList priorityList;

			public delegate object HeaderValueTransformation(InboundMimeHeadersParser parser, Header header, string value);
		}

		private class DefaultHeaderRule : InboundMimeHeadersParser.IHeaderPromotionRule
		{
			public DefaultHeaderRule()
			{
				this.lengthLimit = int.MaxValue;
			}

			public DefaultHeaderRule(int lengthLimit)
			{
				this.lengthLimit = lengthLimit;
			}

			public void PromoteHeader(InboundMimeHeadersParser parser, Header header)
			{
				int num = Math.Min(this.lengthLimit, parser.ConversionLimits.MaxMimeTextHeaderLength);
				string headerValue = MimeHelpers.GetHeaderValue(header, num);
				if (!parser.IsTopLevelMessage || parser.IsStreamToStreamConversion || !MimeConstants.IsInReservedHeaderNamespace(header.Name))
				{
					if (headerValue == null)
					{
						AddressHeader addressHeader = header as AddressHeader;
						if (addressHeader != null)
						{
							List<Participant> list = parser.ParseAddressList(addressHeader);
							StringBuilder stringBuilder = new StringBuilder(list.Count * 32);
							int num2 = 0;
							foreach (Participant participant in list)
							{
								if (participant.EmailAddress != null)
								{
									string displayName = participant.DisplayName;
									string text = null;
									if (participant.RoutingType == "SMTP")
									{
										text = participant.EmailAddress;
									}
									else if (!parser.ConversionOptions.IgnoreImceaDomain)
									{
										text = ImceaAddress.Encode(participant.RoutingType, participant.EmailAddress, parser.ConversionOptions.ImceaEncapsulationDomain);
									}
									if (text != null)
									{
										if (displayName == null || displayName == text)
										{
											stringBuilder.AppendFormat("{0}{1}", (num2 == 0) ? string.Empty : ", ", text);
										}
										else
										{
											stringBuilder.AppendFormat("{0}\"{1}\" <{2}>", (num2 == 0) ? string.Empty : ", ", displayName, text);
										}
										num2++;
									}
								}
							}
							InboundMimeHeadersParser.DefaultHeaderRule.SetInternetHeader(parser.Item, header.Name, stringBuilder.ToString());
							return;
						}
					}
					else
					{
						InboundMimeHeadersParser.DefaultHeaderRule.SetInternetHeader(parser.Item, header.Name, headerValue);
					}
				}
			}

			private static void SetInternetHeader(Item item, string headerName, string headerValue)
			{
				string propertyName = headerName.ToLowerInvariant();
				if (GuidNamePropertyDefinition.IsValidName(WellKnownPropertySet.InternetHeaders, propertyName))
				{
					StorePropertyDefinition propertyDefinition = GuidNamePropertyDefinition.CreateCustom(string.Empty, typeof(string), WellKnownPropertySet.InternetHeaders, propertyName, PropertyFlags.None);
					item[propertyDefinition] = headerValue;
					return;
				}
				StorageGlobals.ContextTraceDebug<string, string>(ExTraceGlobals.CcInboundMimeTracer, "InboundMimeHeadersParser: not promoting X-header to a NamedProperty with an invalid name:\r\n({0}: {1})", headerName, headerValue);
			}

			private int lengthLimit;
		}

		public enum AddressHeaderFlags
		{
			DeencapsulateIfSenderTrusted = 1,
			AlwaysDeencapsulate
		}

		private class AddressHeaderRule : InboundMimeHeadersParser.IHeaderPromotionRule
		{
			public AddressHeaderRule(InboundMimeHeadersParser.AddressHeaderRule.PromoteAddressList listPromotionDelegate)
			{
				this.listPromotionDelegate = listPromotionDelegate;
			}

			public AddressHeaderRule(InboundMimeHeadersParser.AddressHeaderRule.PromoteSingleAddress addressPromotionDelegate, InboundMimeHeadersParser.AddressHeaderFlags flags)
			{
				this.addressPromotionDelegate = addressPromotionDelegate;
				this.flags = flags;
			}

			public void PromoteHeader(InboundMimeHeadersParser parser, Header header)
			{
				AddressHeader addressHeader = (AddressHeader)header;
				if (this.IsSingleAddress)
				{
					Participant participant = parser.ParseAddress(addressHeader, this.flags);
					this.addressPromotionDelegate(parser, header, participant);
					return;
				}
				List<Participant> participantList = parser.ParseAddressList(addressHeader);
				this.listPromotionDelegate(parser, header, participantList);
			}

			private bool IsSingleAddress
			{
				get
				{
					return this.addressPromotionDelegate != null;
				}
			}

			private InboundMimeHeadersParser.AddressHeaderRule.PromoteAddressList listPromotionDelegate;

			private InboundMimeHeadersParser.AddressHeaderRule.PromoteSingleAddress addressPromotionDelegate;

			private InboundMimeHeadersParser.AddressHeaderFlags flags;

			public delegate void PromoteAddressList(InboundMimeHeadersParser parser, Header header, List<Participant> participantList);

			public delegate void PromoteSingleAddress(InboundMimeHeadersParser parser, Header header, Participant participant);
		}

		private class CustomRule : InboundMimeHeadersParser.IHeaderPromotionRule
		{
			public CustomRule(InboundMimeHeadersParser.CustomRule.PromotionDelegate promotionDelegate)
			{
				this.promotionDelegate = promotionDelegate;
				this.lengthLimit = int.MaxValue;
			}

			public CustomRule(InboundMimeHeadersParser.CustomRule.PromotionDelegate promotionDelegate, int lengthLimit)
			{
				this.promotionDelegate = promotionDelegate;
				this.lengthLimit = lengthLimit;
			}

			public void PromoteHeader(InboundMimeHeadersParser parser, Header header)
			{
				int num = Math.Min(this.lengthLimit, parser.ConversionOptions.Limits.MaxMimeTextHeaderLength);
				string headerValue = MimeHelpers.GetHeaderValue(header, num);
				if (headerValue != null)
				{
					this.promotionDelegate(parser, header, headerValue);
				}
			}

			private InboundMimeHeadersParser.CustomRule.PromotionDelegate promotionDelegate;

			private int lengthLimit;

			public delegate void PromotionDelegate(InboundMimeHeadersParser parser, Header header, string value);
		}
	}
}
