using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class UMMailbox : ADPresentationObject
	{
		internal static RecipientTypeDetails[] GetUMRecipientTypeDetails()
		{
			return (RecipientTypeDetails[])UMMailbox.AllowedRecipientTypeDetails.Clone();
		}

		internal static MultiValuedProperty<string> GetExtensionsFromEmailAddresses(ProxyAddressCollection emailAddresses)
		{
			List<string> list = new List<string>();
			bool flag = UMMailbox.ContainsMoreThanOneDialplan(emailAddresses);
			foreach (ProxyAddress proxyAddress in emailAddresses)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.UM)
				{
					int num = proxyAddress.AddressString.IndexOf(";");
					if (num != -1)
					{
						if (flag)
						{
							string phoneContextFromProxyAddress = UMMailbox.GetPhoneContextFromProxyAddress(proxyAddress);
							string extensionFromProxyAddress = UMMailbox.GetExtensionFromProxyAddress(proxyAddress);
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.AppendFormat("{0} ({1})", extensionFromProxyAddress, phoneContextFromProxyAddress);
							list.Add(stringBuilder.ToString());
						}
						else
						{
							list.Add(UMMailbox.GetExtensionFromProxyAddress(proxyAddress));
						}
					}
				}
			}
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			multiValuedProperty.CopyChangesOnly = true;
			foreach (string item in list)
			{
				multiValuedProperty.Add(item);
			}
			return multiValuedProperty;
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return UMMailbox.schema;
			}
		}

		public UMMailbox()
		{
		}

		public UMMailbox(ADUser dataObject) : base(dataObject)
		{
		}

		internal static UMMailbox FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new UMMailbox(dataObject);
		}

		protected override IEnumerable<PropertyInfo> CloneableProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = UMMailbox.cloneableProps) == null)
				{
					result = (UMMailbox.cloneableProps = ADPresentationObject.GetCloneableProperties(this));
				}
				return result;
			}
		}

		protected override IEnumerable<PropertyInfo> CloneableOnceProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = UMMailbox.cloneableOnceProps) == null)
				{
					result = (UMMailbox.cloneableOnceProps = ADPresentationObject.GetCloneableOnceProperties(this));
				}
				return result;
			}
		}

		protected override IEnumerable<PropertyInfo> CloneableEnabledStateProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = UMMailbox.cloneableEnabledStateProps) == null)
				{
					result = (UMMailbox.cloneableEnabledStateProps = ADPresentationObject.GetCloneableEnabledStateProperties(this));
				}
				return result;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal bool UMProvisioningRequested
		{
			get
			{
				return (bool)this[UMMailboxSchema.UMProvisioningRequested];
			}
			set
			{
				this[UMMailboxSchema.UMProvisioningRequested] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[UMMailboxSchema.DisplayName];
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[UMMailboxSchema.EmailAddresses];
			}
			internal set
			{
				this[UMMailboxSchema.EmailAddresses] = value;
			}
		}

		public ProxyAddressCollection UMAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[UMMailboxSchema.UMAddresses];
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[UMMailboxSchema.LegacyExchangeDN];
			}
		}

		public string LinkedMasterAccount
		{
			get
			{
				return (string)this[UMMailboxSchema.LinkedMasterAccount];
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[UMMailboxSchema.PrimarySmtpAddress];
			}
			internal set
			{
				this[UMMailboxSchema.PrimarySmtpAddress] = value;
			}
		}

		public string SamAccountName
		{
			get
			{
				return (string)this[UMMailboxSchema.SamAccountName];
			}
			internal set
			{
				this[UMMailboxSchema.SamAccountName] = value;
			}
		}

		public string ServerLegacyDN
		{
			get
			{
				return (string)this[UMMailboxSchema.ServerLegacyDN];
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[UMMailboxSchema.ServerName];
			}
		}

		public MultiValuedProperty<string> UMDtmfMap
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMMailboxSchema.UMDtmfMap];
			}
		}

		public bool UMEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.UMEnabled];
			}
			internal set
			{
				this[UMMailboxSchema.UMEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TUIAccessToCalendarEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.TUIAccessToCalendarEnabled];
			}
			set
			{
				this[UMMailboxSchema.TUIAccessToCalendarEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FaxEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.FaxEnabled];
			}
			set
			{
				this[UMMailboxSchema.FaxEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TUIAccessToEmailEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.TUIAccessToEmailEnabled];
			}
			set
			{
				this[UMMailboxSchema.TUIAccessToEmailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SubscriberAccessEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.SubscriberAccessEnabled];
			}
			set
			{
				this[UMMailboxSchema.SubscriberAccessEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MissedCallNotificationEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.MissedCallNotificationEnabled];
			}
			set
			{
				this[UMMailboxSchema.MissedCallNotificationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMSMSNotificationOptions UMSMSNotificationOption
		{
			get
			{
				return (UMSMSNotificationOptions)this[UMMailboxSchema.UMSMSNotificationOption];
			}
			set
			{
				this[UMMailboxSchema.UMSMSNotificationOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PinlessAccessToVoiceMailEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.PinlessAccessToVoiceMailEnabled];
			}
			set
			{
				this[UMMailboxSchema.PinlessAccessToVoiceMailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AnonymousCallersCanLeaveMessages
		{
			get
			{
				return (bool)this[UMMailboxSchema.AnonymousCallersCanLeaveMessages];
			}
			set
			{
				this[UMMailboxSchema.AnonymousCallersCanLeaveMessages] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutomaticSpeechRecognitionEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.ASREnabled];
			}
			set
			{
				this[UMMailboxSchema.ASREnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool VoiceMailAnalysisEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.VoiceMailAnalysisEnabled];
			}
			set
			{
				this[UMMailboxSchema.VoiceMailAnalysisEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PlayOnPhoneEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.PlayOnPhoneEnabled];
			}
			set
			{
				this[UMMailboxSchema.PlayOnPhoneEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CallAnsweringRulesEnabled
		{
			get
			{
				return (bool)this[UMMailboxSchema.CallAnsweringRulesEnabled];
			}
			set
			{
				this[UMMailboxSchema.CallAnsweringRulesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AllowUMCallsFromNonUsersFlags AllowUMCallsFromNonUsers
		{
			get
			{
				return (AllowUMCallsFromNonUsersFlags)this[UMMailboxSchema.AllowUMCallsFromNonUsers];
			}
			set
			{
				this[UMMailboxSchema.AllowUMCallsFromNonUsers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OperatorNumber
		{
			get
			{
				return (string)this[UMMailboxSchema.OperatorNumber];
			}
			set
			{
				this[UMMailboxSchema.OperatorNumber] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneProviderId
		{
			get
			{
				return (string)this[UMMailboxSchema.PhoneProviderId];
			}
			set
			{
				this[UMMailboxSchema.PhoneProviderId] = value;
			}
		}

		public ADObjectId UMDialPlan
		{
			get
			{
				return (ADObjectId)this[UMMailboxSchema.UMRecipientDialPlanId];
			}
		}

		public ADObjectId UMMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[UMMailboxSchema.UMMailboxPolicy];
			}
			set
			{
				this[UMMailboxSchema.UMMailboxPolicy] = value;
			}
		}

		public MultiValuedProperty<string> Extensions
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMMailboxSchema.Extensions];
			}
		}

		[Parameter(Mandatory = false)]
		public AudioCodecEnum? CallAnsweringAudioCodec
		{
			get
			{
				return (AudioCodecEnum?)this[UMMailboxSchema.CallAnsweringAudioCodec];
			}
			set
			{
				this[UMMailboxSchema.CallAnsweringAudioCodec] = value;
			}
		}

		public string SIPResourceIdentifier
		{
			get
			{
				return (string)this[UMMailboxSchema.SIPResourceIdentifier];
			}
			internal set
			{
				this[UMMailboxSchema.SIPResourceIdentifier] = value;
			}
		}

		public string PhoneNumber
		{
			get
			{
				return (string)this[UMMailboxSchema.PhoneNumber];
			}
			internal set
			{
				this[UMMailboxSchema.PhoneNumber] = value;
			}
		}

		public MultiValuedProperty<string> AccessTelephoneNumbers
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMMailboxSchema.AccessTelephoneNumbers];
			}
			internal set
			{
				this[UMMailboxSchema.AccessTelephoneNumbers] = value;
			}
		}

		public MultiValuedProperty<string> CallAnsweringRulesExtensions
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMMailboxSchema.CallAnsweringRulesExtensions];
			}
			internal set
			{
				this[UMMailboxSchema.CallAnsweringRulesExtensions] = value;
			}
		}

		public MultiValuedProperty<string> AirSyncNumbers
		{
			get
			{
				ProxyAddressCollection addresses = (ProxyAddressCollection)this[UMMailboxSchema.UMAddresses];
				return UMMailbox.GetExtensionsFromCollection(addresses, ProxyAddressPrefix.ASUM, null);
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImListMigrationCompleted
		{
			get
			{
				return (bool)this[UMMailboxSchema.UCSImListMigrationCompleted];
			}
			set
			{
				this[UMMailboxSchema.UCSImListMigrationCompleted] = value;
			}
		}

		internal static MultiValuedProperty<string> ExtensionsGetter(IPropertyBag propertyBag)
		{
			ProxyAddressCollection emailAddresses = (ProxyAddressCollection)propertyBag[ADRecipientSchema.EmailAddresses];
			return UMMailbox.GetExtensionsFromEmailAddresses(emailAddresses);
		}

		internal static void AddProxy(ADRecipient recipient, ProxyAddressCollection addresses, string extension, UMDialPlan dialPlan, ProxyAddressPrefix prefix)
		{
			string prefixString = (null == UMMailbox.GetPrimaryExtension(addresses, prefix)) ? prefix.PrimaryPrefix : prefix.SecondaryPrefix;
			addresses.Add(UMMailbox.BuildProxyAddressFromExtensionAndPhoneContext(extension, prefixString, dialPlan.PhoneContext));
		}

		internal static void ClearProxy(ADRecipient recipient, ProxyAddressCollection targetAddresses, ProxyAddressPrefix targetPrefix, Hashtable safeTable)
		{
			Hashtable hashtable = new Hashtable();
			foreach (ProxyAddress proxyAddress in targetAddresses)
			{
				if (proxyAddress.Prefix == targetPrefix && (safeTable == null || !safeTable.ContainsKey(proxyAddress.AddressString)))
				{
					hashtable.Add(proxyAddress.AddressString, true);
				}
			}
			UMMailbox.RemoveProxy(recipient, targetAddresses, targetPrefix, hashtable);
		}

		internal static void RemoveProxy(ADRecipient recipient, ProxyAddressCollection collection, ProxyAddressPrefix prefix, Hashtable addressStringTable)
		{
			List<ProxyAddress> list = new List<ProxyAddress>();
			foreach (ProxyAddress proxyAddress in collection)
			{
				if (proxyAddress.Prefix == prefix && addressStringTable.ContainsKey(proxyAddress.AddressString))
				{
					list.Add(proxyAddress);
				}
			}
			foreach (ProxyAddress item in list)
			{
				collection.Remove(item);
			}
		}

		internal static void RemoveProxy(ADRecipient recipient, ProxyAddressCollection collection, ProxyAddressPrefix prefix, ArrayList phoneNumbers, UMDialPlan dialPlan)
		{
			Hashtable hashtable = new Hashtable();
			foreach (object obj in phoneNumbers)
			{
				string extension = (string)obj;
				hashtable.Add(UMMailbox.BuildAddressStringFromExtensionAndPhoneContext(extension, dialPlan.PhoneContext), true);
			}
			UMMailbox.RemoveProxy(recipient, collection, prefix, hashtable);
		}

		internal static string GetPrimaryExtension(ProxyAddressCollection emailAddresses, ProxyAddressPrefix prefix)
		{
			foreach (ProxyAddress proxyAddress in emailAddresses)
			{
				if (proxyAddress.IsPrimaryAddress && proxyAddress.Prefix == prefix)
				{
					string extensionFromProxyAddress = UMMailbox.GetExtensionFromProxyAddress(proxyAddress);
					if (extensionFromProxyAddress != null)
					{
						return extensionFromProxyAddress;
					}
				}
			}
			return null;
		}

		internal static bool ContainsMoreThanOneDialplan(ProxyAddressCollection emailAddresses)
		{
			string text = null;
			foreach (ProxyAddress proxyAddress in emailAddresses)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.UM)
				{
					string phoneContextFromProxyAddress = UMMailbox.GetPhoneContextFromProxyAddress(proxyAddress);
					if (text == null)
					{
						text = phoneContextFromProxyAddress;
					}
					else if (string.Compare(phoneContextFromProxyAddress, text, StringComparison.OrdinalIgnoreCase) != 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static MultiValuedProperty<string> GetExtensionsFromCollection(ProxyAddressCollection addresses, ProxyAddressPrefix prefix, string phoneContext)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			foreach (ProxyAddress proxyAddress in addresses)
			{
				if (proxyAddress.Prefix == prefix && (phoneContext == null || proxyAddress.AddressString.EndsWith(phoneContext)))
				{
					string extensionFromProxyAddress = UMMailbox.GetExtensionFromProxyAddress(proxyAddress);
					if (extensionFromProxyAddress != null)
					{
						multiValuedProperty.Add(extensionFromProxyAddress);
					}
				}
			}
			return multiValuedProperty;
		}

		internal static List<string> GetDialPlanPhoneContexts(ProxyAddressCollection proxyAddresses, bool excludePrimaryContext)
		{
			if (proxyAddresses == null)
			{
				throw new ArgumentNullException("proxyAddresses");
			}
			List<string> list = new List<string>();
			foreach (ProxyAddress proxyAddress in proxyAddresses)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.UM && (!proxyAddress.IsPrimaryAddress || !excludePrimaryContext))
				{
					string phoneContextFromProxyAddress = UMMailbox.GetPhoneContextFromProxyAddress(proxyAddress);
					if (phoneContextFromProxyAddress != null)
					{
						list.Add(phoneContextFromProxyAddress);
					}
				}
			}
			return list;
		}

		internal static Hashtable GetAirSyncSafeTable(ProxyAddressCollection collection, ProxyAddressPrefix prefix, UMDialPlan dialPlan)
		{
			Hashtable hashtable = new Hashtable();
			foreach (ProxyAddress proxyAddress in collection)
			{
				if (proxyAddress.Prefix == prefix)
				{
					hashtable.Add(proxyAddress.AddressString, true);
					hashtable.Add(proxyAddress.AddressString.Substring(dialPlan.CountryOrRegionCode.Length + 1), true);
				}
			}
			return hashtable;
		}

		internal static bool PhoneNumberExists(ProxyAddressCollection addresses, ProxyAddressPrefix prefix, string phoneNumber)
		{
			foreach (ProxyAddress proxyAddress in addresses)
			{
				if (proxyAddress.Prefix == prefix)
				{
					string extensionFromProxyAddress = UMMailbox.GetExtensionFromProxyAddress(proxyAddress);
					if (extensionFromProxyAddress.Equals(phoneNumber))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static int ProxyAddressCount(ProxyAddressCollection collection, ProxyAddressPrefix prefix)
		{
			int num = 0;
			foreach (ProxyAddress proxyAddress in collection)
			{
				if (proxyAddress.Prefix == prefix)
				{
					num++;
				}
			}
			return num;
		}

		internal static bool ExtractExtensionInformation(ProxyAddress address, out string extension, out string phoneContext, out string dialplanDisplayName)
		{
			if (address == null)
			{
				throw new ArgumentException("address");
			}
			extension = null;
			phoneContext = null;
			dialplanDisplayName = null;
			if (address.Prefix == ProxyAddressPrefix.UM)
			{
				extension = UMMailbox.GetExtensionFromProxyAddress(address);
				phoneContext = UMMailbox.GetPhoneContextFromProxyAddress(address);
				dialplanDisplayName = null;
				if (phoneContext != null)
				{
					dialplanDisplayName = phoneContext.Split(new char[]
					{
						'.'
					})[0];
				}
			}
			return extension != null && phoneContext != null && dialplanDisplayName != null;
		}

		internal static ProxyAddress BuildProxyAddressFromExtensionAndPhoneContext(string extension, string prefixString, string phoneContext)
		{
			return ProxyAddress.Parse(prefixString, UMMailbox.BuildAddressStringFromExtensionAndPhoneContext(extension, phoneContext));
		}

		private static string BuildAddressStringFromExtensionAndPhoneContext(string extension, string phoneContext)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}{1}{2}{3}", new object[]
			{
				extension,
				";",
				"phone-context=",
				phoneContext
			});
			return stringBuilder.ToString();
		}

		private static bool IsProxyAddressLocalExtension(ProxyAddress emailAddress)
		{
			string extensionFromProxyAddress = UMMailbox.GetExtensionFromProxyAddress(emailAddress);
			return extensionFromProxyAddress != null && !Regex.IsMatch(extensionFromProxyAddress, "[^0-9]");
		}

		private static string GetExtensionFromProxyAddress(ProxyAddress emailAddress)
		{
			int num = emailAddress.AddressString.IndexOf(";");
			if (-1 != num)
			{
				return emailAddress.AddressString.Substring(0, num);
			}
			return null;
		}

		private static string GetPhoneContextFromProxyAddress(ProxyAddress emailAddress)
		{
			int num = emailAddress.AddressString.IndexOf("phone-context=");
			if (-1 != num)
			{
				return emailAddress.AddressString.Substring(num + "phone-context=".Length);
			}
			return null;
		}

		internal string GetEUMPhoneNumber(UMDialPlan dialPlan)
		{
			Hashtable airSyncSafeTable = UMMailbox.GetAirSyncSafeTable(this.UMAddresses, ProxyAddressPrefix.ASUM, dialPlan);
			foreach (ProxyAddress proxyAddress in this.EmailAddresses)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.UM)
				{
					string phoneContextFromProxyAddress = UMMailbox.GetPhoneContextFromProxyAddress(proxyAddress);
					if (phoneContextFromProxyAddress == dialPlan.PhoneContext && UMMailbox.IsProxyAddressLocalExtension(proxyAddress) && !airSyncSafeTable.ContainsKey(proxyAddress.AddressString))
					{
						return UMMailbox.GetExtensionFromProxyAddress(proxyAddress);
					}
				}
			}
			return null;
		}

		internal UMDialPlan GetDialPlan()
		{
			IConfigurationSession configurationSession = this.GetConfigurationSession();
			return configurationSession.Read<UMDialPlan>(this.UMDialPlan);
		}

		internal UMMailboxPolicy GetPolicy()
		{
			IConfigurationSession configurationSession = this.GetConfigurationSession();
			return configurationSession.Read<UMMailboxPolicy>(this.UMMailboxPolicy);
		}

		internal static QueryFilter GetUMEnabledUserQueryFilter(MailboxDatabase database)
		{
			return new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Database, database.Id),
				new TextFilter(ADRecipientSchema.EmailAddresses, ProxyAddressPrefix.UM.PrimaryPrefix, MatchOptions.Prefix, MatchFlags.Default),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MailboxPlan),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.ArbitrationMailbox),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.SystemMailbox),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.SystemAttendantMailbox)
			});
		}

		private IConfigurationSession GetConfigurationSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.DataObject.OrganizationId, null, false);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 1179, "GetConfigurationSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Management\\UMMailbox.cs");
		}

		internal const string HostedVoiceMailEnabled = "ExchangeHostedVoiceMail=1";

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.RoomMailbox,
			RecipientTypeDetails.EquipmentMailbox,
			RecipientTypeDetails.LegacyMailbox,
			RecipientTypeDetails.LinkedMailbox,
			RecipientTypeDetails.UserMailbox,
			RecipientTypeDetails.TeamMailbox,
			RecipientTypeDetails.SharedMailbox
		};

		private static UMMailboxSchema schema = ObjectSchema.GetInstance<UMMailboxSchema>();

		private static IEnumerable<PropertyInfo> cloneableProps;

		private static IEnumerable<PropertyInfo> cloneableOnceProps;

		private static IEnumerable<PropertyInfo> cloneableEnabledStateProps;
	}
}
