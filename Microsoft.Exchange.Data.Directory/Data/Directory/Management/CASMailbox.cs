using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class CASMailbox : ADPresentationObject
	{
		internal static bool IsMailboxProtocolLoggingEnabled(ADObject adObject, ADPropertyDefinition property)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			int? num = (int?)adObject[property];
			if (num == null)
			{
				return false;
			}
			double num2 = (ExDateTime.UtcNow - CASMailbox.MailboxProtocolLoggingInitialTime).TotalMinutes - (double)num.Value;
			return num2 >= 0.0 && num2 < (double)CASMailbox.MailboxProtocolLoggingLength;
		}

		internal static void SetMailboxProtocolLoggingEnabled(ADObject adObject, ADPropertyDefinition property, bool value)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (!value)
			{
				adObject[property] = null;
				return;
			}
			double totalMinutes = (ExDateTime.UtcNow - CASMailbox.MailboxProtocolLoggingInitialTime).TotalMinutes;
			if (totalMinutes < -2147483648.0 || 2147483647.0 < totalMinutes)
			{
				adObject[property] = null;
				return;
			}
			adObject[property] = (int)totalMinutes;
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return CASMailbox.schema;
			}
		}

		public CASMailbox()
		{
		}

		public CASMailbox(ADUser dataObject) : base(dataObject)
		{
		}

		internal static CASMailbox FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new CASMailbox(dataObject);
		}

		protected override IEnumerable<PropertyInfo> CloneableProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = CASMailbox.cloneableProps) == null)
				{
					result = (CASMailbox.cloneableProps = ADPresentationObject.GetCloneableProperties(this));
				}
				return result;
			}
		}

		protected override IEnumerable<PropertyInfo> CloneableOnceProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = CASMailbox.cloneableOnceProps) == null)
				{
					result = (CASMailbox.cloneableOnceProps = ADPresentationObject.GetCloneableOnceProperties(this));
				}
				return result;
			}
		}

		protected override IEnumerable<PropertyInfo> CloneableEnabledStateProperties
		{
			get
			{
				IEnumerable<PropertyInfo> result;
				if ((result = CASMailbox.cloneableEnabledStateProps) == null)
				{
					result = (CASMailbox.cloneableEnabledStateProps = ADPresentationObject.GetCloneableEnabledStateProperties(this));
				}
				return result;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[CASMailboxSchema.EmailAddresses];
			}
			set
			{
				this[CASMailboxSchema.EmailAddresses] = value;
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[CASMailboxSchema.LegacyExchangeDN];
			}
		}

		public string LinkedMasterAccount
		{
			get
			{
				return (string)this[CASMailboxSchema.LinkedMasterAccount];
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[CASMailboxSchema.PrimarySmtpAddress];
			}
			set
			{
				this[CASMailboxSchema.PrimarySmtpAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SamAccountName
		{
			get
			{
				return (string)this[CASMailboxSchema.SamAccountName];
			}
			set
			{
				this[CASMailboxSchema.SamAccountName] = value;
			}
		}

		public string ServerLegacyDN
		{
			get
			{
				return (string)this[CASMailboxSchema.ServerLegacyDN];
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[CASMailboxSchema.ServerName];
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)this[CASMailboxSchema.DisplayName];
			}
			set
			{
				this[CASMailboxSchema.DisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ActiveSyncAllowedDeviceIDs
		{
			get
			{
				return (MultiValuedProperty<string>)this[CASMailboxSchema.ActiveSyncAllowedDeviceIDs];
			}
			set
			{
				this[CASMailboxSchema.ActiveSyncAllowedDeviceIDs] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ActiveSyncBlockedDeviceIDs
		{
			get
			{
				return (MultiValuedProperty<string>)this[CASMailboxSchema.ActiveSyncBlockedDeviceIDs];
			}
			set
			{
				this[CASMailboxSchema.ActiveSyncBlockedDeviceIDs] = value;
			}
		}

		public ADObjectId ActiveSyncMailboxPolicy
		{
			get
			{
				if (this[CASMailboxSchema.ActiveSyncMailboxPolicy] != null)
				{
					return (ADObjectId)this[CASMailboxSchema.ActiveSyncMailboxPolicy];
				}
				return this.activeSyncMailboxPolicy;
			}
			set
			{
				this[CASMailboxSchema.ActiveSyncMailboxPolicy] = value;
			}
		}

		internal void SetActiveSyncMailboxPolicyLocally(ADObjectId activeSyncMailboxPolicy)
		{
			this.activeSyncMailboxPolicy = activeSyncMailboxPolicy;
		}

		public bool ActiveSyncMailboxPolicyIsDefaulted
		{
			get
			{
				return (bool)(this[CASMailboxSchema.ActiveSyncMailboxPolicyIsDefaulted] ?? false);
			}
			internal set
			{
				this[CASMailboxSchema.ActiveSyncMailboxPolicyIsDefaulted] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ActiveSyncDebugLogging
		{
			get
			{
				return this.activeSyncDebugLogging ?? false;
			}
			set
			{
				this.activeSyncDebugLogging = new bool?(value);
			}
		}

		internal bool ActiveSyncDebugLoggingSpecified
		{
			get
			{
				return this.activeSyncDebugLogging != null;
			}
		}

		[Parameter]
		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		public bool ActiveSyncEnabled
		{
			get
			{
				return (bool)this[CASMailboxSchema.ActiveSyncEnabled];
			}
			set
			{
				this[CASMailboxSchema.ActiveSyncEnabled] = value;
			}
		}

		public bool HasActiveSyncDevicePartnership
		{
			get
			{
				return (bool)this[CASMailboxSchema.HasActiveSyncDevicePartnership];
			}
		}

		public string ExternalImapSettings { get; internal set; }

		public string InternalImapSettings { get; internal set; }

		public string ExternalPopSettings { get; internal set; }

		public string InternalPopSettings { get; internal set; }

		public string ExternalSmtpSettings { get; internal set; }

		public string InternalSmtpSettings { get; internal set; }

		[ProvisionalCloneOnce(CloneSet.CloneExtendedSet)]
		public ADObjectId OwaMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[CASMailboxSchema.OwaMailboxPolicy];
			}
			set
			{
				this[CASMailboxSchema.OwaMailboxPolicy] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		[Parameter]
		public bool OWAEnabled
		{
			get
			{
				return (bool)this[CASMailboxSchema.OWAEnabled];
			}
			set
			{
				this[CASMailboxSchema.OWAEnabled] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		[Parameter(Mandatory = false)]
		public bool OWAforDevicesEnabled
		{
			get
			{
				return (bool)this[CASMailboxSchema.OWAforDevicesEnabled];
			}
			set
			{
				this[CASMailboxSchema.OWAforDevicesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		public bool ECPEnabled
		{
			get
			{
				return (bool)this[CASMailboxSchema.ECPEnabled];
			}
			set
			{
				this[CASMailboxSchema.ECPEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		public bool PopEnabled
		{
			get
			{
				return (bool)this[CASMailboxSchema.PopEnabled];
			}
			set
			{
				this[CASMailboxSchema.PopEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PopUseProtocolDefaults
		{
			get
			{
				return (bool)this[CASMailboxSchema.PopUseProtocolDefaults];
			}
			set
			{
				this[CASMailboxSchema.PopUseProtocolDefaults] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MimeTextFormat PopMessagesRetrievalMimeFormat
		{
			get
			{
				return (MimeTextFormat)this[CASMailboxSchema.PopMessagesRetrievalMimeFormat];
			}
			set
			{
				this[CASMailboxSchema.PopMessagesRetrievalMimeFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PopEnableExactRFC822Size
		{
			get
			{
				return (bool)this[CASMailboxSchema.PopEnableExactRFC822Size];
			}
			set
			{
				this[CASMailboxSchema.PopEnableExactRFC822Size] = value;
			}
		}

		internal bool PopProtocolLoggingEnabled
		{
			get
			{
				return CASMailbox.IsMailboxProtocolLoggingEnabled(this, CASMailboxSchema.PopProtocolLoggingEnabled);
			}
			set
			{
				CASMailbox.SetMailboxProtocolLoggingEnabled(this, CASMailboxSchema.PopProtocolLoggingEnabled, value);
			}
		}

		[Parameter(Mandatory = false)]
		public bool PopSuppressReadReceipt
		{
			get
			{
				return (bool)this[CASMailboxSchema.PopSuppressReadReceipt];
			}
			set
			{
				this[CASMailboxSchema.PopSuppressReadReceipt] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PopForceICalForCalendarRetrievalOption
		{
			get
			{
				return (bool)this[CASMailboxSchema.PopForceICalForCalendarRetrievalOption];
			}
			set
			{
				this[CASMailboxSchema.PopForceICalForCalendarRetrievalOption] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		[Parameter(Mandatory = false)]
		public bool ImapEnabled
		{
			get
			{
				return (bool)this[CASMailboxSchema.ImapEnabled];
			}
			set
			{
				this[CASMailboxSchema.ImapEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImapUseProtocolDefaults
		{
			get
			{
				return (bool)this[CASMailboxSchema.ImapUseProtocolDefaults];
			}
			set
			{
				this[CASMailboxSchema.ImapUseProtocolDefaults] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MimeTextFormat ImapMessagesRetrievalMimeFormat
		{
			get
			{
				return (MimeTextFormat)this[CASMailboxSchema.ImapMessagesRetrievalMimeFormat];
			}
			set
			{
				this[CASMailboxSchema.ImapMessagesRetrievalMimeFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImapEnableExactRFC822Size
		{
			get
			{
				return (bool)this[CASMailboxSchema.ImapEnableExactRFC822Size];
			}
			set
			{
				this[CASMailboxSchema.ImapEnableExactRFC822Size] = value;
			}
		}

		internal bool ImapProtocolLoggingEnabled
		{
			get
			{
				return CASMailbox.IsMailboxProtocolLoggingEnabled(this, CASMailboxSchema.ImapProtocolLoggingEnabled);
			}
			set
			{
				CASMailbox.SetMailboxProtocolLoggingEnabled(this, CASMailboxSchema.ImapProtocolLoggingEnabled, value);
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImapSuppressReadReceipt
		{
			get
			{
				return (bool)this[CASMailboxSchema.ImapSuppressReadReceipt];
			}
			set
			{
				this[CASMailboxSchema.ImapSuppressReadReceipt] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImapForceICalForCalendarRetrievalOption
		{
			get
			{
				return (bool)this[CASMailboxSchema.ImapForceICalForCalendarRetrievalOption];
			}
			set
			{
				this[CASMailboxSchema.ImapForceICalForCalendarRetrievalOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		public bool MAPIEnabled
		{
			get
			{
				return (bool)this[CASMailboxSchema.MAPIEnabled];
			}
			set
			{
				this[CASMailboxSchema.MAPIEnabled] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		[Parameter(Mandatory = false)]
		public bool? MapiHttpEnabled
		{
			get
			{
				return (bool?)this[CASMailboxSchema.MapiHttpEnabled];
			}
			set
			{
				this[CASMailboxSchema.MapiHttpEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		public bool MAPIBlockOutlookNonCachedMode
		{
			get
			{
				return (bool)this[CASMailboxSchema.MAPIBlockOutlookNonCachedMode];
			}
			set
			{
				this[CASMailboxSchema.MAPIBlockOutlookNonCachedMode] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		[Parameter(Mandatory = false)]
		public string MAPIBlockOutlookVersions
		{
			get
			{
				return (string)this[CASMailboxSchema.MAPIBlockOutlookVersions];
			}
			set
			{
				this[CASMailboxSchema.MAPIBlockOutlookVersions] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		public bool MAPIBlockOutlookRpcHttp
		{
			get
			{
				return (bool)this[CASMailboxSchema.MAPIBlockOutlookRpcHttp];
			}
			set
			{
				this[CASMailboxSchema.MAPIBlockOutlookRpcHttp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ProvisionalClone(CloneSet.CloneExtendedSet)]
		public bool MAPIBlockOutlookExternalConnectivity
		{
			get
			{
				return (bool)this[CASMailboxSchema.MAPIBlockOutlookExternalConnectivity];
			}
			set
			{
				this[CASMailboxSchema.MAPIBlockOutlookExternalConnectivity] = value;
			}
		}

		[ProvisionalClone(CloneSet.CloneLimitedSet)]
		[Parameter(Mandatory = false)]
		public bool? EwsEnabled
		{
			get
			{
				return CASMailboxHelper.ToBooleanNullable((int?)this[CASMailboxSchema.EwsEnabled]);
			}
			set
			{
				this[CASMailboxSchema.EwsEnabled] = CASMailboxHelper.ToInt32Nullable(value);
			}
		}

		[Parameter(Mandatory = false)]
		public bool? EwsAllowOutlook
		{
			get
			{
				return (bool?)this[CASMailboxSchema.EwsAllowOutlook];
			}
			set
			{
				this[CASMailboxSchema.EwsAllowOutlook] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? EwsAllowMacOutlook
		{
			get
			{
				return (bool?)this[CASMailboxSchema.EwsAllowMacOutlook];
			}
			set
			{
				this[CASMailboxSchema.EwsAllowMacOutlook] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? EwsAllowEntourage
		{
			get
			{
				return (bool?)this[CASMailboxSchema.EwsAllowEntourage];
			}
			set
			{
				this[CASMailboxSchema.EwsAllowEntourage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
		{
			get
			{
				return (EwsApplicationAccessPolicy?)this[CASMailboxSchema.EwsApplicationAccessPolicy];
			}
			set
			{
				this[CASMailboxSchema.EwsApplicationAccessPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> EwsAllowList
		{
			get
			{
				if ((EwsApplicationAccessPolicy?)this[CASMailboxSchema.EwsApplicationAccessPolicy] == Microsoft.Exchange.Data.Directory.EwsApplicationAccessPolicy.EnforceAllowList)
				{
					return (MultiValuedProperty<string>)this[CASMailboxSchema.EwsExceptions];
				}
				return null;
			}
			set
			{
				this[CASMailboxSchema.EwsExceptions] = value;
				this.ewsAllowListSpecified = true;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> EwsBlockList
		{
			get
			{
				if ((EwsApplicationAccessPolicy?)this[CASMailboxSchema.EwsApplicationAccessPolicy] == Microsoft.Exchange.Data.Directory.EwsApplicationAccessPolicy.EnforceBlockList)
				{
					return (MultiValuedProperty<string>)this[CASMailboxSchema.EwsExceptions];
				}
				return null;
			}
			set
			{
				this[CASMailboxSchema.EwsExceptions] = value;
				this.ewsBlockListSpecified = true;
			}
		}

		internal MultiValuedProperty<string> EwsExceptions
		{
			set
			{
				this[CASMailboxSchema.EwsExceptions] = value;
			}
		}

		internal bool EwsAllowListSpecified
		{
			get
			{
				return this.ewsAllowListSpecified;
			}
		}

		internal bool EwsBlockListSpecified
		{
			get
			{
				return this.ewsBlockListSpecified;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ShowGalAsDefaultView
		{
			get
			{
				return Convert.ToBoolean(this[CASMailboxSchema.AddressBookFlags]);
			}
			set
			{
				this[CASMailboxSchema.AddressBookFlags] = Convert.ToInt32(value);
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static IEnumerable<PropertyInfo> cloneableProps;

		private static IEnumerable<PropertyInfo> cloneableOnceProps;

		private static IEnumerable<PropertyInfo> cloneableEnabledStateProps;

		private static CASMailboxSchema schema = ObjectSchema.GetInstance<CASMailboxSchema>();

		public static ExDateTime MailboxProtocolLoggingInitialTime = new ExDateTime(ExTimeZone.UtcTimeZone, 2009, 1, 1);

		public static int MailboxProtocolLoggingLength = 4320;

		private bool ewsAllowListSpecified;

		private bool ewsBlockListSpecified;

		private ADObjectId activeSyncMailboxPolicy;

		private bool? activeSyncDebugLogging = null;
	}
}
