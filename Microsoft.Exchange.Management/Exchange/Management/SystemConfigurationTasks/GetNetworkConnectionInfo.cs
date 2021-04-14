using System;
using System.Collections.Generic;
using System.Management;
using System.Management.Automation;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "NetworkConnectionInfo", DefaultParameterSetName = "Identity")]
	public sealed class GetNetworkConnectionInfo : GetSystemConfigurationObjectTask<ServerIdParameter, Server>
	{
		protected override void InternalProcessRecord()
		{
			System.Management.ManagementScope scope = null;
			if (this.Identity != null)
			{
				ManagementPath path;
				try
				{
					Server server = (Server)base.GetDataObject<Server>(this.Identity, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Identity.ToString())));
					path = new ManagementPath(string.Format("\\\\{0}\\root\\cimv2", server.Fqdn));
				}
				catch (ManagementObjectNotFoundException)
				{
					path = new ManagementPath(string.Format("\\\\{0}\\root\\cimv2", this.Identity));
				}
				scope = new System.Management.ManagementScope(path);
			}
			try
			{
				IList<NetworkConnectionInfo> connectionInfo = NetworkConnectionInfo.GetConnectionInfo(scope);
				this.WriteResult<NetworkConnectionInfo>(connectionInfo);
			}
			catch (ManagementException ex)
			{
				base.WriteError(new ReadNetworkAdapterInfoException(ex.Message), ErrorCategory.ReadError, null);
			}
			catch (COMException ex2)
			{
				if (ex2.ErrorCode == -2147023174)
				{
					base.WriteError(new ServerNotAvailableException(), ErrorCategory.OpenError, null);
				}
				else
				{
					base.WriteError(ex2, ErrorCategory.ReadError, null);
				}
			}
			catch (UnauthorizedAccessException exception)
			{
				base.WriteError(exception, ErrorCategory.PermissionDenied, null);
			}
		}
	}
}
