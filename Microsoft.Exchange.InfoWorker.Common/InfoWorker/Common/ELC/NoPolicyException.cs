using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class NoPolicyException : LocalizedException
	{
		public NoPolicyException() : base(LocalizedString.Empty)
		{
		}
	}
}
