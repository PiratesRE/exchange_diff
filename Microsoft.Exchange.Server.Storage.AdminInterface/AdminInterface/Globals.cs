using System;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Rpc.AdminRpc;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	public static class Globals
	{
		public static IAdminRpcServer AdminRpcServer { get; private set; }

		public static void Initialize()
		{
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.AdminRpcServer = new AdminRpcServer();
			SimpleQueryTargets.Initialize();
			RopSummaryResolver.Add(OperationType.Admin, (byte operationId) => ((AdminMethod)operationId).ToString());
		}

		public static void DatabaseMounting(Context context, StoreDatabase database)
		{
			SimpleQueryTargets.MountEventHandler(database);
		}

		public static void WriteReferenceData()
		{
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<OperationType>(LoggerManager.TraceGuids.OperationType, (OperationType key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<ClientType>(LoggerManager.TraceGuids.ClientType, (ClientType key) => key != ClientType.MaxValue, (ClientType key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<RopId>(LoggerManager.TraceGuids.RopId, (RopId key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<TaskTypeId>(LoggerManager.TraceGuids.TaskType, (TaskTypeId key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<AdminMethod>(LoggerManager.TraceGuids.AdminMethod, (AdminMethod key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<OperationDetail>(LoggerManager.TraceGuids.OperationDetail, (OperationDetail key) => key == OperationDetail.None, (OperationDetail key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<MapiObjectType>(LoggerManager.TraceGuids.OperationDetail, (MapiObjectType key) => (int)(key + 1000U));
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<AdminRpcServer.AdminDoMaintenanceTask.MaintenanceTask>(LoggerManager.TraceGuids.OperationDetail, (AdminRpcServer.AdminDoMaintenanceTask.MaintenanceTask key) => (int)(key + 2000U));
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<Operation>(LoggerManager.TraceGuids.OperationDetail, (Operation key) => (int)(key + 3000U));
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<AdminRpcServer.AdminExecuteTask50.MaintenanceTask>(LoggerManager.TraceGuids.OperationDetail, (AdminRpcServer.AdminExecuteTask50.MaintenanceTask key) => (int)(key + 4000U));
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<MapiObjectType>(LoggerManager.TraceGuids.OperationDetail, (MapiObjectType key) => true, (MapiObjectType key) => (int)(key + 5000U), (MapiObjectType key) => "Stream." + key.ToString());
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<ErrorCodeValue>(LoggerManager.TraceGuids.ErrorCode, (ErrorCodeValue key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<MailboxStatus>(LoggerManager.TraceGuids.MailboxStatus, (MailboxStatus key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<MailboxInfo.MailboxType>(LoggerManager.TraceGuids.MailboxType, (MailboxInfo.MailboxType key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<MailboxInfo.MailboxTypeDetail>(LoggerManager.TraceGuids.MailboxTypeDetail, (MailboxInfo.MailboxTypeDetail key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<BreadcrumbKind>(LoggerManager.TraceGuids.BreadCrumbKind, (BreadcrumbKind key) => (int)key);
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<ExecutionDiagnostics.OperationSource>(LoggerManager.TraceGuids.OperationSource, (ExecutionDiagnostics.OperationSource key) => (int)key);
			ClientActivityStrings.WriteReferenceData();
		}

		public static void Terminate()
		{
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.AdminRpcServer = null;
		}

		public static void DatabaseMounted(Context context, StoreDatabase database)
		{
		}

		public static void DatabaseDismounting(Context context, StoreDatabase database)
		{
		}

		private static void WriteReferenceData<TEnum>(Guid guid, Func<TEnum, int> convert)
		{
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<TEnum>(guid, (TEnum key) => true, convert);
		}

		private static void WriteReferenceData<TEnum>(Guid guid, Func<TEnum, bool> check, Func<TEnum, int> convert)
		{
			Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceData<TEnum>(guid, check, convert, (TEnum key) => key.ToString());
		}

		private static void WriteReferenceData<TEnum>(Guid guid, Func<TEnum, bool> check, Func<TEnum, int> convert, Func<TEnum, string> label)
		{
			IBinaryLogger logger = LoggerManager.GetLogger(LoggerType.ReferenceData);
			if (logger == null || !logger.IsLoggingEnabled)
			{
				return;
			}
			foreach (object obj in Enum.GetValues(typeof(TEnum)))
			{
				TEnum arg = (TEnum)((object)obj);
				if (check(arg))
				{
					Microsoft.Exchange.Server.Storage.AdminInterface.Globals.WriteReferenceKeyValue(logger, guid, convert(arg), label(arg));
				}
			}
		}

		private static void WriteReferenceKeyValue(IBinaryLogger logger, Guid guid, int key, string value)
		{
			using (TraceBuffer traceBuffer = TraceRecord.Create(guid, true, false, key, value))
			{
				logger.TryWrite(traceBuffer);
			}
		}
	}
}
