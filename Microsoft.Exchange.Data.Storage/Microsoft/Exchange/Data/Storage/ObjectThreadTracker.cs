using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ObjectThreadTracker
	{
		internal int OwningThreadId
		{
			get
			{
				return this.owningThreadId;
			}
		}

		internal ObjectThreadTracker()
		{
			this.lockObject = new object();
			this.owningThreadId = -1;
			this.lastMethodName = string.Empty;
			this.numberOfOwnerReferences = 0;
		}

		internal void Enter(string methodName)
		{
			int currentManagedThreadId = Environment.CurrentManagedThreadId;
			lock (this.lockObject)
			{
				if (this.owningThreadId != -1 && this.owningThreadId != currentManagedThreadId)
				{
					throw new InvalidOperationException(string.Format("In call to {0}, storeSession object is already being used by thread ID:{1} at {2}.", methodName, currentManagedThreadId, this.lastMethodName));
				}
				this.owningThreadId = currentManagedThreadId;
				this.lastMethodName = methodName;
				this.numberOfOwnerReferences++;
			}
		}

		internal void Exit()
		{
			lock (this.lockObject)
			{
				if (this.numberOfOwnerReferences > 0 && Environment.CurrentManagedThreadId == this.owningThreadId)
				{
					this.numberOfOwnerReferences--;
					if (this.numberOfOwnerReferences == 0)
					{
						this.owningThreadId = -1;
					}
				}
				else
				{
					string arg = (this.numberOfOwnerReferences == 0) ? "the calling thread doesn't own this tracker" : "no threads have entered this tracker";
					string message = string.Format("Attempting to Exit() while in an invalid state - {0}. This cannot be recovered from and is fatal.", arg);
					ExDiagnostics.FailFast(message, true);
				}
			}
		}

		private int owningThreadId;

		private string lastMethodName;

		private object lockObject;

		private int numberOfOwnerReferences;
	}
}
