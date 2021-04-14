using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class SetUserCommand : SingleCmdletCommandBase<SetUserRequest, SetUserResponse, SetUser, User>
	{
		public SetUserCommand(CallContext callContext, SetUserRequest request) : base(callContext, request, "Set-User", ScopeLocation.RecipientWrite)
		{
		}

		protected override void PopulateTaskParameters()
		{
			SetUser task = this.cmdletRunner.TaskWrapper.Task;
			SetUserData user = this.request.User;
			this.cmdletRunner.SetTaskParameter("Identity", task, new UserIdParameter(base.CallContext.AccessingADUser.ObjectId));
			User taskParameters = (User)task.GetDynamicParameters();
			this.cmdletRunner.SetTaskParameterIfModified("CountryOrRegion", user, taskParameters, (user.CountryOrRegion == null) ? null : CountryInfo.Parse(user.CountryOrRegion));
			this.cmdletRunner.SetRemainingModifiedTaskParameters(user, taskParameters);
		}

		protected override PSLocalTask<SetUser, User> InvokeCmdletFactory()
		{
			return CmdletTaskFactory.Instance.CreateSetUserTask(base.CallContext.AccessingPrincipal);
		}
	}
}
