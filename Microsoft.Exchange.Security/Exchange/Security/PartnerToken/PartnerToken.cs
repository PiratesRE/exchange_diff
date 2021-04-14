using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;
using System.Linq;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Claim;

namespace Microsoft.Exchange.Security.PartnerToken
{
	internal class PartnerToken
	{
		private PartnerToken(ReadOnlyCollection<ClaimSet> claimSets)
		{
			List<string> list = new List<string>(2);
			foreach (ClaimSet claimSet in claimSets)
			{
				foreach (Claim claim in claimSet)
				{
					string item;
					if (claim.Match(ClaimTypes.NameIdentifier, Rights.Identity))
					{
						SamlNameIdentifierClaimResource samlNameIdentifierClaimResource;
						if (claim.HaveProperResource(out samlNameIdentifierClaimResource))
						{
							this.upn = samlNameIdentifierClaimResource.Name;
						}
					}
					else if (claim.Match("http://schemas.microsoft.com/exchange/services/2006/partnertoken/targettenant", Rights.PossessProperty))
					{
						claim.HaveProperResource(out this.targetTenant);
					}
					else if (claim.Match("http://schemas.microsoft.com/exchange/services/2006/partnertoken/linkedpartnerorganizationid", Rights.PossessProperty))
					{
						claim.HaveProperResource(out this.linkedPartnerOrganizationId);
					}
					else if (claim.Match("http://schemas.microsoft.com/exchange/services/2006/partnertoken/linkedpartnergroupid", Rights.PossessProperty) && claim.HaveProperResource(out item))
					{
						list.Add(item);
					}
				}
			}
			list.Sort();
			this.linkedPartnerGroupIds = list.ToArray();
		}

		public static bool IsPartnerToken(ReadOnlyCollection<ClaimSet> claimSets)
		{
			return claimSets.PossessClaimType("http://schemas.microsoft.com/exchange/services/2006/partnertoken/linkedpartnerorganizationid");
		}

		public static bool TryGetDelegatedPrincipalAndOrganizationId(ReadOnlyCollection<ClaimSet> claimSets, out DelegatedPrincipal delegatedPrincipal, out OrganizationId delegatedOrganizationId, out string errorMessage)
		{
			delegatedPrincipal = null;
			delegatedOrganizationId = null;
			errorMessage = null;
			PartnerToken partnerToken = new PartnerToken(claimSets);
			if (!partnerToken.HasCompleteClaims())
			{
				ExTraceGlobals.PartnerTokenTracer.TraceError<PartnerToken>(0L, "[PartnerToken.TryGetDelegatedPrincipalAndOrganizationId] Did not find all necessary claims: {0}", partnerToken);
				errorMessage = string.Format("Did not find all necessary claims: {0}", partnerToken);
				return false;
			}
			if (!PartnerToken.PartnerTokenValidationResultCache.Singleton.TryValidate(partnerToken, out delegatedOrganizationId))
			{
				ExTraceGlobals.PartnerTokenTracer.TraceError<PartnerToken>(0L, "[PartnerToken.TryGetDelegatedPrincipalAndOrganizationId] no group found for partner token {0}", partnerToken);
				errorMessage = string.Format("no linked group found for the partner token: {0}", partnerToken);
				return false;
			}
			ExTraceGlobals.PartnerTokenTracer.TraceDebug<string, string>(0L, "[PartnerToken.TryGetDelegatedPrincipalAndOrganizationId] the given target domain: {0}, the official tenant name: {1}", partnerToken.targetTenant, delegatedOrganizationId.OrganizationalUnit.Name);
			delegatedPrincipal = new DelegatedPrincipal(DelegatedPrincipal.GetDelegatedIdentity(partnerToken.upn, partnerToken.linkedPartnerOrganizationId, delegatedOrganizationId.OrganizationalUnit.Name, partnerToken.upn, partnerToken.linkedPartnerGroupIds), null);
			return true;
		}

