using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateMessageCommand : ExchangeServiceCommand<CreateMessageRequest, CreateMessageResponse>
	{
		public CreateMessageCommand(CreateMessageRequest request) : base(request)
		{
		}

		protected override CreateMessageResponse InternalExecute()
		{
			MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
			Message result = messageProvider.Create(base.Request.ParentFolderId, base.Request.Template, base.Request.MessageDisposition);
			return new CreateMessageResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
