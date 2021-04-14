using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteFolderCommand : ExchangeServiceCommand<DeleteFolderRequest, DeleteFolderResponse>
	{
		public DeleteFolderCommand(DeleteFolderRequest request) : base(request)
		{
		}

		protected override DeleteFolderResponse InternalExecute()
		{
			FolderProvider folderProvider = new FolderProvider(base.ExchangeService);
			folderProvider.Delete(base.Request.Id);
			return new DeleteFolderResponse(base.Request);
		}
	}
}
