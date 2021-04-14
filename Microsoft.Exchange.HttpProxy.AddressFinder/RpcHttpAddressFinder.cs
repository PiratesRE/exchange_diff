using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal class RpcHttpAddressFinder : IAddressFinder
	{
		IRoutingKey[] IAddressFinder.Find(AddressFinderSource source, IAddressFinderDiagnostics diagnostics)
		{
			AddressFinderHelper.ThrowIfNull(source, diagnostics);
			if (string.IsNullOrEmpty(source.Url.Query))
			{
				return AddressFinderHelper.EmptyRoutingKeyArray;
			}
			RpcHttpQueryString rpcHttpQueryString = new RpcHttpQueryString(source.Url.Query);
			if (!SmtpAddress.IsValidSmtpAddress(rpcHttpQueryString.RcaServer))
			{
				return AddressFinderHelper.EmptyRoutingKeyArray;
			}
			Guid guid = Guid.Empty;
			string text = string.Empty;
			try
			{
				SmtpAddress smtpAddress = new SmtpAddress(rpcHttpQueryString.RcaServer);
				guid = new Guid(smtpAddress.Local);
				text = smtpAddress.Domain;
			}
			catch (FormatException arg)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string, FormatException>(0L, "[RpcHttpAddressFinder::Find]: Caught exception: Reason {0}; Exception {1}.", string.Format("Invalid mailboxGuid {0}", guid), arg);
				return AddressFinderHelper.EmptyRoutingKeyArray;
			}
			string text2 = "MailboxGuidWithDomain";
			if (!text.Contains(".") && !source.Items.IsNullOrEmpty())
			{
				string text3 = source.Items["WLID-MemberName"] as string;
				if (!string.IsNullOrEmpty(text3))
				{
					SmtpAddress smtpAddress2 = new SmtpAddress(text3);
					string domain = smtpAddress2.Domain;
					if (domain != null && !string.Equals(domain, text, StringComparison.OrdinalIgnoreCase))
					{
						ExTraceGlobals.BriefTracer.TraceError<string, string>((long)this.GetHashCode(), "[RpcHttpAddressFinder::Find]: Fixing up invalid domain name from client: {0} to {1}", text, domain);
						text = domain;
						text2 += "-ChangedToUserDomain";
					}
				}
			}
			IRoutingKey routingKey = new MailboxGuidRoutingKey(guid, text);
			diagnostics.AddRoutingkey(routingKey, text2);
			return AddressFinderHelper.GetRoutingKeyArray(new IRoutingKey[]
			{
				routingKey
			});
		}
	}
}
