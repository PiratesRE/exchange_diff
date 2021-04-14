using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class PFTreeManagement
	{
		public PFTreeManagement(IPFTreeTask taskInstance)
		{
			this.taskInstance = taskInstance;
			if (this.taskInstance.Organization == null && this.taskInstance.ExecutingUserOrganizationId != OrganizationId.ForestWideOrgId)
			{
				this.taskInstance.Organization = new OrganizationIdParameter(this.taskInstance.ExecutingUserOrganizationId.ToString());
			}
		}

		public PublicFolderTree PFTree
		{
			get
			{
				if (this.pfTree == null)
				{
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, PublicFolderTreeSchema.PublicFolderTreeType, PublicFolderTreeType.Mapi);
					this.taskInstance.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(this.TenantSession, typeof(PublicFolderTree), filter, null, true));
					PublicFolderTree[] array = this.TenantSession.Find<PublicFolderTree>(null, QueryScope.SubTree, filter, null, 1);
					if (array.Length > 0)
					{
						this.pfTree = array[0];
					}
				}
				return this.pfTree;
			}
		}

		public ADOrganizationalUnit OrganizationalUnit
		{
			get
			{
				if (this.tenantOrganization == null)
				{
					if (this.taskInstance.Organization != null)
					{
						ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.taskInstance.RootOrgContainerId, this.taskInstance.CurrentOrganizationId, this.taskInstance.ExecutingUserOrganizationId, true);
						IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.taskInstance.DomainController, false, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 119, "OrganizationalUnit", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\database\\PFTreeManagement.cs");
						tenantOrTopologyConfigurationSession.UseConfigNC = false;
						this.tenantOrganization = this.taskInstance.GetDataObject<ADOrganizationalUnit>(this.taskInstance.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.taskInstance.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.taskInstance.Organization.ToString())));
					}
					else
					{
						this.tenantOrganization = null;
					}
				}
				return this.tenantOrganization;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				if (this.tenantOrgId == null)
				{
					if (this.taskInstance.Organization != null)
					{
						this.tenantOrgId = this.OrganizationalUnit.OrganizationId;
					}
					else
					{
						this.tenantOrgId = this.taskInstance.ResolveCurrentOrganization();
					}
				}
				return this.tenantOrgId;
			}
		}

		public bool IsLastPublicFolderDatabase
		{
			get
			{
				return this.PFTree.PublicDatabases.Count == 1;
			}
		}

		public void CreatePublicFolderTree()
		{
			this.pfTree = new PublicFolderTree();
			try
			{
				QueryFilter filter;
				ADObjectId adobjectId;
				if (Datacenter.GetExchangeSku() != Datacenter.ExchangeSku.ExchangeDatacenter)
				{
					filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, PublicFolderTreeContainer.DefaultName);
					PublicFolderTreeContainer[] array = this.taskInstance.GlobalConfigSession.Find<PublicFolderTreeContainer>(null, QueryScope.SubTree, filter, null, 1);
					PublicFolderTreeContainer publicFolderTreeContainer;
					if (array == null || array.Length == 0)
					{
						filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, AdministrativeGroup.DefaultName);
						AdministrativeGroup[] array2 = this.taskInstance.GlobalConfigSession.Find<AdministrativeGroup>(null, QueryScope.SubTree, filter, null, 1);
						if (array2 == null || array2.Length < 1)
						{
							throw new AdminGroupNotFoundException(AdministrativeGroup.DefaultName);
						}
						publicFolderTreeContainer = new PublicFolderTreeContainer();
						publicFolderTreeContainer.SetId(array2[0].Id.GetChildId(PublicFolderTreeContainer.DefaultName));
						this.taskInstance.DataSession.Save(publicFolderTreeContainer);
					}
					else
					{
						publicFolderTreeContainer = array[0];
					}
					adobjectId = publicFolderTreeContainer.Id;
				}
				else
				{
					adobjectId = this.OrganizationId.ConfigurationUnit;
					this.pfTree.OrganizationId = this.OrganizationId;
				}
				this.pfTree.SetId(adobjectId.GetChildId("Public Folders"));
				this.taskInstance.WriteVerbose(Strings.VerboseCreatePublicFolderTree(this.pfTree.Id.ToString()));
				this.pfTree.PublicFolderTreeType = PublicFolderTreeType.Mapi;
				filter = new ComparisonFilter(ComparisonOperator.Equal, ExtendedRightSchema.DisplayName, "Create public folder");
				this.taskInstance.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(this.taskInstance.GlobalConfigSession, typeof(ExtendedRight), filter, this.taskInstance.GlobalConfigSession.ConfigurationNamingContext, true));
				ExtendedRight[] array3 = this.taskInstance.GlobalConfigSession.Find<ExtendedRight>(this.taskInstance.GlobalConfigSession.ConfigurationNamingContext, QueryScope.SubTree, filter, null, 1);
				if (0 < array3.Length)
				{
					ObjectAce objectAce = new ObjectAce(AceFlags.None, AceQualifier.AccessAllowed, 256, new SecurityIdentifier("AU"), ObjectAceFlags.ObjectAceTypePresent, array3[0].RightsGuid, Guid.Empty, false, null);
					DiscretionaryAcl discretionaryAcl = new DiscretionaryAcl(false, true, 11);
					discretionaryAcl.AddAccess(AccessControlType.Allow, objectAce.SecurityIdentifier, objectAce.AccessMask, objectAce.InheritanceFlags, objectAce.PropagationFlags, objectAce.ObjectAceFlags, objectAce.ObjectAceType, objectAce.InheritedObjectAceType);
					using (WindowsIdentity current = WindowsIdentity.GetCurrent())
					{
						SecurityIdentifier user = current.User;
						CommonSecurityDescriptor commonSecurityDescriptor = new CommonSecurityDescriptor(false, true, ControlFlags.DiscretionaryAclPresent, user, user, null, discretionaryAcl);
						byte[] binaryForm = new byte[commonSecurityDescriptor.BinaryLength];
						commonSecurityDescriptor.GetBinaryForm(binaryForm, 0);
						this.pfTree.SetPublicFolderDefaultAdminAcl(new RawSecurityDescriptor(binaryForm, 0));
					}
				}
				this.taskInstance.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(this.TenantSession));
				this.taskInstance.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(this.pfTree, this.TenantSession, typeof(PublicFolderTree)));
				this.TenantSession.Save(this.pfTree);
				if (Datacenter.GetExchangeSku() == Datacenter.ExchangeSku.ExchangeDatacenter)
				{
					this.SetOrganizationManagementACLs(this.pfTree);
				}
			}
			finally
			{
				this.taskInstance.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(this.TenantSession));
			}
		}

		public bool DoesPublicFolderDatabaseBelongToCurrentTenant(PublicFolderDatabase database)
		{
			return database.PublicFolderHierarchy.DistinguishedName.Equals(((ADObjectId)this.PFTree.Identity).DistinguishedName, StringComparison.OrdinalIgnoreCase);
		}

		public void RemovePublicFolderTree()
		{
			this.taskInstance.WriteVerbose(Strings.VerboseDeletePFTree(this.PFTree.Id.ToString()));
			this.TenantSession.Delete(this.PFTree);
		}

		private IConfigurationSession TenantSession
		{
			get
			{
				if (this.tenantSession == null)
				{
					if (this.taskInstance.Organization != null)
					{
						ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.taskInstance.RootOrgContainerId, this.OrganizationId, null, false);
						this.tenantSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.taskInstance.DomainController, false, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 367, "TenantSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\database\\PFTreeManagement.cs");
						this.tenantSession.UseConfigNC = true;
					}
					else
					{
						this.tenantSession = (IConfigurationSession)this.taskInstance.DataSession;
					}
				}
				return this.tenantSession;
			}
		}

		internal void SetOrganizationManagementACLs(ADObject obj)
		{
			ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.OrganizationId.ConfigurationUnit, this.OrganizationId, this.taskInstance.ExecutingUserOrganizationId, false);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 403, "SetOrganizationManagementACLs", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\database\\PFTreeManagement.cs");
			ADObjectId childId = this.OrganizationId.OrganizationalUnit.GetChildId("Organization Management");
			ADGroup adgroup = (ADGroup)tenantOrRootOrgRecipientSession.Read(childId);
			SecurityIdentifier sid = adgroup.Sid;
			List<ActiveDirectoryAccessRule> list = new List<ActiveDirectoryAccessRule>();
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.MailEnablePublicFolderGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.CreatePublicFolderExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.CreateTopLevelPublicFolderExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderACLExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderAdminACLExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderDeletedItemRetentionExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderExpiryExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.ModifyPublicFolderQuotasExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreAdminExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreCreateNamedPropertiesExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			list.Add(new ActiveDirectoryAccessRule(sid, ActiveDirectoryRights.ExtendedRight, AccessControlType.Allow, WellKnownGuid.StoreVisibleExtendedRightGuid, ActiveDirectorySecurityInheritance.All));
			DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(this.taskInstance.WriteVerbose), null, obj, list.ToArray());
		}

		private IPFTreeTask taskInstance;

		private IConfigurationSession tenantSession;

		private OrganizationId tenantOrgId;

		private PublicFolderTree pfTree;

		private ADOrganizationalUnit tenantOrganization;
	}
}
