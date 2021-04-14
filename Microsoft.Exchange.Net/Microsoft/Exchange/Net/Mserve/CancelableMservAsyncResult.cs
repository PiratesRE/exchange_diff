using System;
using System.Threading;

namespace Microsoft.Exchange.Net.Mserve
{
	internal sealed class CancelableMservAsyncResult : ICancelableAsyncResult, IAsyncResult
	{
		public CancelableMservAsyncResult(ICancelableAsyncResult internalResult, OutstandingAsyncReadConfig readConfig, object asyncStateOverride)
		{
			if (internalResult == null)
			{
				throw new ArgumentNullException("internalResult");
			}
			if (readConfig == null)
			{
				throw new ArgumentNullException("readConfig");
			}
			if (asyncStateOverride == null)
			{
				throw new ArgumentNullException("asyncStateOverride");
			}
			this.internalResult = internalResult;
			this.readConfig = readConfig;
			this.asyncStateOverride = asyncStateOverride;
		}

		public ICancelableAsyncResult InternalResult
		{
			get
			{
				return this.internalResult;
			}
		}

		public OutstandingAsyncReadConfig ReadConfig
		{
			get
			{
				return this.readConfig;
			}
		}

		public object AsyncState
		{
			get
			{
				return this.asyncStateOverride;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return this.internalResult.AsyncWaitHandle;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.internalResult.IsCompleted;
			}
		}

		public bool IsCanceled
		{
			get
			{
				return this.internalResult.IsCanceled;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this.internalResult.CompletedSynchronously;
			}
		}

		public void Cancel()
		{
			this.internalResult.Cancel();
		}

		private readonly ICancelableAsyncResult internalResult;

		private readonly OutstandingAsyncReadConfig readConfig;

		private readonly object asyncStateOverride;
	}
}
