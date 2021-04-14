using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal class EwsODataAddressFinder : IAddressFinder
	{
		IRoutingKey[] IAddressFinder.Find(AddressFinderSource source, IAddressFinderDiagnostics diagnostics)
		{
			AddressFinderHelper.ThrowIfNull(source, diagnostics);
			if (!string.IsNullOrWhiteSpace(source.Url.PathAndQuery))
			{
				Match match = Constants.UsersEntityRegex.Match(source.Url.PathAndQuery);
				if (match.Success)
				{
					string address = match.Result("${address}");
					if (SmtpAddress.IsValidSmtpAddress(address))
					{
						IRoutingKey routingKey = new SmtpRoutingKey(new SmtpAddress(address));
						diagnostics.AddRoutingkey(routingKey, "TargetMailbox-SMTP");
						return AddressFinderHelper.GetRoutingKeyArray(new IRoutingKey[]
						{
							routingKey
						});
					}
				}
			}
			return AddressFinderHelper.EmptyRoutingKeyArray;
		}
	}
}
