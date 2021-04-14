using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.EventLog
{
	public static class DirectoryEventLogConstants
	{
		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_SYNC_FAILED = new ExEventLog.EventTuple(3221489677U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DC_DOWN = new ExEventLog.EventTuple(1074006038U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_FIND_LOCAL_SERVER_FAILED = new ExEventLog.EventTuple(3221489691U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DISCOVERED_SERVERS = new ExEventLog.EventTuple(1074006048U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_GOING_IN_SITE_DC = new ExEventLog.EventTuple(1074006050U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_GOING_IN_SITE_GC = new ExEventLog.EventTuple(1074006051U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_REG_BAD_DWORD = new ExEventLog.EventTuple(1074006056U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_REG_CDC_BAD = new ExEventLog.EventTuple(2147747881U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_REG_CDC_DOWN = new ExEventLog.EventTuple(2147747882U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_REG_SERVER_BAD = new ExEventLog.EventTuple(2147747883U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_REG_DC = new ExEventLog.EventTuple(1074006060U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_REG_GC = new ExEventLog.EventTuple(1074006061U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_REG_CDC = new ExEventLog.EventTuple(1074006064U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_ALL_DC_DOWN = new ExEventLog.EventTuple(3221489718U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_ALL_GC_DOWN = new ExEventLog.EventTuple(3221489719U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_GETHOSTBYNAME_FAILED = new ExEventLog.EventTuple(2147747899U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_BIND_FAILED = new ExEventLog.EventTuple(3221489726U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_SUITABILITY_CHECK_FAILED = new ExEventLog.EventTuple(3221489727U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_NO_SACL = new ExEventLog.EventTuple(2147747904U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_GOT_SACL = new ExEventLog.EventTuple(1074006081U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_FATAL_ERROR = new ExEventLog.EventTuple(2147747907U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_BAD_OS_VERSION = new ExEventLog.EventTuple(2147747908U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_IMPERSONATED_CALLER = new ExEventLog.EventTuple(2147747909U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DNS_DIAG_SERVER_FAILURE = new ExEventLog.EventTuple(3221489734U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DNS_NAME_ERROR = new ExEventLog.EventTuple(3221489735U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DNS_TIMEOUT = new ExEventLog.EventTuple(3221489736U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DNS_NO_ERROR = new ExEventLog.EventTuple(2147747913U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DNS_OTHER = new ExEventLog.EventTuple(3221489738U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DNS_NO_ERROR_DC_FOUND = new ExEventLog.EventTuple(2147747915U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DNS_NO_ERROR_DC_NOT_FOUND = new ExEventLog.EventTuple(2147747916U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_NO_CONNECTION = new ExEventLog.EventTuple(3221489742U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DC_UP = new ExEventLog.EventTuple(1074006095U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_BASE_CONFIG_SEARCH_FAILED = new ExEventLog.EventTuple(2147747920U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_GET_DC_FROM_DOMAIN = new ExEventLog.EventTuple(1074006097U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_GET_DC_FROM_DOMAIN_FAILED = new ExEventLog.EventTuple(3221489746U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_NEW_CONNECTION = new ExEventLog.EventTuple(1074006099U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_CONNECTION_CLOSED = new ExEventLog.EventTuple(1074006100U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_LONG_RUNNING_OPERATION = new ExEventLog.EventTuple(2147747925U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_NON_UNIQUE_RECIPIENT = new ExEventLog.EventTuple(2147747928U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_ROOTDSE_READ_FAILED = new ExEventLog.EventTuple(3221489753U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_NO_CONNECTION_TO_SERVER = new ExEventLog.EventTuple(3221489754U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_TOPO_INITIALIZATION_TIMEOUT = new ExEventLog.EventTuple(3221489755U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_PREFERRED_TOPOLOGY = new ExEventLog.EventTuple(1074006109U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_ADAM_GET_SERVER_FROM_DOMAIN_DN = new ExEventLog.EventTuple(3221489759U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_AD_DRIVER_PERF_INIT_FAILED = new ExEventLog.EventTuple(2147747941U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_TOPOLOGY_UPDATE = new ExEventLog.EventTuple(1074006118U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_AD_DRIVER_INIT = new ExEventLog.EventTuple(1074006119U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_WRITE_FAILED = new ExEventLog.EventTuple(3221489769U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_RANGED_READ = new ExEventLog.EventTuple(1074006122U, 5, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_VALIDATION_FAILED_FCO_MODE_CONFIG = new ExEventLog.EventTuple(3221489771U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_VALIDATION_FAILED_FCO_MODE_RECIPIENT = new ExEventLog.EventTuple(3221489772U, 6, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_VALIDATION_FAILED_PCO_MODE_CONFIG = new ExEventLog.EventTuple(2147747949U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_VALIDATION_FAILED_PCO_MODE_RECIPIENT = new ExEventLog.EventTuple(2147747950U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_VALIDATION_FAILED_IGNORE_MODE_CONFIG = new ExEventLog.EventTuple(2147747951U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_VALIDATION_FAILED_IGNORE_MODE_RECIPIENT = new ExEventLog.EventTuple(2147747952U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_VALIDATION_FAILED_ATTRIBUTE = new ExEventLog.EventTuple(2147747953U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_CONSTRAINT_READ_FAILED = new ExEventLog.EventTuple(2147747954U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DC_DOWN_FAULT = new ExEventLog.EventTuple(1074006132U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_DUPLICATED_SERVER = new ExEventLog.EventTuple(3221489807U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_MULTIPLE_DEFAULT_ACCEPTED_DOMAIN = new ExEventLog.EventTuple(2147747984U, 9, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED = new ExEventLog.EventTuple(2147747985U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_LDAP_SIZELIMIT_EXCEEDED = new ExEventLog.EventTuple(2147748100U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_LDAP_TIMEOUT = new ExEventLog.EventTuple(2147748181U, 5, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_SERVER_DOES_NOT_HAVE_SITE = new ExEventLog.EventTuple(3221490018U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_RUS_SERVER_LOOKUP_FAILED = new ExEventLog.EventTuple(2147748243U, 7, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_ALLOW_IMPERSONATION = new ExEventLog.EventTuple(1074006420U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_RPC_SERVER_TOO_BUSY = new ExEventLog.EventTuple(2147748245U, 9, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_ISSUE_NOTIFICATION_FAILURE = new ExEventLog.EventTuple(3221490070U, 9, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SITEMON_EVENT_CHECK_FAILED = new ExEventLog.EventTuple(3221490117U, 8, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SITEMON_EVENT_SITE_UPDATED = new ExEventLog.EventTuple(1074006471U, 8, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ADTOPO_RPC_RESOLVE_SID_FAILED = new ExEventLog.EventTuple(2147748393U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ADTOPO_RPC_FLUSH_LOCALSYSTEM_TICKET_FAILED = new ExEventLog.EventTuple(3221490218U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_READ_ROOTDSE_FAILED = new ExEventLog.EventTuple(3221490219U, 5, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADTopologyServiceStartSuccess = new ExEventLog.EventTuple(264944U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADTopologyServiceStopSuccess = new ExEventLog.EventTuple(264945U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_RODC_FOUND = new ExEventLog.EventTuple(2147748594U, 3, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_AD_NOTIFICATION_CALLBACK_TIMED_OUT = new ExEventLog.EventTuple(3221490419U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MoreThanOneOrganizationThrottlingPolicy = new ExEventLog.EventTuple(3221228374U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InitializePerformanceCountersFailed = new ExEventLog.EventTuple(3221228375U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToReadThrottlingPolicy = new ExEventLog.EventTuple(3221228376U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DynamicDistributionGroupFilterError = new ExEventLog.EventTuple(3221228377U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GlobalThrottlingPolicyMissing = new ExEventLog.EventTuple(3221228378U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MoreThanOneGlobalThrottlingPolicy = new ExEventLog.EventTuple(3221228379U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToLoadABProvider = new ExEventLog.EventTuple(3221228382U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ADTOPO_RPC_FLUSH_NETWORKSERVICE_TICKET_FAILED = new ExEventLog.EventTuple(3221490528U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToFindGALForUser = new ExEventLog.EventTuple(2147486561U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DeletedThrottlingPolicyReferenced = new ExEventLog.EventTuple(3221228386U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ExcessiveMassUserThrottling = new ExEventLog.EventTuple(3221228388U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InitializeResourceHealthPerformanceCountersFailed = new ExEventLog.EventTuple(3221228392U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CompanyMainStreamCookiePersisted = new ExEventLog.EventTuple(1073744745U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RecipientMainStreamCookiePersisted = new ExEventLog.EventTuple(1073744746U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TenantFullSyncCompanyPageTokenPersisted = new ExEventLog.EventTuple(1073744747U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TenantFullSyncRecipientPageTokenPersisted = new ExEventLog.EventTuple(1073744748U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TenantFullSyncCompanyPageTokenCleared = new ExEventLog.EventTuple(1073744749U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConcurrencyOverflowQueueTimeoutDetected = new ExEventLog.EventTuple(3221228398U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConcurrencyLongWaitInOverflowQueueDetected = new ExEventLog.EventTuple(2147486575U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConcurrencyOverflowSizeLimitReached = new ExEventLog.EventTuple(3221228400U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConcurrencyOverflowSizeWarningLimitReached = new ExEventLog.EventTuple(2147486577U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConcurrencyStartConcurrencyDoesNotMatchEnd = new ExEventLog.EventTuple(3221228402U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConcurrencyCorruptedState = new ExEventLog.EventTuple(3221228403U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConcurrencyResourceBackToHealthy = new ExEventLog.EventTuple(1073744756U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SyncObjectInvalidProxyAddressStripped = new ExEventLog.EventTuple(2147486581U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TenantFullSyncRecipientPageTokenCleared = new ExEventLog.EventTuple(1073744758U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ResourceHealthRemoteCounterReadTimedOut = new ExEventLog.EventTuple(3221228407U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ResourceHealthRemoteCounterFailed = new ExEventLog.EventTuple(3221228408U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_DeletedObjectIdLinked = new ExEventLog.EventTuple(2147486585U, 6, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MaxResourceConcurrencyReached = new ExEventLog.EventTuple(3221228410U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UMCountryListNotFound = new ExEventLog.EventTuple(3221228411U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidNotificationRequest = new ExEventLog.EventTuple(3221228412U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidNotificationRequestForDeletedObjects = new ExEventLog.EventTuple(3221228413U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ADNotificationsMaxNumberOfNotificationsPerConnection = new ExEventLog.EventTuple(1073744766U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TransportDeletedADNotificationReceived = new ExEventLog.EventTuple(1073744768U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MaximumNumberOrNotificationsForDeletedObjects = new ExEventLog.EventTuple(3221228415U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToCleanupCookies = new ExEventLog.EventTuple(2147486593U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ADHealthReport = new ExEventLog.EventTuple(1073744824U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADHealthFailed = new ExEventLog.EventTuple(3221228473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PolicyRefresh = new ExEventLog.EventTuple(1073744834U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCEnterReadLockFailed = new ExEventLog.EventTuple(2147487649U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCEnterWriteLockFailed = new ExEventLog.EventTuple(2147487650U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCEnterReadLockForOrgRemovalFailed = new ExEventLog.EventTuple(2147487651U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCEnterWriteLockForOrgDataRemovalFailed = new ExEventLog.EventTuple(2147487652U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCProvisioningCacheEnabled = new ExEventLog.EventTuple(1073745829U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCResettingWholeProvisioningCache = new ExEventLog.EventTuple(1073745830U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCClearingExpiredOrganizations = new ExEventLog.EventTuple(1073745831U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCClearingExpiredOrganizationsFinished = new ExEventLog.EventTuple(1073745832U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCResettingOrganizationData = new ExEventLog.EventTuple(1073745833U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCResettingOrganizationDataFinished = new ExEventLog.EventTuple(1073745834U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCResettingGlobalData = new ExEventLog.EventTuple(1073745835U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCResettingGlobalDataFinished = new ExEventLog.EventTuple(1073745836U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCOrganizationDataInvalidated = new ExEventLog.EventTuple(1073745837U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCGlobalDataInvalidated = new ExEventLog.EventTuple(1073745838U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCInvalidationMessageFailedBroadcast = new ExEventLog.EventTuple(3221229488U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCStartingToReceiveInvalidationMessage = new ExEventLog.EventTuple(1073745841U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCInvalidInvalidationMessageReceived = new ExEventLog.EventTuple(3221229490U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCFailedToReceiveInvalidationMessage = new ExEventLog.EventTuple(3221229491U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCUnhandledExceptionInActivity = new ExEventLog.EventTuple(3221229492U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCBadGlobalCacheKeyReceived = new ExEventLog.EventTuple(2147487669U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LowResourceHealthMeasureAverage = new ExEventLog.EventTuple(2147487670U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UserLockedOutThrottling = new ExEventLog.EventTuple(3221229495U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CannotResolveExternalDirectoryOrganizationId = new ExEventLog.EventTuple(3221229496U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UserNoLongerLockedOutThrottling = new ExEventLog.EventTuple(3221229500U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DSC_EVENT_CANNOT_CONTACT_AD_TOPOLOGY_SERVICE = new ExEventLog.EventTuple(3221491643U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SyncPropertySetStartingUpgrade = new ExEventLog.EventTuple(1073745852U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SyncPropertySetFinishedUpgrade = new ExEventLog.EventTuple(1073745854U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RelocationServiceTransientException = new ExEventLog.EventTuple(3221229503U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RelocationServicePermanentException = new ExEventLog.EventTuple(3221229504U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RelocationServiceRemoveUserExperienceMonitoringAccountError = new ExEventLog.EventTuple(3221229505U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCActivityExit = new ExEventLog.EventTuple(1073745857U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCFailedToExitActivity = new ExEventLog.EventTuple(3221229506U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCStartToExitActivity = new ExEventLog.EventTuple(1073745859U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCStartingToReceiveDiagnosticCommand = new ExEventLog.EventTuple(1073745860U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCFailedToReceiveDiagnosticCommand = new ExEventLog.EventTuple(3221229509U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCInvalidDiagnosticCommandReceived = new ExEventLog.EventTuple(3221229510U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCFailedToReceiveClientDiagnosticDommand = new ExEventLog.EventTuple(3221229511U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UpgradeServiceTransientException = new ExEventLog.EventTuple(3221229512U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpgradeServicePermanentException = new ExEventLog.EventTuple(3221229513U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CannotResolveMSAUserNetID = new ExEventLog.EventTuple(3221229514U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CannotDeleteMServEntry = new ExEventLog.EventTuple(3221491716U, 10, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_BudgetActionExceededExpectedTime = new ExEventLog.EventTuple(3221229573U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WcfClientConfigError = new ExEventLog.EventTuple(3221229575U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DnsThroubleshooterError = new ExEventLog.EventTuple(3221491720U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SessionIsScopedToRetiredTenantError = new ExEventLog.EventTuple(3221491721U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CannotContactGLS = new ExEventLog.EventTuple(3221491723U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PCProvisioningCacheInitializationFailed = new ExEventLog.EventTuple(3221229580U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerComponentStateSetOffline = new ExEventLog.EventTuple(3221229581U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerComponentStateSetOnline = new ExEventLog.EventTuple(1073745934U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CannotContactADCacheService = new ExEventLog.EventTuple(3221491727U, 12, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_UpdateServerSettingsAfterSuitabilityError = new ExEventLog.EventTuple(3221491728U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationSettingsLoadError = new ExEventLog.EventTuple(3221229585U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GetActivityContextFailed = new ExEventLog.EventTuple(3221491729U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PCSocketExceptionDisabledProvisioningCache = new ExEventLog.EventTuple(2147487762U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ReadADCacheConfigurationFailed = new ExEventLog.EventTuple(3221491731U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CallADCacheServiceFailed = new ExEventLog.EventTuple(3221491732U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ADCacheServiceUnexpectedException = new ExEventLog.EventTuple(3221491733U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WrongObjectReturned = new ExEventLog.EventTuple(3221491734U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidDatabasesCacheOnAllSites = new ExEventLog.EventTuple(1073745943U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryTaskTransientException = new ExEventLog.EventTuple(3221229592U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryTaskPermanentException = new ExEventLog.EventTuple(3221229593U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SettingOverrideValidationError = new ExEventLog.EventTuple(3221229594U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ApiNotSupported = new ExEventLog.EventTuple(3221229595U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ApiInputNotSupported = new ExEventLog.EventTuple(3221229596U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		private enum Category : short
		{
			General = 1,
			Cache,
			Topology,
			Configuration,
			LDAP,
			Validation,
			Recipient_Update_Service,
			Site_Update,
			Exchange_Topology,
			MSERV,
			GLS,
			Directory_Cache
		}

		internal enum Message : uint
		{
			DSC_EVENT_SYNC_FAILED = 3221489677U,
			DSC_EVENT_DC_DOWN = 1074006038U,
			DSC_EVENT_FIND_LOCAL_SERVER_FAILED = 3221489691U,
			DSC_EVENT_DISCOVERED_SERVERS = 1074006048U,
			DSC_EVENT_GOING_IN_SITE_DC = 1074006050U,
			DSC_EVENT_GOING_IN_SITE_GC,
			DSC_EVENT_REG_BAD_DWORD = 1074006056U,
			DSC_EVENT_REG_CDC_BAD = 2147747881U,
			DSC_EVENT_REG_CDC_DOWN,
			DSC_EVENT_REG_SERVER_BAD,
			DSC_EVENT_REG_DC = 1074006060U,
			DSC_EVENT_REG_GC,
			DSC_EVENT_REG_CDC = 1074006064U,
			DSC_EVENT_ALL_DC_DOWN = 3221489718U,
			DSC_EVENT_ALL_GC_DOWN,
			DSC_EVENT_GETHOSTBYNAME_FAILED = 2147747899U,
			DSC_EVENT_BIND_FAILED = 3221489726U,
			DSC_EVENT_SUITABILITY_CHECK_FAILED,
			DSC_EVENT_NO_SACL = 2147747904U,
			DSC_EVENT_GOT_SACL = 1074006081U,
			DSC_EVENT_FATAL_ERROR = 2147747907U,
			DSC_EVENT_BAD_OS_VERSION,
			DSC_EVENT_IMPERSONATED_CALLER,
			DSC_EVENT_DNS_DIAG_SERVER_FAILURE = 3221489734U,
			DSC_EVENT_DNS_NAME_ERROR,
			DSC_EVENT_DNS_TIMEOUT,
			DSC_EVENT_DNS_NO_ERROR = 2147747913U,
			DSC_EVENT_DNS_OTHER = 3221489738U,
			DSC_EVENT_DNS_NO_ERROR_DC_FOUND = 2147747915U,
			DSC_EVENT_DNS_NO_ERROR_DC_NOT_FOUND,
			DSC_EVENT_NO_CONNECTION = 3221489742U,
			DSC_EVENT_DC_UP = 1074006095U,
			DSC_EVENT_BASE_CONFIG_SEARCH_FAILED = 2147747920U,
			DSC_EVENT_GET_DC_FROM_DOMAIN = 1074006097U,
			DSC_EVENT_GET_DC_FROM_DOMAIN_FAILED = 3221489746U,
			DSC_EVENT_NEW_CONNECTION = 1074006099U,
			DSC_EVENT_CONNECTION_CLOSED,
			DSC_EVENT_LONG_RUNNING_OPERATION = 2147747925U,
			DSC_EVENT_NON_UNIQUE_RECIPIENT = 2147747928U,
			DSC_EVENT_ROOTDSE_READ_FAILED = 3221489753U,
			DSC_EVENT_NO_CONNECTION_TO_SERVER,
			DSC_EVENT_TOPO_INITIALIZATION_TIMEOUT,
			DSC_EVENT_PREFERRED_TOPOLOGY = 1074006109U,
			DSC_EVENT_ADAM_GET_SERVER_FROM_DOMAIN_DN = 3221489759U,
			DSC_EVENT_AD_DRIVER_PERF_INIT_FAILED = 2147747941U,
			DSC_EVENT_TOPOLOGY_UPDATE = 1074006118U,
			DSC_EVENT_AD_DRIVER_INIT,
			DSC_EVENT_WRITE_FAILED = 3221489769U,
			DSC_EVENT_RANGED_READ = 1074006122U,
			DSC_EVENT_VALIDATION_FAILED_FCO_MODE_CONFIG = 3221489771U,
			DSC_EVENT_VALIDATION_FAILED_FCO_MODE_RECIPIENT,
			DSC_EVENT_VALIDATION_FAILED_PCO_MODE_CONFIG = 2147747949U,
			DSC_EVENT_VALIDATION_FAILED_PCO_MODE_RECIPIENT,
			DSC_EVENT_VALIDATION_FAILED_IGNORE_MODE_CONFIG,
			DSC_EVENT_VALIDATION_FAILED_IGNORE_MODE_RECIPIENT,
			DSC_EVENT_VALIDATION_FAILED_ATTRIBUTE,
			DSC_EVENT_CONSTRAINT_READ_FAILED,
			DSC_EVENT_DC_DOWN_FAULT = 1074006132U,
			DSC_EVENT_DUPLICATED_SERVER = 3221489807U,
			DSC_EVENT_MULTIPLE_DEFAULT_ACCEPTED_DOMAIN = 2147747984U,
			DSC_EVENT_INTERNAL_SUITABILITY_CHECK_FAILED,
			DSC_EVENT_LDAP_SIZELIMIT_EXCEEDED = 2147748100U,
			DSC_EVENT_LDAP_TIMEOUT = 2147748181U,
			DSC_EVENT_SERVER_DOES_NOT_HAVE_SITE = 3221490018U,
			DSC_EVENT_RUS_SERVER_LOOKUP_FAILED = 2147748243U,
			DSC_EVENT_ALLOW_IMPERSONATION = 1074006420U,
			DSC_EVENT_RPC_SERVER_TOO_BUSY = 2147748245U,
			DSC_EVENT_ISSUE_NOTIFICATION_FAILURE = 3221490070U,
			SITEMON_EVENT_CHECK_FAILED = 3221490117U,
			SITEMON_EVENT_SITE_UPDATED = 1074006471U,
			ADTOPO_RPC_RESOLVE_SID_FAILED = 2147748393U,
			ADTOPO_RPC_FLUSH_LOCALSYSTEM_TICKET_FAILED = 3221490218U,
			DSC_EVENT_READ_ROOTDSE_FAILED,
			ADTopologyServiceStartSuccess = 264944U,
			ADTopologyServiceStopSuccess,
			DSC_EVENT_RODC_FOUND = 2147748594U,
			DSC_EVENT_AD_NOTIFICATION_CALLBACK_TIMED_OUT = 3221490419U,
			MoreThanOneOrganizationThrottlingPolicy = 3221228374U,
			InitializePerformanceCountersFailed,
			FailedToReadThrottlingPolicy,
			DynamicDistributionGroupFilterError,
			GlobalThrottlingPolicyMissing,
			MoreThanOneGlobalThrottlingPolicy,
			UnableToLoadABProvider = 3221228382U,
			ADTOPO_RPC_FLUSH_NETWORKSERVICE_TICKET_FAILED = 3221490528U,
			UnableToFindGALForUser = 2147486561U,
			DeletedThrottlingPolicyReferenced = 3221228386U,
			ExcessiveMassUserThrottling = 3221228388U,
			InitializeResourceHealthPerformanceCountersFailed = 3221228392U,
			CompanyMainStreamCookiePersisted = 1073744745U,
			RecipientMainStreamCookiePersisted,
			TenantFullSyncCompanyPageTokenPersisted,
			TenantFullSyncRecipientPageTokenPersisted,
			TenantFullSyncCompanyPageTokenCleared,
			ConcurrencyOverflowQueueTimeoutDetected = 3221228398U,
			ConcurrencyLongWaitInOverflowQueueDetected = 2147486575U,
			ConcurrencyOverflowSizeLimitReached = 3221228400U,
			ConcurrencyOverflowSizeWarningLimitReached = 2147486577U,
			ConcurrencyStartConcurrencyDoesNotMatchEnd = 3221228402U,
			ConcurrencyCorruptedState,
			ConcurrencyResourceBackToHealthy = 1073744756U,
			SyncObjectInvalidProxyAddressStripped = 2147486581U,
			TenantFullSyncRecipientPageTokenCleared = 1073744758U,
			ResourceHealthRemoteCounterReadTimedOut = 3221228407U,
			ResourceHealthRemoteCounterFailed,
			DeletedObjectIdLinked = 2147486585U,
			MaxResourceConcurrencyReached = 3221228410U,
			UMCountryListNotFound,
			InvalidNotificationRequest,
			InvalidNotificationRequestForDeletedObjects,
			ADNotificationsMaxNumberOfNotificationsPerConnection = 1073744766U,
			TransportDeletedADNotificationReceived = 1073744768U,
			MaximumNumberOrNotificationsForDeletedObjects = 3221228415U,
			FailedToCleanupCookies = 2147486593U,
			ADHealthReport = 1073744824U,
			ADHealthFailed = 3221228473U,
			PolicyRefresh = 1073744834U,
			PCEnterReadLockFailed = 2147487649U,
			PCEnterWriteLockFailed,
			PCEnterReadLockForOrgRemovalFailed,
			PCEnterWriteLockForOrgDataRemovalFailed,
			PCProvisioningCacheEnabled = 1073745829U,
			PCResettingWholeProvisioningCache,
			PCClearingExpiredOrganizations,
			PCClearingExpiredOrganizationsFinished,
			PCResettingOrganizationData,
			PCResettingOrganizationDataFinished,
			PCResettingGlobalData,
			PCResettingGlobalDataFinished,
			PCOrganizationDataInvalidated,
			PCGlobalDataInvalidated,
			PCInvalidationMessageFailedBroadcast = 3221229488U,
			PCStartingToReceiveInvalidationMessage = 1073745841U,
			PCInvalidInvalidationMessageReceived = 3221229490U,
			PCFailedToReceiveInvalidationMessage,
			PCUnhandledExceptionInActivity,
			PCBadGlobalCacheKeyReceived = 2147487669U,
			LowResourceHealthMeasureAverage,
			UserLockedOutThrottling = 3221229495U,
			CannotResolveExternalDirectoryOrganizationId,
			UserNoLongerLockedOutThrottling = 3221229500U,
			DSC_EVENT_CANNOT_CONTACT_AD_TOPOLOGY_SERVICE = 3221491643U,
			SyncPropertySetStartingUpgrade = 1073745852U,
			SyncPropertySetFinishedUpgrade = 1073745854U,
			RelocationServiceTransientException = 3221229503U,
			RelocationServicePermanentException,
			RelocationServiceRemoveUserExperienceMonitoringAccountError,
			PCActivityExit = 1073745857U,
			PCFailedToExitActivity = 3221229506U,
			PCStartToExitActivity = 1073745859U,
			PCStartingToReceiveDiagnosticCommand,
			PCFailedToReceiveDiagnosticCommand = 3221229509U,
			PCInvalidDiagnosticCommandReceived,
			PCFailedToReceiveClientDiagnosticDommand,
			UpgradeServiceTransientException,
			UpgradeServicePermanentException,
			CannotResolveMSAUserNetID,
			CannotDeleteMServEntry = 3221491716U,
			BudgetActionExceededExpectedTime = 3221229573U,
			WcfClientConfigError = 3221229575U,
			DnsThroubleshooterError = 3221491720U,
			SessionIsScopedToRetiredTenantError,
			CannotContactGLS = 3221491723U,
			PCProvisioningCacheInitializationFailed = 3221229580U,
			ServerComponentStateSetOffline,
			ServerComponentStateSetOnline = 1073745934U,
			CannotContactADCacheService = 3221491727U,
			UpdateServerSettingsAfterSuitabilityError,
			ConfigurationSettingsLoadError = 3221229585U,
			GetActivityContextFailed = 3221491729U,
			PCSocketExceptionDisabledProvisioningCache = 2147487762U,
			ReadADCacheConfigurationFailed = 3221491731U,
			CallADCacheServiceFailed,
			ADCacheServiceUnexpectedException,
			WrongObjectReturned,
			InvalidDatabasesCacheOnAllSites = 1073745943U,
			DirectoryTaskTransientException = 3221229592U,
			DirectoryTaskPermanentException,
			SettingOverrideValidationError,
			ApiNotSupported,
			ApiInputNotSupported
		}
	}
}
