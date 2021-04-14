using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CopyFolderCommand : ExchangeServiceCommand<CopyFolderRequest, CopyFolderResponse>
	{
		public CopyFolderCommand(CopyFolderRequest request) : base(request)
		{
		}

		protected override CopyFolderResponse InternalExecute()
		{
			FolderProvider folderProvider = new FolderProvider(base.ExchangeService);
			Folder result = folderProvider.Copy(base.Request.Id, base.Request.DestinationId);
			return new CopyFolderResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
