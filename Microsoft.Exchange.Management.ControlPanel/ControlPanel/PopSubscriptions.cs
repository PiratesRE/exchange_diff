using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class PopSubscriptions : DataSourceService, IPopSubscriptions, INewObjectService<PimSubscriptionRow, NewPopSubscription>, IEditObjectService<PopSubscription, SetPopSubscription>, IGetObjectService<PopSubscription>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-PopSubscription?Mailbox&Identity@R:Self")]
		public PowerShellResults<PopSubscription> GetObject(Identity identity)
		{
			PSCommand psCommand = new PSCommand().AddCommand("Get-PopSubscription");
			psCommand.AddParameters(new GetPopSubscription());
			return base.GetObject<PopSubscription>(psCommand, identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+New-PopSubscription?Mailbox&Name@W:Self")]
		public PowerShellResults<PimSubscriptionRow> NewObject(NewPopSubscription properties)
		{
			PowerShellResults<PimSubscriptionRow> powerShellResults = base.NewObject<PimSubscriptionRow, NewPopSubscription>("New-PopSubscription", properties);
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

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-PopSubscription?Mailbox&Identity@R:Self+Set-PopSubscription?Mailbox&Identity@W:Self")]
		public PowerShellResults<PopSubscription> SetObject(Identity identity, SetPopSubscription properties)
		{
			PowerShellResults<PopSubscription> powerShellResults = base.SetObject<PopSubscription, SetPopSubscription>("Set-PopSubscription", identity, properties);
			if (powerShellResults.Succeeded)
			{
				PowerShellResults<PopSubscription> @object = this.GetObject(identity);
				if (@object.Succeeded)
				{
					string verificationFeedbackString = @object.Output[0].VerificationFeedbackString;
					if (verificationFeedbackString != null)
					{
						string text = OwaOptionStrings.SetSubscriptionSucceed(verificationFeedbackString);
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

		internal const string GetCmdlet = "Get-PopSubscription";

		internal const string NewCmdlet = "New-PopSubscription";

		internal const string SetCmdlet = "Set-PopSubscription";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string Noun = "PopSubscription";

		private const string GetObjectRole = "MultiTenant+Get-PopSubscription?Mailbox&Identity@R:Self";

		private const string NewObjectRole = "MultiTenant+New-PopSubscription?Mailbox&Name@W:Self";

		private const string SetObjectRole = "MultiTenant+Get-PopSubscription?Mailbox&Identity@R:Self+Set-PopSubscription?Mailbox&Identity@W:Self";
	}
}
