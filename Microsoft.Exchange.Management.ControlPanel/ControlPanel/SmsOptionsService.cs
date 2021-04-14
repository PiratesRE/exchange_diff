using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class SmsOptionsService : DataSourceService, ISmsOptionsService, IEditObjectService<SmsOptions, SetSmsOptions>, IGetObjectService<SmsOptions>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TextMessagingAccount?Identity@R:Self")]
		public PowerShellResults<SmsOptions> GetObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			return base.GetObject<SmsOptions>("Get-TextMessagingAccount", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TextMessagingAccount?Identity@R:Self+Clear-TextMessagingAccount?Identity@W:Self")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TextMessagingAccount?Identity@R:Self")]
		public PowerShellResults<SmsOptions> DisableObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			PSCommand pscommand = new PSCommand();
			pscommand.AddCommand("Clear-TextMessagingAccount");
			PowerShellResults results = base.Invoke(pscommand, new Identity[]
			{
				identity
			}, null);
			PowerShellResults<SmsOptions> @object = this.GetObject(identity);
			@object.MergeErrors(results);
			return @object;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TextMessagingAccount?Identity@R:Self+Set-TextMessagingAccount?Identity@W:Self+Compare-TextMessagingVerificationCode?Identity@W:Self")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TextMessagingAccount?Identity@R:Self+Set-TextMessagingAccount?Identity@W:Self")]
		public PowerShellResults<SmsOptions> SetObject(Identity identity, SetSmsOptions properties)
		{
			properties.FaultIfNull();
			identity = Identity.FromExecutingUserId();
			if (!string.IsNullOrEmpty(properties.VerificationCode))
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Compare-TextMessagingVerificationCode");
				pscommand.AddParameter("VerificationCode", properties.VerificationCode);
				PowerShellResults results = base.Invoke(pscommand, new Identity[]
				{
					identity
				}, new BaseWebServiceParameters
				{
					ShouldContinue = properties.ShouldContinue
				});
				PowerShellResults<SmsOptions> @object = this.GetObject(identity);
				@object.MergeErrors(results);
				return @object;
			}
			if (!string.IsNullOrEmpty(properties.CountryCode) && !string.IsNullOrEmpty(properties.NotificationPhoneNumber) && !properties.NotificationPhoneNumber.StartsWith(properties.CountryCode))
			{
				properties.NotificationPhoneNumber = properties.CountryCode + properties.NotificationPhoneNumber;
			}
			return base.SetObject<SmsOptions, SetSmsOptions>("Set-TextMessagingAccount", identity, properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TextMessagingAccount?Identity@R:Self+Send-TextMessagingVerificationCode?Identity@W:Self")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-TextMessagingAccount?Identity@R:Self+Set-TextMessagingAccount?Identity@W:Self")]
		public PowerShellResults<SmsOptions> SendVerificationCode(Identity identity, SetSmsOptions properties)
		{
			properties.FaultIfNull();
			identity = Identity.FromExecutingUserId();
			properties.VerificationCode = null;
			PowerShellResults<SmsOptions> powerShellResults = this.SetObject(identity, properties);
			if (!powerShellResults.Failed)
			{
				PSCommand pscommand = new PSCommand();
				pscommand.AddCommand("Send-TextMessagingVerificationCode");
				PowerShellResults results = base.Invoke(pscommand, new Identity[]
				{
					identity
				}, new BaseWebServiceParameters
				{
					ShouldContinue = properties.ShouldContinue
				});
				powerShellResults.MergeErrors(results);
			}
			return powerShellResults;
		}

		internal const string DisableCmdlet = "Clear-TextMessagingAccount";

		internal const string GetCmdlet = "Get-TextMessagingAccount";

		internal const string SetCmdlet = "Set-TextMessagingAccount";

		internal const string SendVerificationCodeCmdlet = "Send-TextMessagingVerificationCode";

		internal const string CompareVerificationCodeCmdlet = "Compare-TextMessagingVerificationCode";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		internal const string GetObjectRole = "Get-TextMessagingAccount?Identity@R:Self";

		private const string DisableObjectRole = "Get-TextMessagingAccount?Identity@R:Self+Clear-TextMessagingAccount?Identity@W:Self";

		private const string SetObjectRole = "Get-TextMessagingAccount?Identity@R:Self+Set-TextMessagingAccount?Identity@W:Self";

		private const string CompareVerificationCodeRole = "Get-TextMessagingAccount?Identity@R:Self+Set-TextMessagingAccount?Identity@W:Self+Compare-TextMessagingVerificationCode?Identity@W:Self";

		private const string SendVerificationCodeRole = "Get-TextMessagingAccount?Identity@R:Self+Send-TextMessagingVerificationCode?Identity@W:Self";
	}
}
