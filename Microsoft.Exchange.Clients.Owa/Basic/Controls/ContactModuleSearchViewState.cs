using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class ContactModuleSearchViewState : ContactModuleViewState, ISearchViewState
	{
		public ContactModuleSearchViewState(ClientViewState lastClientViewState, StoreObjectId folderId, string folderType, int pageNumber, string searchString, SearchScope searchScope) : base(folderId, folderType, pageNumber)
		{
			this.searchString = searchString;
			this.searchScope = searchScope;
			ISearchViewState searchViewState = lastClientViewState as ISearchViewState;
			if (searchViewState != null)
			{
				this.lastClientViewStateBeforeSearch = searchViewState.ClientViewStateBeforeSearch();
				return;
			}
			this.lastClientViewStateBeforeSearch = lastClientViewState;
		}

		public string SearchString
		{
			get
			{
				return this.searchString;
			}
		}

		public override PreFormActionResponse ToPreFormActionResponse()
		{
			PreFormActionResponse preFormActionResponse = base.ToPreFormActionResponse();
			if (!string.IsNullOrEmpty(this.SearchString))
			{
				preFormActionResponse.AddParameter("sch", this.SearchString);
				PreFormActionResponse preFormActionResponse2 = preFormActionResponse;
				string name = "scp";
				int num = (int)this.searchScope;
				preFormActionResponse2.AddParameter(name, num.ToString());
				string value = OwaContext.Current.UserContext.Key.Canary.CloneRenewed().ToString();
				preFormActionResponse.AddParameter("canary", value);
			}
			return preFormActionResponse;
		}

		public string ClearSearchQueryString()
		{
			return this.lastClientViewStateBeforeSearch.ToQueryString();
		}

		public ClientViewState ClientViewStateBeforeSearch()
		{
			return this.lastClientViewStateBeforeSearch;
		}

		private ClientViewState lastClientViewStateBeforeSearch;

		private string searchString;

		private SearchScope searchScope;
	}
}
