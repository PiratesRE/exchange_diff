﻿using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IReplicaProgress
	{
		void ReportOneLogCopied();

		void ReportLogsReplayed(long numLogs);
	}
}
