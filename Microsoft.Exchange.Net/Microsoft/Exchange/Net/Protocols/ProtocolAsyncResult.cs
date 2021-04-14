using System;
using System.Threading;

namespace Microsoft.Exchange.Net.Protocols
{
	internal sealed class ProtocolAsyncResult : ICancelableAsyncResult, IAsyncResult
	{
		internal ProtocolAsyncResult(CancelableAsyncCallback completionCallback, object state, ProtocolClient protocolClient)
		{
			if (protocolClient == null)
			{
				throw new ArgumentNullException("protocolClient");
			}
			this.asyncState = state;
			this.completedSynchronously = true;
			this.protocolClient = protocolClient;
			this.completionCallback = completionCallback;
		}

		public object AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		public CancelableAsyncCallback CompletionCallback
		{
			get
			{
				return this.completionCallback;
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
				return this.protocolResult != null && this.protocolResult.IsCanceled;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this.completedSynchronously;
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
				flag = this.protocolClient.TryCancel();
			}
			if (flag)
			{
				this.ProcessCompleted(ProtocolAsyncResult.OperationCanceledException);
			}
		}

		internal void SetAsync()
		{
			this.completedSynchronously = false;
		}

		internal ProtocolResult EndProcess()
		{
			this.EndCalled();
			this.WaitForCompletion();
			return this.protocolResult;
		}

		internal void ProcessCompleted(Exception exception)
		{
			this.protocolResult = new ProtocolResult(exception);
			this.ProcessCompleted();
		}

		internal void ProcessCompleted(ResultData resultData)
		{
			this.protocolResult = new ProtocolResult(resultData);
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

		private static readonly OperationCanceledException OperationCanceledException = new OperationCanceledException();

		private object syncRoot = new object();

		private object asyncState;

		private ProtocolClient protocolClient;

		private ManualResetEvent asyncWaitHandle;

		private bool completedSynchronously;

		private bool isCompleted;

		private CancelableAsyncCallback completionCallback;

		private ProtocolResult protocolResult;

		private bool endCalled;
	}
}
