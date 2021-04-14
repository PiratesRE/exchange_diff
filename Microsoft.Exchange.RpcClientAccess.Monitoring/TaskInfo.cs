using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal class TaskInfo
	{
		private TaskInfo(Type taskType)
		{
			this.Dependencies = TaskInfo.GetDependencies(taskType);
		}

		public static TaskInfo Get(Type taskType)
		{
			TaskInfo result;
			if (!TaskInfo.taskTypeToInfo.TryGetValue(taskType, out result))
			{
				TaskInfo taskInfo = new TaskInfo(taskType);
				lock (TaskInfo.taskTypeToInfoLock)
				{
					if (!TaskInfo.taskTypeToInfo.TryGetValue(taskType, out result))
					{
						TaskInfo.taskTypeToInfo.Add(taskType, taskInfo);
						result = taskInfo;
					}
				}
			}
			return result;
		}

		private static ICollection<ContextProperty> GetDependencies(Type taskType)
		{
			return (from field in taskType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
			where typeof(ContextProperty).IsAssignableFrom(field.FieldType)
			select (ContextProperty)field.GetValue(null)).ToArray<ContextProperty>();
		}

		public readonly ICollection<ContextProperty> Dependencies;

		private static readonly IDictionary<Type, TaskInfo> taskTypeToInfo = new Dictionary<Type, TaskInfo>();

		private static readonly object taskTypeToInfoLock = new object();
	}
}
