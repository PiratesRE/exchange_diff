using System;
using System.Threading;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PrerequisiteAnalysisTaskDataHandler : PrereqBaseTaskDataHandler
	{
		public static PrerequisiteAnalysisTaskDataHandler GetInstance(ISetupContext context, MonadConnection connection)
		{
			return LazyInitializer.EnsureInitialized<PrerequisiteAnalysisTaskDataHandler>(ref PrerequisiteAnalysisTaskDataHandler.instance, () => new PrerequisiteAnalysisTaskDataHandler(context, connection));
		}

		private PrerequisiteAnalysisTaskDataHandler(ISetupContext context, MonadConnection connection) : base("test-SetupPrerequisites", Strings.PrerequisiteAnalysis, null, context, connection)
		{
		}

		private static PrerequisiteAnalysisTaskDataHandler instance;
	}
}
