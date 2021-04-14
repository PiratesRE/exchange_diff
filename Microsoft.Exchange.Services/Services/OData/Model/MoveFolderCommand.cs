using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class MoveFolderCommand : ExchangeServiceCommand<MoveFolderRequest, MoveFolderResponse>
	{
		public MoveFolderCommand(MoveFolderRequest request) : base(request)
		{
		}

		protected override MoveFolderResponse InternalExecute()
		{
			FolderProvider folderProvider = new FolderProvider(base.ExchangeService);
			Folder result = folderProvider.Move(base.Request.Id, base.Request.DestinationId);
			return new MoveFolderResponse(base.Request)
			{
				Result = result
			};
		}
	}
}
