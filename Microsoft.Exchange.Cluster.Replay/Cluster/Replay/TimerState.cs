using System;
using System.Threading;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TimerState : IDisposable
	{
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.M_thisTimer != null)
			{
				this.M_thisTimer.Dispose();
				this.M_thisTimer = null;
			}
		}

		internal ShipControl M_thisShip;

		internal Timer M_thisTimer;
	}
}
