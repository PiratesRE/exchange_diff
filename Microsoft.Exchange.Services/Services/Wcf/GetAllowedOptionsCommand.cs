using System;
using System.Collections.Generic;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class GetAllowedOptionsCommand : ServiceCommand<GetAllowedOptionsResponse>
	{
		public GetAllowedOptionsCommand(CallContext callContext) : base(callContext)
		{
		}

		protected override GetAllowedOptionsResponse InternalExecute()
		{
			OptionsRbacEngine optionsRbacEngine = new OptionsRbacEngine(base.CallContext, true);
			List<string> list = new List<string>();
			foreach (GetAllowedOptionsCommand.OptionQuery optionQuery in GetAllowedOptionsCommand.allOptionQueries)
			{
				if (optionsRbacEngine.EvaluateExpression(optionQuery.RbacQueryExpression))
				{
					list.Add(optionQuery.OptionName);
				}
			}
			return new GetAllowedOptionsResponse
			{
				AllowedOptions = list.ToArray()
			};
		}

		private static List<GetAllowedOptionsCommand.OptionQuery> allOptionQueries = new List<GetAllowedOptionsCommand.OptionQuery>
		{
			new GetAllowedOptionsCommand.OptionQuery("AutomaticReplies", "Mailbox+OWA+MyBaseOptions+Get-MailboxAutoReplyConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("BlockSenders", "JunkEmail+Mailbox+!LiveID+MyBaseOptions+Get-MailboxJunkEmailConfiguration@R:Self,JunkEmail+Mailbox+!ClosedCampus+MyBaseOptions+Get-MailboxJunkEmailConfiguration@R:Self,LiveID+ClosedCampus+Get-SupervisionListEntry@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("CalendarAppearance", "Calendar+Mailbox+MyBaseOptions+Get-MailboxCalendarConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("CalendarNotifications", "TextMessaging+Calendar+RemindersAndNotifications+Mailbox+MyTextMessaging+MachineToPersonTextingOnly+Get-CalendarNotification@R:Self+Get-TextMessagingAccount@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("CalendarProcessing", "Calendar+Mailbox+MyBaseOptions+Get-CalendarProcessing@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("CalendarReminders", "Calendar+Mailbox+MyBaseOptions+Get-MailboxCalendarConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("CallAnswering", "Mailbox+MyBaseOptions+UMIntegration+UM+MyVoiceMail+Get-UMMailbox@R:Self+Set-UMMailbox%PhoneNumber@W:Self+Rules+OWA+Get-UMCallAnsweringRule@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("Clutter", "True"),
			new GetAllowedOptionsCommand.OptionQuery("ConnectedAccounts", "Mailbox+LiveID+MyMailSubscriptions+Get-Subscription@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("Conversations", "Mailbox+MyBaseOptions+OWA+Get-MailboxMessageConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("DeliveryReports", "Mailbox+Get-Recipient@R:Self+Search-MessageTrackingReport@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("Facebook", "FacebookEnabled+Get-ConnectSubscription+New-ConnectSubscription?Facebook@W:Self"),
			new GetAllowedOptionsCommand.OptionQuery("Forwarding", "Mailbox+MyBaseOptions+LiveID+Get-Mailbox@R:Self+Set-Mailbox?DeliverToMailboxAndForward&ForwardingSmtpAddress@W:Self"),
			new GetAllowedOptionsCommand.OptionQuery("GeneralNotifications", "True"),
			new GetAllowedOptionsCommand.OptionQuery("Greetings", "Mailbox+MyBaseOptions+UMIntegration+OWA+MailboxFullAccess+UM+UMConfigured"),
			new GetAllowedOptionsCommand.OptionQuery("ImportContacts", "Mailbox+MyBaseOptions+LiveID+Import-ContactList@W:Self"),
			new GetAllowedOptionsCommand.OptionQuery("InboxRules", "Rules+Mailbox+OWA+Get-InboxRule@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("InstalledApps", "MyBaseOptions+Get-App@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("JunkReporting", "True"),
			new GetAllowedOptionsCommand.OptionQuery("LinkedIn", "LinkedInEnabled+Get-ConnectSubscription"),
			new GetAllowedOptionsCommand.OptionQuery("MailDisplayOptions", "True"),
			new GetAllowedOptionsCommand.OptionQuery("MailSignatures", "Signatures+Mailbox+OWA+MyBaseOptions+Get-MailboxMessageConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("ManageApps", "MyBaseOptions+Get-App@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("MarkAsRead", "Mailbox+OWA+MyBaseOptions+Get-MailboxMessageConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("MessageFormat", "Mailbox+OWA+MyBaseOptions+Get-MailboxMessageConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("MessageList", "True"),
			new GetAllowedOptionsCommand.OptionQuery("MessageOptions", "Mailbox+OWA+MyBaseOptions+Get-MailboxMessageConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("MobileDevice", "Mailbox+MyBaseOptions+MobileDevices+Get-MobileDeviceStatistics@R:Self+Get-CASMailbox@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("MyAccount", "Mailbox+MyBaseOptions+Get-User@R:Self+Get-Mailbox@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("OfflineFolders", "True"),
			new GetAllowedOptionsCommand.OptionQuery("OfflineSettings", "True"),
			new GetAllowedOptionsCommand.OptionQuery("OwaVersion", "True"),
			new GetAllowedOptionsCommand.OptionQuery("Password", "Mailbox+MyBaseOptions+ChangePassword+OWA+!LiveID+!Impersonated+Get-Mailbox@R:Self+Set-Mailbox@W:Self"),
			new GetAllowedOptionsCommand.OptionQuery("PlayOnPhone", "Mailbox+MyBaseOptions+UMIntegration+OWA+MailboxFullAccess+UM+UMConfigured"),
			new GetAllowedOptionsCommand.OptionQuery("PopAndImap", "Mailbox+MyBaseOptions+Get-User@R:Self+Get-Mailbox@R:Self+Get-CASMailbox?ProtocolSettings@R:Self+!PopImapDisabled"),
			new GetAllowedOptionsCommand.OptionQuery("ReadingPane", "Mailbox+OWA+MyBaseOptions+Get-MailboxMessageConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("ReadReceipts", "Mailbox+OWA+MyBaseOptions+Get-MailboxMessageConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("Regional", "Mailbox+MyBaseOptions+Get-MailboxRegionalConfiguration@R:Self+Get-MailboxCalendarConfiguration@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("RetentionPolicies", "Mailbox+MyRetentionPolicies+RetentionPolicy+Get-RetentionPolicyTag@C:OrganizationConfig"),
			new GetAllowedOptionsCommand.OptionQuery("Smime", "True"),
			new GetAllowedOptionsCommand.OptionQuery("TextMessaging", "Mailbox+MyBaseOptions+TextMessaging+Calendar+RemindersAndNotifications+Get-TextMessagingAccount@R:Self,Mailbox+MyBaseOptions+TextMessaging+UMIntegration+UM+Get-TextMessagingAccount@R:Self,Mailbox+MyBaseOptions+TextMessaging+Rules+Get-TextMessagingAccount@R:Self"),
			new GetAllowedOptionsCommand.OptionQuery("Theme", "True"),
			new GetAllowedOptionsCommand.OptionQuery("VoiceAccess", "Mailbox+MyBaseOptions+UMIntegration+OWA+MailboxFullAccess+UM+UMConfigured"),
			new GetAllowedOptionsCommand.OptionQuery("VoiceMailPinReset", "Mailbox+MyBaseOptions+UMIntegration+OWA+MailboxFullAccess+UM+UMConfigured"),
			new GetAllowedOptionsCommand.OptionQuery("VoiceMailPreview", "Mailbox+MyBaseOptions+UMIntegration+OWA+MailboxFullAccess+UM+UMConfigured")
		};

		private class OptionQuery
		{
			public string OptionName { get; private set; }

			public string RbacQueryExpression { get; private set; }

			public OptionQuery(string optionName, string rbacQueryExpression)
			{
				this.OptionName = optionName;
				this.RbacQueryExpression = rbacQueryExpression;
			}
		}
	}
}
