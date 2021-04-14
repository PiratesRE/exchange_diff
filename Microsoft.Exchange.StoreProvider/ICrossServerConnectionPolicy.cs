using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ICrossServerConnectionPolicy
	{
		void Apply(ExRpcConnectionInfo connectionInfo);
	}
}
