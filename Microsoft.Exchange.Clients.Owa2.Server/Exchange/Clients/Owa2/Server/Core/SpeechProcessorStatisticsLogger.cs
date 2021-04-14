using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SpeechProcessorStatisticsLogger : StatisticsLogger
	{
		protected SpeechProcessorStatisticsLogger()
		{
		}

		public static SpeechProcessorStatisticsLogger Instance
		{
			get
			{
				return SpeechProcessorStatisticsLogger.instance;
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
			return DisposeTracker.Get<SpeechProcessorStatisticsLogger>(this);
		}

		private readonly StatisticsLogger.StatisticsLogSchema logSchema = new SpeechProcessorStatisticsLogger.SpeechProcessorStatisticsLogSchema();

		private static SpeechProcessorStatisticsLogger instance = new SpeechProcessorStatisticsLogger();

		private enum Field
		{
			RequestId,
			StartTime,
			ProcessType,
			Culture,
			AudioLength,
			UserObjectGuid,
			TimeZone,
			TenantGuid,
			Tag,
			MobileSpeechRecoResultType,
			ResultText
		}

		public class SpeechProcessorStatisticsLogSchema : StatisticsLogger.StatisticsLogSchema
		{
			public SpeechProcessorStatisticsLogSchema() : this("SpeechProcessorStatistics")
			{
			}

			protected SpeechProcessorStatisticsLogSchema(string logType) : base("1.0", logType, SpeechProcessorStatisticsLogger.SpeechProcessorStatisticsLogSchema.columns)
			{
			}

			public const string SpeechProcessorStatisticsLogType = "SpeechProcessorStatistics";

			public const string SpeechProcessorStatisticsLogVersion = "1.0";

			private static readonly StatisticsLogger.StatisticsLogColumn[] columns = new StatisticsLogger.StatisticsLogColumn[]
			{
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.RequestId.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.StartTime.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.ProcessType.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.Culture.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.AudioLength.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.UserObjectGuid.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.TimeZone.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.TenantGuid.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.Tag.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.MobileSpeechRecoResultType.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(SpeechProcessorStatisticsLogger.Field.ResultText.ToString(), false)
			};
		}

		public class SpeechProcessorStatisticsLogRow : StatisticsLogger.StatisticsLogRow
		{
			public SpeechProcessorStatisticsLogRow() : base(SpeechProcessorStatisticsLogger.Instance.LogSchema)
			{
			}

			public Guid RequestId { get; set; }

			public ExDateTime StartTime { get; set; }

			public SpeechLoggerProcessType? ProcessType { get; set; }

			public CultureInfo Culture { get; set; }

			public MobileSpeechRecoResultType? MobileSpeechRecoResultType { get; set; }

			public string TimeZone { get; set; }

			public int AudioLength { get; set; }

			public Guid? UserObjectGuid { get; set; }

			public Guid? TenantGuid { get; set; }

			public string Tag { get; set; }

			public string ResultText { get; set; }

			public override void PopulateFields()
			{
				base.Fields[0] = this.RequestId.ToString();
				base.Fields[1] = this.StartTime.ToString("o");
				base.Fields[2] = ((this.ProcessType == null) ? string.Empty : this.ProcessType.ToString());
				base.Fields[3] = this.Culture.ToString();
				base.Fields[4] = this.AudioLength.ToString();
				base.Fields[5] = ((this.UserObjectGuid == null) ? string.Empty : this.UserObjectGuid.ToString());
				base.Fields[6] = this.TimeZone;
				base.Fields[7] = ((this.TenantGuid == null) ? string.Empty : this.TenantGuid.ToString());
				base.Fields[8] = this.Tag;
				base.Fields[9] = ((this.MobileSpeechRecoResultType == null) ? string.Empty : this.MobileSpeechRecoResultType.ToString());
				base.Fields[10] = this.ResultText;
			}
		}
	}
}
