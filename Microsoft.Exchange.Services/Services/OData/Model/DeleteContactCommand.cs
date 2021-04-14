using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteContactCommand : ExchangeServiceCommand<DeleteContactRequest, DeleteContactResponse>
	{
		public DeleteContactCommand(DeleteContactRequest request) : base(request)
		{
		}

		protected override DeleteContactResponse InternalExecute()
		{
			ContactProvider contactProvider = new ContactProvider(base.ExchangeService);
			contactProvider.Delete(base.Request.Id);
			return new DeleteContactResponse(base.Request);
		}
	}
}
