using System;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Engine
{
	internal abstract class InterlockedCounter
	{
		protected InterlockedCounter(int initialValue, Func<int, bool> condition)
		{
			this.counter = initialValue;
			this.condition = condition;
		}

		public static InterlockedCounter Create(int initialValue, Func<int, bool> condition, Action action)
		{
			return new InterlockedCounter.CallbackCounter(initialValue, condition, action);
		}

		public static InterlockedCounter.EventCounter Create(int initialValue, Func<int, bool> condition)
		{
			return new InterlockedCounter.EventCounter(initialValue, condition);
		}

		public int Increment()
		{
			int num = Interlocked.Increment(ref this.counter);
			if (this.condition(num))
			{
				this.CounterAction();
			}
			return num;
		}

		public int Decrement()
		{
			int num = Interlocked.Decrement(ref this.counter);
			if (this.condition(num))
			{
				this.CounterAction();
			}
			return num;
		}

		protected abstract void CounterAction();

		private readonly Func<int, bool> condition;

		private int counter;

		internal class CallbackCounter : InterlockedCounter
		{
			internal CallbackCounter(int initialValue, Func<int, bool> condition, Action callbackAction) : base(initialValue, condition)
			{
				Util.ThrowOnNullArgument(callbackAction, "callbackAction");
				Util.ThrowOnNullArgument(condition, "condition");
				this.callbackAction = callbackAction;
			}

			protected override void CounterAction()
			{
				this.callbackAction();
			}

			private readonly Action callbackAction;
		}

		internal class EventCounter : InterlockedCounter, IDisposeTrackable, IDisposable
		{
			internal EventCounter(int initialValue, Func<int, bool> condition) : base(initialValue, condition)
			{
				this.conditionSatisfied = new ManualResetEventSlim(condition(initialValue));
				this.disposeTracker = this.GetDisposeTracker();
			}

			public void Wait(TimeSpan timeout)
			{
				if (!this.conditionSatisfied.Wait(timeout))
				{
					throw new InterlockedCounter.InterlockedCounterException(Strings.InterlockedCounterTimeout);
				}
				if (this.disposedBeforeTimeout)
				{
					throw new InterlockedCounter.InterlockedCounterException(Strings.InterlockedCounterDisposed);
				}
			}

			public void Dispose()
			{
				if (!this.conditionSatisfied.IsSet)
				{
					this.disposedBeforeTimeout = true;
					this.conditionSatisfied.Set();
				}
				this.conditionSatisfied.Dispose();
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
				GC.SuppressFinalize(this);
			}

			public DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<InterlockedCounter.EventCounter>(this);
			}

			public void SuppressDisposeTracker()
			{
				this.disposeTracker.Suppress();
			}

			protected override void CounterAction()
			{
				this.conditionSatisfied.Set();
			}

			private readonly ManualResetEventSlim conditionSatisfied;

			private bool disposedBeforeTimeout;

			private DisposeTracker disposeTracker;
		}

		internal class InterlockedCounterException : ComponentFailedTransientException
		{
			public InterlockedCounterException(LocalizedString message) : base(message)
			{
			}
		}
	}
}
