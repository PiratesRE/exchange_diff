using System;

namespace Microsoft.Exchange.Diagnostics.FaultInjection
{
	[Serializable]
	public class AppDomainInformation
	{
		public AppDomainInformation(string appDomainName) : this(appDomainName, string.Empty)
		{
		}

		public AppDomainInformation(string appDomainName, string identifier)
		{
			this.appDomainName = appDomainName;
			this.identifier = identifier;
		}

		public string AppDomainName
		{
			get
			{
				return this.appDomainName;
			}
		}

		public string Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		private readonly string appDomainName;

		private readonly string identifier;
	}
}
