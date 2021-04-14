using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class MobileSpeechRequestStatisticsLogger : StatisticsLogger
	{
		protected MobileSpeechRequestStatisticsLogger()
		{
		}

		public static MobileSpeechRequestStatisticsLogger Instance
		{
			get
			{
				return MobileSpeechRequestStatisticsLogger.instance;
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
			return DisposeTracker.Get<MobileSpeechRequestStatisticsLogger>(this);
		}

		private readonly StatisticsLogger.StatisticsLogSchema logSchema = new MobileSpeechRequestStatisticsLogger.MobileSpeechRequestStatisticsLogSchema();

		private static MobileSpeechRequestStatisticsLogger instance = new MobileSpeechRequestStatisticsLogger();

		private enum Field
		{
			RequestId,
			StartTime,
			RequestType,
			RequestStepId,
			RequestLanguage,
			AudioLength,
			UserObjectGuid,
			RecognitionErrorCode,
			RecognitionErrorMessage,
			RecognitionTotalResults,
			RequestTotalElapsedTime,
			TimeZone,
			TenantGuid,
			Tag,
			LogOrigin
		}

		public class MobileSpeechRequestStatisticsLogSchema : StatisticsLogger.StatisticsLogSchema
		{
			public MobileSpeechRequestStatisticsLogSchema() : this("MobileSpeechRequestStatistics")
			{
			}

			protected MobileSpeechRequestStatisticsLogSchema(string logType) : base("1.3", logType, MobileSpeechRequestStatisticsLogger.MobileSpeechRequestStatisticsLogSchema.columns)
			{
			}

			public const string MobileSpeechRequestStatisticsLogType = "MobileSpeechRequestStatistics";

			public const string MobileSpeechRequestStatisticsLogVersion = "1.3";

			private static readonly StatisticsLogger.StatisticsLogColumn[] columns = new StatisticsLogger.StatisticsLogColumn[]
			{
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.RequestId.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.StartTime.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.RequestType.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.RequestStepId.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.RequestLanguage.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.AudioLength.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.UserObjectGuid.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.RecognitionErrorMessage.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.RecognitionErrorCode.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.RecognitionTotalResults.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.RequestTotalElapsedTime.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.TimeZone.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.TenantGuid.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.Tag.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MobileSpeechRequestStatisticsLogger.Field.LogOrigin.ToString(), false)
			};
		}

		public class MobileSpeechRequestStatisticsLogRow : StatisticsLogger.StatisticsLogRow
		{
			public MobileSpeechRequestStatisticsLogRow() : base(MobileSpeechRequestStatisticsLogger.Instance.LogSchema)
			{
			}

			public Guid RequestId { get; set; }

			public ExDateTime StartTime { get; set; }

			public MobileSpeechRecoRequestType? RequestType { get; set; }

			public MobileSpeechRecoRequestStepLogId? RequestStepId { get; set; }

			public string RequestLanguage { get; set; }

			public string RecognitionErrorMessage { get; set; }

			public int RecognitionErrorCode { get; set; }

			public int RecognitionTotalResults { get; set; }

			public TimeSpan RequestTotalElapsedTime { get; set; }

			public string TimeZone { get; set; }

			public int AudioLength { get; set; }

			public Guid? UserObjectGuid { get; set; }

			public Guid? TenantGuid { get; set; }

			public string Tag { get; set; }

			public MobileSpeechRecoLogStatisticOrigin? LogOrigin { get; set; }

			public override void PopulateFields()
			{
				base.Fields[0] = this.RequestId.ToString();
				base.Fields[1] = this.StartTime.ToString("o");
				base.Fields[2] = ((this.RequestType == null) ? string.Empty : this.RequestType.ToString());
				base.Fields[3] = this.RequestStepId.ToString();
				base.Fields[4] = ((this.RequestLanguage == null) ? string.Empty : this.RequestLanguage);
				base.Fields[5] = this.AudioLength.ToString();
				base.Fields[6] = ((this.UserObjectGuid == null) ? string.Empty : this.UserObjectGuid.ToString());
				base.Fields[8] = this.RecognitionErrorMessage;
				base.Fields[7] = this.RecognitionErrorCode.ToString();
				base.Fields[9] = this.RecognitionTotalResults.ToString();
				base.Fields[10] = this.RequestTotalElapsedTime.ToString();
				base.Fields[11] = this.TimeZone;
				base.Fields[12] = ((this.TenantGuid == null) ? string.Empty : this.TenantGuid.ToString());
				base.Fields[13] = this.Tag;
				base.Fields[14] = ((this.LogOrigin == null) ? string.Empty : this.LogOrigin.ToString());
			}
		}
	}
}
