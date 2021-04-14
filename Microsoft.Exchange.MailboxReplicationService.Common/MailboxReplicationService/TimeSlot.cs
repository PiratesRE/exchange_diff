using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public abstract class TimeSlot
	{
		private protected ulong[] Values { protected get; private set; }

		private protected uint Head { protected get; private set; }

		private protected ulong MillisecondsPerSlot { protected get; private set; }

		public TimeSlot(uint capacity, ulong millisecondsPerSlot)
		{
			this.Values = new ulong[capacity];
			this.MillisecondsPerSlot = millisecondsPerSlot;
		}

		public TimeSlot(uint capacity, ulong[] inputArray, ulong millisecondsPerSlot)
		{
			if (inputArray != null)
			{
				capacity = Math.Max(capacity, (uint)inputArray.Length);
				this.Values = new ulong[capacity];
				Array.Copy(inputArray, this.Values, inputArray.Length);
			}
			else
			{
				this.Values = new ulong[capacity];
			}
			this.MillisecondsPerSlot = millisecondsPerSlot;
		}

		public uint Capacity
		{
			get
			{
				return (uint)this.Values.Length;
			}
		}

		public ulong this[uint index]
		{
			get
			{
				return this.Values[(int)((UIntPtr)((index + this.Head) % this.Capacity))];
			}
			private set
			{
				this.Values[(int)((UIntPtr)((index + this.Head) % this.Capacity))] = value;
			}
		}

		public override string ToString()
		{
			return string.Join<ulong>("|", this.ToArray());
		}

		public void AddTime(TimeSpan duration)
		{
			DateTime utcNow = TimeProvider.UtcNow;
			ulong num = this.GetLatestSlotMilliseconds(utcNow);
			TimeSpan.FromMilliseconds(num);
			if (num >= duration.TotalMilliseconds)
			{
				num = (ulong)duration.TotalMilliseconds;
			}
			this.Values[(int)((UIntPtr)this.Head)] = this.Values[(int)((UIntPtr)this.Head)] + num;
			ulong num2 = (ulong)(duration.TotalMilliseconds - num);
			if (num2 > 0UL)
			{
				uint num3 = (uint)(num2 / this.MillisecondsPerSlot);
				if (num3 >= this.Capacity - 1U)
				{
					num3 = this.Capacity - 1U;
				}
				else
				{
					ulong num4 = num2 % this.MillisecondsPerSlot;
					uint num5 = (this.Head + num3 + 1U) % this.Capacity;
					this.Values[(int)((UIntPtr)num5)] += num4;
				}
				for (uint num6 = 1U; num6 < num3 + 1U; num6 += 1U)
				{
					this.Values[(int)((UIntPtr)((this.Head + num6) % this.Capacity))] = this.MillisecondsPerSlot;
				}
			}
		}

		public void Refresh(DateTime lastUpdateTime)
		{
			DateTime utcNow = TimeProvider.UtcNow;
			uint elapsedSlotCount = this.GetElapsedSlotCount(lastUpdateTime, utcNow);
			for (uint num = 0U; num < elapsedSlotCount; num += 1U)
			{
				if (this.Head == 0U)
				{
					this.Head = this.Capacity - 1U;
				}
				else
				{
					this.Head = (this.Head - 1U) % this.Capacity;
				}
				this.Values[(int)((UIntPtr)this.Head)] = 0UL;
			}
		}

		protected abstract uint GetElapsedSlotCount(DateTime lastUpdateTime, DateTime currentTime);

		protected abstract ulong GetLatestSlotMilliseconds(DateTime currentTime);

		protected abstract TimeSlotXML GetSlotXML(DateTime time, ulong value);

		public void PopulateFrom(TimeSlot ts1, TimeSlot ts2)
		{
			for (uint num = 0U; num < this.Capacity; num += 1U)
			{
				this.Values[(int)((UIntPtr)num)] = ts1[num] + ts2[num];
			}
		}

		public ulong[] ToArray()
		{
			uint capacity = this.Capacity;
			int num = -1;
			for (int i = (int)(this.Capacity - 1U); i >= 0; i--)
			{
				if (this[(uint)i] != 0UL)
				{
					num = i;
					break;
				}
			}
			List<ulong> list = new List<ulong>();
			uint num2 = 0U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				list.Add(this[num2]);
				num2 += 1U;
			}
			return list.ToArray();
		}

		public TimeSlotXML[] GetDiagnosticXML()
		{
			DateTime utcNow = TimeProvider.UtcNow;
			ulong[] array = this.ToArray();
			List<TimeSlotXML> list = new List<TimeSlotXML>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > 0UL)
				{
					DateTime time = utcNow.Subtract(TimeSpan.FromMilliseconds((long)i * (long)this.MillisecondsPerSlot));
					list.Add(this.GetSlotXML(time, array[i]));
				}
			}
			return list.ToArray();
		}

		protected const ulong MillisecondsPerMonth = 2592000000UL;

		protected const ulong MillisecondsPerDay = 86400000UL;

		protected const ulong MillisecondsPerHour = 3600000UL;

		protected const ulong MillisecondsPerMinute = 60000UL;
	}
}
