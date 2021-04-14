using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaInvalidCanary14Exception : OwaPermanentException
	{
		public UserContextCookie UserContextCookie
		{
			get
			{
				return this.userContextCookie;
			}
		}

		public OwaInvalidCanary14Exception(UserContextCookie newUserContextCookie, string message = null) : base(message)
		{
			this.userContextCookie = newUserContextCookie;
		}

		private UserContextCookie userContextCookie;
	}
}
