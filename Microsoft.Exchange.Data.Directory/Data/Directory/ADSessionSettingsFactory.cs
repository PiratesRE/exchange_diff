using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADSessionSettingsFactory : ADSessionSettings.SessionSettingsFactory
	{
		internal static void RunWithInactiveMailboxVisibilityEnablerForDatacenter(Action action)
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				using (new ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler())
				{
					action();
					return;
				}
			}
			action();
		}

		internal override ADSessionSettings FromAllTenantsOrRootOrgAutoDetect(ADObjectId id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id.DomainId == null)
			{
				ExTraceGlobals.GetConnectionTracer.TraceDebug<string>(0L, "FromAllTenantsOrRootOrgAutoDetect(): Value '{0}' passed to id parameter doesn't have DomainId initialized, falling back to RootOrg scope set", id.ToString());
				return ADSessionSettings.FromRootOrgScopeSet();
			}
			PartitionId partitionId = id.GetPartitionId();
			if (!ADAccountPartitionLocator.IsKnownPartition(partitionId))
			{
				ExTraceGlobals.GetConnectionTracer.TraceDebug<string>(0L, "FromAllTenantsOrRootOrgAutoDetect(): Value '{0}' passed to id parameter doesn't match any known partition, falling back to RootOrg scope set", id.ToString());
				return ADSessionSettings.FromRootOrgScopeSet();
			}
			ExTraceGlobals.GetConnectionTracer.TraceDebug<string, string>(0L, "FromAllTenantsOrRootOrgAutoDetect(): Value '{0}' passed to id parameter matches partition {1}, returning settings bound to that partition", id.ToString(), partitionId.ToString());
			if (ADSession.IsTenantIdentity(id, partitionId.ForestFQDN))
			{
				return ADSessionSettings.FromAllTenantsObjectId(id);
			}
			if (!TopologyProvider.IsAdamTopology())
			{
				return ADSessionSettings.FromAccountPartitionRootOrgScopeSet(id.GetPartitionId());
			}
			return ADSessionSettings.FromRootOrgScopeSet();
		}

		internal override ADSessionSettings FromAllTenantsOrRootOrgAutoDetect(OrganizationId orgId)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			if (!OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				return ADSessionSettings.FromAllTenantsPartitionId(orgId.PartitionId);
			}
			return ADSessionSettings.FromRootOrgScopeSet();
		}

		internal override ADSessionSettings FromTenantCUName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			ExTraceGlobals.GetConnectionTracer.TraceDebug<string>((long)name.GetHashCode(), "FromTenantCUName(): Building session settings from CU name '{0}'", name);
			return ADSessionSettings.FromTenantAcceptedDomain(name);
		}

		internal override ADSessionSettings FromTenantAcceptedDomain(string domain)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			SmtpDomain domainName;
			if (!SmtpDomain.TryParse(domain, out domainName))
			{
				throw new CannotResolveTenantNameException(DirectoryStrings.CannotResolveTenantNameByAcceptedDomain(domain));
			}
			if (ConsumerIdentityHelper.IsConsumerDomain(domainName))
			{
				return ADSessionSettings.FromConsumerOrganization();
			}
			OrganizationId scopingOrganizationId = OrganizationId.FromAcceptedDomain(domain);
			return ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId);
		}

		internal override ADSessionSettings FromTenantMSAUser(string msaUserNetID)
		{
			OrganizationId scopingOrganizationId = OrganizationId.FromMSAUserNetID(msaUserNetID);
			return ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId);
		}

		internal override ADSessionSettings FromAllTenantsPartitionId(PartitionId partitionId)
		{
			return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ScopeSet.GetAllTenantsDefaultScopeSet(partitionId.ForestFQDN), null, OrganizationId.ForestWideOrgId, null, ConfigScopes.AllTenants, partitionId);
		}

		internal override ADSessionSettings FromTenantPartitionHint(TenantPartitionHint partitionHint)
		{
			if (partitionHint == null)
			{
				throw new ArgumentNullException("partitionHint");
			}
			ExTraceGlobals.GetConnectionTracer.TraceDebug<string>(0L, "FromTenantPartitionHint(): Building session settings from partition hint '{0}'", partitionHint.ToString());
			return partitionHint.GetTenantScopedADSessionSettingsServiceOnly();
		}

		internal override ADSessionSettings FromExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId)
		{
			if (externalDirectoryOrganizationId == TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationIdGuid)
			{
				return ADSessionSettings.FromConsumerOrganization();
			}
			OrganizationId scopingOrganizationId = OrganizationId.FromExternalDirectoryOrganizationId(externalDirectoryOrganizationId);
			return ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId);
		}

		internal override ADSessionSettings FromTenantForestAndCN(string exoAccountForest, string exoTenantContainer)
		{
			OrganizationId scopingOrganizationId = OrganizationId.FromTenantForestAndCN(exoAccountForest, exoTenantContainer);
			return ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId);
		}

		internal override ADSessionSettings FromAllTenantsObjectId(ADObjectId id)
		{
			return ADSessionSettings.FromAllTenantsPartitionId(id.GetPartitionId());
		}

		internal override bool InDomain()
		{
			if (this.inDomain == null)
			{
				if (ADSession.IsBoundToAdam)
				{
					this.inDomain = new bool?(false);
				}
				try
				{
					NativeHelpers.GetDomainName();
					this.inDomain = new bool?(true);
				}
				catch (CannotGetDomainInfoException)
				{
					this.inDomain = new bool?(false);
				}
			}
			return this.inDomain.Value;
		}

		protected override OrganizationId RehomeScopingOrganizationIdIfNeeded(OrganizationId currentOrganizationId)
		{
			if (currentOrganizationId != null && ADSessionSettings.SessionSettingsFactory.IsTenantScopedOrganization(currentOrganizationId))
			{
				try
				{
					bool flag;
					TenantRelocationState tenantRelocationState = TenantRelocationStateCache.GetTenantRelocationState(currentOrganizationId.OrganizationalUnit.Name, currentOrganizationId.PartitionId, out flag, false);
					if (flag && tenantRelocationState != null && tenantRelocationState.SourceForestState == TenantRelocationStatus.Retired && tenantRelocationState.TargetOrganizationId != null)
					{
						currentOrganizationId = tenantRelocationState.TargetOrganizationId;
					}
					else if (!flag && tenantRelocationState != null && tenantRelocationState.SourceForestState != TenantRelocationStatus.Retired && tenantRelocationState.OrganizationId != null)
					{
						currentOrganizationId = tenantRelocationState.OrganizationId;
					}
				}
				catch (CannotResolveTenantNameException)
				{
				}
			}
			return currentOrganizationId;
		}

		private bool? inDomain;

		internal abstract class ThreadSessionSettingEnabler : IDisposable
		{
			public ThreadSessionSettingEnabler()
			{
				ADSessionSettingsFactory.ThreadSessionSettingEnabler.Enable(this.PostAction);
			}

			public void Dispose()
			{
				ADSessionSettingsFactory.ThreadSessionSettingEnabler.Disable(this.PostAction);
			}

			public abstract ADSessionSettings.SessionSettingsFactory.PostActionForSettings PostAction { get; }

			public static void Enable(ADSessionSettings.SessionSettingsFactory.PostActionForSettings postAction)
			{
				ADSessionSettings.SessionSettingsFactory.ThreadPostActionForSettings = (ADSessionSettings.SessionSettingsFactory.PostActionForSettings)Delegate.Combine(ADSessionSettings.SessionSettingsFactory.ThreadPostActionForSettings, postAction);
			}

			public static void Disable(ADSessionSettings.SessionSettingsFactory.PostActionForSettings postAction)
			{
				ADSessionSettings.SessionSettingsFactory.ThreadPostActionForSettings = (ADSessionSettings.SessionSettingsFactory.PostActionForSettings)Delegate.Remove(ADSessionSettings.SessionSettingsFactory.ThreadPostActionForSettings, postAction);
			}
		}

		internal sealed class InactiveMailboxVisibilityEnabler : ADSessionSettingsFactory.ThreadSessionSettingEnabler
		{
			public override ADSessionSettings.SessionSettingsFactory.PostActionForSettings PostAction
			{
				get
				{
					return new ADSessionSettings.SessionSettingsFactory.PostActionForSettings(ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler.AddInactiveMailBoxSupport);
				}
			}

			public static void Enable()
			{
				ADSessionSettingsFactory.ThreadSessionSettingEnabler.Enable(new ADSessionSettings.SessionSettingsFactory.PostActionForSettings(ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler.AddInactiveMailBoxSupport));
			}

			public static void Disable()
			{
				ADSessionSettingsFactory.ThreadSessionSettingEnabler.Disable(new ADSessionSettings.SessionSettingsFactory.PostActionForSettings(ADSessionSettingsFactory.InactiveMailboxVisibilityEnabler.AddInactiveMailBoxSupport));
			}

			public static ADSessionSettings AddInactiveMailBoxSupport(ADSessionSettings settings)
			{
				settings.IncludeInactiveMailbox = true;
				return settings;
			}
		}

		internal sealed class SoftDeletedObjectVisibilityEnabler : ADSessionSettingsFactory.ThreadSessionSettingEnabler
		{
			public override ADSessionSettings.SessionSettingsFactory.PostActionForSettings PostAction
			{
				get
				{
					return new ADSessionSettings.SessionSettingsFactory.PostActionForSettings(ADSessionSettingsFactory.SoftDeletedObjectVisibilityEnabler.AddSoftDeletedObjectSupport);
				}
			}

			public static ADSessionSettings AddSoftDeletedObjectSupport(ADSessionSettings settings)
			{
				settings.IncludeSoftDeletedObjects = true;
				return settings;
			}
		}
	}
}
