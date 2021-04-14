using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class AddressBookViewState : ClientViewState
	{
		public AddressBookViewState(ClientViewState lastClientViewState, AddressBook.Mode mode, int pageNumber, StoreObjectId itemId, string itemChangeKey, RecipientItemType recipientWell, ColumnId sortColumnId, SortOrder sortOrder)
		{
			this.mode = mode;
			this.pageNumber = pageNumber;
			this.itemId = itemId;
			this.itemChangeKey = itemChangeKey;
			this.recipientWell = recipientWell;
			this.sortColumnId = sortColumnId;
			this.sortOrder = sortOrder;
			AddressBookViewState addressBookViewState = lastClientViewState as AddressBookViewState;
			if (addressBookViewState != null)
			{
				this.previousViewState = addressBookViewState.PreviousViewState;
				return;
			}
			this.previousViewState = lastClientViewState;
		}

		public AddressBook.Mode Mode
		{
			get
			{
				return this.mode;
			}
		}

		public RecipientItemType RecipientWell
		{
			get
			{
				return this.recipientWell;
			}
		}

		public ClientViewState PreviousViewState
		{
			get
			{
				return this.previousViewState;
			}
		}

		public StoreObjectId ItemId
		{
			get
			{
				return this.itemId;
			}
		}

		public string ItemChangeKey
		{
			get
			{
				return this.itemChangeKey;
			}
		}

		public override PreFormActionResponse ToPreFormActionResponse()
		{
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.Dialog;
			preFormActionResponse.Type = "AddressBook";
			PreFormActionResponse preFormActionResponse2 = preFormActionResponse;
			string name = "ctx";
			int num = (int)this.mode;
			preFormActionResponse2.AddParameter(name, num.ToString());
			if (this.pageNumber > 0)
			{
				preFormActionResponse.AddParameter("pg", this.pageNumber.ToString());
			}
			if (this.itemId != null)
			{
				preFormActionResponse.AddParameter("id", this.itemId.ToBase64String());
			}
			PreFormActionResponse preFormActionResponse3 = preFormActionResponse;
			string name2 = "rw";
			int num2 = (int)this.recipientWell;
			preFormActionResponse3.AddParameter(name2, num2.ToString());
			PreFormActionResponse preFormActionResponse4 = preFormActionResponse;
			string name3 = "cid";
			int num3 = (int)this.sortColumnId;
			preFormActionResponse4.AddParameter(name3, num3.ToString());
			PreFormActionResponse preFormActionResponse5 = preFormActionResponse;
			string name4 = "so";
			int num4 = (int)this.sortOrder;
			preFormActionResponse5.AddParameter(name4, num4.ToString());
			return preFormActionResponse;
		}

		private AddressBook.Mode mode;

		private int pageNumber;

		private StoreObjectId itemId;

		private string itemChangeKey;

		private RecipientItemType recipientWell;

		private ColumnId sortColumnId;

		private SortOrder sortOrder;

		private ClientViewState previousViewState;
	}
}
