using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public static class DisposeTrackerFactory
	{
		public static IDisposable Get<T>(T trackable) where T : IDisposable
		{
			return DisposeTracker.Get<T>(trackable);
		}
	}
}
