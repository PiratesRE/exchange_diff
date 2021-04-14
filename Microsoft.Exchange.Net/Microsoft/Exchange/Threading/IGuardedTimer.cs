using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Threading
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IGuardedTimer : IDisposeTrackable, IDisposable
	{
		bool Change(int dueTime, int period);

		bool Change(long dueTime, long period);

		bool Change(TimeSpan dueTime, TimeSpan period);

		void Pause();

		void Continue();

		void Continue(TimeSpan dueTime, TimeSpan period);

		void Dispose(bool wait);
	}
}
