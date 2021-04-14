using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal class ScomAlertLogger : BaseLogger
	{
		public ScomAlertLogger(Action<LocalizedString> logOutputAction = null)
		{
			if (logOutputAction != null)
			{
				base.LogOutput += logOutputAction;
			}
		}

		public override void TaskStarted(ITaskDescriptor task)
		{
			this.taskStack.Push(task);
			this.LogTaskCaption(task);
			this.LogInputProperties(task);
		}

		public override void TaskCompleted(ITaskDescriptor task, TaskResult result)
		{
			this.LogOutputProperties(task);
			if (this.taskStack.Count == 0 || this.taskStack.Peek() != task)
			{
				string message = string.Format("Task structure violated; Expected to complete task \"{0}\", requested to complete task \"{1}\"", (this.taskStack.Count > 0) ? this.taskStack.Peek().TaskTitle : "< empty stack >", task.TaskTitle);
				throw new InvalidOperationException(message);
			}
			if (result == TaskResult.Success)
			{
				this.LogHierarchicalOutput(-1, Strings.ScomAlertLoggerTaskSucceeded(task.TaskTitle));
			}
			else
			{
				this.LogHierarchicalOutput(-1, Strings.ScomAlertLoggerTaskFailed(task.TaskTitle));
			}
			this.taskStack.Pop();
		}

		protected ITaskDescriptor GetCurrentTask()
		{
			return this.taskStack.Peek();
		}

		protected virtual void LogTaskCaption(ITaskDescriptor task)
		{
			this.LogHierarchicalOutput(-1, Strings.ScomAlertLoggerTaskStarted(task.TaskTitle));
			this.LogHierarchicalOutput(Strings.ScomAlertLoggerTaskDescription(task.TaskDescription));
		}

		protected virtual void LogInputProperties(ITaskDescriptor task)
		{
			this.LogProperties(this.GetPropertyFeed(task, ContextProperty.AccessMode.Get), Strings.ScomAlertLoggerTaskStartProperties);
		}

		protected virtual void LogOutputProperties(ITaskDescriptor task)
		{
			this.LogProperties(this.GetPropertyFeed(task, ContextProperty.AccessMode.Set), Strings.ScomAlertLoggerTaskCompletedProperties);
		}

		protected void LogProperties(IEnumerable<KeyValuePair<ContextProperty, string>> feed, LocalizedString caption)
		{
			bool flag = true;
			foreach (KeyValuePair<ContextProperty, string> keyValuePair in feed)
			{
				if (flag)
				{
					this.LogHierarchicalOutput(caption);
					flag = false;
				}
				this.LogHierarchicalOutput(Strings.ScomAlertLoggerTaskProperty(keyValuePair.Key.ToString(), keyValuePair.Value));
			}
		}

		protected virtual IEnumerable<KeyValuePair<ContextProperty, string>> GetPropertyFeed(ITaskDescriptor task, ContextProperty.AccessMode forAccessMode)
		{
			foreach (ContextProperty property in (from prop in task.DependentProperties
			where (prop.AllowedAccessMode & forAccessMode) == forAccessMode
			select prop).Distinct<ContextProperty>())
			{
				object value;
				if (task.Properties.TryGet(property, out value))
				{
					yield return new KeyValuePair<ContextProperty, string>(property, this.StringizePropertyValue(property, value));
				}
			}
			yield break;
		}

		protected string StringizePropertyValue(ContextProperty property, object value)
		{
			if (value == null)
			{
				return Strings.ScomAlertLoggerTaskPropertyNullValue;
			}
			if (value is NetworkCredential)
			{
				NetworkCredential networkCredential = (NetworkCredential)value;
				return Strings.NetworkCredentialString(networkCredential.Domain, networkCredential.UserName);
			}
			if (value is Array)
			{
				return (from object entry in (IEnumerable)value
				select this.StringizePropertyValue(property, entry)).Aggregate(default(LocalizedString), delegate(LocalizedString list, string entry)
				{
					if (!list.IsEmpty)
					{
						return Strings.ListOfItems(list, entry);
					}
					return new LocalizedString(entry);
				});
			}
			return value.ToString();
		}

		protected void LogHierarchicalOutput(LocalizedString message)
		{
			this.LogHierarchicalOutput(0, message);
		}

		protected void LogHierarchicalOutput(int indentShift, LocalizedString message)
		{
			if (this.ShouldLogTask(this.GetCurrentTask()))
			{
				int num = indentShift + this.taskStack.Select(new Func<ITaskDescriptor, bool>(this.ShouldLogTask)).Aggregate(0, delegate(int level, bool shouldLog)
				{
					if (!shouldLog)
					{
						return level;
					}
					return level + 1;
				});
				LocalizedString localizedString = message;
				for (int i = 0; i < num; i++)
				{
					localizedString = Strings.ScomAlertLoggerIndent(localizedString);
				}
				this.OnLogOutput(localizedString);
			}
		}

		protected virtual bool ShouldLogTask(ITaskDescriptor task)
		{
			return true;
		}

		private readonly Stack<ITaskDescriptor> taskStack = new Stack<ITaskDescriptor>();
	}
}
