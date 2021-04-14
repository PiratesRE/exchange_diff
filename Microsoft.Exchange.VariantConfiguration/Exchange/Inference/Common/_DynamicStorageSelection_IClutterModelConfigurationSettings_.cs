using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Inference.Common
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IClutterModelConfigurationSettings_DataAccessor_ : VariantObjectDataAccessorBase<IClutterModelConfigurationSettings, _DynamicStorageSelection_IClutterModelConfigurationSettings_Implementation_, _DynamicStorageSelection_IClutterModelConfigurationSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal int _MaxModelVersion_MaterializedValue_;

		internal ValueProvider<int> _MaxModelVersion_ValueProvider_;

		internal int _MinModelVersion_MaterializedValue_;

		internal ValueProvider<int> _MinModelVersion_ValueProvider_;

		internal int _NumberOfVersionCrumbsToRecord_MaterializedValue_;

		internal ValueProvider<int> _NumberOfVersionCrumbsToRecord_ValueProvider_;

		internal bool _AllowTrainingOnMutipleModelVersions_MaterializedValue_;

		internal ValueProvider<bool> _AllowTrainingOnMutipleModelVersions_ValueProvider_;

		internal int _NumberOfModelVersionToTrain_MaterializedValue_;

		internal ValueProvider<int> _NumberOfModelVersionToTrain_ValueProvider_;

		internal IList<int> _BlockedModelVersions_MaterializedValue_;

		internal ValueProvider<IList<int>> _BlockedModelVersions_ValueProvider_;

		internal IList<int> _ClassificationModelVersions_MaterializedValue_;

		internal ValueProvider<IList<int>> _ClassificationModelVersions_ValueProvider_;

		internal IList<int> _DeprecatedModelVersions_MaterializedValue_;

		internal ValueProvider<IList<int>> _DeprecatedModelVersions_ValueProvider_;

		internal double _ProbabilityBehaviourSwitchPerWeek_MaterializedValue_;

		internal ValueProvider<double> _ProbabilityBehaviourSwitchPerWeek_ValueProvider_;

		internal double _SymmetricNoise_MaterializedValue_;

		internal ValueProvider<double> _SymmetricNoise_ValueProvider_;
	}
}
