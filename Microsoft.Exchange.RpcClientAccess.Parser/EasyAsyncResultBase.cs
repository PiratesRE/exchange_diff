using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class EasyAsyncResultBase : IAsyncResult
	{
		public EasyAsyncResultBase(object asyncState)
		{
			this.asyncState = asyncState;
			this.isCompleted = false;
			this.completionEvent = null;
		}

		public object AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (this.completionEvent == null)
				{
					lock (this.completionLock)
					{
						if (this.completionEvent == null)
						{
							this.completionEvent = new ManualResetEvent(this.isCompleted);
						}
					}
				}
				return this.completionEvent;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return false;
			}
		}

		public bool IsCompleted
		{
			get
			{
				bool result;
				lock (this.completionLock)
				{
					result = this.isCompleted;
				}
				return result;
			}
		}

		public void WaitForCompletion()
		{
			lock (this.completionLock)
			{
				if (this.isCompleted)
				{
					return;
				}
			}
			this.AsyncWaitHandle.WaitOne();
			lock (this.completionLock)
			{
				Util.DisposeIfPresent(this.completionEvent);
				this.completionEvent = null;
			}
		}

		internal void InvokeCallback()
		{
			lock (this.completionLock)
			{
				if (this.isCompleted)
				{
					return;
				}
				this.isCompleted = true;
			}
			if (this.completionEvent != null)
			{
				this.completionEvent.Set();
			}
			this.InternalCallback();
		}

		protected object CompletionLock
		{
			get
			{
				return this.completionLock;
			}
		}

		protected abstract void InternalCallback();

		private readonly object completionLock = new object();

		private object asyncState;

		private bool isCompleted;

		private ManualResetEvent completionEvent;
	}
}
