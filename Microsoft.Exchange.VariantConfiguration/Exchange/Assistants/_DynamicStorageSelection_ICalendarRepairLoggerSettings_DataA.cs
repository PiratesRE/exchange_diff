using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Assistants
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_ICalendarRepairLoggerSettings_DataAccessor_ : VariantObjectDataAccessorBase<ICalendarRepairLoggerSettings, _DynamicStorageSelection_ICalendarRepairLoggerSettings_Implementation_, _DynamicStorageSelection_ICalendarRepairLoggerSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _InsightLogEnabled_MaterializedValue_;

		internal ValueProvider<bool> _InsightLogEnabled_ValueProvider_;

		internal string _InsightLogDirectoryName_MaterializedValue_;

		internal ValueProvider<string> _InsightLogDirectoryName_ValueProvider_;

		internal TimeSpan _InsightLogFileAgeInDays_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _InsightLogFileAgeInDays_ValueProvider_;

		internal ulong _InsightLogDirectorySizeLimit_MaterializedValue_;

		internal ValueProvider<ulong> _InsightLogDirectorySizeLimit_ValueProvider_;

		internal ulong _InsightLogFileSize_MaterializedValue_;

		internal ValueProvider<ulong> _InsightLogFileSize_ValueProvider_;

		internal ulong _InsightLogCacheSize_MaterializedValue_;

		internal ValueProvider<ulong> _InsightLogCacheSize_ValueProvider_;

		internal TimeSpan _InsightLogFlushIntervalInSeconds_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _InsightLogFlushIntervalInSeconds_ValueProvider_;
	}
}
