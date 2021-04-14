using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class PartnerApplicationRunspaceConfiguration : WebServiceRunspaceConfiguration
	{
		public ADUser LinkedAccountUser { get; private set; }

		public static PartnerApplicationRunspaceConfiguration Create(PartnerApplication partnerApplication)
		{
			if (partnerApplication == null)
			{
				throw new ArgumentNullException("partnerApplication");
			}
			if (partnerApplication.LinkedAccount == null || partnerApplication.LinkedAccount.IsDeleted)
			{
				throw new CmdletAccessDeniedException(Strings.ErrorPartnerApplicationWithoutLinkedAccount(partnerApplication.Id.ToString()));
			}
			ADUser aduser = LinkedAccountCache.Instance.Get(partnerApplication.LinkedAccount);
			if (aduser == null)
			{
				throw new CmdletAccessDeniedException(Strings.ErrorManagementObjectNotFound(partnerApplication.LinkedAccount.ToString()));
			}
			return new PartnerApplicationRunspaceConfiguration(PartnerApplicationRunspaceConfiguration.LinkedAccountIdentity.Create(aduser));
		}

		private PartnerApplicationRunspaceConfiguration(PartnerApplicationRunspaceConfiguration.LinkedAccountIdentity identity) : base(identity)
		{
		}

		public override bool IsTargetObjectInRoleScope(RoleType roleType, ADRecipient targetRecipient)
		{
			if (targetRecipient == null)
			{
				throw new ArgumentNullException("targetRecipient");
			}
			if (!base.HasRoleOfType(roleType))
			{
				ExTraceGlobals.AccessDeniedTracer.TraceWarning<string, RoleType>((long)this.GetHashCode(), "IsTargetObjectInRoleScope() returns false because identity {0} doesn't have role {1}", this.identityName, roleType);
				return false;
			}
			return (this.LinkedAccountUser.OrganizationId == OrganizationId.ForestWideOrgId && targetRecipient.OrganizationId != OrganizationId.ForestWideOrgId) || base.IsTargetObjectInRoleScope(roleType, targetRecipient);
		}

		public static bool IsWebMethodInOfficeExtensionRole(string webMethodName)
		{
			return PartnerApplicationRunspaceConfiguration.IsWebMethodInDefaultRoleType(webMethodName, RoleType.OfficeExtensionApplication, ref PartnerApplicationRunspaceConfiguration.officeExtensionEntries);
		}

		private static bool IsWebMethodInDefaultRoleType(string webMethodName, RoleType roleType, ref List<string> entriesCache)
		{
			if (string.IsNullOrEmpty(webMethodName))
			{
				throw new ArgumentNullException("webMethodName");
			}
			if (entriesCache == null)
			{
				lock (PartnerApplicationRunspaceConfiguration.staticLock)
				{
					if (entriesCache == null)
					{
						ExchangeRole rootRoleForRoleType = PartnerApplicationRunspaceConfiguration.GetRootRoleForRoleType(roleType);
						if (rootRoleForRoleType != null)
						{
							entriesCache = (from x in rootRoleForRoleType.RoleEntries
							select x.Name).ToList<string>();
							entriesCache.Sort();
						}
						else
						{
							entriesCache = new List<string>();
						}
					}
				}
			}
			return entriesCache.BinarySearch(webMethodName, StringComparer.OrdinalIgnoreCase) >= 0;
		}

		private static ExchangeRole GetRootRoleForRoleType(RoleType roleType)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 185, "GetRootRoleForRoleType", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\PartnerApplicationRunspaceConfiguration.cs");
			ExchangeRole[] array = tenantOrTopologyConfigurationSession.Find<ExchangeRole>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ExchangeRoleSchema.RoleType, roleType), null, ADGenericPagedReader<ExchangeRole>.DefaultPageSize);
			ExchangeRole[] array2 = Array.FindAll<ExchangeRole>(array, (ExchangeRole r) => r.IsRootRole);
			if (array2.Length == 0)
			{
				return null;
			}
			if (array2.Length == 1)
			{
				return array2[0];
			}
			throw new DataSourceOperationException(Strings.ErrorFoundMultipleRootRole(roleType.ToString()));
		}

		protected override ADRawEntry LoadExecutingUser(IIdentity identity, IList<PropertyDefinition> properties)
		{
			this.LinkedAccountUser = ((PartnerApplicationRunspaceConfiguration.LinkedAccountIdentity)identity).LinkedAccountUser;
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(this.LinkedAccountUser.Id), 224, "LoadExecutingUser", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\PartnerApplicationRunspaceConfiguration.cs");
			return this.LinkedAccountUser;
		}

		private static object staticLock = new object();

		private static List<string> officeExtensionEntries;

		private class LinkedAccountIdentity : GenericSidIdentity
		{
			public ADUser LinkedAccountUser { get; private set; }

			public static PartnerApplicationRunspaceConfiguration.LinkedAccountIdentity Create(ADUser linkedAccountUser)
			{
				if (linkedAccountUser == null)
				{
					throw new ArgumentNullException("linkedAccountUser");
				}
				string partitionId = null;
				if (!linkedAccountUser.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					partitionId = linkedAccountUser.OrganizationId.PartitionId.ToString();
				}
				return new PartnerApplicationRunspaceConfiguration.LinkedAccountIdentity(linkedAccountUser.Name, "PartnerApplicationLinkedAccount", linkedAccountUser.Sid, partitionId)
				{
					LinkedAccountUser = linkedAccountUser
				};
			}

			private LinkedAccountIdentity(string name, string type, SecurityIdentifier sid, string partitionId) : base(name, type, sid, partitionId)
			{
			}

			private const string AccountType = "PartnerApplicationLinkedAccount";
		}
	}
}
