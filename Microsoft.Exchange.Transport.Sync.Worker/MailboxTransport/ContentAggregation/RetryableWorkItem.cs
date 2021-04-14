using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RetryableWorkItem : DisposeTrackableBase
	{
		protected RetryableWorkItem(int initialRetryInMilliseconds, int retryBackoffFactor, int maximumNumberOfAttempts)
		{
			this.initialRetryInMilliseconds = initialRetryInMilliseconds;
			this.retryBackoffFactor = retryBackoffFactor;
			this.maximumNumberOfAttempts = maximumNumberOfAttempts;
			this.ResetRetryState();
		}

		public bool IsMaximumNumberOfAttemptsReached
		{
			get
			{
				base.CheckDisposed();
				return this.numberOfAttempts >= this.maximumNumberOfAttempts;
			}
		}

		public int CurrentRetryCount
		{
			get
			{
				base.CheckDisposed();
				return this.numberOfAttempts - 1;
			}
		}

		public string TypeFullName
		{
			get
			{
				base.CheckDisposed();
				if (this.typeFullName == null)
				{
					this.typeFullName = base.GetType().FullName;
				}
				return this.typeFullName;
			}
		}

		public int UpdateRetryStateOnRetry()
		{
			base.CheckDisposed();
			this.numberOfAttempts++;
			int result = this.nextRetryWait;
			this.nextRetryWait *= this.retryBackoffFactor;
			return result;
		}

		public void MaxOutRetryCount()
		{
			base.CheckDisposed();
			this.maximumNumberOfAttempts = this.numberOfAttempts;
		}

		protected void ResetRetryState()
		{
			base.CheckDisposed();
			this.numberOfAttempts = 1;
			this.nextRetryWait = this.initialRetryInMilliseconds;
		}

		private readonly int initialRetryInMilliseconds;

		private readonly int retryBackoffFactor;

		private int maximumNumberOfAttempts;

		private int numberOfAttempts;

		private int nextRetryWait;

		private string typeFullName;
	}
}
