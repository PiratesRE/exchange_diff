using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PerServerActivityThrottle<TContext>
	{
		public static PerServerActivityThrottle<TContext> GetPerServerActivityThrottle(string server)
		{
			PerServerActivityThrottle<TContext> result;
			lock (PerServerActivityThrottle<TContext>.cacheLock)
			{
				PerServerActivityThrottle<TContext> perServerActivityThrottle = null;
				if (!PerServerActivityThrottle<TContext>.cache.TryGetValue(server, out perServerActivityThrottle))
				{
					perServerActivityThrottle = new PerServerActivityThrottle<TContext>();
					PerServerActivityThrottle<TContext>.cache.Add(server, perServerActivityThrottle);
				}
				result = perServerActivityThrottle;
			}
			return result;
		}

		public bool TryGetActivityLock(bool force, int maxActivity, out IDisposable activityLock)
		{
			activityLock = null;
			bool result;
			lock (this.activityCounterLock)
			{
				if (!force && this.activityCounter >= maxActivity)
				{
					result = false;
				}
				else
				{
					activityLock = new PerServerActivityThrottle<TContext>.ActivityLock(this);
					this.activityCounter++;
					result = true;
				}
			}
			return result;
		}

		private void DecreaseActivityCount()
		{
			lock (this.activityCounterLock)
			{
				if (this.activityCounter == 0)
				{
					throw new InvalidOperationException("Activity counter already zero and cannot be decreased.");
				}
				this.activityCounter--;
			}
		}

		private static readonly Dictionary<string, PerServerActivityThrottle<TContext>> cache = new Dictionary<string, PerServerActivityThrottle<TContext>>();

		private static readonly object cacheLock = new object();

		private int activityCounter;

		private readonly object activityCounterLock = new object();

		private struct ActivityLock : IDisposeTrackable, IDisposable
		{
			internal ActivityLock(PerServerActivityThrottle<TContext> perServerActivityThrottle)
			{
				this.perServerActivityThrottle = perServerActivityThrottle;
				this.isDisposed = false;
				this.disposeTracker = null;
				this.disposeTracker = DisposeTracker.Get<PerServerActivityThrottle<TContext>.ActivityLock>(this);
			}

			public void Dispose()
			{
				if (!this.isDisposed)
				{
					this.isDisposed = true;
					if (this.perServerActivityThrottle != null)
					{
						this.perServerActivityThrottle.DecreaseActivityCount();
					}
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
					}
				}
			}

			DisposeTracker IDisposeTrackable.GetDisposeTracker()
			{
				return DisposeTracker.Get<PerServerActivityThrottle<TContext>.ActivityLock>(this);
			}

			void IDisposeTrackable.SuppressDisposeTracker()
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Suppress();
				}
			}

			private readonly DisposeTracker disposeTracker;

			private readonly PerServerActivityThrottle<TContext> perServerActivityThrottle;

			private bool isDisposed;
		}
	}
}
