using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Inference.Common
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IClutterModelConfigurationSettings : ISettings
	{
		int MaxModelVersion { get; }

		int MinModelVersion { get; }

		int NumberOfVersionCrumbsToRecord { get; }

		bool AllowTrainingOnMutipleModelVersions { get; }

		int NumberOfModelVersionToTrain { get; }

		IList<int> BlockedModelVersions { get; }

		IList<int> ClassificationModelVersions { get; }

		IList<int> DeprecatedModelVersions { get; }

		double ProbabilityBehaviourSwitchPerWeek { get; }

		double SymmetricNoise { get; }
	}
}
