using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class GetWithIdentityTaskPiiRedactionModule : GetTaskPiiRedactionModule
	{
		public GetWithIdentityTaskPiiRedactionModule(TaskContext context) : base(context)
		{
		}

		public override void Init(ITaskEvent task)
		{
			base.Init(task);
			if (base.CurrentTaskContext.ExchangeRunspaceConfig != null && base.CurrentTaskContext.ExchangeRunspaceConfig.NeedSuppressingPiiData)
			{
				task.PreIterate += this.Task_PreIterate;
			}
		}

		private void Task_PreIterate(object sender, EventArgs e)
		{
			ADIdParameter adidParameter = base.CurrentTaskContext.InvocationInfo.Fields["Identity"] as ADIdParameter;
			if (adidParameter != null && adidParameter.HasRedactedPiiData)
			{
				if (adidParameter.IsRedactedPiiResolved)
				{
					base.CurrentTaskContext.CommandShell.WriteDebug(new LocalizedString("Redacted PII value in the Identity has been resolved."));
					return;
				}
				if (base.CurrentTaskContext.ExchangeRunspaceConfig != null && base.CurrentTaskContext.ExchangeRunspaceConfig.EnablePiiMap)
				{
					base.CurrentTaskContext.CommandShell.WriteWarning(new LocalizedString("The Identity looks like containing redacted PII value. Unfortunately it failed to resolve the redacted PII value in the Identity."));
				}
			}
		}
	}
}
