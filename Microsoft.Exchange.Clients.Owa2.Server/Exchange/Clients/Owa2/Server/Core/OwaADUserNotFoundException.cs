using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public class OwaADUserNotFoundException : OwaADObjectNotFoundException
	{
		public OwaADUserNotFoundException(string userName) : this(userName, null)
		{
		}

		public OwaADUserNotFoundException(string userName, string message, Exception innerException) : base(message, innerException)
		{
			this.UserName = userName;
		}

		public OwaADUserNotFoundException(string userName, string message) : this(userName, message, null)
		{
		}

		public string UserName { get; private set; }
	}
}
