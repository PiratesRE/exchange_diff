using System;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class PostModernGroupItem : PostModernGroupItemBase<PostModernGroupItemRequest, ItemType[]>
	{
		public PostModernGroupItem(CallContext callContext, PostModernGroupItemRequest request) : base(callContext, request)
		{
			base.ModernGroupEmailAddress = request.ModernGroupEmailAddress;
			this.currentMessageType = (base.Request.Items.Items[base.CurrentStep] as MessageType);
			OwsLogRegistry.Register(PostModernGroupItem.PostModernGroupItemActionName, typeof(PostModernGroupItemMetadata), new Type[0]);
			if (this.currentMessageType is SmartResponseType)
			{
				this.isReplying = true;
				this.isReplyingUsingDraft = (((SmartResponseType)this.currentMessageType).UpdateResponseItemId != null);
			}
		}

		internal override int StepCount
		{
			get
			{
				return base.Request.Items.Items.Length;
			}
		}

		protected override bool IsReplying
		{
			get
			{
				return this.isReplying;
			}
		}

		private bool ShouldUseSaveAndSendDispositionForPost
		{
			get
			{
				return base.NeedSend && !this.isReplyingUsingDraft;
			}
		}

		internal override void PreExecuteCommand()
		{
			DateTime utcNow = DateTime.UtcNow;
			base.PreExecuteCommand();
			if (base.Request.ItemShape != null)
			{
				base.ConversationShapeName = base.Request.ItemShape.ConversationShapeName;
				base.Request.ItemShape.ConversationShapeName = null;
			}
			CreateItemRequest createItemRequest = new CreateItemRequest();
			createItemRequest.Items = this.CreateMessageItemsForPost();
			createItemRequest.SavedItemFolderId = base.GetGroupFolderId();
			createItemRequest.IsNonDraft = true;
			if (base.Request.ItemShape != null || base.Request.ShapeName != null)
			{
				createItemRequest.ItemShape = base.Request.ItemShape;
				createItemRequest.ShapeName = base.Request.ShapeName;
			}
			else
			{
				createItemRequest.ShapeName = "QuickComposeItemPart";
				createItemRequest.ItemShape = ServiceCommandBase.DefaultItemResponseShape;
			}
			this.createItemServiceCommand = new CreateItem(base.CallContext, createItemRequest);
			this.createItemServiceCommand.BeforeMessageDisposition += base.OnBeforeSaveOrSend;
			this.createItemServiceCommand.AfterMessageDisposition += base.OnAfterSaveOrSend;
			if (this.ShouldUseSaveAndSendDispositionForPost)
			{
				createItemRequest.MessageDisposition = "SendAndSaveCopy";
				this.createItemServiceCommand.BeforeMessageDisposition += base.OnBeforeSend;
			}
			else
			{
				createItemRequest.MessageDisposition = "SaveOnly";
			}
			DateTime utcNow2 = DateTime.UtcNow;
			base.CallContext.ProtocolLog.Set(PostModernGroupItemMetadata.PreExecuteCommandLatency, (utcNow2 - utcNow).Milliseconds);
			this.createItemServiceCommand.PreExecute();
		}

		internal override ServiceResult<ItemType[]> Execute()
		{
			ServiceResult<ItemType[]> result;
			try
			{
				ServiceResult<ItemType[]> serviceResult = this.createItemServiceCommand.Execute();
				if (this.ShouldUseSaveAndSendDispositionForPost)
				{
					this.createItemServiceCommand.BeforeMessageDisposition -= base.OnBeforeSend;
				}
				this.createItemServiceCommand.BeforeMessageDisposition -= base.OnBeforeSaveOrSend;
				this.createItemServiceCommand.AfterMessageDisposition -= base.OnAfterSaveOrSend;
				if (serviceResult.Value != null && serviceResult.Value.Length > 0 && serviceResult.Value[0] != null)
				{
					if (this.isReplyingUsingDraft)
					{
						base.MoveToInboxAndSendIfNeeded(serviceResult.Value[0]);
					}
					base.AddConversationToResponseItem(serviceResult.Value[0]);
				}
				result = serviceResult;
			}
			catch (Exception)
			{
				this.LogExceptionContext();
				throw;
			}
			return result;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			PostModernGroupItemResponse postModernGroupItemResponse = new PostModernGroupItemResponse();
			postModernGroupItemResponse.BuildForResults<ItemType[]>(base.Results);
			return postModernGroupItemResponse;
		}

		protected override EmailAddressWrapper[] GetToRecipients()
		{
			return this.currentMessageType.ToRecipients;
		}

		protected override EmailAddressWrapper[] GetCcRecipients()
		{
			return this.currentMessageType.CcRecipients;
		}

		protected override void DisposeHelper()
		{
			if (this.createItemServiceCommand != null)
			{
				this.createItemServiceCommand.Dispose();
				this.createItemServiceCommand = null;
			}
		}

		private NonEmptyArrayOfAllItemsType CreateMessageItemsForPost()
		{
			NonEmptyArrayOfAllItemsType nonEmptyArrayOfAllItemsType = new NonEmptyArrayOfAllItemsType();
			nonEmptyArrayOfAllItemsType.Add(this.currentMessageType);
			this.SetReferenceItemIdForYammerInterop();
			return nonEmptyArrayOfAllItemsType;
		}

		private void SetReferenceItemIdForYammerInterop()
		{
			if (base.IsFromYammer)
			{
				SmartResponseType smartResponseType = this.currentMessageType as SmartResponseType;
				if (smartResponseType != null && smartResponseType.ReferenceItemId == null && !string.IsNullOrEmpty(smartResponseType.InReplyTo))
				{
					IStorePropertyBag[] array = AllItemsFolderHelper.FindItemsFromInternetId(base.GroupSession, smartResponseType.InReplyTo, new StorePropertyDefinition[]
					{
						CoreItemSchema.Id
					});
					if (array != null && array.Any<IStorePropertyBag>())
					{
						VersionedId valueOrDefault = array[0].GetValueOrDefault<VersionedId>(CoreItemSchema.Id, null);
						if (valueOrDefault != null && valueOrDefault.ObjectId != null)
						{
							ItemId itemIdFromStoreId = IdConverter.GetItemIdFromStoreId(valueOrDefault, new MailboxId(base.GroupSession));
							smartResponseType.ReferenceItemId = itemIdFromStoreId;
						}
					}
				}
			}
		}

		private void LogExceptionContext()
		{
			base.CallContext.ProtocolLog.Set(PostModernGroupItemMetadata.ConversationTopic, this.currentMessageType.Subject);
			base.CallContext.ProtocolLog.Set(PostModernGroupItemMetadata.ConversationId, base.ConversationId);
			base.CallContext.ProtocolLog.Set(PostModernGroupItemMetadata.ToRecipientCount, (base.ToRecipients == null) ? 0 : base.ToRecipients.Count<EmailAddressWrapper>());
			base.CallContext.ProtocolLog.Set(PostModernGroupItemMetadata.CcRecipientCount, (base.CcRecipients == null) ? 0 : base.CcRecipients.Count<EmailAddressWrapper>());
			base.CallContext.ProtocolLog.Set(PostModernGroupItemMetadata.IsReplying, this.isReplying);
			base.CallContext.ProtocolLog.Set(PostModernGroupItemMetadata.IsReplyingUsingDraft, this.isReplyingUsingDraft);
		}

		private const string DefaultShapeName = "QuickComposeItemPart";

		private static readonly string PostModernGroupItemActionName = typeof(PostModernGroupItem).Name;

		private readonly MessageType currentMessageType;

		private readonly bool isReplyingUsingDraft;

		private readonly bool isReplying;

		private CreateItem createItemServiceCommand;
	}
}
