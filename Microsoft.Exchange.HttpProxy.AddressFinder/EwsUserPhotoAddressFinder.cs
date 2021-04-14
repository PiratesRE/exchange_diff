using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal class EwsUserPhotoAddressFinder : IAddressFinder
	{
		IRoutingKey[] IAddressFinder.Find(AddressFinderSource source, IAddressFinderDiagnostics diagnostics)
		{
			AddressFinderHelper.ThrowIfNull(source, diagnostics);
			string text = source.QueryString["email"];
			if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text))
			{
				IRoutingKey routingKey = new SmtpRoutingKey(new SmtpAddress(text));
				diagnostics.AddRoutingkey(routingKey, "ExplicitLogon-SMTP");
				return AddressFinderHelper.GetRoutingKeyArray(new IRoutingKey[]
				{
					routingKey
				});
			}
			return AddressFinderHelper.EmptyRoutingKeyArray;
		}
	}
}
