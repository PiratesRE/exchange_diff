using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.ControlPanel.Pickers;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class RecipientObjectResolver : AdObjectResolver, IRecipientObjectResolver
	{
		private RecipientObjectResolver()
		{
		}

		internal static IRecipientObjectResolver Instance { get; set; } = new RecipientObjectResolver();

		public IEnumerable<RecipientObjectResolverRow> ResolveObjects(IEnumerable<ADObjectId> identities)
		{
			return from row in base.ResolveObjects<RecipientObjectResolverRow>(identities, RecipientObjectResolverRow.Properties, (ADRawEntry e) => new RecipientObjectResolverRow(e))
			orderby row.DisplayName
			select row;
		}

		public IEnumerable<PeopleRecipientObject> ResolvePeople(IEnumerable<ADObjectId> identities)
		{
			return from row in base.ResolveObjects<PeopleRecipientObject>(identities, PeopleRecipientObject.Properties, (ADRawEntry e) => new PeopleRecipientObject(e))
			orderby row.DisplayName
			select row;
		}

		public IEnumerable<ADRecipient> ResolveProxyAddresses(IEnumerable<ProxyAddress> proxyAddresses)
		{
			if (proxyAddresses != null && proxyAddresses.Any<ProxyAddress>())
			{
				IRecipientSession recipientSession = (IRecipientSession)this.CreateAdSession();
				return from recipient in recipientSession.FindByProxyAddresses(proxyAddresses.ToArray<ProxyAddress>())
				select recipient.Data;
			}
			return null;
		}

		public IEnumerable<ADRecipient> ResolveLegacyDNs(IEnumerable<string> legacyDNs)
		{
			if (legacyDNs != null && legacyDNs.Any<string>())
			{
				IRecipientSession recipientSession = (IRecipientSession)this.CreateAdSession();
				return from recipient in recipientSession.FindADRecipientsByLegacyExchangeDNs(legacyDNs.ToArray<string>())
				where recipient.Data != null
				select recipient.Data;
			}
			return null;
		}

		public IEnumerable<ADRecipient> ResolveSmtpAddress(IEnumerable<string> addresses)
		{
			if (addresses != null && addresses.Any<string>())
			{
				return from recipient in this.ResolveProxyAddresses(from address in addresses
				select ProxyAddress.Parse(address))
				where recipient != null
				select recipient;
			}
			return null;
		}

		public IEnumerable<Identity> ResolveOrganizationUnitIdentity(IEnumerable<ADObjectId> identities)
		{
			return from row in identities
			select row.ToIdentity(row.ToCanonicalName());
		}

		public IEnumerable<RecipientObjectResolverRow> ResolveSmtpAddress(SmtpAddress[] smtpAddresses)
		{
			IEnumerable<ADRecipient> adRecipients = this.ResolveProxyAddresses(from address in smtpAddresses
			select ProxyAddress.Parse(address.ToString()));
			if (adRecipients != null)
			{
				foreach (ADRecipient adRecipient in adRecipients)
				{
					if (adRecipient != null)
					{
						yield return new RecipientObjectResolverRow(adRecipient);
					}
				}
			}
			yield break;
		}

		public IEnumerable<AcePermissionRecipientRow> ResolveSecurityPrincipalId(IEnumerable<SecurityPrincipalIdParameter> sidPrincipalId)
		{
			if (sidPrincipalId != null && sidPrincipalId.Any<SecurityPrincipalIdParameter>())
			{
				IRecipientSession recipientSession = (IRecipientSession)this.CreateAdSession();
				List<AcePermissionRecipientRow> list = new List<AcePermissionRecipientRow>();
				foreach (SecurityPrincipalIdParameter securityPrincipalIdParameter in sidPrincipalId)
				{
					SecurityIdentifier securityIdentifier = securityPrincipalIdParameter.SecurityIdentifier;
					if (!securityIdentifier.IsWellKnown(WellKnownSidType.SelfSid))
					{
						MiniRecipient miniRecipient = recipientSession.FindMiniRecipientBySid<MiniRecipient>(securityIdentifier, AcePermissionRecipientRow.Properties.AsEnumerable<PropertyDefinition>());
						if (miniRecipient != null)
						{
							Identity identity;
							if (miniRecipient.MasterAccountSid == securityIdentifier)
							{
								identity = new Identity(miniRecipient.Guid.ToString(), securityPrincipalIdParameter.ToString());
							}
							else
							{
								identity = new Identity(miniRecipient.Guid.ToString(), string.IsNullOrEmpty(miniRecipient.DisplayName) ? miniRecipient.Name : miniRecipient.DisplayName);
							}
							list.Add(new AcePermissionRecipientRow(identity));
						}
					}
				}
				return list;
			}
			return new List<AcePermissionRecipientRow>();
		}

		public List<SecurityIdentifier> ConvertGuidsToSid(string[] rawGuids)
		{
			List<Guid> list = new List<Guid>();
			foreach (string input in rawGuids)
			{
				Guid guid;
				if (!Guid.TryParse(input, out guid))
				{
					throw new FaultException(string.Format("Guid {0} is invalid", guid));
				}
				list.Add(guid);
			}
			if (list != null && list.Any<Guid>())
			{
				IRecipientSession session = (IRecipientSession)this.CreateAdSession();
				List<SecurityIdentifier> list2 = new List<SecurityIdentifier>();
				foreach (Guid guid2 in list)
				{
					SecurityPrincipalIdParameter securityPrincipalIdParameter = new SecurityPrincipalIdParameter(guid2.ToString());
					IEnumerable<ADRecipient> objects = securityPrincipalIdParameter.GetObjects<ADRecipient>(null, session);
					using (IEnumerator<ADRecipient> enumerator2 = objects.GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							ADRecipient adrecipient = enumerator2.Current;
							list2.Add(adrecipient.MasterAccountSid ?? ((IADSecurityPrincipal)adrecipient).Sid);
							if (enumerator2.MoveNext())
							{
								throw new Exception(Strings.ErrorUserNotUnique(guid2.ToString()));
							}
						}
					}
				}
				return list2;
			}
			return new List<SecurityIdentifier>();
		}

		internal override IDirectorySession CreateAdSession()
		{
			IDirectorySession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, base.TenantSessionSetting, 499, "CreateAdSession", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\Pickers\\RecipientObjectResolver.cs");
			tenantOrRootOrgRecipientSession.SessionSettings.IncludeInactiveMailbox = true;
			return tenantOrRootOrgRecipientSession;
		}
	}
}
