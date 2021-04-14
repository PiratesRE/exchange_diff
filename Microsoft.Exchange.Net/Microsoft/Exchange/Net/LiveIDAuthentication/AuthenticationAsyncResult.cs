using System;
using System.Threading;

namespace Microsoft.Exchange.Net.LiveIDAuthentication
{
	internal sealed class AuthenticationAsyncResult : ICancelableAsyncResult, IAsyncResult
	{
		internal AuthenticationAsyncResult(CancelableAsyncCallback completionCallback, object state, LiveIDAuthenticationClient authenticationClient)
		{
			this.asyncState = state;
			this.completedSynchronously = true;
			this.authenticationClient = authenticationClient;
			this.completionCallback = completionCallback;
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
				lock (this.syncRoot)
				{
					if (this.asyncWaitHandle == null)
					{
						this.asyncWaitHandle = new ManualResetEvent(false);
						if (this.isCompleted)
						{
							this.asyncWaitHandle.Set();
						}
					}
				}
				return this.asyncWaitHandle;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.isCompleted;
			}
		}

		public bool IsCanceled
		{
			get
			{
				return this.exception is OperationCanceledException;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this.completedSynchronously;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public void Cancel()
		{
			bool flag = false;
			lock (this.syncRoot)
			{
				if (this.isCompleted)
				{
					return;
				}
				flag = this.authenticationClient.TryCancel();
			}
			if (flag)
			{
				this.ProcessCompleted(new OperationCanceledException());
			}
		}

		internal void SetAsync()
		{
			this.completedSynchronously = false;
		}

		internal AuthenticationResult EndProcess()
		{
			this.EndCalled();
			this.WaitForCompletion();
			return this.GetAuthenticationResult();
		}

		internal void ProcessCompleted(Exception e)
		{
			this.exception = e;
			this.ProcessCompleted();
		}

		internal void ProcessCompleted(BaseAuthenticationToken authToken)
		{
			this.token = authToken;
			this.ProcessCompleted();
		}

		private void ProcessCompleted()
		{
			lock (this.syncRoot)
			{
				if (this.isCompleted)
				{
					throw new InvalidOperationException("Operation already completed");
				}
				this.isCompleted = true;
				if (this.asyncWaitHandle != null)
				{
					this.asyncWaitHandle.Set();
				}
			}
			if (this.completionCallback != null)
			{
				this.completionCallback(this);
			}
		}

		private void WaitForCompletion()
		{
			WaitHandle waitHandle = null;
			lock (this.syncRoot)
			{
				if (!this.isCompleted)
				{
					waitHandle = this.AsyncWaitHandle;
				}
			}
			if (waitHandle != null)
			{
				waitHandle.WaitOne();
			}
		}

		private AuthenticationResult GetAuthenticationResult()
		{
			if (this.exception == null)
			{
				return new AuthenticationResult(this.token);
			}
			return new AuthenticationResult(this.exception);
		}

		private void EndCalled()
		{
			lock (this.syncRoot)
			{
				if (this.endCalled)
				{
					throw new InvalidOperationException("The EndInvoke can only be called once with an async Result.");
				}
				this.endCalled = true;
			}
		}

		private object syncRoot = new object();

		private object asyncState;

		private LiveIDAuthenticationClient authenticationClient;

		private ManualResetEvent asyncWaitHandle;

		private bool completedSynchronously;

		private bool isCompleted;

		private CancelableAsyncCallback completionCallback;

		private Exception exception;

		private BaseAuthenticationToken token;

		private bool endCalled;
	}
}
