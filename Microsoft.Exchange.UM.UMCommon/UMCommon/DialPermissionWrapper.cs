using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class DialPermissionWrapper
	{
		public ADObjectId SearchRoot
		{
			get
			{
				return this.searchRoot;
			}
			set
			{
				this.searchRoot = value;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
			set
			{
				this.organizationId = value;
			}
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
			set
			{
				this.identity = value;
			}
		}

		public MultiValuedProperty<string> AllowedInCountryGroups
		{
			get
			{
				return this.allowedInCountryGroups;
			}
			set
			{
				this.allowedInCountryGroups = value;
			}
		}

		public MultiValuedProperty<string> AllowedInternationalGroups
		{
			get
			{
				return this.allowedInternationalGroups;
			}
			set
			{
				this.allowedInternationalGroups = value;
			}
		}

		public bool DialPlanSubscribersAllowed
		{
			get
			{
				return this.dialPlanSubscribersAllowed;
			}
			set
			{
				this.dialPlanSubscribersAllowed = value;
			}
		}

		public bool ExtensionLengthNumbersAllowed
		{
			get
			{
				return this.extensionLengthNumbersAllowed;
			}
			set
			{
				this.extensionLengthNumbersAllowed = value;
			}
		}

		public bool CallSomeoneEnabled
		{
			get
			{
				return this.callSomeoneEnabled;
			}
			set
			{
				this.callSomeoneEnabled = value;
			}
		}

		public bool SendVoiceMessageEnabled
		{
			get
			{
				return this.sendVoiceMessageEnabled;
			}
			set
			{
				this.sendVoiceMessageEnabled = value;
			}
		}

		internal bool CallingNonUmExtensionsAllowed
		{
			get
			{
				return this.extensionLengthNumbersAllowed;
			}
		}

		internal DialPermissionType Category
		{
			get
			{
				return this.category;
			}
			set
			{
				this.category = value;
			}
		}

		internal DialScopeEnum ContactScope
		{
			get
			{
				return this.contactScope;
			}
		}

		internal static DialPermissionWrapper CreateFromDialPlan(UMDialPlan dialPlan)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "DialPermissionWrapper::CreateFromDialPlan(for DP = {0})", new object[]
			{
				dialPlan.Name
			});
			DialPermissionWrapper dialPermissionWrapper = new DialPermissionWrapper();
			dialPermissionWrapper.Category = DialPermissionType.DialPlan;
			dialPermissionWrapper.Identity = dialPlan.Id.DistinguishedName;
			dialPermissionWrapper.AllowedInCountryGroups = dialPlan.AllowedInCountryOrRegionGroups;
			dialPermissionWrapper.AllowedInternationalGroups = dialPlan.AllowedInternationalGroups;
			dialPermissionWrapper.DialPlanSubscribersAllowed = dialPlan.AllowDialPlanSubscribers;
			dialPermissionWrapper.ExtensionLengthNumbersAllowed = dialPlan.AllowExtensions;
			dialPermissionWrapper.CallSomeoneEnabled = dialPlan.CallSomeoneEnabled;
			dialPermissionWrapper.contactScope = DialScopeEnum.DialPlan;
			switch (dialPlan.ContactScope)
			{
			case CallSomeoneScopeEnum.DialPlan:
				dialPermissionWrapper.contactScope = DialScopeEnum.DialPlan;
				goto IL_C7;
			case CallSomeoneScopeEnum.GlobalAddressList:
				dialPermissionWrapper.contactScope = DialScopeEnum.GlobalAddressList;
				goto IL_C7;
			case CallSomeoneScopeEnum.AddressList:
				dialPermissionWrapper.contactScope = DialScopeEnum.AddressList;
				goto IL_C7;
			}
			dialPermissionWrapper.contactScope = DialScopeEnum.DialPlan;
			IL_C7:
			dialPermissionWrapper.SendVoiceMessageEnabled = dialPlan.SendVoiceMsgEnabled;
			dialPermissionWrapper.SearchRoot = dialPlan.ContactAddressList;
			dialPermissionWrapper.OrganizationId = dialPlan.OrganizationId;
			return dialPermissionWrapper;
		}

		internal static DialPermissionWrapper CreateFromAutoAttendant(UMAutoAttendant autoAttendant)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "DialPermissionWrapper::CreateFromAutoAttendant(for AA = {0})", new object[]
			{
				autoAttendant.Name
			});
			return new DialPermissionWrapper
			{
				Category = DialPermissionType.AutoAttendant,
				Identity = autoAttendant.Id.DistinguishedName,
				AllowedInCountryGroups = autoAttendant.AllowedInCountryOrRegionGroups,
				AllowedInternationalGroups = autoAttendant.AllowedInternationalGroups,
				DialPlanSubscribersAllowed = autoAttendant.AllowDialPlanSubscribers,
				ExtensionLengthNumbersAllowed = autoAttendant.AllowExtensions,
				CallSomeoneEnabled = autoAttendant.CallSomeoneEnabled,
				contactScope = autoAttendant.ContactScope,
				SendVoiceMessageEnabled = autoAttendant.SendVoiceMsgEnabled,
				SearchRoot = autoAttendant.ContactAddressList,
				OrganizationId = autoAttendant.OrganizationId
			};
		}

		internal static DialPermissionWrapper CreateFromRecipientPolicy(ADUser user)
		{
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, user.DisplayName);
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data, "DialPermissionWrapper::CreateFromRecipientPolicy(for user = _UserDisplayName)", new object[0]);
			DialPermissionWrapper dialPermissionWrapper = new DialPermissionWrapper();
			if (user.UMMailboxPolicy == null)
			{
				throw new ADUMUserInvalidUMMailboxPolicyException(user.PrimarySmtpAddress.ToString());
			}
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(user);
			UMMailboxPolicy policyFromRecipient = iadsystemConfigurationLookup.GetPolicyFromRecipient(user);
			if (policyFromRecipient == null)
			{
				throw new ADUMUserInvalidUMMailboxPolicyException(user.PrimarySmtpAddress.ToString());
			}
			dialPermissionWrapper.Category = DialPermissionType.MailboxPolicy;
			dialPermissionWrapper.Identity = user.DisplayName;
			dialPermissionWrapper.AllowedInCountryGroups = policyFromRecipient.AllowedInCountryOrRegionGroups;
			dialPermissionWrapper.AllowedInternationalGroups = policyFromRecipient.AllowedInternationalGroups;
			dialPermissionWrapper.DialPlanSubscribersAllowed = policyFromRecipient.AllowDialPlanSubscribers;
			dialPermissionWrapper.ExtensionLengthNumbersAllowed = policyFromRecipient.AllowExtensions;
			if (user.AddressBookPolicy != null)
			{
				dialPermissionWrapper.SearchRoot = user.GlobalAddressListFromAddressBookPolicy;
			}
			else
			{
				dialPermissionWrapper.SearchRoot = null;
			}
			dialPermissionWrapper.OrganizationId = user.OrganizationId;
			dialPermissionWrapper.contactScope = ((dialPermissionWrapper.SearchRoot != null) ? DialScopeEnum.AddressList : DialScopeEnum.GlobalAddressList);
			dialPermissionWrapper.CallSomeoneEnabled = true;
			dialPermissionWrapper.SendVoiceMessageEnabled = true;
			return dialPermissionWrapper;
		}

		private MultiValuedProperty<string> allowedInCountryGroups;

		private MultiValuedProperty<string> allowedInternationalGroups;

		private bool dialPlanSubscribersAllowed;

		private bool extensionLengthNumbersAllowed;

		private DialScopeEnum contactScope;

		private bool callSomeoneEnabled;

		private bool sendVoiceMessageEnabled;

		private ADObjectId searchRoot;

		private OrganizationId organizationId;

		private DialPermissionType category;

		private string identity;
	}
}
