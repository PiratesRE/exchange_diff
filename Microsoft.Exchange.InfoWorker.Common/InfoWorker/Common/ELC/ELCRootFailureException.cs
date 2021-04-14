using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class ELCRootFailureException : IWTransientException
	{
		public ELCRootFailureException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		public ELCRootFailureException(LocalizedString message) : base(message)
		{
		}
	}
}
