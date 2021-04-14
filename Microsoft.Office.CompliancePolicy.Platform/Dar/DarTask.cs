using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Dar
{
	public abstract class DarTask
	{
		protected DarTask()
		{
			this.Id = Guid.NewGuid().ToString();
		}

		public string Id { get; set; }

		public virtual string CorrelationId
		{
			get
			{
				return this.Id;
			}
		}

		public string Name { get; set; }

		public abstract string TaskType { get; }

		public string TenantId { get; set; }

		public virtual DarTaskCategory Category { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Priority { get; set; }

		public TaskSynchronizationOption TaskSynchronizationOption { get; set; }

		public string TaskSynchronizationKey { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DarTaskState PreviousTaskState { get; private set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DarTaskState TaskState
		{
			get
			{
				return this.taskState;
			}
			set
			{
				DarTaskState darTaskState = this.taskState;
				this.taskState = value;
				if (this.taskState != darTaskState)
				{
					DateTime utcNow = DateTime.UtcNow;
					switch (this.taskState)
					{
					case DarTaskState.Running:
						if (this.TaskExecutionStartTime == default(DateTime))
						{
							this.TaskExecutionStartTime = utcNow;
						}
						break;
					case DarTaskState.Completed:
					case DarTaskState.Failed:
					case DarTaskState.Cancelled:
						this.TaskCompletionTime = utcNow;
						break;
					}
					this.PreviousTaskState = darTaskState;
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DateTime TaskQueuedTime { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DateTime MinTaskScheduleTime { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DateTime TaskScheduledTime { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DateTime TaskExecutionStartTime { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DateTime TaskCompletionTime { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DateTime TaskLastExecutionTime { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int TaskRetryTotalCount { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public TimeSpan TaskRetryInterval { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public int TaskRetryCurrentCount { get; set; }

		public string SerializedTaskData { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object WorkloadContext { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void InvokeTask(DarTaskManager darTaskManager)
		{
			DarTaskExecutionResult executionResult = DarTaskExecutionResult.Yielded;
			try
			{
				darTaskManager.ExecutionLog.LogInformation("DarTask", null, this.CorrelationId, string.Format("Invoking task {0}", this), new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTask0.ToString())
				});
				this.TaskState = DarTaskState.Running;
				this.TaskExecutionStartTime = DateTime.UtcNow;
				darTaskManager.UpdateTaskState(this);
				if (this.SerializedTaskData != null && !this.RestoreStateFromSerializedData(darTaskManager))
				{
					darTaskManager.ExecutionLog.LogInformation("DarTask", null, this.CorrelationId, string.Format("Restoring state from serialized data returned false for task {0}", this), new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTask1.ToString())
					});
					executionResult = DarTaskExecutionResult.Failed;
				}
				else
				{
					executionResult = this.Execute(darTaskManager);
				}
			}
			catch (Exception exception)
			{
				executionResult = DarTaskExecutionResult.Failed;
				darTaskManager.ExecutionLog.LogError("DarTask", null, this.CorrelationId, exception, string.Format("Task {0} threw exception.", this), new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTask2.ToString())
				});
			}
			finally
			{
				this.TaskLastExecutionTime = DateTime.UtcNow;
				this.SetTaskState(darTaskManager, executionResult);
				if (this.TaskState != DarTaskState.Cancelled && this.TaskState != DarTaskState.Failed)
				{
					if (this.TaskState != DarTaskState.Completed)
					{
						goto IL_1BE;
					}
				}
				try
				{
					this.CompleteTask(darTaskManager);
				}
				catch (Exception exception2)
				{
					executionResult = DarTaskExecutionResult.Failed;
					darTaskManager.ExecutionLog.LogError("DarTask", null, this.CorrelationId, exception2, string.Format("Task {0} completion code threw exception.", this), new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTask7.ToString())
					});
				}
				IL_1BE:
				this.OnExecuted(this.TaskState);
				this.SaveStateToSerializedData(darTaskManager);
				darTaskManager.UpdateTaskState(this, executionResult);
				darTaskManager.ExecutionLog.LogInformation("DarTask", null, this.CorrelationId, string.Format("Exiting task execution {0}", this), new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTask3.ToString())
				});
			}
		}

		public override bool Equals(object obj)
		{
			DarTask darTask = obj as DarTask;
			return darTask != null && this.Id == darTask.Id;
		}

		public override int GetHashCode()
		{
			if (this.Id == null)
			{
				return 0;
			}
			return this.Id.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", this.TaskType, this.Id);
		}

		public virtual void SaveStateToSerializedData(DarTaskManager darTaskManager)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (PropertyInfo propertyInfo in base.GetType().GetProperties())
			{
				if (propertyInfo.GetCustomAttributes(typeof(SerializableTaskDataAttribute), false).Length > 0)
				{
					object value = propertyInfo.GetValue(this, null);
					if (value != null)
					{
						Type type = value.GetType();
						if (!type.IsValueType || !value.Equals(Activator.CreateInstance(type)))
						{
							dictionary.Add(propertyInfo.Name, value);
						}
					}
				}
			}
			if (dictionary.Count > 0)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					this.GetSerializer().WriteObject(memoryStream, dictionary);
					this.SerializedTaskData = Encoding.UTF8.GetString(memoryStream.ToArray());
				}
			}
		}

		public abstract DarTaskExecutionResult Execute(DarTaskManager darTaskManager);

		public virtual bool RestoreStateFromSerializedData(DarTaskManager darTaskManager)
		{
			bool result;
			try
			{
				if (!string.IsNullOrEmpty(this.SerializedTaskData))
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)this.GetSerializer().ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(this.SerializedTaskData)));
					foreach (KeyValuePair<string, object> keyValuePair in dictionary)
					{
						base.GetType().GetProperty(keyValuePair.Key).SetValue(this, keyValuePair.Value, null);
					}
				}
				result = true;
			}
			catch (Exception exception)
			{
				darTaskManager.ExecutionLog.LogError("DarTask", null, this.CorrelationId, exception, string.Format("Could not restore task state from string \"{0}\"", this.SerializedTaskData), new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTask5.ToString())
				});
				result = false;
			}
			return result;
		}

		public abstract void CompleteTask(DarTaskManager darTaskManager);

		internal bool ShouldContinue(DarTaskManager taskManager)
		{
			string arg;
			if (taskManager.ShouldContinue(this, out arg) == DarTaskExecutionCommand.ContinueExecution)
			{
				return true;
			}
			taskManager.ServiceProvider.ExecutionLog.LogInformation("DarTask", null, this.CorrelationId, string.Format("Stopping processing due to ShouldContinue: {0}", arg), new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTask6.ToString())
			});
			return false;
		}

		protected virtual IEnumerable<Type> GetKnownTypes()
		{
			yield return typeof(List<string>);
			yield break;
		}

		protected virtual void OnExecuted(DarTaskState executionState)
		{
		}

		private DataContractJsonSerializer GetSerializer()
		{
			return new DataContractJsonSerializer(typeof(Dictionary<string, object>), new DataContractJsonSerializerSettings
			{
				UseSimpleDictionaryFormat = true,
				KnownTypes = this.GetKnownTypes().ToArray<Type>()
			});
		}

		private void SetTaskState(DarTaskManager darTaskManager, DarTaskExecutionResult executionResult)
		{
			switch (executionResult)
			{
			case DarTaskExecutionResult.Completed:
				this.TaskState = DarTaskState.Completed;
				return;
			case DarTaskExecutionResult.Yielded:
				this.TaskState = DarTaskState.Ready;
				return;
			case DarTaskExecutionResult.Failed:
				this.TaskState = DarTaskState.Failed;
				return;
			case DarTaskExecutionResult.TransientError:
				this.TaskRetryCurrentCount++;
				if (this.TaskRetryCurrentCount > this.TaskRetryTotalCount)
				{
					darTaskManager.ExecutionLog.LogError("DarTask", null, this.CorrelationId, null, string.Format("{0} exceeded total retry count of {1}", this, this.TaskRetryTotalCount), new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.DarTask4.ToString())
					});
					this.TaskState = DarTaskState.Failed;
					return;
				}
				this.MinTaskScheduleTime = DateTime.UtcNow.Add(this.TaskRetryInterval);
				this.TaskState = DarTaskState.Ready;
				return;
			default:
				return;
			}
		}

		private const string LoggingClientId = "DarTask";

		private DarTaskState taskState;
	}
}
