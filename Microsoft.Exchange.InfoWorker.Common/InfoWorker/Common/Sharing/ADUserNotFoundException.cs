using System;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class ADUserNotFoundException : SharingSynchronizationException
	{
		public ADUserNotFoundException() : base(Strings.ADUserNotFoundException)
		{
		}

		public ADUserNotFoundException(Exception innerException) : base(Strings.ADUserNotFoundException, innerException)
		{
		}
	}
}
