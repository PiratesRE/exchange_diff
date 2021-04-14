using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISetViable
	{
		bool Viable { get; }

		void SetViable();

		void ClearViable();
	}
}
