using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public interface INamedObject
	{
		string Name { get; }

		string DetailedName { get; }
	}
}
