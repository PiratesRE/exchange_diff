using System;
using Microsoft.Exchange.Inference.Common.Diagnostics;

namespace Microsoft.Exchange.Inference.Common
{
	internal interface IGroupingModelTrainingConfiguration
	{
		ILogConfig GroupingModelTrainingStatusLogConfig { get; }

		int MaxNumberOfDaysToQueryForFirstRun { get; }

		double GroupPseudocountPruningThreshold { get; }
	}
}
