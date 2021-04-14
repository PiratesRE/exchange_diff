using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateContactCommand : ExchangeServiceCommand<CreateContactRequest, CreateContactResponse>
	{
		public CreateContactCommand(CreateContactRequest request) : base(request)
		{
		}

		protected override CreateContactResponse InternalExecute()
		{
			ContactProvider contactProvider = new ContactProvider(base.ExchangeService);
			Contact result = contactProvider.Create(base.Request.ParentFolderId, base.Request.Template);
			return new CreateContactResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
