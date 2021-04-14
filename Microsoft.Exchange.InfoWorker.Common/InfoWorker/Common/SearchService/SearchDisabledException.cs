using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.SearchService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchDisabledException : LocalizedException
	{
		public SearchDisabledException() : base(Strings.SearchDisabled)
		{
		}
	}
}
