using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct ObjectAccessGuard : IDisposable
	{
		public static ObjectAccessGuard Create(ObjectThreadTracker objectThreadTracker, string methodName)
		{
			return new ObjectAccessGuard(objectThreadTracker, methodName);
		}

		private ObjectAccessGuard(ObjectThreadTracker objectThreadTracker, string methodName)
		{
			this.isDisposed = false;
			this.objectThreadTracker = null;
			this.testHook = null;
			if (ObjectAccessGuard.testHookFactory.Value != null)
			{
				this.testHook = ObjectAccessGuard.testHookFactory.Value(objectThreadTracker, methodName);
				return;
			}
			if (objectThreadTracker == null)
			{
				throw new ArgumentNullException("objectThreadTracker");
			}
			this.objectThreadTracker = objectThreadTracker;
			this.objectThreadTracker.Enter(methodName);
		}

		internal static IDisposable SetTestHook(Func<ObjectThreadTracker, string, IDisposable> factory)
		{
			return ObjectAccessGuard.testHookFactory.SetTestHook(factory);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !this.isDisposed)
			{
				this.isDisposed = true;
				if (this.testHook != null)
				{
					this.testHook.Dispose();
					return;
				}
				if (this.objectThreadTracker != null)
				{
					this.objectThreadTracker.Exit();
				}
			}
		}

		private static Hookable<Func<ObjectThreadTracker, string, IDisposable>> testHookFactory = Hookable<Func<ObjectThreadTracker, string, IDisposable>>.Create(true, null);

		private bool isDisposed;

		private ObjectThreadTracker objectThreadTracker;

		private IDisposable testHook;
	}
}
