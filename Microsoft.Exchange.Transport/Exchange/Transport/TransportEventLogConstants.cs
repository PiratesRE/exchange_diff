using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class TransportEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BindingIPv6ButDisabled = new ExEventLog.EventTuple(3221488616U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AddressInUse = new ExEventLog.EventTuple(3221488617U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfiguredConnectors = new ExEventLog.EventTuple(1074004970U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveMaxConnectionReached = new ExEventLog.EventTuple(2147746801U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveMessageRejected = new ExEventLog.EventTuple(1074004978U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveAuthenticationFailedTooManyErrors = new ExEventLog.EventTuple(2147746804U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveRejectDueToStorageError = new ExEventLog.EventTuple(3221488629U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveAuthorizationSubmitRejected = new ExEventLog.EventTuple(2147746810U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveAuthorizationRejected = new ExEventLog.EventTuple(2147746812U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveMaxConnectionPerSourceReached = new ExEventLog.EventTuple(2147746813U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_InternalSMTPServerListEmpty = new ExEventLog.EventTuple(2147746814U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveSubmitDenied = new ExEventLog.EventTuple(1074004991U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveSendAsDeniedTempAuthFailure = new ExEventLog.EventTuple(1074004992U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveSendAsDeniedSenderAddressDataInvalid = new ExEventLog.EventTuple(3221488641U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveSendAsDenied = new ExEventLog.EventTuple(1074004994U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveSendOnBehalfOfDeniedTempAuthFailure = new ExEventLog.EventTuple(1074004995U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveSendOnBehalfOfDeniedFromAddressDataInvalid = new ExEventLog.EventTuple(3221488644U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveSendOnBehalfOfDenied = new ExEventLog.EventTuple(1074004997U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveCouldNotDetermineUserNameOrSid = new ExEventLog.EventTuple(3221488646U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveMessageRateLimitExceeded = new ExEventLog.EventTuple(1074004999U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveTLSRequiredFailed = new ExEventLog.EventTuple(3221488648U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveCatchAll = new ExEventLog.EventTuple(3221488649U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveAuthenticationInitializationFailed = new ExEventLog.EventTuple(3221488650U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveAuthenticationFailed = new ExEventLog.EventTuple(2147746827U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveDirectTrustFailed = new ExEventLog.EventTuple(3221488652U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveActiveManagerFailure = new ExEventLog.EventTuple(3221488653U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveAvailabilityCounterFailure = new ExEventLog.EventTuple(3221488654U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveProhibitSendQuotaDeniedTempAuthFailure = new ExEventLog.EventTuple(1074005007U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveConnectorAvailabilityLow = new ExEventLog.EventTuple(3221488656U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveConnectorAvailabilityNormal = new ExEventLog.EventTuple(1074005009U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveProxyMserveLookupFailed = new ExEventLog.EventTuple(3221488658U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveProxyInvalidPartnerId = new ExEventLog.EventTuple(3221488659U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveProxyDnsLookupFailed = new ExEventLog.EventTuple(3221488660U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveProxyCatchAll = new ExEventLog.EventTuple(3221488661U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveProxyCounterFailure = new ExEventLog.EventTuple(3221488663U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveTooManyProxySessionFailures = new ExEventLog.EventTuple(3221488664U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveNoDestinationToProxyTo = new ExEventLog.EventTuple(3221488665U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpReceiveProcessingBlobFailed = new ExEventLog.EventTuple(3221488666U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendDnsConnectionFailure = new ExEventLog.EventTuple(2147747792U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendConnectionError = new ExEventLog.EventTuple(2147747794U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendAuthenticationFailed = new ExEventLog.EventTuple(2147747795U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendAckMessage = new ExEventLog.EventTuple(1074005972U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendAckConnection = new ExEventLog.EventTuple(1074005973U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendRemoteDisconnected = new ExEventLog.EventTuple(2147747798U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendNewSession = new ExEventLog.EventTuple(1074005975U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExchangeAuthHashNotSupported = new ExEventLog.EventTuple(3221489627U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SendConnectorInvalidSourceIPAddress = new ExEventLog.EventTuple(3221489630U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendTLSRequiredFailed = new ExEventLog.EventTuple(3221489631U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendAuthenticationInitializationFailed = new ExEventLog.EventTuple(3221489632U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendOutboundAuthenticationFailed = new ExEventLog.EventTuple(3221489633U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendDirectTrustFailed = new ExEventLog.EventTuple(3221489634U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendUnableToTransmitOrar = new ExEventLog.EventTuple(3221489635U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendUnableToTransmitLongOrar = new ExEventLog.EventTuple(3221489636U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendUnableToTransmitRDst = new ExEventLog.EventTuple(3221489637U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendOutboundAtTLSAuthLevelFailed = new ExEventLog.EventTuple(3221489638U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendAuthenticationFailureIgnored = new ExEventLog.EventTuple(2147747815U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendNewProxySession = new ExEventLog.EventTuple(1074005992U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendProxyEhloOptionsDoNotMatch = new ExEventLog.EventTuple(3221489641U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendInboundProxyEhloOptionsDoNotMatch = new ExEventLog.EventTuple(3221489642U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendInboundProxyRecipientLimitsDoNotMatch = new ExEventLog.EventTuple(3221489643U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendPoisonForRemoteThresholdExceeded = new ExEventLog.EventTuple(2147747820U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendInboundProxyNonCriticalEhloOptionsDoNotMatch = new ExEventLog.EventTuple(2147747821U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SmtpSendProxyEhloOptionsDoNotMatchButStillContinueProxying = new ExEventLog.EventTuple(3221489646U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InboundProxyDestinationsTrackerDiagnosticInfo = new ExEventLog.EventTuple(1074005999U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InboundProxyDestinationsTrackerReject = new ExEventLog.EventTuple(3221489648U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InboundProxyDestinationsTrackerNearThreshold = new ExEventLog.EventTuple(2147747825U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InboundProxyAccountForestsTrackerReject = new ExEventLog.EventTuple(3221489650U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InboundProxyAccountForestsTrackerNearThreshold = new ExEventLog.EventTuple(2147747827U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DsnUnableToReadQuarantineConfig = new ExEventLog.EventTuple(3221490620U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DsnUnableToReadSystemMessageConfig = new ExEventLog.EventTuple(3221490621U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DsnDiskFull = new ExEventLog.EventTuple(3221490622U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_XProxyToCommandInvalidEncodedCertificateSubject = new ExEventLog.EventTuple(3221490623U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingPerfCountersLoadFailure = new ExEventLog.EventTuple(3221492617U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingAdUnavailable = new ExEventLog.EventTuple(3221492618U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingWillRetryLoad = new ExEventLog.EventTuple(3221492619U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoServerFqdn = new ExEventLog.EventTuple(3221492620U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoServerAdSite = new ExEventLog.EventTuple(3221492621U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoOwningServerForMdb = new ExEventLog.EventTuple(2147750798U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoRouteToAdSite = new ExEventLog.EventTuple(3221492623U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoRouteToOwningServer = new ExEventLog.EventTuple(3221492624U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoPfTreeMdbRoute = new ExEventLog.EventTuple(2147750801U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoPfTreeRoute = new ExEventLog.EventTuple(3221492626U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoSourceRgForRgConnector = new ExEventLog.EventTuple(3221492627U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoTargetRgForRgConnector = new ExEventLog.EventTuple(3221492628U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoServerRg = new ExEventLog.EventTuple(3221492629U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoSourceBhServers = new ExEventLog.EventTuple(3221492630U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoSourceBhRoute = new ExEventLog.EventTuple(3221492631U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoRouteToConnector = new ExEventLog.EventTuple(3221492632U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoTargetBhServer = new ExEventLog.EventTuple(3221492633U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoTargetBhServers = new ExEventLog.EventTuple(3221492634U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingInvalidSmarthosts = new ExEventLog.EventTuple(3221492635U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingTransientConfigError = new ExEventLog.EventTuple(3221492639U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingMaxConfigLoadRetriesReached = new ExEventLog.EventTuple(3221492640U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoSourceRgForNonRgConnector = new ExEventLog.EventTuple(3221492643U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingLocalConnectorWithConnectedDomains = new ExEventLog.EventTuple(3221492644U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoConnectedRg = new ExEventLog.EventTuple(3221492645U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingTableLogCreationFailure = new ExEventLog.EventTuple(3221492646U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingTableLogDeletionFailure = new ExEventLog.EventTuple(3221492647U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoDeliveryGroupForDatabase = new ExEventLog.EventTuple(2147750827U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoRoutingGroupForDatabase = new ExEventLog.EventTuple(2147750828U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoDagForDatabase = new ExEventLog.EventTuple(2147750829U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoDestinationForDatabase = new ExEventLog.EventTuple(3221492654U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoHubServersSelectedForDatabases = new ExEventLog.EventTuple(3221492655U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingNoHubServersSelectedForTenant = new ExEventLog.EventTuple(3221492656U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InactiveDagsExcludedFromDagSelector = new ExEventLog.EventTuple(1074009009U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DagSelectorDiagnosticInfo = new ExEventLog.EventTuple(1074009010U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TenantDagQuotaDiagnosticInfo = new ExEventLog.EventTuple(1074009011U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingTableDatabaseFullReload = new ExEventLog.EventTuple(1074009012U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RoutingDictionaryInsertFailure = new ExEventLog.EventTuple(3221492661U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PipelineTracingActive = new ExEventLog.EventTuple(2147751292U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerfCountersLoadFailure = new ExEventLog.EventTuple(3221493117U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ExternalServersLatencyTimeNotSync = new ExEventLog.EventTuple(2147751294U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MultiplePreProcessLatencies = new ExEventLog.EventTuple(3221493119U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NullLatencyTreeLeaf = new ExEventLog.EventTuple(3221493120U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MultipleCompletions = new ExEventLog.EventTuple(3221493121U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StopService = new ExEventLog.EventTuple(1074010969U, 6, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseInUse = new ExEventLog.EventTuple(3221494618U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActivationTiming = new ExEventLog.EventTuple(2147752796U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NewDatabaseCreated = new ExEventLog.EventTuple(1074010973U, 6, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseSchemaNotSupported = new ExEventLog.EventTuple(3221494624U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RetrieveServiceState = new ExEventLog.EventTuple(1074010977U, 6, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActivationSlow = new ExEventLog.EventTuple(2147752802U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HubTransportServiceStateChanged = new ExEventLog.EventTuple(2147752803U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FrontendTransportServiceStateChanged = new ExEventLog.EventTuple(2147752804U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FrontendTransportRestartOnServiceStateChange = new ExEventLog.EventTuple(2147752805U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EdgeTransportServiceStateChanged = new ExEventLog.EventTuple(2147752806U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FrontendTransportServiceInitializationFailure = new ExEventLog.EventTuple(3221494631U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EdgeTransportInitializationFailure = new ExEventLog.EventTuple(3221494632U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MSExchangeTransportInitializationFailure = new ExEventLog.EventTuple(3221494633U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResubmitDueToConfigUpdate = new ExEventLog.EventTuple(1074011970U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResubmitDueToInactivityTimeout = new ExEventLog.EventTuple(1074011971U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NonSmtpGWBadDropDirectory = new ExEventLog.EventTuple(3221495620U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NonSmtpGWQuotaExceeded = new ExEventLog.EventTuple(3221495621U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_QueuingStatusAtShutdown = new ExEventLog.EventTuple(1074011974U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NonSmtpGWPathTooLongException = new ExEventLog.EventTuple(3221495623U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NonSmtpGWNoDropDirectory = new ExEventLog.EventTuple(3221495624U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NonSmtpGWUnauthorizedAccess = new ExEventLog.EventTuple(3221495625U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RetryDeliveryIfRejected = new ExEventLog.EventTuple(3221495626U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OnOpenConnectionAgentException = new ExEventLog.EventTuple(3221495627U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OnDeliverMailItemAgentException = new ExEventLog.EventTuple(3221495628U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OnCloseConnectionAgentException = new ExEventLog.EventTuple(3221495629U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResubmitDueToUnavailabilityOfSameVersionHubs = new ExEventLog.EventTuple(1074011982U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RedirectMessageStarted = new ExEventLog.EventTuple(1074011983U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_QueueViewerExceptionDuringAsyncRetryQueue = new ExEventLog.EventTuple(3221495632U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResubmitDueToOutboundConnectorChange = new ExEventLog.EventTuple(1074011985U, 7, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RetryQueueOutboundConnectorLookupFailed = new ExEventLog.EventTuple(3221495634U, 7, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryDoesNotExist = new ExEventLog.EventTuple(1074012969U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NoDirectoryPermission = new ExEventLog.EventTuple(3221496618U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ReadOnlyFileFound = new ExEventLog.EventTuple(2147754795U, 8, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CannotDeleteFile = new ExEventLog.EventTuple(3221496620U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PickupFailedDueToStorageErrors = new ExEventLog.EventTuple(3221496621U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCreatePickupDirectory = new ExEventLog.EventTuple(3221496622U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NoPermissionToRenamePickupFile = new ExEventLog.EventTuple(3221496623U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AccessErrorModifyingPickupRegkey = new ExEventLog.EventTuple(3221496625U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PickupIsBadmailingFile = new ExEventLog.EventTuple(3221496626U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PickupFileEncrypted = new ExEventLog.EventTuple(3221496628U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OnSubmittedMessageAgentException = new ExEventLog.EventTuple(3221496817U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OnRoutedMessageAgentException = new ExEventLog.EventTuple(3221496818U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransportConfigContainerNotFound = new ExEventLog.EventTuple(3221496824U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResolverPerfCountersLoadFailure = new ExEventLog.EventTuple(3221496828U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RetryCategorizationIfFailed = new ExEventLog.EventTuple(3221496829U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmbiguousSender = new ExEventLog.EventTuple(2147755006U, 9, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OnCategorizedMessageAgentException = new ExEventLog.EventTuple(3221496831U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OnResolvedMessageAgentException = new ExEventLog.EventTuple(3221496832U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AmbiguousRecipient = new ExEventLog.EventTuple(3221496833U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NDRForUnrestrictedLargeDL = new ExEventLog.EventTuple(3221496834U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CategorizerErrorRetrievingTenantOverride = new ExEventLog.EventTuple(3221496835U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MessageCountEnqueuedToPoisonQueue = new ExEventLog.EventTuple(2147755793U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PoisonCountUpdated = new ExEventLog.EventTuple(1074013970U, 10, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PoisonMessageCrash = new ExEventLog.EventTuple(3221497619U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DeletedPoisonPickupFile = new ExEventLog.EventTuple(2147755796U, 10, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PoisonMessageLoadFailedRegistryAccessDenied = new ExEventLog.EventTuple(3221497621U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PoisonMessageSaveFailedRegistryAccessDenied = new ExEventLog.EventTuple(3221497622U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PoisonMessageMarkFailedRegistryAccessDenied = new ExEventLog.EventTuple(3221497623U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PoisonMessagePruneFailedRegistryAccessDenied = new ExEventLog.EventTuple(3221497624U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MessageSecurityTLSCertificateValidationFailure = new ExEventLog.EventTuple(3221498621U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TlsDomainCertificateValidationFailure = new ExEventLog.EventTuple(3221498627U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MessageNotAuthenticatedTlsNotStarted = new ExEventLog.EventTuple(3221498628U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MessageToSecureDomainFailedDueToTlsNegotiationFailure = new ExEventLog.EventTuple(3221498629U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MessageToSecureDomainFailedBecauseTlsNotOffered = new ExEventLog.EventTuple(3221498630U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TlsDomainClientCertificateSubjectMismatch = new ExEventLog.EventTuple(3221498631U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TlsDomainServerCertificateSubjectMismatch = new ExEventLog.EventTuple(3221498632U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MessageNotAuthenticatedNoClientCertificate = new ExEventLog.EventTuple(3221498633U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MessageNotAuthenticatedTlsNotAdvertised = new ExEventLog.EventTuple(3221498634U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MessageToSecureDomainFailedBecauseTlsNegotiationFailed = new ExEventLog.EventTuple(3221498635U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TlsDomainServerCertificateValidationFailure = new ExEventLog.EventTuple(3221498636U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TlsDomainSecureDisabled = new ExEventLog.EventTuple(3221498637U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TlsDomainCapabilitiesCertificateValidationFailure = new ExEventLog.EventTuple(3221498638U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SessionFailedBecauseXOorgNotOffered = new ExEventLog.EventTuple(3221498639U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SubjectAlternativeNameLimitExceeded = new ExEventLog.EventTuple(2147756816U, 11, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CertificateRevocationListCheckTrasientFailureTreatedAsSuccess = new ExEventLog.EventTuple(3221498642U, 11, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToFlushTicketCacheOnInitialize = new ExEventLog.EventTuple(3221499617U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigUpdateOccurred = new ExEventLog.EventTuple(1074015970U, 12, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidServerRole = new ExEventLog.EventTuple(3221499619U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReadConfigReceiveConnectorFailed = new ExEventLog.EventTuple(3221499624U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReadConfigReceiveConnectorUnavail = new ExEventLog.EventTuple(3221499625U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADRecipientCachePerfCountersLoadFailure = new ExEventLog.EventTuple(3221499626U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SpnRegisterFailure = new ExEventLog.EventTuple(3221499627U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InternalTransportCertificateMissingInAD = new ExEventLog.EventTuple(3221499628U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CannotLoadInternalTransportCertificateFromStore = new ExEventLog.EventTuple(3221499629U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CannotLoadSTARTTLSCertificateFromStore = new ExEventLog.EventTuple(3221499630U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InternalTransportCertificateExpired = new ExEventLog.EventTuple(3221499631U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_STARTTLSCertificateExpired = new ExEventLog.EventTuple(3221499632U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InternalTransportCertificateExpiresSoon = new ExEventLog.EventTuple(3221499633U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_STARTTLSCertificateExpiresSoon = new ExEventLog.EventTuple(3221499634U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RemoteInternalTransportCertificateExpired = new ExEventLog.EventTuple(3221499635U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RemoteSTARTTLSCertificateExpired = new ExEventLog.EventTuple(3221499636U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReadConfigReceiveConnectorIgnored = new ExEventLog.EventTuple(2147757813U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InternalTransportCertificateCorruptedInAD = new ExEventLog.EventTuple(3221499638U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CannotLoadInternalTransportCertificateFallbackServerFQDN = new ExEventLog.EventTuple(2147757815U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CannotLoadIntTransportCertificateFallbackEphemeralCertificate = new ExEventLog.EventTuple(2147757816U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DisconnectingPerformanceCounters = new ExEventLog.EventTuple(2147757817U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToDisconnectPerformanceCounters = new ExEventLog.EventTuple(2147757818U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProcessHoldingPerformanceCounter = new ExEventLog.EventTuple(2147757820U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_ServerLivingForConsiderableTime = new ExEventLog.EventTuple(1074015997U, 12, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_KillOrphanedWorker = new ExEventLog.EventTuple(2147757822U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AnotherServiceRunning = new ExEventLog.EventTuple(2147757823U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_KillOrphanedWorkerFailed = new ExEventLog.EventTuple(3221499648U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToRegisterForDeletedObjectsNotification = new ExEventLog.EventTuple(3221499649U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SystemLowOnMemory = new ExEventLog.EventTuple(2147757826U, 12, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_Exch50OrgNotFound = new ExEventLog.EventTuple(3221500618U, 13, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ProcessNotResponding = new ExEventLog.EventTuple(3221501617U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AppConfigLoadFailed = new ExEventLog.EventTuple(3221501620U, 14, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResourceUtilizationUp = new ExEventLog.EventTuple(2147760796U, 15, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResourceUtilizationDown = new ExEventLog.EventTuple(1074018973U, 15, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DiskSpaceLow = new ExEventLog.EventTuple(3221502622U, 15, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PrivateBytesHigh = new ExEventLog.EventTuple(3221502623U, 15, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ComponentFailedTransportServerUpdate = new ExEventLog.EventTuple(3221503620U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MessageTrackingLogPathIsNull = new ExEventLog.EventTuple(2147761803U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReceiveProtocolLogPathIsNull = new ExEventLog.EventTuple(2147761804U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SendProtocolLogPathIsNull = new ExEventLog.EventTuple(2147761805U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReceiveProtocolLogPathIsNullUsingOld = new ExEventLog.EventTuple(2147761806U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SendProtocolLogPathIsNullUsingOld = new ExEventLog.EventTuple(2147761807U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DefaultAuthoritativeDomainInvalid = new ExEventLog.EventTuple(3221503632U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActivationFailed = new ExEventLog.EventTuple(3221503633U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationLoaderExternalError = new ExEventLog.EventTuple(2147761810U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationLoaderException = new ExEventLog.EventTuple(2147761811U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidAcceptedDomain = new ExEventLog.EventTuple(3221503637U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationLoaderSuccessfulUpdate = new ExEventLog.EventTuple(1074019990U, 16, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CannotStartAgents = new ExEventLog.EventTuple(3221503639U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RejectedAcceptedDomain = new ExEventLog.EventTuple(3221503640U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidAdapterGuid = new ExEventLog.EventTuple(3221503641U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NetworkAdapterIPQueryFailed = new ExEventLog.EventTuple(3221503642U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationLoaderNoADNotifications = new ExEventLog.EventTuple(2147761819U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationLoaderSuccessfulForcedUpdate = new ExEventLog.EventTuple(1074019996U, 16, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_HeartbeatDestinationConfigChanged = new ExEventLog.EventTuple(2147761821U, 16, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AgentErrorHandlingOverrideConfigError = new ExEventLog.EventTuple(3221503646U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SchemaTypeMismatch = new ExEventLog.EventTuple(3221504617U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SchemaRequiredColumnNotFound = new ExEventLog.EventTuple(3221504618U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetCorruptionError = new ExEventLog.EventTuple(3221504619U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetOutOfSpaceError = new ExEventLog.EventTuple(3221504620U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetLogFileError = new ExEventLog.EventTuple(3221504621U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetPathError = new ExEventLog.EventTuple(3221504622U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetMismatchError = new ExEventLog.EventTuple(3221504623U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StartScanForMessages = new ExEventLog.EventTuple(1074020976U, 17, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StopScanForMessages = new ExEventLog.EventTuple(1074020977U, 17, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EndScanForMessages = new ExEventLog.EventTuple(1074020978U, 17, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetCheckpointFileError = new ExEventLog.EventTuple(3221504627U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetInitInstanceOutOfMemory = new ExEventLog.EventTuple(3221504628U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetInstanceNameInUse = new ExEventLog.EventTuple(3221504629U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetDatabaseNotFound = new ExEventLog.EventTuple(3221504630U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetDatabaseLogSetMismatch = new ExEventLog.EventTuple(3221504631U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetFragmentationError = new ExEventLog.EventTuple(3221504632U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetQuotaExceededError = new ExEventLog.EventTuple(3221504633U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetInsufficientResourcesError = new ExEventLog.EventTuple(3221504634U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetIOError = new ExEventLog.EventTuple(3221504635U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetOperationError = new ExEventLog.EventTuple(3221504636U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetTableNotFound = new ExEventLog.EventTuple(3221504637U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetFileNotFound = new ExEventLog.EventTuple(3221504638U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetFileReadOnly = new ExEventLog.EventTuple(3221504639U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_JetVersionStoreOutOfMemoryError = new ExEventLog.EventTuple(3221504640U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LastMessagesLoadedByBootScanner = new ExEventLog.EventTuple(1074020993U, 17, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseDriveIsNotAccessible = new ExEventLog.EventTuple(3221504642U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ColumnTooBigException = new ExEventLog.EventTuple(3221504643U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AgentDidNotCloseMimeStream = new ExEventLog.EventTuple(3221505617U, 18, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientValidationCacheLoaded = new ExEventLog.EventTuple(1074022969U, 19, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientGroupCacheLoaded = new ExEventLog.EventTuple(1074022970U, 19, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryUnavailableLoadingGroup = new ExEventLog.EventTuple(2147764798U, 19, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryUnavailableLoadingValidationCache = new ExEventLog.EventTuple(2147764799U, 19, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientHasDataValidationException = new ExEventLog.EventTuple(3221506624U, 19, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ORARMessageSubmitted = new ExEventLog.EventTuple(1074023969U, 20, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailParseOrarBlob = new ExEventLog.EventTuple(2147765794U, 20, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailGetRoutingAddress = new ExEventLog.EventTuple(2147765795U, 20, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseRecoveryActionNone = new ExEventLog.EventTuple(3221504717U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseRecoveryActionMove = new ExEventLog.EventTuple(2147762894U, 17, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseRecoveryActionDelete = new ExEventLog.EventTuple(1074021071U, 17, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseRecoveryActionFailed = new ExEventLog.EventTuple(3221504720U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseRecoveryActionFailedRegistryAccessDenied = new ExEventLog.EventTuple(3221504721U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DataBaseCorruptionDetected = new ExEventLog.EventTuple(1074021074U, 17, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseErrorDetected = new ExEventLog.EventTuple(1074021075U, 17, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TableLockedException = new ExEventLog.EventTuple(3221504724U, 17, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ShadowRedundancyMessagesResubmitted = new ExEventLog.EventTuple(1074025969U, 22, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ShadowRedundancyMessageResubmitSuppressed = new ExEventLog.EventTuple(1074025970U, 22, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ShadowRedundancyPrimaryServerDatabaseStateChanged = new ExEventLog.EventTuple(3221509619U, 22, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ShadowRedundancyPrimaryServerHeartbeatFailed = new ExEventLog.EventTuple(2147767796U, 22, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ShadowRedundancyMessageDeferredDueToShadowFailure = new ExEventLog.EventTuple(3221509622U, 22, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ShadowRedundancyForcedHeartbeatReset = new ExEventLog.EventTuple(2147767799U, 22, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ModeratedTransportNoArbitrationMailbox = new ExEventLog.EventTuple(3221510617U, 23, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientStampedWithDeletedArbitrationMailbox = new ExEventLog.EventTuple(3221510618U, 23, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RemovedMessageRepositoryRequest = new ExEventLog.EventTuple(1074027972U, 24, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ModifiedMessageRepositoryRequest = new ExEventLog.EventTuple(1074027973U, 24, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RegisterRpcServerFailure = new ExEventLog.EventTuple(1074027975U, 24, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MessageAttributionFailed = new ExEventLog.EventTuple(3221511626U, 25, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransportWlmLogPathIsNull = new ExEventLog.EventTuple(3221511627U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResubmitRequestExpired = new ExEventLog.EventTuple(1074027980U, 24, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DuplicateResubmitRequest = new ExEventLog.EventTuple(2147769805U, 24, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MaxRunningResubmitRequest = new ExEventLog.EventTuple(2147769806U, 24, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MaxRecentResubmitRequest = new ExEventLog.EventTuple(2147769807U, 24, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InterceptorAgentConfigurationLoadingError = new ExEventLog.EventTuple(3221512617U, 18, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InterceptorAgentConfigurationReplaced = new ExEventLog.EventTuple(287146U, 18, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InterceptorAgentAccessDenied = new ExEventLog.EventTuple(3221512619U, 18, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InterceptorRuleNearingExpiration = new ExEventLog.EventTuple(2147770796U, 18, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_QueueQuotaComponentLogPathIsNull = new ExEventLog.EventTuple(3221512622U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_NotEnoughMemoryToStartService = new ExEventLog.EventTuple(3221512623U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FlowControlLogPathIsNull = new ExEventLog.EventTuple(3221512624U, 16, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FilePathOnLockedVolume = new ExEventLog.EventTuple(3221512625U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BitlockerQueryFailed = new ExEventLog.EventTuple(3221512626U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			SmtpReceive = 1,
			SmtpSend,
			Dsn,
			Routing,
			Logging,
			Components,
			RemoteDelivery,
			Pickup,
			Categorizer,
			PoisonMessage,
			MessageSecurity,
			TransportService,
			Exch50,
			Process,
			ResourceManager,
			Configuration,
			Storage,
			Agents,
			Transport_Address_Book,
			Orar,
			Unused,
			ShadowRedundancy,
			Approval,
			TransportSafetyNet,
			TransportTenantAttribution
		}

		internal enum Message : uint
		{
			BindingIPv6ButDisabled = 3221488616U,
			AddressInUse,
			ConfiguredConnectors = 1074004970U,
			SmtpReceiveMaxConnectionReached = 2147746801U,
			SmtpReceiveMessageRejected = 1074004978U,
			SmtpReceiveAuthenticationFailedTooManyErrors = 2147746804U,
			SmtpReceiveRejectDueToStorageError = 3221488629U,
			SmtpReceiveAuthorizationSubmitRejected = 2147746810U,
			SmtpReceiveAuthorizationRejected = 2147746812U,
			SmtpReceiveMaxConnectionPerSourceReached,
			InternalSMTPServerListEmpty,
			SmtpReceiveSubmitDenied = 1074004991U,
			SmtpReceiveSendAsDeniedTempAuthFailure,
			SmtpReceiveSendAsDeniedSenderAddressDataInvalid = 3221488641U,
			SmtpReceiveSendAsDenied = 1074004994U,
			SmtpReceiveSendOnBehalfOfDeniedTempAuthFailure,
			SmtpReceiveSendOnBehalfOfDeniedFromAddressDataInvalid = 3221488644U,
			SmtpReceiveSendOnBehalfOfDenied = 1074004997U,
			SmtpReceiveCouldNotDetermineUserNameOrSid = 3221488646U,
			SmtpReceiveMessageRateLimitExceeded = 1074004999U,
			SmtpReceiveTLSRequiredFailed = 3221488648U,
			SmtpReceiveCatchAll,
			SmtpReceiveAuthenticationInitializationFailed,
			SmtpReceiveAuthenticationFailed = 2147746827U,
			SmtpReceiveDirectTrustFailed = 3221488652U,
			SmtpReceiveActiveManagerFailure,
			SmtpReceiveAvailabilityCounterFailure,
			SmtpReceiveProhibitSendQuotaDeniedTempAuthFailure = 1074005007U,
			SmtpReceiveConnectorAvailabilityLow = 3221488656U,
			SmtpReceiveConnectorAvailabilityNormal = 1074005009U,
			SmtpReceiveProxyMserveLookupFailed = 3221488658U,
			SmtpReceiveProxyInvalidPartnerId,
			SmtpReceiveProxyDnsLookupFailed,
			SmtpReceiveProxyCatchAll,
			SmtpReceiveProxyCounterFailure = 3221488663U,
			SmtpReceiveTooManyProxySessionFailures,
			SmtpReceiveNoDestinationToProxyTo,
			SmtpReceiveProcessingBlobFailed,
			SmtpSendDnsConnectionFailure = 2147747792U,
			SmtpSendConnectionError = 2147747794U,
			SmtpSendAuthenticationFailed,
			SmtpSendAckMessage = 1074005972U,
			SmtpSendAckConnection,
			SmtpSendRemoteDisconnected = 2147747798U,
			SmtpSendNewSession = 1074005975U,
			ExchangeAuthHashNotSupported = 3221489627U,
			SendConnectorInvalidSourceIPAddress = 3221489630U,
			SmtpSendTLSRequiredFailed,
			SmtpSendAuthenticationInitializationFailed,
			SmtpSendOutboundAuthenticationFailed,
			SmtpSendDirectTrustFailed,
			SmtpSendUnableToTransmitOrar,
			SmtpSendUnableToTransmitLongOrar,
			SmtpSendUnableToTransmitRDst,
			SmtpSendOutboundAtTLSAuthLevelFailed,
			SmtpSendAuthenticationFailureIgnored = 2147747815U,
			SmtpSendNewProxySession = 1074005992U,
			SmtpSendProxyEhloOptionsDoNotMatch = 3221489641U,
			SmtpSendInboundProxyEhloOptionsDoNotMatch,
			SmtpSendInboundProxyRecipientLimitsDoNotMatch,
			SmtpSendPoisonForRemoteThresholdExceeded = 2147747820U,
			SmtpSendInboundProxyNonCriticalEhloOptionsDoNotMatch,
			SmtpSendProxyEhloOptionsDoNotMatchButStillContinueProxying = 3221489646U,
			InboundProxyDestinationsTrackerDiagnosticInfo = 1074005999U,
			InboundProxyDestinationsTrackerReject = 3221489648U,
			InboundProxyDestinationsTrackerNearThreshold = 2147747825U,
			InboundProxyAccountForestsTrackerReject = 3221489650U,
			InboundProxyAccountForestsTrackerNearThreshold = 2147747827U,
			DsnUnableToReadQuarantineConfig = 3221490620U,
			DsnUnableToReadSystemMessageConfig,
			DsnDiskFull,
			XProxyToCommandInvalidEncodedCertificateSubject,
			RoutingPerfCountersLoadFailure = 3221492617U,
			RoutingAdUnavailable,
			RoutingWillRetryLoad,
			RoutingNoServerFqdn,
			RoutingNoServerAdSite,
			RoutingNoOwningServerForMdb = 2147750798U,
			RoutingNoRouteToAdSite = 3221492623U,
			RoutingNoRouteToOwningServer,
			RoutingNoPfTreeMdbRoute = 2147750801U,
			RoutingNoPfTreeRoute = 3221492626U,
			RoutingNoSourceRgForRgConnector,
			RoutingNoTargetRgForRgConnector,
			RoutingNoServerRg,
			RoutingNoSourceBhServers,
			RoutingNoSourceBhRoute,
			RoutingNoRouteToConnector,
			RoutingNoTargetBhServer,
			RoutingNoTargetBhServers,
			RoutingInvalidSmarthosts,
			RoutingTransientConfigError = 3221492639U,
			RoutingMaxConfigLoadRetriesReached,
			RoutingNoSourceRgForNonRgConnector = 3221492643U,
			RoutingLocalConnectorWithConnectedDomains,
			RoutingNoConnectedRg,
			RoutingTableLogCreationFailure,
			RoutingTableLogDeletionFailure,
			RoutingNoDeliveryGroupForDatabase = 2147750827U,
			RoutingNoRoutingGroupForDatabase,
			RoutingNoDagForDatabase,
			RoutingNoDestinationForDatabase = 3221492654U,
			RoutingNoHubServersSelectedForDatabases,
			RoutingNoHubServersSelectedForTenant,
			InactiveDagsExcludedFromDagSelector = 1074009009U,
			DagSelectorDiagnosticInfo,
			TenantDagQuotaDiagnosticInfo,
			RoutingTableDatabaseFullReload,
			RoutingDictionaryInsertFailure = 3221492661U,
			PipelineTracingActive = 2147751292U,
			PerfCountersLoadFailure = 3221493117U,
			ExternalServersLatencyTimeNotSync = 2147751294U,
			MultiplePreProcessLatencies = 3221493119U,
			NullLatencyTreeLeaf,
			MultipleCompletions,
			StopService = 1074010969U,
			DatabaseInUse = 3221494618U,
			ActivationTiming = 2147752796U,
			NewDatabaseCreated = 1074010973U,
			DatabaseSchemaNotSupported = 3221494624U,
			RetrieveServiceState = 1074010977U,
			ActivationSlow = 2147752802U,
			HubTransportServiceStateChanged,
			FrontendTransportServiceStateChanged,
			FrontendTransportRestartOnServiceStateChange,
			EdgeTransportServiceStateChanged,
			FrontendTransportServiceInitializationFailure = 3221494631U,
			EdgeTransportInitializationFailure,
			MSExchangeTransportInitializationFailure,
			ResubmitDueToConfigUpdate = 1074011970U,
			ResubmitDueToInactivityTimeout,
			NonSmtpGWBadDropDirectory = 3221495620U,
			NonSmtpGWQuotaExceeded,
			QueuingStatusAtShutdown = 1074011974U,
			NonSmtpGWPathTooLongException = 3221495623U,
			NonSmtpGWNoDropDirectory,
			NonSmtpGWUnauthorizedAccess,
			RetryDeliveryIfRejected,
			OnOpenConnectionAgentException,
			OnDeliverMailItemAgentException,
			OnCloseConnectionAgentException,
			ResubmitDueToUnavailabilityOfSameVersionHubs = 1074011982U,
			RedirectMessageStarted,
			QueueViewerExceptionDuringAsyncRetryQueue = 3221495632U,
			ResubmitDueToOutboundConnectorChange = 1074011985U,
			RetryQueueOutboundConnectorLookupFailed = 3221495634U,
			DirectoryDoesNotExist = 1074012969U,
			NoDirectoryPermission = 3221496618U,
			ReadOnlyFileFound = 2147754795U,
			CannotDeleteFile = 3221496620U,
			PickupFailedDueToStorageErrors,
			FailedToCreatePickupDirectory,
			NoPermissionToRenamePickupFile,
			AccessErrorModifyingPickupRegkey = 3221496625U,
			PickupIsBadmailingFile,
			PickupFileEncrypted = 3221496628U,
			OnSubmittedMessageAgentException = 3221496817U,
			OnRoutedMessageAgentException,
			TransportConfigContainerNotFound = 3221496824U,
			ResolverPerfCountersLoadFailure = 3221496828U,
			RetryCategorizationIfFailed,
			AmbiguousSender = 2147755006U,
			OnCategorizedMessageAgentException = 3221496831U,
			OnResolvedMessageAgentException,
			AmbiguousRecipient,
			NDRForUnrestrictedLargeDL,
			CategorizerErrorRetrievingTenantOverride,
			MessageCountEnqueuedToPoisonQueue = 2147755793U,
			PoisonCountUpdated = 1074013970U,
			PoisonMessageCrash = 3221497619U,
			DeletedPoisonPickupFile = 2147755796U,
			PoisonMessageLoadFailedRegistryAccessDenied = 3221497621U,
			PoisonMessageSaveFailedRegistryAccessDenied,
			PoisonMessageMarkFailedRegistryAccessDenied,
			PoisonMessagePruneFailedRegistryAccessDenied,
			MessageSecurityTLSCertificateValidationFailure = 3221498621U,
			TlsDomainCertificateValidationFailure = 3221498627U,
			MessageNotAuthenticatedTlsNotStarted,
			MessageToSecureDomainFailedDueToTlsNegotiationFailure,
			MessageToSecureDomainFailedBecauseTlsNotOffered,
			TlsDomainClientCertificateSubjectMismatch,
			TlsDomainServerCertificateSubjectMismatch,
			MessageNotAuthenticatedNoClientCertificate,
			MessageNotAuthenticatedTlsNotAdvertised,
			MessageToSecureDomainFailedBecauseTlsNegotiationFailed,
			TlsDomainServerCertificateValidationFailure,
			TlsDomainSecureDisabled,
			TlsDomainCapabilitiesCertificateValidationFailure,
			SessionFailedBecauseXOorgNotOffered,
			SubjectAlternativeNameLimitExceeded = 2147756816U,
			CertificateRevocationListCheckTrasientFailureTreatedAsSuccess = 3221498642U,
			FailedToFlushTicketCacheOnInitialize = 3221499617U,
			ConfigUpdateOccurred = 1074015970U,
			InvalidServerRole = 3221499619U,
			ReadConfigReceiveConnectorFailed = 3221499624U,
			ReadConfigReceiveConnectorUnavail,
			ADRecipientCachePerfCountersLoadFailure,
			SpnRegisterFailure,
			InternalTransportCertificateMissingInAD,
			CannotLoadInternalTransportCertificateFromStore,
			CannotLoadSTARTTLSCertificateFromStore,
			InternalTransportCertificateExpired,
			STARTTLSCertificateExpired,
			InternalTransportCertificateExpiresSoon,
			STARTTLSCertificateExpiresSoon,
			RemoteInternalTransportCertificateExpired,
			RemoteSTARTTLSCertificateExpired,
			ReadConfigReceiveConnectorIgnored = 2147757813U,
			InternalTransportCertificateCorruptedInAD = 3221499638U,
			CannotLoadInternalTransportCertificateFallbackServerFQDN = 2147757815U,
			CannotLoadIntTransportCertificateFallbackEphemeralCertificate,
			DisconnectingPerformanceCounters,
			FailedToDisconnectPerformanceCounters,
			ProcessHoldingPerformanceCounter = 2147757820U,
			ServerLivingForConsiderableTime = 1074015997U,
			KillOrphanedWorker = 2147757822U,
			AnotherServiceRunning,
			KillOrphanedWorkerFailed = 3221499648U,
			FailedToRegisterForDeletedObjectsNotification,
			SystemLowOnMemory = 2147757826U,
			Exch50OrgNotFound = 3221500618U,
			ProcessNotResponding = 3221501617U,
			AppConfigLoadFailed = 3221501620U,
			ResourceUtilizationUp = 2147760796U,
			ResourceUtilizationDown = 1074018973U,
			DiskSpaceLow = 3221502622U,
			PrivateBytesHigh,
			ComponentFailedTransportServerUpdate = 3221503620U,
			MessageTrackingLogPathIsNull = 2147761803U,
			ReceiveProtocolLogPathIsNull,
			SendProtocolLogPathIsNull,
			ReceiveProtocolLogPathIsNullUsingOld,
			SendProtocolLogPathIsNullUsingOld,
			DefaultAuthoritativeDomainInvalid = 3221503632U,
			ActivationFailed,
			ConfigurationLoaderExternalError = 2147761810U,
			ConfigurationLoaderException,
			InvalidAcceptedDomain = 3221503637U,
			ConfigurationLoaderSuccessfulUpdate = 1074019990U,
			CannotStartAgents = 3221503639U,
			RejectedAcceptedDomain,
			InvalidAdapterGuid,
			NetworkAdapterIPQueryFailed,
			ConfigurationLoaderNoADNotifications = 2147761819U,
			ConfigurationLoaderSuccessfulForcedUpdate = 1074019996U,
			HeartbeatDestinationConfigChanged = 2147761821U,
			AgentErrorHandlingOverrideConfigError = 3221503646U,
			SchemaTypeMismatch = 3221504617U,
			SchemaRequiredColumnNotFound,
			JetCorruptionError,
			JetOutOfSpaceError,
			JetLogFileError,
			JetPathError,
			JetMismatchError,
			StartScanForMessages = 1074020976U,
			StopScanForMessages,
			EndScanForMessages,
			JetCheckpointFileError = 3221504627U,
			JetInitInstanceOutOfMemory,
			JetInstanceNameInUse,
			JetDatabaseNotFound,
			JetDatabaseLogSetMismatch,
			JetFragmentationError,
			JetQuotaExceededError,
			JetInsufficientResourcesError,
			JetIOError,
			JetOperationError,
			JetTableNotFound,
			JetFileNotFound,
			JetFileReadOnly,
			JetVersionStoreOutOfMemoryError,
			LastMessagesLoadedByBootScanner = 1074020993U,
			DatabaseDriveIsNotAccessible = 3221504642U,
			ColumnTooBigException,
			AgentDidNotCloseMimeStream = 3221505617U,
			RecipientValidationCacheLoaded = 1074022969U,
			RecipientGroupCacheLoaded,
			DirectoryUnavailableLoadingGroup = 2147764798U,
			DirectoryUnavailableLoadingValidationCache,
			RecipientHasDataValidationException = 3221506624U,
			ORARMessageSubmitted = 1074023969U,
			FailParseOrarBlob = 2147765794U,
			FailGetRoutingAddress,
			DatabaseRecoveryActionNone = 3221504717U,
			DatabaseRecoveryActionMove = 2147762894U,
			DatabaseRecoveryActionDelete = 1074021071U,
			DatabaseRecoveryActionFailed = 3221504720U,
			DatabaseRecoveryActionFailedRegistryAccessDenied,
			DataBaseCorruptionDetected = 1074021074U,
			DatabaseErrorDetected,
			TableLockedException = 3221504724U,
			ShadowRedundancyMessagesResubmitted = 1074025969U,
			ShadowRedundancyMessageResubmitSuppressed,
			ShadowRedundancyPrimaryServerDatabaseStateChanged = 3221509619U,
			ShadowRedundancyPrimaryServerHeartbeatFailed = 2147767796U,
			ShadowRedundancyMessageDeferredDueToShadowFailure = 3221509622U,
			ShadowRedundancyForcedHeartbeatReset = 2147767799U,
			ModeratedTransportNoArbitrationMailbox = 3221510617U,
			RecipientStampedWithDeletedArbitrationMailbox,
			RemovedMessageRepositoryRequest = 1074027972U,
			ModifiedMessageRepositoryRequest,
			RegisterRpcServerFailure = 1074027975U,
			MessageAttributionFailed = 3221511626U,
			TransportWlmLogPathIsNull,
			ResubmitRequestExpired = 1074027980U,
			DuplicateResubmitRequest = 2147769805U,
			MaxRunningResubmitRequest,
			MaxRecentResubmitRequest,
			InterceptorAgentConfigurationLoadingError = 3221512617U,
			InterceptorAgentConfigurationReplaced = 287146U,
			InterceptorAgentAccessDenied = 3221512619U,
			InterceptorRuleNearingExpiration = 2147770796U,
			QueueQuotaComponentLogPathIsNull = 3221512622U,
			NotEnoughMemoryToStartService,
			FlowControlLogPathIsNull,
			FilePathOnLockedVolume,
			BitlockerQueryFailed
		}
	}
}
