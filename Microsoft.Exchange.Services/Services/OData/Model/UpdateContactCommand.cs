using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateContactCommand : ExchangeServiceCommand<UpdateContactRequest, UpdateContactResponse>
	{
		public UpdateContactCommand(UpdateContactRequest request) : base(request)
		{
		}

		protected override UpdateContactResponse InternalExecute()
		{
			ContactProvider contactProvider = new ContactProvider(base.ExchangeService);
			Contact result = contactProvider.Update(base.Request.Id, base.Request.Change, base.Request.ChangeKey);
			return new UpdateContactResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
