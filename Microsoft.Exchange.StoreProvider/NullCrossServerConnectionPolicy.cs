using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NullCrossServerConnectionPolicy : ICrossServerConnectionPolicy
	{
		public void Apply(ExRpcConnectionInfo connectionInfo)
		{
		}
	}
}
