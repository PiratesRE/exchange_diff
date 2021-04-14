using System;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class OwaUserConfigurationLogUtilities
	{
		internal static void LogAndResetPerfCapture(OwaUserConfigurationLogType logType, StorePerformanceCountersCapture countersCapture, bool restart)
		{
			if (countersCapture == null)
			{
				return;
			}
			StorePerformanceCounters storePerformanceCounters = countersCapture.Stop();
			OwaUserConfigurationLogMetadata owaUserConfigurationLogMetadata;
			OwaUserConfigurationLogMetadata owaUserConfigurationLogMetadata2;
			OwaUserConfigurationLogMetadata owaUserConfigurationLogMetadata3;
			OwaUserConfigurationLogMetadata owaUserConfigurationLogMetadata4;
			OwaUserConfigurationLogMetadata owaUserConfigurationLogMetadata5;
			switch (logType)
			{
			case OwaUserConfigurationLogType.UserOptionsLoad:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.UserOptionsLoadTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.UserOptionsLoadRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.UserOptionsLoadRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.UserOptionsLoadRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.UserOptionsLoadCPUTime;
				break;
			case OwaUserConfigurationLogType.WorkingHours:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.WorkingHoursTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.WorkingHoursRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.WorkingHoursRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.WorkingHoursRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.WorkingHoursCPUTime;
				break;
			case OwaUserConfigurationLogType.LoadReminderOptions:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.ReminderTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.ReminderRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.ReminderRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.ReminderRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.ReminderCPUTime;
				break;
			case OwaUserConfigurationLogType.SessionSettingsMisc:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.SessionSettingsMiscTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.SessionSettingsMiscRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.SessionSettingsMiscRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.SessionSettingsMiscRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.SessionSettingsMiscCPUTime;
				break;
			case OwaUserConfigurationLogType.SessionSettingsMessageSize:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.SessionSettingsMessageSizeTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.SessionSettingsMessageSizeRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.SessionSettingsMessageSizeRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.SessionSettingsMessageSizeRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.SessionSettingsMessageSizeCPUTime;
				break;
			case OwaUserConfigurationLogType.SessionSettingsIsPublicLogon:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.SessionSettingsIsPublicLogonTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.SessionSettingsPublicLogonRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.SessionSettingsPublicLogonRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.SessionSettingsPublicLogonRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.SessionSettingsPublicLogonCPUTime;
				break;
			case OwaUserConfigurationLogType.TeamMailbox:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.TeamMailboxTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.TeamMailboxRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.TeamMailboxRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.TeamMailboxRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.TeamMailboxCPUTime;
				break;
			case OwaUserConfigurationLogType.GetOWAMiniRecipient:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.MiniRecipientTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.MiniRecipientRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.MiniRecipientRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.MiniRecipientRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.MiniRecipientCPUTime;
				break;
			case OwaUserConfigurationLogType.SetDefaultFolderMapping:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.DefaultFolderTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.DefaultFolderRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.DefaultFolderRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.DefaultFolderRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.DefaultFolderCPUTime;
				break;
			case OwaUserConfigurationLogType.UMClient:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.UMClientTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.UMClientRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.UMClientRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.UMClientRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.UMClientCPUTime;
				break;
			case OwaUserConfigurationLogType.IsDatacenterMode:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.IsDatacenterModeTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.IsDatacenterModeRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.IsDatacenterModeRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.IsDatacenterModeRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.IsDatacenterModeCPUTime;
				break;
			case OwaUserConfigurationLogType.OwaViewStateConfiguration:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.ViewStateTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.ViewStateRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.ViewStateRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.ViewStateRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.ViewStateCPUTime;
				break;
			case OwaUserConfigurationLogType.GetMailTipsLargeAudienceThreshold:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.MailTipsTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.MailTipsRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.MailTipsRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.MailTipsRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.MailTipsCPUTime;
				break;
			case OwaUserConfigurationLogType.GetRetentionPolicyTags:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.RetentionPolicyTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.RetentionPolicyRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.RetentionPolicyRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.RetentionPolicyRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.RetentionPolicyCPUTime;
				break;
			case OwaUserConfigurationLogType.GetMasterCategoryListType:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.MasterCategoryListTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.MasterCategoryListRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.MasterCategoryListRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.MasterCategoryListRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.MasterCategoryListCPUTime;
				break;
			case OwaUserConfigurationLogType.GetMaxRecipientsPerMessage:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.MaxRecipientsTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.MaxRecipientsRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.MaxRecipientsRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.MaxRecipientsRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.MaxRecipientsCPUTime;
				break;
			case OwaUserConfigurationLogType.PolicySettings:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.PolicySettingsTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.PolicySettingsRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.PolicySettingsRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.PolicySettingsRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.PolicySettingsCPUTime;
				break;
			case OwaUserConfigurationLogType.SessionSettings:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.SessionSettingsTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.SessionSettingsRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.SessionSettingsRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.SessionSettingsRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.SessionSettingsCPUTime;
				break;
			case OwaUserConfigurationLogType.ConfigContext:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.ConfigContextTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.ConfigContextRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.ConfigContextRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.ConfigContextRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.ConfigContextCPUTime;
				break;
			case OwaUserConfigurationLogType.SegmentationSettings:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.SegmentationSettingsTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.SegmentationSettingsRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.SegmentationSettingsRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.SegmentationSettingsRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.SegmentationSettingsCPUTime;
				break;
			case OwaUserConfigurationLogType.AttachmentPolicy:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.AttachmentPolicyTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.AttachmentPolicyRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.AttachmentPolicyRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.AttachmentPolicyRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.AttachmentPolicyCPUTime;
				break;
			case OwaUserConfigurationLogType.PlacesWeather:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.PlacesWeatherTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.PlacesWeatherRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.PlacesWeatherRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.PlacesWeatherRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.PlacesWeatherCPUTime;
				break;
			case OwaUserConfigurationLogType.DefaultPublicFolderMailbox:
				owaUserConfigurationLogMetadata = OwaUserConfigurationLogMetadata.DefaultPublicFolderMailboxTime;
				owaUserConfigurationLogMetadata2 = OwaUserConfigurationLogMetadata.DefaultPublicFolderMailboxRpcCount;
				owaUserConfigurationLogMetadata3 = OwaUserConfigurationLogMetadata.DefaultPublicFolderMailboxRpcLatency;
				owaUserConfigurationLogMetadata4 = OwaUserConfigurationLogMetadata.DefaultPublicFolderMailboxRpcLatencyOnStore;
				owaUserConfigurationLogMetadata5 = OwaUserConfigurationLogMetadata.DefaultPublicFolderMailboxCPUTime;
				break;
			default:
				return;
			}
			RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
			if (getRequestDetailsLogger != null)
			{
				getRequestDetailsLogger.Set(owaUserConfigurationLogMetadata, storePerformanceCounters.ElapsedMilliseconds);
				getRequestDetailsLogger.Set(owaUserConfigurationLogMetadata5, storePerformanceCounters.Cpu);
				getRequestDetailsLogger.Set(owaUserConfigurationLogMetadata2, storePerformanceCounters.RpcCount);
				getRequestDetailsLogger.Set(owaUserConfigurationLogMetadata3, storePerformanceCounters.RpcLatency);
				getRequestDetailsLogger.Set(owaUserConfigurationLogMetadata4, storePerformanceCounters.RpcLatencyOnStore);
			}
			if (restart)
			{
				countersCapture.Restart();
			}
		}

		internal static void LogAndResetPerfCapture(OwaUserConfigurationLogType logType, StorePerformanceCountersCapture countersCapture, bool restart, string errorString)
		{
			OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(logType, countersCapture, restart);
			RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
			if (getRequestDetailsLogger != null && !string.IsNullOrWhiteSpace(errorString) && logType == OwaUserConfigurationLogType.DefaultPublicFolderMailbox)
			{
				getRequestDetailsLogger.Set(OwaUserConfigurationLogMetadata.DefaultPublicFolderMailboxError, errorString);
			}
		}
	}
}
