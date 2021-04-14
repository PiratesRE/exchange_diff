using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Inference.Common
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DynamicStorageSelection_IInferenceTrainingConfigurationSettings_DataAccessor_ : VariantObjectDataAccessorBase<IInferenceTrainingConfigurationSettings, _DynamicStorageSelection_IInferenceTrainingConfigurationSettings_Implementation_, _DynamicStorageSelection_IInferenceTrainingConfigurationSettings_DataAccessor_>
	{
		internal string _Name_MaterializedValue_;

		internal bool _IsLoggingEnabled_MaterializedValue_;

		internal ValueProvider<bool> _IsLoggingEnabled_ValueProvider_;

		internal string _LogPath_MaterializedValue_;

		internal ValueProvider<string> _LogPath_ValueProvider_;

		internal int _MaxLogAgeInDays_MaterializedValue_;

		internal ValueProvider<int> _MaxLogAgeInDays_ValueProvider_;

		internal ulong _MaxLogDirectorySizeInMB_MaterializedValue_;

		internal ValueProvider<ulong> _MaxLogDirectorySizeInMB_ValueProvider_;

		internal ulong _MaxLogFileSizeInMB_MaterializedValue_;

		internal ValueProvider<ulong> _MaxLogFileSizeInMB_ValueProvider_;

		internal int _MinNumberOfItemsForRetrospectiveTraining_MaterializedValue_;

		internal ValueProvider<int> _MinNumberOfItemsForRetrospectiveTraining_ValueProvider_;

		internal int _MaxNumberOfCyclesForRetrospectiveTraining_MaterializedValue_;

		internal ValueProvider<int> _MaxNumberOfCyclesForRetrospectiveTraining_ValueProvider_;

		internal int _MaxActionHistorySize_MaterializedValue_;

		internal ValueProvider<int> _MaxActionHistorySize_ValueProvider_;

		internal int _TargetPrecisionForThresholdAnalysis_MaterializedValue_;

		internal ValueProvider<int> _TargetPrecisionForThresholdAnalysis_ValueProvider_;

		internal int _TargetRecallForThresholdAnalysis_MaterializedValue_;

		internal ValueProvider<int> _TargetRecallForThresholdAnalysis_ValueProvider_;

		internal int _TargetFalsePositiveRateForThresholdAnalysis_MaterializedValue_;

		internal ValueProvider<int> _TargetFalsePositiveRateForThresholdAnalysis_ValueProvider_;

		internal int _ConfidenceThresholdForThresholdAnalysis_MaterializedValue_;

		internal ValueProvider<int> _ConfidenceThresholdForThresholdAnalysis_ValueProvider_;

		internal int _CoefficientForUserValueComputation_MaterializedValue_;

		internal ValueProvider<int> _CoefficientForUserValueComputation_ValueProvider_;

		internal int _NumberOfHistoryDaysForThresholdComputation_MaterializedValue_;

		internal ValueProvider<int> _NumberOfHistoryDaysForThresholdComputation_ValueProvider_;

		internal int _ActionShareExponentForScaleFactorComputation_MaterializedValue_;

		internal ValueProvider<int> _ActionShareExponentForScaleFactorComputation_ValueProvider_;

		internal int _MinPrecisionForInvitation_MaterializedValue_;

		internal ValueProvider<int> _MinPrecisionForInvitation_ValueProvider_;

		internal int _MinRecallForInvitation_MaterializedValue_;

		internal ValueProvider<int> _MinRecallForInvitation_ValueProvider_;

		internal int _MaxFalsePositiveRateForInvitation_MaterializedValue_;

		internal ValueProvider<int> _MaxFalsePositiveRateForInvitation_ValueProvider_;

		internal int _ConfidenceThresholdForInvitation_MaterializedValue_;

		internal ValueProvider<int> _ConfidenceThresholdForInvitation_ValueProvider_;

		internal int _MinClutterPerDayForInvitation_MaterializedValue_;

		internal ValueProvider<int> _MinClutterPerDayForInvitation_ValueProvider_;

		internal int _MinNonClutterPerDayForInvitation_MaterializedValue_;

		internal ValueProvider<int> _MinNonClutterPerDayForInvitation_ValueProvider_;

		internal int _NumberOfHistoryDaysForInvitationPerDayAverages_MaterializedValue_;

		internal ValueProvider<int> _NumberOfHistoryDaysForInvitationPerDayAverages_ValueProvider_;

		internal int _MinPrecisionForAutoEnablement_MaterializedValue_;

		internal ValueProvider<int> _MinPrecisionForAutoEnablement_ValueProvider_;

		internal int _MinRecallForAutoEnablement_MaterializedValue_;

		internal ValueProvider<int> _MinRecallForAutoEnablement_ValueProvider_;

		internal int _MaxFalsePositiveRateForAutoEnablement_MaterializedValue_;

		internal ValueProvider<int> _MaxFalsePositiveRateForAutoEnablement_ValueProvider_;

		internal int _ConfidenceThresholdForAutoEnablement_MaterializedValue_;

		internal ValueProvider<int> _ConfidenceThresholdForAutoEnablement_ValueProvider_;

		internal int _MinClutterPerDayForAutoEnablement_MaterializedValue_;

		internal ValueProvider<int> _MinClutterPerDayForAutoEnablement_ValueProvider_;

		internal int _MinNonClutterPerDayForAutoEnablement_MaterializedValue_;

		internal ValueProvider<int> _MinNonClutterPerDayForAutoEnablement_ValueProvider_;

		internal int _NumberOfHistoryDaysForAutoEnablementPerDayAverages_MaterializedValue_;

		internal ValueProvider<int> _NumberOfHistoryDaysForAutoEnablementPerDayAverages_ValueProvider_;

		internal bool _IsModelHistoryEnabled_MaterializedValue_;

		internal ValueProvider<bool> _IsModelHistoryEnabled_ValueProvider_;

		internal int _NumberOfModelHistoryCopiesToKeep_MaterializedValue_;

		internal ValueProvider<int> _NumberOfModelHistoryCopiesToKeep_ValueProvider_;

		internal bool _IsMultiStepTrainingEnabled_MaterializedValue_;

		internal ValueProvider<bool> _IsMultiStepTrainingEnabled_ValueProvider_;

		internal int _VacationDetectionMinActivityCountThreshold_MaterializedValue_;

		internal ValueProvider<int> _VacationDetectionMinActivityCountThreshold_ValueProvider_;

		internal double _VacationDetectionActivityCountNumStandardDeviations_MaterializedValue_;

		internal ValueProvider<double> _VacationDetectionActivityCountNumStandardDeviations_ValueProvider_;

		internal int _VacationDetectionMinimumConsecutiveDays_MaterializedValue_;

		internal ValueProvider<int> _VacationDetectionMinimumConsecutiveDays_ValueProvider_;
	}
}
