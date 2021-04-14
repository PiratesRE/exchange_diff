using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal static class Utility
	{
		internal static bool IsValidAddress(RoutingAddress address)
		{
			return address.IsValid && !(address == RoutingAddress.NullReversePath);
		}

		internal static void Separate(string address, out string local, out string domain)
		{
			RoutingAddress address2 = (RoutingAddress)address;
			if (!Utility.IsValidAddress(address2))
			{
				local = null;
				domain = null;
			}
			local = address2.LocalPart;
			domain = address2.DomainPart;
		}
	}
}
