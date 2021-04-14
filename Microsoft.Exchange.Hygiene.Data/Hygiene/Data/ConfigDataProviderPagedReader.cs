using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class ConfigDataProviderPagedReader<TResult> : IPagedReader<TResult>, IEnumerable<TResult>, IEnumerable where TResult : IConfigurable, new()
	{
		public ConfigDataProviderPagedReader(IConfigDataProvider dataProvider, ADObjectId rootId, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			if (dataProvider == null)
			{
				throw new ArgumentNullException("dataProvider");
			}
			if (pageSize < 0 || pageSize > 10000)
			{
				throw new ArgumentOutOfRangeException("pageSize", pageSize, string.Format("pageSize should be between 1 and {0} or 0 to use the default page size: {1}", 10000, ConfigDataProviderPagedReader<TResult>.DefaultPageSize));
			}
			this.dataProvider = dataProvider;
			this.rootId = rootId;
			this.filter = filter;
			this.sortBy = sortBy;
			this.pageSize = ((pageSize == 0) ? ConfigDataProviderPagedReader<TResult>.DefaultPageSize : pageSize);
			this.RetrievedAllData = false;
			this.PagesReturned = 0;
		}

		public bool RetrievedAllData { get; private set; }

		public int PagesReturned { get; private set; }

		public int PageSize
		{
			get
			{
				return this.pageSize;
			}
		}

		public TResult[] ReadAllPages()
		{
			if (this.RetrievedAllData)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionPagedReaderIsSingleUse);
			}
			if (this.PagesReturned > 0)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionPagedReaderReadAllAfterEnumerating);
			}
			List<TResult> list = new List<TResult>();
			foreach (TResult item in this)
			{
				list.Add(item);
			}
			return list.ToArray();
		}

		public IEnumerator<TResult> GetEnumerator()
		{
			if (this.RetrievedAllData)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionPagedReaderIsSingleUse);
			}
			while (!this.RetrievedAllData)
			{
				TResult[] results = this.GetNextPage();
				foreach (TResult result in results)
				{
					yield return result;
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public TResult[] GetNextPage()
		{
			TResult[] result = null;
			QueryFilter pagingFilter = null;
			bool retrievedAllData = true;
			if (!this.valid)
			{
				throw new InvalidOperationException("GetNextPage() called after reader was marked invalid");
			}
			if (this.RetrievedAllData)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionPagedReaderIsSingleUse);
			}
			try
			{
				pagingFilter = PagingHelper.GetPagingQueryFilter(this.filter, this.cookie);
				result = this.dataProvider.FindPaged<TResult>(pagingFilter, this.rootId, true, this.sortBy, this.pageSize).ToArray<TResult>();
			}
			catch (PermanentDALException)
			{
				this.valid = false;
				this.RetrievedAllData = true;
				throw;
			}
			this.cookie = PagingHelper.GetProcessedCookie(pagingFilter, out retrievedAllData);
			this.RetrievedAllData = retrievedAllData;
			this.PagesReturned++;
			return result;
		}

		public const int MaximumPageSize = 10000;

		public static readonly int DefaultPageSize = 1000;

		private readonly IConfigDataProvider dataProvider;

		private readonly ADObjectId rootId;

		private readonly QueryFilter filter;

		private readonly SortBy sortBy;

		private readonly int pageSize;

		private string cookie;

		private bool valid = true;
	}
}
