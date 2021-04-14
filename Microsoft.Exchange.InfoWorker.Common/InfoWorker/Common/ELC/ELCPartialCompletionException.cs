using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class ELCPartialCompletionException : LocalizedException
	{
		public ELCPartialCompletionException(LocalizedString message) : base(message)
		{
		}
	}
}
