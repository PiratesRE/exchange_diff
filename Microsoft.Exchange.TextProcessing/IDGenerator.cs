using System;
using System.Threading;

namespace Microsoft.Exchange.TextProcessing
{
	internal class IDGenerator
	{
		public static long GetNextID()
		{
			return Interlocked.Increment(ref IDGenerator.id);
		}

		private static long id;
	}
}
