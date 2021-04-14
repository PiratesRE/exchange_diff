using System;

namespace Microsoft.Exchange.Diagnostics
{
	public static class DisposeTrackerFactory
	{
		public static void Register(DisposeTrackerFactory.DisposeTrackerFactoryDelegate factoryDelegate)
		{
			DisposeTrackerFactory.factoryDelegate = factoryDelegate;
		}

		public static DisposeTracker Get(IDisposable obj)
		{
			if (DisposeTrackerFactory.factoryDelegate == null)
			{
				return DisposeTrackerNullObject.Instance;
			}
			return DisposeTrackerFactory.factoryDelegate(obj);
		}

		private static DisposeTrackerFactory.DisposeTrackerFactoryDelegate factoryDelegate;

		public delegate DisposeTracker DisposeTrackerFactoryDelegate(IDisposable obj);
	}
}
