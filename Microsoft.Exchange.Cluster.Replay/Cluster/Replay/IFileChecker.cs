using System;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IFileChecker
	{
		bool RunChecks();

		bool RecalculateRequiredGenerations(ref JET_DBINFOMISC dbinfo);

		bool RecalculateRequiredGenerations();

		bool CheckRequiredLogfilesForPassiveOrInconsistentDatabase(bool checkForReplay);

		void CheckCheckpoint();

		void PrepareToStop();

		FileState FileState { get; }
	}
}
