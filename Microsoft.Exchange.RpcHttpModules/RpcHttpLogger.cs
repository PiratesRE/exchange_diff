using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcHttpModules
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RpcHttpLogger : ExtensibleLogger
	{
		public RpcHttpLogger() : base(new RpcHttpLogConfiguration())
		{
		}
	}
}
