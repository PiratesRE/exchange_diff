using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CopyFolder : MoveCopyFolderCommandBase
	{
		public CopyFolder(CallContext callContext, CopyFolderRequest request) : base(callContext, request)
		{
		}

		protected override BaseInfoResponse CreateResponse()
		{
			return new CopyFolderResponse();
		}

		protected override AggregateOperationResult DoOperation(StoreSession destinationSession, StoreSession sourceSession, StoreId sourceId)
		{
			return sourceSession.Copy(destinationSession, this.destinationFolder.Id, new StoreId[]
			{
				sourceId
			});
		}

		protected override void SubclassValidateOperation(StoreSession storeSession, IdAndSession idAndSession)
		{
			if (idAndSession.Session is PublicFolderSession || this.destinationFolder.Session is PublicFolderSession)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)4177991609U);
			}
		}
	}
}
