using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IReplidGuidMap
	{
		Guid GetGuidFromReplid(Context context, ushort replid);

		ushort GetReplidFromGuid(Context context, Guid guid);

		Guid InternalGetGuidFromReplid(Context context, ushort replid);
	}
}
