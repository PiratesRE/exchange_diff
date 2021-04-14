using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IMapiFxProxyEx : IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		void SetProps(PropValueData[] pvda);

		void SetItemProperties(ItemPropertiesBase props);
	}
}
