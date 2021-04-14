using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public abstract class WorkItemResult : IWorkData
	{
		public WorkItemResult()
		{
			this.ExecutionStartTime = DateTime.MaxValue;
			this.ExecutionEndTime = DateTime.MaxValue;
			this.ResultType = ResultType.Abandoned;
			this.MachineName = Environment.MachineName;
		}

		public WorkItemResult(WorkDefinition definition) : this()
		{
			this.MachineName = Settings.MachineName;
			this.PoisonedCount = definition.PoisonedCount;
			this.ExecutionId = definition.ExecutionId;
			this.ResultName = definition.ConstructWorkItemResultName();
			this.DeploymentId = definition.DeploymentId;
			this.WorkItemId = definition.Id;
			this.ServiceName = definition.ServiceName;
			this.DefinitionName = definition.Name;
		}

		public virtual int ResultId { get; protected internal set; }

		public abstract string ServiceName { get; set; }

		public abstract bool IsNotified { get; set; }

		public abstract string ResultName { get; set; }

		public abstract int WorkItemId { get; internal set; }

		public abstract int DeploymentId { get; internal set; }

		public abstract string MachineName { get; internal set; }

		public abstract string Error { get; set; }

		public abstract string Exception { get; set; }

		public abstract byte RetryCount { get; internal set; }

		public abstract string StateAttribute1 { get; set; }

		public abstract string StateAttribute2 { get; set; }

		public abstract string StateAttribute3 { get; set; }

		public abstract string StateAttribute4 { get; set; }

		public abstract string StateAttribute5 { get; set; }

		public abstract double StateAttribute6 { get; set; }

		public abstract double StateAttribute7 { get; set; }

		public abstract double StateAttribute8 { get; set; }

		public abstract double StateAttribute9 { get; set; }

		public abstract double StateAttribute10 { get; set; }

		public abstract ResultType ResultType { get; set; }

		public abstract int ExecutionId { get; protected set; }

		public abstract DateTime ExecutionStartTime { get; set; }

		public abstract DateTime ExecutionEndTime { get; set; }

		public abstract byte PoisonedCount { get; set; }

		public virtual string InternalStorageKey
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual string ExternalStorageKey
		{
			get
			{
				return this.DefinitionName;
			}
		}

		public virtual string SecondaryExternalStorageKey
		{
			get
			{
				return string.Format("{0}_{1}_{2}_{3}_{4}", new object[]
				{
					this.ExecutionEndTime.Ticks,
					this.WorkItemId,
					this.ResultId,
					Settings.InstanceName,
					Settings.MachineName
				});
			}
		}

		internal abstract int Version { get; set; }

		internal string DefinitionName { get; private set; }

		protected internal TracingContext TraceContext
		{
			get
			{
				return new TracingContext(null)
				{
					LId = this.WorkItemId,
					Id = this.ExecutionId
				};
			}
		}

		public virtual void AssignResultId()
		{
		}

		public virtual void SetCompleted(ResultType resultType)
		{
			if (this.ExecutionStartTime == DateTime.MaxValue)
			{
				this.ExecutionStartTime = DateTime.UtcNow;
			}
			this.ExecutionEndTime = DateTime.UtcNow;
			this.ResultType = resultType;
			this.ResultName = this.TruncateStringProperty(this.ResultName, 1280);
			this.StateAttribute1 = this.TruncateStringProperty(this.StateAttribute1, 1024);
			this.StateAttribute2 = this.TruncateStringProperty(this.StateAttribute2, 1024);
			this.StateAttribute3 = this.TruncateStringProperty(this.StateAttribute3, 1024);
			this.StateAttribute4 = this.TruncateStringProperty(this.StateAttribute4, 1024);
			this.StateAttribute5 = this.TruncateStringProperty(this.StateAttribute5, 1024);
			if (string.IsNullOrEmpty(this.Error) && this.ResultType != ResultType.Succeeded)
			{
				this.Error = this.TruncateStringProperty(CoreResources.WorkItemFailedDefaultError(resultType.ToString()), 4000);
			}
			WTFDiagnostics.TraceDebug<ResultType>(WTFLog.Core, this.TraceContext, "[WorkItemResult.SetResult]: workitem completed with result: {0}", resultType, null, "SetCompleted", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\WorkItemResult.cs", 309);
		}

		public void SetCompleted(ResultType resultType, Exception e)
		{
			if (e is AggregateException)
			{
				e = ((AggregateException)e).Flatten().InnerException;
			}
			this.Exception = e.ToString();
			this.Exception = this.TruncateStringProperty(this.Exception, 4000);
			this.Error = e.Message;
			this.Error = this.TruncateStringProperty(this.Error, 4000);
			this.SetCompleted(resultType);
			WTFDiagnostics.TraceError<string>(WTFLog.Core, this.TraceContext, "[WorkItemResult.SetResult]: workitem completed with exception: {0}", this.Exception, null, "SetCompleted", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\Core\\WorkItemResult.cs", 331);
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", base.ToString(), this.WorkItemId);
		}

		internal string TruncateStringProperty(string property, int maxLength)
		{
			if (!string.IsNullOrEmpty(property) && property.Length > maxLength)
			{
				property = property.Substring(0, maxLength);
			}
			return property;
		}

		internal const int StateAttributeStringColumnSize = 1024;

		private const int ErrorAttributeColumnSize = 4000;

		private const int ResultNameColumnSize = 1280;
	}
}
