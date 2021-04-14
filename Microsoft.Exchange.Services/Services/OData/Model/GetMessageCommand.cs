using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetMessageCommand : ExchangeServiceCommand<GetMessageRequest, GetMessageResponse>
	{
		public GetMessageCommand(GetMessageRequest request) : base(request)
		{
		}

		protected override GetMessageResponse InternalExecute()
		{
			MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
			Message result = messageProvider.Read(base.Request.Id, new MessageQueryAdapter(MessageSchema.SchemaInstance, base.Request.ODataQueryOptions));
			return new GetMessageResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
