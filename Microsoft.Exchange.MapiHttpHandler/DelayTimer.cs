using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.Exchange.MapiHttp
{
	internal class DelayTimer
	{
		private DelayTimer()
		{
			int num = (int)this.maxDelayPeriod.TotalSeconds / this.bucketTimeInSeconds + 1;
			this.buckets = new TaskCompletionSource<bool>[num];
			for (int i = 0; i < num; i++)
			{
				this.buckets[i] = new TaskCompletionSource<bool>();
			}
			Task.Factory.StartNew<Task>(async delegate()
			{
				await this.BackgroundTimerAsync();
			});
		}

		public static DelayTimer Instance
		{
			get
			{
				return DelayTimer.instance.Value;
			}
		}

		public Task<bool> GetTimerTask(TimeSpan delayTime)
		{
			if (delayTime > this.maxDelayPeriod)
			{
				throw new ArgumentOutOfRangeException("delayTime", "Requested delay is more than the maximum delay time allowed");
			}
			if (delayTime < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("delayTime", "Requested delay time cannot be negative");
			}
			int num = ((int)delayTime.TotalSeconds + (this.bucketTimeInSeconds - 1)) / this.bucketTimeInSeconds;
			int num2 = (this.currentBucketIndex + num) % this.buckets.Length;
			return this.buckets[num2].Task;
		}

		[DebuggerStepThrough]
		private Task BackgroundTimerAsync()
		{
			DelayTimer.<BackgroundTimerAsync>d__5 <BackgroundTimerAsync>d__;
			<BackgroundTimerAsync>d__.<>4__this = this;
			<BackgroundTimerAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<BackgroundTimerAsync>d__.<>1__state = -1;
			AsyncTaskMethodBuilder <>t__builder = <BackgroundTimerAsync>d__.<>t__builder;
			<>t__builder.Start<DelayTimer.<BackgroundTimerAsync>d__5>(ref <BackgroundTimerAsync>d__);
			return <BackgroundTimerAsync>d__.<>t__builder.Task;
		}

		private static Lazy<DelayTimer> instance = new Lazy<DelayTimer>(() => new DelayTimer());

		private readonly int bucketTimeInSeconds = (int)TimeSpan.FromSeconds(2.0).TotalSeconds;

		private readonly TimeSpan maxDelayPeriod = TimeSpan.FromMinutes(1.0);

		private readonly TaskCompletionSource<bool>[] buckets;

		private int currentBucketIndex;
	}
}
