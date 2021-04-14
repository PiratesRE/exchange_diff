using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("CM")]
	[OwaEventObjectId(typeof(OwaStoreObjectId))]
	[OwaEventSegmentation(Feature.Contacts)]
	internal sealed class ContactModuleEventHandler : ContactVirtualListViewEventHandler
	{
		protected override ViewType ViewType
		{
			get
			{
				return ViewType.ContactModule;
			}
		}

		public const string EventNamespace = "CM";
	}
}
