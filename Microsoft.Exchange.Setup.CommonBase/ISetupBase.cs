using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Setup.Parser;

namespace Microsoft.Exchange.Setup.CommonBase
{
	internal interface ISetupBase
	{
		Dictionary<string, object> ParsedArguments { get; }

		CommandLineParser Parser { get; }

		string SourceDir { get; }

		string TargetDir { get; }

		ExitCode HasValidArgs { get; }

		CommandInteractionHandler InteractionHandler { get; set; }

		bool IsExchangeInstalled { get; }

		ISetupLogger Logger { get; }

		void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs eventArgs);

		void ReportException(Exception e);

		void ReportError(string error);

		void ReportMessage(string message);

		void ReportMessage();

		void ReportWarning(string warning);

		void WriteError(string error);

		ExitCode SetupChecks();

		int Run();

		ExitCode ProcessArguments();
	}
}
