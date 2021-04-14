using System;
using System.Reflection;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Diagnostics.CmdletInfra;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class TaskEvent : ITaskEvent
	{
		public event EventHandler<EventArgs> PreInit;

		public event EventHandler<EventArgs> InitCompleted;

		public event EventHandler<EventArgs> PreIterate;

		public event EventHandler<EventArgs> IterateCompleted;

		public event EventHandler<EventArgs> PreRelease;

		public event EventHandler<EventArgs> Release;

		public event EventHandler<EventArgs> PreStop;

		public event EventHandler<EventArgs> Stop;

		public event EventHandler<GenericEventArg<TaskErrorEventArg>> Error;

		public TaskEvent(TaskContext taskContext)
		{
			this.taskContext = taskContext;
		}

		public void OnPreInit(EventArgs e)
		{
			this.TriggerEvent<EventArgs>(this.PreInit, e, false);
		}

		public void OnInitCompleted(EventArgs e)
		{
			this.TriggerEvent<EventArgs>(this.InitCompleted, e, true);
		}

		public void OnPreIterate(EventArgs e)
		{
			this.TriggerEvent<EventArgs>(this.PreIterate, e, false);
		}

		public void OnIterateCompleted(EventArgs e)
		{
			this.TriggerEvent<EventArgs>(this.IterateCompleted, e, true);
		}

		public void OnPreRelease(EventArgs e)
		{
			this.TriggerEvent<EventArgs>(this.PreRelease, e, false);
		}

		public void OnRelease(EventArgs e)
		{
			this.TriggerEvent<EventArgs>(this.Release, e, true);
		}

		public void OnPreStop(EventArgs e)
		{
			this.TriggerEvent<EventArgs>(this.PreStop, e, false);
		}

		public void OnStop(EventArgs e)
		{
			this.TriggerEvent<EventArgs>(this.Stop, e, true);
		}

		public void OnError(GenericEventArg<TaskErrorEventArg> e)
		{
			this.TriggerEvent<GenericEventArg<TaskErrorEventArg>>(this.Error, e, true);
		}

		private void TriggerEvent<T>(EventHandler<T> eventHandler, T e, bool inReversedOrder) where T : EventArgs
		{
			if (eventHandler != null)
			{
				Delegate[] invocationList = eventHandler.GetInvocationList();
				if (inReversedOrder)
				{
					for (int i = invocationList.Length - 1; i >= 0; i--)
					{
						this.ExecuteEventHandler<T>(e, invocationList[i]);
					}
					return;
				}
				for (int j = 0; j < invocationList.Length; j++)
				{
					this.ExecuteEventHandler<T>(e, invocationList[j]);
				}
			}
		}

		private void ExecuteEventHandler<T>(T e, Delegate handler) where T : EventArgs
		{
			Type declaringType = handler.Method.DeclaringType;
			string str = (declaringType == null) ? "Global" : declaringType.Name;
			string name = handler.Method.Name;
			string text = str + "." + name;
			TaskLogger.Trace(Strings.LogFunctionEnter(declaringType, name, string.Join<ParameterInfo>(",", handler.Method.GetParameters())));
			using (new CmdletMonitoredScope(this.taskContext.UniqueId, "TaskModuleLatency", text, LoggerHelper.CmdletPerfMonitors))
			{
				ICriticalFeature feature = handler.Target as ICriticalFeature;
				feature.Execute(delegate
				{
					((EventHandler<T>)handler)(this, e);
				}, this.taskContext, text);
			}
			TaskLogger.Trace(Strings.LogFunctionExit(declaringType, name));
		}

		private readonly TaskContext taskContext;
	}
}
