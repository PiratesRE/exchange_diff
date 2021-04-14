using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("DB")]
	[OwaEventObjectId(typeof(ADObjectId))]
	internal sealed class DirectoryBrowserEventHandler : DirectoryVirtualListViewEventHandler
	{
		protected override ViewType ViewType
		{
			get
			{
				return ViewType.DirectoryBrowser;
			}
		}

		protected override void PersistReadingPane(ReadingPanePosition readingPanePosition)
		{
			AddressBookViewState.PersistReadingPane(base.UserContext, readingPanePosition);
		}

		protected override void PersistMultiLineState()
		{
			AddressBookViewState.PersistMultiLineState(base.UserContext, this.ListViewState.IsMultiLine, false);
		}

		public const string EventNamespace = "DB";
	}
}
