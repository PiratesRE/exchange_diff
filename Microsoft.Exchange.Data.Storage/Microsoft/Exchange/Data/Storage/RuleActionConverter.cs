using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class RuleActionConverter
	{
		internal static RuleAction ConvertRuleAction(StoreSession session, ExTimeZone timeZone, RuleAction ruleAction)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(timeZone, "timeZone");
			Util.ThrowOnNullArgument(ruleAction, "ruleAction");
			RuleAction ruleAction2;
			switch (ruleAction.ActionType)
			{
			case RuleActionType.Move:
			{
				RuleAction.MoveAction moveAction = (RuleAction.MoveAction)ruleAction;
				if (moveAction.ExternalDestinationFolderId != null)
				{
					ruleAction2 = new RuleAction.ExternalMove(moveAction.DestinationStoreEntryId, moveAction.ExternalDestinationFolderId);
				}
				else
				{
					ruleAction2 = new RuleAction.InMailboxMove(moveAction.DestinationFolderId.ProviderLevelItemId);
				}
				break;
			}
			case RuleActionType.Copy:
			{
				RuleAction.CopyAction copyAction = (RuleAction.CopyAction)ruleAction;
				if (copyAction.ExternalDestinationFolderId != null)
				{
					ruleAction2 = new RuleAction.ExternalCopy(copyAction.DestinationStoreEntryId, copyAction.ExternalDestinationFolderId);
				}
				else
				{
					ruleAction2 = new RuleAction.InMailboxCopy(copyAction.DestinationFolderId.ProviderLevelItemId);
				}
				break;
			}
			case RuleActionType.Reply:
			{
				RuleAction.ReplyAction replyAction = (RuleAction.ReplyAction)ruleAction;
				ruleAction2 = new RuleAction.Reply(RuleActionConverter.GetReplyTemplateMessageEntryId(replyAction.ReplyTemplateMessageId), replyAction.ReplyTemplateGuid, RuleActionConverter.ReplyFlagsToMapiReplyFlags(replyAction.Flags));
				break;
			}
			case RuleActionType.OutOfOfficeReply:
			{
				RuleAction.OutOfOfficeReplyAction outOfOfficeReplyAction = (RuleAction.OutOfOfficeReplyAction)ruleAction;
				ruleAction2 = new RuleAction.OOFReply(RuleActionConverter.GetReplyTemplateMessageEntryId(outOfOfficeReplyAction.ReplyTemplateMessageId), outOfOfficeReplyAction.ReplyTemplateGuid);
				break;
			}
			case RuleActionType.DeferAction:
			{
				RuleAction.DeferAction deferAction = (RuleAction.DeferAction)ruleAction;
				ruleAction2 = new RuleAction.Defer(deferAction.Data);
				break;
			}
			case RuleActionType.Bounce:
			{
				RuleAction.BounceAction bounceAction = (RuleAction.BounceAction)ruleAction;
				ruleAction2 = new RuleAction.Bounce((RuleAction.Bounce.BounceCode)bounceAction.BounceCode);
				break;
			}
			case RuleActionType.Forward:
			{
				RuleAction.ForwardAction forwardAction = (RuleAction.ForwardAction)ruleAction;
				ruleAction2 = new RuleAction.Forward(RuleActionConverter.GetAdrEntries(session, timeZone, forwardAction.Recipients), RuleActionConverter.ForwardFlagsToMapiForwardFlags(forwardAction.Flags));
				break;
			}
			case RuleActionType.Delegate:
			{
				RuleAction.DelegateAction delegateAction = (RuleAction.DelegateAction)ruleAction;
				ruleAction2 = new RuleAction.Delegate(RuleActionConverter.GetAdrEntries(session, timeZone, delegateAction.Recipients));
				break;
			}
			case RuleActionType.Tag:
			{
				RuleAction.TagAction tagAction = (RuleAction.TagAction)ruleAction;
				PropTag propTag = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(session.Mailbox.MapiStore, session, new NativeStorePropertyDefinition[]
				{
					tagAction.PropertyDefinition
				}).First<PropTag>();
				ruleAction2 = new RuleAction.Tag(MapiPropertyBag.GetPropValueFromValue(session, timeZone, propTag, tagAction.PropertyValue));
				break;
			}
			case RuleActionType.Delete:
			{
				RuleAction.DeleteAction deleteAction = (RuleAction.DeleteAction)ruleAction;
				ruleAction2 = new RuleAction.Delete();
				break;
			}
			case RuleActionType.MarkAsRead:
			{
				RuleAction.MarkAsReadAction markAsReadAction = (RuleAction.MarkAsReadAction)ruleAction;
				ruleAction2 = new RuleAction.MarkAsRead();
				break;
			}
			default:
				throw new ArgumentException(string.Format("Invalid action type {0}.", ruleAction.ActionType));
			}
			ruleAction2.UserFlags = ruleAction.UserFlags;
			return ruleAction2;
		}

		internal static RuleAction ConvertRuleAction(StoreSession session, ExTimeZone timeZone, RuleAction ruleAction)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(timeZone, "timeZone");
			Util.ThrowOnNullArgument(ruleAction, "ruleAction");
			switch (ruleAction.ActionType)
			{
			case RuleAction.Type.OP_MOVE:
			{
				RuleAction.MoveCopy moveCopy = (RuleAction.MoveCopy)ruleAction;
				if (moveCopy.FolderIsInThisStore)
				{
					return new RuleAction.MoveAction(moveCopy.UserFlags, RuleActionConverter.EntryIdToStoreObjectId(moveCopy.FolderEntryID, StoreObjectType.Folder));
				}
				return new RuleAction.MoveAction(moveCopy.UserFlags, moveCopy.StoreEntryID, moveCopy.FolderEntryID);
			}
			case RuleAction.Type.OP_COPY:
			{
				RuleAction.MoveCopy moveCopy2 = (RuleAction.MoveCopy)ruleAction;
				if (moveCopy2.FolderIsInThisStore)
				{
					return new RuleAction.CopyAction(moveCopy2.UserFlags, RuleActionConverter.EntryIdToStoreObjectId(moveCopy2.FolderEntryID, StoreObjectType.Folder));
				}
				return new RuleAction.CopyAction(moveCopy2.UserFlags, moveCopy2.StoreEntryID, moveCopy2.FolderEntryID);
			}
			case RuleAction.Type.OP_REPLY:
			{
				RuleAction.Reply reply = (RuleAction.Reply)ruleAction;
				return new RuleAction.ReplyAction(reply.UserFlags, RuleActionConverter.MapiReplyFlagsToReplyFlags(reply.Flags), RuleActionConverter.GetReplyTemplateStoreObjectId(reply.ReplyTemplateMessageEntryID), reply.ReplyTemplateGuid);
			}
			case RuleAction.Type.OP_OOF_REPLY:
			{
				RuleAction.OOFReply oofreply = (RuleAction.OOFReply)ruleAction;
				return new RuleAction.OutOfOfficeReplyAction(oofreply.UserFlags, RuleActionConverter.GetReplyTemplateStoreObjectId(oofreply.ReplyTemplateMessageEntryID), oofreply.ReplyTemplateGuid);
			}
			case RuleAction.Type.OP_DEFER_ACTION:
			{
				RuleAction.Defer defer = (RuleAction.Defer)ruleAction;
				return new RuleAction.DeferAction(defer.UserFlags, defer.Data);
			}
			case RuleAction.Type.OP_BOUNCE:
			{
				RuleAction.Bounce bounce = (RuleAction.Bounce)ruleAction;
				return new RuleAction.BounceAction(bounce.UserFlags, (uint)bounce.Code);
			}
			case RuleAction.Type.OP_FORWARD:
			{
				RuleAction.Forward forward = (RuleAction.Forward)ruleAction;
				return new RuleAction.ForwardAction(forward.UserFlags, RuleActionConverter.GetRecipients(session, timeZone, forward.Recipients), RuleActionConverter.MapiForwardFlagsToForwardFlags(forward.Flags));
			}
			case RuleAction.Type.OP_DELEGATE:
			{
				RuleAction.Delegate @delegate = (RuleAction.Delegate)ruleAction;
				return new RuleAction.DelegateAction(@delegate.UserFlags, RuleActionConverter.GetRecipients(session, timeZone, @delegate.Recipients));
			}
			case RuleAction.Type.OP_TAG:
			{
				RuleAction.Tag tag = (RuleAction.Tag)ruleAction;
				NativeStorePropertyDefinition propertyDefinition = null;
				try
				{
					propertyDefinition = PropertyTagCache.Cache.SafePropertyDefinitionsFromPropTags(session, new PropTag[]
					{
						tag.Value.PropTag
					})[0];
				}
				catch (CorruptDataException)
				{
					if (session.IsMoveUser)
					{
						return new RuleAction.TagAction(tag.UserFlags, InternalSchema.RuleError, 0);
					}
					throw;
				}
				object valueFromPropValue = MapiPropertyBag.GetValueFromPropValue(session, timeZone, propertyDefinition, tag.Value);
				return new RuleAction.TagAction(tag.UserFlags, propertyDefinition, valueFromPropValue);
			}
			case RuleAction.Type.OP_DELETE:
			{
				RuleAction.Delete delete = (RuleAction.Delete)ruleAction;
				return new RuleAction.DeleteAction(delete.UserFlags);
			}
			case RuleAction.Type.OP_MARK_AS_READ:
			{
				RuleAction.MarkAsRead markAsRead = (RuleAction.MarkAsRead)ruleAction;
				return new RuleAction.MarkAsReadAction(markAsRead.UserFlags);
			}
			default:
				throw new ArgumentException(string.Format("Invalid action type {0}.", ruleAction.ActionType));
			}
		}

		internal static StoreObjectId GetReplyTemplateStoreObjectId(byte[] entryId)
		{
			if (entryId == null)
			{
				return null;
			}
			return RuleActionConverter.EntryIdToStoreObjectId(entryId, StoreObjectType.Message);
		}

		private static byte[] GetReplyTemplateMessageEntryId(StoreObjectId storeObjectId)
		{
			if (storeObjectId == null)
			{
				return null;
			}
			return storeObjectId.ProviderLevelItemId;
		}

		private static AdrEntry[] GetAdrEntries(StoreSession session, ExTimeZone timeZone, IList<RuleAction.ForwardActionBase.ActionRecipient> recipients)
		{
			Util.ThrowOnNullArgument(recipients, "recipients");
			AdrEntry[] array = new AdrEntry[recipients.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = RuleActionConverter.GetAdrEntry(session, timeZone, recipients[i]);
			}
			return array;
		}

		private static AdrEntry GetAdrEntry(StoreSession session, ExTimeZone timeZone, RuleAction.ForwardActionBase.ActionRecipient recipient)
		{
			Util.ThrowOnNullArgument(recipient, "recipient");
			ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(session.Mailbox.MapiStore, session, recipient.PropertyDefinitions);
			PropValue[] array = new PropValue[recipient.PropertyDefinitions.Count];
			int num = 0;
			foreach (PropTag propTag in collection)
			{
				array[num] = MapiPropertyBag.GetPropValueFromValue(session, timeZone, propTag, recipient.PropertyValues[num]);
				num++;
			}
			return new AdrEntry(array);
		}

		private static RuleAction.ForwardActionBase.ActionRecipient[] GetRecipients(StoreSession session, ExTimeZone timeZone, IList<AdrEntry> adrEntries)
		{
			Util.ThrowOnNullArgument(adrEntries, "adrEntries");
			RuleAction.ForwardActionBase.ActionRecipient[] array = new RuleAction.ForwardActionBase.ActionRecipient[adrEntries.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = RuleActionConverter.GetRecipient(session, timeZone, adrEntries[i]);
			}
			return array;
		}

		private static RuleAction.ForwardActionBase.ActionRecipient GetRecipient(StoreSession session, ExTimeZone timeZone, AdrEntry adrEntry)
		{
			Util.ThrowOnNullArgument(adrEntry, "adrEntry");
			PropTag[] array = new PropTag[adrEntry.Values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = adrEntry.Values[i].PropTag;
			}
			NativeStorePropertyDefinition[] array2 = PropertyTagCache.Cache.SafePropertyDefinitionsFromPropTags(session, array);
			object[] array3 = new object[adrEntry.Values.Length];
			for (int j = 0; j < array3.Length; j++)
			{
				array3[j] = MapiPropertyBag.GetValueFromPropValue(session, timeZone, array2[j], adrEntry.Values[j]);
			}
			return new RuleAction.ForwardActionBase.ActionRecipient(array2, array3);
		}

		private static StoreObjectId EntryIdToStoreObjectId(byte[] entryId, StoreObjectType storeObjectType)
		{
			return StoreObjectId.FromProviderSpecificId(entryId, storeObjectType);
		}

		private static RuleAction.Forward.ActionFlags ForwardFlagsToMapiForwardFlags(RuleAction.ForwardAction.ForwardFlags forwardFlags)
		{
			RuleAction.Forward.ActionFlags actionFlags = RuleAction.Forward.ActionFlags.None;
			if ((forwardFlags & RuleAction.ForwardAction.ForwardFlags.DoNotChangeMessage) == RuleAction.ForwardAction.ForwardFlags.DoNotChangeMessage)
			{
				actionFlags |= RuleAction.Forward.ActionFlags.DoNotMungeMessage;
			}
			if ((forwardFlags & RuleAction.ForwardAction.ForwardFlags.ForwardAsAttachment) == RuleAction.ForwardAction.ForwardFlags.ForwardAsAttachment)
			{
				actionFlags |= RuleAction.Forward.ActionFlags.ForwardAsAttachment;
			}
			if ((forwardFlags & RuleAction.ForwardAction.ForwardFlags.PreserveSender) == RuleAction.ForwardAction.ForwardFlags.PreserveSender)
			{
				actionFlags |= RuleAction.Forward.ActionFlags.PreserveSender;
			}
			if ((forwardFlags & RuleAction.ForwardAction.ForwardFlags.SendSmsAlert) == RuleAction.ForwardAction.ForwardFlags.SendSmsAlert)
			{
				actionFlags |= RuleAction.Forward.ActionFlags.SendSmsAlert;
			}
			return actionFlags;
		}

		private static RuleAction.ForwardAction.ForwardFlags MapiForwardFlagsToForwardFlags(RuleAction.Forward.ActionFlags mapiForwardFlags)
		{
			RuleAction.ForwardAction.ForwardFlags forwardFlags = RuleAction.ForwardAction.ForwardFlags.None;
			if ((mapiForwardFlags & RuleAction.Forward.ActionFlags.DoNotMungeMessage) == RuleAction.Forward.ActionFlags.DoNotMungeMessage)
			{
				forwardFlags |= RuleAction.ForwardAction.ForwardFlags.DoNotChangeMessage;
			}
			if ((mapiForwardFlags & RuleAction.Forward.ActionFlags.ForwardAsAttachment) == RuleAction.Forward.ActionFlags.ForwardAsAttachment)
			{
				forwardFlags |= RuleAction.ForwardAction.ForwardFlags.ForwardAsAttachment;
			}
			if ((mapiForwardFlags & RuleAction.Forward.ActionFlags.PreserveSender) == RuleAction.Forward.ActionFlags.PreserveSender)
			{
				forwardFlags |= RuleAction.ForwardAction.ForwardFlags.PreserveSender;
			}
			if ((mapiForwardFlags & RuleAction.Forward.ActionFlags.SendSmsAlert) == RuleAction.Forward.ActionFlags.SendSmsAlert)
			{
				forwardFlags |= RuleAction.ForwardAction.ForwardFlags.SendSmsAlert;
			}
			return forwardFlags;
		}

		private static RuleAction.Reply.ActionFlags ReplyFlagsToMapiReplyFlags(RuleAction.ReplyAction.ReplyFlags replyFlags)
		{
			RuleAction.Reply.ActionFlags actionFlags = RuleAction.Reply.ActionFlags.None;
			if ((replyFlags & RuleAction.ReplyAction.ReplyFlags.DoNotSendToOriginator) == RuleAction.ReplyAction.ReplyFlags.DoNotSendToOriginator)
			{
				actionFlags |= RuleAction.Reply.ActionFlags.DoNotSendToOriginator;
			}
			if ((replyFlags & RuleAction.ReplyAction.ReplyFlags.UseStockReplyTemplate) == RuleAction.ReplyAction.ReplyFlags.UseStockReplyTemplate)
			{
				actionFlags |= RuleAction.Reply.ActionFlags.UseStockReplyTemplate;
			}
			return actionFlags;
		}

		internal static RuleAction.ReplyAction.ReplyFlags MapiReplyFlagsToReplyFlags(RuleAction.Reply.ActionFlags mapiReplyFlags)
		{
			RuleAction.ReplyAction.ReplyFlags replyFlags = RuleAction.ReplyAction.ReplyFlags.None;
			if ((mapiReplyFlags & RuleAction.Reply.ActionFlags.DoNotSendToOriginator) == RuleAction.Reply.ActionFlags.DoNotSendToOriginator)
			{
				replyFlags |= RuleAction.ReplyAction.ReplyFlags.DoNotSendToOriginator;
			}
			if ((mapiReplyFlags & RuleAction.Reply.ActionFlags.UseStockReplyTemplate) == RuleAction.Reply.ActionFlags.UseStockReplyTemplate)
			{
				replyFlags |= RuleAction.ReplyAction.ReplyFlags.UseStockReplyTemplate;
			}
			return replyFlags;
		}
	}
}
