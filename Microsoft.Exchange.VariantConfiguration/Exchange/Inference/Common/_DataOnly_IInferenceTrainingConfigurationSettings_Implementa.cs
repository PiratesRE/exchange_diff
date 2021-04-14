using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Inference.Common
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	internal sealed class _DataOnly_IInferenceTrainingConfigurationSettings_Implementation_ : IInferenceTrainingConfigurationSettings, ISettings, IVariantObjectInstance, IVariantObjectInstanceProvider
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return null;
			}
		}

		IVariantObjectInstance IVariantObjectInstanceProvider.GetVariantObjectInstance(VariantContextSnapshot context)
		{
			return this;
		}

		public string Name
		{
			get
			{
				return this._Name_MaterializedValue_;
			}
		}

		public bool IsLoggingEnabled
		{
			get
			{
				return this._IsLoggingEnabled_MaterializedValue_;
			}
		}

		public string LogPath
		{
			get
			{
				return this._LogPath_MaterializedValue_;
			}
		}

		public int MaxLogAgeInDays
		{
			get
			{
				return this._MaxLogAgeInDays_MaterializedValue_;
			}
		}

		public ulong MaxLogDirectorySizeInMB
		{
			get
			{
				return this._MaxLogDirectorySizeInMB_MaterializedValue_;
			}
		}

		public ulong MaxLogFileSizeInMB
		{
			get
			{
				return this._MaxLogFileSizeInMB_MaterializedValue_;
			}
		}

		public int MinNumberOfItemsForRetrospectiveTraining
		{
			get
			{
				return this._MinNumberOfItemsForRetrospectiveTraining_MaterializedValue_;
			}
		}

		public int MaxNumberOfCyclesForRetrospectiveTraining
		{
			get
			{
				return this._MaxNumberOfCyclesForRetrospectiveTraining_MaterializedValue_;
			}
		}

		public int MaxActionHistorySize
		{
			get
			{
				return this._MaxActionHistorySize_MaterializedValue_;
			}
		}

		public int TargetPrecisionForThresholdAnalysis
		{
			get
			{
				return this._TargetPrecisionForThresholdAnalysis_MaterializedValue_;
			}
		}

		public int TargetRecallForThresholdAnalysis
		{
			get
			{
				return this._TargetRecallForThresholdAnalysis_MaterializedValue_;
			}
		}

		public int TargetFalsePositiveRateForThresholdAnalysis
		{
			get
			{
				return this._TargetFalsePositiveRateForThresholdAnalysis_MaterializedValue_;
			}
		}

		public int ConfidenceThresholdForThresholdAnalysis
		{
			get
			{
				return this._ConfidenceThresholdForThresholdAnalysis_MaterializedValue_;
			}
		}

		public int CoefficientForUserValueComputation
		{
			get
			{
				return this._CoefficientForUserValueComputation_MaterializedValue_;
			}
		}

		public int NumberOfHistoryDaysForThresholdComputation
		{
			get
			{
				return this._NumberOfHistoryDaysForThresholdComputation_MaterializedValue_;
			}
		}

		public int ActionShareExponentForScaleFactorComputation
		{
			get
			{
				return this._ActionShareExponentForScaleFactorComputation_MaterializedValue_;
			}
		}

		public int MinPrecisionForInvitation
		{
			get
			{
				return this._MinPrecisionForInvitation_MaterializedValue_;
			}
		}

		public int MinRecallForInvitation
		{
			get
			{
				return this._MinRecallForInvitation_MaterializedValue_;
			}
		}

		public int MaxFalsePositiveRateForInvitation
		{
			get
			{
				return this._MaxFalsePositiveRateForInvitation_MaterializedValue_;
			}
		}

		public int ConfidenceThresholdForInvitation
		{
			get
			{
				return this._ConfidenceThresholdForInvitation_MaterializedValue_;
			}
		}

		public int MinClutterPerDayForInvitation
		{
			get
			{
				return this._MinClutterPerDayForInvitation_MaterializedValue_;
			}
		}

		public int MinNonClutterPerDayForInvitation
		{
			get
			{
				return this._MinNonClutterPerDayForInvitation_MaterializedValue_;
			}
		}

		public int NumberOfHistoryDaysForInvitationPerDayAverages
		{
			get
			{
				return this._NumberOfHistoryDaysForInvitationPerDayAverages_MaterializedValue_;
			}
		}

		public int MinPrecisionForAutoEnablement
		{
			get
			{
				return this._MinPrecisionForAutoEnablement_MaterializedValue_;
			}
		}

		public int MinRecallForAutoEnablement
		{
			get
			{
				return this._MinRecallForAutoEnablement_MaterializedValue_;
			}
		}

		public int MaxFalsePositiveRateForAutoEnablement
		{
			get
			{
				return this._MaxFalsePositiveRateForAutoEnablement_MaterializedValue_;
			}
		}

		public int ConfidenceThresholdForAutoEnablement
		{
			get
			{
				return this._ConfidenceThresholdForAutoEnablement_MaterializedValue_;
			}
		}

		public int MinClutterPerDayForAutoEnablement
		{
			get
			{
				return this._MinClutterPerDayForAutoEnablement_MaterializedValue_;
			}
		}

		public int MinNonClutterPerDayForAutoEnablement
		{
			get
			{
				return this._MinNonClutterPerDayForAutoEnablement_MaterializedValue_;
			}
		}

		public int NumberOfHistoryDaysForAutoEnablementPerDayAverages
		{
			get
			{
				return this._NumberOfHistoryDaysForAutoEnablementPerDayAverages_MaterializedValue_;
			}
		}

		public bool IsModelHistoryEnabled
		{
			get
			{
				return this._IsModelHistoryEnabled_MaterializedValue_;
			}
		}

		public int NumberOfModelHistoryCopiesToKeep
		{
			get
			{
				return this._NumberOfModelHistoryCopiesToKeep_MaterializedValue_;
			}
		}

		public bool IsMultiStepTrainingEnabled
		{
			get
			{
				return this._IsMultiStepTrainingEnabled_MaterializedValue_;
			}
		}

		public int VacationDetectionMinActivityCountThreshold
		{
			get
			{
				return this._VacationDetectionMinActivityCountThreshold_MaterializedValue_;
			}
		}

		public double VacationDetectionActivityCountNumStandardDeviations
		{
			get
			{
				return this._VacationDetectionActivityCountNumStandardDeviations_MaterializedValue_;
			}
		}

		public int VacationDetectionMinimumConsecutiveDays
		{
			get
			{
				return this._VacationDetectionMinimumConsecutiveDays_MaterializedValue_;
			}
		}

		internal string _Name_MaterializedValue_;

		internal bool _IsLoggingEnabled_MaterializedValue_;

		internal string _LogPath_MaterializedValue_;

		internal int _MaxLogAgeInDays_MaterializedValue_;

		internal ulong _MaxLogDirectorySizeInMB_MaterializedValue_;

		internal ulong _MaxLogFileSizeInMB_MaterializedValue_;

		internal int _MinNumberOfItemsForRetrospectiveTraining_MaterializedValue_;

		internal int _MaxNumberOfCyclesForRetrospectiveTraining_MaterializedValue_;

		internal int _MaxActionHistorySize_MaterializedValue_;

		internal int _TargetPrecisionForThresholdAnalysis_MaterializedValue_;

		internal int _TargetRecallForThresholdAnalysis_MaterializedValue_;

		internal int _TargetFalsePositiveRateForThresholdAnalysis_MaterializedValue_;

		internal int _ConfidenceThresholdForThresholdAnalysis_MaterializedValue_;

		internal int _CoefficientForUserValueComputation_MaterializedValue_;

		internal int _NumberOfHistoryDaysForThresholdComputation_MaterializedValue_;

		internal int _ActionShareExponentForScaleFactorComputation_MaterializedValue_;

		internal int _MinPrecisionForInvitation_MaterializedValue_;

		internal int _MinRecallForInvitation_MaterializedValue_;

		internal int _MaxFalsePositiveRateForInvitation_MaterializedValue_;

		internal int _ConfidenceThresholdForInvitation_MaterializedValue_;

		internal int _MinClutterPerDayForInvitation_MaterializedValue_;

		internal int _MinNonClutterPerDayForInvitation_MaterializedValue_;

		internal int _NumberOfHistoryDaysForInvitationPerDayAverages_MaterializedValue_;

		internal int _MinPrecisionForAutoEnablement_MaterializedValue_;

		internal int _MinRecallForAutoEnablement_MaterializedValue_;

		internal int _MaxFalsePositiveRateForAutoEnablement_MaterializedValue_;

		internal int _ConfidenceThresholdForAutoEnablement_MaterializedValue_;

		internal int _MinClutterPerDayForAutoEnablement_MaterializedValue_;

		internal int _MinNonClutterPerDayForAutoEnablement_MaterializedValue_;

		internal int _NumberOfHistoryDaysForAutoEnablementPerDayAverages_MaterializedValue_;

		internal bool _IsModelHistoryEnabled_MaterializedValue_;

		internal int _NumberOfModelHistoryCopiesToKeep_MaterializedValue_;

		internal bool _IsMultiStepTrainingEnabled_MaterializedValue_;

		internal int _VacationDetectionMinActivityCountThreshold_MaterializedValue_;

		internal double _VacationDetectionActivityCountNumStandardDeviations_MaterializedValue_;

		internal int _VacationDetectionMinimumConsecutiveDays_MaterializedValue_;
	}
}
