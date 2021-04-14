using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class MeumProxyAddressGateway : MeumProxyAddress
	{
		public MeumProxyAddressGateway(string address, bool primaryAddress) : base(address, primaryAddress)
		{
			if (!MeumProxyAddressGateway.ValidateAddress(address))
			{
				throw new ArgumentOutOfRangeException(DataStrings.ExceptionInvalidMeumAddress(address ?? "<null>"), null);
			}
		}

		internal static MeumProxyAddressGateway CreateFromGuid(Guid gatewayObjectGuid, bool primaryAddress)
		{
			SmtpAddress smtpAddress = new SmtpAddress(gatewayObjectGuid.ToString("D").ToLowerInvariant(), "umgateway.exchangelabs.com");
			if (!smtpAddress.IsValidAddress)
			{
				throw new ArgumentOutOfRangeException("gatewayObjectGuid", gatewayObjectGuid, string.Format("Invalid SMTP address - {0}", smtpAddress.ToString()));
			}
			return new MeumProxyAddressGateway(smtpAddress.ToString(), primaryAddress);
		}

		internal static bool ValidateAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			SmtpAddress smtpAddress = new SmtpAddress(address);
			if (!smtpAddress.IsValidAddress)
			{
				return false;
			}
			Guid empty = Guid.Empty;
			return GuidHelper.TryParseGuid(smtpAddress.Local, out empty) && string.Equals(smtpAddress.Domain, "umgateway.exchangelabs.com", StringComparison.OrdinalIgnoreCase);
		}

		private const string Domain = "umgateway.exchangelabs.com";
	}
}
