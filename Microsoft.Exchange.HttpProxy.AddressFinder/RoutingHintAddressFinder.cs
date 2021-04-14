using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal class RoutingHintAddressFinder : IAddressFinder
	{
		IRoutingKey[] IAddressFinder.Find(AddressFinderSource source, IAddressFinderDiagnostics diagnostics)
		{
			AddressFinderHelper.ThrowIfNull(source, diagnostics);
			List<IRoutingKey> list = new List<IRoutingKey>();
			if (!source.Headers.IsNullOrEmpty())
			{
				IRoutingKey routingKey = RoutingHintAddressFinder.FindByTargetServer(source.Headers[WellKnownHeader.XTargetServer], diagnostics);
				if (routingKey != null)
				{
					list.Add(routingKey);
				}
				else
				{
					IRoutingKey[] collection = RoutingHintAddressFinder.FindByAnchorMailbox(source.Headers[WellKnownHeader.AnchorMailbox], diagnostics);
					if (!collection.IsNullOrEmpty())
					{
						list.AddRange(collection);
					}
				}
			}
			if (list.Count == 0)
			{
				return AddressFinderHelper.EmptyRoutingKeyArray;
			}
			return list.ToArray();
		}

		private static IRoutingKey FindByTargetServer(string targetServer, IAddressFinderDiagnostics diagnostics)
		{
			if (string.IsNullOrEmpty(targetServer))
			{
				return null;
			}
			IRoutingKey routingKey = new ServerRoutingKey(targetServer);
			diagnostics.AddRoutingkey(routingKey, "TargetServerInHeader");
			return routingKey;
		}

		private static IRoutingKey[] FindByAnchorMailbox(string anchorMailbox, IAddressFinderDiagnostics diagnostics)
		{
			if (string.IsNullOrEmpty(anchorMailbox))
			{
				return AddressFinderHelper.EmptyRoutingKeyArray;
			}
			Match match = Constants.GuidAtDomainRegex.Match(anchorMailbox);
			if (match != null && match.Success)
			{
				string text = match.Result("${mailboxguid}");
				if (!string.IsNullOrEmpty(text))
				{
					Guid mailboxGuid = new Guid(text);
					string text2 = match.Result("${domain}");
					IRoutingKey routingKey = new MailboxGuidRoutingKey(mailboxGuid, text2);
					diagnostics.AddRoutingkey(routingKey, "MailboxGuidInAnchorMailbox");
					if (string.IsNullOrEmpty(text2))
					{
						return AddressFinderHelper.GetRoutingKeyArray(new IRoutingKey[]
						{
							routingKey
						});
					}
					if (SmtpAddress.IsValidSmtpAddress(anchorMailbox))
					{
						IRoutingKey routingKey2 = new SmtpRoutingKey(new SmtpAddress(anchorMailbox));
						diagnostics.AddRoutingkey(routingKey2, "FallbackSmtpAddressInAnchorMailbox");
						return AddressFinderHelper.GetRoutingKeyArray(new IRoutingKey[]
						{
							routingKey,
							routingKey2
						});
					}
					ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "[RoutingHintAddressFinder::FindByAnchorMailbox]: Malformed smtp address in X-AnchorMailbox {0}.", anchorMailbox);
				}
			}
			if (SmtpAddress.IsValidSmtpAddress(anchorMailbox))
			{
				IRoutingKey routingKey3 = new SmtpRoutingKey(new SmtpAddress(anchorMailbox));
				diagnostics.AddRoutingkey(routingKey3, "SmtpAddressInAnchorMailbox");
				return AddressFinderHelper.GetRoutingKeyArray(new IRoutingKey[]
				{
					routingKey3
				});
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "[RoutingHintAddressFinder::FindByAnchorMailbox]: Invalid smtp address in X-AnchorMailbox {0}.", anchorMailbox);
			return AddressFinderHelper.EmptyRoutingKeyArray;
		}

		private const string MailboxGuidMatchGroup = "${mailboxguid}";

		private const string DomainMatchGroup = "${domain}";
	}
}
