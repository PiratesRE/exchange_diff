using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISetGeneration : IGetStatus
	{
		void SetCopyGeneration(long gen, DateTime writeTime);

		void SetInspectGeneration(long gen, DateTime writeTime);

		void SetCopyNotificationGeneration(long gen, DateTime writeTime);

		void SetLogStreamStartGeneration(long generation);

		void ClearLogStreamStartGeneration();

		bool IsLogStreamStartGenerationResetPending { get; }
	}
}
