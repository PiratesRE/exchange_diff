using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class UserTokenStaticHelper
	{
		internal static ADRawEntry GetADRawEntry(UserToken token)
		{
			return UserTokenStaticHelper.GetADRawEntry(token.PartitionId, token.Organization, token.UserSid);
		}

		internal static ADRawEntry GetADRawEntry(PartitionId partitionId, OrganizationId organizationId, SecurityIdentifier sid)
		{
			ADRawEntry adrawEntry = null;
			if (UserTokenStaticHelper.adRawEntryCache.TryGetValue(sid, out adrawEntry))
			{
				return adrawEntry;
			}
			IRecipientSession recipientSession;
			if (partitionId != null)
			{
				recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 92, "GetADRawEntry", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\Core\\Common\\UserTokenStaticHelper.cs");
			}
			else if (organizationId != null && !OrganizationId.ForestWideOrgId.Equals(organizationId))
			{
				recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 99, "GetADRawEntry", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\Core\\Common\\UserTokenStaticHelper.cs");
			}
			else
			{
				recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 106, "GetADRawEntry", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\Core\\Common\\UserTokenStaticHelper.cs");
			}
			recipientSession.UseGlobalCatalog = true;
			adrawEntry = recipientSession.FindADRawEntryBySid(sid, UserTokenStaticHelper.properties);
			UserTokenStaticHelper.adRawEntryCache.InsertAbsolute(sid, adrawEntry, UserTokenStaticHelper.cacheTimeout, null);
			return adrawEntry;
		}

		private static PropertyDefinition[] userProperties = new PropertyDefinition[]
		{
			ADObjectSchema.RawName,
			ADObjectSchema.Name,
			ADObjectSchema.OrganizationId,
			ADRecipientSchema.DisplayName,
			ADObjectSchema.Id,
			ADObjectSchema.ExchangeVersion,
			IADSecurityPrincipalSchema.Sid,
			ADRecipientSchema.MasterAccountSid,
			ADRecipientSchema.ProtocolSettings,
			ADRecipientSchema.RemotePowerShellEnabled,
			ADUserSchema.UserPrincipalName,
			ADRecipientSchema.WindowsLiveID,
			ADObjectSchema.OrganizationalUnitRoot
		};

		private static PropertyDefinition[] properties = UserTokenStaticHelper.userProperties.Union(ObjectSchema.GetInstance<ClientAccessRulesRecipientFilterSchema>().AllProperties).ToArray<PropertyDefinition>();

		private static TimeoutCache<SecurityIdentifier, ADRawEntry> adRawEntryCache = new TimeoutCache<SecurityIdentifier, ADRawEntry>(20, 1000, false);

		private static TimeSpan cacheTimeout = TimeSpan.FromMinutes(5.0);
	}
}
