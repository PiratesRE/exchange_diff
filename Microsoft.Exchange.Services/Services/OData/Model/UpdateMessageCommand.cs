using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateMessageCommand : ExchangeServiceCommand<UpdateMessageRequest, UpdateMessageResponse>
	{
		public UpdateMessageCommand(UpdateMessageRequest request) : base(request)
		{
		}

		protected override UpdateMessageResponse InternalExecute()
		{
			MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
			Message result = messageProvider.Update(base.Request.Id, base.Request.Change, base.Request.ChangeKey);
			return new UpdateMessageResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
