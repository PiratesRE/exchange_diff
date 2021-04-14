using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class MoveMessageCommand : ExchangeServiceCommand<MoveMessageRequest, MoveMessageResponse>
	{
		public MoveMessageCommand(MoveMessageRequest request) : base(request)
		{
		}

		protected override MoveMessageResponse InternalExecute()
		{
			MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
			Message result = messageProvider.Move(base.Request.Id, base.Request.DestinationId);
			return new MoveMessageResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
