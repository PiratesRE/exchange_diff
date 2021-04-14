using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class ContactModuleViewState : ListViewViewState
	{
		public ContactModuleViewState(StoreObjectId folderId, string folderType, int pageNumber) : base(NavigationModule.Contacts, folderId, folderType, pageNumber)
		{
		}
	}
}
