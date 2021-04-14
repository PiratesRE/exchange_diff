using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal static class TransportConstant
	{
		internal const string DirectoryPrefix = "Microsoft.Exchange.Transport.DirectoryData.";

		public const string RecipientType = "Microsoft.Exchange.Transport.DirectoryData.RecipientType";

		public const string IsResource = "Microsoft.Exchange.Transport.DirectoryData.IsResource";

		public const string RecipientTypeDetailsRaw = "Microsoft.Exchange.Transport.DirectoryData.RecipientTypeDetailsRaw";

		public const string ExternalEmailAddress = "Microsoft.Exchange.Transport.DirectoryData.ExternalEmailAddress";

		public const string ForwardingSmtpAddress = "Microsoft.Exchange.Transport.DirectoryData.ForwardingSmtpAddress";

		public const string DeliverToMailboxAndForward = "Microsoft.Exchange.Transport.DirectoryData.DeliverToMailboxAndForward";

		public const string Database = "Microsoft.Exchange.Transport.DirectoryData.Database";

		public const string MailboxGuid = "Microsoft.Exchange.Transport.DirectoryData.ExchangeGuid";

		public const string ThrottlingPolicy = "Microsoft.Exchange.Transport.DirectoryData.ThrottlingPolicy";

		public const string RedirectHosts = "Microsoft.Exchange.Transport.DirectoryData.RedirectHosts";

		public const string JournalRecipientList = "Microsoft.Exchange.JournalRecipientList";

		public const string OriginalP1RecipientList = "Microsoft.Exchange.OriginalP1RecipientList";

		public const string EnvelopeJournalRecipientList = "Microsoft.Exchange.EnvelopeJournalRecipientList";

		public const string EnvelopeJournalRecipientType = "Microsoft.Exchange.EnvelopeJournalRecipientType";

		public const string EnvelopeJournalExpansionHistory = "Microsoft.Exchange.EnvelopeJournalExpansionHistory";

		public const string LegacyJournalReport = "Microsoft.Exchange.LegacyJournalReport";

		public const string IsRemoteRecipient = "Microsoft.Exchange.Transport.IsRemoteRecipient";

		public const string RecipientP2TypeProperty = "Microsoft.Exchange.Transport.RecipientP2Type";

		public const string ElcJournalReport = "Microsoft.Exchange.Transport.ElcJournalReport";

		public const string OpenDomainRoutingDisabled = "Microsoft.Exchange.Transport.OpenDomainRoutingDisabled";

		public const string ResentMapiMessage = "Microsoft.Exchange.Transport.ResentMapiMessage";

		public const string ResentMapiP2ToRecipients = "Microsoft.Exchange.Transport.ResentMapiP2ToRecipients";

		public const string ResentMapiP2CcRecipients = "Microsoft.Exchange.Transport.ResentMapiP2CcRecipients";

		public const string ContentIdentifierProperty = "Microsoft.Exchange.ContentIdentifier";

		public const string SmtpMuaSubmission = "Microsoft.Exchange.SmtpMuaSubmission";

		public const string TlsDomain = "Microsoft.Exchange.Transport.MailRecipient.TlsDomain";

		public const string TenantInboundConnectorId = "Microsoft.Exchange.Hygiene.TenantInboundConnectorId";

		public const string TenantOutboundConnectorId = "Microsoft.Exchange.Hygiene.TenantOutboundConnectorId";

		public const string TenantOutboundConnectorCustomData = "Microsoft.Exchange.Hygiene.TenantOutboundConnectorCustomData";

		public const string EffectiveTlsAuthLevel = "Microsoft.Exchange.Transport.MailRecipient.EffectiveTlsAuthLevel";

		public const string RequiredTlsAuthLevel = "Microsoft.Exchange.Transport.MailRecipient.RequiredTlsAuthLevel";

		public const string OutboundIPPoolLabel = "Microsoft.Exchange.Transport.MailRecipient.OutboundIPPool";

		public const string OriginatorOrganization = "Microsoft.Exchange.Transport.MailRecipient.OriginatorOrganization";

		public const string OrganizationScope = "Microsoft.Exchange.Transport.MailRecipient.OrganizationScope";

		public const string NetworkMessageId = "Microsoft.Exchange.Transport.MailRecipient.NetworkMessageId";

		public const string AddressBookPolicy = "Microsoft.Exchange.Transport.MailRecipient.AddressBookPolicy";

		public const string DisplayName = "Microsoft.Exchange.Transport.MailRecipient.DisplayName";

		public const string MailFlowPartnerInternalMailContentType = "Microsoft.Exchange.Transport.Agent.OpenDomainRouting.MailFlowPartnerSettings.InternalMailContentType";

		public const string RecipientDiagnosticInfo = "Microsoft.Exchange.Transport.RecipientDiagnosticInfo";

		public const string TransportDecrypted = "Microsoft.Exchange.RightsManagement.TransportDecrypted";

		public const string TransportDecryptionPL = "Microsoft.Exchange.RightsManagement.TransportDecryptionPL";

		public const string TransportDecryptionUL = "Microsoft.Exchange.RightsManagement.TransportDecryptionUL";

		public const string TransportDecryptionLicenseUri = "Microsoft.Exchange.RightsManagement.TransportDecryptionLicenseUri";

		public const string TransportE4ERpmsg = "Microsoft.Exchange.Encryption.TransportRpmsg";

		public const string TransportE4EDecryptionPL = "Microsoft.Exchange.Encryption.TransportDecryptionPL";

		public const string TransportE4EDecryptionUL = "Microsoft.Exchange.Encryption.TransportDecryptionUL";

		public const string TransportE4EDecryptionLicenseUri = "Microsoft.Exchange.Encryption.TransportDecryptionLicenseUri";

		public const string DecryptionTokenRecipient = "Microsoft.Exchange.RightsManagement.DecryptionTokenRecipient";

		public const string DecryptionE4ETokenRecipient = "Microsoft.Exchange.Encryption.DecryptionTokenRecipient";

		public const string RecipientEUL = "Microsoft.Exchange.RightsManagement.DRMLicense";

		public const string RecipientDrmRights = "Microsoft.Exchange.RightsManagement.DRMRights";

		public const string RecipientDrmExpiryTime = "Microsoft.Exchange.RightsManagement.DRMExpiryTime";

		public const string RecipientDrmPropsSignature = "Microsoft.Exchange.RightsManagement.DRMPropsSignature";

		public const string RecipientDrmFailure = "Microsoft.Exchange.RightsManagement.DRMFailure";

		public const string RecipientB2BEul = "Microsoft.Exchange.RightsManagement.B2BDRMLicense";

		internal const string SystemMessageContentIdentifier = "ExSysMessage";

		internal const string VoicemailMessageClass = "IPM.Note.Microsoft.Voicemail.UM";

		internal const string VoicemailMessageClassCA = "IPM.Note.Microsoft.Voicemail.UM.CA";

		internal const string MissedCallMessageClass = "IPM.Note.Microsoft.Missed.Voice";

		public const string QuarantineMessageMarkerHeader = "X-MS-Exchange-Organization-Quarantine";

		public const string SmtpServicePrincipalName = "SmtpSvc";

		public const string AttachmentRemovedLabel = "a4bb0cb2-4395-4d18-9799-1f904b20fe92";

		public const string MExConfigFilePath = "TransportRoles\\Shared\\agents.config";

		public const string MExFrontEndConfigFilePath = "TransportRoles\\Shared\\fetagents.config";

		public const string ModerationOriginalMessageFileName = "OriginalMessage";

		public const string ModerationReplayXHeaderFileName = "ReplayXHeaders";

		public const string ModerationFireWalledHeadersFileName = "FireWalledHeaders";

		public const string InboundTrustEnabled = "Microsoft.Exchange.Transport.InboundTrustEnabled";

		public const string MailboxRuleDelegateAccessRuleName = ".DelegateAccess";

		public const string GeneratedByMailboxRule = "Microsoft.Exchange.Transport.GeneratedByMailboxRule";

		public const string AddedByTransportRule = "Microsoft.Exchange.Transport.AddedByTransportRule";

		public const string ModeratedByTransportRule = "Microsoft.Exchange.Transport.ModeratedByTransportRule";

		public const string PFNoReplicaRoutingOverride = "Microsoft.Exchange.Transport.RoutingOverride";

		public const string SmtpInSessionId = "Microsoft.Exchange.Transport.SmtpInSessionId";

		public const string NextHopFqdn = "Microsoft.Exchange.Transport.Proxy.NextHopFqdn";

		public const string IsNextHopInternal = "Microsoft.Exchange.Transport.Proxy.IsNextHopInternal";

		public const string RetryOnDuplicateDelivery = "Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery ";

		public const string InternalMessageId = "Microsoft.Exchange.Transport.MailboxTransport.InternalMessageId";

		public const string MailboxTransportSmtpInClientHostname = "Microsoft.Exchange.Transport.MailboxTransport.SmtpInClientHostname";

		public const string DeliveryQueueMailboxSubComponent = "Microsoft.Exchange.Transport.DeliveryQueueMailboxSubComponent";

		public const string DeliveryQueueMailboxSubComponentLatency = "Microsoft.Exchange.Transport.DeliveryQueueMailboxSubComponentLatency";

		public const string SmtpReceiveAgentsDiagnosticsComponentName = "SmtpReceiveAgents";

		public const string RoutingAgentsDiagnosticsComponentName = "RoutingAgents";

		public const string DeliveryAgentsDiagnosticsComponentName = "DeliveryAgents";

		public const string StorageAgentsDiagnosticsComponentName = "StorageAgents";

		public const string SmtpReceiveAgentsMExRuntimeAgentType = "SmtpReceiveAgent";

		public const string RoutingAgentsMExRuntimeAgentType = "RoutingAgent";

		public const string DeliveryAgentsMExRuntimeAgentType = "DeliveryAgent";

		public const string StorageAgentsMExRuntimeAgentType = "StorageAgent";

		public const string CategorizerDiagnosticsComponentName = "Categorizer";

		public const string DeliveryDiagnosticsComponentName = "RemoteDelivery";

		public const string ShadowRedundancyDiagnosticsComponentName = "ShadowRedundancy";

		public const string PreserveCrossPremisesHeaders = "PreserveCrossPremisesHeaders";

		public const string CrossPremisesHeadersBlockedHeader = "X-MS-Exchange-Organization-Cross-Premises-Headers-Blocked";

		public const string CrossPremisesHeadersPromotedHeader = "X-MS-Exchange-Organization-Cross-Premises-Headers-Promoted";

		public const string ConnectorRedirectPropertyName = "ConnectorRedirect";

		public const string IsProbePropertyName = "IsProbe";

		public const string ProbeTypePropertyName = "ProbeType";

		public const string PersistProbeTracePropertyName = "PersistProbeTrace";

		public const string ConsolidateAdvancedRoutingAppConfigKey = "ConsolidateAdvancedRouting";

		internal const string VoltageEncryptedHeaderName = "X-Voltage-Encrypted";

		internal const string VoltageEncryptedHeaderValue = "Encrypted";

		internal const string VoltageDecryptedHeaderName = "X-Voltage-Decrypted";

		internal const string VoltageDecryptedHeaderValue = "Decrypted";

		internal const string HeaderNameForVoltageEncryptAction = "X-Voltage-Encrypt";

		internal const string HeaderValueForVoltageEncryptAction = "Encrypt";

		internal const string HeaderNameForVoltageDecryptAction = "X-Voltage-Decrypt";

		internal const string HeaderValueForVoltageDecryptAction = "Decrypt";

		internal class ComponentCost
		{
			internal const string PropertyName = "CompCost";

			internal const string TransportRules = "ETR";

			internal const string Antimalware = "AMA";

			internal const string SpamFilter = "SFA";
		}
	}
}
