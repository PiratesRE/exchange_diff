using System;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IListCollection : IClientObject<ListCollection>
	{
		IList GetByTitle(string title);

		IList GetById(Guid guid);
	}
}
