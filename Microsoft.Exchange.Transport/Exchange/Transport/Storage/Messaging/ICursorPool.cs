using System;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal interface ICursorPool
	{
		bool ReturnTo(DataTableCursor cursor);
	}
}
