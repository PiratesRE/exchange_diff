using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Assistants
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IMailboxAssistantSettings_DataAccessor_ : VariantObjectDataAccessorBase<IMailboxAssistantSettings, _DynamicStorageSelection_IMailboxAssistantSettings_Implementation_, _DynamicStorageSelection_IMailboxAssistantSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _Enabled_MaterializedValue_;

		internal ValueProvider<bool> _Enabled_ValueProvider_;

		internal TimeSpan _MailboxNotInterestingLogInterval_MaterializedValue_ = default(TimeSpan);

		internal ValueProvider<TimeSpan> _MailboxNotInterestingLogInterval_ValueProvider_;

		internal bool _SpreadLoad_MaterializedValue_;

		internal ValueProvider<bool> _SpreadLoad_ValueProvider_;

		internal bool _SlaMonitoringEnabled_MaterializedValue_;

		internal ValueProvider<bool> _SlaMonitoringEnabled_ValueProvider_;

		internal bool _CompletionMonitoringEnabled_MaterializedValue_;

		internal ValueProvider<bool> _CompletionMonitoringEnabled_ValueProvider_;

		internal bool _ActiveDatabaseProcessingMonitoringEnabled_MaterializedValue_;

		internal ValueProvider<bool> _ActiveDatabaseProcessingMonitoringEnabled_ValueProvider_;

		internal float _SlaUrgentThreshold_MaterializedValue_;

		internal ValueProvider<float> _SlaUrgentThreshold_ValueProvider_;

		internal float _SlaNonUrgentThreshold_MaterializedValue_;

		internal ValueProvider<float> _SlaNonUrgentThreshold_ValueProvider_;
	}
}
