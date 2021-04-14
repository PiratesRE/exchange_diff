using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.CertificateAuthentication;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class TaskFaultInjectionModule : ITaskModule, ICriticalFeature
	{
		private protected TaskContext CurrentTaskContext { protected get; private set; }

		private static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (TaskFaultInjectionModule.faultInjectionTracer == null)
				{
					lock (TaskFaultInjectionModule.lockObject)
					{
						if (TaskFaultInjectionModule.faultInjectionTracer == null)
						{
							FaultInjectionTrace faultInjectionTrace = ExTraceGlobals.FaultInjectionTracer;
							faultInjectionTrace.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(TaskFaultInjectionModule.Callback));
							TaskFaultInjectionModule.faultInjectionTracer = faultInjectionTrace;
						}
					}
				}
				return TaskFaultInjectionModule.faultInjectionTracer;
			}
		}

		public TaskFaultInjectionModule(TaskContext context)
		{
			this.CurrentTaskContext = context;
		}

		bool ICriticalFeature.IsCriticalException(Exception ex)
		{
			return typeof(ADTransientException).IsInstanceOfType(ex);
		}

		public virtual void Init(ITaskEvent task)
		{
			task.PreInit += this.InitFaultInjection;
		}

		public void Dispose()
		{
		}

		private static Exception Callback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null)
			{
				if (typeof(ADTransientException).FullName.Equals(exceptionType))
				{
					return new ADTransientException(new LocalizedString("fault injection! ADTransientException"));
				}
				if (typeof(ApplicationException).FullName.Equals(exceptionType))
				{
					return new ApplicationException(new LocalizedString("fault injection!ApplicationException"));
				}
			}
			return result;
		}

		private void InitFaultInjection(object sender, EventArgs e)
		{
			TaskFaultInjectionModule.FaultInjectionTracer.TraceTest(3859164477U);
		}

		private static object lockObject = new object();

		private static FaultInjectionTrace faultInjectionTracer = null;
	}
}
