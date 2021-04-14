using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Extension;
using Microsoft.Exchange.Management.MapiTasks;
using Microsoft.Exchange.Management.MapiTasks.Presentation;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Management.SecureMail;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Management.Supervision;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Management.PSDirectInvoke
{
	internal class CmdletTaskFactory
	{
		private CmdletTaskFactory()
		{
			foreach (object obj in Enum.GetValues(typeof(TaskModuleKey)))
			{
				TaskModuleKey key = (TaskModuleKey)obj;
				TaskModuleFactory.DisableModule(key);
			}
			TaskModuleFactory.EnableModule(TaskModuleKey.RunspaceServerSettingsInit);
			TaskModuleFactory.EnableModule(TaskModuleKey.RunspaceServerSettingsFinalize);
			string configStringValue = AppConfigLoader.GetConfigStringValue("PSDirectInvokeEnabledModules", string.Empty);
			string[] array = configStringValue.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string value in array)
			{
				TaskModuleKey key2;
				if (Enum.TryParse<TaskModuleKey>(value, true, out key2))
				{
					TaskModuleFactory.EnableModule(key2);
				}
			}
		}

		public static CmdletTaskFactory Instance
		{
			get
			{
				if (CmdletTaskFactory.instance == null)
				{
					lock (CmdletTaskFactory.syncLock)
					{
						if (CmdletTaskFactory.instance == null)
						{
							CmdletTaskFactory.instance = new CmdletTaskFactory();
						}
					}
				}
				return CmdletTaskFactory.instance;
			}
		}

		public PSLocalTask<EnableApp, object> CreateEnableAppTask(ExchangePrincipal executingUser)
		{
			EnableApp task = new EnableApp();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Enable-App", "Identity");
			return new PSLocalTask<EnableApp, object>(task);
		}

		public PSLocalTask<DisableApp, object> CreateDisableAppTask(ExchangePrincipal executingUser)
		{
			DisableApp task = new DisableApp();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Disable-App", "Identity");
			return new PSLocalTask<DisableApp, object>(task);
		}

		public PSLocalTask<RemoveApp, object> CreateRemoveAppTask(ExchangePrincipal executingUser)
		{
			RemoveApp task = new RemoveApp();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Remove-App", "Identity");
			return new PSLocalTask<RemoveApp, object>(task);
		}

		public PSLocalTask<NewGroupMailbox, GroupMailbox> CreateNewGroupMailboxTask(ExchangePrincipal executingUser)
		{
			NewGroupMailbox task = new NewGroupMailbox();
			this.InitializeTaskToExecuteInMode(executingUser, task, "New-GroupMailbox", "GroupMailbox");
			return new PSLocalTask<NewGroupMailbox, GroupMailbox>(task);
		}

		public PSLocalTask<NewMailbox, Mailbox> CreateNewMonitoringMailboxTask(OrganizationId organizationId, SmtpAddress msaUserMemberName)
		{
			NewMailbox task = new NewMailbox();
			this.InitializeTaskToExecuteInMode(organizationId, null, null, msaUserMemberName, task, "New-Mailbox", "Monitoring");
			return new PSLocalTask<NewMailbox, Mailbox>(task);
		}

		public PSLocalTask<NewMailbox, Mailbox> CreateNewMailboxTask(OrganizationId organizationId, SmtpAddress msaUserMemberName, NetID msaUserNetID)
		{
			NewMailbox task = new NewMailbox();
			this.InitializeTaskToExecuteInMode(organizationId, null, msaUserNetID.ToString(), msaUserMemberName, task, "New-Mailbox", "User");
			return new PSLocalTask<NewMailbox, Mailbox>(task);
		}

		public PSLocalTask<RemoveMailbox, Mailbox> CreateRemoveMailboxTask(ExchangePrincipal executingUser, string parameterSet)
		{
			RemoveMailbox task = new RemoveMailbox();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Remove-Mailbox", parameterSet);
			return new PSLocalTask<RemoveMailbox, Mailbox>(task);
		}

		public PSLocalTask<RemoveMailbox, Mailbox> CreateRemoveMailboxTask(OrganizationId organizationId, SmtpAddress msaUserMemberName)
		{
			RemoveMailbox task = new RemoveMailbox();
			this.InitializeTaskToExecuteInMode(organizationId, null, null, msaUserMemberName, task, "Remove-Mailbox", "Identity");
			return new PSLocalTask<RemoveMailbox, Mailbox>(task);
		}

		public PSLocalTask<NewSyncRequest, SyncRequest> CreateNewSyncRequestTask(ExchangePrincipal executingUser, string parameterSet)
		{
			NewSyncRequest task = new NewSyncRequest();
			this.InitializeTaskToExecuteInMode(executingUser, task, "New-SyncRequest", parameterSet);
			return new PSLocalTask<NewSyncRequest, SyncRequest>(task);
		}

		public PSLocalTask<GetCalendarNotification, CalendarNotification> CreateGetCalendarNotificationTask(ExchangePrincipal executingUser)
		{
			GetCalendarNotification task = new GetCalendarNotification();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-CalendarNotification", "User");
			return new PSLocalTask<GetCalendarNotification, CalendarNotification>(task);
		}

		public PSLocalTask<SetCalendarNotification, CalendarNotification> CreateSetCalendarNotificationTask(ExchangePrincipal executingUser)
		{
			SetCalendarNotification task = new SetCalendarNotification();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-CalendarNotification", "User");
			return new PSLocalTask<SetCalendarNotification, CalendarNotification>(task);
		}

		public PSLocalTask<GetCalendarProcessing, CalendarConfiguration> CreateGetCalendarProcessingTask(ExchangePrincipal executingUser)
		{
			GetCalendarProcessing task = new GetCalendarProcessing();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-CalendarProcessing", "User");
			return new PSLocalTask<GetCalendarProcessing, CalendarConfiguration>(task);
		}

		public PSLocalTask<SetCalendarProcessing, CalendarConfiguration> CreateSetCalendarProcessingTask(ExchangePrincipal executingUser)
		{
			SetCalendarProcessing task = new SetCalendarProcessing();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-CalendarProcessing", "User");
			return new PSLocalTask<SetCalendarProcessing, CalendarConfiguration>(task);
		}

		public PSLocalTask<GetCASMailbox, CASMailbox> CreateGetCASMailboxTask(ExchangePrincipal executingUser)
		{
			GetCASMailbox task = new GetCASMailbox();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-CASMailbox", "User");
			return new PSLocalTask<GetCASMailbox, CASMailbox>(task);
		}

		public PSLocalTask<SetCASMailbox, CASMailbox> CreateSetCASMailboxTask(ExchangePrincipal executingUser)
		{
			SetCASMailbox task = new SetCASMailbox();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-CASMailbox", "User");
			return new PSLocalTask<SetCASMailbox, CASMailbox>(task);
		}

		public PSLocalTask<GetConnectSubscription, ConnectSubscriptionProxy> CreateGetConnectSubscriptionTask(ExchangePrincipal executingUser)
		{
			GetConnectSubscription task = new GetConnectSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-ConnectSubscription", "Identity");
			return new PSLocalTask<GetConnectSubscription, ConnectSubscriptionProxy>(task);
		}

		public PSLocalTask<NewConnectSubscription, ConnectSubscriptionProxy> CreateNewConnectSubscriptionTask(ExchangePrincipal executingUser, string parameterSet)
		{
			NewConnectSubscription task = new NewConnectSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "New-ConnectSubscription", parameterSet);
			return new PSLocalTask<NewConnectSubscription, ConnectSubscriptionProxy>(task);
		}

		public PSLocalTask<RemoveConnectSubscription, object> CreateRemoveConnectSubscriptionTask(ExchangePrincipal executingUser)
		{
			RemoveConnectSubscription task = new RemoveConnectSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Remove-ConnectSubscription", "Identity");
			return new PSLocalTask<RemoveConnectSubscription, object>(task);
		}

		public PSLocalTask<SetConnectSubscription, object> CreateSetConnectSubscriptionTask(ExchangePrincipal executingUser, string parameterSet)
		{
			SetConnectSubscription task = new SetConnectSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-ConnectSubscription", parameterSet);
			return new PSLocalTask<SetConnectSubscription, object>(task);
		}

		public PSLocalTask<GetHotmailSubscription, HotmailSubscriptionProxy> CreateGetHotmailSubscriptionTask(ExchangePrincipal executingUser)
		{
			GetHotmailSubscription task = new GetHotmailSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-HotmailSubscription", "Identity");
			return new PSLocalTask<GetHotmailSubscription, HotmailSubscriptionProxy>(task);
		}

		public PSLocalTask<SetHotmailSubscription, HotmailSubscriptionProxy> CreateSetHotmailSubscriptionTask(ExchangePrincipal executingUser)
		{
			SetHotmailSubscription task = new SetHotmailSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-HotmailSubscription", "Identity");
			return new PSLocalTask<SetHotmailSubscription, HotmailSubscriptionProxy>(task);
		}

		public PSLocalTask<GetImapSubscription, IMAPSubscriptionProxy> CreateGetImapSubscriptionTask(ExchangePrincipal executingUser)
		{
			GetImapSubscription task = new GetImapSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-ImapSubscription", "Identity");
			return new PSLocalTask<GetImapSubscription, IMAPSubscriptionProxy>(task);
		}

		public PSLocalTask<NewImapSubscription, IMAPSubscriptionProxy> CreateNewImapSubscriptionTask(ExchangePrincipal executingUser)
		{
			NewImapSubscription task = new NewImapSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "New-ImapSubscription", "Identity");
			return new PSLocalTask<NewImapSubscription, IMAPSubscriptionProxy>(task);
		}

		public PSLocalTask<SetImapSubscription, IMAPSubscriptionProxy> CreateSetImapSubscriptionTask(ExchangePrincipal executingUser)
		{
			SetImapSubscription task = new SetImapSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-ImapSubscription", "Identity");
			return new PSLocalTask<SetImapSubscription, IMAPSubscriptionProxy>(task);
		}

		public PSLocalTask<ImportContactList, ImportContactListResult> CreateImportContactListTask(ExchangePrincipal executingUser)
		{
			ImportContactList task = new ImportContactList();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Import-ContactList", "Identity");
			return new PSLocalTask<ImportContactList, ImportContactListResult>(task);
		}

		public PSLocalTask<DisableInboxRule, InboxRule> CreateDisableInboxRuleTask(ExchangePrincipal executingUser)
		{
			DisableInboxRule task = new DisableInboxRule();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Disable-InboxRule", "Identity");
			return new PSLocalTask<DisableInboxRule, InboxRule>(task);
		}

		public PSLocalTask<EnableInboxRule, InboxRule> CreateEnableInboxRuleTask(ExchangePrincipal executingUser)
		{
			EnableInboxRule task = new EnableInboxRule();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Enable-InboxRule", "Identity");
			return new PSLocalTask<EnableInboxRule, InboxRule>(task);
		}

		public PSLocalTask<GetInboxRule, InboxRule> CreateGetInboxRuleTask(ExchangePrincipal executingUser)
		{
			GetInboxRule task = new GetInboxRule();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-InboxRule", "Identity");
			return new PSLocalTask<GetInboxRule, InboxRule>(task);
		}

		public PSLocalTask<NewInboxRule, InboxRule> CreateNewInboxRuleTask(ExchangePrincipal executingUser)
		{
			NewInboxRule task = new NewInboxRule();
			this.InitializeTaskToExecuteInMode(executingUser, task, "New-InboxRule", "Identity");
			return new PSLocalTask<NewInboxRule, InboxRule>(task);
		}

		public PSLocalTask<RemoveInboxRule, InboxRule> CreateRemoveInboxRuleTask(ExchangePrincipal executingUser)
		{
			RemoveInboxRule task = new RemoveInboxRule();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Remove-InboxRule", "Identity");
			return new PSLocalTask<RemoveInboxRule, InboxRule>(task);
		}

		public PSLocalTask<SetInboxRule, InboxRule> CreateSetInboxRuleTask(ExchangePrincipal executingUser)
		{
			SetInboxRule task = new SetInboxRule();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-InboxRule", "Identity");
			return new PSLocalTask<SetInboxRule, InboxRule>(task);
		}

		public PSLocalTask<GetMailboxCalendarConfiguration, MailboxCalendarConfiguration> CreateGetMailboxCalendarConfigurationTask(ExchangePrincipal executingUser)
		{
			GetMailboxCalendarConfiguration task = new GetMailboxCalendarConfiguration();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-MailboxCalendarConfiguration", "User");
			return new PSLocalTask<GetMailboxCalendarConfiguration, MailboxCalendarConfiguration>(task);
		}

		public PSLocalTask<GetMailboxAutoReplyConfiguration, MailboxAutoReplyConfiguration> CreateGetMailboxAutoReplyConfigurationTask(ExchangePrincipal executingUser)
		{
			GetMailboxAutoReplyConfiguration task = new GetMailboxAutoReplyConfiguration();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-MailboxAutoReplyConfiguration", "User");
			return new PSLocalTask<GetMailboxAutoReplyConfiguration, MailboxAutoReplyConfiguration>(task);
		}

		public PSLocalTask<SetMailboxAutoReplyConfiguration, MailboxAutoReplyConfiguration> CreateSetMailboxAutoReplyConfigurationTask(ExchangePrincipal executingUser)
		{
			SetMailboxAutoReplyConfiguration task = new SetMailboxAutoReplyConfiguration();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-MailboxAutoReplyConfiguration", "User");
			return new PSLocalTask<SetMailboxAutoReplyConfiguration, MailboxAutoReplyConfiguration>(task);
		}

		public PSLocalTask<SetMailboxCalendarConfiguration, MailboxCalendarConfiguration> CreateSetMailboxCalendarConfigurationTask(ExchangePrincipal executingUser)
		{
			SetMailboxCalendarConfiguration task = new SetMailboxCalendarConfiguration();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-MailboxCalendarConfiguration", "User");
			return new PSLocalTask<SetMailboxCalendarConfiguration, MailboxCalendarConfiguration>(task);
		}

		public PSLocalTask<GetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration> CreateGetMailboxJunkEmailConfigurationTask(ExchangePrincipal executingUser)
		{
			GetMailboxJunkEmailConfiguration task = new GetMailboxJunkEmailConfiguration();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-MailboxJunkEmailConfiguration", "Identity");
			return new PSLocalTask<GetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration>(task);
		}

		public PSLocalTask<SetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration> CreateSetMailboxJunkEmailConfigurationTask(ExchangePrincipal executingUser)
		{
			SetMailboxJunkEmailConfiguration task = new SetMailboxJunkEmailConfiguration();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-MailboxJunkEmailConfiguration", "Identity");
			return new PSLocalTask<SetMailboxJunkEmailConfiguration, MailboxJunkEmailConfiguration>(task);
		}

		public PSLocalTask<GetMailboxRegionalConfiguration, MailboxRegionalConfiguration> CreateGetMailboxRegionalConfigurationTask(ExchangePrincipal executingUser)
		{
			GetMailboxRegionalConfiguration task = new GetMailboxRegionalConfiguration();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-MailboxRegionalConfiguration", "Identity");
			return new PSLocalTask<GetMailboxRegionalConfiguration, MailboxRegionalConfiguration>(task);
		}

		public PSLocalTask<SetMailboxRegionalConfiguration, MailboxRegionalConfiguration> CreateSetMailboxRegionalConfigurationTask(ExchangePrincipal executingUser)
		{
			SetMailboxRegionalConfiguration task = new SetMailboxRegionalConfiguration();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-MailboxRegionalConfiguration", "Identity");
			return new PSLocalTask<SetMailboxRegionalConfiguration, MailboxRegionalConfiguration>(task);
		}

		public PSLocalTask<GetMailboxMessageConfiguration, MailboxMessageConfiguration> CreateGetMailboxMessageConfigurationTask(ExchangePrincipal executingUser)
		{
			GetMailboxMessageConfiguration task = new GetMailboxMessageConfiguration();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-MailboxMessageConfiguration", "User");
			return new PSLocalTask<GetMailboxMessageConfiguration, MailboxMessageConfiguration>(task);
		}

		public PSLocalTask<SetMailboxMessageConfiguration, MailboxMessageConfiguration> CreateSetMailboxMessageConfigurationTask(ExchangePrincipal executingUser)
		{
			SetMailboxMessageConfiguration task = new SetMailboxMessageConfiguration();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-MailboxMessageConfiguration", "User");
			return new PSLocalTask<SetMailboxMessageConfiguration, MailboxMessageConfiguration>(task);
		}

		public PSLocalTask<GetMessageCategory, MessageCategory> CreateGetMessageCategoryTask(ExchangePrincipal executingUser)
		{
			GetMessageCategory task = new GetMessageCategory();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-MessageCategory", "Identity");
			return new PSLocalTask<GetMessageCategory, MessageCategory>(task);
		}

		public PSLocalTask<GetMessageClassification, MessageClassification> CreateGetMessageClassificationTask(ExchangePrincipal executingUser)
		{
			GetMessageClassification task = new GetMessageClassification();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-MessageClassification", "Identity");
			return new PSLocalTask<GetMessageClassification, MessageClassification>(task);
		}

		public PSLocalTask<GetMailboxStatistics, MailboxStatistics> CreateGetMailboxStatisticsTask(ExchangePrincipal executingUser)
		{
			GetMailboxStatistics task = new GetMailboxStatistics();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-MailboxStatistics", "Identity");
			return new PSLocalTask<GetMailboxStatistics, MailboxStatistics>(task);
		}

		public PSLocalTask<GetPopSubscription, PopSubscriptionProxy> CreateGetPopSubscriptionTask(ExchangePrincipal executingUser)
		{
			GetPopSubscription task = new GetPopSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-PopSubscription", "Identity");
			return new PSLocalTask<GetPopSubscription, PopSubscriptionProxy>(task);
		}

		public PSLocalTask<NewPopSubscription, PopSubscriptionProxy> CreateNewPopSubscriptionTask(ExchangePrincipal executingUser)
		{
			NewPopSubscription task = new NewPopSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "New-PopSubscription", "Identity");
			return new PSLocalTask<NewPopSubscription, PopSubscriptionProxy>(task);
		}

		public PSLocalTask<SetPopSubscription, PopSubscriptionProxy> CreateSetPopSubscriptionTask(ExchangePrincipal executingUser)
		{
			SetPopSubscription task = new SetPopSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-PopSubscription", "Identity");
			return new PSLocalTask<SetPopSubscription, PopSubscriptionProxy>(task);
		}

		public PSLocalTask<GetRetentionPolicyTag, RetentionPolicyTag> CreateGetRetentionPolicyTagTask(ExchangePrincipal executingUser)
		{
			GetRetentionPolicyTag task = new GetRetentionPolicyTag();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-RetentionPolicyTag", "Identity");
			return new PSLocalTask<GetRetentionPolicyTag, RetentionPolicyTag>(task);
		}

		public PSLocalTask<SetRetentionPolicyTag, RetentionPolicyTag> CreateSetRetentionPolicyTagTask(ExchangePrincipal executingUser)
		{
			SetRetentionPolicyTag task = new SetRetentionPolicyTag();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-RetentionPolicyTag", "ParameterSetMailboxTask");
			return new PSLocalTask<SetRetentionPolicyTag, RetentionPolicyTag>(task);
		}

		public PSLocalTask<GetSendAddress, SendAddress> CreateGetSendAddressTask(ExchangePrincipal executingUser)
		{
			GetSendAddress task = new GetSendAddress();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-SendAddress", "Identity");
			return new PSLocalTask<GetSendAddress, SendAddress>(task);
		}

		public PSLocalTask<GetSubscription, PimSubscriptionProxy> CreateGetSubscriptionTask(ExchangePrincipal executingUser)
		{
			GetSubscription task = new GetSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-Subscription", "Identity");
			return new PSLocalTask<GetSubscription, PimSubscriptionProxy>(task);
		}

		public PSLocalTask<NewSubscription, PimSubscriptionProxy> CreateNewSubscriptionTask(ExchangePrincipal executingUser)
		{
			NewSubscription task = new NewSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "New-Subscription", "Identity");
			return new PSLocalTask<NewSubscription, PimSubscriptionProxy>(task);
		}

		public PSLocalTask<RemoveSubscription, PimSubscriptionProxy> CreateRemoveSubscriptionTask(ExchangePrincipal executingUser)
		{
			RemoveSubscription task = new RemoveSubscription();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Remove-Subscription", "Identity");
			return new PSLocalTask<RemoveSubscription, PimSubscriptionProxy>(task);
		}

		public PSLocalTask<GetUser, User> CreateGetUserTask(ExchangePrincipal executingUser)
		{
			GetUser task = new GetUser();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-User", "Identity");
			return new PSLocalTask<GetUser, User>(task);
		}

		public PSLocalTask<SetUser, User> CreateSetUserTask(ExchangePrincipal executingUser)
		{
			SetUser task = new SetUser();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-User", "Identity");
			return new PSLocalTask<SetUser, User>(task);
		}

		public PSLocalTask<GetGroupMailbox, GroupMailbox> CreateGetGroupMailboxTask(ExchangePrincipal executingUser)
		{
			GetGroupMailbox task = new GetGroupMailbox();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-GroupMailbox", "GroupMailbox");
			return new PSLocalTask<GetGroupMailbox, GroupMailbox>(task);
		}

		public PSLocalTask<GetSyncRequest, SyncRequest> CreateGetSyncRequestTask(ExchangePrincipal executingUser)
		{
			GetSyncRequest task = new GetSyncRequest();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-SyncRequest", "Identity");
			return new PSLocalTask<GetSyncRequest, SyncRequest>(task);
		}

		public PSLocalTask<GetSyncRequestStatistics, SyncRequestStatistics> CreateGetSyncRequestStatisticsTask(ExchangePrincipal executingUser)
		{
			GetSyncRequestStatistics task = new GetSyncRequestStatistics();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-SyncRequestStatistics", "Identity");
			return new PSLocalTask<GetSyncRequestStatistics, SyncRequestStatistics>(task);
		}

		public PSLocalTask<SetGroupMailbox, object> CreateSetGroupMailboxTask(ExchangePrincipal executingUser)
		{
			SetGroupMailbox task = new SetGroupMailbox();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-GroupMailbox", "GroupMailbox");
			return new PSLocalTask<SetGroupMailbox, object>(task);
		}

		public PSLocalTask<GetMailbox, Mailbox> CreateGetMailboxTask(ExchangePrincipal executingUser, string parameterSet)
		{
			GetMailbox task = new GetMailbox();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-Mailbox", parameterSet);
			return new PSLocalTask<GetMailbox, Mailbox>(task);
		}

		public PSLocalTask<SetMailbox, object> CreateSetMailboxTask(ExchangePrincipal executingUser, string parameterSet)
		{
			SetMailbox task = new SetMailbox();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-Mailbox", parameterSet);
			return new PSLocalTask<SetMailbox, object>(task);
		}

		public PSLocalTask<SetMailbox, object> CreateSetMailboxTask(OrganizationId monitoringOrganizationId)
		{
			SetMailbox task = new SetMailbox();
			this.InitializeTaskToExecuteInMode(monitoringOrganizationId, null, null, SmtpAddress.Empty, task, "Set-Mailbox", "Identity");
			return new PSLocalTask<SetMailbox, object>(task);
		}

		public PSLocalTask<SetSyncRequest, object> CreateSetSyncRequestTask(ExchangePrincipal executingUser)
		{
			SetSyncRequest task = new SetSyncRequest();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-SyncRequest", "Identity");
			return new PSLocalTask<SetSyncRequest, object>(task);
		}

		public PSLocalTask<RemoveGroupMailbox, object> CreateRemoveGroupMailboxTask(ExchangePrincipal executingUser)
		{
			RemoveGroupMailbox task = new RemoveGroupMailbox();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Remove-GroupMailbox", "GroupMailbox");
			return new PSLocalTask<RemoveGroupMailbox, object>(task);
		}

		public PSLocalTask<RemoveSyncRequest, object> CreateRemoveSyncRequestTask(ExchangePrincipal executingUser)
		{
			RemoveSyncRequest task = new RemoveSyncRequest();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Remove-SyncRequest", "Identity");
			return new PSLocalTask<RemoveSyncRequest, object>(task);
		}

		public PSLocalTask<GetMobileDeviceStatistics, MobileDeviceConfiguration> CreateGetMobileDeviceStatisticsTask(ExchangePrincipal executingUser)
		{
			GetMobileDeviceStatistics task = new GetMobileDeviceStatistics();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-MobileDeviceStatistics", "Mailbox");
			return new PSLocalTask<GetMobileDeviceStatistics, MobileDeviceConfiguration>(task);
		}

		public PSLocalTask<RemoveMobileDevice, MobileDevice> CreateRemoveMobileDeviceTask(ExchangePrincipal executingUser)
		{
			RemoveMobileDevice task = new RemoveMobileDevice();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Remove-MobileDevice", "Identity");
			return new PSLocalTask<RemoveMobileDevice, MobileDevice>(task);
		}

		public PSLocalTask<ClearMobileDevice, MobileDevice> CreateClearMobileDeviceTask(ExchangePrincipal executingUser)
		{
			ClearMobileDevice task = new ClearMobileDevice();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Clear-MobileDevice", "Identity");
			return new PSLocalTask<ClearMobileDevice, MobileDevice>(task);
		}

		public PSLocalTask<ClearTextMessagingAccount, object> CreateClearTextMessagingAccountTask(ExchangePrincipal executingUser)
		{
			ClearTextMessagingAccount task = new ClearTextMessagingAccount();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Clear-TextMessagingAccount", "Identity");
			return new PSLocalTask<ClearTextMessagingAccount, object>(task);
		}

		public PSLocalTask<GetTextMessagingAccount, TextMessagingAccount> CreateGetTextMessagingAccountTask(ExchangePrincipal executingUser)
		{
			GetTextMessagingAccount task = new GetTextMessagingAccount();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-TextMessagingAccount", "Identity");
			return new PSLocalTask<GetTextMessagingAccount, TextMessagingAccount>(task);
		}

		public PSLocalTask<SetTextMessagingAccount, object> CreateSetTextMessagingAccountTask(ExchangePrincipal executingUser)
		{
			SetTextMessagingAccount task = new SetTextMessagingAccount();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Set-TextMessagingAccount", "Identity");
			return new PSLocalTask<SetTextMessagingAccount, object>(task);
		}

		public PSLocalTask<CompareTextMessagingVerificationCode, object> CreateCompareTextMessagingVerificationCodeTask(ExchangePrincipal executingUser)
		{
			CompareTextMessagingVerificationCode task = new CompareTextMessagingVerificationCode();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Compare-TextMessagingVerificationCode", "Identity");
			return new PSLocalTask<CompareTextMessagingVerificationCode, object>(task);
		}

		public PSLocalTask<SendTextMessagingVerificationCode, object> CreateSendTextMessagingVerificationCodeTask(ExchangePrincipal executingUser)
		{
			SendTextMessagingVerificationCode task = new SendTextMessagingVerificationCode();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Send-TextMessagingVerificationCode", "Identity");
			return new PSLocalTask<SendTextMessagingVerificationCode, object>(task);
		}

		public PSLocalTask<GetSupervisionPolicy, SupervisionPolicy> CreateGetSupervisionPolicyTask(ExchangePrincipal executingUser)
		{
			GetSupervisionPolicy task = new GetSupervisionPolicy();
			this.InitializeTaskToExecuteInMode(executingUser, task, "Get-SupervisionPolicy", "Identity");
			return new PSLocalTask<GetSupervisionPolicy, SupervisionPolicy>(task);
		}

		private void InitializeTaskToExecuteInMode(OrganizationId executingUserOrganizationId, ADObjectId executingUserId, string executingUserIdentityName, SmtpAddress executingWindowsLiveId, Task task, string cmdletName, string parameterSet)
		{
			task.CurrentTaskContext.UserInfo = new TaskUserInfo(executingUserOrganizationId, executingUserOrganizationId, executingUserId, executingUserIdentityName, executingWindowsLiveId);
			TaskInvocationInfo taskInvocationInfo = TaskInvocationInfo.CreateForDirectTaskInvocation(cmdletName);
			taskInvocationInfo.IsDebugOn = false;
			taskInvocationInfo.IsVerboseOn = false;
			if (parameterSet != null)
			{
				taskInvocationInfo.ParameterSetName = parameterSet;
			}
			task.CurrentTaskContext.InvocationInfo = taskInvocationInfo;
			PSLocalSessionState sessionState = new PSLocalSessionState();
			task.CurrentTaskContext.SessionState = sessionState;
			ExchangePropertyContainer.InitExchangePropertyContainer(sessionState, executingUserOrganizationId, executingUserId);
		}

		private void InitializeTaskToExecuteInMode(ExchangePrincipal executingUser, Task task, string cmdletName, string parameterSet)
		{
			SmtpAddress executingWindowsLiveId = SmtpAddress.Empty;
			IUserPrincipal userPrincipal = executingUser as IUserPrincipal;
			if (userPrincipal != null)
			{
				executingWindowsLiveId = userPrincipal.WindowsLiveId;
			}
			this.InitializeTaskToExecuteInMode(executingUser.MailboxInfo.OrganizationId, executingUser.ObjectId, executingUser.ObjectId.Name, executingWindowsLiveId, task, cmdletName, parameterSet);
		}

		private const string PSDirectInvokeEnabledModulesConfigName = "PSDirectInvokeEnabledModules";

		private static readonly object syncLock = new object();

		private static volatile CmdletTaskFactory instance;
	}
}
