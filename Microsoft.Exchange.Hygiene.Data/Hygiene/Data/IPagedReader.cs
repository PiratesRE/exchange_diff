using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal interface IPagedReader<TResult> : IEnumerable<TResult>, IEnumerable where TResult : IConfigurable, new()
	{
		bool RetrievedAllData { get; }

		int PagesReturned { get; }

		int PageSize { get; }

		TResult[] ReadAllPages();

		TResult[] GetNextPage();
	}
}
