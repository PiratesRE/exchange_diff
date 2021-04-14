using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.SQM;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class TaskModuleFactory : ITaskModuleFactory
	{
		public TaskModuleFactory()
		{
			this.RegisterModules();
		}

		public static void DisableModule(TaskModuleKey key)
		{
			TaskModuleFactory.disabledTaskModules[(int)key] = true;
		}

		public static void EnableModule(TaskModuleKey key)
		{
			TaskModuleFactory.disabledTaskModules[(int)key] = false;
		}

		public IEnumerable<ITaskModule> Create(TaskContext context)
		{
			for (int i = 0; i < this.taskModules.Length; i++)
			{
				if (!TaskModuleFactory.disabledTaskModules[i] && this.taskModules[i] != null)
				{
					yield return (ITaskModule)Activator.CreateInstance(this.taskModules[i], new object[]
					{
						context
					});
				}
			}
			yield break;
		}

		private void RegisterModules()
		{
			this.RegisterModule(TaskModuleKey.Logging, typeof(LoggingModule));
			this.RegisterModule(TaskModuleKey.LatencyTracker, typeof(LatencyTrackingModule));
			this.RegisterModule(TaskModuleKey.Rbac, typeof(RbacModule));
			this.RegisterModule(TaskModuleKey.RunspaceServerSettingsInit, typeof(RunspaceServerSettingsInitModule));
			this.RegisterModule(TaskModuleKey.ReportException, typeof(ReportExceptionModule));
			this.RegisterModule(TaskModuleKey.CmdletHealthCounters, typeof(CmdletHealthCountersModule));
			this.RegisterModule(TaskModuleKey.SetErrorExecutionContext, typeof(SetErrorExecutionContextModule));
			this.RegisterModule(TaskModuleKey.Throttling, typeof(ThrottlingModule<ResourceThrottlingCallback>));
			this.RegisterModule(TaskModuleKey.TaskFaultInjection, typeof(TaskFaultInjectionModule));
			if (CmdletSqmSession.Instance.Enabled)
			{
				this.RegisterModule(TaskModuleKey.Sqm, typeof(SqmModule));
			}
			this.RegisterModule(TaskModuleKey.PiiRedaction, typeof(PiiRedactionModuleBase));
			if (Constants.IsPowerShellWebService)
			{
				this.RegisterModule(TaskModuleKey.PswsPropertyConverter, typeof(PswsPropertyConverterModule));
				this.RegisterModule(TaskModuleKey.PswsErrorHandling, typeof(PswsErrorHandling));
			}
			else
			{
				this.RegisterModule(TaskModuleKey.AutoReportProgress, typeof(AutoReportProgressModule));
			}
			if (TaskLogger.IsSetupLogging)
			{
				this.RegisterModule(TaskModuleKey.SetupLogging, typeof(SetupLoggingModule));
			}
			this.RegisterModule(TaskModuleKey.RunspaceServerSettingsFinalize, typeof(RunspaceServerSettingsFinalizeModule));
			this.RegisterModule(TaskModuleKey.CmdletIterationEvent, typeof(CmdletIterationEventModule));
			this.RegisterModule(TaskModuleKey.CmdletProxy, typeof(ProxyModule));
		}

		protected void RegisterModule(TaskModuleKey key, Type module)
		{
			this.taskModules[(int)key] = module;
		}

		protected void UnregisterModule(TaskModuleKey key)
		{
			this.taskModules[(int)key] = null;
		}

		private static readonly int moduleCount = Enum.GetValues(typeof(TaskModuleKey)).Length;

		private static readonly bool[] disabledTaskModules = new bool[TaskModuleFactory.moduleCount];

		private readonly Type[] taskModules = new Type[TaskModuleFactory.moduleCount];
	}
}
