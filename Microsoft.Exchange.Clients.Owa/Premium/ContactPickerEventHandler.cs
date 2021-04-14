using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventSegmentation(Feature.Contacts)]
	[OwaEventNamespace("CP")]
	[OwaEventObjectId(typeof(OwaStoreObjectId))]
	internal sealed class ContactPickerEventHandler : ContactVirtualListViewEventHandler
	{
		protected override ViewType ViewType
		{
			get
			{
				return ViewType.ContactPicker;
			}
		}

		protected override void PersistMultiLineState()
		{
			AddressBookViewState.PersistMultiLineState(base.UserContext, this.ListViewState.IsMultiLine, true);
		}

		public const string EventNamespace = "CP";
	}
}
