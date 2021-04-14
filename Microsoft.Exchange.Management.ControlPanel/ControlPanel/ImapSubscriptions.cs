using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ImapSubscriptions : DataSourceService, IImapSubscriptions, INewObjectService<PimSubscriptionRow, NewImapSubscription>, IEditObjectService<ImapSubscription, SetImapSubscription>, IGetObjectService<ImapSubscription>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-ImapSubscription?Mailbox&Identity@R:Self")]
		public PowerShellResults<ImapSubscription> GetObject(Identity identity)
		{
			PSCommand psCommand = new PSCommand().AddCommand("Get-ImapSubscription");
			psCommand.AddParameters(new GetImapSubscription());
			return base.GetObject<ImapSubscription>(psCommand, identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+New-ImapSubscription?Mailbox&Name@W:Self")]
		public PowerShellResults<PimSubscriptionRow> NewObject(NewImapSubscription properties)
		{
			PowerShellResults<PimSubscriptionRow> powerShellResults = base.NewObject<PimSubscriptionRow, NewImapSubscription>("New-ImapSubscription", properties);
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

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-ImapSubscription?Mailbox&Identity@R:Self+Set-ImapSubscription?Mailbox&Identity@W:Self")]
		public PowerShellResults<ImapSubscription> SetObject(Identity identity, SetImapSubscription properties)
		{
			PowerShellResults<ImapSubscription> powerShellResults = base.SetObject<ImapSubscription, SetImapSubscription>("Set-ImapSubscription", identity, properties);
			if (powerShellResults.Succeeded)
			{
				PowerShellResults<ImapSubscription> @object = this.GetObject(identity);
				if (@object.Succeeded)
				{
					string verificationFeedbackString = @object.Output[0].VerificationFeedbackString;
					if (verificationFeedbackString != null)
					{
						string text = OwaOptionStrings.NewSubscriptionSucceed(verificationFeedbackString);
						powerShellResults.Informations = new string[]
						{
							text
						};
					}
				}
				Util.NotifyOWAUserSettingsChanged(UserSettings.Mail);
			}
			return powerShellResults;
		}

		internal const string GetCmdlet = "Get-ImapSubscription";

		internal const string NewCmdlet = "New-ImapSubscription";

		internal const string SetCmdlet = "Set-ImapSubscription";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string Noun = "ImapSubscription";

		private const string GetObjectRole = "MultiTenant+Get-ImapSubscription?Mailbox&Identity@R:Self";

		private const string NewObjectRole = "MultiTenant+New-ImapSubscription?Mailbox&Name@W:Self";

		private const string SetObjectRole = "MultiTenant+Get-ImapSubscription?Mailbox&Identity@R:Self+Set-ImapSubscription?Mailbox&Identity@W:Self";
	}
}
