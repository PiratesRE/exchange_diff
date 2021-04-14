using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal class MrsSessionStatisticsCsvSchema : BaseMrsMonitorCsvSchema
	{
		public MrsSessionStatisticsCsvSchema() : base(BaseMigMonCsvSchema.GetRequiredColumns(MrsSessionStatisticsCsvSchema.requiredColumnsIds, MrsSessionStatisticsCsvSchema.requiredColumnsAsIs, "Time"), BaseMigMonCsvSchema.GetOptionalColumns(MrsSessionStatisticsCsvSchema.optionalColumnsIds, MrsSessionStatisticsCsvSchema.optionalColumnsAsIs), null)
		{
		}

		public override List<ColumnDefinition<int>> GetRequiredColumnsIds()
		{
			return MrsSessionStatisticsCsvSchema.requiredColumnsIds;
		}

		public override List<IColumnDefinition> GetRequiredColumnsAsIs()
		{
			return MrsSessionStatisticsCsvSchema.requiredColumnsAsIs;
		}

		public override List<ColumnDefinition<int>> GetOptionalColumnsIds()
		{
			return MrsSessionStatisticsCsvSchema.optionalColumnsIds;
		}

		public override List<IColumnDefinition> GetOptionalColumnsAsIs()
		{
			return MrsSessionStatisticsCsvSchema.optionalColumnsAsIs;
		}

		public const string MaxProviderDurationMethodNameColumn = "MaxProviderDurationMethodName";

		private static readonly List<ColumnDefinition<int>> requiredColumnsIds = new List<ColumnDefinition<int>>();

		private static readonly List<IColumnDefinition> requiredColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<Guid>("RequestGuid"),
			new ColumnDefinition<int>("SessionId"),
			new ColumnDefinition<int>("SessionId_Archive")
		};

		private static readonly List<ColumnDefinition<int>> optionalColumnsIds = new List<ColumnDefinition<int>>
		{
			new ColumnDefinition<int>("MaxProviderDurationMethodName", "MaxProviderDurationMethodNameId", KnownStringType.MaxProviderDurationMethodName)
		};

		private static readonly List<IColumnDefinition> optionalColumnsAsIs = new List<IColumnDefinition>
		{
			new ColumnDefinition<double>("MaxProviderDurationInMilliseconds"),
			new ColumnDefinition<double>("CI_TotalTimeInMilliseconds"),
			new ColumnDefinition<int>("SourceLatencyInMillisecondsCurrent"),
			new ColumnDefinition<int>("SourceLatencyInMillisecondsAverage"),
			new ColumnDefinition<int>("SourceLatencyInMillisecondsMin"),
			new ColumnDefinition<int>("SourceLatencyInMillisecondsMax"),
			new ColumnDefinition<int>("SourceLatencyNumberOfSamples"),
			new ColumnDefinition<int>("SourceLatencyTotalNumberOfRemoteCalls"),
			new ColumnDefinition<int>("DestinationLatencyInMillisecondsCurrent"),
			new ColumnDefinition<int>("DestinationLatencyInMillisecondsAverage"),
			new ColumnDefinition<int>("DestinationLatencyInMillisecondsMin"),
			new ColumnDefinition<int>("DestinationLatencyInMillisecondsMax"),
			new ColumnDefinition<int>("DestinationLatencyNumberOfSamples"),
			new ColumnDefinition<int>("DestinationLatencyTotalNumberOfRemoteCalls"),
			new ColumnDefinition<double>("SourceProvider_TotalDurationInMilliseconds"),
			new ColumnDefinition<double>("DestinationProvider_TotalDurationInMilliseconds"),
			new ColumnDefinition<double>("SourceDuration_ISourceMailbox.ExportMessages"),
			new ColumnDefinition<double>("DestinationDuration_IMapiFxProxy.ProcessRequest"),
			new ColumnDefinition<double>("CI_TotalTimeInMilliseconds_Archive"),
			new ColumnDefinition<int>("SourceLatencyInMillisecondsCurrent_Archive"),
			new ColumnDefinition<int>("SourceLatencyInMillisecondsAverage_Archive"),
			new ColumnDefinition<int>("SourceLatencyInMillisecondsMin_Archive"),
			new ColumnDefinition<int>("SourceLatencyInMillisecondsMax_Archive"),
			new ColumnDefinition<int>("SourceLatencyNumberOfSamples_Archive"),
			new ColumnDefinition<int>("SourceLatencyTotalNumberOfRemoteCalls_Archive"),
			new ColumnDefinition<int>("DestinationLatencyInMillisecondsCurrent_Archive"),
			new ColumnDefinition<int>("DestinationLatencyInMillisecondsAverage_Archive"),
			new ColumnDefinition<int>("DestinationLatencyInMillisecondsMin_Archive"),
			new ColumnDefinition<int>("DestinationLatencyInMillisecondsMax_Archive"),
			new ColumnDefinition<int>("DestinationLatencyNumberOfSamples_Archive"),
			new ColumnDefinition<int>("DestinationLatencyTotalNumberOfRemoteCalls_Archive"),
			new ColumnDefinition<double>("SourceProvider_TotalDurationInMilliseconds_Archive"),
			new ColumnDefinition<double>("DestinationProvider_TotalDurationInMilliseconds_Archive"),
			new ColumnDefinition<double>("SourceDuration_ISourceMailbox.ExportMessages_Archive"),
			new ColumnDefinition<double>("DestinationDuration_IMapiFxProxy.ProcessRequest_Archive")
		};
	}
}
