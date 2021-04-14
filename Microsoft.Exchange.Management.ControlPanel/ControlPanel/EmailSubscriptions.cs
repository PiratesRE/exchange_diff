using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class EmailSubscriptions : DataSourceService, IEmailSubscriptions, INewObjectService<PimSubscription, NewSubscription>, IGetListService<EmailSubscriptionFilter, PimSubscriptionRow>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-Subscription?Mailbox@R:Self")]
		public PowerShellResults<PimSubscriptionRow> GetList(EmailSubscriptionFilter filter, SortOptions sort)
		{
			return base.GetList<PimSubscriptionRow, EmailSubscriptionFilter>("Get-Subscription", filter, sort, "EmailAddress");
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+New-Subscription?Mailbox&Force&DisplayName&Name@W:Self")]
		public PowerShellResults<PimSubscription> NewObject(NewSubscription properties)
		{
			PowerShellResults<PimSubscription> powerShellResults = base.NewObject<PimSubscription, NewSubscription>("New-Subscription", properties);
			if (powerShellResults.Succeeded)
			{
				string text = OwaOptionStrings.NewSubscriptionSucceed(powerShellResults.Output[0].VerificationFeedbackString);
				powerShellResults.Informations = new string[]
				{
					text
				};
				Util.NotifyOWAUserSettingsChanged(UserSettings.Mail);
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Remove-Subscription?Mailbox&Identity@W:Self")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			PSCommand psCommand = new PSCommand().AddCommand("Remove-Subscription");
			psCommand.AddParameters(new RemoveSubscription());
			PowerShellResults powerShellResults = base.RemoveObjects(psCommand, identities, parameters);
			if (powerShellResults != null && powerShellResults.Succeeded)
			{
				Util.NotifyOWAUserSettingsChanged(UserSettings.Mail);
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-PopSubscription?Mailbox&Identity@R:Self+Set-PopSubscription?Identity&ResendVerification@W:Self")]
		public PowerShellResults<PimSubscriptionRow> ResendPopVerificationEmail(Identity[] identities, BaseWebServiceParameters parameters)
		{
			identities.FaultIfNotExactlyOne();
			SetPopSubscription setPopSubscription = new SetPopSubscription();
			setPopSubscription.ResendVerification = true;
			PowerShellResults<PimSubscriptionRow> powerShellResults = base.SetObject<PopSubscription, SetPopSubscription, PimSubscriptionRow>("Set-PopSubscription", identities[0], setPopSubscription);
			if (powerShellResults.Succeeded)
			{
				PopSubscriptions popSubscriptions = new PopSubscriptions();
				PowerShellResults<PopSubscription> @object = popSubscriptions.GetObject(identities[0]);
				if (!@object.SucceededWithValue)
				{
					throw new FaultException(OwaOptionStrings.SubscriptionProcessingError);
				}
				string verificationFeedbackString = @object.Output[0].VerificationFeedbackString;
				if (verificationFeedbackString != null)
				{
					powerShellResults.Informations = new string[]
					{
						verificationFeedbackString
					};
				}
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-ImapSubscription?Mailbox&Identity@R:Self+Set-ImapSubscription?Identity&ResendVerification@W:Self")]
		public PowerShellResults<PimSubscriptionRow> ResendImapVerificationEmail(Identity[] identities, BaseWebServiceParameters parameters)
		{
			identities.FaultIfNotExactlyOne();
			SetImapSubscription setImapSubscription = new SetImapSubscription();
			setImapSubscription.ResendVerification = true;
			PowerShellResults<PimSubscriptionRow> powerShellResults = base.SetObject<ImapSubscription, SetImapSubscription, PimSubscriptionRow>("Set-ImapSubscription", identities[0], setImapSubscription);
			if (powerShellResults.Succeeded)
			{
				ImapSubscriptions imapSubscriptions = new ImapSubscriptions();
				PowerShellResults<ImapSubscription> @object = imapSubscriptions.GetObject(identities[0]);
				if (!@object.SucceededWithValue)
				{
					throw new FaultException(OwaOptionStrings.SubscriptionProcessingError);
				}
				string verificationFeedbackString = @object.Output[0].VerificationFeedbackString;
				if (verificationFeedbackString != null)
				{
					powerShellResults.Informations = new string[]
					{
						verificationFeedbackString
					};
				}
			}
			return powerShellResults;
		}

		private const string Noun = "Subscription";

		internal const string GetCmdlet = "Get-Subscription";

		internal const string GetImapCmdlet = "Get-ImapSubscription";

		internal const string GetPopCmdlet = "Get-PopSubscription";

		internal const string NewCmdlet = "New-Subscription";

		internal const string RemoveCmdlet = "Remove-Subscription";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetListRole = "MultiTenant+Get-Subscription?Mailbox@R:Self";

		private const string NewObjectRole = "MultiTenant+New-Subscription?Mailbox&Force&DisplayName&Name@W:Self";

		private const string RemoveObjectsRole = "MultiTenant+Remove-Subscription?Mailbox&Identity@W:Self";

		private const string ResendPopVerificationRole = "MultiTenant+Get-PopSubscription?Mailbox&Identity@R:Self+Set-PopSubscription?Identity&ResendVerification@W:Self";

		private const string ResendImapVerificationRole = "MultiTenant+Get-ImapSubscription?Mailbox&Identity@R:Self+Set-ImapSubscription?Identity&ResendVerification@W:Self";
	}
}
