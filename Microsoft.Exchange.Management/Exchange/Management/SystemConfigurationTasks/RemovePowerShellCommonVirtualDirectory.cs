using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class RemovePowerShellCommonVirtualDirectory<T> : RemoveExchangeVirtualDirectory<T> where T : ADPowerShellCommonVirtualDirectory, new()
	{
		protected abstract PowerShellVirtualDirectoryType AllowedVirtualDirectoryType { get; }

		protected override void InternalValidate()
		{
			base.InternalValidate();
			T dataObject = base.DataObject;
			if (dataObject.VirtualDirectoryType != this.AllowedVirtualDirectoryType)
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound((this.Identity != null) ? this.Identity.ToString() : null, typeof(T).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, null);
			}
		}
	}
}
