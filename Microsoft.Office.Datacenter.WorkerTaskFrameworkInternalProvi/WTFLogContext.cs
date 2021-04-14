using System;
using System.Text;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	internal struct WTFLogContext
	{
		public override string ToString()
		{
			int capacity = this.WorkItemInstance.Length + this.WorkItemType.Length + this.WorkItemDefinition.Length + this.WorkItemCreatedBy.Length + this.WorkItemResult.Length + this.ComponentName.Length + this.ProcessAndThread.Length + this.CallerMethod.Length + this.CallerSourceLine.Length + this.Message.Length + 12;
			StringBuilder stringBuilder = new StringBuilder(capacity);
			stringBuilder.Append(this.WorkItemInstance);
			stringBuilder.Append('/');
			stringBuilder.Append(this.WorkItemType);
			stringBuilder.Append('/');
			stringBuilder.Append(this.WorkItemDefinition);
			stringBuilder.Append('/');
			stringBuilder.Append(this.WorkItemCreatedBy);
			stringBuilder.Append('/');
			stringBuilder.Append(this.WorkItemResult);
			stringBuilder.Append('/');
			stringBuilder.Append(this.ComponentName);
			stringBuilder.Append('/');
			stringBuilder.Append(this.ProcessAndThread);
			stringBuilder.Append('/');
			stringBuilder.Append(this.CallerMethod);
			stringBuilder.Append('/');
			stringBuilder.Append(this.CallerSourceLine);
			stringBuilder.Append('/');
			stringBuilder.Append(this.Message);
			return stringBuilder.ToString();
		}

		public string WorkItemInstance;

		public string WorkItemType;

		public string WorkItemDefinition;

		public string WorkItemCreatedBy;

		public string WorkItemResult;

		public string ComponentName;

		public string ProcessAndThread;

		public string LogLevel;

		public string CallerMethod;

		public string CallerSourceLine;

		public string Message;
	}
}
