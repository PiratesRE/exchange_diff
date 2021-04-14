using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class MessageModuleViewState : ListViewViewState
	{
		public MessageModuleViewState(StoreObjectId folderId, string folderType, SecondaryNavigationArea selectedUsing, int pageNumber) : base(NavigationModule.Mail, folderId, folderType, pageNumber)
		{
			this.selectedUsing = selectedUsing;
		}

		public SecondaryNavigationArea SelectedUsing
		{
			get
			{
				return this.selectedUsing;
			}
		}

		public override PreFormActionResponse ToPreFormActionResponse()
		{
			PreFormActionResponse preFormActionResponse = base.ToPreFormActionResponse();
			PreFormActionResponse preFormActionResponse2 = preFormActionResponse;
			string name = "slUsng";
			int num = (int)this.selectedUsing;
			preFormActionResponse2.AddParameter(name, num.ToString());
			return preFormActionResponse;
		}

		private SecondaryNavigationArea selectedUsing;
	}
}