		private bool HasCompleteClaims()
		{
			if (!string.IsNullOrEmpty(this.upn) && !string.IsNullOrEmpty(this.targetTenant) && !string.IsNullOrEmpty(this.linkedPartnerOrganizationId) && this.linkedPartnerGroupIds != null && this.linkedPartnerGroupIds.Length > 0)
			{
				return this.linkedPartnerGroupIds.All((string groupId) => !string.IsNullOrEmpty(groupId));
			}
			return false;
		}

		internal static bool FindLinkedRoleGroupInDelegatedOrganization(OrganizationId delegateOrganizationId, string linkedPartnerOrganizationId, string[] linkedPartnerGroupIds)
		{
			ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), delegateOrganizationId, delegateOrganizationId, false), 187, "FindLinkedRoleGroupInDelegatedOrganization", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\PartnerToken\\PartnerToken.cs");
			LinkedPartnerGroupInformation[] array = (from groupId in linkedPartnerGroupIds
			select new LinkedPartnerGroupInformation
			{
				LinkedPartnerGroupId = groupId,
				LinkedPartnerOrganizationId = linkedPartnerOrganizationId
			}).ToArray<LinkedPartnerGroupInformation>();
			int num = 0;
			foreach (Result<ADRawEntry> result in tenantRecipientSession.ReadMultipleByLinkedPartnerId(array, PartnerInfo.groupProperties))
			{
				if (result.Data != null)
				{
					ExTraceGlobals.PartnerTokenTracer.TraceDebug<ADObjectId, LinkedPartnerGroupInformation>(0L, "[PartnerToken::FindLinkedRoleGroupInDelegatedOrganization] found the group '{0}' with linked group info '{1}'", result.Data.Id, array[num++]);
					return true;
				}
				ExTraceGlobals.PartnerTokenTracer.TraceDebug<LinkedPartnerGroupInformation, ProviderError>(0L, "[PartnerToken::FindLinkedRoleGroupInDelegatedOrganization] failed to find a group with linked group info '{0}', error was '{1}'", array[num++], result.Error);
			}
			return false;
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Format("UPN: {0}; TargetTenant: {1}; LinkedPartnerOrganizationId: {2}; LinkedPartnerGroupIds: {3}", new object[]
				{
					this.upn,
					this.targetTenant,
					this.linkedPartnerOrganizationId,
					string.Join(",", this.linkedPartnerGroupIds)
				});
			}
			return this.toString;
		}

		public override bool Equals(object obj)
		{
			PartnerToken partnerToken = obj as PartnerToken;
			if (partnerToken == null)
			{
				return false;
			}
			if (!string.Equals(this.upn, partnerToken.upn, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (!string.Equals(this.targetTenant, partnerToken.targetTenant, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (!string.Equals(this.linkedPartnerOrganizationId, partnerToken.linkedPartnerOrganizationId, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (this.linkedPartnerGroupIds.Length != partnerToken.linkedPartnerGroupIds.Length)
			{
				return false;
			}
			for (int i = 0; i < this.linkedPartnerGroupIds.Length; i++)
			{
				if (!string.Equals(this.linkedPartnerGroupIds[i], partnerToken.linkedPartnerGroupIds[i], StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		private readonly string upn;

		private string targetTenant;

		private string linkedPartnerOrganizationId;

		private string[] linkedPartnerGroupIds;

		private string toString;

		internal class PartnerTokenValidationResultCache : LazyLookupTimeoutCache<PartnerToken, PartnerToken.PartnerTokenValidationResultCache.ValidationResult>
		{
			private PartnerTokenValidationResultCache() : base(1, PartnerToken.PartnerTokenValidationResultCache.cacheSize.Value, false, PartnerToken.PartnerTokenValidationResultCache.cacheTimeToLive.Value)
			{
			}

			public static PartnerToken.PartnerTokenValidationResultCache Singleton
			{
				get
				{
					if (PartnerToken.PartnerTokenValidationResultCache.singleton == null)
					{
						lock (PartnerToken.PartnerTokenValidationResultCache.lockObj)
						{
							if (PartnerToken.PartnerTokenValidationResultCache.singleton == null)
							{
								PartnerToken.PartnerTokenValidationResultCache.singleton = new PartnerToken.PartnerTokenValidationResultCache();
							}
						}
					}
					return PartnerToken.PartnerTokenValidationResultCache.singleton;
				}
			}

			protected override PartnerToken.PartnerTokenValidationResultCache.ValidationResult CreateOnCacheMiss(PartnerToken key, ref bool shouldAdd)
			{
				OrganizationId organizationId = null;
				bool flag = false;
				for (int i = 0; i < PartnerToken.PartnerTokenValidationResultCache.lookupRetryMax.Value; i++)
				{
					try
					{
						organizationId = DomainToOrganizationIdCache.Singleton.Get(new SmtpDomain(key.targetTenant));
						if (organizationId == null)
						{
							ExTraceGlobals.PartnerTokenTracer.TraceDebug<string>(0L, "[PartnerTokenCache::CreateOnCacheMiss] failed to get the organization id for tenant domain name {0}", key.targetTenant);
							break;
						}
						flag = PartnerToken.FindLinkedRoleGroupInDelegatedOrganization(organizationId, key.linkedPartnerOrganizationId, key.linkedPartnerGroupIds);
						ExTraceGlobals.PartnerTokenTracer.TraceDebug<bool, OrganizationId, PartnerToken>(0L, "[PartnerTokenCache::CreateOnCacheMiss] will return {0}, org '{1}' for partner token '{2}'", flag, organizationId, key);
						break;
					}
					catch (ADTransientException arg)
					{
						ExTraceGlobals.PartnerTokenTracer.TraceWarning<PartnerToken, ADTransientException>(0L, "[PartnerTokenCache::CreateOnCacheMiss] will return false for partner token '{0}' due to the ADTransient exception: {1}", key, arg);
					}
				}
				if (organizationId == null)
				{
					return PartnerToken.PartnerTokenValidationResultCache.ValidationResult.OrganizationIdNotFound;
				}
				return new PartnerToken.PartnerTokenValidationResultCache.ValidationResult(organizationId, flag);
			}

			public bool TryValidate(PartnerToken token, out OrganizationId delegatedOrganizationId)
			{
				PartnerToken.PartnerTokenValidationResultCache.ValidationResult validationResult = base.Get(token);
				delegatedOrganizationId = validationResult.OrganizationId;
				return validationResult.RoleGroupFound;
			}

			private static TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("PartnerTokenValidationResultCacheTimeToLive", TimeSpanUnit.Seconds, TimeSpan.FromMinutes(5.0), ExTraceGlobals.PartnerTokenTracer);

			private static IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("PartnerTokenValidationResultCacheMaxItems", 1000, ExTraceGlobals.PartnerTokenTracer);

			private static IntAppSettingsEntry lookupRetryMax = new IntAppSettingsEntry("PartnerTokenValidationResultLookupRetryMax", 3, ExTraceGlobals.PartnerTokenTracer);

			private static readonly object lockObj = new object();

			private static PartnerToken.PartnerTokenValidationResultCache singleton = null;

			internal class ValidationResult
			{
				public ValidationResult(OrganizationId organizationId, bool roleGroupFound)
				{
					this.OrganizationId = organizationId;
					this.RoleGroupFound = roleGroupFound;
				}

				public OrganizationId OrganizationId { get; private set; }

				public bool RoleGroupFound { get; private set; }

				public static PartnerToken.PartnerTokenValidationResultCache.ValidationResult OrganizationIdNotFound
				{
					get
					{
						return PartnerToken.PartnerTokenValidationResultCache.ValidationResult.orgIdNotFound;
					}
				}

				private static PartnerToken.PartnerTokenValidationResultCache.ValidationResult orgIdNotFound = new PartnerToken.PartnerTokenValidationResultCache.ValidationResult(null, false);
			}
		}
	}
}
