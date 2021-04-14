using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.Analysis
{
	internal class Results<T> : IEnumerable<Result<T>>, IEnumerable
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
				return (from x in this.results
				where !x.HasException
				select x).Count<Result<T>>();
			}
		}

		public Result<T> Result
		{
			get
			{
				if (this.results.Skip(1).Any<Result<T>>())
				{
					throw new MultipleResultsException(this.Source);
				}
				Result<T> result = this.results.FirstOrDefault<Result<T>>();
				if (result == null)
				{
					throw new EmptyResultsException(this.Source);
				}
				return result;
			}
		}

		public T Value
		{
			get
			{
				return this.Result.Value;
			}
		}

		public T ValueOrDefault
		{
			get
			{
				T result;
				try
				{
					result = this.Result.ValueOrDefault;
				}
				catch
				{
					result = default(T);
				}
				return result;
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
