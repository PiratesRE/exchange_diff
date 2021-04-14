using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventObjectId(typeof(ADObjectId))]
	[OwaEventNamespace("RB")]
	internal sealed class RoomBrowserEventHandler : DirectoryVirtualListViewEventHandler
	{
		protected override ViewType ViewType
		{
			get
			{
				return ViewType.RoomBrowser;
			}
		}

		public const string EventNamespace = "RB";
	}
}
