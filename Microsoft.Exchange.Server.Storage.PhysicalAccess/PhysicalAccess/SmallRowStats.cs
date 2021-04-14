using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public struct SmallRowStats
	{
		public bool IsEmpty
		{
			get
			{
				foreach (int num in this.counters)
				{
					if (num != 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		private static SmallRowStatsTableClassIndex[] CreateTableClassIndex()
		{
			SmallRowStatsTableClassIndex[] array = new SmallRowStatsTableClassIndex[66];
			array[1] = SmallRowStatsTableClassIndex.Temp;
			array[2] = SmallRowStatsTableClassIndex.Index;
			array[3] = SmallRowStatsTableClassIndex.Index;
			return array;
		}

		private static SmallRowStatsTableClassIndex GetTableClassIndex(TableClass tableClass)
		{
			return SmallRowStats.tableClassIndex[(int)tableClass];
		}

		private static int GetCounterIndex(SmallRowStatsTableClassIndex tableClassIndex, RowStatsCounterType counterType)
		{
			return (int)(tableClassIndex * (SmallRowStatsTableClassIndex)6 + (int)counterType);
		}

		private static int GetCounterIndex(TableClass tableClass, RowStatsCounterType counterType)
		{
			return SmallRowStats.GetCounterIndex(SmallRowStats.GetTableClassIndex(tableClass), counterType);
		}

		public void Initialize()
		{
			this.counters = new int[18];
		}

		public void IncrementCount(TableClass tableClass, RowStatsCounterType counterType)
		{
			this.counters[SmallRowStats.GetCounterIndex(tableClass, counterType)]++;
		}

		public void AddCount(TableClass tableClass, RowStatsCounterType counterType, int value)
		{
			this.counters[SmallRowStats.GetCounterIndex(tableClass, counterType)] += value;
		}

		private int GetCounter(SmallRowStatsTableClassIndex tableClassIndex, RowStatsCounterType counterType)
		{
			return this.counters[SmallRowStats.GetCounterIndex(tableClassIndex, counterType)];
		}

		private bool IsTableClassEmpty(SmallRowStatsTableClassIndex tableClassIndex)
		{
			return this.GetCounter(tableClassIndex, RowStatsCounterType.Read) == 0 && this.GetCounter(tableClassIndex, RowStatsCounterType.Seek) == 0 && this.GetCounter(tableClassIndex, RowStatsCounterType.Accept) == 0 && this.GetCounter(tableClassIndex, RowStatsCounterType.Write) == 0 && this.GetCounter(tableClassIndex, RowStatsCounterType.ReadBytes) == 0 && 0 == this.GetCounter(tableClassIndex, RowStatsCounterType.WriteBytes);
		}

		public void Aggregate(SmallRowStats other)
		{
			for (int i = 0; i < this.counters.Length; i++)
			{
				this.counters[i] += other.counters[i];
			}
		}

		public override string ToString()
		{
			TraceContentBuilder traceContentBuilder = TraceContentBuilder.Create();
			this.AppendToString(traceContentBuilder);
			return traceContentBuilder.ToString();
		}

		public bool AppendToString(TraceContentBuilder cb)
		{
			bool flag = false;
			if (this.counters != null)
			{
				flag |= this.AppendCountersToString(cb, "IDX", SmallRowStatsTableClassIndex.Index, flag);
				flag |= this.AppendCountersToString(cb, "BASE", SmallRowStatsTableClassIndex.Base, flag);
				flag |= this.AppendCountersToString(cb, "TMP", SmallRowStatsTableClassIndex.Temp, flag);
			}
			return flag;
		}

		private bool AppendCountersToString(TraceContentBuilder cb, string name, SmallRowStatsTableClassIndex tableClassIndex, bool continueList)
		{
			if (!this.IsTableClassEmpty(tableClassIndex))
			{
				if (name != null)
				{
					if (continueList)
					{
						cb.Append(" ");
					}
					cb.Append(name);
					cb.Append(":[");
				}
				continueList = false;
				continueList = RowStats.AppendCounterToString(cb, "r", this.GetCounter(tableClassIndex, RowStatsCounterType.Read), continueList);
				continueList = RowStats.AppendCounterToString(cb, "s", this.GetCounter(tableClassIndex, RowStatsCounterType.Seek), continueList);
				continueList = RowStats.AppendCounterToString(cb, "a", this.GetCounter(tableClassIndex, RowStatsCounterType.Accept), continueList);
				continueList = RowStats.AppendCounterToString(cb, "w", this.GetCounter(tableClassIndex, RowStatsCounterType.Write), continueList);
				continueList = RowStats.AppendCounterToString(cb, "rb", this.GetCounter(tableClassIndex, RowStatsCounterType.ReadBytes), continueList);
				continueList = RowStats.AppendCounterToString(cb, "wb", this.GetCounter(tableClassIndex, RowStatsCounterType.WriteBytes), continueList);
				if (name != null)
				{
					cb.Append("]");
				}
				return true;
			}
			return continueList;
		}

		private static readonly SmallRowStatsTableClassIndex[] tableClassIndex = SmallRowStats.CreateTableClassIndex();

		private int[] counters;
	}
}
