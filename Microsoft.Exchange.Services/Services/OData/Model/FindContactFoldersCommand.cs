using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindContactFoldersCommand : ExchangeServiceCommand<FindContactFoldersRequest, FindContactFoldersResponse>
	{
		public FindContactFoldersCommand(FindContactFoldersRequest request) : base(request)
		{
		}

		protected override FindContactFoldersResponse InternalExecute()
		{
			ContactFolderProvider contactFolderProvider = new ContactFolderProvider(base.ExchangeService);
			IFindEntitiesResult<ContactFolder> findEntitiesResult = contactFolderProvider.Find(base.Request.ParentFolderId, new ContactFolderQueryAdapter(ContactFolderSchema.SchemaInstance, base.Request.ODataQueryOptions));
			if (base.Request.ODataQueryOptions.Expands(ContactFolderSchema.ChildFolders.Name))
			{
				foreach (ContactFolder contactFolder in findEntitiesResult)
				{
					contactFolder.ChildFolders = contactFolderProvider.Find(contactFolder.Id, null);
				}
			}
			if (base.Request.ODataQueryOptions.Expands(ContactFolderSchema.Contacts.Name))
			{
				ContactProvider contactProvider = new ContactProvider(base.ExchangeService);
				foreach (ContactFolder contactFolder2 in findEntitiesResult)
				{
					contactFolder2.Contacts = contactProvider.Find(contactFolder2.Id, null);
				}
			}
			return new FindContactFoldersResponse(base.Request)
			{
				Result = findEntitiesResult
			};
		}
	}
}
