using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Common
{
	public class AsyncEnumerator : IDisposable
	{
		public AsyncEnumerator(AsyncResultCallback callback, object asyncState, Func<AsyncEnumerator, IEnumerator<int>> enumeratorCallback) : this(callback, asyncState, enumeratorCallback, true)
		{
		}

		public AsyncEnumerator(AsyncCallback callback, object asyncState, Func<AsyncEnumerator, IEnumerator<int>> enumeratorCallback) : this(callback, asyncState, enumeratorCallback, true)
		{
		}

		public AsyncEnumerator(AsyncResultCallback callback, object asyncState, Func<AsyncEnumerator, IEnumerator<int>> enumeratorCallback, bool startAsyncOperation)
		{
			this.abortFunctions = new List<Action>();
			this.pendingResults = new List<IAsyncResult>();
			this.completedResults = new List<IAsyncResult>();
			base..ctor();
			this.AsyncResult = new AsyncResult(this, callback, asyncState);
			this.enumerator = enumeratorCallback(this);
			if (startAsyncOperation)
			{
				this.Begin();
			}
			this.ConstructorDone = true;
		}

		public AsyncEnumerator(AsyncCallback callback, object asyncState, Func<AsyncEnumerator, IEnumerator<int>> enumeratorCallback, bool startAsyncOperation)
		{
			this.abortFunctions = new List<Action>();
			this.pendingResults = new List<IAsyncResult>();
			this.completedResults = new List<IAsyncResult>();
			base..ctor();
			this.AsyncResult = new AsyncResult(this, delegate(AsyncResult ar)
			{
				if (callback != null)
				{
					callback(ar);
				}
			}, asyncState);
			this.enumerator = enumeratorCallback(this);
			if (startAsyncOperation)
			{
				this.Begin();
			}
			this.ConstructorDone = true;
		}

		protected AsyncEnumerator()
		{
			this.abortFunctions = new List<Action>();
			this.pendingResults = new List<IAsyncResult>();
			this.completedResults = new List<IAsyncResult>();
			base..ctor();
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.enumerator.Dispose();
			}
		}

		public IList<IAsyncResult> CompletedAsyncResults
		{
			get
			{
				return this.completedResults;
			}
		}

		public AsyncResult AsyncResult
		{
			get
			{
				return this.result;
			}
			protected set
			{
				this.result = value;
				this.result.OnAbort += this.Abort;
			}
		}

		public bool IsAborted { get; private set; }

		public AsyncCallback GetAsyncCallback()
		{
			if (!this.IsAborted && this.isDisposed)
			{
				throw new ObjectDisposedException("AsyncEnumerator disposed");
			}
			this.ThrowForMoreAsyncsAfterCompletion();
			bool flag = false;
			AsyncCallback asyncCallback;
			try
			{
				List<IAsyncResult> obj;
				Monitor.Enter(obj = this.pendingResults, ref flag);
				int callbackIndex = this.pendingResults.Count;
				this.pendingResults.Add(null);
				this.abortFunctions.Add(null);
				asyncCallback = delegate(IAsyncResult ar)
				{
					lock (this.pendingResults)
					{
						this.pendingResults[callbackIndex] = ar;
					}
					if (Interlocked.Decrement(ref this.pendingAsyncOps) == 0)
					{
						this.Advance();
					}
				};
			}
			finally
			{
				if (flag)
				{
					List<IAsyncResult> obj;
					Monitor.Exit(obj);
				}
			}
			return asyncCallback;
		}

		public AsyncResultCallback GetAsyncResultCallback()
		{
			if (!this.IsAborted && this.isDisposed)
			{
				throw new ObjectDisposedException("AsyncEnumerator disposed");
			}
			this.ThrowForMoreAsyncsAfterCompletion();
			bool flag = false;
			AsyncResultCallback asyncResultCallback;
			try
			{
				List<IAsyncResult> obj;
				Monitor.Enter(obj = this.pendingResults, ref flag);
				int callbackIndex = this.pendingResults.Count;
				this.pendingResults.Add(null);
				this.abortFunctions.Add(null);
				asyncResultCallback = delegate(AsyncResult ar)
				{
					lock (this.pendingResults)
					{
						this.pendingResults[callbackIndex] = ar;
					}
					if (Interlocked.Decrement(ref this.pendingAsyncOps) == 0)
					{
						this.Advance();
					}
				};
			}
			finally
			{
				if (flag)
				{
					List<IAsyncResult> obj;
					Monitor.Exit(obj);
				}
			}
			return asyncResultCallback;
		}

		public AsyncResultCallback<T> GetAsyncResultCallback<T>()
		{
			if (!this.IsAborted && this.isDisposed)
			{
				throw new ObjectDisposedException("AsyncEnumerator disposed");
			}
			this.ThrowForMoreAsyncsAfterCompletion();
			bool flag = false;
			AsyncResultCallback<T> asyncResultCallback;
			try
			{
				List<IAsyncResult> obj;
				Monitor.Enter(obj = this.pendingResults, ref flag);
				int callbackIndex = this.pendingResults.Count;
				this.pendingResults.Add(null);
				this.abortFunctions.Add(null);
				asyncResultCallback = delegate(AsyncResult<T> ar)
				{
					lock (this.pendingResults)
					{
						this.pendingResults[callbackIndex] = ar;
					}
					if (Interlocked.Decrement(ref this.pendingAsyncOps) == 0)
					{
						this.Advance();
					}
				};
			}
			finally
			{
				if (flag)
				{
					List<IAsyncResult> obj;
					Monitor.Exit(obj);
				}
			}
			return asyncResultCallback;
		}

		public T AddAsync<T>(T asyncResult) where T : IAsyncResult
		{
			this.ThrowForMoreAsyncsAfterCompletion();
			lock (this.pendingResults)
			{
				this.pendingResults[this.pendingResults.Count - 1] = asyncResult;
				if (this.IsAborted)
				{
					this.AbortPendingResult(this.pendingResults.Count - 1);
				}
			}
			return asyncResult;
		}

		public T AddAsync<T>(T asyncResult, Action abortRequest) where T : IAsyncResult
		{
			this.ThrowForMoreAsyncsAfterCompletion();
			lock (this.pendingResults)
			{
				this.pendingResults[this.pendingResults.Count - 1] = asyncResult;
				this.abortFunctions[this.pendingResults.Count - 1] = abortRequest;
				if (this.IsAborted)
				{
					this.AbortPendingResult(this.pendingResults.Count - 1);
				}
			}
			return asyncResult;
		}

		public AsyncResult AddAsyncEnumerator(Func<AsyncEnumerator, IEnumerator<int>> enumerator)
		{
			return this.AddAsync<AsyncResult>(new AsyncEnumerator(this.GetAsyncResultCallback(), null, enumerator).AsyncResult);
		}

		public AsyncResult<T> AddAsyncEnumerator<T>(Func<AsyncEnumerator<T>, IEnumerator<int>> enumerator)
		{
			return this.AddAsync<AsyncResult<T>>(new AsyncEnumerator<T>(this.GetAsyncResultCallback<T>(), null, enumerator).AsyncResult);
		}

		public void End()
		{
			this.AsyncResult.CompletedSynchronously = !this.ConstructorDone;
			this.AsyncResult.IsCompleted = true;
			this.VerifySuccessfullyCompleted();
		}

		public void Begin()
		{
			if (!this.enumerationStarted)
			{
				this.enumerationStarted = true;
				this.Advance();
				return;
			}
			throw new InvalidOperationException("Async enumeration already started");
		}

		protected bool ConstructorDone { get; set; }

		private void Advance()
		{
			try
			{
				while (!this.IsAborted && !this.isDisposed)
				{
					List<IAsyncResult> list = this.pendingResults;
					this.pendingResults = this.completedResults;
					this.completedResults = list;
					this.pendingResults.Clear();
					this.abortFunctions.Clear();
					bool flag = this.enumerator.MoveNext();
					this.DisposeResults(this.completedResults);
					if (!flag)
					{
						if (!this.AsyncResult.IsCompleted)
						{
							throw new InvalidOperationException("Should call AsyncEnumerator.End before stopping the enumerator");
						}
						this.enumerator.Dispose();
						this.AsyncCompleted();
					}
					else
					{
						this.ThrowForMoreAsyncsAfterCompletion();
						if (this.enumerator.Current != this.pendingResults.Count)
						{
							throw new InvalidOperationException("for some reason number of async callbacks and expected callbacks doesn't match");
						}
						lock (this.pendingResults)
						{
							using (List<IAsyncResult>.Enumerator enumerator = this.pendingResults.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									if (enumerator.Current == null)
									{
										throw new InvalidOperationException("Not all async operations were added");
									}
								}
							}
						}
						if (Interlocked.Add(ref this.pendingAsyncOps, this.enumerator.Current) == 0)
						{
							continue;
						}
					}
					return;
				}
				this.DisposeResults(this.pendingResults);
				this.enumerator.Dispose();
			}
			catch (Exception exception)
			{
				this.enumerator.Dispose();
				if (this.AsyncResult.IsCompleted)
				{
					throw;
				}
				this.AsyncResult.Exception = exception;
				this.AsyncResult.IsCompleted = true;
				this.AsyncResult.CompletedSynchronously = !this.ConstructorDone;
				this.AsyncCompleted();
			}
		}

		protected virtual void AsyncCompleted()
		{
			this.DisposeResults(this.pendingResults);
			this.DisposeResults(this.completedResults);
		}

		protected virtual void VerifySuccessfullyCompleted()
		{
			if (!this.AsyncResult.IsCompleted)
			{
				throw new InvalidOperationException("Wrong completion was called");
			}
		}

		protected virtual void ThrowForMoreAsyncsAfterCompletion()
		{
			if (this.AsyncResult.IsCompleted)
			{
				throw new InvalidOperationException("Can't do more asyncCalls after End has been called on AsyncEnumerator");
			}
		}

		protected void Begin(IEnumerator<int> enumerator, bool startEnumeration)
		{
			if (this.enumerator != null)
			{
				throw new InvalidOperationException("AsyncEnumerator already being used");
			}
			this.enumerator = enumerator;
			if (startEnumeration)
			{
				this.Begin();
			}
		}

		private void Abort()
		{
			if (!this.IsAborted)
			{
				this.IsAborted = true;
				lock (this.pendingResults)
				{
					for (int i = 0; i < this.pendingResults.Count; i++)
					{
						this.AbortPendingResult(i);
					}
				}
			}
		}

		private void AbortPendingResult(int iResult)
		{
			if (!this.pendingResults[iResult].IsCompleted)
			{
				if (this.abortFunctions[iResult] != null)
				{
					this.abortFunctions[iResult]();
				}
				AsyncResult asyncResult = this.pendingResults[iResult] as AsyncResult;
				if (asyncResult != null)
				{
					asyncResult.Abort();
				}
			}
		}

		private void DisposeResults(IList<IAsyncResult> asyncResults)
		{
			foreach (IAsyncResult asyncResult in asyncResults)
			{
				IDisposable disposable = asyncResult as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		private List<Action> abortFunctions;

		private List<IAsyncResult> pendingResults;

		private List<IAsyncResult> completedResults;

		private AsyncResult result;

		protected IEnumerator<int> enumerator;

		private bool enumerationStarted;

		private bool isDisposed;

		private int pendingAsyncOps;
	}
}
