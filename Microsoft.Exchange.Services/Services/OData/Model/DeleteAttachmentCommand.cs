using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteAttachmentCommand : ExchangeServiceCommand<DeleteAttachmentRequest, DeleteAttachmentResponse>
	{
		public DeleteAttachmentCommand(DeleteAttachmentRequest request) : base(request)
		{
		}

		protected override DeleteAttachmentResponse InternalExecute()
		{
			AttachmentProvider attachmentProvider = new AttachmentProvider(base.ExchangeService);
			attachmentProvider.Delete(base.Request.RootItemId, base.Request.Id);
			return new DeleteAttachmentResponse(base.Request);
		}
	}
}
