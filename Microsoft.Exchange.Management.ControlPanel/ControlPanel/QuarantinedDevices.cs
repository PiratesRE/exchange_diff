using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class QuarantinedDevices : DataSourceService, IQuarantinedDevices, IGetListService<QuarantinedDeviceFilter, QuarantinedDevice>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileDevice?ResultSize&Filter&ActiveSync@R:Organization")]
		public PowerShellResults<QuarantinedDevice> GetList(QuarantinedDeviceFilter filter, SortOptions sort)
		{
			return base.GetList<QuarantinedDevice, QuarantinedDeviceFilter>("Get-MobileDevice", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileDevice?Identity@R:Organization+Get-CASMailbox?Identity@R:Organization+Set-CASMailbox?Identity&ActiveSyncAllowedDeviceIDs@W:Organization")]
		public PowerShellResults AllowDevice(Identity[] identities, BaseWebServiceParameters parameters)
		{
			PowerShellResults powerShellResults = new PowerShellResults();
			foreach (Identity identity in identities)
			{
				PowerShellResults<MobileDevice> @object = base.GetObject<MobileDevice>("Get-MobileDevice", identity);
				powerShellResults.MergeErrors<MobileDevice>(@object);
				if (@object.HasValue)
				{
					MobileDevice value = @object.Value;
					Identity identity2 = value.Id.Parent.Parent.ToIdentity();
					PowerShellResults<CASMailbox> object2 = base.GetObject<CASMailbox>("Get-CASMailbox", identity2);
					powerShellResults.MergeErrors<CASMailbox>(object2);
					if (object2.HasValue)
					{
						MultiValuedProperty<string> activeSyncAllowedDeviceIDs = object2.Value.ActiveSyncAllowedDeviceIDs;
						if (!activeSyncAllowedDeviceIDs.Contains(value.DeviceId))
						{
							activeSyncAllowedDeviceIDs.Add(value.DeviceId);
							PSCommand psCommand = new PSCommand().AddCommand("Set-CASMailbox").AddParameter("Identity", identity2).AddParameter("ActiveSyncAllowedDeviceIDs", activeSyncAllowedDeviceIDs);
							powerShellResults.MergeErrors(base.Invoke(psCommand));
						}
					}
				}
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MobileDevice?Identity@R:Organization+Get-CASMailbox?Identity@R:Organization+Set-CASMailbox?Identity&ActiveSyncBlockedDeviceIDs@W:Organization")]
		public PowerShellResults BlockDevice(Identity[] identities, BaseWebServiceParameters parameters)
		{
			PowerShellResults powerShellResults = new PowerShellResults();
			foreach (Identity identity in identities)
			{
				PowerShellResults<MobileDevice> @object = base.GetObject<MobileDevice>("Get-MobileDevice", identity);
				powerShellResults.MergeErrors<MobileDevice>(@object);
				if (@object.HasValue)
				{
					MobileDevice value = @object.Value;
					Identity identity2 = value.Id.Parent.Parent.ToIdentity();
					PowerShellResults<CASMailbox> object2 = base.GetObject<CASMailbox>("Get-CASMailbox", identity2);
					powerShellResults.MergeErrors<CASMailbox>(object2);
					if (object2.HasValue)
					{
						MultiValuedProperty<string> activeSyncBlockedDeviceIDs = object2.Value.ActiveSyncBlockedDeviceIDs;
						if (!activeSyncBlockedDeviceIDs.Contains(value.DeviceId))
						{
							activeSyncBlockedDeviceIDs.Add(value.DeviceId);
							PSCommand psCommand = new PSCommand().AddCommand("Set-CASMailbox").AddParameter("Identity", identity2).AddParameter("ActiveSyncBlockedDeviceIDs", activeSyncBlockedDeviceIDs);
							powerShellResults.MergeErrors(base.Invoke(psCommand));
						}
					}
				}
			}
			return powerShellResults;
		}

		internal const string GetCmdlet = "Get-MobileDevice";

		internal const string GetMailboxCmdlet = "Get-CASMailbox";

		internal const string SetMailboxCmdlet = "Set-CASMailbox";

		internal const string ReadScope = "@R:Organization";

		internal const string WriteScope = "@W:Organization";

		private const string GetListRole = "Get-MobileDevice?ResultSize&Filter&ActiveSync@R:Organization";

		internal const string AllowDeviceRole = "Get-MobileDevice?Identity@R:Organization+Get-CASMailbox?Identity@R:Organization+Set-CASMailbox?Identity&ActiveSyncAllowedDeviceIDs@W:Organization";

		internal const string BlockDeviceRole = "Get-MobileDevice?Identity@R:Organization+Get-CASMailbox?Identity@R:Organization+Set-CASMailbox?Identity&ActiveSyncBlockedDeviceIDs@W:Organization";
	}
}
