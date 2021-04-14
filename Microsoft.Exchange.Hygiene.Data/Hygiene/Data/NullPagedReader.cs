using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class NullPagedReader<TResult> : IPagedReader<TResult>, IEnumerable<!0>, IEnumerable where TResult : IConfigurable, new()
	{
		public bool RetrievedAllData
		{
			get
			{
				return true;
			}
		}

		public int PagesReturned
		{
			get
			{
				return 0;
			}
		}

		public int PageSize
		{
			get
			{
				return 0;
			}
		}

		public TResult[] ReadAllPages()
		{
			return new TResult[0];
		}

		public IEnumerator<TResult> GetEnumerator()
		{
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public TResult[] GetNextPage()
		{
			return new TResult[0];
		}
	}
}
