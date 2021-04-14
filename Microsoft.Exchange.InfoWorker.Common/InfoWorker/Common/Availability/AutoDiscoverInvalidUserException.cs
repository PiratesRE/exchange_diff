using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class AutoDiscoverInvalidUserException : AutoDiscoverFailedException
	{
		public AutoDiscoverInvalidUserException(LocalizedString message) : base(message)
		{
		}
	}
}
