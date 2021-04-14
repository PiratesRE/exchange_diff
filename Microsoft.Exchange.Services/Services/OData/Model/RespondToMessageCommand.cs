using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class RespondToMessageCommand : ExchangeServiceCommand<RespondToMessageRequest, RespondToMessageResponse>
	{
		public RespondToMessageCommand(RespondToMessageRequest request) : base(request)
		{
		}

		protected override RespondToMessageResponse InternalExecute()
		{
			MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
			messageProvider.PerformMessageResponseAction(base.Request.Id, base.Request.ResponseType, true, base.Request.Comment, base.Request.ToRecipients);
			return new RespondToMessageResponse(base.Request);
		}
	}
}
