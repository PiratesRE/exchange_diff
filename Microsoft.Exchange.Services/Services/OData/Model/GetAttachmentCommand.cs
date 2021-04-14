using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetAttachmentCommand : ExchangeServiceCommand<GetAttachmentRequest, GetAttachmentResponse>
	{
		public GetAttachmentCommand(GetAttachmentRequest request) : base(request)
		{
		}

		protected override GetAttachmentResponse InternalExecute()
		{
			AttachmentProvider attachmentProvider = new AttachmentProvider(base.ExchangeService);
			AttachmentSchema entitySchema = base.Request.ODataContext.EntityType.GetSchema() as AttachmentSchema;
			Attachment result = attachmentProvider.Read(base.Request.RootItemId, base.Request.Id, new AttachmentQueryAdapter(entitySchema, base.Request.ODataQueryOptions));
			return new GetAttachmentResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
