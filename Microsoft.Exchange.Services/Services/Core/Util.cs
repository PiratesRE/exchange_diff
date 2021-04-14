using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Clutter;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Security.Application;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Util
	{
		internal static bool IsValidSmtpAddress(string smtpAddress)
		{
			return !string.IsNullOrEmpty(smtpAddress) && SmtpAddress.IsValidSmtpAddress(smtpAddress);
		}

		internal static void ValidateSmtpAddress(string smtpAddress)
		{
			if (!Util.IsValidSmtpAddress(smtpAddress))
			{
				throw new InvalidSmtpAddressException();
			}
		}

		internal static bool GetItemBlockStatus(Item item, bool blockExternalImages, bool blockExternalImagesIfSenderUntrusted)
		{
			return Util.GetItemBlockStatus(StaticParticipantResolver.DefaultInstance, item, blockExternalImages, blockExternalImagesIfSenderUntrusted);
		}

		internal static bool GetItemBlockStatus(IParticipantResolver participantResolver, Item item, bool blockExternalImages, bool blockExternalImagesIfSenderUntrusted)
		{
			bool result = false;
			if (blockExternalImages)
			{
				result = true;
			}
			else if (blockExternalImagesIfSenderUntrusted)
			{
				BlockStatus blockStatus = BlockStatus.DontKnow;
				if (item != null)
				{
					blockStatus = item.GetValueOrDefault<BlockStatus>(ItemSchema.BlockStatus, BlockStatus.DontKnow);
				}
				if (blockStatus != BlockStatus.NoNeverAgain)
				{
					result = !Util.IsSafeSender(participantResolver, item);
				}
			}
			return result;
		}

		internal static bool GetItemBlockStatus(IParticipantResolver participantResolver, ItemPart itemPart, bool blockExternalImages, bool blockExternalImagesIfSenderUntrusted)
		{
			bool result = false;
			if (blockExternalImages)
			{
				result = true;
			}
			else if (blockExternalImagesIfSenderUntrusted)
			{
				BlockStatus blockStatus = BlockStatus.DontKnow;
				if (itemPart != null)
				{
					blockStatus = itemPart.StorePropertyBag.GetValueOrDefault<BlockStatus>(ItemSchema.BlockStatus, BlockStatus.DontKnow);
				}
				if (blockStatus != BlockStatus.NoNeverAgain)
				{
					Participant valueOrDefault = itemPart.StorePropertyBag.GetValueOrDefault<Participant>(ItemSchema.Sender, null);
					bool valueOrDefault2 = itemPart.StorePropertyBag.GetValueOrDefault<bool>(MessageItemSchema.IsDraft, false);
					result = !Util.IsSafeSender(participantResolver, valueOrDefault, valueOrDefault2);
				}
			}
			return result;
		}

		internal static bool IsSafeSender(IParticipantResolver participantResolver, Item item)
		{
			bool result = false;
			MessageItem messageItem = item as MessageItem;
			if (messageItem != null)
			{
				result = Util.IsSafeSender(participantResolver, messageItem.Sender, messageItem.IsDraft);
			}
			CalendarItem calendarItem = item as CalendarItem;
			if (calendarItem != null)
			{
				result = Util.IsSafeSender(participantResolver, calendarItem.Organizer, false);
			}
			return result;
		}

		internal static bool IsTrustedSender(string senderEmailAddress)
		{
			JunkEmailRule junkEmailRuleForAccessingPrincipal = Util.GetJunkEmailRuleForAccessingPrincipal();
			return junkEmailRuleForAccessingPrincipal != null && junkEmailRuleForAccessingPrincipal.TrustedSenderEmailCollection.Contains(senderEmailAddress);
		}

		internal static JunkEmailRule GetJunkEmailRuleForAccessingPrincipal()
		{
			if (EWSSettings.JunkEmailRule == null)
			{
				MailboxSession mailboxIdentityMailboxSession = CallContext.Current.SessionCache.GetMailboxIdentityMailboxSession();
				if (mailboxIdentityMailboxSession != null)
				{
					EWSSettings.JunkEmailRule = mailboxIdentityMailboxSession.JunkEmailRule;
				}
			}
			return EWSSettings.JunkEmailRule;
		}

		internal static T GetItemPropertyOrDefault<T>(object[] row, PropertyDefinition propertyDefinition, T defaultValue, Dictionary<PropertyDefinition, int> propertyMap)
		{
			int num = propertyMap[propertyDefinition];
			object obj = row[num];
			if (!(obj is T))
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		internal static Dictionary<PropertyDefinition, int> LoadPropertyMap(PropertyDefinition[] properties)
		{
			Dictionary<PropertyDefinition, int> dictionary = new Dictionary<PropertyDefinition, int>();
			for (int i = 0; i < properties.Length; i++)
			{
				dictionary[properties[i]] = i;
			}
			return dictionary;
		}

		internal static ExDateTime GetLocalTime()
		{
			ExTimeZone timeZone;
			if (EWSSettings.RequestTimeZone != null)
			{
				timeZone = EWSSettings.RequestTimeZone;
			}
			else
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "[Util::GetLocalTime] EWSSettings.RequestTimeZone is null");
				timeZone = ExTimeZone.UtcTimeZone;
			}
			return ExDateTime.GetNow(timeZone);
		}

		internal static bool IsArchiveMailbox(StoreSession session)
		{
			MailboxSession mailboxSession = session as MailboxSession;
			return mailboxSession != null && mailboxSession.MailboxOwner.MailboxInfo.IsArchive;
		}

		internal static void ThrowOnNullArgument(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		internal static AttachmentCollection GetEffectiveAttachmentCollection(Item item, bool getAttachmentCollectionWhenClientSmimeInstalled = false)
		{
			if (Util.IsSMimeButNotSecureSign(item))
			{
				item = Util.GetSignedSmimeItemIfNeeded(item, getAttachmentCollectionWhenClientSmimeInstalled);
			}
			return IrmUtils.GetAttachmentCollection(item);
		}

		internal static Body GetEffectiveBody(Item item)
		{
			if (Util.IsSMimeButNotSecureSign(item))
			{
				item = Util.GetSignedSmimeItemIfNeeded(item, false);
			}
			return IrmUtils.GetBody(item);
		}

		public static ConversationType LoadConversationUsingConversationId(ConversationId conversationId, string conversationShapeName, TargetFolderId conversationFolderId, IdConverter idConverter, int hashCode, RequestDetailsLogger protocolLogFromCallContext)
		{
			ConversationType result = null;
			if (!string.IsNullOrEmpty(conversationShapeName))
			{
				ConversationResponseShape responseShape = Global.ResponseShapeResolver.GetResponseShape<ConversationResponseShape>(conversationShapeName, null, null);
				if (responseShape != null)
				{
					IdAndSession idAndSession = idConverter.ConvertFolderIdToIdAndSession(conversationFolderId.BaseFolderId, IdConverter.ConvertOption.IgnoreChangeKey);
					PropertyListForViewRowDeterminer propertyListForViewRowDeterminer = PropertyListForViewRowDeterminer.BuildForConversation(responseShape);
					PropertyDefinition[] propertiesToFetch = propertyListForViewRowDeterminer.GetPropertiesToFetch();
					QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.ConversationId, conversationId);
					using (Folder folder = Folder.Bind(idAndSession.Session, idAndSession.Id, null))
					{
						using (QueryResult queryResult = folder.ConversationItemQuery(queryFilter, null, propertiesToFetch))
						{
							BasePageResult basePageResult = BasePagingType.ApplyPostQueryPaging(queryResult, null);
							ConversationType[] array = basePageResult.View.ConvertToConversationObjects(propertiesToFetch, propertyListForViewRowDeterminer, idAndSession, protocolLogFromCallContext);
							if (array.Length > 0)
							{
								result = array[0];
								if (array.Length > 1)
								{
									ExTraceGlobals.CreateItemCallTracer.TraceError<int, string>((long)hashCode, "CreateUpdateItemCommandBase.PopulateServiceObjectWithConversationIfNecessary: {0} conversations found with store conversation id {1} where only 1 conversation was expected.", array.Length, conversationId.ToString());
								}
							}
							else
							{
								ExTraceGlobals.CreateItemCallTracer.TraceError<string>((long)hashCode, "CreateUpdateItemCommandBase.PopulateServiceObjectWithConversationIfNecessary: No conversations found with store conversation id {0}, skipping returning conversation.", conversationId.ToString());
							}
						}
					}
				}
			}
			return result;
		}

		public static ClutterState GetMailboxClutterState(MailboxSession session)
		{
			VariantConfigurationSnapshot configuration = session.MailboxOwner.GetConfiguration();
			return new ClutterState
			{
				IsClassificationEnabled = ClutterUtilities.IsClassificationEnabled(session, configuration),
				IsClutterEligible = ClutterUtilities.IsClutterEligible(session, configuration),
				IsClutterEnabled = ClutterUtilities.IsClutterEnabled(session, configuration)
			};
		}

		private static Item GetSignedSmimeItemIfNeeded(Item item, bool getAttachmentCollectionWhenClientSmimeInstalled = false)
		{
			int hashCode = item.GetHashCode();
			CallContext callContext = CallContext.Current;
			if (callContext.IsOwa && (!callContext.IsSmimeInstalled || (getAttachmentCollectionWhenClientSmimeInstalled && ObjectClass.IsSmimeClearSigned(item.ClassName))))
			{
				Item result;
				if (Util.TryOpenSMimeContent(item, out result))
				{
					ExTraceGlobals.ItemAlgorithmTracer.TraceInformation(hashCode, 0L, "GetSignedSmimeItemIfAvailable: Caller requested to open signed smime message and the message is clear or opaque signed, signed message opened and returned");
					return result;
				}
				ExTraceGlobals.ItemAlgorithmTracer.TraceWarning(hashCode, 0L, "GetSmimeAttachmentCollection: We were not able to open the signed smime message, using the outer attachment collection instead");
			}
			else
			{
				ExTraceGlobals.ItemAlgorithmTracer.TraceInformation(hashCode, 0L, "GetSmimeAttachmentCollection: Caller did not request to open signed smime message, returning null");
			}
			return item;
		}

		private static bool IsSMimeButNotSecureSign(Item message)
		{
			return !ObjectClass.IsOfClass(message.ClassName, "IPM.Note.Secure.Sign") && ObjectClass.IsSmime(message.ClassName);
		}

		private static bool IsOpaqueSigned(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			string className = item.ClassName;
			return (ObjectClass.IsOfClass(className, "IPM.Note.Secure") || (ObjectClass.IsSmime(className) && !ObjectClass.IsSmimeClearSigned(className))) && ConvertUtils.IsMessageOpaqueSigned(item);
		}

		private static bool TryOpenSMimeContent(Item smimeMessage, out Item signedSmimeItem)
		{
			bool flag = ItemConversion.TryOpenSMimeContent(smimeMessage as MessageItem, CallContext.Current.DefaultDomain.DomainName.ToString(), out signedSmimeItem);
			if (flag)
			{
				CallContext.Current.ObjectsToDisposeList.Add(signedSmimeItem);
			}
			return flag;
		}

		internal static bool IsSafeSender(IParticipantResolver resolver, Participant sender, bool isDraft)
		{
			if (PropertyCommand.InMemoryProcessOnly)
			{
				return true;
			}
			bool result = false;
			MailboxSession mailboxIdentityMailboxSession = CallContext.Current.SessionCache.GetMailboxIdentityMailboxSession();
			if (mailboxIdentityMailboxSession != null)
			{
				if (isDraft)
				{
					result = true;
				}
				else if (sender != null)
				{
					SmtpAddress smtpAddress = resolver.ResolveToSmtpAddress(sender);
					result = (CalendarSharingPermissionsUtils.CheckIfRecipientDomainIsInternal(mailboxIdentityMailboxSession.MailboxOwner.MailboxInfo.OrganizationId, smtpAddress.Domain) || Util.IsTrustedSender(smtpAddress.ToString()));
				}
			}
			return result;
		}

		internal static string EncodeForAntiXSS(string s)
		{
			return Encoder.HtmlEncode(s, true);
		}
	}
}
