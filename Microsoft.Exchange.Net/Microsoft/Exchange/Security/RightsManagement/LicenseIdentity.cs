using System;
using System.Text;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal sealed class LicenseIdentity
	{
		public LicenseIdentity(string email, string[] proxyAddresses)
		{
			this.Email = email;
			this.ProxyAddresses = proxyAddresses;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Email);
			if (this.ProxyAddresses != null && this.ProxyAddresses.Length > 0)
			{
				stringBuilder.Append(",");
				for (int i = 0; i < this.ProxyAddresses.Length; i++)
				{
					stringBuilder.Append(this.ProxyAddresses[i]);
					if (i != this.ProxyAddresses.Length - 1)
					{
						stringBuilder.Append(",");
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToString(LicenseIdentity[] licenseIdentities)
		{
			if (licenseIdentities == null)
			{
				throw new ArgumentNullException("licenseIdentities");
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = Math.Min(licenseIdentities.Length, 10);
			for (int i = 0; i < num; i++)
			{
				stringBuilder.Append(licenseIdentities[i].ToString());
				if (i != num - 1)
				{
					stringBuilder.Append(" | ");
				}
			}
			if (num < licenseIdentities.Length)
			{
				stringBuilder.Append(" ... (" + licenseIdentities.Length + ")");
			}
			return stringBuilder.ToString();
		}

		private const int MaxLicenseIdentitiesToTrace = 10;

		public readonly string Email;

		public readonly string[] ProxyAddresses;
	}
}
