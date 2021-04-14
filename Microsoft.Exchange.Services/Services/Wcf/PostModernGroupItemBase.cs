using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Data.Storage.GroupMailbox.Escalation;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class PostModernGroupItemBase<RequestType, SingleItemType> : MultiStepServiceCommand<RequestType, SingleItemType>, IDisposeTrackable, IDisposable where RequestType : BaseRequest
	{
		public PostModernGroupItemBase(CallContext callContext, RequestType request) : base(callContext, request)
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		protected bool NeedSend
		{
			get
			{
				return this.needSend;
			}
		}

		private protected MailboxSession GroupSession { protected get; private set; }

		protected Participant GroupParticipant
		{
			get
			{
				return this.groupParticipant;
			}
		}

		protected EmailAddressWrapper[] ToRecipients
		{
			get
			{
				if (this.toRecipients == null)
				{
					this.toRecipients = this.GetToRecipients();
				}
				return this.toRecipients;
			}
		}

		protected EmailAddressWrapper[] CcRecipients
		{
			get
			{
				if (this.ccRecipients == null)
				{
					this.ccRecipients = this.GetCcRecipients();
				}
				return this.ccRecipients;
			}
		}

		protected EmailAddressWrapper ModernGroupEmailAddress
		{
			get
			{
				return this.modernGroupEmailAddress;
			}
			set
			{
				this.modernGroupEmailAddress = value;
			}
		}

		protected ConversationId ConversationId
		{
			get
			{
				return this.conversationId;
			}
		}

		public string ConversationShapeName
		{
			get
			{
				return this.conversationShapeName;
			}
			set
			{
				this.conversationShapeName = value;
			}
		}

		protected bool IsFromYammer
		{
			get
			{
				return base.CallContext.CallerApplicationId == WellknownPartnerApplicationIdentifiers.Yammer;
			}
		}

		protected virtual bool IsReplying
		{
			get
			{
				return false;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PostModernGroupItemBase<RequestType, SingleItemType>>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.DisposeHelper();
			GC.SuppressFinalize(this);
		}

		internal override void PreExecuteCommand()
		{
			this.GroupSession = (MailboxSession)base.IdConverter.ConvertTargetFolderIdToIdAndContentSession(this.GetGroupFolderId().BaseFolderId, true).Session;
			this.groupParticipant = new Participant(this.GroupSession.MailboxOwner);
			this.needSend = (this.HasOtherRecipients(this.ToRecipients) || this.HasOtherRecipients(this.CcRecipients));
		}

		protected void OnBeforeSend(MessageItem messageItem)
		{
			messageItem.SuppressAllAutoResponses();
			messageItem.MarkRecipientAsSubmitted(new Participant[]
			{
				this.GroupParticipant
			});
		}

		protected void OnBeforeSaveOrSend(MessageItem messageItem)
		{
			this.onBeforeSaveOrSendCalled = true;
			messageItem.From = this.GetMeParticipant();
			messageItem.Sender = this.GroupParticipant;
			if (!this.IsGroupInRecipients(messageItem.Recipients))
			{
				messageItem.Recipients.Add(this.GroupParticipant);
			}
		}

		protected void OnAfterSaveOrSend(MessageItem messageItem)
		{
			this.conversationId = (ConversationId)PropertyCommand.GetPropertyValueFromStoreObject(messageItem, ItemSchema.ConversationId);
			this.EscalateIfNecessary(messageItem);
			if (this.GroupSession != null && this.GroupSession.ActivitySession != null)
			{
				this.GroupSession.ActivitySession.CaptureActivity(this.IsReplying ? ActivityId.ModernGroupsQuickReply : ActivityId.ModernGroupsQuickCompose, messageItem.StoreObjectId, null, base.CallContext.GetAccessingInformation());
			}
		}

		protected abstract void DisposeHelper();

		protected abstract EmailAddressWrapper[] GetToRecipients();

		protected abstract EmailAddressWrapper[] GetCcRecipients();

		protected TargetFolderId GetGroupFolderId()
		{
			return new TargetFolderId(new DistinguishedFolderId
			{
				Mailbox = this.ModernGroupEmailAddress,
				Id = DistinguishedFolderIdName.inbox
			});
		}

		protected void AddConversationToResponseItem(ItemType item)
		{
			if (item != null && this.conversationShapeName != null)
			{
				ConversationType conversation = Util.LoadConversationUsingConversationId(this.conversationId, this.conversationShapeName, this.GetGroupFolderId(), base.IdConverter, this.GetHashCode(), base.CallContext.ProtocolLog);
				item.Conversation = conversation;
			}
		}

		protected void MoveToInboxAndSendIfNeeded(ItemType responseItem)
		{
			ServiceResult<ItemType> serviceResult = this.MoveItemToInbox(responseItem.ItemId.Id);
			if (serviceResult != null && serviceResult.Value != null && serviceResult.Value.ItemId != null)
			{
				responseItem.ItemId = serviceResult.Value.ItemId;
				if (this.NeedSend && this.SendMessageAfterPostingDraft(responseItem.ItemId))
				{
					this.LoadUpdatedChangeKeyForItem(responseItem.ItemId);
				}
			}
		}

		private void LoadUpdatedChangeKeyForItem(ItemId itemId)
		{
			GetItemRequest getItemRequest = new GetItemRequest();
			getItemRequest.Ids = new BaseItemId[]
			{
				itemId
			};
			getItemRequest.ItemShape = new ItemResponseShape(ShapeEnum.IdOnly, BodyResponseType.Best, false, null);
			GetItem getItem = new GetItem(base.CallContext, getItemRequest);
			if (getItem.PreExecute())
			{
				ServiceResult<ItemType[]> serviceResult = getItem.Execute();
				ExTraceGlobals.ModernGroupsTracer.TraceDebug((long)this.GetHashCode(), "PostModernGroupItemBase.LoadUpdatedChangeKeyForItem: Execute completed.");
				if (serviceResult.Value != null && serviceResult.Value.Length > 0 && serviceResult.Value[0] != null && serviceResult.Value[0].ItemId != null)
				{
					itemId.ChangeKey = serviceResult.Value[0].ItemId.ChangeKey;
					return;
				}
			}
			else
			{
				ExTraceGlobals.ModernGroupsTracer.TraceError((long)this.GetHashCode(), "PostModernGroupItemBase.LoadUpdatedChangeKeyForItem: pre-execute failed.");
			}
		}

		private ServiceResult<ItemType> MoveItemToInbox(string itemId)
		{
			MoveItemRequest moveItemRequest = new MoveItemRequest();
			ServiceResult<ItemType> result = null;
			moveItemRequest.ToFolderId = this.GetGroupFolderId();
			moveItemRequest.ReturnNewItemIds = true;
			moveItemRequest.Ids = new BaseItemId[]
			{
				new ItemId(itemId, null)
			};
			MoveItem moveItem = new MoveItem(base.CallContext, moveItemRequest);
			if (moveItem.PreExecute())
			{
				result = moveItem.Execute();
				ExTraceGlobals.ModernGroupsTracer.TraceDebug((long)this.GetHashCode(), "PostModernGroupItemBase.MoveItemToInbox: Execute completed.");
			}
			else
			{
				ExTraceGlobals.ModernGroupsTracer.TraceError((long)this.GetHashCode(), "PostModernGroupItemBase.MoveItemToInbox: pre-execute failed.");
			}
			return result;
		}

		private bool SendMessageAfterPostingDraft(ItemId itemId)
		{
			SendItemRequest sendItemRequest = new SendItemRequest();
			ServiceResult<ServiceResultNone> serviceResult = null;
			sendItemRequest.Ids = new BaseItemId[]
			{
				itemId
			};
			sendItemRequest.SavedItemFolderId = this.GetGroupFolderId();
			sendItemRequest.SaveItemToFolder = true;
			SendItem sendItem = new SendItem(base.CallContext, sendItemRequest);
			sendItem.BeforeMessageDisposition += this.OnBeforeSend;
			if (sendItem.PreExecute())
			{
				serviceResult = sendItem.Execute();
				sendItem.BeforeMessageDisposition -= this.OnBeforeSend;
				ExTraceGlobals.ModernGroupsTracer.TraceDebug((long)this.GetHashCode(), "PostModernGroupItemBase.SendMessageAfterPostingDraft: Execute completed.");
				if (serviceResult.Error != null)
				{
					ExTraceGlobals.ModernGroupsTracer.TraceDebug<string>((long)this.GetHashCode(), "PostModernGroupItemBase.SendMessageAfterPostingDraft: Execute completed with error {0}", serviceResult.Error.MessageText);
				}
			}
			else
			{
				ExTraceGlobals.ModernGroupsTracer.TraceError((long)this.GetHashCode(), "PostModernGroupItemBase.SendMessageAfterPostingDraft: pre-execute failed.");
			}
			return serviceResult != null && serviceResult.Error == null;
		}

		private void EscalateIfNecessary(MessageItem messageItem)
		{
			if (!GroupEscalation.IsEscalationEnabled())
			{
				ExTraceGlobals.ModernGroupsTracer.Information((long)this.GetHashCode(), "COWGroupMessageEscalation.SkipItemOperation: skipping group message escalation as the feature is disabled.");
				return;
			}
			IGroupEscalationFlightInfo groupEscalationFlightInfo = new GroupEscalationFlightInfo(this.GroupSession.MailboxOwner.GetContext(null));
			GroupEscalation groupEscalation = new GroupEscalation(XSOFactory.Default, groupEscalationFlightInfo, new MailboxUrls(this.GroupSession.MailboxOwner, false));
			bool flag;
			groupEscalation.EscalateItem(messageItem, this.GroupSession, out flag, this.IsFromYammer);
		}

		private bool HasOtherRecipients(EmailAddressWrapper[] recipients)
		{
			if (recipients == null)
			{
				ExTraceGlobals.ModernGroupsTracer.TraceDebug((long)this.GetHashCode(), "PostModernGroupItem.HasOtherRecipients: recipient count is 0");
				return false;
			}
			for (int i = 0; i < recipients.Length; i++)
			{
				if (!this.IsGroupEmail(recipients[i]))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsGroupInRecipients(RecipientCollection recipients)
		{
			bool result = false;
			if (recipients != null && recipients.Contains(this.GroupParticipant))
			{
				result = true;
			}
			return result;
		}

		private bool IsGroupEmail(EmailAddressWrapper emailAddressWrapper)
		{
			bool flag = true;
			ProxyAddress proxyAddress;
			Participant participant;
			if (ProxyAddress.TryParse(emailAddressWrapper.EmailAddress, out proxyAddress))
			{
				flag = false;
				ADRecipient adrecipient = base.CallContext.ADRecipientSessionContext.GetADRecipientSession().FindByProxyAddress(proxyAddress);
				if (adrecipient == null)
				{
					ExTraceGlobals.ModernGroupsTracer.TraceDebug<string>((long)this.GetHashCode(), "PostModernGroupItemBase.IsGroupEmail: {0} can't be found in AD or AD cache", proxyAddress.AddressString);
					return false;
				}
				participant = new Participant(adrecipient);
				ExTraceGlobals.ModernGroupsTracer.TraceDebug<bool>((long)this.GetHashCode(), "PostModernGroupItemBase.IsGroupEmail: recipient.IsCached is {0}", adrecipient.IsCached);
			}
			else
			{
				participant = new Participant(emailAddressWrapper.OriginalDisplayName, emailAddressWrapper.EmailAddress, emailAddressWrapper.RoutingType);
				ExTraceGlobals.ModernGroupsTracer.TraceDebug<string>((long)this.GetHashCode(), "PostModernGroupItemBase.IsGroupEmail: {0} can't be parsed as ProxyAddress", emailAddressWrapper.EmailAddress);
			}
			if (flag)
			{
				base.CallContext.ProtocolLog.Set(PostModernGroupItemMetadata.MissedAdCache, true);
			}
			bool flag2 = Participant.HasSameEmail(this.groupParticipant, participant, this.GroupSession, flag);
			ExTraceGlobals.ModernGroupsTracer.TraceDebug<string, string, bool>((long)this.GetHashCode(), "PostModernGroupItemBase.IsGroupEmail: passed in email is {0}, group email is {1}, isGroupEmail = {2}", emailAddressWrapper.ToString(), this.groupParticipant.ToString(), flag2);
			return flag2;
		}

		private Participant GetMeParticipant()
		{
			EmailAddressWrapper address = ResolveNames.EmailAddressWrapperFromRecipient(base.CallContext.AccessingADUser);
			return MailboxHelper.GetParticipantFromAddress(address);
		}

		private bool needSend;

		private bool onBeforeSaveOrSendCalled;

		private readonly DisposeTracker disposeTracker;

		private EmailAddressWrapper[] toRecipients;

		private EmailAddressWrapper[] ccRecipients;

		private Participant groupParticipant;

		private EmailAddressWrapper modernGroupEmailAddress;

		private ConversationId conversationId;

		private string conversationShapeName;
	}
}
