using System;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	public class OwaUrl
	{
		public string AuthenticationMethods
		{
			get
			{
				return this.authenticationMethods;
			}
			set
			{
				this.authenticationMethods = value;
			}
		}

		public string Url
		{
			get
			{
				return this.url;
			}
			set
			{
				this.url = value;
			}
		}

		private string authenticationMethods;

		private string url;
	}
}
