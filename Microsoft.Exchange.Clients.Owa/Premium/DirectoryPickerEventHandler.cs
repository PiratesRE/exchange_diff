using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventObjectId(typeof(ADObjectId))]
	[OwaEventNamespace("DP")]
	internal sealed class DirectoryPickerEventHandler : DirectoryVirtualListViewEventHandler
	{
		protected override ViewType ViewType
		{
			get
			{
				return ViewType.DirectoryPicker;
			}
		}

		protected override void PersistMultiLineState()
		{
			AddressBookViewState.PersistMultiLineState(base.UserContext, this.ListViewState.IsMultiLine, true);
		}

		public const string EventNamespace = "DP";
	}
}
