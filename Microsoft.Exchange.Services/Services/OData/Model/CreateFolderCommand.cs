using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateFolderCommand : ExchangeServiceCommand<CreateFolderRequest, CreateFolderResponse>
	{
		public CreateFolderCommand(CreateFolderRequest request) : base(request)
		{
		}

		protected override CreateFolderResponse InternalExecute()
		{
			FolderProvider folderProvider = new FolderProvider(base.ExchangeService);
			Folder result = folderProvider.Create(base.Request.ParentFolderId, base.Request.Template);
			return new CreateFolderResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
