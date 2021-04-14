using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindContactsCommand : ExchangeServiceCommand<FindContactsRequest, FindContactsResponse>
	{
		public FindContactsCommand(FindContactsRequest request) : base(request)
		{
		}

		protected override FindContactsResponse InternalExecute()
		{
			ContactProvider contactProvider = new ContactProvider(base.ExchangeService);
			ContactQueryAdapter queryAdapter = new ContactQueryAdapter(ContactSchema.SchemaInstance, base.Request.ODataQueryOptions);
			IFindEntitiesResult<Contact> result = contactProvider.Find(base.Request.ParentFolderId, queryAdapter);
			return new FindContactsResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
