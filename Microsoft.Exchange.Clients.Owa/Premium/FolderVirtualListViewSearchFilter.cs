using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Search;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventStruct("FVLVSF")]
	internal class FolderVirtualListViewSearchFilter
	{
		public SearchResultsIn ResultsIn
		{
			get
			{
				return (SearchResultsIn)this.ResultsInInt;
			}
		}

		public const string StructNamespace = "FVLVSF";

		public const string ReExecuteSearchName = "res";

		public const string AsyncSearch = "asrchsup";

		public const string SearchScopeName = "scp";

		public const string SearchStringName = "srch";

		public const string ResultsInName = "ri";

		public const string CategoryName = "cat";

		public const string RecipientName = "rcp";

		public const string RecipientValueName = "rcps";

		[OwaEventField("res", true, null)]
		public bool ReExecuteSearch;

		[OwaEventField("asrchsup", true, null)]
		public bool IsAsyncSearchEnabled;

		[OwaEventField("scp")]
		public SearchScope Scope;

		[OwaEventField("srch", true, null)]
		public string SearchString;

		[OwaEventField("ri", true, 0)]
		public int ResultsInInt;

		[OwaEventField("cat", true, null)]
		public string Category;

		[OwaEventField("rcp", true, null)]
		public SearchRecipient RecipientType;

		[OwaEventField("rcps", true, null)]
		public string RecipientValue;
	}
}
