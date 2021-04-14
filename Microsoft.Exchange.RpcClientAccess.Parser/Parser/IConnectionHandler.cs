using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal interface IConnectionHandler : IDisposable
	{
		IRopHandler RopHandler { get; }

		INotificationHandler NotificationHandler { get; }

		void BeginRopProcessing(AuxiliaryData auxiliaryData);

		void EndRopProcessing(AuxiliaryData auxiliaryData);

		void LogInputRops(IEnumerable<RopId> rops);

		void LogPrepareForRop(RopId ropId);

		void LogCompletedRop(RopId ropId, ErrorCode errorCode);
	}
}
