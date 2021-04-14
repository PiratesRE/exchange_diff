using System;
using System.IO;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class WatcherState : FileSystemWatcher
	{
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		internal ShipControl M_thisShip;
	}
}
