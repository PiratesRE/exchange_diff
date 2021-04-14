using System;
using System.Security.Principal;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AdminAuditExternalAccessDeterminer
	{
		public static bool IsExternalAccess(string userId, OrganizationId userOrganization, OrganizationId currentOrganization)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return true;
			}
			if (AuditFeatureManager.IsExternalAccessCheckOnDedicatedEnabled())
			{
				NTAccount ntaccount = new NTAccount(AdminAuditExternalAccessDeterminer.TransformUserID(userId));
				SecurityIdentifier securityIdentifier;
				try
				{
					securityIdentifier = (SecurityIdentifier)ntaccount.Translate(typeof(SecurityIdentifier));
				}
				catch (IdentityNotMappedException)
				{
					ntaccount = new NTAccount(userId);
					try
					{
						securityIdentifier = (SecurityIdentifier)ntaccount.Translate(typeof(SecurityIdentifier));
					}
					catch (IdentityNotMappedException)
					{
						return true;
					}
				}
				bool flag;
				return AdminAuditExternalAccessDeterminer.externalAccessLRUCache.Get(securityIdentifier.ToString(), out flag);
			}
			bool flag2 = userOrganization == null || currentOrganization == null;
			return !flag2 && !userOrganization.Equals(currentOrganization);
		}

		private static bool UserExistsInAD(string sid)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 103, "UserExistsInAD", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Auditing\\AdminAuditExternalAccessDeterminer.cs");
			PropertyDefinition[] properties = new PropertyDefinition[]
			{
				ADMailboxRecipientSchema.Sid
			};
			ADRawEntry adrawEntry = tenantOrRootOrgRecipientSession.FindUserBySid(new SecurityIdentifier(sid), properties);
			return adrawEntry != null;
		}

		private static string TransformUserID(string userId)
		{
			int num = userId.IndexOf('/');
			if (num > 0)
			{
				return userId.Substring(0, num) + "\\" + userId.Substring(userId.LastIndexOf('/') + 1);
			}
			return userId;
		}

		private const int ExternalAccessCacheCapacity = 10000;

		private static LRUCache<string, bool> externalAccessLRUCache = new LRUCache<string, bool>(10000, (string key) => !AdminAuditExternalAccessDeterminer.UserExistsInAD(key));
	}
}
