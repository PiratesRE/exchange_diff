using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAutoProvision
	{
		string[] Hostnames { get; }

		int[] ConnectivePorts { get; }
	}
}
