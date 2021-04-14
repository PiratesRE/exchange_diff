using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class UpdateAndPostModernGroupItem : PostModernGroupItemBase<UpdateAndPostModernGroupItemRequest, UpdateItemResponseWrapper>
	{
		public UpdateAndPostModernGroupItem(CallContext callContext, UpdateAndPostModernGroupItemRequest request) : base(callContext, request)
		{
			base.ModernGroupEmailAddress = request.ModernGroupEmailAddress;
			OwsLogRegistry.Register(UpdateAndPostModernGroupItem.UpdateAndPostModernGroupItemActionName, typeof(UpdateAndPostModernGroupItemMetadata), new Type[0]);
		}

		internal override int StepCount
		{
			get
			{
				if (base.Request.ItemChanges != null)
				{
					return base.Request.ItemChanges.Length;
				}
				return 0;
			}
		}

		internal override void PreExecuteCommand()
		{
			base.PreExecuteCommand();
			if (base.Request.ItemShape != null)
			{
				base.ConversationShapeName = base.Request.ItemShape.ConversationShapeName;
				base.Request.ItemShape.ConversationShapeName = null;
			}
			UpdateItemRequest updateItemRequest = new UpdateItemRequest();
			updateItemRequest.ItemChanges = base.Request.ItemChanges;
			updateItemRequest.MessageDisposition = "SaveOnly";
			updateItemRequest.InternetMessageId = base.Request.InternetMessageId;
			if (base.Request.ItemShape != null || base.Request.ShapeName != null)
			{
				updateItemRequest.ItemShape = base.Request.ItemShape;
				updateItemRequest.ShapeName = base.Request.ShapeName;
			}
			this.updateItemServiceCommand = new UpdateItem(base.CallContext, updateItemRequest);
			this.updateItemServiceCommand.BeforeMessageDisposition += base.OnBeforeSaveOrSend;
			this.updateItemServiceCommand.AfterMessageDisposition += base.OnAfterSaveOrSend;
			this.updateItemServiceCommand.PreExecute();
		}

		internal override ServiceResult<UpdateItemResponseWrapper> Execute()
		{
			ServiceResult<UpdateItemResponseWrapper> result;
			try
			{
				ServiceResult<UpdateItemResponseWrapper> serviceResult = this.updateItemServiceCommand.Execute();
				this.updateItemServiceCommand.BeforeMessageDisposition -= base.OnBeforeSaveOrSend;
				this.updateItemServiceCommand.AfterMessageDisposition -= base.OnAfterSaveOrSend;
				if (serviceResult.Value != null && serviceResult.Value.Item != null)
				{
					base.MoveToInboxAndSendIfNeeded(serviceResult.Value.Item);
					base.AddConversationToResponseItem(serviceResult.Value.Item);
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
			UpdateAndPostModernGroupItemResponse updateAndPostModernGroupItemResponse = new UpdateAndPostModernGroupItemResponse();
			updateAndPostModernGroupItemResponse.BuildForUpdateItemResults(base.Results);
			return updateAndPostModernGroupItemResponse;
		}

		protected override void DisposeHelper()
		{
			if (this.updateItemServiceCommand != null)
			{
				this.updateItemServiceCommand = null;
			}
		}

		protected override EmailAddressWrapper[] GetToRecipients()
		{
			List<EmailAddressWrapper> list = new List<EmailAddressWrapper>();
			foreach (PropertyUpdate propertyUpdate in base.Request.ItemChanges[base.CurrentStep].PropertyUpdates)
			{
				SetItemPropertyUpdate setItemPropertyUpdate = propertyUpdate as SetItemPropertyUpdate;
				if (setItemPropertyUpdate != null)
				{
					MessageType messageType = setItemPropertyUpdate.Item as MessageType;
					if (messageType != null && messageType.ToRecipients != null)
					{
						list.AddRange(messageType.ToRecipients);
					}
				}
			}
			return list.ToArray();
		}

		protected override EmailAddressWrapper[] GetCcRecipients()
		{
			List<EmailAddressWrapper> list = new List<EmailAddressWrapper>();
			foreach (PropertyUpdate propertyUpdate in base.Request.ItemChanges[base.CurrentStep].PropertyUpdates)
			{
				SetItemPropertyUpdate setItemPropertyUpdate = propertyUpdate as SetItemPropertyUpdate;
				if (setItemPropertyUpdate != null)
				{
					MessageType messageType = setItemPropertyUpdate.Item as MessageType;
					if (messageType != null && messageType.CcRecipients != null)
					{
						list.AddRange(messageType.CcRecipients);
					}
				}
			}
			return list.ToArray();
		}

		private string GetConversationTopic()
		{
			foreach (PropertyUpdate propertyUpdate in base.Request.ItemChanges[base.CurrentStep].PropertyUpdates)
			{
				SetItemPropertyUpdate setItemPropertyUpdate = propertyUpdate as SetItemPropertyUpdate;
				if (setItemPropertyUpdate != null)
				{
					MessageType messageType = setItemPropertyUpdate.Item as MessageType;
					if (messageType != null && messageType.Subject != null)
					{
						return messageType.Subject;
					}
				}
			}
			return null;
		}

		private void LogExceptionContext()
		{
			base.CallContext.ProtocolLog.Set(UpdateAndPostModernGroupItemMetadata.ConversationTopic, this.GetConversationTopic());
			base.CallContext.ProtocolLog.Set(UpdateAndPostModernGroupItemMetadata.ConversationId, base.ConversationId);
			base.CallContext.ProtocolLog.Set(UpdateAndPostModernGroupItemMetadata.ToRecipientCount, (base.ToRecipients == null) ? 0 : base.ToRecipients.Count<EmailAddressWrapper>());
			base.CallContext.ProtocolLog.Set(UpdateAndPostModernGroupItemMetadata.CcRecipientCount, (base.CcRecipients == null) ? 0 : base.CcRecipients.Count<EmailAddressWrapper>());
		}

		private static readonly string UpdateAndPostModernGroupItemActionName = typeof(UpdateAndPostModernGroupItem).Name;

		private UpdateItem updateItemServiceCommand;
	}
}
