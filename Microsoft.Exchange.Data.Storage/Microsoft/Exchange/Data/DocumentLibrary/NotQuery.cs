using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotQuery : Query
	{
		internal NotQuery(Query query)
		{
			this.Query = query;
		}

		public override bool IsMatch(object[] row)
		{
			return !this.Query.IsMatch(row);
		}

		private readonly Query Query;
	}
}
