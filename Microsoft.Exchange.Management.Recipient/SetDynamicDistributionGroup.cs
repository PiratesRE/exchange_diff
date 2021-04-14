using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "DynamicDistributionGroup", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetDynamicDistributionGroup : SetMailEnabledRecipientObjectTask<DynamicGroupIdParameter, DynamicDistributionGroup, ADDynamicGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetDynamicDistributionGroup(this.Identity.ToString());
			}
		}

		[Parameter]
		public string RecipientFilter
		{
			get
			{
				return (string)base.Fields[DynamicDistributionGroupSchema.RecipientFilter];
			}
			set
			{
				base.Fields[DynamicDistributionGroupSchema.RecipientFilter] = (value ?? string.Empty);
				this.innerFilter = this.ConvertToQueryFilter(value);
			}
		}

		[Parameter]
		public OrganizationalUnitIdParameter RecipientContainer
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields[DynamicDistributionGroupSchema.RecipientContainer];
			}
			set
			{
				base.Fields[DynamicDistributionGroupSchema.RecipientContainer] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ExpansionServer
		{
			get
			{
				return (string)base.Fields["ExpansionServer"];
			}
			set
			{
				base.Fields["ExpansionServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public GeneralRecipientIdParameter ManagedBy
		{
			get
			{
				return (GeneralRecipientIdParameter)base.Fields["ManagedBy"];
			}
			set
			{
				base.Fields["ManagedBy"] = value;
			}
		}

		[Parameter]
		public SwitchParameter ForceUpgrade
		{
			get
			{
				return (SwitchParameter)(base.Fields["ForceUpgrade"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ForceUpgrade"] = value;
			}
		}

		protected override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			return base.ShouldUpgradeExchangeVersion(adObject) || base.Fields.IsModified(DynamicDistributionGroupSchema.RecipientFilter) || RecipientFilterHelper.IsRecipientFilterPropertiesModified(adObject, false);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.Fields.IsModified(DynamicDistributionGroupSchema.RecipientFilter))
			{
				DynamicDistributionGroup adObject = (DynamicDistributionGroup)this.GetDynamicParameters();
				if (RecipientFilterHelper.IsRecipientFilterPropertiesModified(adObject, false))
				{
					base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorBothCustomAndPrecannedFilterSpecified, null), ExchangeErrorCategory.Client, null);
				}
			}
			if (base.Fields.IsModified("ExpansionServer"))
			{
				if (string.IsNullOrEmpty(this.ExpansionServer))
				{
					this.ExpansionServer = string.Empty;
					this.homeMTA = null;
				}
				else
				{
					Server server = SetDynamicDistributionGroup.ResolveExpansionServer(this.ExpansionServer, base.GlobalConfigSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<Server>), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
					base.ValidateExpansionServer(server, true);
					this.ExpansionServer = server.ExchangeLegacyDN;
					this.homeMTA = server.ResponsibleMTA;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			if (this.ManagedBy != null)
			{
				this.managedBy = (ADRecipient)base.GetDataObject<ADRecipient>(this.ManagedBy, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.ManagedBy.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.ManagedBy.ToString())), ExchangeErrorCategory.Client);
			}
			if (this.RecipientContainer != null)
			{
				this.recipientContainerId = this.ValidateRecipientContainer(this.RecipientContainer);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			DistributionGroupTaskHelper.CheckModerationInMixedEnvironment(this.DataObject, new Task.TaskWarningLoggingDelegate(this.WriteWarning), Strings.WarningLegacyExchangeServer);
			if (base.Fields.IsModified(DynamicDistributionGroupSchema.RecipientFilter) && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.SupportOptimizedFilterOnlyInDDG.Enabled)
			{
				QueryFilter oldFilter = this.ConvertToQueryFilter(this.originalFilter);
				LocalizedString? localizedString;
				if (!DynamicDistributionGroupFilterValidation.IsFullOptimizedOrImproved(oldFilter, this.innerFilter, out localizedString))
				{
					base.WriteError(new RecipientTaskException(localizedString.Value, null), ExchangeErrorCategory.Client, this.DataObject.Identity);
				}
			}
			TaskLogger.LogExit();
		}

		internal static Server ResolveExpansionServer(string expansionServer, ITopologyConfigurationSession scSession, DataAccessHelper.CategorizedGetDataObjectDelegate getUniqueDataObjectDelegate, Task.ErrorLoggerDelegate errorHandler)
		{
			if (string.IsNullOrEmpty(expansionServer))
			{
				throw new ArgumentNullException("expansionServer");
			}
			if (scSession == null)
			{
				throw new ArgumentNullException("scSession");
			}
			if (getUniqueDataObjectDelegate == null)
			{
				throw new ArgumentNullException("getUniqueDataObjectDelegate");
			}
			if (errorHandler == null)
			{
				throw new ArgumentNullException("errorHandler");
			}
			ServerIdParameter id = null;
			try
			{
				id = ServerIdParameter.Parse(expansionServer);
			}
			catch (ArgumentException)
			{
				errorHandler(new TaskArgumentException(Strings.ErrorInvalidExpansionServer(expansionServer)), ExchangeErrorCategory.Client, null);
			}
			return (Server)getUniqueDataObjectDelegate(id, scSession, null, null, new LocalizedString?(Strings.ErrorServerNotFound(expansionServer)), new LocalizedString?(Strings.ErrorServerNotUnique(expansionServer)), ExchangeErrorCategory.Client);
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADDynamicGroup addynamicGroup = (ADDynamicGroup)base.PrepareDataObject();
			this.originalFilter = addynamicGroup.RecipientFilter;
			if (base.Fields.IsModified(DynamicDistributionGroupSchema.RecipientContainer))
			{
				addynamicGroup.RecipientContainer = this.recipientContainerId;
			}
			else if (addynamicGroup.IsChanged(DynamicDistributionGroupSchema.RecipientContainer) && addynamicGroup.RecipientContainer != null)
			{
				addynamicGroup.RecipientContainer = this.ValidateRecipientContainer(new OrganizationalUnitIdParameter(addynamicGroup.RecipientContainer));
			}
			if (base.Fields.IsModified("ManagedBy"))
			{
				addynamicGroup.ManagedBy = ((this.managedBy == null) ? null : this.managedBy.Id);
			}
			if (addynamicGroup.ManagedBy != null)
			{
				if (this.managedBy == null)
				{
					this.managedBy = (ADRecipient)base.GetDataObject<ADRecipient>(new GeneralRecipientIdParameter(addynamicGroup.ManagedBy), base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(addynamicGroup.ManagedBy.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(addynamicGroup.ManagedBy.ToString())), ExchangeErrorCategory.Client);
				}
				if (!addynamicGroup.OrganizationId.Equals(this.managedBy.OrganizationId))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorManagedByCrossTenant(this.managedBy.Identity.ToString())), ExchangeErrorCategory.Client, addynamicGroup.Identity);
				}
			}
			if (base.Fields.IsModified("ExpansionServer"))
			{
				addynamicGroup.ExpansionServer = this.ExpansionServer;
				addynamicGroup.HomeMTA = this.homeMTA;
			}
			else if (addynamicGroup.IsChanged(DistributionGroupBaseSchema.ExpansionServer))
			{
				if (!string.IsNullOrEmpty(addynamicGroup.ExpansionServer))
				{
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ExchangeLegacyDN, addynamicGroup.ExpansionServer);
					base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.GlobalConfigSession, typeof(Server), filter, null, true));
					Server[] array = null;
					try
					{
						array = base.GlobalConfigSession.Find<Server>(null, QueryScope.SubTree, filter, null, 2);
					}
					finally
					{
						base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.GlobalConfigSession));
					}
					switch (array.Length)
					{
					case 0:
						base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorServerNotFound(addynamicGroup.ExpansionServer)), ExchangeErrorCategory.Client, this.Identity);
						return null;
					case 1:
						base.ValidateExpansionServer(array[0], false);
						addynamicGroup.ExpansionServer = array[0].ExchangeLegacyDN;
						break;
					case 2:
						base.WriteError(new ManagementObjectAmbiguousException(Strings.ErrorServerNotUnique(addynamicGroup.ExpansionServer)), ExchangeErrorCategory.Client, this.Identity);
						return null;
					}
					addynamicGroup.HomeMTA = array[0].ResponsibleMTA;
				}
				else
				{
					addynamicGroup.HomeMTA = null;
				}
			}
			if (base.Fields.IsModified(DynamicDistributionGroupSchema.RecipientFilter))
			{
				addynamicGroup.SetRecipientFilter(this.innerFilter);
			}
			addynamicGroup.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.DynamicDistributionGroup);
			TaskLogger.LogExit();
			return addynamicGroup;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (RecipientFilterHelper.FixExchange12RecipientFilterMetadata(this.DataObject, ADObjectSchema.ExchangeVersion, ADDynamicGroupSchema.PurportedSearchUI, ADDynamicGroupSchema.RecipientFilterMetadata, string.Empty))
			{
				base.WriteVerbose(Strings.WarningFixTheInvalidRecipientFilterMetadata(this.Identity.ToString()));
			}
			if (!base.IsUpgrading || this.ForceUpgrade || base.ShouldContinue(Strings.ContinueUpgradeObjectVersion(this.DataObject.Name)))
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private ADObjectId ValidateRecipientContainer(OrganizationalUnitIdParameter recipientContainer)
		{
			bool useConfigNC = this.ConfigurationSession.UseConfigNC;
			bool useGlobalCatalog = this.ConfigurationSession.UseGlobalCatalog;
			ADObjectId id;
			try
			{
				this.ConfigurationSession.UseConfigNC = false;
				if (string.IsNullOrEmpty(this.ConfigurationSession.DomainController))
				{
					this.ConfigurationSession.UseGlobalCatalog = true;
				}
				ADConfigurationObject adconfigurationObject = (ADConfigurationObject)base.GetDataObject<ExchangeOrganizationalUnit>(recipientContainer, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorRecipientContainerInvalidOrNotExists), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(recipientContainer.ToString())), ExchangeErrorCategory.Client);
				id = adconfigurationObject.Id;
			}
			finally
			{
				this.ConfigurationSession.UseConfigNC = useConfigNC;
				this.ConfigurationSession.UseGlobalCatalog = useGlobalCatalog;
			}
			return id;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return DynamicDistributionGroup.FromDataObject((ADDynamicGroup)dataObject);
		}

		private QueryFilter ConvertToQueryFilter(string filter)
		{
			MonadFilter monadFilter = new MonadFilter(filter ?? string.Empty, this, ObjectSchema.GetInstance<ADRecipientProperties>());
			return monadFilter.InnerFilter;
		}

		private ADObjectId homeMTA;

		private ADObjectId recipientContainerId;

		private QueryFilter innerFilter;

		private string originalFilter;

		private ADRecipient managedBy;
	}
}
