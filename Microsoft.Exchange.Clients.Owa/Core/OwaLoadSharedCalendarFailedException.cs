using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaLoadSharedCalendarFailedException : OwaPermanentException
	{
		public OwaLoadSharedCalendarFailedException() : base(null)
		{
		}

		public OwaLoadSharedCalendarFailedException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public OwaLoadSharedCalendarFailedException(string message) : base(message)
		{
		}
	}
}
