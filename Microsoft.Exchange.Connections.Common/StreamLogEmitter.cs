using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StreamLogEmitter : ILogEmitter
	{
		internal StreamLogEmitter(StreamWriter writer)
		{
			this.Writer = writer;
		}

		private protected StreamWriter Writer { protected get; private set; }

		public void Emit(string formatString, params object[] args)
		{
			this.Writer.WriteLine(formatString, args);
		}
	}
}
