using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateMessageResponseDraftCommand : ExchangeServiceCommand<CreateMessageResponseDraftRequest, CreateMessageResponseDraftResponse>
	{
		public CreateMessageResponseDraftCommand(CreateMessageResponseDraftRequest request) : base(request)
		{
		}

		protected override CreateMessageResponseDraftResponse InternalExecute()
		{
			MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
			Message result = messageProvider.PerformMessageResponseAction(base.Request.Id, base.Request.ResponseType, false, null, null);
			return new CreateMessageResponseDraftResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
