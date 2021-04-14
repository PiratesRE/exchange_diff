using System;

namespace Microsoft.Exchange.Data
{
	internal static class MeumProxyAddressFactory
	{
		public static MeumProxyAddress CreateFromAddressString(string address, bool primaryAddress)
		{
			if (MeumProxyAddressE164.ValidateAddress(address))
			{
				return new MeumProxyAddressE164(address, primaryAddress);
			}
			if (MeumProxyAddressGateway.ValidateAddress(address))
			{
				return new MeumProxyAddressGateway(address, primaryAddress);
			}
			throw new ArgumentOutOfRangeException(DataStrings.ExceptionInvalidMeumAddress(address ?? "<null>"), null);
		}

		public static MeumProxyAddress CreateFromE164(string phoneNumber, bool primaryAddress)
		{
			return MeumProxyAddressE164.CreateFromE164(phoneNumber, primaryAddress);
		}

		public static MeumProxyAddress CreateFromGatewayGuid(Guid gatewayObjectGuid, bool primaryAddress)
		{
			return MeumProxyAddressGateway.CreateFromGuid(gatewayObjectGuid, primaryAddress);
		}
	}
}
