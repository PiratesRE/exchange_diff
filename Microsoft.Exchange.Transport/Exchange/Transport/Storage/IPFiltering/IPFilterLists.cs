using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Storage.IPFiltering
{
	internal static class IPFilterLists
	{
		public static IPFilterList AddressAllowList
		{
			get
			{
				return IPFilterLists.addressAllowList;
			}
		}

		public static IPFilterList AddressDenyList
		{
			get
			{
				return IPFilterLists.addressDenyList;
			}
		}

		public static void Load()
		{
			using (DataTableCursor cursor = Database.Table.GetCursor())
			{
				using (Transaction transaction = cursor.BeginTransaction())
				{
					IPFilterLists.BulkLoader bulkLoader = new IPFilterLists.BulkLoader();
					bulkLoader.Scan(transaction, cursor, true);
				}
			}
			IPFilterLists.AddressAllowList.Sort();
			IPFilterLists.AddressDenyList.Sort();
		}

		public static void Cleanup()
		{
			DateTime utcNow = DateTime.UtcNow;
			IPFilterLists.AddressDenyList.Cleanup(utcNow);
			IPFilterLists.AddressAllowList.Cleanup(utcNow);
			IPFilterLists.cleanupScanner.Cleanup(utcNow);
		}

		public static int AddRestriction(IPFilterRange range)
		{
			IPFilterRow ipfilterRow = new IPFilterRow();
			ipfilterRow.LowerBound = range.LowerBound;
			ipfilterRow.UpperBound = range.UpperBound;
			ipfilterRow.ExpiresOn = range.ExpiresOn;
			ipfilterRow.TypeFlags = range.TypeFlags;
			ipfilterRow.Comment = range.Comment;
			ipfilterRow.Commit();
			range.PurgeComment();
			range.Identity = ipfilterRow.Identity;
			if (range.PolicyType == PolicyType.Allow)
			{
				IPFilterLists.AddressAllowList.Add(range);
			}
			else
			{
				IPFilterLists.AddressDenyList.Add(range);
			}
			return ipfilterRow.Identity;
		}

		public static int AdminRemove(int identity, int filter)
		{
			using (DataTableCursor cursor = Database.Table.GetCursor())
			{
				using (Transaction transaction = cursor.BeginTransaction())
				{
					IPFilterRow ipfilterRow = new IPFilterRow(identity);
					if (ipfilterRow.TrySeekCurrent(cursor))
					{
						ipfilterRow = IPFilterRow.LoadFromRow(cursor);
						if ((ipfilterRow.TypeFlags & 240) == (filter & 240))
						{
							IPFilterRange range = IPFilterRange.FromRowWithoutComment(ipfilterRow);
							ipfilterRow.MarkToDelete();
							ipfilterRow.Commit(transaction, cursor);
							IPFilterLists.RemoveFromMemoryLists(range);
							transaction.Commit();
							return 1;
						}
					}
				}
			}
			return 0;
		}

		public static List<IPFilterRow> AdminGetItems(int startIdentity, int flags, IPvxAddress address, int count)
		{
			if (startIdentity < 0)
			{
				throw new InvalidOperationException("startIdentity must be zero or grater");
			}
			if ((flags & -4081) != 0)
			{
				throw new InvalidOperationException("flags can only specifiy 0x0ff0 nybbles");
			}
			if (count <= 0)
			{
				throw new InvalidOperationException("count must be positive");
			}
			List<IPFilterRow> result;
			using (DataTableCursor cursor = Database.Table.GetCursor())
			{
				using (Transaction transaction = cursor.BeginTransaction())
				{
					IPFilterRow ipfilterRow = new IPFilterRow(startIdentity);
					if (!ipfilterRow.TrySeekCurrentPrefix(cursor, 1))
					{
						result = new List<IPFilterRow>();
					}
					else
					{
						IPFilterLists.IPFilterAdminScanner ipfilterAdminScanner = new IPFilterLists.IPFilterAdminScanner();
						result = ipfilterAdminScanner.AdminGetItems(transaction, cursor, flags, address, count);
					}
				}
			}
			return result;
		}

		private static void RemoveFromMemoryLists(IPFilterRange range)
		{
			if (range.PolicyType == PolicyType.Allow)
			{
				IPFilterLists.AddressAllowList.Remove(range);
				return;
			}
			IPFilterLists.AddressDenyList.Remove(range);
		}

		private static IPFilterList addressAllowList = new IPFilterList();

		private static IPFilterList addressDenyList = new IPFilterList();

		private static IPFilterLists.CleanupScanner cleanupScanner = new IPFilterLists.CleanupScanner();

		private class BulkLoader : ChunkingScanner
		{
			protected override ChunkingScanner.ScanControl HandleRecord(DataTableCursor cursor)
			{
				IPFilterRow ipfilterRow = IPFilterRow.LoadFromRow(cursor);
				if (ipfilterRow.ExpiresOn > DateTime.UtcNow)
				{
					if (ipfilterRow.Policy == PolicyType.Allow)
					{
						IPFilterLists.AddressAllowList.Add(ipfilterRow);
					}
					else
					{
						IPFilterLists.AddressDenyList.Add(ipfilterRow);
					}
				}
				return ChunkingScanner.ScanControl.Continue;
			}
		}

		private class IPFilterAdminScanner : ChunkingScanner
		{
			public List<IPFilterRow> AdminGetItems(Transaction transaction, DataTableCursor cursor, int flags, IPvxAddress filter, int maximum)
			{
				this.mode = IPFilterLists.IPFilterAdminScanner.Mode.GetItems;
				this.list = new List<IPFilterRow>();
				this.filterFlags = flags;
				this.filterAddress = filter;
				this.maxCount = maximum;
				base.ScanFromCurrentPosition(transaction, cursor, true);
				return this.list;
			}

			protected override ChunkingScanner.ScanControl HandleRecord(DataTableCursor cursor)
			{
				IPFilterRow ipfilterRow = IPFilterRow.LoadFromRow(cursor);
				if (!this.MatchesOrWild(240, ipfilterRow.TypeFlags))
				{
					return ChunkingScanner.ScanControl.Continue;
				}
				if (!this.MatchesOrWild(3840, ipfilterRow.TypeFlags))
				{
					return ChunkingScanner.ScanControl.Continue;
				}
				if (this.filterAddress != IPvxAddress.None)
				{
					IPFilterRange ipfilterRange = IPFilterRange.FromRowWithoutComment(ipfilterRow);
					if (!ipfilterRange.Contains(this.filterAddress))
					{
						return ChunkingScanner.ScanControl.Continue;
					}
				}
				if (this.mode == IPFilterLists.IPFilterAdminScanner.Mode.GetItems)
				{
					this.list.Add(ipfilterRow);
					if (this.list.Count >= this.maxCount)
					{
						return ChunkingScanner.ScanControl.Stop;
					}
				}
				else
				{
					if (this.mode != IPFilterLists.IPFilterAdminScanner.Mode.RemoveItems)
					{
						throw new InvalidOperationException();
					}
					IPFilterRange range = IPFilterRange.FromRowWithoutComment(ipfilterRow);
					ipfilterRow.MarkToDelete();
					using (Transaction transaction = cursor.Connection.BeginTransaction())
					{
						ipfilterRow.Commit(transaction, cursor);
						transaction.Commit();
					}
					IPFilterLists.RemoveFromMemoryLists(range);
					this.removeCount++;
				}
				return ChunkingScanner.ScanControl.Continue;
			}

			private bool MatchesOrWild(int nybbleMask, int flags)
			{
				int num = this.filterFlags & nybbleMask;
				return num == nybbleMask || num == (flags & nybbleMask);
			}

			private IPFilterLists.IPFilterAdminScanner.Mode mode;

			private List<IPFilterRow> list;

			private int filterFlags;

			private int maxCount;

			private int removeCount;

			private IPvxAddress filterAddress;

			private enum Mode
			{
				GetItems,
				RemoveItems
			}
		}

		private sealed class CleanupScanner : ChunkingScanner
		{
			public void Cleanup(DateTime now)
			{
				this.now = now;
				this.entriesRemoved = 0;
				using (DataTableCursor cursor = Database.Table.GetCursor())
				{
					using (Transaction transaction = cursor.BeginTransaction())
					{
						this.Scan(transaction, cursor, true);
						transaction.Commit();
					}
				}
			}

			protected override ChunkingScanner.ScanControl HandleRecord(DataTableCursor cursor)
			{
				IPFilterRow ipfilterRow = IPFilterRow.LoadFromRow(cursor);
				IPFilterRange ipfilterRange = IPFilterRange.FromRowWithoutComment(ipfilterRow);
				if (ipfilterRange.PolicyType == PolicyType.Deny && !ipfilterRange.AdminCreated && this.now > ipfilterRange.ExpiresOn + IPFilterLists.CleanupScanner.ExpirationThreshold)
				{
					ipfilterRow.MarkToDelete();
					using (Transaction transaction = cursor.Connection.BeginTransaction())
					{
						ipfilterRow.Commit(transaction, cursor);
						transaction.Commit();
					}
					this.entriesRemoved++;
				}
				if (this.entriesRemoved >= 10000)
				{
					return ChunkingScanner.ScanControl.Stop;
				}
				return ChunkingScanner.ScanControl.Continue;
			}

			private const int MaxEntriesCleanedPerRun = 10000;

			private static readonly TimeSpan ExpirationThreshold = new TimeSpan(14, 0, 0, 0);

			private DateTime now;

			private int entriesRemoved;
		}
	}
}
