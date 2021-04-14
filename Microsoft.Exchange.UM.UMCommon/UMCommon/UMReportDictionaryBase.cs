using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class UMReportDictionaryBase : Dictionary<DateTime, UMReportRawCounters>
	{
		public abstract int MaxItemsInDictionary { get; }
	}
}
