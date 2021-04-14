using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.SearchService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchOverBudgetException : LocalizedException
	{
		public SearchOverBudgetException(int budget) : base(Strings.SearchOverBudget(budget))
		{
		}
	}
}
