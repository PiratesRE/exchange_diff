using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateFolderCommand : ExchangeServiceCommand<UpdateFolderRequest, UpdateFolderResponse>
	{
		public UpdateFolderCommand(UpdateFolderRequest request) : base(request)
		{
		}

		protected override UpdateFolderResponse InternalExecute()
		{
			FolderProvider folderProvider = new FolderProvider(base.ExchangeService);
			Folder result = folderProvider.Update(base.Request.Id, base.Request.Change);
			return new UpdateFolderResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
