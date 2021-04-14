using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindFoldersCommand : ExchangeServiceCommand<FindFoldersRequest, FindFoldersResponse>
	{
		public FindFoldersCommand(FindFoldersRequest request) : base(request)
		{
		}

		protected override FindFoldersResponse InternalExecute()
		{
			FolderProvider folderProvider = new FolderProvider(base.ExchangeService);
			IFindEntitiesResult<Folder> findEntitiesResult = folderProvider.Find(base.Request.ParentFolderId, new FolderQueryAdapter(FolderSchema.SchemaInstance, base.Request.ODataQueryOptions));
			if (base.Request.ODataQueryOptions.Expands(FolderSchema.ChildFolders.Name))
			{
				foreach (Folder folder in findEntitiesResult)
				{
					folder.ChildFolders = folderProvider.Find(folder.Id, null);
				}
			}
			if (base.Request.ODataQueryOptions.Expands(FolderSchema.Messages.Name))
			{
				MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
				foreach (Folder folder2 in findEntitiesResult)
				{
					folder2.Messages = messageProvider.Find(folder2.Id, null);
				}
			}
			return new FindFoldersResponse(base.Request)
			{
				Result = findEntitiesResult
			};
		}
	}
}
