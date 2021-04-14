using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Constants
	{
		public const string TeamMailboxSyncSessionClientString = "Client=TeamMailbox;Action=CommitChanges;Interactive=False";

		public const string TeamMailboxGetDiagnosticsSessionClientString = "Client=TeamMailbox;Action=GetDiagnostics;Interactive=False";

		public const string TeamMailboxSendWelcomeMessageSessionClientString = "Client=TeamMailbox;Action=SendWelcomeMessageToSiteMailbox;Interactive=False";

		public const string TeamMailboxSendNotificationSessionClientString = "Client=TeamMailbox;Action=Send_Notification";

		public const string TeamMailboxCmdletInitialLogonSessionClientString = "Client=TeamMailbox;Action=CmdletInitialLogon";

		public const string DocumentLibSynchronizerMetadataName = "DocumentLibSynchronizerConfigurations";

		public const string SiteSychronizerMetadataName = "SiteSynchronizerConfigurations";

		public const string MembershipSychronizerMetadataName = "MembershipSynchronizerConfigurations";

		public const string MaintenanceSychronizerMetadataName = "MaintenanceSynchronizerConfigurations";

		public const string AssistantMetadataName = "SiteMailboxAssistantConfigurations";

		public const string DocumentSyncLogConfigurationName = "TeamMailboxDocumentLastSyncCycleLog";

		public const string MembershipSyncLogConfigurationName = "TeamMailboxMembershipLastSyncCycleLog";

		public const string MaintenanceSyncLogConfigurationName = "TeamMailboxMaintenanceLastSyncCycleLog";

		public const string AssistantLogConfigurationName = "SiteMailboxAssistantCycleLog";

		public const string FirstAttemptedSyncTime = "FirstAttemptedSyncTime";

		public const string LastAttemptedSyncTime = "LastAttemptedSyncTime";

		public const string LastSuccessfulSyncTime = "LastSuccessfulSyncTime";

		public const string LastSyncFailure = "LastSyncFailure";

		public const string LastFailedSyncTime = "LastFailedSyncTime";

		public const string LastFailedSyncEmailTime = "LastFailedSyncEmailTime";

		public static readonly TimeSpan MailboxSemaphoreTimeout = TimeSpan.FromSeconds(30.0);
	}
}
