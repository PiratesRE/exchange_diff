using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetFolderCommand : ExchangeServiceCommand<GetFolderRequest, GetFolderResponse>
	{
		public GetFolderCommand(GetFolderRequest request) : base(request)
		{
		}

		protected override GetFolderResponse InternalExecute()
		{
			FolderProvider folderProvider = new FolderProvider(base.ExchangeService);
			Folder folder = folderProvider.Read(base.Request.Id, new FolderQueryAdapter(FolderSchema.SchemaInstance, base.Request.ODataQueryOptions));
			if (base.Request.ODataQueryOptions.Expands(FolderSchema.ChildFolders.Name))
			{
				folder.ChildFolders = folderProvider.Find(folder.Id, null);
			}
			if (base.Request.ODataQueryOptions.Expands(FolderSchema.Messages.Name))
			{
				MessageProvider messageProvider = new MessageProvider(base.ExchangeService);
				folder.Messages = messageProvider.Find(folder.Id, null);
			}
			return new GetFolderResponse(base.Request)
			{
				Result = folder
			};
		}
	}
}
