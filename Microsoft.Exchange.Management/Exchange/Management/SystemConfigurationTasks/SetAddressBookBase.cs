using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class SetAddressBookBase<TIdParameter, TRepresentationObject> : SetSystemConfigurationObjectTask<TIdParameter, TRepresentationObject, AddressBookBase> where TIdParameter : ADIdParameter, new() where TRepresentationObject : AddressListBase, new()
	{
		[Parameter]
		public string RecipientFilter
		{
			get
			{
				return (string)base.Fields[AddressListBaseSchema.RecipientFilter];
			}
			set
			{
				base.Fields[AddressListBaseSchema.RecipientFilter] = (value ?? string.Empty);
				MonadFilter monadFilter = new MonadFilter(value ?? string.Empty, this, ObjectSchema.GetInstance<ADRecipientProperties>());
				this.innerFilter = monadFilter.InnerFilter;
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
			return base.Fields.IsModified(AddressListBaseSchema.RecipientFilter) || RecipientFilterHelper.IsRecipientFilterPropertiesModified(adObject, false);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			AddressListBase adObject = (TRepresentationObject)((object)this.GetDynamicParameters());
			if (base.Fields.IsModified(AddressListBaseSchema.RecipientFilter) && RecipientFilterHelper.IsRecipientFilterPropertiesModified(adObject, false))
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorBothCustomAndPrecannedFilterSpecified, null), ErrorCategory.InvalidArgument, null);
			}
			TaskLogger.LogExit();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			AddressBookBase addressBookBase = (AddressBookBase)dataObject;
			this.originalVersion = addressBookBase.ExchangeVersion;
			base.StampChangesOn(dataObject);
			if (base.Fields.IsModified(AddressBookBaseSchema.RecipientFilter))
			{
				addressBookBase.SetRecipientFilter(this.innerFilter);
			}
		}

		protected void ValidateBrokenRecipientFilterChange(QueryFilter expectedRecipientFilter)
		{
			if (!this.DataObject.IsModified(AddressBookBaseSchema.RecipientFilter) || this.originalVersion.IsOlderThan(AddressBookBaseSchema.RecipientFilter.VersionAdded))
			{
				if (this.DataObject.IsChanged(AddressBookBaseSchema.LdapRecipientFilter))
				{
					string b = (expectedRecipientFilter == null) ? string.Empty : LdapFilterBuilder.LdapFilterFromQueryFilter(expectedRecipientFilter);
					if (!string.Equals(this.DataObject.LdapRecipientFilter, b, StringComparison.OrdinalIgnoreCase))
					{
						string expected = (expectedRecipientFilter == null) ? string.Empty : expectedRecipientFilter.GenerateInfixString(FilterLanguage.Monad);
						if (this.DataObject.IsTopContainer)
						{
							TIdParameter identity = this.Identity;
							base.WriteError(new InvalidOperationException(Strings.ErrorInvalidFilterForAddressBook(identity.ToString(), this.DataObject.RecipientFilter, expected)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
							return;
						}
						TIdParameter identity2 = this.Identity;
						base.WriteError(new InvalidOperationException(Strings.ErrorInvalidFilterForDefaultGlobalAddressList(identity2.ToString(), this.DataObject.RecipientFilter, GlobalAddressList.RecipientFilterForDefaultGal.GenerateInfixString(FilterLanguage.Monad))), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
				}
				return;
			}
			if (this.DataObject.IsTopContainer)
			{
				TIdParameter identity3 = this.Identity;
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnContainer(identity3.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				return;
			}
			TIdParameter identity4 = this.Identity;
			base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnDefaultGAL(identity4.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
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
			else if (this.DataObject.IsModified(AddressBookBaseSchema.RecipientContainer) && this.DataObject.RecipientContainer != null)
			{
				organizationalUnitIdParameter = new OrganizationalUnitIdParameter(this.DataObject.RecipientContainer);
			}
			if (organizationalUnitIdParameter != null)
			{
				if (base.GlobalConfigSession.IsInPreE14InteropMode())
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorCannotSetRecipientContainer), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				}
				OrganizationId organizationId = this.DataObject.OrganizationId;
				this.DataObject.RecipientContainer = NewAddressBookBase.GetRecipientContainer(organizationalUnitIdParameter, this.ConfigurationSession, organizationId, new NewAddressBookBase.GetUniqueObject(base.GetDataObject<ExchangeOrganizationalUnit>), new Task.ErrorLoggerDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			if (this.IsObjectStateChanged() && this.DataObject.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				TIdParameter identity = this.Identity;
				base.WriteError(new InvalidOperationException(Strings.ErrorObjectNotManagableFromCurrentConsole(identity.ToString(), this.DataObject.ExchangeVersion.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (!base.HasErrors)
			{
				if (this.DataObject.IsTopContainer)
				{
					if (this.DataObject.IsModified(ADObjectSchema.Name) || this.DataObject.IsModified(AddressBookBaseSchema.DisplayName))
					{
						TIdParameter identity2 = this.Identity;
						base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnContainer(identity2.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
					}
					this.ValidateBrokenRecipientFilterChange(null);
				}
				if (!base.HasErrors && (this.DataObject.IsChanged(AddressBookBaseSchema.RecipientFilter) || this.DataObject.IsChanged(AddressBookBaseSchema.RecipientContainer)))
				{
					this.DataObject[AddressBookBaseSchema.RecipientFilterApplied] = false;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (RecipientFilterHelper.FixExchange12RecipientFilterMetadata(this.DataObject, ADObjectSchema.ExchangeVersion, AddressBookBaseSchema.PurportedSearchUI, AddressBookBaseSchema.RecipientFilterMetadata, this.DataObject.LdapRecipientFilter))
			{
				TIdParameter identity = this.Identity;
				base.WriteVerbose(Strings.WarningFixTheInvalidRecipientFilterMetadata(identity.ToString()));
			}
			if (!base.IsUpgrading || this.ForceUpgrade || base.ShouldContinue(Strings.ContinueUpgradeObjectVersion(this.DataObject.Name)))
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private QueryFilter innerFilter;

		private ExchangeObjectVersion originalVersion;
	}
}
