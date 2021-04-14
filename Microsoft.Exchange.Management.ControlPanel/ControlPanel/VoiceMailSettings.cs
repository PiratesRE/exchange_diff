using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class VoiceMailSettings : DataSourceService, IVoiceMailSettings, IEditObjectService<GetVoiceMailSettings, SetVoiceMailSettings>, IGetObjectService<GetVoiceMailSettings>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self")]
		public PowerShellResults<GetVoiceMailSettings> GetObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			PowerShellResults<GetVoiceMailSettings> @object = base.GetObject<GetVoiceMailSettings>("Get-UMMailbox", identity);
			if (@object.SucceededWithValue)
			{
				PowerShellResults<SmsOptions> object2 = base.GetObject<SmsOptions>("Get-TextMessagingAccount", identity);
				@object.MergeErrors<SmsOptions>(object2);
				if (object2.SucceededWithValue)
				{
					@object.Value.SmsOptions = object2.Value;
				}
			}
			else if (@object.Failed && @object.ErrorRecords[0].Exception is ManagementObjectNotFoundException)
			{
				@object.ErrorRecords = new ErrorRecord[0];
			}
			return @object;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self+Set-UMMailbox?Identity@W:Self")]
		public PowerShellResults<GetVoiceMailSettings> SetObject(Identity identity, SetVoiceMailSettings properties)
		{
			identity = Identity.FromExecutingUserId();
			return base.SetObject<GetVoiceMailSettings, SetVoiceMailSettings>("Set-UMMailbox", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self+Set-UMMailboxPIN?Identity@W:Self")]
		public PowerShellResults ResetPIN(Identity identity)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Set-UMMailboxPIN");
			pscommand.AddParameter("Identity", Identity.FromExecutingUserId());
			pscommand.AddParameter("PinExpired", true);
			return base.Invoke(pscommand);
		}

		internal const string GetCmdlet = "Get-UMMailbox";

		internal const string GetSmsAccountCmdlet = "Get-TextMessagingAccount";

		internal const string SetCmdlet = "Set-UMMailbox";

		internal const string SetPINCmdlet = "Set-UMMailboxPIN";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetObjectRole = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self";

		private const string SetObjectRole = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self+Set-UMMailbox?Identity@W:Self";

		private const string ResetPINRole = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self+Set-UMMailboxPIN?Identity@W:Self";
	}
}
