using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IServerModelConfiguration
	{
		int MaxModelVersion { get; }

		int MinModelVersion { get; }

		int NumberOfVersionCrumbsToRecord { get; }

		bool AllowTrainingOnMutipleModelVersions { get; }

		int NumberOfModelVersionToTrain { get; }

		IEnumerable<int> BlockedModelVersions { get; }

		IEnumerable<int> ClassificationModelVersions { get; }

		IEnumerable<int> DeprecatedModelVersions { get; }

		double ProbabilityBehaviourSwitchPerWeek { get; }

		double SymmetricNoise { get; }
	}
}
