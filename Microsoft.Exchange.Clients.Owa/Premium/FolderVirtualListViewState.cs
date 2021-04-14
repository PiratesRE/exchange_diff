using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventStruct("FVLVS")]
	[OwaEventObjectId(typeof(OwaStoreObjectId))]
	public class FolderVirtualListViewState : VirtualListViewState
	{
		public const string StructNamespace = "FVLVS";
	}
}
