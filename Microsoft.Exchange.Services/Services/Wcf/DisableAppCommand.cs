using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Extension;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class DisableAppCommand : SingleCmdletCommandBase<DisableAppDataRequest, DisableAppDataResponse, DisableApp, object>
	{
		public DisableAppCommand(CallContext callContext, DisableAppDataRequest request) : base(callContext, request, "Disable-App", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			DisableApp task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new AppIdParameter(this.request.Identity));
		}

		protected override PSLocalTask<DisableApp, object> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateDisableAppTask(base.CallContext.AccessingPrincipal);
		}
	}
}
