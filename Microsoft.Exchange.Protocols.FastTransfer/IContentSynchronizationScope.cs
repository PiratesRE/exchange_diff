using System;
using System.Collections.Generic;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal interface IContentSynchronizationScope : IDisposable
	{
		ExchangeId GetExchangeId(long shortTermId);

		ReplId GuidToReplid(Guid guid);

		IdSet GetServerCnsetSeen(MapiContext operationContext, bool conversations);

		IEnumerable<Properties> GetChangedMessages(MapiContext operationContext, IcsState icsState);

		IdSet GetDeletes(MapiContext operationContext, IcsState icsState);

		IdSet GetSoftDeletes(MapiContext operationContext, IcsState icsState);

		void GetNewReadsUnreads(MapiContext operationContext, IcsState icsState, out IdSet midsetNewReads, out IdSet midsetNewUnreads, out IdSet finalCnsetRead);

		FastTransferMessage OpenMessage(ExchangeId mid);

		PropertyGroupMapping GetPropertyGroupMapping();
	}
}
