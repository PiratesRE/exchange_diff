using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct PCL : IEnumerable<byte[]>, IEnumerable
	{
		public PCL(int initialCapacity)
		{
			this.cnset = new List<byte[]>(initialCapacity);
		}

		public int Count
		{
			get
			{
				if (this.cnset != null)
				{
					return this.cnset.Count;
				}
				return 0;
			}
		}

		public byte[] this[int index]
		{
			get
			{
				return this.cnset[index];
			}
		}

		public void Add(ExchangeId id)
		{
			this.Add(id.To22ByteArray());
		}

		public void Add(byte[] id)
		{
			bool flag = false;
			int i = 0;
			while (i < this.Count)
			{
				if (PCL.GuidsEqual(this.cnset[i], id))
				{
					flag = true;
					if (!PCL.CounterGreaterOrEqual(this.cnset[i], id))
					{
						this.ReplaceEntry(i, id);
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			if (!flag)
			{
				this.AddEntry(id);
			}
		}

		public bool TryLoadBinaryLXCN(byte[] buffer)
		{
			if (buffer != null)
			{
				int num = buffer.Length;
				int i = 0;
				while (i < num)
				{
					if (num - i < 2)
					{
						return false;
					}
					byte b = buffer[i++];
					if (b <= 16 || (int)b > num - i)
					{
						return false;
					}
					byte[] array = new byte[(int)b];
					Buffer.BlockCopy(buffer, i, array, 0, (int)b);
					i += (int)b;
					this.Add(array);
				}
			}
			return true;
		}

		public void LoadBinaryLXCN(byte[] buffer)
		{
			if (!this.TryLoadBinaryLXCN(buffer))
			{
				throw new StoreException((LID)56917U, ErrorCodeValue.InvalidParameter);
			}
		}

		public byte[] DumpBinaryLXCN()
		{
			if (this.Count == 0)
			{
				return PCL.emptyByteArray;
			}
			this.cnset.Sort(new Comparison<byte[]>(PCL.CompareGuids));
			int num = 0;
			for (int i = 0; i < this.cnset.Count; i++)
			{
				num += 1 + this.cnset[i].Length;
			}
			byte[] array = new byte[num];
			int num2 = 0;
			for (int j = 0; j < this.cnset.Count; j++)
			{
				array[num2++] = (byte)this.cnset[j].Length;
				Buffer.BlockCopy(this.cnset[j], 0, array, num2, this.cnset[j].Length);
				num2 += this.cnset[j].Length;
			}
			return array;
		}

		public void LoadBinaryLTID(byte[] buffer)
		{
			if (!this.TryLoadBinaryLTID(buffer))
			{
				throw new StoreException((LID)59512U, ErrorCodeValue.InvalidParameter);
			}
		}

		public bool TryLoadBinaryLTID(byte[] buffer)
		{
			if (buffer != null)
			{
				int num = buffer.Length;
				if (num % 24 != 0)
				{
					return false;
				}
				int i = 0;
				while (i < num)
				{
					byte[] array = new byte[22];
					Buffer.BlockCopy(buffer, i, array, 0, 22);
					i += 24;
					this.Add(array);
				}
			}
			return true;
		}

		public byte[] DumpBinaryLTID()
		{
			if (this.Count == 0)
			{
				return PCL.emptyByteArray;
			}
			this.cnset.Sort(new Comparison<byte[]>(PCL.CompareGuids));
			int num = 0;
			for (int i = 0; i < this.cnset.Count; i++)
			{
				if (this.cnset[i].Length == 22 || this.cnset[i].Length == 24)
				{
					num += 24;
				}
			}
			byte[] array = new byte[num];
			int num2 = 0;
			for (int j = 0; j < this.cnset.Count; j++)
			{
				if (this.cnset[j].Length == 22 || this.cnset[j].Length == 24)
				{
					Buffer.BlockCopy(this.cnset[j], 0, array, num2, this.cnset[j].Length);
					num2 += 24;
				}
			}
			return array;
		}

		public bool IgnoreChange(PCL remotePCL)
		{
			for (int i = 0; i < remotePCL.Count; i++)
			{
				bool flag = false;
				int j = 0;
				while (j < this.Count)
				{
					if (PCL.GuidsEqual(this.cnset[j], remotePCL.cnset[i]))
					{
						flag = true;
						if (!PCL.CounterGreaterOrEqual(this.cnset[j], remotePCL.cnset[i]))
						{
							return false;
						}
						break;
					}
					else
					{
						j++;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public void Merge(PCL remotePCL)
		{
			for (int i = 0; i < remotePCL.Count; i++)
			{
				bool flag = false;
				int j = 0;
				while (j < this.Count)
				{
					if (PCL.GuidsEqual(this.cnset[j], remotePCL.cnset[i]))
					{
						flag = true;
						if (!PCL.CounterGreaterOrEqual(this.cnset[j], remotePCL.cnset[i]))
						{
							this.ReplaceEntry(j, remotePCL.cnset[i]);
							break;
						}
						break;
					}
					else
					{
						j++;
					}
				}
				if (!flag)
				{
					this.AddEntry(remotePCL.cnset[i]);
				}
			}
		}

		public List<byte[]>.Enumerator GetEnumerator()
		{
			if (this.cnset == null)
			{
				return PCL.emptyList.GetEnumerator();
			}
			return this.cnset.GetEnumerator();
		}

		IEnumerator<byte[]> IEnumerable<byte[]>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private static int CompareGuids(byte[] lhs, byte[] rhs)
		{
			for (int i = 0; i < 16; i++)
			{
				if (lhs[i] > rhs[i])
				{
					return 1;
				}
				if (lhs[i] < rhs[i])
				{
					return -1;
				}
			}
			return 0;
		}

		private static bool GuidsEqual(byte[] lhs, byte[] rhs)
		{
			for (int i = 0; i < 16; i++)
			{
				if (rhs[i] != lhs[i])
				{
					return false;
				}
			}
			return true;
		}

		private static bool CounterGreaterOrEqual(byte[] lhs, byte[] rhs)
		{
			if (lhs.Length != rhs.Length)
			{
				throw new StoreException((LID)51320U, ErrorCodeValue.InvalidParameter);
			}
			for (int i = 16; i < rhs.Length; i++)
			{
				if (lhs[i] < rhs[i])
				{
					return false;
				}
				if (lhs[i] > rhs[i])
				{
					break;
				}
			}
			return true;
		}

		private bool AlreadyThere(byte[] cn)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (ValueHelper.ArraysEqual<byte>(this.cnset[i], cn))
				{
					return true;
				}
			}
			return false;
		}

		private void AddEntry(byte[] cn)
		{
			if (this.cnset == null)
			{
				this.cnset = new List<byte[]>(4);
			}
			this.cnset.Add(cn);
		}

		private void ReplaceEntry(int index, byte[] cn)
		{
			this.cnset[index] = cn;
		}

		private const int LengthOfGID = 22;

		private const int LengthOfLTID = 24;

		private static readonly List<byte[]> emptyList = new List<byte[]>(0);

		private static readonly byte[] emptyByteArray = new byte[0];

		private List<byte[]> cnset;
	}
}
