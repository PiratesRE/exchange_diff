using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class NewAddressBookBase : NewMultitenancySystemConfigurationObjectTask<AddressBookBase>
	{
		protected virtual int MaxAddressLists
		{
			get
			{
				if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.MaxAddressBookPolicies.Enabled)
				{
					return int.MaxValue;
				}
				int? maxAddressBookPolicies = this.ConfigurationSession.GetOrgContainer().MaxAddressBookPolicies;
				if (maxAddressBookPolicies == null)
				{
					return 250;
				}
				return maxAddressBookPolicies.GetValueOrDefault();
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "CustomFilter")]
		public string RecipientFilter
		{
			get
			{
				return (string)base.Fields["RecipientFilter"];
			}
			set
			{
				base.Fields["RecipientFilter"] = (value ?? string.Empty);
				MonadFilter monadFilter = new MonadFilter(value ?? string.Empty, this, ObjectSchema.GetInstance<ADRecipientProperties>());
				this.DataObject.SetRecipientFilter(monadFilter.InnerFilter);
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "PrecannedFilter")]
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

		[Parameter]
		public OrganizationalUnitIdParameter RecipientContainer
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields["RecipientContainer"];
			}
			set
			{
				base.Fields["RecipientContainer"] = value;
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

		protected abstract ADObjectId GetContainerId();

		internal static ADObjectId GetRecipientContainer(OrganizationalUnitIdParameter recipientContainer, IConfigurationSession cfgSession, OrganizationId organizationId, NewAddressBookBase.GetUniqueObject getDataObject, Task.ErrorLoggerDelegate writeError, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			bool useConfigNC = cfgSession.UseConfigNC;
			bool useGlobalCatalog = cfgSession.UseGlobalCatalog;
			cfgSession.UseConfigNC = false;
			cfgSession.UseGlobalCatalog = true;
			ExchangeOrganizationalUnit exchangeOrganizationalUnit;
			try
			{
				exchangeOrganizationalUnit = (ExchangeOrganizationalUnit)getDataObject(recipientContainer, cfgSession, organizationId.OrganizationalUnit, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(recipientContainer.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(recipientContainer.ToString())), ExchangeErrorCategory.Client);
				RecipientTaskHelper.IsOrgnizationalUnitInOrganization(cfgSession, organizationId, exchangeOrganizationalUnit, writeVerbose, writeError);
			}
			finally
			{
				cfgSession.UseConfigNC = useConfigNC;
				cfgSession.UseGlobalCatalog = useGlobalCatalog;
			}
			return exchangeOrganizationalUnit.Id;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (AddressBookBase)base.PrepareDataObject();
			if (!base.HasErrors)
			{
				if (!this.DataObject.IsModified(AddressBookBaseSchema.DisplayName))
				{
					this.DataObject.DisplayName = base.Name;
				}
				ADObjectId containerId = this.GetContainerId();
				if (!base.HasErrors)
				{
					this.DataObject.SetId(containerId.GetChildId(base.Name));
				}
			}
			OrganizationalUnitIdParameter organizationalUnitIdParameter = null;
			if (base.Fields.IsModified("RecipientContainer"))
			{
				if (this.RecipientContainer == null)
				{
					this.DataObject.RecipientContainer = null;
				}
				else
				{
					organizationalUnitIdParameter = this.RecipientContainer;
				}
			}
			else if (this.DataObject.RecipientContainer != null)
			{
				organizationalUnitIdParameter = new OrganizationalUnitIdParameter(this.DataObject.RecipientContainer);
			}
			if (organizationalUnitIdParameter != null)
			{
				if (base.GlobalConfigSession.IsInPreE14InteropMode())
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorCannotSetRecipientContainer), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				}
				this.DataObject.RecipientContainer = NewAddressBookBase.GetRecipientContainer(organizationalUnitIdParameter, (IConfigurationSession)base.DataSession, base.OrganizationId ?? OrganizationId.ForestWideOrgId, new NewAddressBookBase.GetUniqueObject(base.GetDataObject<ExchangeOrganizationalUnit>), new Task.ErrorLoggerDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.CheckLimit();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.DataObject.IsModified(AddressBookBaseSchema.LdapRecipientFilter))
			{
				RecipientFilterHelper.StampE2003FilterMetadata(this.DataObject, this.DataObject.LdapRecipientFilter, AddressBookBaseSchema.PurportedSearchUI);
			}
			TaskLogger.LogExit();
		}

		protected void CheckLimit()
		{
			int maxAddressLists = this.MaxAddressLists;
			if (maxAddressLists < 2147483647)
			{
				IEnumerable<AddressBookBase> allAddressLists = AddressBookBase.GetAllAddressLists(this.GetContainerId(), null, this.ConfigurationSession, null);
				int num = 0;
				foreach (AddressBookBase addressBookBase in allAddressLists)
				{
					if (!addressBookBase.IsTopContainer)
					{
						num++;
						if (num >= maxAddressLists)
						{
							base.WriteError(new ManagementObjectAlreadyExistsException(Strings.ErrorTooManyItems(maxAddressLists)), ErrorCategory.LimitsExceeded, base.Name);
							break;
						}
					}
				}
			}
		}

		internal delegate IConfigurable GetUniqueObject(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError, ExchangeErrorCategory errorCategory);
	}
}
