using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public class Results<T> : IEnumerable<Result<!0>>, IEnumerable
	{
		public Results(AnalysisMember source, IEnumerable<Result<T>> results)
		{
			this.Source = source;
			this.results = results;
		}

		public AnalysisMember Source { get; private set; }

		public int Count
		{
			get
			{
				return this.results.Count((Result<T> x) => !x.HasException);
			}
		}

		public Result<T> this[int index]
		{
			get
			{
				return (from x in this.results
				where !x.HasException
				select x).Skip(index).First<Result<T>>();
			}
		}

		public IEnumerator<Result<T>> GetEnumerator()
		{
			return this.results.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.results.GetEnumerator();
		}

		private IEnumerable<Result<T>> results;
	}
}
