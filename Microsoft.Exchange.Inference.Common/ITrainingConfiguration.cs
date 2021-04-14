using System;
using Microsoft.Exchange.Inference.Common.Diagnostics;

namespace Microsoft.Exchange.Inference.Common
{
	internal interface ITrainingConfiguration
	{
		ILogConfig TrainingStatusLogConfig { get; }

		ILogConfig TruthLabelsStatusLogConfig { get; }

		int MinNumberOfItemsForRetrospectiveTraining { get; }

		int MaxNumberOfCyclesForRetrospectiveTraining { get; }

		int MaxActionHistorySize { get; }

		int TargetPrecisionForThresholdAnalysis { get; }

		int TargetRecallForThresholdAnalysis { get; }

		int TargetFalsePositiveRateForThresholdAnalysis { get; }

		int ConfidenceThresholdForThresholdAnalysis { get; }

		int CoefficientForUserValueComputation { get; }

		int NumberOfHistoryDaysForThresholdComputation { get; }

		int ActionShareExponentForScaleFactorComputation { get; }

		int? MinPrecisionForInvitation { get; }

		int? MinRecallForInvitation { get; }

		int? MaxFalsePositiveRateForInvitation { get; }

		int ConfidenceThresholdForInvitation { get; }

		int? MinClutterPerDayForInvitation { get; }

		int? MinNonClutterPerDayForInvitation { get; }

		int NumberOfHistoryDaysForInvitationPerDayAverages { get; }

		int? MinPrecisionForAutoEnablement { get; }

		int? MinRecallForAutoEnablement { get; }

		int? MaxFalsePositiveRateForAutoEnablement { get; }

		int ConfidenceThresholdForAutoEnablement { get; }

		int? MinClutterPerDayForAutoEnablement { get; }

		int? MinNonClutterPerDayForAutoEnablement { get; }

		int NumberOfHistoryDaysForAutoEnablementPerDayAverages { get; }

		bool IsModelHistoryEnabled { get; }

		int NumberOfModelHistoryCopiesToKeep { get; }

		bool IsMultiStepTrainingEnabled { get; }

		int VacationDetectionMinActivityCountThreshold { get; }

		double VacationDetectionActivityCountNumStandardDeviations { get; }

		int VacationDetectionMinimumConsecutiveDays { get; }
	}
}
