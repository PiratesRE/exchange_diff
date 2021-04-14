using System;

namespace Microsoft.Exchange.Search.Query
{
	internal class RefinerDataEntry
	{
		internal RefinerDataEntry(string displayName, long hitCount, string refinementQuery)
		{
			InstantSearch.ThrowOnNullOrEmptyArgument(displayName, "displayName");
			InstantSearch.ThrowOnNullOrEmptyArgument(refinementQuery, "refinementQuery");
			this.DisplayName = displayName;
			this.HitCount = hitCount;
			this.RefinementQuery = refinementQuery;
		}

		public string DisplayName { get; private set; }

		public long HitCount { get; private set; }

		public string RefinementQuery { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"DisplayName=",
				this.DisplayName,
				", HitCount=",
				this.HitCount,
				", RefinementQuery=",
				this.RefinementQuery
			});
		}
	}
}
