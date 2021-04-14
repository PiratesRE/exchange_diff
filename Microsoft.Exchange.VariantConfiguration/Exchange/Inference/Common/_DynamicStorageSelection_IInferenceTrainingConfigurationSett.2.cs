using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Search.Platform.Parallax.Core.Model;

namespace Microsoft.Exchange.Inference.Common
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class _DynamicStorageSelection_IInferenceTrainingConfigurationSettings_Implementation_ : IInferenceTrainingConfigurationSettings, ISettings, IDataAccessorBackedObject<_DynamicStorageSelection_IInferenceTrainingConfigurationSettings_DataAccessor_>, IVariantObjectInstance
	{
		VariantContextSnapshot IVariantObjectInstance.Context
		{
			get
			{
				return this.context;
			}
		}

		_DynamicStorageSelection_IInferenceTrainingConfigurationSettings_DataAccessor_ IDataAccessorBackedObject<_DynamicStorageSelection_IInferenceTrainingConfigurationSettings_DataAccessor_>.DataAccessor
		{
			get
			{
				return this.dataAccessor;
			}
		}

		void IDataAccessorBackedObject<_DynamicStorageSelection_IInferenceTrainingConfigurationSettings_DataAccessor_>.Initialize(_DynamicStorageSelection_IInferenceTrainingConfigurationSettings_DataAccessor_ dataAccessor, VariantContextSnapshot context)
		{
			this.dataAccessor = dataAccessor;
			this.context = context;
		}

		public string Name
		{
			get
			{
				return this.dataAccessor._Name_MaterializedValue_;
			}
		}

		public bool IsLoggingEnabled
		{
			get
			{
				if (this.dataAccessor._IsLoggingEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._IsLoggingEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._IsLoggingEnabled_MaterializedValue_;
			}
		}

		public string LogPath
		{
			get
			{
				if (this.dataAccessor._LogPath_ValueProvider_ != null)
				{
					return this.dataAccessor._LogPath_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._LogPath_MaterializedValue_;
			}
		}

		public int MaxLogAgeInDays
		{
			get
			{
				if (this.dataAccessor._MaxLogAgeInDays_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxLogAgeInDays_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxLogAgeInDays_MaterializedValue_;
			}
		}

		public ulong MaxLogDirectorySizeInMB
		{
			get
			{
				if (this.dataAccessor._MaxLogDirectorySizeInMB_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxLogDirectorySizeInMB_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxLogDirectorySizeInMB_MaterializedValue_;
			}
		}

		public ulong MaxLogFileSizeInMB
		{
			get
			{
				if (this.dataAccessor._MaxLogFileSizeInMB_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxLogFileSizeInMB_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxLogFileSizeInMB_MaterializedValue_;
			}
		}

		public int MinNumberOfItemsForRetrospectiveTraining
		{
			get
			{
				if (this.dataAccessor._MinNumberOfItemsForRetrospectiveTraining_ValueProvider_ != null)
				{
					return this.dataAccessor._MinNumberOfItemsForRetrospectiveTraining_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinNumberOfItemsForRetrospectiveTraining_MaterializedValue_;
			}
		}

		public int MaxNumberOfCyclesForRetrospectiveTraining
		{
			get
			{
				if (this.dataAccessor._MaxNumberOfCyclesForRetrospectiveTraining_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxNumberOfCyclesForRetrospectiveTraining_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxNumberOfCyclesForRetrospectiveTraining_MaterializedValue_;
			}
		}

		public int MaxActionHistorySize
		{
			get
			{
				if (this.dataAccessor._MaxActionHistorySize_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxActionHistorySize_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxActionHistorySize_MaterializedValue_;
			}
		}

		public int TargetPrecisionForThresholdAnalysis
		{
			get
			{
				if (this.dataAccessor._TargetPrecisionForThresholdAnalysis_ValueProvider_ != null)
				{
					return this.dataAccessor._TargetPrecisionForThresholdAnalysis_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._TargetPrecisionForThresholdAnalysis_MaterializedValue_;
			}
		}

		public int TargetRecallForThresholdAnalysis
		{
			get
			{
				if (this.dataAccessor._TargetRecallForThresholdAnalysis_ValueProvider_ != null)
				{
					return this.dataAccessor._TargetRecallForThresholdAnalysis_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._TargetRecallForThresholdAnalysis_MaterializedValue_;
			}
		}

		public int TargetFalsePositiveRateForThresholdAnalysis
		{
			get
			{
				if (this.dataAccessor._TargetFalsePositiveRateForThresholdAnalysis_ValueProvider_ != null)
				{
					return this.dataAccessor._TargetFalsePositiveRateForThresholdAnalysis_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._TargetFalsePositiveRateForThresholdAnalysis_MaterializedValue_;
			}
		}

		public int ConfidenceThresholdForThresholdAnalysis
		{
			get
			{
				if (this.dataAccessor._ConfidenceThresholdForThresholdAnalysis_ValueProvider_ != null)
				{
					return this.dataAccessor._ConfidenceThresholdForThresholdAnalysis_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ConfidenceThresholdForThresholdAnalysis_MaterializedValue_;
			}
		}

		public int CoefficientForUserValueComputation
		{
			get
			{
				if (this.dataAccessor._CoefficientForUserValueComputation_ValueProvider_ != null)
				{
					return this.dataAccessor._CoefficientForUserValueComputation_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._CoefficientForUserValueComputation_MaterializedValue_;
			}
		}

		public int NumberOfHistoryDaysForThresholdComputation
		{
			get
			{
				if (this.dataAccessor._NumberOfHistoryDaysForThresholdComputation_ValueProvider_ != null)
				{
					return this.dataAccessor._NumberOfHistoryDaysForThresholdComputation_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._NumberOfHistoryDaysForThresholdComputation_MaterializedValue_;
			}
		}

		public int ActionShareExponentForScaleFactorComputation
		{
			get
			{
				if (this.dataAccessor._ActionShareExponentForScaleFactorComputation_ValueProvider_ != null)
				{
					return this.dataAccessor._ActionShareExponentForScaleFactorComputation_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ActionShareExponentForScaleFactorComputation_MaterializedValue_;
			}
		}

		public int MinPrecisionForInvitation
		{
			get
			{
				if (this.dataAccessor._MinPrecisionForInvitation_ValueProvider_ != null)
				{
					return this.dataAccessor._MinPrecisionForInvitation_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinPrecisionForInvitation_MaterializedValue_;
			}
		}

		public int MinRecallForInvitation
		{
			get
			{
				if (this.dataAccessor._MinRecallForInvitation_ValueProvider_ != null)
				{
					return this.dataAccessor._MinRecallForInvitation_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinRecallForInvitation_MaterializedValue_;
			}
		}

		public int MaxFalsePositiveRateForInvitation
		{
			get
			{
				if (this.dataAccessor._MaxFalsePositiveRateForInvitation_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxFalsePositiveRateForInvitation_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxFalsePositiveRateForInvitation_MaterializedValue_;
			}
		}

		public int ConfidenceThresholdForInvitation
		{
			get
			{
				if (this.dataAccessor._ConfidenceThresholdForInvitation_ValueProvider_ != null)
				{
					return this.dataAccessor._ConfidenceThresholdForInvitation_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ConfidenceThresholdForInvitation_MaterializedValue_;
			}
		}

		public int MinClutterPerDayForInvitation
		{
			get
			{
				if (this.dataAccessor._MinClutterPerDayForInvitation_ValueProvider_ != null)
				{
					return this.dataAccessor._MinClutterPerDayForInvitation_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinClutterPerDayForInvitation_MaterializedValue_;
			}
		}

		public int MinNonClutterPerDayForInvitation
		{
			get
			{
				if (this.dataAccessor._MinNonClutterPerDayForInvitation_ValueProvider_ != null)
				{
					return this.dataAccessor._MinNonClutterPerDayForInvitation_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinNonClutterPerDayForInvitation_MaterializedValue_;
			}
		}

		public int NumberOfHistoryDaysForInvitationPerDayAverages
		{
			get
			{
				if (this.dataAccessor._NumberOfHistoryDaysForInvitationPerDayAverages_ValueProvider_ != null)
				{
					return this.dataAccessor._NumberOfHistoryDaysForInvitationPerDayAverages_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._NumberOfHistoryDaysForInvitationPerDayAverages_MaterializedValue_;
			}
		}

		public int MinPrecisionForAutoEnablement
		{
			get
			{
				if (this.dataAccessor._MinPrecisionForAutoEnablement_ValueProvider_ != null)
				{
					return this.dataAccessor._MinPrecisionForAutoEnablement_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinPrecisionForAutoEnablement_MaterializedValue_;
			}
		}

		public int MinRecallForAutoEnablement
		{
			get
			{
				if (this.dataAccessor._MinRecallForAutoEnablement_ValueProvider_ != null)
				{
					return this.dataAccessor._MinRecallForAutoEnablement_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinRecallForAutoEnablement_MaterializedValue_;
			}
		}

		public int MaxFalsePositiveRateForAutoEnablement
		{
			get
			{
				if (this.dataAccessor._MaxFalsePositiveRateForAutoEnablement_ValueProvider_ != null)
				{
					return this.dataAccessor._MaxFalsePositiveRateForAutoEnablement_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MaxFalsePositiveRateForAutoEnablement_MaterializedValue_;
			}
		}

		public int ConfidenceThresholdForAutoEnablement
		{
			get
			{
				if (this.dataAccessor._ConfidenceThresholdForAutoEnablement_ValueProvider_ != null)
				{
					return this.dataAccessor._ConfidenceThresholdForAutoEnablement_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._ConfidenceThresholdForAutoEnablement_MaterializedValue_;
			}
		}

		public int MinClutterPerDayForAutoEnablement
		{
			get
			{
				if (this.dataAccessor._MinClutterPerDayForAutoEnablement_ValueProvider_ != null)
				{
					return this.dataAccessor._MinClutterPerDayForAutoEnablement_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinClutterPerDayForAutoEnablement_MaterializedValue_;
			}
		}

		public int MinNonClutterPerDayForAutoEnablement
		{
			get
			{
				if (this.dataAccessor._MinNonClutterPerDayForAutoEnablement_ValueProvider_ != null)
				{
					return this.dataAccessor._MinNonClutterPerDayForAutoEnablement_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._MinNonClutterPerDayForAutoEnablement_MaterializedValue_;
			}
		}

		public int NumberOfHistoryDaysForAutoEnablementPerDayAverages
		{
			get
			{
				if (this.dataAccessor._NumberOfHistoryDaysForAutoEnablementPerDayAverages_ValueProvider_ != null)
				{
					return this.dataAccessor._NumberOfHistoryDaysForAutoEnablementPerDayAverages_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._NumberOfHistoryDaysForAutoEnablementPerDayAverages_MaterializedValue_;
			}
		}

		public bool IsModelHistoryEnabled
		{
			get
			{
				if (this.dataAccessor._IsModelHistoryEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._IsModelHistoryEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._IsModelHistoryEnabled_MaterializedValue_;
			}
		}

		public int NumberOfModelHistoryCopiesToKeep
		{
			get
			{
				if (this.dataAccessor._NumberOfModelHistoryCopiesToKeep_ValueProvider_ != null)
				{
					return this.dataAccessor._NumberOfModelHistoryCopiesToKeep_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._NumberOfModelHistoryCopiesToKeep_MaterializedValue_;
			}
		}

		public bool IsMultiStepTrainingEnabled
		{
			get
			{
				if (this.dataAccessor._IsMultiStepTrainingEnabled_ValueProvider_ != null)
				{
					return this.dataAccessor._IsMultiStepTrainingEnabled_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._IsMultiStepTrainingEnabled_MaterializedValue_;
			}
		}

		public int VacationDetectionMinActivityCountThreshold
		{
			get
			{
				if (this.dataAccessor._VacationDetectionMinActivityCountThreshold_ValueProvider_ != null)
				{
					return this.dataAccessor._VacationDetectionMinActivityCountThreshold_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._VacationDetectionMinActivityCountThreshold_MaterializedValue_;
			}
		}

		public double VacationDetectionActivityCountNumStandardDeviations
		{
			get
			{
				if (this.dataAccessor._VacationDetectionActivityCountNumStandardDeviations_ValueProvider_ != null)
				{
					return this.dataAccessor._VacationDetectionActivityCountNumStandardDeviations_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._VacationDetectionActivityCountNumStandardDeviations_MaterializedValue_;
			}
		}

		public int VacationDetectionMinimumConsecutiveDays
		{
			get
			{
				if (this.dataAccessor._VacationDetectionMinimumConsecutiveDays_ValueProvider_ != null)
				{
					return this.dataAccessor._VacationDetectionMinimumConsecutiveDays_ValueProvider_.GetValue(this.context);
				}
				return this.dataAccessor._VacationDetectionMinimumConsecutiveDays_MaterializedValue_;
			}
		}

		private _DynamicStorageSelection_IInferenceTrainingConfigurationSettings_DataAccessor_ dataAccessor;

		private VariantContextSnapshot context;
	}
}
