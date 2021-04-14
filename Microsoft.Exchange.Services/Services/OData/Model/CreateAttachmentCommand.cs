using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateAttachmentCommand : ExchangeServiceCommand<CreateAttachmentRequest, CreateAttachmentResponse>
	{
		public CreateAttachmentCommand(CreateAttachmentRequest request) : base(request)
		{
		}

		protected override CreateAttachmentResponse InternalExecute()
		{
			AttachmentProvider attachmentProvider = new AttachmentProvider(base.ExchangeService);
			Attachment result = attachmentProvider.Create(base.Request.RootItemId, base.Request.Template);
			return new CreateAttachmentResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
