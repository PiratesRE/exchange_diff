﻿using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplaySystemQueue : PrioritizedQueue<ReplaySystemQueuedItem>
	{
	}
}
