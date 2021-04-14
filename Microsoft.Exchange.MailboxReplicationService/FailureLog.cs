using System;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FailureLog : ObjectLog<FailureData>
	{
		private FailureLog() : base(new FailureLog.FailureLogSchema(), new SimpleObjectLogConfiguration("Failure", "FailureLogEnabled", "FailureLogMaxDirSize", "FailureLogMaxFileSize"))
		{
		}

		public static string GetDataContextToPersist(string dataContext)
		{
			if (dataContext.Length > 1000)
			{
				dataContext = dataContext.Substring(0, 997) + "...";
			}
			return dataContext;
		}

		public static void Write(Guid requestGuid, Exception failure, bool isFatal, RequestState requestState, SyncStage syncStage, string folderName = null, string operationType = null)
		{
			FailureLog.WriteInternal(requestGuid, failure, isFatal, requestState, syncStage, folderName, operationType, Guid.NewGuid(), 0);
		}

		private static void WriteInternal(Guid requestGuid, Exception failure, bool isFatal, RequestState requestState, SyncStage syncStage, string folderName, string operationType, Guid failureGuid, int failureLevel)
		{
			FailureData objectToLog = default(FailureData);
			objectToLog.FailureGuid = failureGuid;
			objectToLog.RequestGuid = requestGuid;
			objectToLog.Failure = failure;
			objectToLog.FailureLevel = failureLevel;
			objectToLog.IsFatal = isFatal;
			objectToLog.RequestState = requestState;
			objectToLog.SyncStage = syncStage;
			objectToLog.FolderName = folderName;
			objectToLog.OperationType = operationType;
			if (objectToLog.OperationType == null && objectToLog.Failure != null)
			{
				string dataContext = ExecutionContext.GetDataContext(failure);
				objectToLog.OperationType = FailureLog.GetDataContextToPersist(dataContext);
			}
			GenericSettingsContext genericSettingsContext = new GenericSettingsContext("FailureType", CommonUtils.GetFailureType(failure), null);
			using (genericSettingsContext.Activate())
			{
				if (ConfigBase<MRSConfigSchema>.GetConfig<bool>("SendGenericWatson"))
				{
					string watsonHash;
					CommonUtils.SendGenericWatson(failure, CommonUtils.FullFailureMessageWithCallStack(failure, 5), out watsonHash);
					objectToLog.WatsonHash = watsonHash;
				}
				else
				{
					objectToLog.WatsonHash = CommonUtils.ComputeCallStackHash(failure, 5);
				}
			}
			FailureLog.instance.LogObject(objectToLog);
			if (failure.InnerException != null)
			{
				FailureLog.WriteInternal(requestGuid, failure.InnerException, isFatal, requestState, syncStage, folderName, operationType, failureGuid, failureLevel + 1);
			}
		}

		public const int MaxDataContextLength = 1000;

		private static FailureLog instance = new FailureLog();

		private class FailureLogSchema : ObjectLogSchema
		{
			public override string Software
			{
				get
				{
					return "Microsoft Exchange Mailbox Replication Service";
				}
			}

			public override string LogType
			{
				get
				{
					return "Failure Log";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> RequestGuid = new ObjectLogSimplePropertyDefinition<FailureData>("RequestGuid", (FailureData d) => d.RequestGuid);

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> IsFatal = new ObjectLogSimplePropertyDefinition<FailureData>("IsFatal", (FailureData d) => d.IsFatal.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> RequestState = new ObjectLogSimplePropertyDefinition<FailureData>("RequestState", (FailureData d) => d.RequestState.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> SyncStage = new ObjectLogSimplePropertyDefinition<FailureData>("SyncStage", (FailureData d) => d.SyncStage.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> FailureSide = new ObjectLogSimplePropertyDefinition<FailureData>("FailureSide", (FailureData d) => (CommonUtils.GetExceptionSide(d.Failure) ?? ExceptionSide.None).ToString());

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> FailureType = new ObjectLogSimplePropertyDefinition<FailureData>("FailureType", (FailureData d) => CommonUtils.GetFailureType(d.Failure));

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> FailureCode = new ObjectLogSimplePropertyDefinition<FailureData>("FailureCode", (FailureData d) => CommonUtils.HrFromException(d.Failure));

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> MapiLowLevelError = new ObjectLogSimplePropertyDefinition<FailureData>("MapiLowLevelError", (FailureData d) => CommonUtils.GetMapiLowLevelError(d.Failure));

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> FolderName = new ObjectLogSimplePropertyDefinition<FailureData>("FolderName", (FailureData d) => d.FolderName);

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> OperationType = new ObjectLogSimplePropertyDefinition<FailureData>("OperationType", (FailureData d) => d.OperationType);

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> WatsonHash = new ObjectLogSimplePropertyDefinition<FailureData>("WatsonHash", (FailureData d) => d.WatsonHash);

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> StackTrace = new ObjectLogSimplePropertyDefinition<FailureData>("StackTrace", (FailureData d) => CommonUtils.FullFailureMessageWithCallStack(d.Failure, 5));

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> AppVersion = new ObjectLogSimplePropertyDefinition<FailureData>("AppVersion", (FailureData d) => ExWatson.RealApplicationVersion.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> FailureGuid = new ObjectLogSimplePropertyDefinition<FailureData>("FailureGuid", (FailureData d) => d.FailureGuid);

			public static readonly ObjectLogSimplePropertyDefinition<FailureData> FailureLevel = new ObjectLogSimplePropertyDefinition<FailureData>("FailureLevel", (FailureData d) => d.FailureLevel);
		}
	}
}
