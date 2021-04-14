using System;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	public class AlternateMailbox
	{
		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		public string LegacyDN
		{
			get
			{
				return this.legacyDN;
			}
			set
			{
				this.legacyDN = value;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
			set
			{
				this.server = value;
			}
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
			set
			{
				this.smtpAddress = value;
			}
		}

		public string OwnerSmtpAddress
		{
			get
			{
				return this.ownerSmtpAddress;
			}
			set
			{
				this.ownerSmtpAddress = value;
			}
		}

		private string type;

		private string displayName;

		private string legacyDN;

		private string server;

		private string smtpAddress;

		private string ownerSmtpAddress;
	}
}
