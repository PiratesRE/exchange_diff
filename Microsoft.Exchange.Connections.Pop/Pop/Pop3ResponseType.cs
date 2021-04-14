using System;

namespace Microsoft.Exchange.Connections.Pop
{
	internal enum Pop3ResponseType
	{
		ok,
		err,
		unknown,
		sendMore
	}
}
