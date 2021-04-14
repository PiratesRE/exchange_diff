using System;
using System.Collections.Generic;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal interface IHierarchySynchronizationScope
	{
		ExchangeId GetExchangeId(long shortTermId);

		ReplId GuidToReplid(Guid guid);

		Guid ReplidToGuid(ReplId replid);

		IdSet GetServerCnsetSeen(MapiContext operationContext);

		void GetChangedAndDeletedFolders(MapiContext operationContext, SyncFlag syncFlags, IdSet cnsetSeen, IdSet idsetGiven, out IList<FolderChangeEntry> changedFolders, out IdSet idsetNewDeletes);

		ExchangeId GetRootFid();

		MapiFolder OpenFolder(ExchangeId fid);
	}
}
