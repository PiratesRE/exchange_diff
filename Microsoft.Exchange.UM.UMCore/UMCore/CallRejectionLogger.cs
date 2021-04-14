using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CallRejectionLogger : StatisticsLogger
	{
		protected CallRejectionLogger()
		{
		}

		public static CallRejectionLogger Instance
		{
			get
			{
				return CallRejectionLogger.instance;
			}
		}

		public new void Init()
		{
			if (CommonConstants.UseDataCenterLogging)
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					this.Component = currentProcess.MainModule.ModuleName;
				}
				base.Init(AppConfig.Instance.Service.CallRejectionLoggingEnabled, "Logging\\UMCallRejectionLogs");
			}
		}

		protected override StatisticsLogger.StatisticsLogSchema LogSchema
		{
			get
			{
				return this.logSchema;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CallRejectionLogger>(this);
		}

		private const string LogPath = "Logging\\UMCallRejectionLogs";

		private readonly StatisticsLogger.StatisticsLogSchema logSchema = new CallRejectionLogger.CallRejectionLogSchema();

		private static CallRejectionLogger instance = new CallRejectionLogger();

		protected string Component;

		private enum Field
		{
			TimeStamp,
			UMServerName,
			Component,
			ErrorCode,
			ErrorType,
			ErrorCategory,
			ErrorDescription,
			ExtraInfo
		}

		public class CallRejectionLogSchema : StatisticsLogger.StatisticsLogSchema
		{
			public CallRejectionLogSchema() : this("CallRejection")
			{
			}

			protected CallRejectionLogSchema(string logType) : base("1.0", logType, CallRejectionLogger.CallRejectionLogSchema.columns)
			{
			}

			private const string CallRejectionLogType = "CallRejection";

			private const string CallRejectionLogVersion = "1.0";

			private static readonly StatisticsLogger.StatisticsLogColumn[] columns = new StatisticsLogger.StatisticsLogColumn[]
			{
				new StatisticsLogger.StatisticsLogColumn(CallRejectionLogger.Field.TimeStamp.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallRejectionLogger.Field.UMServerName.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallRejectionLogger.Field.Component.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallRejectionLogger.Field.ErrorCode.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallRejectionLogger.Field.ErrorType.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallRejectionLogger.Field.ErrorCategory.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallRejectionLogger.Field.ErrorDescription.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallRejectionLogger.Field.ExtraInfo.ToString(), false)
			};
		}

		public class CallRejectionLogRow : StatisticsLogger.StatisticsLogRow
		{
			public CallRejectionLogRow() : base(CallRejectionLogger.Instance.LogSchema)
			{
			}

			public string UMServerName { get; set; }

			public DateTime TimeStamp { get; set; }

			public int ErrorCode { get; set; }

			public string ErrorType { get; set; }

			public string ErrorCategory { get; set; }

			public string ErrorDescription { get; set; }

			public string ExtraInfo { get; set; }

			public override void PopulateFields()
			{
				base.Fields[0] = this.TimeStamp.ToString("o");
				base.Fields[1] = this.UMServerName;
				base.Fields[2] = CallRejectionLogger.Instance.Component;
				base.Fields[3] = this.ErrorCode.ToString();
				base.Fields[4] = this.ErrorType;
				base.Fields[5] = this.ErrorCategory;
				base.Fields[6] = this.ErrorDescription;
				base.Fields[7] = this.ExtraInfo;
			}
		}
	}
}
