using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CopyMessageCommand : ExchangeServiceCommand<CopyMessageRequest, CopyMessageResponse>
	{
		public CopyMessageCommand(CopyMessageRequest request) : base(request)
		{
		}

		protected override CopyMessageResponse InternalExecute()
		{
			MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
			Message result = messageProvider.Copy(base.Request.Id, base.Request.DestinationId);
			return new CopyMessageResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
