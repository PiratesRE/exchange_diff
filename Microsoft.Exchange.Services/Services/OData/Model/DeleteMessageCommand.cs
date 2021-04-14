using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteMessageCommand : ExchangeServiceCommand<DeleteMessageRequest, DeleteMessageResponse>
	{
		public DeleteMessageCommand(DeleteMessageRequest request) : base(request)
		{
		}

		protected override DeleteMessageResponse InternalExecute()
		{
			MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
			messageProvider.Delete(base.Request.Id);
			return new DeleteMessageResponse(base.Request);
		}
	}
}
