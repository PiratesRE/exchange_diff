using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.OData.Model
{
	public class FindEntitiesResult<T> : IFindEntitiesResult<T>, IFindEntitiesResult, IEnumerable<T>, IEnumerable
	{
		public FindEntitiesResult(IEnumerable<T> entities, int totalCount)
		{
			this.entities = entities;
			this.totalCount = totalCount;
		}

		public int TotalCount
		{
			get
			{
				return this.totalCount;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.entities.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly IEnumerable<T> entities;

		private readonly int totalCount;
	}
}
