using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class SendAddressSetting : DataSourceService, ISendAddressSetting, IEditObjectService<SendAddressConfiguration, SetSendAddressDefaultConfiguration>, IGetObjectService<SendAddressConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-MailboxMessageConfiguration?Identity@R:Self")]
		public PowerShellResults<SendAddressConfiguration> GetObject(Identity identity)
		{
			identity = Identity.FromExecutingUserId();
			PowerShellResults<SendAddressConfiguration> @object = base.GetObject<SendAddressConfiguration>("Get-MailboxMessageConfiguration", identity);
			if (@object.Failed)
			{
				return @object;
			}
			SendAddress sendAddress = new SendAddress();
			PowerShellResults<SendAddressRow> list = sendAddress.GetList(new SendAddressFilter
			{
				AddressId = (@object.Value.SendAddressDefault ?? string.Empty),
				IgnoreNullOrEmpty = false
			}, null);
			if (list.Failed)
			{
				@object.MergeErrors<SendAddressRow>(list);
				return @object;
			}
			@object.Value.SendAddressDefault = list.Value.Value;
			return @object;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-MailboxMessageConfiguration?Identity@R:Self+MultiTenant+Set-MailboxMessageConfiguration?Identity@W:Self")]
		public PowerShellResults<SendAddressConfiguration> SetObject(Identity identity, SetSendAddressDefaultConfiguration properties)
		{
			identity = Identity.FromExecutingUserId();
			PowerShellResults<SendAddressConfiguration> powerShellResults;
			lock (RbacPrincipal.Current.OwaOptionsLock)
			{
				powerShellResults = base.SetObject<SendAddressConfiguration, SetSendAddressDefaultConfiguration>("Set-MailboxMessageConfiguration", identity, properties);
			}
			if (powerShellResults.Succeeded)
			{
				Util.NotifyOWAUserSettingsChanged(UserSettings.Mail);
			}
			return powerShellResults;
		}

		internal const string GetCmdlet = "Get-MailboxMessageConfiguration";

		internal const string SetCmdlet = "Set-MailboxMessageConfiguration";

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		private const string GetObjectRole = "MultiTenant+Get-MailboxMessageConfiguration?Identity@R:Self";

		private const string SetObjectRole = "MultiTenant+Get-MailboxMessageConfiguration?Identity@R:Self+MultiTenant+Set-MailboxMessageConfiguration?Identity@W:Self";
	}
}
