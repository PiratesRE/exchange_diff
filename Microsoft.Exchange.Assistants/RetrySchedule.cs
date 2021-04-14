using System;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class RetrySchedule
	{
		public RetrySchedule(FinalAction finalAction, TimeSpan timeToGiveUp, params TimeSpan[] retryIntervals)
		{
			this.finalAction = finalAction;
			this.timeToGiveUp = timeToGiveUp;
			this.retryIntervals = retryIntervals;
		}

		public TimeSpan[] RetryIntervals
		{
			get
			{
				return this.retryIntervals;
			}
			set
			{
				this.retryIntervals = value;
			}
		}

		public TimeSpan TimeToGiveUp
		{
			get
			{
				return this.timeToGiveUp;
			}
			set
			{
				this.timeToGiveUp = value;
			}
		}

		public FinalAction FinalAction
		{
			get
			{
				return this.finalAction;
			}
			set
			{
				this.finalAction = value;
			}
		}

		public TimeSpan GetRetryInterval(uint n)
		{
			if ((ulong)n < (ulong)((long)this.RetryIntervals.Length))
			{
				return this.RetryIntervals[(int)((UIntPtr)n)];
			}
			return this.RetryIntervals[this.RetryIntervals.Length - 1];
		}

		private FinalAction finalAction;

		private TimeSpan timeToGiveUp;

		private TimeSpan[] retryIntervals;
	}
}
