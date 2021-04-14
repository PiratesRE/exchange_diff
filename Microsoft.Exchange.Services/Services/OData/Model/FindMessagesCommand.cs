using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindMessagesCommand : ExchangeServiceCommand<FindMessagesRequest, FindMessagesResponse>
	{
		public FindMessagesCommand(FindMessagesRequest request) : base(request)
		{
		}

		protected override FindMessagesResponse InternalExecute()
		{
			MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
			MessageQueryAdapter queryAdapter = new MessageQueryAdapter(MessageSchema.SchemaInstance, base.Request.ODataQueryOptions);
			IFindEntitiesResult<Message> result = messageProvider.Find(base.Request.ParentFolderId, queryAdapter);
			return new FindMessagesResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
