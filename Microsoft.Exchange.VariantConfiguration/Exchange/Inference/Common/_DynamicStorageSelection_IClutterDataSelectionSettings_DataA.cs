using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Inference.Common
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IClutterDataSelectionSettings_DataAccessor_ : VariantObjectDataAccessorBase<IClutterDataSelectionSettings, _DynamicStorageSelection_IClutterDataSelectionSettings_Implementation_, _DynamicStorageSelection_IClutterDataSelectionSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal int _MaxFolderCount_MaterializedValue_;

		internal ValueProvider<int> _MaxFolderCount_ValueProvider_;

		internal int _BatchSizeForTrainedModel_MaterializedValue_;

		internal ValueProvider<int> _BatchSizeForTrainedModel_ValueProvider_;

		internal int _BatchSizeForDefaultModel_MaterializedValue_;

		internal ValueProvider<int> _BatchSizeForDefaultModel_ValueProvider_;

		internal int _MaxInboxFolderProportion_MaterializedValue_;

		internal ValueProvider<int> _MaxInboxFolderProportion_ValueProvider_;

		internal int _MaxDeletedFolderProportion_MaterializedValue_;

		internal ValueProvider<int> _MaxDeletedFolderProportion_ValueProvider_;

		internal int _MaxOtherFolderProportion_MaterializedValue_;

		internal ValueProvider<int> _MaxOtherFolderProportion_ValueProvider_;

		internal int _MinRespondActionShare_MaterializedValue_;

		internal ValueProvider<int> _MinRespondActionShare_ValueProvider_;

		internal int _MinIgnoreActionShare_MaterializedValue_;

		internal ValueProvider<int> _MinIgnoreActionShare_ValueProvider_;

		internal int _MaxIgnoreActionShare_MaterializedValue_;

		internal ValueProvider<int> _MaxIgnoreActionShare_ValueProvider_;

		internal int _NumberOfMonthsToIncludeInRetrospectiveTraining_MaterializedValue_;

		internal ValueProvider<int> _NumberOfMonthsToIncludeInRetrospectiveTraining_ValueProvider_;

		internal int _NumberOfDaysToSkipFromCurrentForTraining_MaterializedValue_;

		internal ValueProvider<int> _NumberOfDaysToSkipFromCurrentForTraining_ValueProvider_;
	}
}
