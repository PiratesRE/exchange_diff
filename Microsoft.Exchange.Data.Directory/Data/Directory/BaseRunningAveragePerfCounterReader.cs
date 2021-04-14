using System;
using System.Timers;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class BaseRunningAveragePerfCounterReader : DisposeTrackableBase, IPerfCounterReader
	{
		public BaseRunningAveragePerfCounterReader(ushort numberOfSamples, uint intervalTime)
		{
			if (intervalTime < 100U)
			{
				throw new ArgumentException("intervalTime must be greater than 100 milliseconds", "intervalTime");
			}
			this.cachedValue = new RunningAverageFloat(numberOfSamples);
			this.AcquireCounter();
			this.timer = new Timer(intervalTime);
			this.timer.Elapsed += this.HandleTimer;
			this.timer.Start();
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.timer.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BaseRunningAveragePerfCounterReader>(this);
		}

		public float GetValue()
		{
			return this.cachedValue.Value;
		}

		protected virtual void HandleTimer(object sender, ElapsedEventArgs e)
		{
			if (!this.BeforeRead())
			{
				return;
			}
			lock (this.lockObject)
			{
				float newValue = this.ReadCounter();
				this.cachedValue.Update(newValue);
			}
		}

		protected virtual bool BeforeRead()
		{
			return true;
		}

		protected abstract bool AcquireCounter();

		protected abstract float ReadCounter();

		private RunningAverageFloat cachedValue;

		private Timer timer;

		private object lockObject = new object();
	}
}
