using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CopyItem : MoveCopyItemCommandBase
	{
		public CopyItem(CallContext callContext, CopyItemRequest request) : base(callContext, request)
		{
		}

		protected override BaseInfoResponse CreateResponse()
		{
			return new CopyItemResponse();
		}

		protected override AggregateOperationResult DoOperation(StoreSession destinationSession, StoreSession sourceSession, StoreId sourceId)
		{
			return sourceSession.Copy(destinationSession, this.destinationFolder.Id, sourceSession == destinationSession, new StoreId[]
			{
				sourceId
			});
		}
	}
}
