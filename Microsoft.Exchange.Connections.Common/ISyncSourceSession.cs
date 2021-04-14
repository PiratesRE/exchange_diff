using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISyncSourceSession
	{
		string Protocol { get; }

		string SessionId { get; }

		string Server { get; }
	}
}
