using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Inference.Common
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IInferenceTrainingConfigurationSettings : ISettings
	{
		bool IsLoggingEnabled { get; }

		string LogPath { get; }

		int MaxLogAgeInDays { get; }

		ulong MaxLogDirectorySizeInMB { get; }

		ulong MaxLogFileSizeInMB { get; }

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

		int MinPrecisionForInvitation { get; }

		int MinRecallForInvitation { get; }

		int MaxFalsePositiveRateForInvitation { get; }

		int ConfidenceThresholdForInvitation { get; }

		int MinClutterPerDayForInvitation { get; }

		int MinNonClutterPerDayForInvitation { get; }

		int NumberOfHistoryDaysForInvitationPerDayAverages { get; }

		int MinPrecisionForAutoEnablement { get; }

		int MinRecallForAutoEnablement { get; }

		int MaxFalsePositiveRateForAutoEnablement { get; }

		int ConfidenceThresholdForAutoEnablement { get; }

		int MinClutterPerDayForAutoEnablement { get; }

		int MinNonClutterPerDayForAutoEnablement { get; }

		int NumberOfHistoryDaysForAutoEnablementPerDayAverages { get; }

		bool IsModelHistoryEnabled { get; }

		int NumberOfModelHistoryCopiesToKeep { get; }

		bool IsMultiStepTrainingEnabled { get; }

		int VacationDetectionMinActivityCountThreshold { get; }

		double VacationDetectionActivityCountNumStandardDeviations { get; }

		int VacationDetectionMinimumConsecutiveDays { get; }
	}
}
