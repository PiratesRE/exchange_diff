using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.RbacTasks;
using Microsoft.Exchange.Management.Tasks.ManagementScopeExtensions;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "ManagementScope", SupportsShouldProcess = true, DefaultParameterSetName = "RecipientFilter")]
	public sealed class SetManagementScope : SetSystemConfigurationObjectTask<ManagementScopeIdParameter, ManagementScope>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetManagementScope(this.DataObject.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override ManagementScopeIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RecipientFilter")]
		public string RecipientRestrictionFilter
		{
			get
			{
				return (string)base.Fields["RecipientRestrictionFilter"];
			}
			set
			{
				base.Fields["RecipientRestrictionFilter"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RecipientFilter")]
		public OrganizationalUnitIdParameter RecipientRoot
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields["RecipientRoot"];
			}
			set
			{
				base.Fields["RecipientRoot"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ServerFilter")]
		public string ServerRestrictionFilter
		{
			get
			{
				return (string)base.Fields["ServerRestrictionFilter"];
			}
			set
			{
				base.Fields["ServerRestrictionFilter"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DatabaseFilter")]
		public string DatabaseRestrictionFilter
		{
			get
			{
				return (string)base.Fields["DatabaseRestrictionFilter"];
			}
			set
			{
				base.Fields["DatabaseRestrictionFilter"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "PartnerFilter")]
		public string PartnerDelegatedTenantRestrictionFilter
		{
			get
			{
				return (string)base.Fields["PartnerDelegatedTenantRestrictionFilter"];
			}
			set
			{
				base.Fields["PartnerDelegatedTenantRestrictionFilter"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			if (base.ExchangeRunspaceConfig != null && this.DataObject.ObjectState != ObjectState.Unchanged)
			{
				base.WriteVerbose(Strings.VerboseLoadingAssignmentsByScope(this.DataObject.Id.ToString()));
				ExchangeRoleAssignment[] array = configurationSession.FindAssignmentsForManagementScope(this.DataObject, true);
				if (array.Length != 0)
				{
					ManagementScope managementScope = (ManagementScope)this.DataObject.GetOriginalObject();
					ManagementScope managementScope2 = (ManagementScope)this.DataObject.Clone();
					managementScope2.SetId(new ADObjectId("CN=TemporaryNewScope" + Guid.NewGuid()));
					managementScope2.ResetChangeTracking();
					Dictionary<ADObjectId, ManagementScope> dictionary = new Dictionary<ADObjectId, ManagementScope>(base.ExchangeRunspaceConfig.ScopesCache);
					if (!dictionary.ContainsKey(managementScope.Id))
					{
						dictionary.Add(managementScope.Id, managementScope);
					}
					dictionary.Add(managementScope2.Id, managementScope2);
					RoleHelper.LoadScopesByAssignmentsToNewCache(dictionary, array, configurationSession);
					foreach (ExchangeRoleAssignment exchangeRoleAssignment in array)
					{
						if (ADObjectId.Equals(exchangeRoleAssignment.CustomRecipientWriteScope, this.DataObject.Id))
						{
							exchangeRoleAssignment.CustomRecipientWriteScope = managementScope2.Id;
						}
						if (ADObjectId.Equals(exchangeRoleAssignment.CustomConfigWriteScope, this.DataObject.Id))
						{
							exchangeRoleAssignment.CustomConfigWriteScope = managementScope2.Id;
						}
						base.WriteVerbose(Strings.VerboseSetScopeValidateNewScopedAssignment(this.DataObject.Id.ToString(), exchangeRoleAssignment.Id.ToString()));
						if (!RoleHelper.HasDelegatingHierarchicalRoleAssignment(base.ExecutingUserOrganizationId, base.ExchangeRunspaceConfig.RoleAssignments, dictionary, exchangeRoleAssignment, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)))
						{
							base.WriteError(new InvalidOperationException(Strings.ErrorSetScopeAddPermission(this.DataObject.Id.ToString(), exchangeRoleAssignment.Id.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Id);
						}
					}
					List<ExchangeRoleAssignment> list = new List<ExchangeRoleAssignment>(base.ExchangeRunspaceConfig.RoleAssignments);
					List<ExchangeRoleAssignment> list2 = new List<ExchangeRoleAssignment>(array.Length);
					List<ADObjectId> list3 = new List<ADObjectId>(list.Count);
					foreach (ExchangeRoleAssignment exchangeRoleAssignment2 in array)
					{
						for (int k = list.Count - 1; k >= 0; k--)
						{
							if (ADObjectId.Equals(list[k].Id, exchangeRoleAssignment2.Id))
							{
								list.RemoveAt(k);
								list.Add(exchangeRoleAssignment2);
								list3.Add(exchangeRoleAssignment2.Id);
								break;
							}
						}
						list2.Add((ExchangeRoleAssignment)exchangeRoleAssignment2.GetOriginalObject());
					}
					foreach (ExchangeRoleAssignment exchangeRoleAssignment3 in list2)
					{
						base.WriteVerbose(Strings.VerboseSetScopeValidateRemoveOriginalScopedAssignment(this.DataObject.Id.ToString(), exchangeRoleAssignment3.Id.ToString()));
						if (!RoleHelper.HasDelegatingHierarchicalRoleAssignment(base.ExecutingUserOrganizationId, list, dictionary, exchangeRoleAssignment3, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose)))
						{
							if (list3.Contains(exchangeRoleAssignment3.Id))
							{
								base.WriteError(new InvalidOperationException(Strings.ErrorSetScopeToBlockSelf(this.DataObject.Id.ToString(), exchangeRoleAssignment3.Id.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Id);
							}
							else
							{
								base.WriteError(new InvalidOperationException(Strings.ErrorSetScopeNeedHierarchicalRoleAssignment(this.DataObject.Id.ToString(), exchangeRoleAssignment3.Id.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Id);
							}
						}
					}
				}
			}
			ManagementScope[] array4 = configurationSession.FindSimilarManagementScope(this.DataObject);
			if (array4.Length > 0)
			{
				base.WriteError(new ArgumentException(Strings.SimilarScopeFound(array4[0].Name)), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (ManagementScope)base.PrepareDataObject();
			if (base.Fields.IsModified("RecipientRoot"))
			{
				if (this.RecipientRoot == null)
				{
					this.DataObject.RecipientRoot = null;
				}
				else
				{
					this.DataObject.RecipientRoot = this.GetADObjectIdFromOrganizationalUnitIdParameter((IConfigurationSession)base.DataSession, this.RecipientRoot);
				}
			}
			if (base.Fields.IsModified("RecipientRestrictionFilter"))
			{
				this.ValidateAndSetFilterOnDataObject("RecipientRestrictionFilter", this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.Fields.IsModified("ServerRestrictionFilter"))
			{
				this.ValidateAndSetFilterOnDataObject("ServerRestrictionFilter", this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.Fields.IsModified("PartnerDelegatedTenantRestrictionFilter"))
			{
				this.ValidateAndSetFilterOnDataObject("PartnerDelegatedTenantRestrictionFilter", this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.Fields.IsModified("DatabaseRestrictionFilter"))
			{
				this.ValidateAndSetFilterOnDataObject("DatabaseRestrictionFilter", this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}

		private ADObjectId GetADObjectIdFromOrganizationalUnitIdParameter(IConfigurationSession configurationSession, OrganizationalUnitIdParameter root)
		{
			OrganizationId organizationId = OrganizationId.ForestWideOrgId;
			if (configurationSession is ITenantConfigurationSession)
			{
				organizationId = TaskHelper.ResolveOrganizationId(this.DataObject.Id, ManagementScope.RdnScopesContainerToOrganization, (ITenantConfigurationSession)configurationSession);
			}
			bool useConfigNC = configurationSession.UseConfigNC;
			bool useGlobalCatalog = configurationSession.UseGlobalCatalog;
			ADObjectId id;
			try
			{
				configurationSession.UseConfigNC = false;
				configurationSession.UseGlobalCatalog = true;
				IConfigurable configurable = (ADConfigurationObject)base.GetDataObject<ExchangeOrganizationalUnit>(root, configurationSession, (null == organizationId) ? null : organizationId.OrganizationalUnit, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(root.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(root.ToString())));
				id = ((ADObject)configurable).Id;
			}
			finally
			{
				configurationSession.UseConfigNC = useConfigNC;
				configurationSession.UseGlobalCatalog = useGlobalCatalog;
			}
			return id;
		}

		protected override IConfigurable ResolveDataObject()
		{
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			return base.ResolveDataObject();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.Force && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
