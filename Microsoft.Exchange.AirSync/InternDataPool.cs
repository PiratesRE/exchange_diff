using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync
{
	internal class InternDataPool<DataType> where DataType : class
	{
		public InternDataPool(int initSize)
		{
			this.cache = new Dictionary<DataType, InternDataPool<DataType>.DataRecord>(initSize);
		}

		public DataType Intern(DataType data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			DataType result;
			lock (this.thisLock)
			{
				InternDataPool<DataType>.DataRecord dataRecord;
				if (this.cache.TryGetValue(data, out dataRecord))
				{
					result = dataRecord.Data;
					dataRecord.Count += 1L;
				}
				else
				{
					result = data;
					this.cache.Add(data, new InternDataPool<DataType>.DataRecord(data));
				}
			}
			return result;
		}

		public void Release(DataType data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			lock (this.thisLock)
			{
				InternDataPool<DataType>.DataRecord dataRecord;
				if (this.cache.TryGetValue(data, out dataRecord))
				{
					long count = dataRecord.Count;
					if (count <= 1L)
					{
						this.cache.Remove(data);
					}
					else
					{
						dataRecord.Count -= 1L;
					}
				}
			}
		}

		private object thisLock = new object();

		private Dictionary<DataType, InternDataPool<DataType>.DataRecord> cache;

		private class DataRecord
		{
			public DataRecord(DataType data)
			{
				this.Data = data;
				this.Count = 1L;
			}

			public DataType Data { get; set; }

			public long Count { get; set; }
		}
	}
}
