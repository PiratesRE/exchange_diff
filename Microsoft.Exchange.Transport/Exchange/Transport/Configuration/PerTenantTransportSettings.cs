using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal sealed class PerTenantTransportSettings : TenantConfigurationCacheableItem<TransportConfigContainer>, ITransportSettingsFacade
	{
		public PerTenantTransportSettings()
		{
		}

		public PerTenantTransportSettings(TransportConfigContainer transportConfigContainer) : base(true)
		{
			this.SetInternalData(transportConfigContainer);
		}

		public MultiValuedProperty<SmtpDomain> TLSReceiveDomainSecureList
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return Components.Configuration.TransportSettings.TransportSettings.TLSReceiveDomainSecureList;
			}
		}

		public MultiValuedProperty<SmtpDomain> TLSSendDomainSecureList
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return Components.Configuration.TransportSettings.TransportSettings.TLSSendDomainSecureList;
			}
		}

		public MultiValuedProperty<EnhancedStatusCode> GenerateCopyOfDSNFor
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return Components.Configuration.TransportSettings.TransportSettings.GenerateCopyOfDSNFor;
			}
		}

		public MultiValuedProperty<IPRange> InternalSMTPServers
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return Components.Configuration.TransportSettings.TransportSettings.InternalSMTPServers;
			}
		}

		public SmtpAddress JournalingReportNdrTo
		{
			get
			{
				return this.journalingReportNdrTo;
			}
		}

		public string OrganizationFederatedMailbox
		{
			get
			{
				return this.organizationFederatedMailbox.ToString();
			}
		}

		public ByteQuantifiedSize MaxDumpsterSizePerDatabase
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return Components.Configuration.TransportSettings.TransportSettings.MaxDumpsterSizePerDatabase;
			}
		}

		public EnhancedTimeSpan MaxDumpsterTime
		{
			get
			{
				return Components.Configuration.TransportSettings.TransportSettings.MaxDumpsterTime;
			}
		}

		public bool VerifySecureSubmitEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return Components.Configuration.TransportSettings.TransportSettings.VerifySecureSubmitEnabled;
			}
		}

		public bool ClearCategories
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.clearCategories;
			}
		}

		public bool OpenDomainRoutingEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.openDomainRoutingEnabled;
			}
		}

		public bool AddressBookPolicyRoutingEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return Components.Configuration.TransportSettings.TransportSettings.AddressBookPolicyRoutingEnabled && this.addressBookPolicyRoutingEnabled;
			}
		}

		public bool VoicemailJournalingEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return Components.Configuration.TransportSettings.TransportSettings.VoicemailJournalingEnabled;
			}
		}

		public bool Xexch50Enabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return Components.Configuration.TransportSettings.TransportSettings.Xexch50Enabled;
			}
		}

		public bool Rfc2231EncodingEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.rfc2231EncodingEnabled;
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxReceiveSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.maxReceiveSize;
			}
		}

		public Unlimited<int> MaxRecipientEnvelopeLimit
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return Components.Configuration.TransportSettings.TransportSettings.MaxRecipientEnvelopeLimit;
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxSendSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.maxSendSize;
			}
		}

		public bool ExternalDelayDsnEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.externalDelayDsnEnabled;
			}
		}

		public CultureInfo ExternalDsnDefaultLanguage
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.externalDsnDefaultLanguage;
			}
		}

		public bool ExternalDsnLanguageDetectionEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.externalDsnLanguageDetectionEnabled;
			}
		}

		public ByteQuantifiedSize ExternalDsnMaxMessageAttachSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.externalDsnMaxMessageAttachSize;
			}
		}

		public SmtpDomain ExternalDsnReportingAuthority
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.externalDsnReportingAuthority;
			}
		}

		public bool ExternalDsnSendHtml
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.externalDsnSendHtml;
			}
		}

		public SmtpAddress? ExternalPostmasterAddress
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.externalPostmasterAddress;
			}
		}

		public bool InternalDelayDsnEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.internalDelayDsnEnabled;
			}
		}

		public CultureInfo InternalDsnDefaultLanguage
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.internalDsnDefaultLanguage;
			}
		}

		public bool InternalDsnLanguageDetectionEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.internalDsnLanguageDetectionEnabled;
			}
		}

		public ByteQuantifiedSize InternalDsnMaxMessageAttachSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.internalDsnMaxMessageAttachSize;
			}
		}

		public SmtpDomain InternalDsnReportingAuthority
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.internalDsnReportingAuthority;
			}
		}

		public bool InternalDsnSendHtml
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.internalDsnSendHtml;
			}
		}

		public IList<string> SupervisionTags
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.supervisionTags;
			}
		}

		public override long ItemSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return (long)this.estimatedSize;
			}
		}

		public string HygieneSuite
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.hygieneSuite;
			}
		}

		public bool ConvertReportToMessage
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.convertReportToMessage;
			}
		}

		public bool PreserveReportBodypart
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.preserveReportBodypart;
			}
		}

		public bool MigrationEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.migrationEnabled;
			}
		}

		public bool LegacyJournalingMigrationEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.legacyJournalingMigrationEnabled;
			}
		}

		public bool LegacyArchiveJournalingEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.legacyArchiveJournalingEnabled;
			}
		}

		public bool LegacyArchiveLiveJournalingEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.legacyArchiveLiveJournalingEnabled;
			}
		}

		public bool DropUnprovisionedUserMessagesForLegacyArchiveJournaling
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.dropUnprovisionedUserMessagesForLegacyArchiveJournaling;
			}
		}

		public bool RedirectDLMessagesForLegacyArchiveJournaling
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.redirectDLMessagesForLegacyArchiveJournaling;
			}
		}

		public bool JournalArchivingEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.journalArchivingEnabled;
			}
		}

		public int TransportRuleAttachmentTextScanLimit
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.transportRuleAttachmentTextScanLimit;
			}
		}

		public Version TransportRuleMinProductVersion
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.transportRuleMinProductVersion;
			}
		}

		public override void ReadData(IConfigurationSession session)
		{
			TransportConfigContainer[] array = session.Find<TransportConfigContainer>(null, QueryScope.SubTree, null, null, 2);
			if (array == null || array.Length == 0)
			{
				ExTraceGlobals.ConfigurationTracer.TraceError<string>((long)this.GetHashCode(), "Could not find transport settings for {0}", PerTenantTransportSettings.GetOrgIdString(session));
				throw new TenantTransportSettingsNotFoundException(PerTenantTransportSettings.GetOrgIdString(session));
			}
			if (array.Length > 1)
			{
				ExTraceGlobals.ConfigurationTracer.TraceError<string>((long)this.GetHashCode(), "Found more than one transport settings for {0}", PerTenantTransportSettings.GetOrgIdString(session));
				throw new TransportSettingsAmbiguousException(PerTenantTransportSettings.GetOrgIdString(session));
			}
			this.SetInternalData(array[0]);
		}

		private static string GetOrgIdString(IConfigurationSession session)
		{
			if (!(session.SessionSettings.CurrentOrganizationId != null))
			{
				return "<First Organization>";
			}
			return session.SessionSettings.CurrentOrganizationId.ToString();
		}

		private void SetInternalData(TransportConfigContainer transportConfigContainer)
		{
			this.clearCategories = transportConfigContainer.ClearCategories;
			this.rfc2231EncodingEnabled = transportConfigContainer.Rfc2231EncodingEnabled;
			this.openDomainRoutingEnabled = transportConfigContainer.OpenDomainRoutingEnabled;
			this.addressBookPolicyRoutingEnabled = transportConfigContainer.AddressBookPolicyRoutingEnabled;
			this.externalDelayDsnEnabled = transportConfigContainer.ExternalDelayDsnEnabled;
			this.externalDsnDefaultLanguage = transportConfigContainer.ExternalDsnDefaultLanguage;
			this.externalDsnLanguageDetectionEnabled = transportConfigContainer.ExternalDsnLanguageDetectionEnabled;
			this.externalDsnMaxMessageAttachSize = transportConfigContainer.ExternalDsnMaxMessageAttachSize;
			this.externalDsnReportingAuthority = transportConfigContainer.ExternalDsnReportingAuthority;
			this.externalDsnSendHtml = transportConfigContainer.ExternalDsnSendHtml;
			this.externalPostmasterAddress = transportConfigContainer.ExternalPostmasterAddress;
			this.internalDelayDsnEnabled = transportConfigContainer.InternalDelayDsnEnabled;
			this.internalDsnDefaultLanguage = transportConfigContainer.InternalDsnDefaultLanguage;
			this.internalDsnLanguageDetectionEnabled = transportConfigContainer.InternalDsnLanguageDetectionEnabled;
			this.internalDsnMaxMessageAttachSize = transportConfigContainer.InternalDsnMaxMessageAttachSize;
			this.internalDsnReportingAuthority = transportConfigContainer.InternalDsnReportingAuthority;
			this.internalDsnSendHtml = transportConfigContainer.InternalDsnSendHtml;
			this.journalingReportNdrTo = transportConfigContainer.JournalingReportNdrTo;
			this.organizationFederatedMailbox = transportConfigContainer.OrganizationFederatedMailbox;
			this.migrationEnabled = transportConfigContainer.MigrationEnabled;
			this.convertReportToMessage = transportConfigContainer.ConvertReportToMessage;
			this.preserveReportBodypart = transportConfigContainer.PreserveReportBodypart;
			this.legacyJournalingMigrationEnabled = transportConfigContainer.LegacyJournalingMigrationEnabled;
			this.legacyArchiveJournalingEnabled = transportConfigContainer.LegacyArchiveJournalingEnabled;
			this.legacyArchiveLiveJournalingEnabled = transportConfigContainer.LegacyArchiveLiveJournalingEnabled;
			this.dropUnprovisionedUserMessagesForLegacyArchiveJournaling = !transportConfigContainer.RedirectUnprovisionedUserMessagesForLegacyArchiveJournaling;
			this.redirectDLMessagesForLegacyArchiveJournaling = transportConfigContainer.RedirectDLMessagesForLegacyArchiveJournaling;
			this.journalArchivingEnabled = transportConfigContainer.JournalArchivingEnabled;
			this.transportRuleAttachmentTextScanLimit = (int)transportConfigContainer.TransportRuleAttachmentTextScanLimit.ToBytes();
			this.transportRuleMinProductVersion = transportConfigContainer.TransportRuleMinProductVersion;
			this.maxReceiveSize = transportConfigContainer.MaxReceiveSize;
			this.maxSendSize = transportConfigContainer.MaxSendSize;
			switch (transportConfigContainer.HygieneSuite)
			{
			case HygieneSuiteEnum.Premium:
				this.hygieneSuite = "Premium";
				goto IL_1BE;
			}
			this.hygieneSuite = "Standard";
			IL_1BE:
			this.supervisionTags = transportConfigContainer.SupervisionTags;
			int num = (this.internalDsnReportingAuthority != null) ? this.internalDsnReportingAuthority.Domain.Length : 0;
			int num2 = (this.externalDsnReportingAuthority != null) ? this.externalDsnReportingAuthority.Domain.Length : 0;
			int num3 = (this.externalPostmasterAddress != null) ? this.externalPostmasterAddress.Value.Length : 0;
			int num4 = this.journalingReportNdrTo.Length * 2 + 18;
			int num5 = this.organizationFederatedMailbox.Length * 2 + 18;
			int num6 = 0;
			if (this.supervisionTags != null)
			{
				foreach (string text in this.supervisionTags)
				{
					num6 += text.Length;
				}
				num6 *= 2;
			}
			this.estimatedSize = 27 + num + num2 + num3 + num4 + num5 + num6 + 16;
		}

		private const int CultureInfoReferenceSize = 8;

		private const string PremiumHygieneSuite = "Premium";

		private const string StandardHygieneSuite = "Standard";

		private int estimatedSize;

		private bool rfc2231EncodingEnabled;

		private bool clearCategories;

		private bool openDomainRoutingEnabled;

		private bool addressBookPolicyRoutingEnabled;

		private bool externalDelayDsnEnabled;

		private CultureInfo externalDsnDefaultLanguage;

		private bool externalDsnLanguageDetectionEnabled;

		private ByteQuantifiedSize externalDsnMaxMessageAttachSize;

		private SmtpDomain externalDsnReportingAuthority;

		private bool externalDsnSendHtml;

		private SmtpAddress? externalPostmasterAddress;

		private bool internalDelayDsnEnabled;

		private CultureInfo internalDsnDefaultLanguage;

		private bool internalDsnLanguageDetectionEnabled;

		private ByteQuantifiedSize internalDsnMaxMessageAttachSize;

		private SmtpDomain internalDsnReportingAuthority;

		private bool internalDsnSendHtml;

		private SmtpAddress journalingReportNdrTo;

		private SmtpAddress organizationFederatedMailbox;

		private IList<string> supervisionTags;

		private string hygieneSuite;

		private bool convertReportToMessage;

		private bool preserveReportBodypart;

		private bool migrationEnabled;

		private bool legacyJournalingMigrationEnabled;

		private bool legacyArchiveJournalingEnabled;

		private bool redirectDLMessagesForLegacyArchiveJournaling;

		private bool dropUnprovisionedUserMessagesForLegacyArchiveJournaling;

		private bool journalArchivingEnabled;

		private bool legacyArchiveLiveJournalingEnabled;

		private int transportRuleAttachmentTextScanLimit;

		private Version transportRuleMinProductVersion;

		private Unlimited<ByteQuantifiedSize> maxReceiveSize;

		private Unlimited<ByteQuantifiedSize> maxSendSize;
	}
}
