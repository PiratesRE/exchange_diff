using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IMessageProxy : IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		void SaveChanges();

		void WriteToMime(byte[] buffer);
	}
}
