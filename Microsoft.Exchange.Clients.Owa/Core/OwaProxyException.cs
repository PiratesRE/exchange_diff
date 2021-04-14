using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaProxyException : OwaPermanentException
	{
		public OwaProxyException(string message, string localizedError, Exception innerException, bool hideDebugInformation) : base(message, innerException)
		{
			this.localizedError = localizedError;
			this.hideDebugInformation = hideDebugInformation;
		}

		public OwaProxyException(string message, string localizedError) : this(message, localizedError, null, true)
		{
		}

		public string LocalizedError
		{
			get
			{
				return this.localizedError;
			}
		}

		public bool HideDebugInformation
		{
			get
			{
				return this.hideDebugInformation;
			}
		}

		private string localizedError;

		private bool hideDebugInformation = true;
	}
}
