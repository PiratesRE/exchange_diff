using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.SQM;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "UMMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetUMMailbox : SetUMMailboxBase<MailboxIdParameter, UMMailbox>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetUMMailbox(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneNumber
		{
			get
			{
				return (string)base.Fields["PhoneNumber"];
			}
			set
			{
				base.Fields["PhoneNumber"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AirSyncNumbers
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["AirSyncNumbers"];
			}
			set
			{
				base.Fields["AirSyncNumbers"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter VerifyGlobalRoutingEntry
		{
			get
			{
				return (SwitchParameter)(base.Fields["VerifyGlobalRoutingEntry"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["VerifyGlobalRoutingEntry"] = value;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (MailboxTaskHelper.ExcludeMailboxPlan(adrecipient, false))
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1000, this.Identity);
			}
			return adrecipient;
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			if (base.UMMailboxPolicy != null)
			{
				this.newMailboxPolicy = (UMMailboxPolicy)base.GetDataObject<UMMailboxPolicy>(base.UMMailboxPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorManagedFolderMailboxPolicyNotFound(base.UMMailboxPolicy.ToString())), new LocalizedString?(Strings.ErrorManagedFolderMailboxPolicyNotUnique(base.UMMailboxPolicy.ToString())));
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)base.PrepareDataObject();
			if ((aduser.UMEnabledFlags & UMEnabledFlags.UMEnabled) != UMEnabledFlags.UMEnabled)
			{
				base.WriteError(new RecipientTaskException(Strings.MailboxNotUmEnabled(this.Identity.ToString())), (ErrorCategory)1000, aduser);
			}
			if (base.UMMailboxPolicy != null)
			{
				IConfigurationSession configurationSession = this.ConfigurationSession;
				ADObjectId ummailboxPolicy = aduser.UMMailboxPolicy;
				if (ummailboxPolicy != null)
				{
					MailboxPolicyIdParameter id = new MailboxPolicyIdParameter(ummailboxPolicy);
					UMMailboxPolicy ummailboxPolicy2 = (UMMailboxPolicy)base.GetDataObject<UMMailboxPolicy>(id, configurationSession, null, new LocalizedString?(Strings.ErrorManagedFolderMailboxPolicyNotFound(ummailboxPolicy.ToString())), new LocalizedString?(Strings.ErrorManagedFolderMailboxPolicyNotUnique(ummailboxPolicy.ToString())));
					if (!ummailboxPolicy2.UMDialPlan.Equals(this.newMailboxPolicy.UMDialPlan))
					{
						base.WriteError(new RecipientTaskException(Strings.NewPolicyMustBeInTheSameDialPlanAsOldPolicy(ummailboxPolicy2.UMDialPlan.Name)), (ErrorCategory)1000, aduser);
					}
				}
				aduser.UMMailboxPolicy = this.newMailboxPolicy.Id;
			}
			if (base.Fields.IsModified("PhoneNumber"))
			{
				this.SetPhoneNumber(aduser);
			}
			if (base.Fields.IsModified("AirSyncNumbers"))
			{
				this.SetAirSyncNumber(aduser);
			}
			TaskLogger.LogExit();
			return aduser;
		}

		private void SetAirSyncNumber(ADUser user)
		{
			UMMailboxPolicy ummailboxPolicy = this.ReadPolicyObject(user);
			UMDialPlan dialPlan = ummailboxPolicy.GetDialPlan();
			MultiValuedProperty<string> extensionsFromCollection = UMMailbox.GetExtensionsFromCollection(user.UMAddresses, ProxyAddressPrefix.ASUM, dialPlan.PhoneContext);
			Hashtable hashtable = new Hashtable();
			List<string> list = new List<string>();
			foreach (string key in extensionsFromCollection)
			{
				hashtable.Add(key, false);
			}
			foreach (string text in this.AirSyncNumbers)
			{
				if (hashtable.ContainsKey(text))
				{
					hashtable[text] = true;
				}
				else
				{
					list.Add(text);
				}
			}
			foreach (object obj in hashtable)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				if (!(bool)dictionaryEntry.Value)
				{
					TelephoneNumberProcessStatus status;
					AirSyncUtils.RemoveAirSyncPhoneNumber(user, (string)dictionaryEntry.Key, out status);
					ArgumentException ex = this.HandleResult(status, (string)dictionaryEntry.Key);
					if (ex != null)
					{
						base.WriteError(ex, (ErrorCategory)1001, null);
					}
				}
			}
			foreach (string text2 in list)
			{
				TelephoneNumberProcessStatus status2;
				AirSyncUtils.AddAirSyncPhoneNumber(user, text2, out status2);
				ArgumentException ex2 = this.HandleResult(status2, text2);
				if (ex2 != null)
				{
					base.WriteError(ex2, (ErrorCategory)1001, null);
				}
			}
		}

		private void SetPhoneNumber(ADUser user)
		{
			UMMailboxPolicy ummailboxPolicy = this.ReadPolicyObject(user);
			UMDialPlan dialPlan = ummailboxPolicy.GetDialPlan();
			if (dialPlan.URIType != UMUriType.E164 || dialPlan.SubscriberType != UMSubscriberType.Consumer)
			{
				base.WriteError(new ArgumentException(Strings.PhoneNumberAllowedOnlyOnE164ConsumerDialplan, "PhoneNumber"), (ErrorCategory)1000, null);
			}
			if (string.IsNullOrEmpty(dialPlan.CountryOrRegionCode))
			{
				base.WriteError(new ArgumentException(Strings.PhoneNumberAllowedOnlyWithDialplanWithCountryCode, "PhoneNumber"), (ErrorCategory)1000, null);
			}
			if (this.PhoneNumber == string.Empty)
			{
				Utils.UMPopulate(user, null, null, ummailboxPolicy, dialPlan);
				return;
			}
			PhoneNumber phoneNumber;
			if (!Microsoft.Exchange.UM.UMCommon.PhoneNumber.TryParse(this.PhoneNumber, out phoneNumber) || phoneNumber.UriType != UMUriType.TelExtn)
			{
				base.WriteError(new ArgumentException(Strings.PhoneNumberNotANumber(dialPlan.NumberOfDigitsInExtension), this.PhoneNumber), (ErrorCategory)1000, null);
			}
			string sipResourceIdentifier = "+" + dialPlan.CountryOrRegionCode + phoneNumber.Number;
			IRecipientSession tenantLocalRecipientSession = RecipientTaskHelper.GetTenantLocalRecipientSession(user.OrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId);
			LocalizedException ex = null;
			TelephoneNumberProcessStatus telephoneNumberProcessStatus;
			Utils.ValidateExtensionsAndSipResourceIdentifier(tenantLocalRecipientSession, this.ConfigurationSession, CommonConstants.DataCenterADPresent, user, dialPlan, new string[]
			{
				phoneNumber.Number
			}, new string[]
			{
				this.PhoneNumber
			}, sipResourceIdentifier, out ex, out telephoneNumberProcessStatus);
			if (ex != null)
			{
				base.WriteError(ex, (ErrorCategory)1000, null);
			}
			if (telephoneNumberProcessStatus != TelephoneNumberProcessStatus.PhoneNumberAlreadyRegistered)
			{
				Utils.UMPopulate(user, sipResourceIdentifier, new MultiValuedProperty<string>(phoneNumber.Number), ummailboxPolicy, dialPlan);
			}
		}

		private UMMailboxPolicy ReadPolicyObject(ADUser user)
		{
			return (UMMailboxPolicy)base.GetDataObject<UMMailboxPolicy>(new MailboxPolicyIdParameter(user.UMMailboxPolicy), this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorManagedFolderMailboxPolicyNotFound(user.UMMailboxPolicy.ToString())), new LocalizedString?(Strings.ErrorManagedFolderMailboxPolicyNotUnique(user.UMMailboxPolicy.ToString())));
		}

		private ArgumentException HandleResult(TelephoneNumberProcessStatus status, string phoneNumber)
		{
			switch (status)
			{
			case TelephoneNumberProcessStatus.DialPlanNotSupported:
				return new ArgumentException(Strings.PhoneNumberAllowedOnlyOnE164ConsumerDialplanWithCountryCode, "AirSyncNumbers");
			case TelephoneNumberProcessStatus.PhoneNumberAlreadyRegistered:
				return new ArgumentException(Strings.PhoneNumberAlreadyRegistered, phoneNumber);
			case TelephoneNumberProcessStatus.PhoneNumberReachQuota:
				return new ArgumentException(Strings.PhoneNumberReachQuota, phoneNumber);
			case TelephoneNumberProcessStatus.PhoneNumberUsedByOthers:
				return new ArgumentException(Strings.PhoneNumberUsedByOthers, phoneNumber);
			case TelephoneNumberProcessStatus.PhoneNumberInvalidFormat:
				return new ArgumentException(Strings.PhoneNumberIsNotE164, phoneNumber);
			case TelephoneNumberProcessStatus.PhoneNumberInvalidCountryCode:
				return new ArgumentException(Strings.PhoneNumberInvalidCountryCode, phoneNumber);
			case TelephoneNumberProcessStatus.PhoneNumberInvalidLength:
				return new ArgumentException(Strings.PhoneNumberInvalidLength, phoneNumber);
			default:
				return null;
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			UMMailbox ummailbox = UMMailbox.FromDataObject(this.DataObject);
			SmsSqmDataPointHelper.AddNotificationConfigDataPoint(SmsSqmSession.Instance, ummailbox.Id, ummailbox.LegacyExchangeDN, SmsSqmDataPointHelper.TranslateEnumForSqm<UMSMSNotificationOptions>(ummailbox.UMSMSNotificationOption));
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return UMMailbox.FromDataObject((ADUser)dataObject);
		}

		private const string VerifyGlobalRoutingEntryName = "VerifyGlobalRoutingEntry";

		private const string PhoneNumberName = "PhoneNumber";

		private const string AirSyncNumbersName = "AirSyncNumbers";

		private UMMailboxPolicy newMailboxPolicy;

		private static TimeSpan VerifyGlobalRoutingEntryTimeout = TimeSpan.FromMinutes(1.0);

		private static TimeSpan VerifyGlobalRoutingEntryPollingInterval = TimeSpan.FromSeconds(5.0);
	}
}
