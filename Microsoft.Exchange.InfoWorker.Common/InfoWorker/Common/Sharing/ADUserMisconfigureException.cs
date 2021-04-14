using System;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class ADUserMisconfigureException : SharingSynchronizationException
	{
		public ADUserMisconfigureException() : base(Strings.ADUserMisconfiguredException)
		{
		}

		public ADUserMisconfigureException(Exception innerException) : base(Strings.ADUserMisconfiguredException, innerException)
		{
		}
	}
}
