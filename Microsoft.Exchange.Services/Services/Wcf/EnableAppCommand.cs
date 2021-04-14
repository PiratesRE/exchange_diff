using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Extension;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class EnableAppCommand : SingleCmdletCommandBase<EnableAppDataRequest, EnableAppDataResponse, EnableApp, object>
	{
		public EnableAppCommand(CallContext callContext, EnableAppDataRequest request) : base(callContext, request, "Enable-App", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			EnableApp task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new AppIdParameter(this.request.Identity));
		}

		protected override PSLocalTask<EnableApp, object> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateEnableAppTask(base.CallContext.AccessingPrincipal);
		}
	}
}
