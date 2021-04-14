using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class DataPoolBase<DataType> where DataType : class
	{
		protected virtual int MaxNumberOfEntries
		{
			get
			{
				return 500;
			}
		}

		public DataType Intern(DataType data)
		{
			if (data == null)
			{
				return default(DataType);
			}
			DataType data2 = this.GetData(data);
			if (data2 == null && this.dataDataTable.Count < this.MaxNumberOfEntries)
			{
				uint hashCode;
				byte[] bytes;
				this.ProcessData(data, out hashCode, out bytes);
				this.SetData(hashCode, data, bytes);
				return data;
			}
			return data2;
		}

		public DataType GetData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			uint hashCode;
			int startIndex;
			int length;
			this.ProcessStream(reader, componentDataPool, out hashCode, out startIndex, out length);
			MemoryStream memoryStream = (MemoryStream)componentDataPool.ConstStringDataReader.BaseStream;
			return this.GetData(hashCode, memoryStream.GetBuffer(), startIndex, length);
		}

		protected void SetData(uint hashCode, DataType data, byte[] bytes)
		{
			try
			{
				this.poolLock.EnterWriteLock();
				DataPoolBase<DataType>.PoolDataList poolDataList;
				if (!this.hashDataTable.TryGetValue(hashCode, out poolDataList))
				{
					poolDataList = new DataPoolBase<DataType>.PoolDataList();
					poolDataList.AddData(data, bytes);
					this.hashDataTable.Add(hashCode, poolDataList);
					this.dataDataTable.Add(data, data);
				}
				else if (!poolDataList.Contains(data))
				{
					poolDataList.AddData(data, bytes);
					this.dataDataTable.Add(data, data);
				}
			}
			finally
			{
				try
				{
					this.poolLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		protected DataType GetData(uint hashCode, byte[] bytes, int startIndex, int length)
		{
			try
			{
				this.poolLock.EnterReadLock();
				DataPoolBase<DataType>.PoolDataList poolDataList;
				if (this.hashDataTable.TryGetValue(hashCode, out poolDataList))
				{
					return poolDataList.GetData(bytes, startIndex, length);
				}
			}
			finally
			{
				try
				{
					this.poolLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return default(DataType);
		}

		protected DataType GetData(DataType data)
		{
			try
			{
				this.poolLock.EnterReadLock();
				DataType result;
				if (this.dataDataTable.TryGetValue(data, out result))
				{
					return result;
				}
			}
			finally
			{
				try
				{
					this.poolLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return default(DataType);
		}

		protected abstract void ProcessStream(BinaryReader reader, ComponentDataPool componentDataPool, out uint hashCode, out int startIndex, out int length);

		protected abstract void ProcessData(DataType data, out uint hashCode, out byte[] bytes);

		private ReaderWriterLockSlim poolLock = new ReaderWriterLockSlim();

		private Dictionary<uint, DataPoolBase<DataType>.PoolDataList> hashDataTable = new Dictionary<uint, DataPoolBase<DataType>.PoolDataList>(100);

		private Dictionary<DataType, DataType> dataDataTable = new Dictionary<DataType, DataType>(100);

		private class PoolData
		{
			public PoolData(DataType data, byte[] bytes)
			{
				this.data = data;
				this.bytes = bytes;
			}

			public DataType Data
			{
				get
				{
					return this.data;
				}
			}

			public byte[] Bytes
			{
				get
				{
					return this.bytes;
				}
			}

			public bool Equals(DataType data)
			{
				return data != null && data.Equals(this.data);
			}

			public bool Equals(byte[] bytes, int startIndex, int length)
			{
				if (bytes == null || this.bytes.Length != length)
				{
					return false;
				}
				for (int i = 0; i < this.bytes.Length; i++)
				{
					if (bytes[i + startIndex] != this.bytes[i])
					{
						return false;
					}
				}
				return true;
			}

			private DataType data;

			private byte[] bytes;
		}

		private class PoolDataList
		{
			public DataType GetData(byte[] bytes, int startIndex, int length)
			{
				foreach (DataPoolBase<DataType>.PoolData poolData in this.dataList)
				{
					if (poolData.Equals(bytes, startIndex, length))
					{
						return poolData.Data;
					}
				}
				return default(DataType);
			}

			public void AddData(DataType data, byte[] bytes)
			{
				this.dataList.Add(new DataPoolBase<DataType>.PoolData(data, bytes));
			}

			public bool Contains(DataType data)
			{
				foreach (DataPoolBase<DataType>.PoolData poolData in this.dataList)
				{
					if (poolData.Equals(data))
					{
						return true;
					}
				}
				return false;
			}

			private List<DataPoolBase<DataType>.PoolData> dataList = new List<DataPoolBase<DataType>.PoolData>(5);
		}
	}
}
