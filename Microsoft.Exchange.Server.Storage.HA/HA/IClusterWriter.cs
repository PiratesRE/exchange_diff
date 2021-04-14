using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal interface IClusterWriter
	{
		bool IsClusterRunning();

		Exception TryWriteLastLog(Guid dbGuid, long lastLogGen);
	}
}
