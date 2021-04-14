using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaExplicitLogonException : OwaPermanentException
	{
		public OwaExplicitLogonException(string message, string localizedError, Exception innerException) : base(message, innerException)
		{
			this.localizedError = localizedError;
		}

		public OwaExplicitLogonException(string message, string localizedError) : this(message, localizedError, null)
		{
		}

		public string LocalizedError
		{
			get
			{
				return this.localizedError;
			}
		}

		private string localizedError;
	}
}
