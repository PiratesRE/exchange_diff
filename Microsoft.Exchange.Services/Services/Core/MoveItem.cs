using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class MoveItem : MoveCopyItemCommandBase
	{
		public MoveItem(CallContext callContext, MoveItemRequest request) : base(callContext, request)
		{
		}

		internal override TimeSpan? MaxExecutionTime
		{
			get
			{
				return new TimeSpan?(TimeSpan.FromMinutes(5.0));
			}
		}

		protected override BaseInfoResponse CreateResponse()
		{
			return new MoveItemResponse();
		}

		protected override AggregateOperationResult DoOperation(StoreSession destinationSession, StoreSession sourceSession, StoreId sourceId)
		{
			return sourceSession.Move(destinationSession, this.destinationFolder.Id, sourceSession == destinationSession, new StoreId[]
			{
				sourceId
			});
		}
	}
}
