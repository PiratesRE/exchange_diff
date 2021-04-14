using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventSegmentation(Feature.Contacts)]
	[OwaEventNamespace("CB")]
	[OwaEventObjectId(typeof(OwaStoreObjectId))]
	internal sealed class ContactBrowserEventHandler : ContactVirtualListViewEventHandler
	{
		protected override ViewType ViewType
		{
			get
			{
				return ViewType.ContactBrowser;
			}
		}

		[OwaEvent("PersistReadingPane")]
		[OwaEventParameter("s", typeof(ReadingPanePosition))]
		public override void PersistReadingPane()
		{
			AddressBookViewState.PersistReadingPane(base.UserContext, (ReadingPanePosition)base.GetParameter("s"));
		}

		protected override void PersistMultiLineState()
		{
			AddressBookViewState.PersistMultiLineState(base.UserContext, this.ListViewState.IsMultiLine, false);
		}

		public const string EventNamespace = "CB";
	}
}
