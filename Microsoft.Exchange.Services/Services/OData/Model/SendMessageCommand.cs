using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class SendMessageCommand : ExchangeServiceCommand<SendMessageRequest, SendMessageResponse>
	{
		public SendMessageCommand(SendMessageRequest request) : base(request)
		{
		}

		protected override SendMessageResponse InternalExecute()
		{
			MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
			messageProvider.SendDraft(base.Request.Id);
			return new SendMessageResponse(base.Request);
		}
	}
}
