using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Storage.IPFiltering
{
	internal class IPFilterList
	{
		public int Count
		{
			get
			{
				return this.singleAddresses.Count + this.rangeAddresses.Count;
			}
		}

		public void Add(IPFilterRange range)
		{
			this.syncLock.TryEnterWriteLock(int.MaxValue);
			try
			{
				if (range.LowerBound == range.UpperBound)
				{
					IPFilterList.Insert(this.singleAddresses, range);
				}
				else
				{
					IPFilterList.Insert(this.rangeAddresses, range);
				}
			}
			finally
			{
				this.syncLock.ExitWriteLock();
			}
		}

		public IPFilterRange Search(IPvxAddress address)
		{
			IPFilterRange address2 = new IPFilterRange(address);
			this.syncLock.TryEnterReadLock(int.MaxValue);
			try
			{
				IPFilterRange ipfilterRange = this.SearchSingles(address2);
				if (ipfilterRange != null)
				{
					return ipfilterRange;
				}
				ipfilterRange = this.SearchRanges(address2);
				if (ipfilterRange != null)
				{
					return ipfilterRange;
				}
			}
			finally
			{
				this.syncLock.ExitReadLock();
			}
			return null;
		}

		public bool ContainsAdminIPRange(IPFilterRange range)
		{
			this.syncLock.TryEnterReadLock(int.MaxValue);
			bool result;
			try
			{
				foreach (IPFilterRange ipfilterRange in this.singleAddresses)
				{
					if (range.AdminCreated && ipfilterRange.Equals(range))
					{
						return true;
					}
				}
				foreach (IPFilterRange ipfilterRange2 in this.rangeAddresses)
				{
					if (range.AdminCreated && ipfilterRange2.Equals(range))
					{
						return true;
					}
				}
				result = false;
			}
			finally
			{
				this.syncLock.ExitReadLock();
			}
			return result;
		}

		public void Remove(IPFilterRange target)
		{
			this.syncLock.TryEnterWriteLock(int.MaxValue);
			try
			{
				if (target.LowerBound == target.UpperBound)
				{
					IPFilterList.RemoveFromList(this.singleAddresses, target);
				}
				else
				{
					IPFilterList.RemoveFromList(this.rangeAddresses, target);
				}
			}
			finally
			{
				this.syncLock.ExitWriteLock();
			}
		}

		public void Cleanup(DateTime now)
		{
			this.syncLock.TryEnterWriteLock(int.MaxValue);
			try
			{
				for (int i = 0; i < this.singleAddresses.Count; i++)
				{
					IPFilterRange ipfilterRange = this.singleAddresses[i];
					if (ipfilterRange.IsExpired(now))
					{
						this.singleAddresses.RemoveAt(i);
						i--;
					}
				}
				for (int j = 0; j < this.rangeAddresses.Count; j++)
				{
					IPFilterRange ipfilterRange2 = this.rangeAddresses[j];
					if (ipfilterRange2.IsExpired(now))
					{
						this.rangeAddresses.RemoveAt(j);
						j--;
					}
				}
			}
			finally
			{
				this.syncLock.ExitWriteLock();
			}
		}

		internal void Sort()
		{
			this.singleAddresses.Sort();
			this.rangeAddresses.Sort();
		}

		internal void Add(IPFilterRow row)
		{
			IPFilterRange ipfilterRange = IPFilterRange.FromRowWithoutComment(row);
			if (ipfilterRange.LowerBound == ipfilterRange.UpperBound)
			{
				this.singleAddresses.Add(ipfilterRange);
				return;
			}
			this.rangeAddresses.Add(ipfilterRange);
		}

		private static void Insert(List<IPFilterRange> list, IPFilterRange range)
		{
			int num = list.BinarySearch(range);
			if (num == ~list.Count)
			{
				list.Add(range);
				return;
			}
			if (num < 0)
			{
				num = ~num;
			}
			list.Insert(num, range);
		}

		private static void RemoveFromList(List<IPFilterRange> list, IPFilterRange target)
		{
			int num = list.BinarySearch(target);
			if (num >= 0)
			{
				for (int i = num; i < list.Count; i++)
				{
					IPFilterRange ipfilterRange = list[i];
					if (ipfilterRange.Identity == target.Identity)
					{
						list.RemoveAt(i);
						return;
					}
					if (ipfilterRange.LowerBound != target.LowerBound)
					{
						break;
					}
				}
				for (int j = num - 1; j >= 0; j--)
				{
					IPFilterRange ipfilterRange2 = list[j];
					if (ipfilterRange2.Identity == target.Identity)
					{
						list.RemoveAt(j);
						return;
					}
					if (ipfilterRange2.LowerBound != target.LowerBound)
					{
						return;
					}
				}
			}
		}

		private IPFilterRange SearchSingles(IPFilterRange address)
		{
			int num = this.singleAddresses.BinarySearch(address);
			if (num < 0)
			{
				return null;
			}
			DateTime utcNow = DateTime.UtcNow;
			for (int i = num; i < this.singleAddresses.Count; i++)
			{
				IPFilterRange ipfilterRange = this.singleAddresses[i];
				if (ipfilterRange.LowerBound != address.LowerBound)
				{
					break;
				}
				if (!ipfilterRange.IsExpired(utcNow))
				{
					return ipfilterRange;
				}
			}
			for (int j = num - 1; j >= 0; j--)
			{
				IPFilterRange ipfilterRange2 = this.singleAddresses[j];
				if (ipfilterRange2.LowerBound != address.LowerBound)
				{
					break;
				}
				if (!ipfilterRange2.IsExpired(utcNow))
				{
					return ipfilterRange2;
				}
			}
			return null;
		}

		private IPFilterRange SearchRanges(IPFilterRange address)
		{
			int num = this.rangeAddresses.BinarySearch(address);
			if (num < ~this.rangeAddresses.Count)
			{
				return null;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (num >= 0)
			{
				for (int i = num; i < this.rangeAddresses.Count; i++)
				{
					IPFilterRange ipfilterRange = this.rangeAddresses[i];
					if (ipfilterRange.LowerBound != address.LowerBound)
					{
						break;
					}
					if (!ipfilterRange.IsExpired(utcNow))
					{
						return ipfilterRange;
					}
				}
			}
			else
			{
				num = ~num;
			}
			for (int j = num - 1; j >= 0; j--)
			{
				IPFilterRange ipfilterRange2 = this.rangeAddresses[j];
				if (!ipfilterRange2.IsExpired(utcNow) && ipfilterRange2.Contains(address.LowerBound))
				{
					return ipfilterRange2;
				}
			}
			return null;
		}

		private List<IPFilterRange> singleAddresses = new List<IPFilterRange>();

		private List<IPFilterRange> rangeAddresses = new List<IPFilterRange>();

		private ReaderWriterLockSlim syncLock = new ReaderWriterLockSlim();
	}
}
