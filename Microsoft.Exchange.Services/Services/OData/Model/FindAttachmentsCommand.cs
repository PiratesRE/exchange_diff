using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindAttachmentsCommand : ExchangeServiceCommand<FindAttachmentsRequest, FindAttachmentsResponse>
	{
		public FindAttachmentsCommand(FindAttachmentsRequest request) : base(request)
		{
		}

		protected override FindAttachmentsResponse InternalExecute()
		{
			AttachmentProvider attachmentProvider = new AttachmentProvider(base.ExchangeService);
			AttachmentSchema entitySchema = base.Request.ODataContext.EntityType.GetSchema() as AttachmentSchema;
			IFindEntitiesResult<Attachment> result = attachmentProvider.Find(base.Request.RootItemId, new AttachmentQueryAdapter(entitySchema, base.Request.ODataQueryOptions));
			return new FindAttachmentsResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
