using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Sync.Common.Logging
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncLogBlackBox
	{
		internal SyncLogBlackBox() : this(false)
		{
		}

		internal SyncLogBlackBox(bool ignoreUser)
		{
			this.capacity = SyncLogBlackBox.InitialBlackBoxCapacity;
			this.syncLogs = new string[this.Capacity];
			this.ignoreUser = ignoreUser;
		}

		public int Capacity
		{
			get
			{
				int result;
				lock (this.syncRoot)
				{
					result = this.capacity;
				}
				return result;
			}
			private set
			{
				this.capacity = value;
			}
		}

		public override string ToString()
		{
			string result;
			lock (this.syncRoot)
			{
				if (this.IsEmpty())
				{
					result = SyncLogBlackBox.EmptyOutput;
				}
				else if (!this.IsFull())
				{
					result = string.Join(Environment.NewLine, this.syncLogs, 0, this.end);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder(this.Capacity * 200);
					int num = this.end;
					for (int i = 0; i < this.Capacity; i++)
					{
						stringBuilder.AppendLine(this.syncLogs[num]);
						num = this.ComputeNextIndex(num);
					}
					result = stringBuilder.ToString();
				}
			}
			return result;
		}

		public void ResizeAndClear(int newCapacity)
		{
			lock (this.syncRoot)
			{
				this.Capacity = newCapacity;
				this.InternalClear();
			}
		}

		public void Clear()
		{
			lock (this.syncRoot)
			{
				if (this.Capacity != SyncLogBlackBox.InitialBlackBoxCapacity)
				{
					this.Capacity = SyncLogBlackBox.InitialBlackBoxCapacity;
				}
				this.InternalClear();
			}
		}

		public void Append(LogRowFormatter row)
		{
			lock (this.syncRoot)
			{
				this.syncLogs[this.end] = string.Concat(new object[]
				{
					ExDateTime.UtcNow.ToString(),
					", ",
					row[1].ToString(),
					", ",
					row[2].ToString(),
					", ",
					this.ignoreUser ? string.Empty : row[3],
					", ",
					row[4],
					", ",
					row[5],
					", ",
					row[6],
					", ",
					row[7]
				});
				this.end = this.ComputeNextIndex(this.end);
			}
		}

		private bool IsFull()
		{
			return this.syncLogs[this.end] != null;
		}

		private bool IsEmpty()
		{
			return !this.IsFull() && this.end == 0;
		}

		private int ComputeNextIndex(int index)
		{
			return (index + 1) % this.Capacity;
		}

		private void InternalClear()
		{
			this.end = 0;
			this.syncLogs = new string[this.Capacity];
		}

		private const int EstimateSizePerLine = 200;

		public static readonly int InitialBlackBoxCapacity = 200;

		public static readonly string EmptyOutput = "<empty>";

		private string[] syncLogs;

		private int end;

		private bool ignoreUser;

		private int capacity;

		private object syncRoot = new object();
	}
}
