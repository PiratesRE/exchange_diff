using System;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DisposeTrackerNullObject : DisposeTracker
	{
		private DisposeTrackerNullObject()
		{
			DisposeTrackerOptions.RefreshNowIfNecessary();
		}

		public static DisposeTrackerNullObject Instance
		{
			get
			{
				if (DisposeTrackerNullObject.instance == null)
				{
					DisposeTrackerNullObject.instance = new DisposeTrackerNullObject();
				}
				return DisposeTrackerNullObject.instance;
			}
		}

		protected override void Dispose(bool disposing)
		{
		}

		private static DisposeTrackerNullObject instance;
	}
}
