using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel.Pickers
{
	public interface IRecipientObjectResolver
	{
		IEnumerable<ADRecipient> ResolveLegacyDNs(IEnumerable<string> legacyDNs);

		IEnumerable<RecipientObjectResolverRow> ResolveObjects(IEnumerable<ADObjectId> identities);

		IEnumerable<Identity> ResolveOrganizationUnitIdentity(IEnumerable<ADObjectId> identities);

		IEnumerable<PeopleRecipientObject> ResolvePeople(IEnumerable<ADObjectId> identities);

		IEnumerable<ADRecipient> ResolveProxyAddresses(IEnumerable<ProxyAddress> proxyAddresses);

		IEnumerable<AcePermissionRecipientRow> ResolveSecurityPrincipalId(IEnumerable<SecurityPrincipalIdParameter> sidPrincipalId);

		IEnumerable<RecipientObjectResolverRow> ResolveSmtpAddress(SmtpAddress[] smtpAddresses);

		IEnumerable<ADRecipient> ResolveSmtpAddress(IEnumerable<string> addresses);

		List<SecurityIdentifier> ConvertGuidsToSid(string[] rawGuids);
	}
}
