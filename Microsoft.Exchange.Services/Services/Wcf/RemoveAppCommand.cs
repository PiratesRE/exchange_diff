using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Extension;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class RemoveAppCommand : SingleCmdletCommandBase<RemoveAppDataRequest, RemoveAppDataResponse, RemoveApp, object>
	{
		public RemoveAppCommand(CallContext callContext, RemoveAppDataRequest request) : base(callContext, request, "Remove-App", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			RemoveApp task = this.cmdletRunner.TaskWrapper.Task;
			this.cmdletRunner.SetTaskParameter("Identity", task, new AppIdParameter(this.request.Identity));
		}

		protected override PSLocalTask<RemoveApp, object> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateRemoveAppTask(base.CallContext.AccessingPrincipal);
		}
	}
}
