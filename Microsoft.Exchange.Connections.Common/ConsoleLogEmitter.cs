using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConsoleLogEmitter : ILogEmitter
	{
		public void Emit(string formatString, params object[] args)
		{
			ConsoleColor foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			this.WriteInternal(formatString, args);
			Console.ForegroundColor = foregroundColor;
		}

		private void WriteInternal(string formatString, params object[] args)
		{
			if (args == null || args.Length == 0)
			{
				Console.WriteLine(formatString);
				return;
			}
			Console.WriteLine(formatString, args);
		}
	}
}
