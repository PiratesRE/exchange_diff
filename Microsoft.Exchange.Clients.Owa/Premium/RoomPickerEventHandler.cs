using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventObjectId(typeof(ADObjectId))]
	[OwaEventNamespace("RP")]
	internal sealed class RoomPickerEventHandler : DirectoryVirtualListViewEventHandler
	{
		protected override ViewType ViewType
		{
			get
			{
				return ViewType.RoomPicker;
			}
		}

		public const string EventNamespace = "RP";
	}
}
