using System;
using System.IO;

namespace Microsoft.Exchange.Data.Internal
{
	internal abstract class StreamOnDataStorage : Stream
	{
		public abstract DataStorage Storage { get; }

		public abstract long Start { get; }

		public abstract long End { get; }

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
