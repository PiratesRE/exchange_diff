using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class SingleCmdletCommandBase<TRequest, TResponse, TTask, TResult> : OptionServiceCommandBase<TRequest, TResponse> where TResponse : OptionsResponseBase, new() where TTask : Task
	{
		public SingleCmdletCommandBase(CallContext callContext, TRequest request, string cmdletName, ScopeLocation rbacScope) : base(callContext, request)
		{
			this.cmdletName = cmdletName;
			this.rbacScope = rbacScope;
		}

		protected override TResponse CreateTaskAndExecute()
		{
			TResponse result;
			using (PSLocalTask<TTask, TResult> pslocalTask = this.InvokeCmdletFactory())
			{
				this.cmdletRunner = new CmdletRunner<TTask, TResult>(base.CallContext, this.cmdletName, this.rbacScope, pslocalTask);
				this.PopulateTaskParameters();
				this.cmdletRunner.Execute();
				TResponse tresponse = Activator.CreateInstance<TResponse>();
				tresponse.WasSuccessful = true;
				TResponse tresponse2 = tresponse;
				this.PopulateResponseData(tresponse2);
				result = tresponse2;
			}
			return result;
		}

		protected abstract PSLocalTask<TTask, TResult> InvokeCmdletFactory();

		protected virtual void PopulateTaskParameters()
		{
		}

		protected virtual void PopulateResponseData(TResponse response)
		{
		}

		private readonly string cmdletName;

		private readonly ScopeLocation rbacScope;

		protected CmdletRunner<TTask, TResult> cmdletRunner;
	}
}
