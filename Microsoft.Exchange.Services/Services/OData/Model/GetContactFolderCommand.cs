using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetContactFolderCommand : ExchangeServiceCommand<GetContactFolderRequest, GetContactFolderResponse>
	{
		public GetContactFolderCommand(GetContactFolderRequest request) : base(request)
		{
		}

		protected override GetContactFolderResponse InternalExecute()
		{
			ContactFolderProvider contactFolderProvider = new ContactFolderProvider(base.ExchangeService);
			ContactFolder contactFolder = contactFolderProvider.Read(base.Request.Id, new ContactFolderQueryAdapter(ContactFolderSchema.SchemaInstance, base.Request.ODataQueryOptions));
			if (base.Request.ODataQueryOptions.Expands(ContactFolderSchema.ChildFolders.Name))
			{
				contactFolder.ChildFolders = contactFolderProvider.Find(contactFolder.Id, null);
			}
			if (base.Request.ODataQueryOptions.Expands(ContactFolderSchema.ChildFolders.Name))
			{
				ContactProvider contactProvider = new ContactProvider(base.ExchangeService);
				contactFolder.Contacts = contactProvider.Find(contactFolder.Id, null);
			}
			return new GetContactFolderResponse(base.Request)
			{
				Result = contactFolder
			};
		}
	}
}
