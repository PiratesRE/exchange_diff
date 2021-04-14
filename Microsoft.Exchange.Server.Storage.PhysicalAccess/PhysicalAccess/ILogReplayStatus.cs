using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface ILogReplayStatus
	{
		void GetReplayStatus(out uint nextLogToReplay, out byte[] databaseInfo, out uint patchPageNumber, out byte[] patchToken, out byte[] patchData, out uint[] corruptPages);

		void SetMaxLogGenerationToReplay(uint value, uint logReplayFlags);

		void GetDatabaseInformation(out byte[] databaseInfo);
	}
}
