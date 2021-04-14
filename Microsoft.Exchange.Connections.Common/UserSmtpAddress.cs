using System;

namespace Microsoft.Exchange.Connections.Common
{
	[Serializable]
	public struct UserSmtpAddress
	{
		public UserSmtpAddress(string localPart, string domainPart)
		{
			this.address = localPart + "@" + domainPart;
			this.local = localPart;
			this.domain = domainPart;
		}

		public string Address
		{
			get
			{
				return this.address;
			}
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public string Local
		{
			get
			{
				return this.local;
			}
		}

		public static explicit operator string(UserSmtpAddress address)
		{
			return address.address ?? string.Empty;
		}

		private readonly string address;

		private readonly string domain;

		private readonly string local;
	}
}
