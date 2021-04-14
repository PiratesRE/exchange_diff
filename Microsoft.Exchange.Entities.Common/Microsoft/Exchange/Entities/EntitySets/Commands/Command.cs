using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.Diagnostics;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	[DataContract]
	public abstract class Command<TResult>
	{
		static Command()
		{
			ActivityContext.RegisterMetadata(typeof(EntitiesMetadata));
		}

		protected Command()
		{
			this.Id = Guid.NewGuid();
			this.OnDeserialized();
		}

		[DataMember]
		public Guid Id { get; private set; }

		protected virtual CommandContext Context { get; set; }

		protected abstract ITracer Trace { get; }

		public TResult Execute(CommandContext context)
		{
			this.Context = context;
			Stopwatch stopwatch = Stopwatch.StartNew();
			TResult result;
			try
			{
				this.onBeforeExecute();
				result = this.OnExecute();
			}
			catch (Exception obj)
			{
				this.SetCustomLoggingData("Exception", obj);
				throw;
			}
			finally
			{
				stopwatch.Stop();
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null)
				{
					currentActivityScope.SetProperty(EntitiesMetadata.CommandName, base.GetType().Name);
					currentActivityScope.SetProperty(EntitiesMetadata.CoreExecutionLatency, stopwatch.ElapsedMilliseconds.ToString());
					string customLoggingData = this.GetCustomLoggingData();
					currentActivityScope.SetProperty(EntitiesMetadata.CustomData, customLoggingData);
				}
			}
			return result;
		}

		protected virtual void SetCustomLoggingData(string key, object obj)
		{
			if (obj != null)
			{
				IPropertyChangeTracker<PropertyDefinition> propertyChangeTracker = obj as IPropertyChangeTracker<PropertyDefinition>;
				try
				{
					string value = (propertyChangeTracker == null) ? obj.ToString() : EntityLogger.GetLoggingDetails(propertyChangeTracker);
					this.SetCustomLoggingData(key, value);
				}
				catch (Exception exception)
				{
					ExWatson.SendReport(exception, ReportOptions.DoNotCollectDumps | ReportOptions.DoNotLogProcessAndThreadIds | ReportOptions.DoNotFreezeThreads, string.Empty);
				}
			}
		}

		protected void RegisterOnBeforeExecute(Action action)
		{
			this.onBeforeExecute = (Action)Delegate.Combine(this.onBeforeExecute, action);
		}

		protected virtual string GetCommandTraceDetails()
		{
			return string.Empty;
		}

		protected virtual void UpdateCustomLoggingData()
		{
			this.SetCustomLoggingData("CommandContext", this.Context);
		}

		protected abstract TResult OnExecute();

		private void TraceExecution()
		{
			if (this.Trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.Trace.TraceDebug<string, string, CommandContext>((long)this.GetHashCode(), "{0}::Execute({1}){2}", base.GetType().Name, this.GetCommandTraceDetails(), this.Context);
			}
		}

		private void OnDeserialized()
		{
			this.RegisterOnBeforeExecute(new Action(this.TraceExecution));
			this.RegisterOnBeforeExecute(new Action(this.LogInputData));
			this.customLogData = new Dictionary<string, string>();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext streamingContext)
		{
			this.OnDeserialized();
		}

		private void LogInputData()
		{
			try
			{
				this.UpdateCustomLoggingData();
			}
			catch (Exception exception)
			{
				ExWatson.SendReport(exception, ReportOptions.DoNotCollectDumps | ReportOptions.DoNotLogProcessAndThreadIds | ReportOptions.DoNotFreezeThreads, string.Empty);
			}
		}

		private void SetCustomLoggingData(string key, string value)
		{
			this.customLogData[key] = value;
		}

		private string GetCustomLoggingData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in this.customLogData)
			{
				stringBuilder.Append(string.Format("[{0}-{1}]", keyValuePair.Key, keyValuePair.Value));
			}
			return stringBuilder.ToString();
		}

		private Action onBeforeExecute;

		private Dictionary<string, string> customLogData;
	}
}
