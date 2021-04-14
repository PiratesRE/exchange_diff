using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.IPFilter;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class IPListDataProvider : IConfigDataProvider
	{
		public IPListDataProvider(string machineName)
		{
			this.connection = new IPFilterRpcClient(machineName);
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			int index = ((IPListEntryIdentity)identity).Index;
			IPFilterRange[] items = this.GetItems(index, this.GetFilterMask<T>(), IPvxAddress.None, 1);
			if (items != null && items.Length > 0 && items[0].Identity == index)
			{
				return IPListEntry.NewIPListEntry<T>(items[0]);
			}
			return null;
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			return new IPListDataProvider.PagedReader<T>(this, filter, pageSize);
		}

		public IConfigurable[] Find<T>(QueryFilter queryFilter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			LinkedList<IConfigurable> linkedList = new LinkedList<IConfigurable>();
			IPListDataProvider.PagedReader<T> pagedReader = new IPListDataProvider.PagedReader<T>(this, queryFilter);
			foreach (T t in pagedReader)
			{
				IConfigurable value = t;
				linkedList.AddLast(value);
				if (linkedList.Count >= 1000)
				{
					break;
				}
			}
			IConfigurable[] array = new IConfigurable[linkedList.Count];
			linkedList.CopyTo(array, 0);
			return array;
		}

		public void Save(IConfigurable instance)
		{
			IPListEntry entry = (IPListEntry)instance;
			int num = (int)RpcClientHelper.Invoke(() => this.connection.Add(entry.ListType == IPListEntryType.Block, entry.ToIPFilterRange()));
			if (num == -1)
			{
				throw new DataSourceOperationException(Strings.IPListEntryExists(entry.IPRange.ToString()));
			}
			entry.Identity = new IPListEntryIdentity(num);
		}

		public void Delete(IConfigurable instance)
		{
			IPListEntry entry = (IPListEntry)instance;
			RpcClientHelper.Invoke(delegate
			{
				this.connection.Remove(((IPListEntryIdentity)entry.Identity).Index, entry.ListType == IPListEntryType.Block);
				return null;
			});
		}

		public string Source
		{
			get
			{
				return null;
			}
		}

		private int GetFilterMask<T>() where T : IConfigurable, new()
		{
			IPListEntry iplistEntry = (IPListEntry)((object)((default(T) == null) ? Activator.CreateInstance<T>() : default(T)));
			switch (iplistEntry.ListType)
			{
			case IPListEntryType.Allow:
				return 3856;
			case IPListEntryType.Block:
				return 3872;
			default:
				throw new ArgumentException("Invalid IPListEntryType.  This should never happen.");
			}
		}

		private IPFilterRange[] GetItems(int startIndex, int typeFilter, IPvxAddress address, int count)
		{
			return (IPFilterRange[])RpcClientHelper.Invoke(() => this.connection.GetItems(startIndex, typeFilter, (ulong)(address >> 64), (ulong)address, count));
		}

		private IPFilterRpcClient connection;

		private struct EntryTypeMask
		{
			public const int Allow = 3856;

			public const int Block = 3872;
		}

		private sealed class PagedReader<T> : IEnumerable<!0>, IEnumerable where T : IConfigurable, new()
		{
			public PagedReader(IPListDataProvider provider, QueryFilter filter, int pageSize)
			{
				if (provider == null)
				{
					throw new ArgumentNullException("provider");
				}
				if (pageSize < 0)
				{
					throw new ArgumentOutOfRangeException("pageSize", "page size must be greater than or equal to zero.");
				}
				this.provider = provider;
				this.filter = (filter as IPListQueryFilter);
				this.pageSize = ((pageSize == 0) ? 1000 : pageSize);
			}

			public PagedReader(IPListDataProvider provider, QueryFilter filter) : this(provider, filter, 1000)
			{
			}

			public IEnumerator<T> GetEnumerator()
			{
				IEnumerator<IPFilterRange[]> pages = this.GetPages();
				while (pages.MoveNext())
				{
					foreach (IPFilterRange range in pages.Current)
					{
						yield return IPListEntry.NewIPListEntry<T>(range);
					}
				}
				yield break;
			}

			private IEnumerator<IPFilterRange[]> GetPages()
			{
				bool finished = false;
				int startIndex = 0;
				IPvxAddress address = (this.filter != null) ? this.filter.Address : IPvxAddress.None;
				int listTypeMask = this.provider.GetFilterMask<T>();
				while (!finished)
				{
					IPFilterRange[] matches = this.provider.GetItems(startIndex, listTypeMask, address, this.pageSize);
					if (matches == null || matches.Length <= 0)
					{
						finished = true;
					}
					else
					{
						yield return matches;
						startIndex = matches[matches.Length - 1].Identity + 1;
					}
				}
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private const int DefaultPageSize = 1000;

			private readonly IPListDataProvider provider;

			private readonly IPListQueryFilter filter;

			private readonly int pageSize;
		}
	}
}
