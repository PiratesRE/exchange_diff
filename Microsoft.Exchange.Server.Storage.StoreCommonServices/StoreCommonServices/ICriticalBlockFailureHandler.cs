using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface ICriticalBlockFailureHandler
	{
		void OnCriticalBlockFailed(LID lid, Context context, CriticalBlockScope criticalBlockScope);
	}
}
