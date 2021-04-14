using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal sealed class FolderIdConverter : IdConverter
	{
		internal FolderIdConverter() : base(PropertyTag.Fid, CoreFolderSchema.Id)
		{
		}

		protected override long CreateClientId(StoreSession session, StoreId id)
		{
			return session.IdConverter.GetFidFromId(StoreId.GetStoreObjectId(id));
		}
	}
}
