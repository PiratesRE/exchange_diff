using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal class LogonUserAddressFinder : IAddressFinder
	{
		IRoutingKey[] IAddressFinder.Find(AddressFinderSource source, IAddressFinderDiagnostics diagnostics)
		{
			AddressFinderHelper.ThrowIfNull(source, diagnostics);
			string text = source.Items["WLID-MemberName"] as string;
			if (string.IsNullOrEmpty(text))
			{
				return AddressFinderHelper.EmptyRoutingKeyArray;
			}
			if (!SmtpAddress.IsValidSmtpAddress(text))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[LogonUserAddressFinder::Find]: Malformed memberName {0}.", text);
				return AddressFinderHelper.EmptyRoutingKeyArray;
			}
			string text2 = source.Items["WLID-OrganizationContext"] as string;
			if (!string.IsNullOrEmpty(text2) && !SmtpAddress.IsValidDomain(text2))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[LogonUserAddressFinder::Find]: Malformed organizationContext {0}.", text2);
				text2 = null;
			}
			IRoutingKey routingKey = new LiveIdMemberNameRoutingKey(new SmtpAddress(text), text2);
			diagnostics.AddRoutingkey(routingKey, "LogonUser");
			return AddressFinderHelper.GetRoutingKeyArray(new IRoutingKey[]
			{
				routingKey
			});
		}
	}
}
