using System;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class InvalidContactException : SharingSynchronizationException
	{
		public InvalidContactException() : base(Strings.InvalidContactException)
		{
		}

		public InvalidContactException(Exception innerException) : base(Strings.InvalidContactException, innerException)
		{
		}
	}
}
