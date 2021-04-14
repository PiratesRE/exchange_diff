using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class GetFolderRequestProcessingException : AvailabilityException
	{
		public GetFolderRequestProcessingException(LocalizedString message) : base(ErrorConstants.GetFolderRequestProcessingFailed, message)
		{
		}
	}
}
