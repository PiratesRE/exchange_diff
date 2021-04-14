using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class TimePeriodBigMap<TKey, TValue> where TValue : IInitialize, new()
	{
		public TimePeriodBigMap(bool mergePrevious, int internalNoOfStore, int initialCapacity = 71993) : this(internalNoOfStore, TimePeriodBigMap<TKey, TValue>.defaultSwapTime, TimePeriodBigMap<TKey, TValue>.defaultMergeTime, TimePeriodBigMap<TKey, TValue>.defaultCleanTime, initialCapacity)
		{
			this.mergePrevious = mergePrevious;
		}

		public TimePeriodBigMap() : this(31, TimePeriodBigMap<TKey, TValue>.defaultSwapTime, TimePeriodBigMap<TKey, TValue>.defaultMergeTime, TimePeriodBigMap<TKey, TValue>.defaultCleanTime, 71993)
		{
			this.mergePrevious = true;
		}

		public TimePeriodBigMap(int internalNoOfStore, TimeSpan swapTime, TimeSpan mergeTime, TimeSpan cleanTime, int initialCapacity = 71993)
		{
			if (mergeTime >= cleanTime || cleanTime >= swapTime)
			{
				throw new ArgumentException("it should be mergeTime < cleanTime < swapTime");
			}
			this.maps = new BigMap<TKey, TValue>[2];
			this.maps[0] = new BigMap<TKey, TValue>(internalNoOfStore, initialCapacity);
			this.maps[1] = new BigMap<TKey, TValue>(internalNoOfStore, initialCapacity);
			this.SwapTime = swapTime;
			this.MergeTime = mergeTime;
			this.CleanTime = cleanTime;
			this.mergePrevious = true;
		}

		public TimeSpan SwapTime { get; private set; }

		public TimeSpan MergeTime { get; private set; }

		public TimeSpan CleanTime { get; private set; }

		public int Count
		{
			get
			{
				return this.maps[0].Count + this.maps[1].Count;
			}
		}

		public BigMap<TKey, TValue> GetCurrentMap(DateTime dateTime)
		{
			int num;
			return this.GetCurrentMap(dateTime, out num);
		}

		public BigMap<TKey, TValue> GetCurrentMap(DateTime dateTime, out int currentIdx)
		{
			currentIdx = (int)((dateTime.Ticks / this.SwapTime.Ticks + 1L) % 2L);
			BigMap<TKey, TValue> bigMap = this.maps[currentIdx];
			if (bigMap.TimeStamp != this.StartBoundaryTime(dateTime))
			{
				bigMap.Clear();
				bigMap.TimeStamp = this.StartBoundaryTime(dateTime);
			}
			return bigMap;
		}

		public TValue GetValue(DateTime dateTime, TKey key)
		{
			int num;
			BigMap<TKey, TValue> currentMap = this.GetCurrentMap(dateTime, out num);
			if (dateTime - currentMap.TimeStamp > this.CleanTime)
			{
				BigMap<TKey, TValue> previousMap = this.GetPreviousMap(dateTime);
				if (previousMap.TimeStamp != currentMap.TimeStamp + this.SwapTime)
				{
					previousMap.Clear();
					previousMap.TimeStamp = currentMap.TimeStamp + this.SwapTime;
				}
			}
			TValue tvalue;
			if (currentMap.TryGetValue(key, out tvalue))
			{
				return tvalue;
			}
			if (this.mergePrevious && dateTime - currentMap.TimeStamp < this.MergeTime)
			{
				BigMap<TKey, TValue> previousMap2 = this.GetPreviousMap(dateTime);
				if (previousMap2.TimeStamp == this.PreviousStartBoundaryTime(dateTime))
				{
					previousMap2.TryGetValue(key, out tvalue);
				}
			}
			if (object.Equals(tvalue, default(TValue)))
			{
				tvalue = ((default(TValue) == null) ? Activator.CreateInstance<TValue>() : default(TValue));
			}
			return currentMap.AddOrGet(key, tvalue, delegate
			{
			});
		}

		public bool TryGetValue(DateTime dateTime, TKey key, out TValue value)
		{
			BigMap<TKey, TValue> currentMap = this.GetCurrentMap(dateTime);
			if (dateTime - currentMap.TimeStamp > this.CleanTime)
			{
				BigMap<TKey, TValue> previousMap = this.GetPreviousMap(dateTime);
				if (previousMap.TimeStamp != currentMap.TimeStamp + this.SwapTime)
				{
					previousMap.Clear();
					previousMap.TimeStamp = currentMap.TimeStamp + this.SwapTime;
				}
			}
			if (currentMap.TryGetValue(key, out value))
			{
				return true;
			}
			if (this.mergePrevious && dateTime - currentMap.TimeStamp < this.MergeTime)
			{
				BigMap<TKey, TValue> previousMap2 = this.GetPreviousMap(dateTime);
				if (previousMap2.TimeStamp == this.PreviousStartBoundaryTime(dateTime) && previousMap2.TryGetValue(key, out value) && !object.Equals(value, default(TValue)))
				{
					currentMap.GetOrAdd(key, value);
					return true;
				}
			}
			return false;
		}

		public TValue GetOrAdd(DateTime dateTime, TKey key, TValue value)
		{
			BigMap<TKey, TValue> currentMap = this.GetCurrentMap(dateTime);
			return currentMap.GetOrAdd(key, value);
		}

		public TValue GetOrAdd(DateTime dateTime, TKey key, Func<TKey, TValue> valueFactory)
		{
			BigMap<TKey, TValue> currentMap = this.GetCurrentMap(dateTime);
			return currentMap.GetOrAdd(key, valueFactory);
		}

		public TValue AddOrUpdate(DateTime dateTime, TKey key, TValue value, Func<TKey, TValue, TValue> updateValueFactory)
		{
			BigMap<TKey, TValue> currentMap = this.GetCurrentMap(dateTime);
			return currentMap.AddOrUpdate(key, value, updateValueFactory);
		}

		internal BigMap<TKey, TValue> GetPreviousMap(DateTime dateTime)
		{
			return this.maps[(int)(dateTime.Ticks / this.SwapTime.Ticks % 2L)];
		}

		private DateTime StartBoundaryTime(DateTime dateTime)
		{
			return new DateTime(dateTime.Ticks - dateTime.Ticks % this.SwapTime.Ticks);
		}

		private DateTime PreviousStartBoundaryTime(DateTime dateTime)
		{
			return new DateTime(dateTime.Ticks - dateTime.Ticks % this.SwapTime.Ticks - this.SwapTime.Ticks);
		}

		private static TimeSpan defaultSwapTime = TimeSpan.FromMinutes(5.0);

		private static TimeSpan defaultMergeTime = TimeSpan.FromMinutes(1.0);

		private static TimeSpan defaultCleanTime = TimeSpan.FromMinutes(2.0);

		private readonly BigMap<TKey, TValue>[] maps;

		private readonly bool mergePrevious;
	}
}
