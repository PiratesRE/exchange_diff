using System;
using System.Globalization;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal class ProxyAddressHasher
	{
		public ProxyAddressHasher()
		{
			this.hasher = new StringHasher(UsageScenario.Production);
		}

		public ProxyAddressHasher(UsageScenario scenario)
		{
			this.hasher = new StringHasher(scenario);
		}

		public static string GetHashedFormWithoutPrefix(StringHasher stringHasher, string smtpAddress)
		{
			return stringHasher.GetHash(smtpAddress).ToString("X16", NumberFormatInfo.InvariantInfo);
		}

		public string GetHashedFormWithPrefix(string smtpAddress)
		{
			return "sh:" + this.hasher.GetHash(smtpAddress).ToString("X16", NumberFormatInfo.InvariantInfo);
		}

		internal const string SmtpHashPrefix = "sh";

		internal const string SmtpHashPrefixWithColon = "sh:";

		internal const string HashCodeFormat = "X16";

		private StringHasher hasher;
	}
}
