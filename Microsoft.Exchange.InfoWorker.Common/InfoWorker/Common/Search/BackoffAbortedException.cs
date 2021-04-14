using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class BackoffAbortedException : LocalizedException
	{
		public BackoffAbortedException() : base(Strings.SearchAborted)
		{
		}
	}
}
