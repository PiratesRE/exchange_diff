using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaRespondOlderVersionMeetingException : OwaPermanentException
	{
		public string SharerDisplayName { get; private set; }

		public OwaRespondOlderVersionMeetingException(string message, string sharerDisplayName) : base(message)
		{
			this.SharerDisplayName = sharerDisplayName;
		}
	}
}
