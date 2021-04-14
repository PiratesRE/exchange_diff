using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IFolderProxy : IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
	{
		IMessageProxy OpenMessage(byte[] entryId);

		IMessageProxy CreateMessage(bool isAssociated);

		void DeleteMessage(byte[] entryId);
	}
}
