using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal static class Engine
	{
		public static void Execute(ITaskContext taskContext, IWorkflow workflow)
		{
			foreach (ITask task in workflow.Tasks)
			{
				LocalizedString localizedString = new LocalizedString(task.Name);
				taskContext.UI.WriteVerbose(localizedString);
				taskContext.UI.WriteProgessIndicator(HybridStrings.HybridActivityConfigure, localizedString, workflow.PercentCompleted);
				Engine.Execute(taskContext, task);
				workflow.UpdateProgress(task);
			}
			taskContext.UI.WriteProgessIndicator(HybridStrings.HybridActivityConfigure, HybridStrings.HybridActivityCompleted, workflow.PercentCompleted);
			taskContext.Logger.LogInformation(HybridStrings.HybridInfoExecutionComplete);
		}

		public static void Execute(ITaskContext taskContext, ITask task)
		{
			Engine.ExecuteSubStep("CheckPrereqs", taskContext, task, (ITaskContext tc, ITask t) => t.CheckPrereqs(tc), (ITaskContext tc, ITask t, Exception e) => new TaskCheckPrereqsException(t.Name, e, tc.Errors), true);
			if (Engine.ExecuteSubStep("NeedsConfiguration", taskContext, task, (ITaskContext tc, ITask t) => t.NeedsConfiguration(tc), (ITaskContext tc, ITask t, Exception e) => new TaskNeedsConfigurationException(t.Name, e, tc.Errors), false))
			{
				Engine.ExecuteSubStep("Configure", taskContext, task, (ITaskContext tc, ITask t) => t.Configure(tc), (ITaskContext tc, ITask t, Exception e) => new TaskConfigureException(t.Name, e, tc.Errors), true);
				Engine.ExecuteSubStep("ValidateConfiguration", taskContext, task, (ITaskContext tc, ITask t) => t.ValidateConfiguration(tc), (ITaskContext tc, ITask t, Exception e) => new TaskValidateConfigurationException(t.Name, e, tc.Errors), true);
			}
		}

		private static bool ExecuteSubStep(string subStepName, ITaskContext taskContext, ITask task, Func<ITaskContext, ITask, bool> substep, Func<ITaskContext, ITask, Exception, Exception> createException, bool throwOnFalse)
		{
			bool flag = false;
			Exception ex = null;
			ExDateTime now = ExDateTime.Now;
			try
			{
				taskContext.Logger.LogInformation(HybridStrings.HybridInfoTaskSubStepStart(task.Name, subStepName));
				flag = substep(taskContext, task);
				if (taskContext.Warnings.Count > 0)
				{
					foreach (LocalizedString localizedString in taskContext.Warnings)
					{
						taskContext.Logger.LogWarning(localizedString);
						taskContext.UI.WriteWarning(localizedString);
					}
					taskContext.Warnings.Clear();
				}
				if (throwOnFalse && !flag)
				{
					ex = createException(taskContext, task, null);
				}
			}
			catch (Exception arg)
			{
				ex = createException(taskContext, task, arg);
			}
			finally
			{
				if (ex != null)
				{
					taskContext.Logger.LogError(ex.ToString());
				}
				double totalMilliseconds = ExDateTime.Now.Subtract(now).TotalMilliseconds;
				taskContext.Logger.LogInformation(HybridStrings.HybridInfoTaskSubStepFinish(task.Name, subStepName, flag, totalMilliseconds));
				taskContext.Logger.LogInformation(new string('-', 128));
				if (ex != null)
				{
					throw ex;
				}
			}
			return flag;
		}

		internal const string SubStepCheckPrereqs = "CheckPrereqs";

		internal const string SubStepNeedsConfiguration = "NeedsConfiguration";

		internal const string SubStepConfigure = "Configure";

		internal const string SubStepValidateConfiguration = "ValidateConfiguration";
	}
}
