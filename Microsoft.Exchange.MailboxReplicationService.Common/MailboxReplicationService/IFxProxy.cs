using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IFxProxy : IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		void Flush();
	}
}
