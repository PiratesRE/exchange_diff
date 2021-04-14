using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MobileDevices : DataSourceService, IMobileDevices, IGetListService<MobileDeviceFilter, MobileDeviceRow>, IGetObjectService<MobileDevice>, IGetObjectForListService<MobileDeviceRow>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileDeviceStatistics?Mailbox&ShowRecoveryPassword&ActiveSync@R:Organization+Get-CASMailbox?Identity@R:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileDeviceStatistics?Mailbox&ShowRecoveryPassword&ActiveSync@R:Self+Get-CASMailbox?Identity@R:Self")]
		public PowerShellResults<MobileDeviceRow> GetList(MobileDeviceFilter filter, SortOptions sort)
		{
			PSCommand psCommand = new PSCommand().AddCommand("Get-CASMailbox").AddParameter("ActiveSyncDebugLogging", new SwitchParameter(true));
			PowerShellResults<CASMailbox> @object = base.GetObject<CASMailbox>(psCommand, (filter == null) ? Identity.FromExecutingUserId() : filter.Mailbox);
			bool isLoggingRunning = false;
			if (@object.HasValue)
			{
				isLoggingRunning = @object.Value.ActiveSyncDebugLogging;
			}
			PowerShellResults<MobileDeviceRow> list = base.GetList<MobileDeviceRow, MobileDeviceFilter>("Get-MobileDeviceStatistics", filter, sort);
			if (@object.HasValue)
			{
				foreach (MobileDeviceRow mobileDeviceRow in list.Output)
				{
					mobileDeviceRow.IsLoggingRunning = isLoggingRunning;
				}
			}
			return list;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileDeviceStatistics?Identity@R:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileDeviceStatistics?Identity@R:Self")]
		public PowerShellResults<MobileDevice> GetObject(Identity identity)
		{
			return base.GetObject<MobileDevice>("Get-MobileDeviceStatistics", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileDeviceStatistics?Identity&ShowRecoveryPassword@R:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileDeviceStatistics?Identity&ShowRecoveryPassword@R:Self")]
		public PowerShellResults<MobileDeviceRow> GetObjectForList(Identity identity)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Get-MobileDeviceStatistics");
			pscommand.AddParameter("ShowRecoveryPassword", true);
			return base.GetObject<MobileDeviceRow>(pscommand, identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Set-CASMailbox?Identity&ActiveSyncDebugLogging@W:Self")]
		public PowerShellResults StartLogging(Identity[] identities, BaseWebServiceParameters parameters)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Set-CASMailbox");
			pscommand.AddParameter("Identity", Identity.FromExecutingUserId());
			pscommand.AddParameter("ActiveSyncDebugLogging", true);
			return base.Invoke(pscommand);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Set-CASMailbox?Identity&ActiveSyncDebugLogging@W:Self+Get-MobileDeviceStatistics?Identity&GetMailboxLog@R:Self")]
		public PowerShellResults StopAndRetrieveLog(Identity[] identities, BaseWebServiceParameters parameters)
		{
			PSCommand pscommand = new PSCommand().AddCommand("Set-CASMailbox");
			pscommand.AddParameter("Identity", Identity.FromExecutingUserId());
			pscommand.AddParameter("ActiveSyncDebugLogging", false);
			PowerShellResults powerShellResults = base.Invoke(pscommand);
			PSCommand psCommand = new PSCommand().AddCommand("Get-MobileDeviceStatistics").AddParameter("GetMailboxLog", new SwitchParameter(true));
			return powerShellResults.MergeErrors(base.Invoke(psCommand, identities, parameters));
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Clear-MobileDevice?Identity&Cancel@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Clear-MobileDevice?Identity&Cancel@W:Self")]
		public PowerShellResults<MobileDeviceRow> BlockOrWipeDevice(Identity[] identities, BaseWebServiceParameters parameters)
		{
			PSCommand psCommand = new PSCommand().AddCommand("Clear-MobileDevice").AddParameter("Cancel", new SwitchParameter(false));
			return base.InvokeAndGetObject<MobileDeviceRow>(psCommand, identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Clear-MobileDevice?Identity&Cancel@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Clear-MobileDevice?Identity&Cancel@W:Self")]
		public PowerShellResults<MobileDeviceRow> UnBlockOrCancelWipeDevice(Identity[] identities, BaseWebServiceParameters parameters)
		{
			PSCommand psCommand = new PSCommand().AddCommand("Clear-MobileDevice").AddParameter("Cancel", new SwitchParameter(true));
			return base.InvokeAndGetObject<MobileDeviceRow>(psCommand, identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-MobileDevice?Identity@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-MobileDevice?Identity@W:Self")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-MobileDevice", identities, parameters);
		}

		internal const string GetCmdlet = "Get-MobileDeviceStatistics";

		internal const string SetCmdlet = "Clear-MobileDevice";

		internal const string RemoveCmdlet = "Remove-MobileDevice";

		internal const string GetLoggingCmdlet = "Get-CASMailbox";

		internal const string SetLoggingCmdlet = "Set-CASMailbox";

		internal const string GetListRole_Self = "Get-MobileDeviceStatistics?Mailbox&ShowRecoveryPassword&ActiveSync@R:Self+Get-CASMailbox?Identity@R:Self";

		internal const string GetListRole_Org = "Get-MobileDeviceStatistics?Mailbox&ShowRecoveryPassword&ActiveSync@R:Organization+Get-CASMailbox?Identity@R:Organization";

		internal const string GetObjectRole_Self = "Get-MobileDeviceStatistics?Identity@R:Self";

		internal const string GetObjectRole_Org = "Get-MobileDeviceStatistics?Identity@R:Organization";

		internal const string GetObjectForListRole_Self = "Get-MobileDeviceStatistics?Identity&ShowRecoveryPassword@R:Self";

		internal const string GetObjectForListRole_Org = "Get-MobileDeviceStatistics?Identity&ShowRecoveryPassword@R:Organization";

		private const string StartLoggingRole = "Set-CASMailbox?Identity&ActiveSyncDebugLogging@W:Self";

		private const string StopLoggingAndRetrieveRole = "Set-CASMailbox?Identity&ActiveSyncDebugLogging@W:Self+Get-MobileDeviceStatistics?Identity&GetMailboxLog@R:Self";

		internal const string BlockOrWipeDeviceRole_Self = "Clear-MobileDevice?Identity&Cancel@W:Self";

		internal const string BlockOrWipeDeviceRole_Org = "Clear-MobileDevice?Identity&Cancel@W:Organization";

		internal const string RemoveObjectsRole_Self = "Remove-MobileDevice?Identity@W:Self";

		internal const string RemoveObjectsRole_Org = "Remove-MobileDevice?Identity@W:Organization";
	}
}
