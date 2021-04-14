using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FixedTimeSumProgress : FixedTimeSumBase
	{
		public FixedTimeSumProgress(uint windowBucketLength, ushort numberOfBuckets) : base(windowBucketLength, numberOfBuckets, null)
		{
		}

		public FixedTimeSumProgress(uint windowBucketLength, ushort numberOfBuckets, FixedTimeSumSlot[] input) : base(windowBucketLength, numberOfBuckets, null)
		{
			this.Deserialize(input);
		}

		internal void Add(uint value)
		{
			base.TryAdd(value);
		}

		internal FixedTimeSumSlot[] Serialize()
		{
			List<FixedTimeSumBase.WindowBucket> list = new List<FixedTimeSumBase.WindowBucket>();
			if (this.currentBucket != null)
			{
				list.Add(this.currentBucket);
			}
			if (this.waitingBuckets != null && this.waitingBuckets.Count > 0)
			{
				list.AddRange((from bucket in this.waitingBuckets
				orderby bucket.ExpireAt descending
				select bucket).ToList<FixedTimeSumBase.WindowBucket>());
			}
			FixedTimeSumSlot[] array = new FixedTimeSumSlot[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = new FixedTimeSumSlot
				{
					StartTimeInTicks = list[i].ExpireAt.Subtract(TimeSpan.FromMilliseconds(this.windowBucketLength)).Ticks,
					Value = list[i].Value
				};
			}
			return array;
		}

		private void Deserialize(FixedTimeSumSlot[] input)
		{
			if (input == null || input.Length == 0)
			{
				return;
			}
			DateTime utcNow = new DateTime(input[0].StartTimeInTicks, DateTimeKind.Utc);
			this.currentBucket = new FixedTimeSumBase.WindowBucket(utcNow, this.windowBucketLength);
			this.currentBucket.Add(utcNow, input[0].Value);
			this.value += input[0].Value;
			for (int i = 1; i < input.Length; i++)
			{
				FixedTimeSumBase.WindowBucket windowBucket = new FixedTimeSumBase.WindowBucket(new DateTime(input[i].StartTimeInTicks), this.windowBucketLength);
				windowBucket.Add(new DateTime(input[i].StartTimeInTicks, DateTimeKind.Utc), input[i].Value);
				this.waitingBuckets.Enqueue(windowBucket);
				this.value += input[i].Value;
			}
		}
	}
}
