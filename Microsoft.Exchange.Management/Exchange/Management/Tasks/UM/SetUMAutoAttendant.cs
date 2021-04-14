using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Set", "UMAutoAttendant", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class SetUMAutoAttendant : SetSystemConfigurationObjectTask<UMAutoAttendantIdParameter, UMAutoAttendant>
	{
		[Parameter(Mandatory = false)]
		public UMAutoAttendantIdParameter DTMFFallbackAutoAttendant
		{
			get
			{
				return (UMAutoAttendantIdParameter)base.Fields["DTMFFallbackAutoAttendant"];
			}
			set
			{
				base.Fields["DTMFFallbackAutoAttendant"] = value;
			}
		}

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

		[Parameter(Mandatory = false)]
		public string TimeZone
		{
			get
			{
				return (string)base.Fields["TimeZone"];
			}
			set
			{
				base.Fields["TimeZone"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMTimeZone TimeZoneName
		{
			get
			{
				return (UMTimeZone)base.Fields["TimeZoneName"];
			}
			set
			{
				base.Fields["TimeZoneName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxIdParameter DefaultMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["DefaultMailbox"];
			}
			set
			{
				base.Fields["DefaultMailbox"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetUMAutoAttendant(this.Identity.ToString());
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

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || ValidationHelper.IsKnownException(exception);
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			UMAutoAttendant umautoAttendant = (UMAutoAttendant)base.PrepareDataObject();
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
			if (base.Fields.IsModified("TimeZone") && base.Fields.IsModified("TimeZoneName"))
			{
				base.WriteError(new InvalidParameterException(Strings.InvalidTimeZoneParameters), ErrorCategory.NotSpecified, null);
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
						umautoAttendant.ContactAddressList = adconfigurationObject.Id;
					}
				}
				else
				{
					umautoAttendant.ContactAddressList = null;
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
							umautoAttendant.ContactAddressList = (ADObjectId)enumerator.Current.Identity;
						}
						goto IL_193;
					}
				}
				umautoAttendant.ContactAddressList = null;
			}
			IL_193:
			if (base.Fields.IsModified("DTMFFallbackAutoAttendant"))
			{
				UMAutoAttendantIdParameter dtmffallbackAutoAttendant = this.DTMFFallbackAutoAttendant;
				if (dtmffallbackAutoAttendant != null)
				{
					this.fallbackAA = (UMAutoAttendant)base.GetDataObject<UMAutoAttendant>(dtmffallbackAutoAttendant, this.ConfigurationSession, null, new LocalizedString?(Strings.NonExistantAutoAttendant(dtmffallbackAutoAttendant.ToString())), new LocalizedString?(Strings.MultipleAutoAttendantsWithSameId(dtmffallbackAutoAttendant.ToString())));
					umautoAttendant.DTMFFallbackAutoAttendant = this.fallbackAA.Id;
				}
				else
				{
					umautoAttendant.DTMFFallbackAutoAttendant = null;
				}
			}
			if (base.Fields.IsModified("DefaultMailbox"))
			{
				if (this.DefaultMailbox == null)
				{
					umautoAttendant.DefaultMailbox = null;
					umautoAttendant.DefaultMailboxLegacyDN = null;
				}
				else
				{
					IRecipientSession recipientSessionScopedToOrganization = Utility.GetRecipientSessionScopedToOrganization(umautoAttendant.OrganizationId, true);
					LocalizedString value = Strings.InvalidMailbox(this.DefaultMailbox.ToString(), "DefaultMailbox");
					umautoAttendant.DefaultMailbox = (ADUser)base.GetDataObject<ADUser>(this.DefaultMailbox, recipientSessionScopedToOrganization, null, null, new LocalizedString?(value), new LocalizedString?(value));
					umautoAttendant.DefaultMailboxLegacyDN = umautoAttendant.DefaultMailbox.LegacyExchangeDN;
				}
			}
			if (!base.HasErrors)
			{
				if (base.Fields.IsModified("TimeZone"))
				{
					umautoAttendant.TimeZone = this.TimeZone;
				}
				if (base.Fields.IsModified("TimeZoneName"))
				{
					umautoAttendant.TimeZoneName = this.TimeZoneName;
				}
			}
			TaskLogger.LogExit();
			return umautoAttendant;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				UMDialPlan dialPlan = this.DataObject.GetDialPlan();
				if (dialPlan == null)
				{
					base.WriteError(new DialPlanNotFoundException(this.DataObject.UMDialPlan.Name), ErrorCategory.NotSpecified, null);
				}
				int numberOfDigitsInExtension = dialPlan.NumberOfDigitsInExtension;
				MultiValuedProperty<string> multiValuedProperty = null;
				multiValuedProperty = this.DataObject.PilotIdentifierList;
				if (this.DataObject.IsChanged(UMAutoAttendantSchema.PilotIdentifierList) && multiValuedProperty != null)
				{
					LocalizedException ex = ValidationHelper.ValidateDialedNumbers(this.DataObject.PilotIdentifierList, dialPlan);
					if (ex != null)
					{
						base.WriteError(ex, ErrorCategory.NotSpecified, this.DataObject);
					}
					foreach (string text in this.DataObject.PilotIdentifierList)
					{
						UMAutoAttendant umautoAttendant = UMAutoAttendant.FindAutoAttendantByPilotIdentifierAndDialPlan(text, this.DataObject.UMDialPlan);
						if (umautoAttendant != null && !umautoAttendant.Guid.Equals(this.DataObject.Guid))
						{
							base.WriteError(new AutoAttendantExistsException(text, this.DataObject.UMDialPlan.Name), ErrorCategory.NotSpecified, null);
						}
					}
					if (dialPlan.URIType == UMUriType.SipName)
					{
						Utility.CheckForPilotIdentifierDuplicates(this.DataObject, this.ConfigurationSession, multiValuedProperty, new Task.TaskErrorLoggingDelegate(base.WriteError));
					}
				}
				string timeZone = this.DataObject.TimeZone;
				if (this.DataObject.IsChanged(UMAutoAttendantSchema.BusinessHourFeatures))
				{
					ValidationHelper.ValidateTimeZone(timeZone);
				}
				string property;
				try
				{
					property = UMAutoAttendantSchema.BusinessHoursKeyMapping.ToString();
					MultiValuedProperty<CustomMenuKeyMapping> multiValuedProperty2 = this.DataObject.BusinessHoursKeyMapping;
					if (multiValuedProperty2 != null && multiValuedProperty2.Count > 0)
					{
						bool flag;
						ValidationHelper.ValidateCustomMenu(Strings.BusinessHoursSettings, this.ConfigurationSession, property, multiValuedProperty2, numberOfDigitsInExtension, this.DataObject, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), out flag);
						if (flag)
						{
							this.DataObject.BusinessHoursKeyMapping = multiValuedProperty2;
						}
					}
					property = UMAutoAttendantSchema.AfterHoursKeyMapping.ToString();
					multiValuedProperty2 = this.DataObject.AfterHoursKeyMapping;
					if (multiValuedProperty2 != null && multiValuedProperty2.Count > 0)
					{
						bool flag2;
						ValidationHelper.ValidateCustomMenu(Strings.AfterHoursSettings, this.ConfigurationSession, property, multiValuedProperty2, numberOfDigitsInExtension, this.DataObject, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), out flag2);
						if (flag2)
						{
							this.DataObject.AfterHoursKeyMapping = multiValuedProperty2;
						}
					}
				}
				catch (LocalizedException exception)
				{
					base.WriteError(exception, ErrorCategory.NotSpecified, null);
				}
				bool speechEnabled = this.DataObject.SpeechEnabled;
				StatusEnum status = this.DataObject.Status;
				property = UMAutoAttendantSchema.DTMFFallbackAutoAttendant.ToString();
				if (this.fallbackAA != null)
				{
					ValidationHelper.ValidateDtmfFallbackAA(this.DataObject, dialPlan, this.fallbackAA);
				}
				ADObjectId adobjectId = null;
				property = UMAutoAttendantSchema.AutomaticSpeechRecognitionEnabled.ToString();
				if (this.DataObject.IsChanged(UMAutoAttendantSchema.AutomaticSpeechRecognitionEnabled) && speechEnabled && ValidationHelper.IsFallbackAAInDialPlan(this.ConfigurationSession, this.DataObject, out adobjectId))
				{
					base.WriteError(new InvalidDtmfFallbackAutoAttendantException(Strings.InvalidSpeechEnabledAutoAttendant(adobjectId.ToString())), ErrorCategory.NotSpecified, null);
				}
				property = UMAutoAttendantSchema.Language.ToString();
				if (this.DataObject.IsChanged(UMAutoAttendantSchema.Language))
				{
					UMLanguage language = this.DataObject.Language;
					if (!Utility.IsUMLanguageAvailable(language))
					{
						base.WriteError(new InvalidLanguageIdException(language.ToString()), ErrorCategory.NotSpecified, null);
					}
				}
				bool flag3 = this.IsBusinessHours();
				if (!this.DataObject.NameLookupEnabled && !this.DataObject.CallSomeoneEnabled && ((flag3 && !this.DataObject.BusinessHoursTransferToOperatorEnabled && !this.DataObject.BusinessHoursKeyMappingEnabled) || (!flag3 && !this.DataObject.AfterHoursTransferToOperatorEnabled && !this.DataObject.AfterHoursKeyMappingEnabled)))
				{
					base.WriteError(new InvalidAutoAttendantException(Strings.InvalidMethodToDisableAA), ErrorCategory.NotSpecified, null);
				}
				LocalizedString empty = LocalizedString.Empty;
				if (!DialGroupEntry.ValidateGroup(dialPlan.ConfiguredInCountryOrRegionGroups, this.DataObject.AllowedInCountryOrRegionGroups, true, out empty))
				{
					base.WriteError(new Exception(empty), ErrorCategory.WriteError, this.DataObject);
				}
				if (!DialGroupEntry.ValidateGroup(dialPlan.ConfiguredInternationalGroups, this.DataObject.AllowedInternationalGroups, false, out empty))
				{
					base.WriteError(new Exception(empty), ErrorCategory.WriteError, this.DataObject);
				}
				if (this.DataObject.ForwardCallsToDefaultMailbox && string.IsNullOrEmpty(this.DataObject.DefaultMailboxLegacyDN))
				{
					base.WriteError(new InvalidParameterException(Strings.DefaultMailboxRequiredWhenForwardTrue), ErrorCategory.NotSpecified, null);
				}
				if (this.DataObject.IsModified(UMAutoAttendantSchema.ContactScope) && this.DataObject.ContactScope == DialScopeEnum.DialPlan && dialPlan.SubscriberType == UMSubscriberType.Consumer)
				{
					base.WriteError(new InvalidParameterException(Strings.InvalidAutoAttendantScopeSetting), (ErrorCategory)1000, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (!base.IsUpgrading || this.ForceUpgrade || base.ShouldContinue(Strings.ShouldUpgradeObjectVersion("UMAutoAttendant")))
			{
				base.InternalProcessRecord();
			}
		}

		private bool IsBusinessHours()
		{
			bool result = false;
			foreach (ScheduleInterval scheduleInterval in this.DataObject.BusinessHoursSchedule)
			{
				if (scheduleInterval.Length > TimeSpan.Zero)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private UMAutoAttendant fallbackAA;
	}
}
