using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ConsoleLog : Log
	{
		public ConsoleLog() : base(new ConsoleLogEmitter(), LogLevel.LogAll)
		{
		}
	}
}
