using System;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class AutodiscoverFailedException : SharingSynchronizationException
	{
		public AutodiscoverFailedException() : base(Strings.AutodiscoverFailedException)
		{
		}

		public AutodiscoverFailedException(Exception innerException, Exception additionalException) : base(Strings.AutodiscoverFailedException, innerException)
		{
			base.AdditionalException = additionalException;
		}
	}
}
