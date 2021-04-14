using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class MeumProxyAddressE164 : MeumProxyAddress
	{
		public MeumProxyAddressE164(string address, bool primaryAddress) : base(address, primaryAddress)
		{
			if (!MeumProxyAddressE164.ValidateAddress(address))
			{
				throw new ArgumentOutOfRangeException(DataStrings.ExceptionInvalidMeumAddress(address ?? "<null>"), null);
			}
		}

		internal static MeumProxyAddressE164 CreateFromE164(string phoneNumber, bool primaryAddress)
		{
			if (!MeumProxyAddressE164.ValidateE164Number(phoneNumber))
			{
				throw new ArgumentOutOfRangeException("phoneNumber", phoneNumber, "Invalid E164 number");
			}
			SmtpAddress smtpAddress = new SmtpAddress(phoneNumber.Substring(1), "um.exchangelabs.com");
			if (!smtpAddress.IsValidAddress)
			{
				throw new ArgumentOutOfRangeException("phoneNumber", phoneNumber, string.Format("Invalid SMTP address - {0}", smtpAddress.ToString()));
			}
			return new MeumProxyAddressE164(smtpAddress.ToString(), primaryAddress);
		}

		internal static bool ValidateAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			SmtpAddress smtpAddress = new SmtpAddress(address);
			return smtpAddress.IsValidAddress && MeumProxyAddressE164.IsNumber(smtpAddress.Local) && string.Equals(smtpAddress.Domain, "um.exchangelabs.com", StringComparison.OrdinalIgnoreCase);
		}

		internal static bool ValidateE164Number(string phoneNumber)
		{
			return !string.IsNullOrEmpty(phoneNumber) && phoneNumber[0] == '+' && phoneNumber.Length > 1 && MeumProxyAddressE164.IsNumber(phoneNumber.Substring(1));
		}

		private static bool IsNumber(string phoneNumber)
		{
			if (string.IsNullOrEmpty(phoneNumber))
			{
				return false;
			}
			for (int i = 0; i < phoneNumber.Length; i++)
			{
				if (!char.IsDigit(phoneNumber[i]))
				{
					return false;
				}
			}
			return true;
		}

		private const string Domain = "um.exchangelabs.com";
	}
}
