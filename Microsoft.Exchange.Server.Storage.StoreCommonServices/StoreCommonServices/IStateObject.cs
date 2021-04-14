using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IStateObject
	{
		void OnBeforeCommit(Context context);

		void OnCommit(Context context);

		void OnAbort(Context context);
	}
}
