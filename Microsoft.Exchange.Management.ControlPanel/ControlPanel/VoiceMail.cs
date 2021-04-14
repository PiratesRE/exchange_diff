using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks.UM;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class VoiceMail : DataSourceService, IVoiceMail, IEditObjectService<GetVoiceMailConfiguration, SetVoiceMailConfiguration>, IGetObjectService<GetVoiceMailConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self")]
		public PowerShellResults<GetVoiceMailConfiguration> GetObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			PowerShellResults<GetVoiceMailConfiguration> @object = base.GetObject<GetVoiceMailConfiguration>("Get-UMMailbox", identity);
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

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self+Set-UMMailbox?Identity@W:Self+Set-UMMailboxPIN?Identity@W:Self")]
		public PowerShellResults<GetVoiceMailConfiguration> SetObject(Identity identity, SetVoiceMailConfiguration properties)
		{
			properties.FaultIfNull();
			identity = Identity.FromExecutingUserId();
			PowerShellResults<GetVoiceMailConfiguration> powerShellResults = new PowerShellResults<GetVoiceMailConfiguration>();
			PowerShellResults<GetVoiceMailConfiguration> @object = this.GetObject(identity);
			powerShellResults.MergeErrors<GetVoiceMailConfiguration>(@object);
			if (powerShellResults.Failed)
			{
				return powerShellResults;
			}
			powerShellResults.MergeErrors<UMMailboxPin>(base.SetObject<UMMailboxPin, SetVoiceMailPIN>("Set-UMMailboxPIN", identity, properties.SetVoiceMailPIN));
			if (powerShellResults.Failed)
			{
				return powerShellResults;
			}
			properties.ReturnObjectType = ReturnObjectTypes.Full;
			powerShellResults.MergeAll(base.SetObject<GetVoiceMailConfiguration, SetVoiceMailConfiguration>("Set-UMMailbox", identity, properties));
			if (powerShellResults.SucceededWithValue)
			{
				GetVoiceMailConfiguration value = powerShellResults.Value;
				RbacPrincipal.Current.RbacConfiguration.ExecutingUserIsUmConfigured = value.IsConfigured;
				if (this.IsPhoneVerified(value.PhoneNumber, value) && !string.IsNullOrEmpty(properties.PhoneProviderId))
				{
					PowerShellResults<SmsOptions> results = this.SetTextMessagingAccount(identity, value.PhoneNumber, value.PhoneProviderId, value);
					powerShellResults.MergeErrors<SmsOptions>(results);
				}
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self+Set-UMMailbox?Identity@W:Self")]
		public PowerShellResults<GetVoiceMailConfiguration> ClearSettings(Identity identity)
		{
			return this.SetObject(identity, new SetVoiceMailConfiguration
			{
				PhoneNumber = string.Empty,
				PhoneProviderId = string.Empty,
				SMSNotificationOption = "None",
				PinlessAccessToVoiceMailEnabled = false
			});
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self+Set-TextMessagingAccount?Identity@W:Self+Send-TextMessagingVerificationCode?Identity@W:Self+Compare-TextMessagingVerificationCode?Identity@W:Self")]
		public PowerShellResults<GetVoiceMailConfiguration> RegisterPhone(Identity identity, SetVoiceMailConfiguration properties)
		{
			properties.FaultIfNull();
			identity = Identity.FromExecutingUserId();
			PowerShellResults<GetVoiceMailConfiguration> @object = this.GetObject(identity);
			if (@object.Failed || string.IsNullOrEmpty(properties.PhoneNumber))
			{
				return @object;
			}
			GetVoiceMailConfiguration value = @object.Value;
			value.VerificationCodeRequired = !this.IsPhoneVerified(properties.PhoneNumber, value);
			if (string.IsNullOrEmpty(properties.PhoneProviderId))
			{
				properties.PhoneProviderId = value.PhoneProviderId;
			}
			if (value.VerificationCodeRequired)
			{
				if (!string.IsNullOrEmpty(properties.VerificationCode))
				{
					@object.MergeErrors(this.ComparePasscode(identity, properties.VerificationCode));
					value.VerificationCodeRequired = !@object.Succeeded;
				}
				else
				{
					PowerShellResults<SmsOptions> powerShellResults = this.SetTextMessagingAccount(identity, properties.PhoneNumber, properties.PhoneProviderId, value);
					@object.MergeErrors<SmsOptions>(powerShellResults);
					if (powerShellResults.SucceededWithValue)
					{
						value.SmsOptions = powerShellResults.Value;
						value.VerificationCodeRequired = !powerShellResults.Value.NotificationPhoneNumberVerified;
						if (value.VerificationCodeRequired)
						{
							@object.MergeErrors(this.SendVerificationCode(identity));
						}
					}
				}
			}
			return this.ClearOutputOnFailure(@object);
		}

		private PowerShellResults ComparePasscode(Identity identity, string passcode)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Compare-TextMessagingVerificationCode");
			pscommand.AddParameter("Identity", identity);
			pscommand.AddParameter("VerificationCode", passcode);
			return base.Invoke(pscommand);
		}

		private PowerShellResults<SmsOptions> SetTextMessagingAccount(Identity identity, string phoneNumber, string phoneProviderId, GetVoiceMailConfiguration voiceMailConfig)
		{
			SetSmsOptions setSmsOptions = new SetSmsOptions();
			setSmsOptions.CountryCode = "+" + voiceMailConfig.CountryOrRegionCode;
			setSmsOptions.CountryRegionId = voiceMailConfig.CountryOrRegionId;
			setSmsOptions.MobileOperatorId = phoneProviderId;
			setSmsOptions.NotificationPhoneNumber = setSmsOptions.CountryCode + phoneNumber;
			setSmsOptions.VerificationCode = null;
			PowerShellResults<SmsOptions> powerShellResults = new PowerShellResults<SmsOptions>();
			PowerShellResults results = base.SetObject<SmsOptions, SetSmsOptions>("Set-TextMessagingAccount", identity, setSmsOptions);
			powerShellResults.MergeErrors(results);
			if (powerShellResults.Succeeded)
			{
				powerShellResults.MergeAll(base.GetObject<SmsOptions>("Get-TextMessagingAccount", identity));
			}
			return powerShellResults;
		}

		private PowerShellResults SendVerificationCode(Identity identity)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Send-TextMessagingVerificationCode");
			pscommand.AddParameter("Identity", identity);
			return base.Invoke(pscommand);
		}

		private bool IsPhoneVerified(string phoneNumber, GetVoiceMailConfiguration config)
		{
			return config.SmsOptions.NotificationPhoneNumberVerified && string.Equals(phoneNumber, config.SMSNotificationPhoneNumber, StringComparison.InvariantCultureIgnoreCase);
		}

		private PowerShellResults<GetVoiceMailConfiguration> ClearOutputOnFailure(PowerShellResults<GetVoiceMailConfiguration> results)
		{
			if (results.Failed)
			{
				results.Output = new GetVoiceMailConfiguration[0];
			}
			return results;
		}

		internal const string GetCmdlet = "Get-UMMailbox";

		internal const string SetCmdlet = "Set-UMMailbox";

		internal const string GetSmsAccountCmdlet = "Get-TextMessagingAccount";

		internal const string SetSmsAccountCmdlet = "Set-TextMessagingAccount";

		internal const string SetUMMailboxPINCmdlet = "Set-UMMailboxPIN";

		internal const string SendSmsPasscodeCmdlet = "Send-TextMessagingVerificationCode";

		internal const string CompareSmsPasscodeCmdlet = "Compare-TextMessagingVerificationCode";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetObjectRole = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self";

		private const string SetObjectRole = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self+Set-UMMailbox?Identity@W:Self+Set-UMMailboxPIN?Identity@W:Self";

		private const string ClearSettingsRole = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self+Set-UMMailbox?Identity@W:Self";

		private const string RegisterPhoneRole = "Get-UMMailbox?Identity@R:Self+Get-TextMessagingAccount?Identity@R:Self+Set-TextMessagingAccount?Identity@W:Self+Send-TextMessagingVerificationCode?Identity@W:Self+Compare-TextMessagingVerificationCode?Identity@W:Self";
	}
}
