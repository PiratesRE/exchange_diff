using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal static class Test
	{
		public static Test.NotifyAllWatermarksCommittedDelegate NotifyAllWatermarksCommitted { get; set; }

		public static Test.NotifyOnPostWatermarkCommitDelegate NotifyOnPostWatermarkCommit { get; set; }

		public static Test.NotifyPhase1ShutdownCompleteDelegate NotifyPhase1ShutdownComplete { get; set; }

		public static Test.NotifyPoisonEventSkippedDelegate NotifyPoisonEventSkipped { get; set; }

		public static Test.NotifyPoisonMailboxSkippedDelegate NotifyPoisonMailboxSkipped { get; set; }

		public delegate void NotifyPhase1ShutdownCompleteDelegate();

		internal delegate void NotifyAllWatermarksCommittedDelegate();

		internal delegate void NotifyOnPostWatermarkCommitDelegate(Watermark[] watermarksToSave, bool experiencedPartialCompletion);

		internal delegate void NotifyPoisonEventSkippedDelegate(DatabaseInfo databaseInfo, MapiEvent mapiEvent);

		internal delegate void NotifyPoisonMailboxSkippedDelegate(DatabaseInfo databaseInfo, Guid mailboxGuid);
	}
}
