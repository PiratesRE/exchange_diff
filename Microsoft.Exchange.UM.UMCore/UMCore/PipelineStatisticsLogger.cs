using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PipelineStatisticsLogger : StatisticsLogger
	{
		protected PipelineStatisticsLogger()
		{
		}

		public static PipelineStatisticsLogger Instance
		{
			get
			{
				return PipelineStatisticsLogger.instance;
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
			return DisposeTracker.Get<PipelineStatisticsLogger>(this);
		}

		private readonly StatisticsLogger.StatisticsLogSchema logSchema = new PipelineStatisticsLogger.PipelineStatisticsLogSchema();

		private static PipelineStatisticsLogger instance = new PipelineStatisticsLogger();

		private enum Field
		{
			SentTime,
			WorkId,
			MessageType,
			TranscriptionLanguage,
			TranscriptionResultType,
			TranscriptionErrorType,
			TranscriptionConfidence,
			TranscriptionTotalWords,
			TranscriptionCustomWords,
			TranscriptionTopNWords,
			TranscriptionElapsedTime,
			AudioCodec,
			AudioCompressionElapsedTime,
			CallerName,
			CalleeAlias,
			OrganizationId
		}

		public class PipelineStatisticsLogSchema : StatisticsLogger.StatisticsLogSchema
		{
			public PipelineStatisticsLogSchema() : this("PipelineStatistics")
			{
			}

			protected PipelineStatisticsLogSchema(string logType) : base("1.0", logType, PipelineStatisticsLogger.PipelineStatisticsLogSchema.columns)
			{
			}

			public const string PipelineStatisticsLogType = "PipelineStatistics";

			public const string PipelineStatisticsLogVersion = "1.0";

			private static readonly StatisticsLogger.StatisticsLogColumn[] columns = new StatisticsLogger.StatisticsLogColumn[]
			{
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.SentTime.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.WorkId.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.MessageType.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.TranscriptionLanguage.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.TranscriptionResultType.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.TranscriptionErrorType.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.TranscriptionConfidence.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.TranscriptionTotalWords.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.TranscriptionCustomWords.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.TranscriptionTopNWords.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.TranscriptionElapsedTime.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.AudioCodec.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.AudioCompressionElapsedTime.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.CallerName.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.CalleeAlias.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(PipelineStatisticsLogger.Field.OrganizationId.ToString(), false)
			};
		}

		public class PipelineStatisticsLogRow : StatisticsLogger.StatisticsLogRow
		{
			public PipelineStatisticsLogRow() : base(PipelineStatisticsLogger.Instance.LogSchema)
			{
				this.TranscriptionResultType = RecoResultType.Skipped;
				this.TranscriptionErrorType = RecoErrorType.Other;
			}

			public DateTime SentTime { get; set; }

			public Guid WorkId { get; set; }

			public string MessageType { get; set; }

			public CultureInfo TranscriptionLanguage { get; set; }

			public RecoResultType TranscriptionResultType { get; set; }

			public RecoErrorType TranscriptionErrorType { get; set; }

			public float TranscriptionConfidence { get; set; }

			public int TranscriptionTotalWords { get; set; }

			public int TranscriptionCustomWords { get; set; }

			public int TranscriptionTopNWords { get; set; }

			public TimeSpan TranscriptionElapsedTime { get; set; }

			public AudioCodecEnum AudioCodec { get; set; }

			public TimeSpan AudioCompressionElapsedTime { get; set; }

			public string CallerName { get; set; }

			public string CalleeAlias { get; set; }

			public string OrganizationId { get; set; }

			public override void PopulateFields()
			{
				base.Fields[0] = this.SentTime.ToString(CultureInfo.InvariantCulture);
				base.Fields[1] = this.WorkId.ToString();
				base.Fields[2] = this.MessageType;
				base.Fields[3] = ((this.TranscriptionLanguage != null) ? this.TranscriptionLanguage.ToString() : string.Empty);
				base.Fields[4] = this.TranscriptionResultType.ToString();
				base.Fields[5] = this.TranscriptionErrorType.ToString();
				base.Fields[6] = this.TranscriptionConfidence.ToString();
				base.Fields[7] = this.TranscriptionTotalWords.ToString();
				base.Fields[8] = this.TranscriptionCustomWords.ToString();
				base.Fields[9] = this.TranscriptionTopNWords.ToString();
				base.Fields[10] = this.TranscriptionElapsedTime.ToString();
				base.Fields[11] = this.AudioCodec.ToString();
				base.Fields[12] = this.AudioCompressionElapsedTime.ToString();
				base.Fields[13] = this.CallerName;
				base.Fields[14] = this.CalleeAlias;
				base.Fields[15] = this.OrganizationId;
			}
		}
	}
}
