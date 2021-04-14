using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class DeleteAttachment : MultiStepServiceCommand<DeleteAttachmentRequest, RootItemIdType>
	{
		public DeleteAttachment(CallContext callContext, DeleteAttachmentRequest request) : base(callContext, request)
		{
			this.attachmentIds = base.Request.AttachmentIds;
			ServiceCommandBase.ThrowIfNullOrEmpty<AttachmentIdType>(this.attachmentIds, "attachmentIds", "DeleteAttachment::Execute");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			foreach (ServiceResult<RootItemIdType> serviceResult in base.Results)
			{
				if (serviceResult != null && serviceResult.Code == ServiceResultCode.Success)
				{
					serviceResult.Value.RootItemChangeKey = this.rootItemIds[serviceResult.Value.RootItemId];
				}
			}
			DeleteAttachmentResponse deleteAttachmentResponse = new DeleteAttachmentResponse();
			deleteAttachmentResponse.BuildForResults<RootItemIdType>(base.Results);
			return deleteAttachmentResponse;
		}

		internal override int StepCount
		{
			get
			{
				return this.attachmentIds.Length;
			}
		}

		internal override ServiceResult<RootItemIdType> Execute()
		{
			IdAndSession idAndSession = base.IdConverter.ConvertAttachmentIdToIdAndSessionReadOnly(this.attachmentIds[base.CurrentStep]);
			RootItemIdType rootItemIdType = new RootItemIdType();
			using (AttachmentHierarchy attachmentHierarchy = new AttachmentHierarchy(idAndSession, true, base.Request.ClientSupportsIrm))
			{
				if (attachmentHierarchy.Last.Attachment.IsContactPhoto)
				{
					attachmentHierarchy.RootItem[ContactSchema.HasPicturePropertyDef] = false;
				}
				attachmentHierarchy.DeleteLast();
				attachmentHierarchy.SaveAll();
				attachmentHierarchy.RootItem.Load();
				ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(attachmentHierarchy.RootItem.Id, idAndSession, null);
				this.rootItemIds[concatenatedId.Id] = concatenatedId.ChangeKey;
				rootItemIdType.RootItemId = concatenatedId.Id;
			}
			return new ServiceResult<RootItemIdType>(rootItemIdType);
		}

		private AttachmentIdType[] attachmentIds;

		private Dictionary<string, string> rootItemIds = new Dictionary<string, string>();
	}
}
