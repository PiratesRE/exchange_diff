using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class IWShutdownException : LocalizedException
	{
		public IWShutdownException() : base(LocalizedString.Empty)
		{
		}
	}
}
