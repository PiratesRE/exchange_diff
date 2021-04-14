using System;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	public class PopImapSmtpConnection
	{
		public string EncryptionMethod
		{
			get
			{
				return this.encryptionMethod;
			}
			set
			{
				this.encryptionMethod = value;
			}
		}

		public string Hostname
		{
			get
			{
				return this.hostname;
			}
			set
			{
				this.hostname = value;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
			set
			{
				this.port = value;
			}
		}

		private string encryptionMethod;

		private string hostname;

		private int port;
	}
}
