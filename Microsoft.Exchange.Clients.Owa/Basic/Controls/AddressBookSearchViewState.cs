using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal sealed class AddressBookSearchViewState : AddressBookViewState, ISearchViewState
	{
		public AddressBookSearchViewState(ClientViewState lastClientViewState, AddressBook.Mode mode, string addressBookToSearch, string searchString, int pageNumber, StoreObjectId itemId, string itemChangeKey, RecipientItemType recipientWell, ColumnId sortColumnId, SortOrder sortOrder) : base(lastClientViewState, mode, pageNumber, itemId, itemChangeKey, recipientWell, sortColumnId, sortOrder)
		{
			this.searchLocation = addressBookToSearch;
			this.searchString = searchString;
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

		public string SearchLocation
		{
			get
			{
				return this.searchLocation;
			}
		}

		public override PreFormActionResponse ToPreFormActionResponse()
		{
			PreFormActionResponse preFormActionResponse = base.ToPreFormActionResponse();
			if (!string.IsNullOrEmpty(this.searchString))
			{
				preFormActionResponse.AddParameter("sch", this.searchString);
				if (!string.IsNullOrEmpty(this.searchLocation))
				{
					preFormActionResponse.AddParameter("ab", this.searchLocation);
				}
				string value = OwaContext.Current.UserContext.Key.Canary.CloneRenewed().ToString();
				preFormActionResponse.AddParameter("canary", value);
			}
			return preFormActionResponse;
		}

		public string ClearSearchQueryString()
		{
			if (!AddressBook.IsEditingMode(base.Mode) || this.lastClientViewStateBeforeSearch is AddressBookViewState)
			{
				return this.lastClientViewStateBeforeSearch.ToQueryString();
			}
			return "?" + base.ToPreFormActionResponse().GetUrl();
		}

		public ClientViewState ClientViewStateBeforeSearch()
		{
			return this.lastClientViewStateBeforeSearch;
		}

		private ClientViewState lastClientViewStateBeforeSearch;

		private string searchLocation;

		private string searchString;
	}
}
