using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class TransientDataProviderUnavailableException : TransientDALException
	{
		public TransientDataProviderUnavailableException(string message) : base(message)
		{
		}

		public TransientDataProviderUnavailableException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
