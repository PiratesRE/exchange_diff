using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaSecurityContextSidLimitException : OwaPermanentException
	{
		public OwaSecurityContextSidLimitException(string message, string name, string authenticationType) : base(message)
		{
			this.name = name;
			this.authenticationType = authenticationType;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
		}

		private string name;

		private string authenticationType;
	}
}
