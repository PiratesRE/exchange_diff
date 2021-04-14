using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InternalCategoryEntry
	{
		public int SpinLock;

		public int CategoryNameHashCode;

		public int CategoryNameOffset;

		public int FirstInstanceOffset;

		public int NextCategoryOffset;

		public int IsConsistent;
	}
}
