using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NullLog : Log
	{
		public NullLog() : base(new NullLogEmitter(), LogLevel.LogNone)
		{
		}

		public override void Assert(bool condition, string formatString, params object[] args)
		{
		}

		public override void RetailAssert(bool condition, string formatString, params object[] args)
		{
		}
	}
}
