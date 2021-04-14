using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Inference.Common
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface IClutterDataSelectionSettings : ISettings
	{
		int MaxFolderCount { get; }

		int BatchSizeForTrainedModel { get; }

		int BatchSizeForDefaultModel { get; }

		int MaxInboxFolderProportion { get; }

		int MaxDeletedFolderProportion { get; }

		int MaxOtherFolderProportion { get; }

		int MinRespondActionShare { get; }

		int MinIgnoreActionShare { get; }

		int MaxIgnoreActionShare { get; }

		int NumberOfMonthsToIncludeInRetrospectiveTraining { get; }

		int NumberOfDaysToSkipFromCurrentForTraining { get; }
	}
}
