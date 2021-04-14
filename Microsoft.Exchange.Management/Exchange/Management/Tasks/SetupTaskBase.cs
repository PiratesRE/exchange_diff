using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public class SetupTaskBase : Task
	{
		[Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false)]
		public string DomainController
		{
			get
			{
				return (string)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		public virtual OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		protected OrganizationId OrganizationId
		{
			get
			{
				if (this.organization != null)
				{
					return this.organization.OrganizationId;
				}
				return OrganizationId.ForestWideOrgId;
			}
		}

		protected ADObjectId OrgContainerId
		{
			get
			{
				if (this.organization != null)
				{
					return this.organization.ConfigurationUnit;
				}
				return this.configurationSession.GetOrgContainerId();
			}
		}

		protected void LogReadObject(ADRawEntry obj)
		{
			base.WriteVerbose(Strings.InfoReadObjectFromDC(obj.OriginatingServer, obj.Id.DistinguishedName));
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception) || exception is SecurityDescriptorAccessDeniedException;
		}

		protected void LogWriteObject(ADObject obj)
		{
			base.WriteVerbose(Strings.InfoWroteObjectToDC(obj.OriginatingServer, obj.DistinguishedName));
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.rootOrgSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 168, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
			if (ADSessionSettings.GetProcessServerSettings() != null && this.Organization == null)
			{
				this.PrepareSessionsForRootOrg();
				return;
			}
			this.PrepareSessionsForTenant();
		}

		protected T ResolveExchangeGroupGuid<T>(Guid wkg) where T : ADObject, new()
		{
			T t = default(T);
			try
			{
				t = this.rootDomainRecipientSession.ResolveWellKnownGuid<T>(wkg, this.configurationSession.ConfigurationNamingContext);
			}
			catch (ADReferralException)
			{
			}
			if (t == null)
			{
				bool useGlobalCatalog = this.recipientSession.UseGlobalCatalog;
				this.recipientSession.UseGlobalCatalog = true;
				try
				{
					t = this.recipientSession.ResolveWellKnownGuid<T>(wkg, this.configurationSession.ConfigurationNamingContext);
				}
				finally
				{
					this.recipientSession.UseGlobalCatalog = useGlobalCatalog;
				}
			}
			if (t != null)
			{
				this.LogReadObject(t);
			}
			return t;
		}

		protected T ResolveHostedExchangeGroupGuid<T>(Guid wkg, OrganizationId orgId) where T : ADObject, new()
		{
			if (null == orgId)
			{
				throw new ArgumentNullException("orgId");
			}
			if (OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				throw new ArgumentOutOfRangeException("orgId");
			}
			T t = this.orgDomainRecipientSession.ResolveWellKnownGuid<T>(wkg, orgId.ConfigurationUnit);
			if (t != null)
			{
				this.LogReadObject(t);
			}
			return t;
		}

		internal static void Save(ADRecipient o, IRecipientSession recipientSession)
		{
			if (o == null)
			{
				throw new ArgumentNullException("o");
			}
			IRecipientSession recipientSession2 = o.Session;
			if (recipientSession2 == null)
			{
				recipientSession2 = recipientSession;
			}
			recipientSession2.Save(o);
		}

		protected void ReplaceAddressListACEs(ADObjectId addressBookContainerRoot, SecurityIdentifier originalSid, SecurityIdentifier[] replacementSids)
		{
			AddressBookBase[] array = this.configurationSession.Find<AddressBookBase>(addressBookContainerRoot, QueryScope.SubTree, null, null, 0);
			foreach (AddressBookBase addressBookBase in array)
			{
				bool flag = false;
				RawSecurityDescriptor rawSecurityDescriptor = addressBookBase.ReadSecurityDescriptor();
				ActiveDirectorySecurity activeDirectorySecurity = new ActiveDirectorySecurity();
				byte[] array3 = new byte[rawSecurityDescriptor.BinaryLength];
				rawSecurityDescriptor.GetBinaryForm(array3, 0);
				activeDirectorySecurity.SetSecurityDescriptorBinaryForm(array3);
				AuthorizationRuleCollection accessRules = activeDirectorySecurity.GetAccessRules(true, false, typeof(SecurityIdentifier));
				foreach (object obj in accessRules)
				{
					ActiveDirectoryAccessRule activeDirectoryAccessRule = (ActiveDirectoryAccessRule)obj;
					if (activeDirectoryAccessRule.IdentityReference == originalSid)
					{
						flag = true;
						activeDirectorySecurity.RemoveAccessRuleSpecific(activeDirectoryAccessRule);
						foreach (SecurityIdentifier identity in replacementSids)
						{
							ActiveDirectoryAccessRule rule = new ActiveDirectoryAccessRule(identity, activeDirectoryAccessRule.ActiveDirectoryRights, activeDirectoryAccessRule.AccessControlType, activeDirectoryAccessRule.ObjectType, activeDirectoryAccessRule.InheritanceType, activeDirectoryAccessRule.InheritedObjectType);
							activeDirectorySecurity.AddAccessRule(rule);
						}
					}
				}
				if (flag && base.ShouldProcess(addressBookBase.DistinguishedName, Strings.InfoProcessAction(addressBookBase.DistinguishedName), null))
				{
					addressBookBase.SaveSecurityDescriptor(new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm(), 0));
				}
			}
		}

		protected bool GetADSplitPermissionMode(ADGroup ets, ADGroup ewp)
		{
			bool result = false;
			if (ets == null)
			{
				ets = this.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EtsWkGuid);
				if (ets == null)
				{
					base.ThrowTerminatingError(new ExTrustedSubsystemGroupNotFoundException(WellKnownGuid.EtsWkGuid), ErrorCategory.InvalidData, null);
				}
				this.LogReadObject(ets);
			}
			if (ewp == null)
			{
				ewp = this.ResolveExchangeGroupGuid<ADGroup>(WellKnownGuid.EwpWkGuid);
				if (ewp == null)
				{
					base.ThrowTerminatingError(new ExWindowsPermissionsGroupNotFoundException(WellKnownGuid.EwpWkGuid), ErrorCategory.InvalidData, null);
				}
				this.LogReadObject(ewp);
			}
			if (!ewp.Members.Contains(ets.Id))
			{
				result = true;
			}
			return result;
		}

		private void ResolveOrganization()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.rootOrgId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 395, "ResolveOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			IEnumerable<ADOrganizationalUnit> objects = this.Organization.GetObjects<ADOrganizationalUnit>(null, tenantOrTopologyConfigurationSession);
			using (IEnumerator<ADOrganizationalUnit> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					this.organization = enumerator.Current;
					if (enumerator.MoveNext())
					{
						base.ThrowTerminatingError(new ArgumentException(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())), ErrorCategory.InvalidArgument, null);
					}
				}
				else
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), ErrorCategory.InvalidArgument, null);
				}
			}
			base.CurrentOrganizationId = this.organization.OrganizationId;
		}

		private void PrepareSessionsForRootOrg()
		{
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 435, "PrepareSessionsForRootOrg", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
			this.recipientSession.UseGlobalCatalog = false;
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 443, "PrepareSessionsForRootOrg", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
			this.domainConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 448, "PrepareSessionsForRootOrg", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
			this.domainConfigurationSession.UseConfigNC = false;
			this.ReadRootDomainFromDc(OrganizationId.ForestWideOrgId);
			this.rootDomainRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.rootDomain.OriginatingServer, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAccountPartitionWideScopeSet(this.recipientSession.SessionSettings.PartitionId), 458, "PrepareSessionsForRootOrg", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
		}

		private void PrepareSessionsForTenant()
		{
			this.rootOrgId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			if (this.Organization != null)
			{
				this.ResolveOrganization();
				this.LogReadObject(this.organization);
				this.orgDomainRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.organization.OriginatingServer, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsObjectId(this.organization.Id), 479, "PrepareSessionsForTenant", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
			}
			if (this.organization != null)
			{
				this.rootOrgId = ((null != this.organization.Id.GetPartitionId() && this.organization.Id.GetPartitionId().ForestFQDN != null) ? ADSystemConfigurationSession.GetRootOrgContainerId(this.organization.Id.GetPartitionId().ForestFQDN, null, null) : ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest());
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.rootOrgId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, false);
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 503, "PrepareSessionsForTenant", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
			this.recipientSession.UseGlobalCatalog = false;
			this.configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 514, "PrepareSessionsForTenant", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
			this.domainConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, false, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 521, "PrepareSessionsForTenant", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
			this.domainConfigurationSession.UseConfigNC = false;
			this.ReadRootDomainFromDc(base.CurrentOrganizationId);
			this.rootDomainRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(this.rootDomain.OriginatingServer, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(base.CurrentOrganizationId.PartitionId), 533, "PrepareSessionsForTenant", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\SetupTaskBase.cs");
		}

		private void ReadRootDomainFromDc(OrganizationId orgId)
		{
			this.rootDomain = null;
			if (orgId.Equals(OrganizationId.ForestWideOrgId))
			{
				this.rootDomain = ADForest.GetLocalForest().FindRootDomain(true);
			}
			else
			{
				this.rootDomain = ADForest.GetForest(orgId.PartitionId).FindRootDomain(true);
			}
			if (this.rootDomain == null)
			{
				base.ThrowTerminatingError(new RootDomainNotFoundException(), ErrorCategory.InvalidData, null);
			}
			this.LogReadObject(this.rootDomain);
		}

		internal IRecipientSession recipientSession;

		internal IRecipientSession rootDomainRecipientSession;

		internal IConfigurationSession configurationSession;

		internal IConfigurationSession domainConfigurationSession;

		private IRecipientSession orgDomainRecipientSession;

		protected ADDomain rootDomain;

		protected ADOrganizationalUnit organization;

		private ADObjectId rootOrgId;

		internal ITopologyConfigurationSession rootOrgSession;
	}
}
