using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class RemoveMobileDeviceCommand : SingleCmdletCommandBase<RemoveMobileDeviceRequest, RemoveMobileDeviceResponse, RemoveMobileDevice, MobileDevice>
	{
		public RemoveMobileDeviceCommand(CallContext callContext, RemoveMobileDeviceRequest request) : base(callContext, request, "Remove-MobileDevice", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			RemoveMobileDevice task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new MobileDeviceIdParameter(this.request.Identity));
		}

		protected override PSLocalTask<RemoveMobileDevice, MobileDevice> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateRemoveMobileDeviceTask(base.CallContext.AccessingPrincipal);
		}
	}
}
