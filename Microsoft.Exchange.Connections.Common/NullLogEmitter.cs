using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NullLogEmitter : ILogEmitter
	{
		public void Emit(string formatString, params object[] args)
		{
		}
	}
}
