using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class ClearMobileDeviceCommand : SingleCmdletCommandBase<ClearMobileDeviceRequest, ClearMobileDeviceResponse, ClearMobileDevice, MobileDevice>
	{
		public ClearMobileDeviceCommand(CallContext callContext, ClearMobileDeviceRequest request) : base(callContext, request, "Clear-MobileDevice", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			ClearMobileDevice task = this.cmdletRunner.TaskWrapper.Task;
			ClearMobileDeviceOptions options = this.request.Options;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MobileDeviceIdParameter(options.Identity));
			this.cmdletRunner.SetTaskParameterIfModified("Cancel", this.request.Options, task, new SwitchParameter(options.Cancel));
		}

		protected override PSLocalTask<ClearMobileDevice, MobileDevice> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateClearMobileDeviceTask(base.CallContext.AccessingPrincipal);
		}
	}
}
