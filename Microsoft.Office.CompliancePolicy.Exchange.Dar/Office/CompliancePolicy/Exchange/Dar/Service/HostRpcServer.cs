using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Dar;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Execution;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.LocStrings;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Utility;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Service
{
	internal class HostRpcServer : ExDarHostRpcServer
	{
		public static bool Start()
		{
			if (HostRpcServer.registered == 1)
			{
				return true;
			}
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
			FileSystemAccessRule accessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.Read, AccessControlType.Allow);
			FileSecurity fileSecurity = new FileSecurity();
			fileSecurity.SetOwner(securityIdentifier);
			fileSecurity.SetAccessRule(accessRule);
			bool result;
			try
			{
				RpcServerBase.RegisterServer(typeof(HostRpcServer), fileSecurity, 131209);
				Interlocked.CompareExchange(ref HostRpcServer.registered, 1, 0);
				InstanceManager.Current.Start();
				result = true;
			}
			catch
			{
				Interlocked.CompareExchange(ref HostRpcServer.registered, 0, 1);
				result = false;
			}
			return result;
		}

		public static void Stop()
		{
			int num = Interlocked.CompareExchange(ref HostRpcServer.registered, 0, 1);
			if (num == 1)
			{
				InstanceManager.Current.Stop();
				RpcServerBase.StopServer(ExDarHostRpcServer.RpcIntfHandle);
			}
		}

		public override byte[] SendHostRequest(int version, int type, byte[] inputParameterBytes)
		{
			Guid guid = Guid.NewGuid();
			DarTaskResult darTaskResult;
			try
			{
				try
				{
					HostRpcServer.Log(guid.ToString(), "HandleRpcRequest", "Serving DAR Runtime Request of type: " + ((RpcRequestType)type).ToString(), ResultSeverityLevel.Informational);
					darTaskResult = HostRpcServer.GetSendHostRequestResult(version, (RpcRequestType)type, guid.ToString(), inputParameterBytes);
				}
				catch (AggregateException ex)
				{
					if (ex.InnerExceptions.Count == 1)
					{
						throw ex.InnerException;
					}
					throw;
				}
			}
			catch (ApplicationException ex2)
			{
				HostRpcServer.Log(guid.ToString(), "HandleRpcRequestFailure", ex2.ToString(), ResultSeverityLevel.Warning);
				darTaskResult = new DarTaskResult
				{
					LocalizedError = ex2.Message
				};
			}
			catch (DataSourceOperationException ex3)
			{
				HostRpcServer.Log(guid.ToString(), "HandleRpcRequestFailure", ex3.ToString(), ResultSeverityLevel.Warning);
				darTaskResult = new DarTaskResult
				{
					LocalizedError = ex3.LocalizedString
				};
			}
			catch (Exception ex4)
			{
				HostRpcServer.Log(guid.ToString(), "HandleRpcRequestFailure", ex4.ToString(), ResultSeverityLevel.Error);
				darTaskResult = new DarTaskResult
				{
					LocalizedError = Strings.ErrorDuringDarCall(guid.ToString())
				};
			}
			if (darTaskResult != null)
			{
				try
				{
					return darTaskResult.ToBytes();
				}
				catch (Exception ex5)
				{
					HostRpcServer.Log(guid.ToString(), "HandleRpcRequestResultSerializationFailure", ex5.ToString(), ResultSeverityLevel.Error);
					throw;
				}
			}
			return HostRpcServer.ok;
		}

		private static DarTaskResult GetSendHostRequestResult(int version, RpcRequestType type, string correlationId, byte[] inputParameterBytes)
		{
			switch (type)
			{
			case RpcRequestType.NotifyTaskStoreChange:
			case RpcRequestType.EnsureTenantMonitoring:
				InstanceManager.Current.NotifyTaskStoreChange(HostRpcServer.GetTenantId(inputParameterBytes), correlationId);
				return null;
			case RpcRequestType.GetDarTask:
				return HostRpcServer.GetDarTask(inputParameterBytes, correlationId);
			case RpcRequestType.SetDarTask:
				return HostRpcServer.SetDarTask(inputParameterBytes, correlationId);
			case RpcRequestType.GetDarTaskAggregate:
				return HostRpcServer.GetDarTaskAggregate(inputParameterBytes, correlationId);
			case RpcRequestType.SetDarTaskAggregate:
				return HostRpcServer.SetDarTaskAggregate(inputParameterBytes, correlationId);
			case RpcRequestType.RemoveCompletedDarTasks:
				return HostRpcServer.RemoveCompletedDarTasks(inputParameterBytes, correlationId);
			case RpcRequestType.RemoveDarTaskAggregate:
				return HostRpcServer.RemoveDarTaskAggregate(inputParameterBytes, correlationId);
			case RpcRequestType.GetDarInfo:
				return HostRpcServer.GetDarInfo();
			default:
				throw new InvalidOperationException("Unknown request RpcRequestType");
			}
		}

		private static string GetTenantId(byte[] inputParameterBytes)
		{
			if (inputParameterBytes == null)
			{
				throw new ApplicationException(Strings.TenantMustBeSpecified);
			}
			return Convert.ToBase64String(inputParameterBytes);
		}

		private static DarTaskResult GetDarInfo()
		{
			string localizedInformation = string.Join("\n", Helper.DumpObject(InstanceManager.Current, "DARRuntime", 3).ToArray<string>());
			return new DarTaskResult
			{
				LocalizedInformation = localizedInformation
			};
		}

		private static DarTaskResult GetDarTask(byte[] inputParameterBytes, string correlationId)
		{
			DarTaskParams darTaskParams = DarTaskParamsBase.FromBytes<DarTaskParams>(inputParameterBytes);
			string tenantId = HostRpcServer.GetTenantId(darTaskParams.TenantId);
			HostRpcServer.Log(correlationId, "GetDarTask", Helper.DumpObject(darTaskParams), ResultSeverityLevel.Informational);
			if (darTaskParams.ActiveInRuntime)
			{
				IEnumerable<DarTask> activeTaskList = InstanceManager.Current.GetActiveTaskList(tenantId);
				return new DarTaskResult
				{
					DarTasks = activeTaskList.Select(new Func<DarTask, TaskStoreObject>(HostRpcServer.GetTaskStoreObject)).ToArray<TaskStoreObject>()
				};
			}
			SearchFilter taskFilter = TaskHelper.GetTaskFilter(darTaskParams);
			IEnumerable<TaskStoreObject> source = TenantStore.Find<TaskStoreObject>(tenantId, taskFilter, false, correlationId);
			return new DarTaskResult
			{
				DarTasks = source.ToArray<TaskStoreObject>()
			};
		}

		private static TaskStoreObject GetTaskStoreObject(DarTask task)
		{
			TaskStoreObject taskStoreObject = task.WorkloadContext as TaskStoreObject;
			if (taskStoreObject != null)
			{
				return taskStoreObject;
			}
			taskStoreObject = new TaskStoreObject();
			taskStoreObject.UpdateFromDarTask(task);
			return taskStoreObject;
		}

		private static TaskAggregateStoreObject GetTaskAggregateStoreObject(DarTaskAggregate task)
		{
			TaskAggregateStoreObject taskAggregateStoreObject = task.WorkloadContext as TaskAggregateStoreObject;
			if (taskAggregateStoreObject != null)
			{
				return taskAggregateStoreObject;
			}
			taskAggregateStoreObject = new TaskAggregateStoreObject();
			taskAggregateStoreObject.UpdateFromDarTaskAggregate(task);
			return taskAggregateStoreObject;
		}

		private static DarTaskResult RemoveCompletedDarTasks(byte[] inputParameterBytes, string correlationId)
		{
			DarTaskParams darTaskParams = DarTaskParamsBase.FromBytes<DarTaskParams>(inputParameterBytes);
			DarServiceProvider darServiceProvider = new ExDarServiceProvider();
			HostRpcServer.Log(correlationId, "RemoveCompletedDarTasks", Helper.DumpObject(darTaskParams), ResultSeverityLevel.Informational);
			darServiceProvider.DarTaskQueue.DeleteCompletedTask(darTaskParams.MaxCompletionTime, darTaskParams.TaskType, HostRpcServer.GetTenantId(darTaskParams.TenantId));
			return DarTaskResult.Nothing();
		}

		private static DarTaskResult SetDarTask(byte[] inputParameterBytes, string correlationId)
		{
			TaskStoreObject taskStoreObject = DarTaskResult.ObjectFromBytes<TaskStoreObject>(inputParameterBytes);
			SearchFilter filter = new SearchFilter.IsEqualTo(TaskStoreObjectSchema.Id.StorePropertyDefinition, taskStoreObject.Id);
			string tenantId = HostRpcServer.GetTenantId(taskStoreObject.TenantId);
			DarTask darTask = (from t in TenantStore.Find<TaskStoreObject>(tenantId, filter, false, correlationId)
			select t.ToDarTask(InstanceManager.Current.Provider)).FirstOrDefault<DarTask>();
			DarTaskManager darTaskManager = new DarTaskManager(InstanceManager.Current.Provider);
			if (taskStoreObject.ObjectState == ObjectState.New)
			{
				if (darTask != null)
				{
					throw new DataSourceOperationException(new LocalizedString(Strings.TaskAlreadyExists));
				}
				darTask = InstanceManager.Current.Provider.DarTaskFactory.CreateTask(taskStoreObject.TaskType);
				darTask.Id = taskStoreObject.Id;
				darTask.Priority = taskStoreObject.Priority;
				darTask.TenantId = tenantId;
				darTask.SerializedTaskData = taskStoreObject.SerializedTaskData;
				if (!darTask.RestoreStateFromSerializedData(darTaskManager))
				{
					throw new DataSourceOperationException(new LocalizedString(Strings.TaskCannotBeRestored));
				}
				HostRpcServer.Log(correlationId, "NewDarTask", Helper.DumpObject(darTask), ResultSeverityLevel.Informational);
				darTaskManager.Enqueue(darTask);
			}
			else
			{
				if (darTask == null)
				{
					throw new DataSourceOperationException(new LocalizedString(Strings.TaskNotFound));
				}
				darTask.TaskState = taskStoreObject.TaskState;
				darTask.Priority = taskStoreObject.Priority;
				HostRpcServer.Log(correlationId, "SetDarTask", Helper.DumpObject(darTask), ResultSeverityLevel.Informational);
				darTaskManager.UpdateTaskState(darTask);
			}
			return DarTaskResult.Nothing();
		}

		private static DarTaskResult GetDarTaskAggregate(byte[] inputParameterBytes, string correlationId)
		{
			DarTaskAggregateParams darTaskAggregateParams = DarTaskParamsBase.FromBytes<DarTaskAggregateParams>(inputParameterBytes);
			SearchFilter taskAggregateFilter = TaskHelper.GetTaskAggregateFilter(darTaskAggregateParams);
			HostRpcServer.Log(correlationId, "GetDarTaskAggregate", Helper.DumpObject(darTaskAggregateParams), ResultSeverityLevel.Informational);
			HostRpcServer.GetTenantId(darTaskAggregateParams.TenantId);
			IEnumerable<TaskAggregateStoreObject> source = TenantStore.Find<TaskAggregateStoreObject>(HostRpcServer.GetTenantId(darTaskAggregateParams.TenantId), taskAggregateFilter, false, correlationId);
			return new DarTaskResult
			{
				DarTaskAggregates = source.ToArray<TaskAggregateStoreObject>()
			};
		}

		private static DarTaskResult SetDarTaskAggregate(byte[] inputParameterBytes, string correlationId)
		{
			TaskAggregateStoreObject taskAggregateStoreObject = DarTaskResult.ObjectFromBytes<TaskAggregateStoreObject>(inputParameterBytes);
			string tenantId = HostRpcServer.GetTenantId(taskAggregateStoreObject.ScopeId);
			if (taskAggregateStoreObject.ObjectState == ObjectState.New)
			{
				InstanceManager.Current.TaskAggregates.Remove(tenantId, taskAggregateStoreObject.TaskType, correlationId);
			}
			DarTaskAggregate darTaskAggregate = InstanceManager.Current.TaskAggregates.Get(tenantId, taskAggregateStoreObject.TaskType, correlationId);
			darTaskAggregate.MaxRunningTasks = taskAggregateStoreObject.MaxRunningTasks;
			darTaskAggregate.Enabled = taskAggregateStoreObject.Enabled;
			HostRpcServer.Log(correlationId, "SetDarTaskAggregate", Helper.DumpObject(darTaskAggregate), ResultSeverityLevel.Informational);
			InstanceManager.Current.TaskAggregates.Set(darTaskAggregate, correlationId);
			return DarTaskResult.Nothing();
		}

		private static DarTaskResult RemoveDarTaskAggregate(byte[] inputParameterBytes, string correlationId)
		{
			DarTaskAggregateParams darTaskAggregateParams = DarTaskParamsBase.FromBytes<DarTaskAggregateParams>(inputParameterBytes);
			string tenantId = HostRpcServer.GetTenantId(darTaskAggregateParams.TenantId);
			HostRpcServer.Log(correlationId, "RemoveDarTaskAggregate", "Type: " + darTaskAggregateParams.TaskType + ", tenantId: " + tenantId, ResultSeverityLevel.Informational);
			if (!InstanceManager.Current.TaskAggregates.Remove(tenantId, darTaskAggregateParams.TaskType, correlationId))
			{
				throw new DataSourceOperationException(new LocalizedString(Strings.TaskNotFound));
			}
			return DarTaskResult.Nothing();
		}

		private static void Log(string correlationId, string tag, string message, ResultSeverityLevel severity = ResultSeverityLevel.Informational)
		{
			LogItem.Publish("HostRpcServer", tag, message, correlationId, severity);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static HostRpcServer()
		{
			byte[] array = new byte[1];
			HostRpcServer.ok = array;
		}

		private const string LogComponent = "HostRpcServer";

		private static byte[] ok;

		private static int registered;
	}
}
