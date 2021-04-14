using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public interface IConnection<TConnectionClass>
	{
		TConnectionClass Initialize();
	}
}
