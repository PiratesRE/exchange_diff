using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class CompositePagedReader<TResult> : IPagedReader<TResult>, IEnumerable<TResult>, IEnumerable where TResult : IConfigurable, new()
	{
		public CompositePagedReader(params IPagedReader<TResult>[] subReaders)
		{
			if (subReaders == null)
			{
				throw new ArgumentNullException("subReaders");
			}
			if (subReaders.Length == 0)
			{
				throw new ArgumentException("subReaders must contain 1 or more elements");
			}
			this.readerQueue = new Queue<IPagedReader<TResult>>(subReaders);
			this.RetrievedAllData = false;
			this.PagesReturned = 0;
			this.pageSize = (from r in subReaders
			select r.PageSize).Max();
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
			if (!this.valid)
			{
				throw new InvalidOperationException("GetNextPage() called after reader was marked invalid");
			}
			if (this.RetrievedAllData)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionPagedReaderIsSingleUse);
			}
			TResult[] result;
			try
			{
				IPagedReader<TResult> pagedReader = this.readerQueue.Peek();
				TResult[] nextPage = pagedReader.GetNextPage();
				this.PagesReturned++;
				if (pagedReader.RetrievedAllData)
				{
					this.readerQueue.Dequeue();
					this.RetrievedAllData = (this.readerQueue.Count == 0);
				}
				result = nextPage;
			}
			catch (PermanentDALException)
			{
				this.valid = false;
				this.RetrievedAllData = true;
				throw;
			}
			return result;
		}

		private readonly int pageSize;

		private bool valid = true;

		private Queue<IPagedReader<TResult>> readerQueue;
	}
}
