using System;
using System.Globalization;
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
	[Cmdlet("New", "DynamicDistributionGroup", SupportsShouldProcess = true, DefaultParameterSetName = "PrecannedFilter")]
	public sealed class NewDynamicDistributionGroup : NewMailEnabledRecipientObjectTask<ADDynamicGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("CustomFilter" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageNewDynamicDistributionGroupCustomFilter(base.Name.ToString(), this.RecipientFilter.ToString(), base.RecipientContainerId.ToString());
				}
				return Strings.ConfirmationMessageNewDynamicDistributionGroupPrecannedFilter(base.Name.ToString(), this.IncludedRecipients.ToString(), base.RecipientContainerId.ToString());
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CustomFilter")]
		public string RecipientFilter
		{
			get
			{
				return (string)base.Fields[DynamicDistributionGroupSchema.RecipientFilter];
			}
			set
			{
				base.Fields[DynamicDistributionGroupSchema.RecipientFilter] = (value ?? string.Empty);
				MonadFilter monadFilter = new MonadFilter(value ?? string.Empty, this, ObjectSchema.GetInstance<ADRecipientProperties>());
				this.innerFilter = monadFilter.InnerFilter;
			}
		}

		[Parameter]
		public OrganizationalUnitIdParameter RecipientContainer
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields[ADDynamicGroupSchema.RecipientContainer];
			}
			set
			{
				base.Fields[ADDynamicGroupSchema.RecipientContainer] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "PrecannedFilter")]
		public WellKnownRecipientType? IncludedRecipients
		{
			get
			{
				return this.DataObject.IncludedRecipients;
			}
			set
			{
				this.DataObject.IncludedRecipients = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalDepartment
		{
			get
			{
				return this.DataObject.ConditionalDepartment;
			}
			set
			{
				this.DataObject.ConditionalDepartment = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCompany
		{
			get
			{
				return this.DataObject.ConditionalCompany;
			}
			set
			{
				this.DataObject.ConditionalCompany = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalStateOrProvince
		{
			get
			{
				return this.DataObject.ConditionalStateOrProvince;
			}
			set
			{
				this.DataObject.ConditionalStateOrProvince = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute1
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute1;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute1 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute2
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute2;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute2 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute3
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute3;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute3 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute4
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute4;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute4 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute5
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute5;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute5 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute6
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute6;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute6 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute7
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute7;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute7 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute8
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute8;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute8 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute9
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute9;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute9 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute10
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute10;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute10 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute11
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute11;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute11 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute12
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute12;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute12 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute13
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute13;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute13 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute14
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute14;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute14 = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
		public MultiValuedProperty<string> ConditionalCustomAttribute15
		{
			get
			{
				return this.DataObject.ConditionalCustomAttribute15;
			}
			set
			{
				this.DataObject.ConditionalCustomAttribute15 = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.Fields.IsModified(DynamicDistributionGroupSchema.RecipientFilter))
			{
				LocalizedString? localizedString;
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.SupportOptimizedFilterOnlyInDDG.Enabled && DynamicDistributionGroupFilterValidation.ContainsNonOptimizedCondition(this.innerFilter, out localizedString))
				{
					base.ThrowTerminatingError(new TaskArgumentException(localizedString.Value, null), ExchangeErrorCategory.Client, null);
				}
				this.DataObject.SetRecipientFilter(this.innerFilter);
			}
			TaskLogger.LogExit();
		}

		protected override void StampDefaultValues(ADDynamicGroup dataObject)
		{
			base.StampDefaultValues(dataObject);
			dataObject.StampDefaultValues(RecipientType.DynamicDistributionGroup);
		}

		protected override void PrepareRecipientObject(ADDynamicGroup group)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(group);
			if (base.OrganizationalUnit == null && group[ADRecipientSchema.DefaultDistributionListOU] != null)
			{
				ADObjectId adobjectId = (ADObjectId)group[ADRecipientSchema.DefaultDistributionListOU];
				RecipientTaskHelper.ResolveOrganizationalUnitInOrganization(new OrganizationalUnitIdParameter(adobjectId), this.ConfigurationSession, base.CurrentOrganizationId, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ExchangeOrganizationalUnit>), ExchangeErrorCategory.ServerOperation, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				group.SetId(adobjectId.GetChildId(base.Name));
			}
			if (this.RecipientContainer == null)
			{
				if (!base.Fields.IsModified(ADDynamicGroupSchema.RecipientContainer))
				{
					group.RecipientContainer = group.Id.Parent;
				}
			}
			else
			{
				bool useConfigNC = this.ConfigurationSession.UseConfigNC;
				bool useGlobalCatalog = this.ConfigurationSession.UseGlobalCatalog;
				this.ConfigurationSession.UseConfigNC = false;
				if (string.IsNullOrEmpty(this.ConfigurationSession.DomainController))
				{
					this.ConfigurationSession.UseGlobalCatalog = true;
				}
				ExchangeOrganizationalUnit exchangeOrganizationalUnit = (ExchangeOrganizationalUnit)base.GetDataObject<ExchangeOrganizationalUnit>(this.RecipientContainer, this.ConfigurationSession, (base.CurrentOrganizationId != null) ? base.CurrentOrganizationId.OrganizationalUnit : null, null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(this.RecipientContainer.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(this.RecipientContainer.ToString())), ExchangeErrorCategory.Client);
				RecipientTaskHelper.IsOrgnizationalUnitInOrganization(this.ConfigurationSession, group.OrganizationId, exchangeOrganizationalUnit, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				this.ConfigurationSession.UseConfigNC = useConfigNC;
				this.ConfigurationSession.UseGlobalCatalog = useGlobalCatalog;
				group.RecipientContainer = (ADObjectId)exchangeOrganizationalUnit.Identity;
			}
			group.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.DynamicDistributionGroup);
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (string.IsNullOrEmpty(this.DataObject.LegacyExchangeDN))
			{
				AdministrativeGroup administrativeGroup = base.GlobalConfigSession.GetAdministrativeGroup();
				string parentLegacyDN = string.Format(CultureInfo.InvariantCulture, "{0}/cn=Recipients", new object[]
				{
					administrativeGroup.LegacyExchangeDN
				});
				this.DataObject.LegacyExchangeDN = LegacyDN.GenerateLegacyDN(parentLegacyDN, this.DataObject, true, new LegacyDN.LegacyDNIsUnique(this.LegacyDNIsUnique));
			}
			if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.EmailAddressPolicy.Enabled)
			{
				this.DataObject.EmailAddressPolicyEnabled = false;
			}
			DistributionGroupTaskHelper.CheckModerationInMixedEnvironment(this.DataObject, new Task.TaskWarningLoggingDelegate(this.WriteWarning), Strings.WarningLegacyExchangeServer);
			TaskLogger.LogExit();
		}

		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			DynamicDistributionGroup result2 = new DynamicDistributionGroup((ADDynamicGroup)result);
			base.WriteResult(result2);
			TaskLogger.LogExit();
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(DynamicDistributionGroup).FullName;
			}
		}

		private bool LegacyDNIsUnique(string legacyDN)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, legacyDN),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, this.DataObject.Id)
			});
			IRecipientSession recipientSession = RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(base.DomainController, this.DataObject.Id);
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(recipientSession, typeof(ADRecipient), filter, null, true));
			ADRecipient[] array = null;
			try
			{
				array = recipientSession.Find(null, QueryScope.SubTree, filter, null, 1);
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(recipientSession));
			}
			return 0 == array.Length;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return DynamicDistributionGroup.FromDataObject((ADDynamicGroup)dataObject);
		}

		private QueryFilter innerFilter;
	}
}
