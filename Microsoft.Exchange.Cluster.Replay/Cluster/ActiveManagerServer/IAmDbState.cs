using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal interface IAmDbState : IDisposable
	{
		AmDbStateInfo Read(Guid dbGuid);

		AmDbStateInfo[] ReadAll();

		void Write(AmDbStateInfo state);

		bool GetLastLogGenerationNumber(Guid dbGuid, out long lastLogGenNumber);

		void SetLastLogGenerationNumber(Guid dbGuid, long generation);

		bool GetLastLogGenerationTimeStamp(Guid dbGuid, out ExDateTime lastLogGenTimeStamp);

		void SetLastLogGenerationTimeStamp(Guid dbGuid, ExDateTime timeStamp);

		string ReadStateString(Guid dbGuid);

		T GetDebugOption<T>(AmServerName serverName, AmDebugOptions dbgOption, T defaultValue);

		bool GetLastServerTimeStamp(string serverName, out ExDateTime lastServerTimeStamp);
	}
}
