using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal class IWTransientException : TransientException
	{
		public IWTransientException(LocalizedString message) : base(message)
		{
		}

		public IWTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
