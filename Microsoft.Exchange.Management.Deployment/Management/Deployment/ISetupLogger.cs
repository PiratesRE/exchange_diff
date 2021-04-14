using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Deployment
{
	public interface ISetupLogger
	{
		bool IsPrereqLogging { get; set; }

		void StartLogging();

		void StopLogging();

		void Log(LocalizedString localizedString);

		void LogWarning(LocalizedString localizedString);

		void LogError(Exception e);

		void TraceEnter(params object[] arguments);

		void TraceExit();

		void IncreaseIndentation(LocalizedString tag);

		void DecreaseIndentation();

		void LogForDataMining(string task, DateTime startTime);
	}
}
