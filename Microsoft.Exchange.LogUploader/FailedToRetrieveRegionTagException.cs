using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class FailedToRetrieveRegionTagException : Exception
	{
		public FailedToRetrieveRegionTagException(string message) : base(message)
		{
		}
	}
}
