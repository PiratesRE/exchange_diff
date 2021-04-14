using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Set", "UMDialPlan", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class SetUMDialPlan : SetSystemConfigurationObjectTask<UMDialPlanIdParameter, UMDialPlan>
	{
		[Parameter(Mandatory = false)]
		public AddressListIdParameter ContactAddressList
		{
			get
			{
				return (AddressListIdParameter)base.Fields["ContactAddressList"];
			}
			set
			{
				base.Fields["ContactAddressList"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationalUnitIdParameter ContactRecipientContainer
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields["ContactRecipientContainer"];
			}
			set
			{
				base.Fields["ContactRecipientContainer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMAutoAttendantIdParameter UMAutoAttendant
		{
			get
			{
				return (UMAutoAttendantIdParameter)base.Fields["UMAutoAttendant"];
			}
			set
			{
				base.Fields["UMAutoAttendant"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string CountryOrRegionCode
		{
			get
			{
				return (string)base.Fields["CountryOrRegionCode"];
			}
			set
			{
				base.Fields["CountryOrRegionCode"] = value;
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetUMDialPlan(this.Identity.ToString());
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
			return true;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			UMDialPlan umdialPlan = (UMDialPlan)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (base.Fields.IsModified("ContactRecipientContainer") && base.Fields.IsModified("ContactAddressList"))
			{
				base.WriteError(new InvalidALParameterException(), ErrorCategory.NotSpecified, null);
				TaskLogger.LogExit();
				return null;
			}
			if (base.Fields.IsModified("CountryOrRegionCode"))
			{
				if (string.IsNullOrEmpty(this.CountryOrRegionCode))
				{
					base.WriteError(new InvalidParameterException(Strings.EmptyCountryOrRegionCode), ErrorCategory.InvalidArgument, null);
				}
				else
				{
					umdialPlan.CountryOrRegionCode = this.CountryOrRegionCode;
				}
			}
			if (base.Fields.IsModified("ContactRecipientContainer"))
			{
				OrganizationalUnitIdParameter contactRecipientContainer = this.ContactRecipientContainer;
				if (contactRecipientContainer != null)
				{
					bool useConfigNC = this.ConfigurationSession.UseConfigNC;
					this.ConfigurationSession.UseConfigNC = false;
					ADConfigurationObject adconfigurationObject = (ADConfigurationObject)base.GetDataObject<ExchangeOrganizationalUnit>(contactRecipientContainer, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(this.ContactRecipientContainer.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(this.ContactRecipientContainer.ToString())));
					this.ConfigurationSession.UseConfigNC = useConfigNC;
					if (!base.HasErrors)
					{
						umdialPlan.ContactAddressList = adconfigurationObject.Id;
					}
				}
				else
				{
					umdialPlan.ContactAddressList = null;
				}
			}
			if (base.Fields.IsModified("ContactAddressList"))
			{
				AddressListIdParameter contactAddressList = this.ContactAddressList;
				if (contactAddressList != null)
				{
					IEnumerable<AddressBookBase> objects = contactAddressList.GetObjects<AddressBookBase>(null, this.ConfigurationSession);
					using (IEnumerator<AddressBookBase> enumerator = objects.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							umdialPlan.ContactAddressList = (ADObjectId)enumerator.Current.Identity;
						}
						goto IL_19C;
					}
				}
				umdialPlan.ContactAddressList = null;
			}
			IL_19C:
			if (umdialPlan.ContactScope != CallSomeoneScopeEnum.AddressList)
			{
				umdialPlan.ContactAddressList = null;
			}
			if (base.Fields.IsModified("UMAutoAttendant"))
			{
				UMAutoAttendantIdParameter umautoAttendant = this.UMAutoAttendant;
				if (umautoAttendant != null)
				{
					UMAutoAttendant umautoAttendant2 = (UMAutoAttendant)base.GetDataObject<UMAutoAttendant>(umautoAttendant, this.ConfigurationSession, null, new LocalizedString?(Strings.NonExistantAutoAttendant(umautoAttendant.ToString())), new LocalizedString?(Strings.MultipleAutoAttendantsWithSameId(umautoAttendant.ToString())));
					if (!base.HasErrors)
					{
						umdialPlan.UMAutoAttendant = umautoAttendant2.Id;
					}
				}
				else
				{
					umdialPlan.UMAutoAttendant = null;
				}
			}
			TaskLogger.LogExit();
			return umdialPlan;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			LocalizedString empty = LocalizedString.Empty;
			if (!DialGroupEntry.ValidateGroup(this.DataObject.ConfiguredInCountryOrRegionGroups, this.DataObject.AllowedInCountryOrRegionGroups, true, out empty))
			{
				base.WriteError(new Exception(empty), ErrorCategory.WriteError, this.DataObject);
			}
			if (!DialGroupEntry.ValidateGroup(this.DataObject.ConfiguredInternationalGroups, this.DataObject.AllowedInternationalGroups, false, out empty))
			{
				base.WriteError(new Exception(empty), ErrorCategory.WriteError, this.DataObject);
			}
			if (!string.IsNullOrEmpty(this.DataObject.DefaultOutboundCallingLineId) && !Utils.IsUriValid(this.DataObject.DefaultOutboundCallingLineId, this.DataObject))
			{
				base.WriteError(new InvalidParameterException(Strings.InvalidDefaultOutboundCallingLineId), ErrorCategory.WriteError, this.DataObject);
			}
			if (this.DataObject.IsModified(UMDialPlanSchema.DefaultLanguage) && !Utility.IsUMLanguageAvailable(this.DataObject.DefaultLanguage))
			{
				base.WriteError(new InvalidParameterException(Strings.DefaultLanguageNotAvailable(this.DataObject.DefaultLanguage.DisplayName)), ErrorCategory.WriteError, this.DataObject);
			}
			MultiValuedProperty<string> pilotIdentifierList = this.DataObject.PilotIdentifierList;
			if (this.DataObject.IsChanged(UMDialPlanSchema.PilotIdentifierList) && pilotIdentifierList != null)
			{
				LocalizedException ex = ValidationHelper.ValidateE164Entries(this.DataObject, this.DataObject.PilotIdentifierList);
				if (ex != null)
				{
					base.WriteError(ex, ErrorCategory.NotSpecified, this.DataObject);
				}
				if (this.DataObject.URIType == UMUriType.SipName)
				{
					Utility.CheckForPilotIdentifierDuplicates(this.DataObject, this.ConfigurationSession, pilotIdentifierList, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (!base.IsUpgrading || this.ForceUpgrade || base.ShouldContinue(Strings.ShouldUpgradeObjectVersion("UMDialPlan")))
			{
				base.InternalProcessRecord();
			}
		}

		private const string CountryOrRegionCodeName = "CountryOrRegionCode";
	}
}
