using System;
using System.Web;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal class EwsAddressFinder : IAddressFinder
	{
		IRoutingKey[] IAddressFinder.Find(AddressFinderSource source, IAddressFinderDiagnostics diagnostics)
		{
			AddressFinderHelper.ThrowIfNull(source, diagnostics);
			string text = source.Headers["X-PreferServerAffinity"];
			if (!string.IsNullOrEmpty(text) && text.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
			{
				HttpCookie httpCookie = source.Cookies["X-BackEndOverrideCookie"];
				string text2 = (httpCookie == null) ? null : httpCookie.Value;
				if (!string.IsNullOrEmpty(text2))
				{
					string text3;
					string s;
					Utilities.GetTwoSubstrings(text2, '~', out text3, out s);
					int value;
					if (!string.IsNullOrWhiteSpace(text3) && int.TryParse(s, out value))
					{
						IRoutingKey routingKey = new ServerRoutingKey(text3, new int?(value));
						diagnostics.AddRoutingkey(routingKey, "X-BackEndOverrideCookie");
						return AddressFinderHelper.GetRoutingKeyArray(new IRoutingKey[]
						{
							routingKey
						});
					}
					diagnostics.AddErrorInfo("Unable to parse TargetServer:" + text2);
				}
			}
			return AddressFinderHelper.EmptyRoutingKeyArray;
		}
	}
}
