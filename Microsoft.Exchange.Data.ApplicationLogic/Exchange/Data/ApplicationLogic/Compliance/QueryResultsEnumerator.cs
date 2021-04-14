using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal abstract class QueryResultsEnumerator : IEnumerator<List<object[]>>, IDisposable, IEnumerator
	{
		protected QueryResultsEnumerator(QueryResult queryResult, int batchSize)
		{
			this.queryResult = queryResult;
			this.batchSize = batchSize;
			this.disposed = false;
		}

		protected QueryResultsEnumerator(QueryResult queryResult) : this(queryResult, 2000)
		{
		}

		public List<object[]> Current
		{
			get
			{
				return this.current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.current;
			}
		}

		public virtual void Dispose()
		{
			if (!this.disposed && this.queryResult != null)
			{
				this.queryResult.Dispose();
				this.disposed = true;
			}
		}

		public virtual bool MoveNext()
		{
			this.current = new List<object[]>(this.batchSize);
			if (this.queryResult == null)
			{
				return false;
			}
			Exception ex = null;
			int i = 0;
			try
			{
				while (i < this.batchSize)
				{
					object[][] rows = this.queryResult.GetRows(100);
					if (rows.Length <= 0)
					{
						break;
					}
					i += rows.Length;
					this.current.AddRange(rows);
				}
			}
			catch (ObjectNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (InvalidFolderLanguageIdException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				this.HandleException(ex);
				i = 0;
			}
			return i > 0;
		}

		public virtual void Reset()
		{
			if (this.queryResult != null)
			{
				this.queryResult.SeekToOffset(SeekReference.OriginBeginning, 0);
			}
		}

		protected abstract void HandleException(Exception ex);

		private readonly QueryResult queryResult;

		private readonly int batchSize;

		private List<object[]> current;

		private bool disposed;
	}
}
