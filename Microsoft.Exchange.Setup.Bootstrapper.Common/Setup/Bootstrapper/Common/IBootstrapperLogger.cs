using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	public interface IBootstrapperLogger
	{
		void StartLogging();

		void StopLogging();

		void Log(LocalizedString localizedString);

		void LogWarning(LocalizedString localizedString);

		void LogError(Exception e);

		void IncreaseIndentation(LocalizedString tag);

		void DecreaseIndentation();
	}
}
