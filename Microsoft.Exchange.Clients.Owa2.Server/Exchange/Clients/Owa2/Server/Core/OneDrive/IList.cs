using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IList : IClientObject<List>
	{
		IFolder RootFolder { get; }

		IListItemCollection GetItems(CamlQuery query);

		IListItem GetItemById(string id);
	}
}
