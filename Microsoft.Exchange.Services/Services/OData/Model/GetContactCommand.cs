using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetContactCommand : ExchangeServiceCommand<GetContactRequest, GetContactResponse>
	{
		public GetContactCommand(GetContactRequest request) : base(request)
		{
		}

		protected override GetContactResponse InternalExecute()
		{
			ContactProvider contactProvider = new ContactProvider(base.ExchangeService);
			Contact result = contactProvider.Read(base.Request.Id, new ContactQueryAdapter(ContactSchema.SchemaInstance, base.Request.ODataQueryOptions));
			return new GetContactResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
