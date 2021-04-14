using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal class MapiAddressFinder : IAddressFinder
	{
		IRoutingKey[] IAddressFinder.Find(AddressFinderSource source, IAddressFinderDiagnostics diagnostics)
		{
			AddressFinderHelper.ThrowIfNull(source, diagnostics);
			List<IRoutingKey> list = new List<IRoutingKey>();
			IRoutingKey routingKey = MapiAddressFinder.FindByMailboxId(source.QueryString["mailboxId"], diagnostics);
			if (routingKey != null)
			{
				list.Add(routingKey);
			}
			routingKey = MapiAddressFinder.FindBySmtpAddress(source.QueryString["smtpAddress"], diagnostics);
			if (routingKey != null)
			{
				list.Add(routingKey);
			}
			if (list.Count == 0)
			{
				return AddressFinderHelper.EmptyRoutingKeyArray;
			}
			return list.ToArray();
		}

		private static IRoutingKey FindBySmtpAddress(string smtpAddress, IAddressFinderDiagnostics diagnostics)
		{
			if (string.IsNullOrEmpty(smtpAddress))
			{
				return null;
			}
			if (!SmtpAddress.IsValidSmtpAddress(smtpAddress))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "[MapiAddressFinder::FindBySmtpAddress]: Malformed smtpAddress {0}.", smtpAddress);
				return null;
			}
			IRoutingKey routingKey = new SmtpRoutingKey(new SmtpAddress(smtpAddress));
			diagnostics.AddRoutingkey(routingKey, "SmtpAddressInQueryString");
			return routingKey;
		}

		private static IRoutingKey FindByMailboxId(string mailboxId, IAddressFinderDiagnostics diagnostics)
		{
			if (string.IsNullOrEmpty(mailboxId))
			{
				return null;
			}
			if (!SmtpAddress.IsValidSmtpAddress(mailboxId))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "[MapiAddressFinder::FindByMailboxId]: Malformed mailboxId {0}.", mailboxId);
				return null;
			}
			Guid guid = Guid.Empty;
			string tenantDomain = string.Empty;
			try
			{
				SmtpAddress smtpAddress = new SmtpAddress(mailboxId);
				guid = new Guid(smtpAddress.Local);
				tenantDomain = smtpAddress.Domain;
			}
			catch (FormatException arg)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string, FormatException>(0L, "[MapiAddressFinder::FindByMailboxId]: Caught exception: Reason {0}; Exception {1}.", string.Format("Invalid mailboxGuid {0}", guid), arg);
				return null;
			}
			IRoutingKey routingKey = new MailboxGuidRoutingKey(guid, tenantDomain);
			diagnostics.AddRoutingkey(routingKey, "MailboxGuidInQueryString");
			return routingKey;
		}

		private const string UseMailboxOfAuthenticatedUserParameter = "useMailboxOfAuthenticatedUser";

		private const string MailboxIdParameter = "mailboxId";

		private const string SmtpAddressParameter = "smtpAddress";
	}
}
